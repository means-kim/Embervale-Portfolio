using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PlayerAttackSetupValidator
{
    [MenuItem("Tools/Player/Validate Attack Setup")]
    public static void Validate()
    {
        var checks = new List<string>();
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Player/Player.prefab");
        var bindings = player.GetComponent<PlayerInput>().AttackBindings;
        if (bindings.Length == 0 || bindings.Any(b => b == null || b.key == KeyCode.None))
            throw new InvalidOperationException("공격 키 연결 누락");
        if (bindings.Select(b => b.key).Distinct().Count() != bindings.Length)
            throw new InvalidOperationException("공격 키 중복");
        var controller = player.GetComponent<Animator>().runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        foreach (var binding in bindings)
        {
            var definition = binding.attack;
            if (definition == null || definition.attackPrefab == null) throw new InvalidOperationException("공격 연결 누락");
            var hitbox = definition.attackPrefab.GetComponent<PlayerDamageHitbox>();
            if (hitbox == null || !hitbox.IsConfigured) throw new InvalidOperationException("판정 연결 누락: " + definition.name);
            if (definition.attackPrefab.GetComponent<VFXFlip>() != null || definition.attackPrefab.GetComponent<VFXOffset>() != null)
                throw new InvalidOperationException("중복 오프셋/반전: " + definition.name);
            var states = controller.layers[0].stateMachine.states.Where(s => s.state.name == definition.animClipName).ToArray();
            if (states.Length != 1) throw new InvalidOperationException("애니메이션 연결 누락: " + definition.name);
            float duration = states[0].state.motion.averageDuration / states[0].state.speed;
            if (definition.windupDuration + definition.recoveryDuration + 0.0001f < duration)
                throw new InvalidOperationException("공격 행동이 애니메이션보다 먼저 끝납니다: " + definition.name);
            checks.Add(definition.name + " | key=" + binding.key + " | offset=" + definition.spawnOffset + " | damage=" + definition.damage + " | recovery=" + definition.recoveryDuration);
        }
        if (UnityEditorInternal.InternalEditorUtility.tags.Contains("Enemy")) throw new InvalidOperationException("Enemy 태그 미제거");
        Directory.CreateDirectory("Library/PlayerAttackRefactor");
        File.WriteAllLines("Library/PlayerAttackRefactor/validation.txt", checks);
        Debug.Log("Player attack asset validation PASS: " + checks.Count);
    }
}