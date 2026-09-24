var preview = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
var obj = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Boss/Boss_SnowGiant.prefab"));
UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, preview);
var reports = new System.Collections.Generic.List<object>();
var errors = new System.Collections.Generic.List<string>();
try {
    var a = obj.GetComponent<Animator>();
    a.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    a.Rebind(); a.Update(0f);
    var rig = obj.GetComponent<BossColliderRig>();
    a.Play("Boss_Slam", 0, .81f / 1.65f); a.Update(0f);
    if (!rig.SlamHitbox.enabled) errors.Add("Slam impact inactive");
    a.SetTrigger("IsHit"); a.Update(.001f); a.Update(0f);
    var ok = a.GetCurrentAnimatorStateInfo(0).IsName("Boss_Hit") &&
        !rig.SlamHitbox.enabled && !rig.SwingHitbox.enabled && !rig.ChargeHitbox.enabled;
    reports.Add(new { test = "Slam -> IsHit", passed = ok });
    if (!ok) errors.Add("IsHit interruption");
    a.Play("Boss_Swing", 0, .65f / 1.5f); a.Update(0f);
    if (!rig.SwingHitbox.enabled) errors.Add("Swing impact inactive");
    a.SetTrigger("IsDeath"); a.Update(.001f); a.Update(0f);
    ok = a.GetCurrentAnimatorStateInfo(0).IsName("Boss_Death") && !rig.Hurtbox.enabled &&
        !rig.SlamHitbox.enabled && !rig.SwingHitbox.enabled && !rig.ChargeHitbox.enabled;
    reports.Add(new { test = "Swing -> IsDeath", passed = ok });
    if (!ok) errors.Add("IsDeath interruption");
    a.Play("Boss_Charge", 0, .9f / 2.45f); a.Update(0f);
    if (!rig.ChargeHitbox.enabled) errors.Add("Charge inactive");
    obj.GetComponent<Health>().TakeDamage(10000);
    // Direct invocation in this editor-only fixture avoids Unity SendMessage lifecycle assertions.
    typeof(BossColliderRig).GetMethod("LateUpdate", System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Instance).Invoke(rig, null);
    ok = !rig.Hurtbox.enabled && !rig.SlamHitbox.enabled && !rig.SwingHitbox.enabled && !rig.ChargeHitbox.enabled;
    reports.Add(new { test = "Health.IsDead suppresses all", passed = ok });
    if (!ok) errors.Add("Health gate");
} finally {
    UnityEngine.Object.DestroyImmediate(obj);
    UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(preview);
}
return new { errors = errors, reports = reports };
