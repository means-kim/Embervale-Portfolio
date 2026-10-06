using System.IO;
using Game.Effects;
using UnityEditor;
using UnityEngine;

namespace Game.Effects.EditorTools
{
    /// <summary>
    /// 메뉴 한 번으로 텍스처 임포트 설정 → 머티리얼 → Settings 에셋 → 프리팹까지 생성한다.
    /// 수동 세팅 실수(임포트 타입, 시트 분할, 정렬 순서 등)를 없애기 위한 에디터 전용 도구.
    /// </summary>
    public static class PlayerHitEffectBuilder
    {
        private const string RootDir = "Assets/PlayerHitEffect";
        private const string TextureDir = RootDir + "/Textures";
        private const string MaterialDir = RootDir + "/Materials";
        private const string SettingsPath = RootDir + "/PlayerHitEffectSettings.asset";
        private const string PrefabPath = RootDir + "/PlayerHitEffect.prefab";
        private const string ParticleShaderName = "Sprites/Default";

        private const string SplatTexture = "HitSplat.png";
        private const string CrackTexture = "HitCrack.png";
        private const string VignetteTexture = "HitVignette.png";
        private const string ShadeSheetTexture = "ShadeBlob_2x2.png";
        private const string ShardSheetTexture = "WhiteShard_2x2.png";

        /// <summary>프리팹을 (재)생성한다. 기존 Settings 에셋은 유지된다.</summary>
        [MenuItem("Tools/Player Hit Effect/Build Prefab")]
        public static void Build()
        {
            Sprite splat = ImportAsSprite(SplatTexture);
            Sprite crack = ImportAsSprite(CrackTexture);
            Sprite vignette = ImportAsSprite(VignetteTexture);
            Texture2D shadeSheet = ImportAsTexture(ShadeSheetTexture);
            Texture2D shardSheet = ImportAsTexture(ShardSheetTexture);

            PlayerHitEffectSettings settings = LoadOrCreateSettings();
            Material shadeMaterial = LoadOrCreateParticleMaterial("ShadeParticle.mat", shadeSheet);
            Material shardMaterial = LoadOrCreateParticleMaterial("WhiteShardParticle.mat", shardSheet);

            var root = new GameObject("PlayerHitEffect");
            try
            {
                SpriteRenderer splatRenderer = CreateSpriteChild(root.transform, "Splat", splat);
                SpriteRenderer crackRenderer = CreateSpriteChild(root.transform, "Crack", crack);
                SpriteRenderer dimRenderer = CreateSpriteChild(root.transform, "ScreenDim", vignette);
                RadialBurstEmitter shadeEmitter = CreateEmitterChild(root.transform, "ShadeBurst", shadeMaterial);
                RadialBurstEmitter shardEmitter = CreateEmitterChild(root.transform, "ShardBurst", shardMaterial);

                shadeEmitter.Configure(settings.shadeBurst, settings.sortingLayerName, settings.particleSortingOrder);
                shardEmitter.Configure(settings.shardBurst, settings.sortingLayerName, settings.particleSortingOrder);

                var effect = root.AddComponent<PlayerHitEffect>();
                var serialized = new SerializedObject(effect);
                serialized.FindProperty("settings").objectReferenceValue = settings;
                serialized.FindProperty("splatRenderer").objectReferenceValue = splatRenderer;
                serialized.FindProperty("crackRenderer").objectReferenceValue = crackRenderer;
                serialized.FindProperty("screenDimRenderer").objectReferenceValue = dimRenderer;
                serialized.FindProperty("shadeEmitter").objectReferenceValue = shadeEmitter;
                serialized.FindProperty("shardEmitter").objectReferenceValue = shardEmitter;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                Selection.activeObject = prefab;
                EditorGUIUtility.PingObject(prefab);
                Debug.Log($"[PlayerHitEffect] 프리팹 생성 완료: {PrefabPath}");
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static SpriteRenderer CreateSpriteChild(Transform parent, string name, Sprite sprite)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = false;
            return spriteRenderer;
        }

        private static RadialBurstEmitter CreateEmitterChild(Transform parent, string name, Material material)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<ParticleSystem>(); // ParticleSystemRenderer도 함께 추가됨
            go.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
            return go.AddComponent<RadialBurstEmitter>();
        }

        private static Sprite ImportAsSprite(string fileName)
        {
            string path = $"{TextureDir}/{fileName}";
            TextureImporter importer = GetImporter(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            ApplyCommonImportSettings(importer);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Texture2D ImportAsTexture(string fileName)
        {
            string path = $"{TextureDir}/{fileName}";
            TextureImporter importer = GetImporter(path);
            importer.textureType = TextureImporterType.Default;
            ApplyCommonImportSettings(importer);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static void ApplyCommonImportSettings(TextureImporter importer)
        {
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            // 부드러운 알파 그라데이션이 압축으로 깨지지 않게.
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }

        private static TextureImporter GetImporter(string path)
        {
            if (AssetImporter.GetAtPath(path) is TextureImporter importer)
            {
                return importer;
            }

            throw new FileNotFoundException($"텍스처를 찾을 수 없음: {path} (폴더 위치를 확인)");
        }

        private static PlayerHitEffectSettings LoadOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PlayerHitEffectSettings>(SettingsPath);
            if (settings != null)
            {
                return settings;
            }

            settings = ScriptableObject.CreateInstance<PlayerHitEffectSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        private static Material LoadOrCreateParticleMaterial(string fileName, Texture2D texture)
        {
            if (!AssetDatabase.IsValidFolder(MaterialDir))
            {
                AssetDatabase.CreateFolder(RootDir, "Materials");
            }

            string path = $"{MaterialDir}/{fileName}";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                // Sprites/Default: 빌트인/URP 모두에서 동작하는 언릿 알파블렌드 셰이더.
                material = new Material(Shader.Find(ParticleShaderName));
                AssetDatabase.CreateAsset(material, path);
            }

            material.mainTexture = texture;
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
            return material;
        }
    }
}
