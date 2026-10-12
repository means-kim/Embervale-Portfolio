using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class GlideVerification
{
    static void Call(object target, string method, params object[] args) => target.GetType().GetMethod(method,
        BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, args);
    public static string Main()
    {
        if (!Application.isPlaying) throw new Exception("Play mode required");
        var results = new List<string>();
        var scene = SceneManager.CreateScene("GlideVerificationTransient", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Player/Player.prefab");
        var p = UnityEngine.Object.Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(p, scene);
        p.transform.position = new Vector3(1000, 1000);
        var jump = p.GetComponent<PlayerJump>(); var glide = p.GetComponent<PlayerGlide>();
        var move = p.GetComponent<PlayerMove>(); var body = p.GetComponent<Rigidbody2D>();
        var input = p.GetComponent<PlayerInput>(); var controller = p.GetComponent<PlayerController>();
        var physics = scene.GetPhysicsScene2D();
        Action<string, bool> check = (name, valid) => { if (!valid) throw new Exception(name); results.Add(name + " PASS"); };
        Action press = () => {
            if (!glide.TryHandleJumpInput()) {
                var action = (Action)typeof(PlayerInput).GetField("OnJumpInput", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(input);
                action?.Invoke();
            }
        };
        Action reset = () => { glide.ResetForRespawn(); jump.ResetForRespawn(); body.position = new Vector2(1000,1000); body.linearVelocity = Vector2.zero; };
        Action tick = () => { Call(move, "FixedUpdate"); Call(glide, "FixedUpdate"); physics.Simulate(Time.fixedDeltaTime); };
        try
        {
            reset();
            press(); check("first jump cannot glide", jump.CurrentJumpCount == 1 && !glide.IsGliding);
            press(); check("second jump cannot auto-glide", jump.CurrentJumpCount == 2 && !glide.IsGliding);
            press(); check("third Space starts glide without consuming jump", glide.IsGliding && jump.CurrentJumpCount == 2);
            Call(controller, "Update"); p.GetComponent<Animator>().Update(.1f);
            check("Glide state and real sprite clip", controller.CurrentState == PlayerState.Glide
                && p.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Glide")
                && p.GetComponent<SpriteRenderer>().sprite.name.StartsWith("Glide_"));
            body.linearVelocity = new Vector2(0, -30); Call(move, "HandleMove", Vector2.right); tick();
            float fall = body.linearVelocity.y; float speed = body.linearVelocity.x;
            check("fall limited to 5", Mathf.Abs(fall + 5f) < .1f);
            check("horizontal speed 20 to 30", Mathf.Abs(speed - 30f) < .1f);
            press(); check("Space cancels without jump", !glide.IsGliding && jump.CurrentJumpCount == 2);
            Call(move, "HandleMove", Vector2.right); tick();
            check("normal speed restored", Mathf.Abs(body.linearVelocity.x - 20f) < .1f);
            check("normal gravity restored", body.linearVelocity.y < fall - .5f && Mathf.Abs(body.gravityScale - 10f) < .01f);
            press(); check("can toggle on again airborne", glide.IsGliding);
            Call(move, "HandleMove", Vector2.left); tick();
            check("left control and facing", body.linearVelocity.x < -29f && move.FacingDirection == -1);
            var collider = p.GetComponent<Collider2D>();
            var wall = new GameObject("VerificationWall"); SceneManager.MoveGameObjectToScene(wall,scene);
            wall.tag = "Ground"; var wallBox = wall.AddComponent<BoxCollider2D>(); wallBox.size = new Vector2(1,20);
            wall.transform.position = new Vector3(collider.bounds.min.x - .48f, body.position.y);
            Physics2D.SyncTransforms(); physics.Simulate(Time.fixedDeltaTime);
            check("real wall collision stops glide", !glide.IsGliding);
            UnityEngine.Object.DestroyImmediate(wall);
            reset(); press(); press(); press();
            body.linearVelocity = new Vector2(0,-30);
            var floor = new GameObject("VerificationFloor"); SceneManager.MoveGameObjectToScene(floor,scene);
            floor.tag="Ground"; var floorBox=floor.AddComponent<BoxCollider2D>(); floorBox.size = new Vector2(20,1);
            floor.transform.position=new Vector3(body.position.x,collider.bounds.min.y-.48f);
            Physics2D.SyncTransforms(); physics.Simulate(Time.fixedDeltaTime);
            check("real floor collision stops glide", !glide.IsGliding && jump.IsGrounded);
            check("toggle did not buffer landing jump", jump.CurrentJumpCount == 0);
            UnityEngine.Object.DestroyImmediate(floor);
            reset(); press(); press(); press();
            p.GetComponent<PlayerDash>().SendMessage("HandleDash");
            check("dash stops glide", !glide.IsGliding && p.GetComponent<PlayerDash>().IsDashing);
            p.GetComponent<PlayerDash>().ResetForRespawn();
            reset(); press(); press(); press();
            var charge = p.GetComponent<PlayerChargeJump>();
            Call(charge, "TryBegin");
            check("charge jump interrupts glide", charge.IsActive && !glide.IsGliding);
            charge.CancelChargeJump();
            reset(); press(); press(); press();
            p.GetComponent<Health>().TakeDamage(1);
            check("damage stops glide immediately", !glide.IsGliding);
            p.GetComponent<PlayerHit>().ResetForRespawn();
            reset(); press(); press(); press();
            p.GetComponent<Health>().SpendHealth(p.GetComponent<Health>().CurrentHealth);
            check("death stops glide and clears eligibility", !glide.IsGliding && p.GetComponent<Health>().IsDead);
            p.GetComponent<Health>().ReviveFull(); p.GetComponent<PlayerHit>().ResetForRespawn();
            p.GetComponent<PlayerController>().ResetForRespawn();
            reset(); press(); press(); press(); p.GetComponent<Player>().MoveToCheckpoint(new Vector3(1000,1000));
            check("respawn clears glide eligibility", !glide.IsGliding && !glide.TryHandleJumpInput());
            reset(); press(); press(); press(); glide.enabled=false;
            check("disable cleans up", !glide.IsGliding && body.gravityScale == 10f);
            glide.enabled=true;
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/player/Glide.anim");
            var binding=EditorCurveBinding.PPtrCurve("",typeof(SpriteRenderer),"m_Sprite");
            var keys=AnimationUtility.GetObjectReferenceCurve(clip,binding);
            check("eight unique animation frames", new HashSet<UnityEngine.Object>(Array.ConvertAll(keys,k=>k.value)).Count==8);
            results.Add($"Measured vx={speed:F3}, vy={fall:F3}; prefab gravity={body.gravityScale:F3}");
            System.IO.File.WriteAllLines("Docs/Art/PlayerGlide_v01/Verification.txt",results);
            return string.Join("\n",results);
        }
        finally { UnityEngine.Object.DestroyImmediate(p); SceneManager.UnloadSceneAsync(scene); }
    }
}
