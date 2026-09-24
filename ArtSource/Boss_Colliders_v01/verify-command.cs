var errors = new System.Collections.Generic.List<string>();
var samples = new System.Collections.Generic.List<object>();
var preview = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Boss/Boss_SnowGiant.prefab");
var obj = UnityEngine.Object.Instantiate(prefab);
UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, preview);
obj.transform.position = Vector3.zero;
obj.transform.rotation = Quaternion.identity;
obj.transform.localScale = Vector3.one;
var anim = obj.GetComponent<Animator>();
anim.enabled = false;
var names = new[] { "Hurtbox", "SlamHitbox", "SwingHitbox", "ChargeHitbox" };
var guids = UnityEditor.AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/05.Animation/Boss" });
var clipInfo = new System.Collections.Generic.List<object>();
try {
    foreach (var guid in guids) {
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        var keys = UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,
            UnityEditor.EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
        clipInfo.Add(new { path = path, name = clip.name, length = clip.length,
            loop = clip.isLooping, curves = UnityEditor.AnimationUtility.GetCurveBindings(clip).Length });
        var times = new System.Collections.Generic.SortedSet<float>();
        foreach (var key in keys) {
            times.Add(key.time);
            if (key.time > .001f) times.Add(key.time - .001f);
            times.Add(Mathf.Min(key.time + .001f, clip.length - .001f));
        }
        times.Add(clip.length - .001f);
        foreach (var time in times) {
            clip.SampleAnimation(obj, time);
            var sprite = obj.GetComponent<SpriteRenderer>().sprite;
            var spriteName = sprite.name;
            var index = int.Parse(spriteName.Substring(spriteName.LastIndexOf('_') + 1));
            var expected = new[] { !spriteName.StartsWith("Boss_Death"),
                spriteName.StartsWith("Boss_Slam") && (index == 4 || index == 5),
                spriteName.StartsWith("Boss_Swing") && (index == 4 || index == 5),
                spriteName.StartsWith("Boss_Charge") && index >= 3 && index <= 7 };
            var boxes = new System.Collections.Generic.List<object>();
            for (var i = 0; i < names.Length; i++) {
                var t = obj.transform.Find("Colliders/" + names[i]);
                var box = t.GetComponent<BoxCollider2D>();
                if (box.enabled != expected[i]) errors.Add(clip.name + " @ " + time + " " + names[i] + " wrong enabled state");
                if (!box.isTrigger || box.tag != "Boss" || box.gameObject.layer != 10 || box.attachedRigidbody != obj.GetComponent<Rigidbody2D>())
                    errors.Add(names[i] + " wrong trigger/tag/layer/rigidbody");
                boxes.Add(new { name = names[i], x = t.localPosition.x, y = t.localPosition.y,
                    width = box.size.x, height = box.size.y, rotation = Mathf.DeltaAngle(0, t.localEulerAngles.z), enabled = box.enabled });
            }
            samples.Add(new { clip = clip.name, time = time, sprite = spriteName, boxes = boxes });
        }
    }
    // Reuse the same object: attack -> Hit/Death must reset every attack collider.
    var slam = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Boss_Slam.anim");
    var hit = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Boss_Hit.anim");
    var death = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Boss_Death.anim");
    foreach (var target in new[] { hit, death }) {
        slam.SampleAnimation(obj, .81f);
        target.SampleAnimation(obj, 0f);
        for (var i = 1; i < names.Length; i++)
            if (obj.transform.Find("Colliders/" + names[i]).GetComponent<Collider2D>().enabled)
                errors.Add("Attack remained on after " + target.name);
    }
    slam.SampleAnimation(obj, .81f);
    var renderer = obj.GetComponent<SpriteRenderer>();
    var rig = obj.GetComponent<BossColliderRig>();
    renderer.flipX = false; rig.SyncFacing();
    var normal = rig.SlamHitbox.transform.position;
    renderer.flipX = true; rig.SyncFacing();
    var flipped = rig.SlamHitbox.transform.position;
    if (Mathf.Abs(normal.x + flipped.x) > .001f || Mathf.Abs(normal.y - flipped.y) > .001f)
        errors.Add("flipX did not mirror trigger");
} finally {
    UnityEngine.Object.DestroyImmediate(obj);
    UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(preview);
}
var liveBoss = GameObject.Find("Boss_SnowGiant");
return new { errors = errors, sampleCount = samples.Count, clips = clipInfo, samples = samples,
    liveColliderCount = liveBoss.GetComponentsInChildren<Collider2D>(true).Length,
    liveComponents = System.Array.ConvertAll(liveBoss.GetComponents<Component>(), c => c.GetType().Name) };
