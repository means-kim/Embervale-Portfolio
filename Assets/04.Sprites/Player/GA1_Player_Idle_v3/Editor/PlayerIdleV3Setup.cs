#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace GA1.PlayerArt.Editor
{
    /// <summary>Builds preview assets from this package's 32 PNG frames.</summary>
    public sealed class PlayerIdleV3Setup : EditorWindow
    {
        private const int FrameCount = 32;
        private const float Fps = 20f;
        private static readonly Vector2 Pivot = new Vector2(0.5f, 0.19921875f);
        [SerializeField] private float pixelsPerUnit = 200f;

        [MenuItem("Tools/GA1/Build Player Idle v3")]
        private static void Open()
        {
            GetWindow<PlayerIdleV3Setup>(true, "Player Idle v3");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Idle: 32 frames / 20 fps / 1.6 seconds", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Builds an animation clip, a preview controller and a preview prefab inside this package. Match PPU to your player project.", MessageType.Info);
            pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", pixelsPerUnit);
            using (new EditorGUI.DisabledScope(pixelsPerUnit <= 0f || float.IsNaN(pixelsPerUnit) || float.IsInfinity(pixelsPerUnit)))
            {
                if (GUILayout.Button("Build Idle Assets")) Build();
            }
        }

        private void Build()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string packagePath = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)).Replace('\\', '/');
            string framesPath = packagePath + "/Frames";
            string generatedPath = packagePath + "/Generated";
            try
            {
                // Check the complete input before modifying import settings.
                for (int i = 0; i < FrameCount; i++)
                {
                    string framePath = framesPath + $"/Idle_{i:00}.png";
                    if (!File.Exists(framePath)) throw new FileNotFoundException("Missing idle frame", framePath);
                }

                var sprites = new Sprite[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    string framePath = framesPath + $"/Idle_{i:00}.png";
                    EditorUtility.DisplayProgressBar("Player Idle v3", $"Importing {i + 1}/{FrameCount}", (float)i / FrameCount);
                    var importer = AssetImporter.GetAtPath(framePath) as TextureImporter;
                    if (importer == null) throw new InvalidOperationException("Texture importer unavailable: " + framePath);
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
                    settings.spritePivot = Pivot;
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                    sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(framePath);
                    if (sprites[i] == null) throw new InvalidOperationException("Sprite import failed: " + framePath);
                }

                if (!AssetDatabase.IsValidFolder(generatedPath)) AssetDatabase.CreateFolder(packagePath, "Generated");
                string clipPath = generatedPath + "/Player_Idle_v3.anim";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                if (clip == null)
                {
                    clip = new AnimationClip { name = "Player_Idle_v3" };
                    AssetDatabase.CreateAsset(clip, clipPath);
                }
                clip.ClearCurves();
                clip.frameRate = Fps;
                clip.wrapMode = WrapMode.Loop;
                var keys = new ObjectReferenceKeyframe[FrameCount + 1];
                for (int i = 0; i < FrameCount; i++)
                    keys[i] = new ObjectReferenceKeyframe { time = i / Fps, value = sprites[i] };
                // This key marks the boundary. It does not add a 33rd held frame.
                keys[FrameCount] = new ObjectReferenceKeyframe { time = FrameCount / Fps, value = sprites[0] };
                AnimationUtility.SetObjectReferenceCurve(clip,
                    EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
                var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                clipSettings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                EditorUtility.SetDirty(clip);

                string controllerPath = generatedPath + "/Player_Idle_v3_Preview.controller";
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
                if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                var machine = controller.layers[0].stateMachine;
                var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Idle");
                if (state == null) state = machine.AddState("Idle");
                state.motion = clip;
                state.speed = 1f;
                state.writeDefaultValues = false;
                machine.defaultState = state;
                EditorUtility.SetDirty(controller);

                var preview = new GameObject("Player_Idle_v3_Preview");
                try
                {
                    preview.AddComponent<SpriteRenderer>().sprite = sprites[0];
                    var animator = preview.AddComponent<Animator>();
                    animator.runtimeAnimatorController = controller;
                    animator.applyRootMotion = false;
                    PrefabUtility.SaveAsPrefabAsset(preview, generatedPath + "/Player_Idle_v3_Preview.prefab");
                }
                finally { DestroyImmediate(preview); }
                AssetDatabase.SaveAssets();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(generatedPath + "/Player_Idle_v3_Preview.prefab");
                EditorGUIUtility.PingObject(Selection.activeObject);
                Debug.Log("Player Idle v3 ready: " + generatedPath + ". Clip binds to SpriteRenderer on the same GameObject as Animator.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("Idle build failed", ex.Message, "OK");
            }
            finally { EditorUtility.ClearProgressBar(); }
        }
    }
}
#endif
