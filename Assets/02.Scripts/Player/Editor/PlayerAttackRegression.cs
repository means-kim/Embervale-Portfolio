using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// 실제 Play Mode 객체/콜라이더를 사용하는 회귀 검사. 기존 씬은 저장하지 않는다.
public static class PlayerAttackRegression
{
    private static readonly List<string> Results = new();
    private static readonly List<GameObject> Temporary = new();
    private static Scene _scene;
    private static GameObject _player;
    private static PlayerAttack _attack;
    private static Health _health;
    private static PlayerMove _move;
    private static PlayerController _controller;
    private static PlayerAttackDataSO[] _definitions;

    [MenuItem("Tools/Player/Run Attack Regression (Play Mode)")]
    private static void RunFromMenu() => Debug.Log(Run());

    public static string Run()
    {
        if (!EditorApplication.isPlaying) throw new InvalidOperationException("Play Mode가 필요합니다.");
        Results.Clear();
        var previousScene = SceneManager.GetActiveScene();
        _scene = SceneManager.CreateScene("PlayerAttackRegression", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        SceneManager.SetActiveScene(_scene);
        try
        {
            _player = Spawn("Assets/03.Prefabs/Player/Player.prefab");
            _player.transform.position = new Vector3(10000, 10000);
            _player.GetComponent<Rigidbody2D>().gravityScale = 0;
            _attack = _player.GetComponent<PlayerAttack>();
            _health = _player.GetComponent<Health>();
            _move = _player.GetComponent<PlayerMove>();
            _controller = _player.GetComponent<PlayerController>();
            _definitions = _player.GetComponent<PlayerInput>().AttackBindings.Select(b => b.attack).ToArray();
            Check("Boss 태그 통일 / Enemy 제거", () => Assert(!UnityEditorInternal.InternalEditorUtility.tags.Contains("Enemy")));
            foreach (var definition in _definitions)
            {
                Check(definition.name + " 좌우 판정 반전", () =>
                {
                    var right = AttackObject(definition, 1);
                    var left = AttackObject(definition, -1);
                    Physics2D.SyncTransforms();
                    Assert(right.TryGetHitBounds(out var rb));
                    Assert(left.TryGetHitBounds(out var lb));
                    Assert(Mathf.Abs((rb.center.x - right.transform.position.x) + (lb.center.x - left.transform.position.x)) < 0.001f);
                    Assert(Vector3.Distance(rb.size, lb.size) < 0.001f);
                });
                Check(definition.name + " 시작/취소/잠금", () =>
                {
                    Assert(_attack.TryStartAttack(definition));
                    Assert(_move.MovementLocked && _controller.CurrentState == PlayerState.Attack);
                    var instance = (PlayerAttackInstance)typeof(PlayerAttack).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_attack);
                    _attack.CancelAttack();
                    Assert(!_attack.IsAttacking && !_move.MovementLocked);
                    if (definition.attackType != PlayerAttackType.FireBall) Assert(!instance.GetComponent<PlayerDamageHitbox>().IsActive);
                    else Assert(instance.GetComponent<PlayerDamageHitbox>().IsActive);
                });
                Check(definition.name + " 자연 종료와 판정 시간 분리", () =>
                {
                    Assert(_attack.TryStartAttack(definition));
                    var instance = (PlayerAttackInstance)typeof(PlayerAttack).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_attack);
                    Set(_attack, "_elapsed", definition.windupDuration + definition.recoveryDuration + 0.01f);
                    Invoke(_attack, "Update");
                    Assert(!_attack.IsAttacking && !_move.MovementLocked);
                    if (definition.attackType == PlayerAttackType.FireBall) Assert(instance.GetComponent<PlayerDamageHitbox>().IsActive);
                    Set(instance, "_age", 100f);
                    Invoke(instance, "Update");
                    Assert(!instance.GetComponent<PlayerDamageHitbox>().IsActive);
                });
            }
            Check("준비 구간 취소 시 공격 객체가 생성되지 않음", () =>
            {
                var definition = Object.Instantiate(_definitions[0]);
                try
                {
                    definition.windupDuration = 100;
                    Assert(_attack.TryStartAttack(definition));
                    Assert(typeof(PlayerAttack).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_attack) == null);
                    _attack.CancelAttack();
                    Assert(!_attack.IsAttacking && !_move.MovementLocked);
                }
                finally { Object.DestroyImmediate(definition); }
            });
            Check("실제 설인 Hurtbox 명중 / 한 공격 중복 타격 방지", () =>
            {
                var boss = Spawn("Assets/03.Prefabs/EliteMonster/EliteMonster_SnowGiant.prefab");
                var receiver = boss.GetComponentInChildren<DamageReceiver>();
                var health = receiver.Health;
                int before = health.CurrentHealth;
                var hitbox = AttackObject(_definitions[0], 1).GetComponent<PlayerDamageHitbox>();
                Assert(hitbox.TryHit(receiver.GetComponent<Collider2D>()));
                Assert(!hitbox.TryHit(receiver.GetComponent<Collider2D>()));
                Assert(health.CurrentHealth == before - _definitions[0].damage);
                var weapon = boss.GetComponentsInChildren<Collider2D>(true).First(c => c.GetComponent<EliteMonsterAttackHitbox>() != null);
                Assert(!hitbox.TryHit(weapon));
            });
            Check("화염구도 공통 피해 판정 사용", () =>
            {
                var dummy = Spawn("Assets/03.Prefabs/Dummy/Dummy.prefab");
                var health = dummy.GetComponent<Health>();
                int before = health.CurrentHealth;
                var hitbox = AttackObject(_definitions.First(d => d.attackType == PlayerAttackType.FireBall), -1).GetComponent<PlayerDamageHitbox>();
                Assert(hitbox.TryHit(dummy.GetComponent<Collider2D>()));
                Assert(health.CurrentHealth == before - 10 && !hitbox.IsActive);
            });
            Check("공격 종료가 다른 행동의 잠금을 해제하지 않음", () =>
            {
                Assert(_attack.TryStartAttack(_definitions[0]));
                var source = _player.GetComponent<PlayerWall>();
                _move.SetMovementLocked(source, true);
                _attack.CancelAttack();
                Assert(_move.MovementLocked);
                _move.SetMovementLocked(source, false);
                Assert(!_move.MovementLocked);
            });
            Check("착지 시 생성한 근접 판정도 취소", () =>
            {
                Assert(_attack.TryStartAttack(_definitions[0]));
                var instance = (PlayerAttackInstance)typeof(PlayerAttack).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_attack);
                Invoke(_controller, "HandleLanded");
                Assert(!_attack.IsAttacking && !instance.GetComponent<PlayerDamageHitbox>().IsActive);
            });
            Check("피격 시 공격 취소 / 피격 중 신규 공격 차단", () =>
            {
                Assert(_attack.TryStartAttack(_definitions[0]));
                var instance = (PlayerAttackInstance)typeof(PlayerAttack).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_attack);
                _health.TakeDamage(1);
                Assert(!_attack.IsAttacking && !instance.GetComponent<PlayerDamageHitbox>().IsActive);
                Assert(!_attack.TryStartAttack(_definitions[0]));
                Set(_player.GetComponent<PlayerHit>(), "_hitUntil", 0f);
                Invoke(_player.GetComponent<PlayerHit>(), "ReleaseKnockbackLock");
            });
            Check("대시 중 공격 차단", () =>
            {
                var dash = _player.GetComponent<PlayerDash>();
                Set(dash, "<IsDashing>k__BackingField", true);
                try { Assert(!_attack.TryStartAttack(_definitions[0])); }
                finally { Set(dash, "<IsDashing>k__BackingField", false); }
            });
            Check("실제 물리 트리거로 Boss 피해 전달", () =>
            {
                var dummy = Spawn("Assets/03.Prefabs/Dummy/Dummy.prefab");
                foreach (var script in dummy.GetComponents<MonoBehaviour>()) if (!(script is Health) && !(script is DamageReceiver)) script.enabled = false;
                var instance = AttackObject(_definitions[0], 1);
                Physics2D.SyncTransforms();
                instance.TryGetHitBounds(out var bounds);
                var collider = dummy.GetComponent<Collider2D>();
                dummy.transform.position += bounds.center - collider.bounds.center;
                dummy.GetComponent<Rigidbody2D>().gravityScale = 0;
                Physics2D.SyncTransforms();
                int before = dummy.GetComponent<Health>().CurrentHealth;
                Assert(_scene.GetPhysicsScene2D().Simulate(0.02f));
                Assert(dummy.GetComponent<Health>().CurrentHealth == before - _definitions[0].damage);
            });
            Check("화염구가 얇은 벽을 sweep 검사로 정지", () =>
            {
                var instance = AttackObject(_definitions.First(d => d.attackType == PlayerAttackType.FireBall), 1);
                var fireball = instance.GetComponent<FireBallPrefab>();
                Invoke(fireball, "Start");
                Physics2D.SyncTransforms();
                instance.TryGetHitBounds(out var bounds);
                var wall = new GameObject("ThinWall");
                Temporary.Add(wall);
                SceneManager.MoveGameObjectToScene(wall, _scene);
                wall.tag = "Wall";
                wall.transform.position = new Vector3(bounds.max.x + 0.1f, bounds.center.y);
                wall.AddComponent<BoxCollider2D>().size = new Vector2(0.02f, 3);
                Physics2D.SyncTransforms();
                Invoke(fireball, "FixedUpdate");
                Assert(!instance.GetComponent<PlayerDamageHitbox>().IsActive);
            });
            Check("사망 시 공격 취소 / 입력으로 Death 덮어쓰기 차단", () =>
            {
                Assert(_attack.TryStartAttack(_definitions[0]));
                _health.TakeDamage(_health.CurrentHealth);
                Assert(!_attack.IsAttacking && _controller.CurrentState == PlayerState.Death);
                Assert(!_attack.TryStartAttack(_definitions[0]));
                _controller.ChangeState(PlayerState.Attack);
                Assert(_controller.CurrentState == PlayerState.Death);
            });
        }
        finally
        {
            foreach (var item in Temporary) if (item != null) Object.DestroyImmediate(item);
            Temporary.Clear();
            SceneManager.SetActiveScene(previousScene);
            SceneManager.UnloadSceneAsync(_scene);
        }
        Directory.CreateDirectory("Library/PlayerAttackRefactor");
        File.WriteAllLines("Library/PlayerAttackRefactor/play-regression.txt", Results);
        if (Results.Any(r => r.StartsWith("FAIL"))) throw new InvalidOperationException(string.Join("\n", Results));
        return string.Join("\n", Results);
    }
    private static PlayerAttackInstance AttackObject(PlayerAttackDataSO definition, int direction)
    {
        var root = Object.Instantiate(definition.attackPrefab.gameObject);
        Temporary.Add(root);
        SceneManager.MoveGameObjectToScene(root, _scene);
        root.transform.position = _player.transform.position + new Vector3(0, Temporary.Count * 10);
        var instance = root.GetComponent<PlayerAttackInstance>();
        Assert(instance.Initialize(definition.damage, direction, _player.transform));
        return instance;
    }
    private static GameObject Spawn(string path)
    {
        var root = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        Temporary.Add(root);
        SceneManager.MoveGameObjectToScene(root, _scene);
        return root;
    }
    private static void Check(string name, Action action)
    {
        try { action(); Results.Add("PASS " + name); }
        catch (Exception exception) { Results.Add("FAIL " + name + ": " + exception.GetBaseException().Message); }
    }
    private static void Assert(bool condition) { if (!condition) throw new InvalidOperationException("assertion failed"); }
    private static void Invoke(object target, string method) => target.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, null);
    private static void Set(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);
}
