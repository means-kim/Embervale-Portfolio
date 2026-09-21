#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace GA1.PlayerArt.Editor
{
    public sealed class PlayerMotionsV4Setup : EditorWindow
    {
        [Serializable] private sealed class Manifest { public int version; public int[] frameSize; public float[] pivot; public ClipSpec[] clips; }
        [Serializable] private sealed class ClipSpec
        {
            public string name; public int frameCount; public float fps; public bool loop;
            public float duration; public float[] times; public string[] frames;
        }
        [SerializeField] private float pixelsPerUnit = 200f;
        [SerializeField] private string rendererPath = "";

        [MenuItem("Tools/GA1/Build Player Motions v4")]
        private static void Open() { GetWindow<PlayerMotionsV4Setup>(true, "Player Motions v4"); }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("16 motion clips / 101 frames / Original Run fix", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use the same PPU as Idle v3. Empty SpriteRenderer Path means Animator and SpriteRenderer are on the same GameObject. Builds clips and a separate preview prefab.", MessageType.Info);
            pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", pixelsPerUnit);
            rendererPath = EditorGUILayout.TextField("SpriteRenderer Path", rendererPath);
            using (new EditorGUI.DisabledScope(pixelsPerUnit <= 0f || float.IsNaN(pixelsPerUnit) || float.IsInfinity(pixelsPerUnit)))
            {
                if (GUILayout.Button("Build Motion Assets")) Build(false);
                if (GUILayout.Button("Update Run Only")) Build(true);
            }
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + name)) AssetDatabase.CreateFolder(parent, name);
        }

        private void Build(bool runOnly)
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string packagePath = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)).Replace('\\', '/');
            string binding = rendererPath.Trim().Trim('/');
            GameObject preview = null;
            try
            {
                if (binding.Contains("\\") || binding.Split('/').Any(part => part == "." || part == ".." || (binding.Length > 0 && part.Length == 0)))
                    throw new InvalidOperationException("Use a relative hierarchy path such as Visual or Graphics/Sprite; leave empty for the same GameObject.");
                var data = JsonUtility.FromJson<Manifest>(File.ReadAllText(packagePath + "/Motions.json"));
                if (data == null || data.version != 4 || data.clips == null || data.clips.Length != 16 || data.frameSize == null || data.frameSize.Length != 2 || data.frameSize[0] != 512 || data.frameSize[1] != 512 || data.pivot == null || data.pivot.Length != 2)
                    throw new InvalidOperationException("Invalid or incomplete Motions.json.");
                foreach (var spec in data.clips)
                {
                    if (spec.frames == null || spec.times == null || spec.frameCount != spec.frames.Length || spec.times.Length != spec.frames.Length || spec.frameCount < 1 || spec.fps <= 0f || spec.times[0] != 0f || spec.duration <= spec.times[spec.times.Length - 1])
                        throw new InvalidOperationException("Invalid clip timing: " + spec.name);
                    for (int i = 0; i < spec.frameCount; i++)
                    {
                        if (i > 0 && spec.times[i] <= spec.times[i - 1]) throw new InvalidOperationException("Unordered frame timing: " + spec.name);
                        if (!File.Exists(packagePath + "/" + spec.frames[i])) throw new FileNotFoundException("Missing motion frame", spec.frames[i]);
                    }
                }
                EnsureFolder(packagePath, "Generated");
                string generated = packagePath + "/Generated";
                EnsureFolder(generated, "Clips");
                var selectedClips = runOnly ? data.clips.Where(s => s.name == "Run").ToArray() : data.clips;
                if (selectedClips.Length == 0) throw new InvalidOperationException("Run clip not found in Motions.json.");
                var clips = new AnimationClip[selectedClips.Length];
                Sprite firstSprite = null;
                int completed = 0, total = selectedClips.Sum(s => s.frameCount);
                for (int c = 0; c < selectedClips.Length; c++)
                {
                    var spec = selectedClips[c];
                    var sprites = new Sprite[spec.frameCount];
                    for (int i = 0; i < spec.frameCount; i++)
                    {
                        string framePath = packagePath + "/" + spec.frames[i];
                        EditorUtility.DisplayProgressBar("Player Motions v4", spec.name + " " + (i + 1) + "/" + spec.frameCount, (float)completed / total);
                        var importer = AssetImporter.GetAtPath(framePath) as TextureImporter;
                        if (importer == null) throw new InvalidOperationException("Texture importer missing: " + framePath);
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.spritePixelsPerUnit = pixelsPerUnit;
                        importer.alphaIsTransparency = true;
                        importer.mipmapEnabled = false;
                        importer.npotScale = TextureImporterNPOTScale.None;
                        importer.filterMode = FilterMode.Bilinear;
                        importer.wrapMode = TextureWrapMode.Clamp;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.maxTextureSize = 512;
                        var settings = new TextureImporterSettings();
                        importer.ReadTextureSettings(settings);
                        settings.spriteAlignment = (int)SpriteAlignment.Custom;
                        settings.spritePivot = new Vector2(data.pivot[0], data.pivot[1]);
                        settings.spriteMeshType = SpriteMeshType.FullRect;
                        importer.SetTextureSettings(settings);
                        importer.SaveAndReimport();
                        sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(framePath);
                        if (sprites[i] == null) throw new InvalidOperationException("Sprite import failed: " + framePath);
                        completed++;
                    }
                    if (firstSprite == null) firstSprite = sprites[0];
                    string clipPath = generated + "/Clips/Player_" + spec.name + "_v4.anim";
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                    if (clip == null)
                    {
                        clip = new AnimationClip { name = "Player_" + spec.name + "_v4" };
                        AssetDatabase.CreateAsset(clip, clipPath);
                    }
                    clip.ClearCurves();
                    clip.frameRate = spec.fps;
                    clip.wrapMode = spec.loop ? WrapMode.Loop : WrapMode.ClampForever;
                    var keys = new ObjectReferenceKeyframe[spec.frameCount + 1];
                    for (int i = 0; i < spec.frameCount; i++) keys[i] = new ObjectReferenceKeyframe { time = spec.times[i], value = sprites[i] };
                    // Boundary key sets clip duration without adding a held duplicate frame.
                    keys[spec.frameCount] = new ObjectReferenceKeyframe { time = spec.duration, value = spec.loop ? sprites[0] : sprites[spec.frameCount - 1] };
                    AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve(binding, typeof(SpriteRenderer), "m_Sprite"), keys);
                    var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                    clipSettings.loopTime = spec.loop;
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                    // No Animation Events are installed: gameplay owns attack, input and physics.
                    EditorUtility.SetDirty(clip);
                    clips[c] = clip;
                }
                if (runOnly)
                {
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = clips[0];
                    EditorGUIUtility.PingObject(clips[0]);
                    Debug.Log("Original Run restored: 8 original frames + 2 boundary frames / 0.5 s. Existing Run clip reference is preserved.");
                    return;
                }
                string controllerPath = generated + "/Player_Motions_v4_Preview.controller";
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
                if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                var machine = controller.layers[0].stateMachine;
                for (int i = 0; i < data.clips.Length; i++)
                {
                    var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == data.clips[i].name);
                    if (state == null) state = machine.AddState(data.clips[i].name, new Vector3(280 * (i % 4), 90 * (i / 4)));
                    state.motion = clips[i]; state.speed = 1f; state.writeDefaultValues = false;
                    if (i == 0) machine.defaultState = state;
                }
                EditorUtility.SetDirty(controller);
                preview = new GameObject("Player_Motions_v4_Preview");
                Transform visual = preview.transform;
                if (binding.Length > 0)
                    foreach (string part in binding.Split('/'))
                    {
                        var child = new GameObject(part); child.transform.SetParent(visual, false); visual = child.transform;
                    }
                visual.gameObject.AddComponent<SpriteRenderer>().sprite = firstSprite;
                var animator = preview.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller; animator.applyRootMotion = false;
                preview.AddComponent<GA1.PlayerArt.PlayerMotionPreviewV4>();
                PrefabUtility.SaveAsPrefabAsset(preview, generated + "/Player_Motions_v4_Preview.prefab");
                AssetDatabase.SaveAssets();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(generated + "/Player_Motions_v4_Preview.prefab");
                EditorGUIUtility.PingObject(Selection.activeObject);
                Debug.Log("Player Motions v4 ready: " + generated + "/Clips. Attach these .anim clips to states in your existing player controller. The preview prefab is only for inspection.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("Motion build failed", ex.Message, "OK");
            }
            finally
            {
                if (preview != null) DestroyImmediate(preview);
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
#endif
