using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EndingFX.Editor
{
    /// <summary>
    /// 메뉴 한 번으로 EndingFX 머티리얼/프리팹/데모 씬을 생성한다.
    /// 파티클 수치를 코드에 모아둔 이유: 여러 프리팹(일반 타격/최종 폭발)이 같은 레이어 구성을 배율만 달리해서 쓰므로
    /// 한 곳(BuildBurstSpecs)에서 관리해야 수치가 어긋나지 않는다. 생성 후에는 Inspector 에서 자유롭게 다듬으면 된다.
    /// </summary>
    public static class EndingFxAssetBuilder
    {
        private const string Root = "Assets/EndingFX";
        private const string TextureDir = Root + "/Textures";
        private const string SlashSpriteDir = Root + "/Sprites/FireSlash";
        private const string BossSpriteDir = Root + "/Sprites/Boss";
        private const string BossIntroDir = BossSpriteDir + "/Intro";
        private const string BossDamageDir = BossSpriteDir + "/Damage";
        private const string BossDeathDir = BossSpriteDir + "/Death";

        // 보스 프레임 1장(정사각 셀)의 월드 크기. 6.144유닛 → 몸통 약 4.6유닛 = ortho 3.5 카메라에서 클로즈업 구도
        // PPU 는 이미지 해상도에서 역산하므로 1024px / 1536px 원본 어느 쪽을 넣어도 화면 크기가 같다.
        private const float BossCellWorldSize = 6.144f;
        // 화면에서 보스가 차지하는 크기(1080p 기준 약 900px)에는 1024 로 충분. 메모리 절약
        private const int BossMaxTextureSize = 1024;
        private const string MaterialDir = Root + "/Materials";
        private const string PrefabDir = Root + "/Prefabs";
        private const string DataDir = Root + "/Data";
        private const string ShaderName = "EndingFX/Particle";

        // 불 색상 팔레트 (첨부한 슬래시 이펙트의 흰 코어 → 노랑 → 주황 → 빨강 꼬리에 맞춤)
        private static readonly Color WhiteHot = new Color(1f, 0.97f, 0.86f);
        private static readonly Color Yellow = new Color(1f, 0.82f, 0.32f);
        private static readonly Color Orange = new Color(1f, 0.48f, 0.1f);
        private static readonly Color Red = new Color(0.85f, 0.16f, 0.05f);
        private static readonly Color AshColor = new Color(0.12f, 0.05f, 0.03f);

        // 보스(영혼/얼음 계열) 팔레트: 보스 스프라이트의 옅은 하늘색 → 진한 남색에서 추출
        private static readonly Color SpiritWhite = new Color(0.93f, 0.97f, 1f);
        private static readonly Color SpiritCyan = new Color(0.62f, 0.84f, 1f);
        private static readonly Color SpiritBlue = new Color(0.32f, 0.5f, 1f);
        private static readonly Color SpiritDeep = new Color(0.14f, 0.18f, 0.6f);

        private enum Blend { Additive, Alpha }

        /// <summary>파티클 레이어 1개의 설정값.</summary>
        private sealed class LayerSpec
        {
            public string Name;
            public Material Material;
            public Vector2 Lifetime;
            public Vector2 Speed;
            public Vector2 Size;
            public Gradient Color;
            public AnimationCurve SizeOverLife;
            public float Drag;
            public float Gravity;
            public float Noise;
            public float NoiseFrequency = 0.8f;
            public AnimationCurve NoiseOverLife;   // null = 일정. 소멸 연출에서 "모양 유지 → 점점 흩어짐"에 사용
            public Vector2 Rise;            // 월드 Y 상승 속도 (Velocity over Lifetime)
            public bool Stretch;
            public float StretchVelocityScale = 0.035f;
            public bool RandomRotation;
            public float ShapeRadius;
            public bool SpriteShape;
            public int MaxParticles = 500;
            public int SortingOrder;
            public Vector2Int Count;        // ImpactBurst min/max 개수
            public Vector2 AngularVelocity; // 도/초. 파편이 회전하며 날아감
            public Vector2Int SheetTiles;   // 텍스처 아틀라스 타일 수. (2,2) → 4종 중 랜덤 1장
        }

        private enum BurstStyle { Ember, Shard }

        /// <summary>타격 파티클 색 구성. Hot(코어) → Mid → Low → Tail(소멸) 순으로 식어간다.</summary>
        private sealed class BurstPalette
        {
            public string Name;
            public Color Hot, Mid, Low, Tail;
            public Color FlashHot, FlashMid;
            public Color Accent;
            public bool Dark;            // true 면 주 레이어를 알파 블렌드로 (가산은 검정이 안 보임)
            public bool AccentAdditive;

            public static readonly BurstPalette Fire = new BurstPalette
            {
                Name = "Fire",
                Hot = WhiteHot, Mid = Yellow, Low = Orange, Tail = Red,
                FlashHot = WhiteHot, FlashMid = Orange,
                Accent = AshColor,
            };

            public static readonly BurstPalette Black = new BurstPalette
            {
                Name = "Black",
                // 순흑으로 시작하면 어두운 배경에서 안 보임 → 회색으로 터져서 검정으로 식어간다
                Hot = new Color(0.55f, 0.52f, 0.6f), Mid = new Color(0.22f, 0.2f, 0.26f),
                Low = new Color(0.07f, 0.06f, 0.09f), Tail = new Color(0f, 0f, 0f),
                FlashHot = new Color(1f, 1f, 1f), FlashMid = new Color(0.55f, 0.55f, 0.6f),
                Accent = new Color(0.85f, 0.85f, 0.9f),   // 밝은 티끌로 검은 파티클 윤곽을 살림
                Dark = true,
                AccentAdditive = true,
            };

            public static readonly BurstPalette Crimson = new BurstPalette
            {
                Name = "Red",
                Hot = new Color(1f, 0.88f, 0.86f), Mid = new Color(1f, 0.3f, 0.25f),
                Low = new Color(0.75f, 0.04f, 0.06f), Tail = new Color(0.25f, 0f, 0.03f),
                FlashHot = new Color(1f, 0.92f, 0.9f), FlashMid = new Color(1f, 0.25f, 0.2f),
                Accent = new Color(0.08f, 0.01f, 0.02f),
            };

            public static readonly BurstPalette Blue = new BurstPalette
            {
                Name = "Blue",
                Hot = SpiritWhite, Mid = SpiritCyan, Low = SpiritBlue, Tail = SpiritDeep,
                FlashHot = SpiritWhite, FlashMid = SpiritCyan,
                Accent = new Color(0.02f, 0.03f, 0.08f),
            };
        }

        private sealed class Materials
        {
            public Material Glow, Spark, Ember, Ring, Ash, Slash, GlowSprite;
            public Material Shard, SparkAlpha, EmberAlpha, RingAlpha, ShardAlpha, GlowAlpha;
        }

        private sealed class BossClips
        {
            public FlipbookClip Intro, Damage, Death;
        }

        [MenuItem("Tools/Ending FX/Build Assets")]
        public static void BuildAssets()
        {
            ConfigureTextureImports();
            Materials mats = BuildMaterials();
            // 첨부 이펙트 기준: 0=예비 점, 1=가는 선, 2=완전히 펼쳐진 호(피크), 3~6=흩어지며 소멸
            FlipbookClip clip = BuildClip("FireSlashClip", SlashSpriteDir, fps: 24f, peakIndex: 2);
            BuildBossClips();

            BuildFireSlashPrefab(mats, clip);
            BuildBurstPrefab("FX_HitBurst", mats, 0f, BurstPalette.Fire, BurstStyle.Ember);
            BuildBurstPrefab("FX_FinalBurst", mats, 1f, BurstPalette.Fire, BurstStyle.Ember);
            BuildBurstVariants(mats);
            BuildBossDissolvePrefab(mats);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[EndingFX] 에셋 생성 완료 → " + PrefabDir);
        }

        // ───────────────────────── Import / Material ─────────────────────────

        private static void ConfigureTextureImports()
        {
            foreach (string path in FindAssetPaths("t:Texture2D", TextureDir))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Default;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = true;
                importer.SaveAndReimport();
            }

            ImportSprites(SlashSpriteDir, 100f, 2048);
            ImportSprites(BossSpriteDir, null, BossMaxTextureSize);
        }

        /// <param name="pixelsPerUnit">null 이면 보스 셀 크기 기준으로 원본 해상도에서 계산.</param>
        private static void ImportSprites(string folder, float? pixelsPerUnit, int maxSize)
        {
            foreach (string path in FindAssetPaths("t:Texture2D", folder))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit ?? ReadPngWidth(path) / BossCellWorldSize;
                importer.spritePivot = new Vector2(0.5f, 0.5f);   // 셀 중심 고정 → 프레임 간 위치가 어긋나지 않음
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = maxSize;
                importer.SaveAndReimport();
            }
        }

        private static Materials BuildMaterials()
        {
            EnsureFolder(MaterialDir);
            return new Materials
            {
                Glow = CreateMaterial("M_FX_Glow", "fx_glow", Blend.Additive, 1.6f),
                Spark = CreateMaterial("M_FX_Spark", "fx_spark", Blend.Additive, 2.0f),
                Ember = CreateMaterial("M_FX_Ember", "fx_ember", Blend.Additive, 1.8f),
                Ring = CreateMaterial("M_FX_Ring", "fx_ring", Blend.Additive, 1.5f),
                Ash = CreateMaterial("M_FX_Ash", "fx_ash", Blend.Alpha, 1f),
                // 슬래시는 알파 블렌드: 밝은 카드 배경(베이지 등)에서도 색이 날아가지 않음
                Slash = CreateMaterial("M_FX_FireSlash", null, Blend.Alpha, 1.35f),
                GlowSprite = CreateMaterial("M_FX_GlowSprite", null, Blend.Additive, 1.5f),
                // 파편/어두운 팔레트용
                Shard = CreateMaterial("M_FX_Shard", "fx_shards", Blend.Additive, 1.6f),
                ShardAlpha = CreateMaterial("M_FX_Shard_Alpha", "fx_shards", Blend.Alpha, 1f),
                SparkAlpha = CreateMaterial("M_FX_Spark_Alpha", "fx_spark", Blend.Alpha, 1f),
                EmberAlpha = CreateMaterial("M_FX_Ember_Alpha", "fx_ember", Blend.Alpha, 1f),
                RingAlpha = CreateMaterial("M_FX_Ring_Alpha", "fx_ring", Blend.Alpha, 1f),
                GlowAlpha = CreateMaterial("M_FX_Glow_Alpha", "fx_glow", Blend.Alpha, 1f),
            };
        }

        private static Material CreateMaterial(string name, string textureName, Blend blend, float intensity)
        {
            string path = $"{MaterialDir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find(ShaderName));
                AssetDatabase.CreateAsset(mat, path);
            }

            if (textureName != null)
            {
                mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TextureDir}/{textureName}.png");
            }

            mat.SetFloat("_Intensity", intensity);
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", blend == Blend.Additive
                ? (float)UnityEngine.Rendering.BlendMode.One
                : (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        // 포효 시트에서 팔을 활짝 펼친 구간(몸 너비 기준 4~10번). 이 구간을 느리게 재생하고 공격을 시작한다
        private const int RoarStartFrame = 4;
        private const int RoarEndFrame = 10;
        private const float RoarSpeed = 0.25f;

        private static BossClips BuildBossClips()
        {
            // 포효: 마지막 프레임 = Damage 0번 (원본 시트에서 픽셀 단위로 동일)
            FlipbookClip intro = BuildClip("BossIntroClip", BossIntroDir, fps: 14f, peakIndex: 0);
            intro.EditorSetSlowSection(RoarStartFrame, RoarEndFrame, RoarSpeed);
            EditorUtility.SetDirty(intro);

            return new BossClips
            {
                Intro = intro,
                // 피격: 재생하지 않고 진행도로 프레임 선택 (fps 미사용)
                Damage = BuildClip("BossDamageClip", BossDamageDir, fps: 12f, peakIndex: 0),
                Death = BuildClip("BossDeathClip", BossDeathDir, fps: 12f, peakIndex: 0),
            };
        }

        private static FlipbookClip BuildClip(string assetName, string spriteFolder, float fps, int peakIndex)
        {
            EnsureFolder(DataDir);
            string path = $"{DataDir}/{assetName}.asset";
            var clip = AssetDatabase.LoadAssetAtPath<FlipbookClip>(path);
            if (clip == null)
            {
                clip = ScriptableObject.CreateInstance<FlipbookClip>();
                AssetDatabase.CreateAsset(clip, path);
            }

            Sprite[] frames = FindAssetPaths("t:Sprite", spriteFolder)
                .OrderBy(p => p, System.StringComparer.Ordinal)
                .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
                .Where(sprite => sprite != null)
                .ToArray();

            if (frames.Length == 0)
            {
                Debug.LogWarning($"[EndingFX] {spriteFolder} 에 프레임이 없음. {assetName} 을 직접 채워줘.");
            }

            clip.EditorSetFrames(frames, fps, peakIndex);
            EditorUtility.SetDirty(clip);
            return clip;
        }

        // ───────────────────────── Prefabs ─────────────────────────

        private static void BuildFireSlashPrefab(Materials mats, FlipbookClip clip)
        {
            var root = new GameObject("FX_FireSlash");
            var sr = root.AddComponent<SpriteRenderer>();
            sr.sharedMaterial = mats.Slash;
            sr.sortingOrder = 20;

            // 피크 순간 슬래시 모양에서 떨어져 나가는 불티
            ParticleSystem shed = CreateSystem(root.transform, new LayerSpec
            {
                Name = "EmberShed",
                Material = mats.Ember,
                Lifetime = new Vector2(0.35f, 0.9f),
                Speed = new Vector2(0.8f, 3.5f),
                Size = new Vector2(0.05f, 0.13f),
                Color = FireGradient(),
                SizeOverLife = Curve(0f, 1f, 1f, 0f),
                Drag = 2.5f,
                Gravity = -0.08f,
                Noise = 0.5f,
                RandomRotation = true,
                SpriteShape = true,
                SortingOrder = 21,
            });
            ParticleSystem.ShapeModule shape = shed.shape;
            shape.spriteRenderer = sr;

            var effect = root.AddComponent<FireSlashEffect>();
            var so = new SerializedObject(effect);
            so.FindProperty("clip").objectReferenceValue = clip;
            so.FindProperty("emberShed").objectReferenceValue = shed;
            so.ApplyModifiedPropertiesWithoutUndo();

            SavePrefab(root);
        }

        /// <param name="power">0 = 일반 타격, 1 = 마지막 일격 (개수/속도/수명/크기 배율)</param>
        private static void BuildBurstPrefab(string name, Materials mats, float power, BurstPalette palette, BurstStyle style)
        {
            var root = new GameObject(name);
            var layers = new List<ImpactBurst.Layer>();
            foreach (LayerSpec spec in BuildBurstSpecs(mats, power, palette, style))
            {
                layers.Add(new ImpactBurst.Layer(CreateSystem(root.transform, spec), spec.Count.x, spec.Count.y));
            }

            root.AddComponent<ImpactBurst>().EditorSetLayers(layers.ToArray());
            SavePrefab(root);
        }

        /// <summary>색 변형(검정/빨강/파랑) × 모양 변형(불티/파편) = 6종을 일반 타격·마지막 일격 각각 생성.</summary>
        private static void BuildBurstVariants(Materials mats)
        {
            foreach (BurstPalette palette in new[] { BurstPalette.Black, BurstPalette.Crimson, BurstPalette.Blue })
            {
                foreach (BurstStyle style in new[] { BurstStyle.Ember, BurstStyle.Shard })
                {
                    string suffix = $"{palette.Name}_{style}";
                    BuildBurstPrefab($"FX_HitBurst_{suffix}", mats, 0f, palette, style);
                    BuildBurstPrefab($"FX_FinalBurst_{suffix}", mats, 1f, palette, style);
                }
            }
        }

        /// <summary>
        /// 타격 폭발 레이어 구성. 할로우 나이트 엔딩 요소를 치환:
        /// 화이트 플래시 → Flash / 흰 파편 → Sparks / 분홍 점 → Embers(또는 Shards) / 분홍 링 → Rings(또는 Chips) / 검은 점 → Accent
        /// 수치(속도/수명/개수)는 스타일만 따르고, 색과 블렌드는 팔레트만 따른다. → 조합이 늘어도 수치가 한 곳에 모여 있음
        /// </summary>
        private static IEnumerable<LayerSpec> BuildBurstSpecs(Materials mats, float power, BurstPalette p, BurstStyle style)
        {
            float speedMul = Mathf.Lerp(1f, 1.5f, power);
            float lifeMul = Mathf.Lerp(1f, 1.6f, power);
            float countMul = Mathf.Lerp(1f, 3f, power);

            Vector2Int Counts(int min, int max) => power > 0f
                ? new Vector2Int(Mathf.RoundToInt(max * countMul), Mathf.RoundToInt(max * countMul))
                : new Vector2Int(min, max);

            // 어두운 팔레트는 가산으로는 보이지 않으므로 알파 블렌드 머티리얼을 쓴다
            Material Pick(Material additive, Material alpha) => p.Dark ? alpha : additive;

            yield return new LayerSpec
            {
                Name = "Flash",
                Material = mats.Glow,   // 임팩트 순간 판독용 → 어두운 팔레트도 플래시는 가산(밝게)
                Lifetime = new Vector2(0.14f, 0.18f) * lifeMul,
                Size = new Vector2(3.5f, 4f) * Mathf.Lerp(1f, 2.5f, power) * (p.Dark ? 0.6f : 1f),
                Color = MakeGradient(new[] { (0f, p.FlashHot), (0.4f, p.FlashMid), (1f, p.FlashMid) },
                                     new[] { (0f, 1f), (0.3f, 0.8f), (1f, 0f) }),
                SizeOverLife = Curve(0f, 0.6f, 1f, 1.3f),
                SortingOrder = 14,
                Count = new Vector2Int(1, 1),
            };

            if (p.Dark)
            {
                // 어두운 팔레트 전용: 먹물이 번지듯 퍼지는 어두운 덩어리. 밝은 보스/플래시 위에서 확실히 읽힘
                yield return new LayerSpec
                {
                    Name = "Void",
                    Material = mats.GlowAlpha,
                    Lifetime = new Vector2(0.3f, 0.4f) * lifeMul,
                    Size = new Vector2(2.2f, 2.8f) * Mathf.Lerp(1f, 2f, power),
                    Color = MakeGradient(new[] { (0f, p.Tail), (1f, p.Tail) },
                                         new[] { (0f, 0.75f), (0.4f, 0.55f), (1f, 0f) }),
                    SizeOverLife = Curve(0f, 0.5f, 1f, 1.4f),
                    SortingOrder = 9,
                    Count = new Vector2Int(1, 1),
                };
            }

            yield return new LayerSpec
            {
                Name = "Sparks",
                Material = Pick(mats.Spark, mats.SparkAlpha),
                Lifetime = new Vector2(0.18f, 0.38f) * lifeMul,
                Speed = new Vector2(14f, 30f) * speedMul,
                Size = new Vector2(0.12f, 0.22f) * (p.Dark ? 1.3f : 1f),
                Color = MakeGradient(new[] { (0f, p.Hot), (0.5f, p.Mid), (1f, p.Low) },
                                     new[] { (0f, 1f), (0.7f, 1f), (1f, 0f) }),
                SizeOverLife = Curve(0f, 1f, 1f, 0.2f),
                Drag = 6f,
                Stretch = true,
                ShapeRadius = 0.15f,
                SortingOrder = 13,
                Count = style == BurstStyle.Shard ? Counts(6, 20) : Counts(10, 36),
            };

            if (style == BurstStyle.Ember)
            {
                yield return new LayerSpec
                {
                    Name = "Embers",
                    Material = Pick(mats.Ember, mats.EmberAlpha),
                    Lifetime = new Vector2(0.6f, 1.4f) * lifeMul,
                    Speed = new Vector2(3f, 11f) * speedMul,
                    Size = new Vector2(0.1f, 0.26f),
                    Color = PaletteGradient(p),
                    SizeOverLife = Curve(0f, 1f, 1f, 0f),
                    Drag = 3f,
                    Gravity = -0.12f,
                    Noise = 0.8f,
                    RandomRotation = true,
                    ShapeRadius = 0.2f,
                    SortingOrder = 12,
                    MaxParticles = 1000,
                    Count = Counts(16, 64),
                };

                yield return new LayerSpec
                {
                    Name = "Rings",
                    Material = Pick(mats.Ring, mats.RingAlpha),
                    Lifetime = new Vector2(1.2f, 2.2f) * lifeMul,   // 길게 → 연타 시 화면에 누적
                    Speed = new Vector2(1.5f, 6f) * speedMul,
                    Size = new Vector2(0.18f, 0.42f),
                    Color = MakeGradient(new[] { (0f, p.Hot), (0.5f, p.Mid), (1f, p.Low) },
                                         new[] { (0f, 0f), (0.08f, 1f), (0.7f, 0.8f), (1f, 0f) }),
                    SizeOverLife = Curve(0f, 0.6f, 0.2f, 1.1f, 1f, 0.8f),
                    Drag = 2.5f,
                    Noise = 0.5f,
                    ShapeRadius = 0.3f,
                    SortingOrder = 11,
                    MaxParticles = 800,
                    Count = Counts(3, 22),
                };
            }
            else
            {
                // 큰 파편: 빠르게 튀어나와 회전하며 떨어진다
                yield return new LayerSpec
                {
                    Name = "Shards",
                    Material = Pick(mats.Shard, mats.ShardAlpha),
                    Lifetime = new Vector2(0.5f, 1.1f) * lifeMul,
                    Speed = new Vector2(6f, 16f) * speedMul,
                    Size = new Vector2(0.14f, 0.36f),
                    Color = MakeGradient(new[] { (0f, p.Hot), (0.35f, p.Mid), (1f, p.Low) },
                                         new[] { (0f, 1f), (0.75f, 1f), (1f, 0f) }),
                    SizeOverLife = Curve(0f, 1f, 1f, 0.7f),
                    Drag = 3.5f,
                    Gravity = 0.6f,
                    RandomRotation = true,
                    AngularVelocity = new Vector2(-720f, 720f),
                    SheetTiles = new Vector2Int(2, 2),
                    ShapeRadius = 0.2f,
                    SortingOrder = 12,
                    MaxParticles = 600,
                    Count = Counts(10, 40),
                };

                // 잔 조각: 느리게 오래 남아서 연타 시 누적 (Rings 역할)
                yield return new LayerSpec
                {
                    Name = "Chips",
                    Material = Pick(mats.Shard, mats.ShardAlpha),
                    Lifetime = new Vector2(0.9f, 1.8f) * lifeMul,
                    Speed = new Vector2(2.5f, 9f) * speedMul,
                    Size = new Vector2(0.05f, 0.12f),
                    Color = MakeGradient(new[] { (0f, p.Mid), (0.6f, p.Low), (1f, p.Tail) },
                                         new[] { (0f, 1f), (0.7f, 0.9f), (1f, 0f) }),
                    Drag = 2.5f,
                    Gravity = 0.3f,
                    Noise = 0.3f,
                    RandomRotation = true,
                    AngularVelocity = new Vector2(-540f, 540f),
                    SheetTiles = new Vector2Int(2, 2),
                    ShapeRadius = 0.3f,
                    SortingOrder = 11,
                    MaxParticles = 1000,
                    Count = Counts(12, 50),
                };
            }

            // 대비 강조 레이어: 밝은 팔레트엔 어두운 조각, 어두운 팔레트엔 밝은 티끌 → 어느 배경에서도 읽힘
            yield return new LayerSpec
            {
                Name = "Accent",
                Material = style == BurstStyle.Shard
                    ? (p.AccentAdditive ? mats.Shard : mats.ShardAlpha)
                    : (p.AccentAdditive ? mats.Ember : mats.Ash),
                Lifetime = new Vector2(0.5f, 1.1f) * lifeMul,
                Speed = new Vector2(5f, 12f) * speedMul,
                Size = new Vector2(0.06f, 0.16f),
                Color = MakeGradient(new[] { (0f, p.Accent), (1f, p.Accent) },
                                     new[] { (0f, 1f), (0.6f, 1f), (1f, 0f) }),
                SizeOverLife = Curve(0f, 1f, 1f, 0.6f),
                Drag = 4f,
                Gravity = 0.25f,
                RandomRotation = true,
                AngularVelocity = style == BurstStyle.Shard ? new Vector2(-600f, 600f) : Vector2.zero,
                SheetTiles = style == BurstStyle.Shard ? new Vector2Int(2, 2) : Vector2Int.zero,
                ShapeRadius = 0.15f,
                SortingOrder = 10,
                Count = Counts(2, 16),
            };
        }

        private static void BuildBossDissolvePrefab(Materials mats)
        {
            var root = new GameObject("FX_BossDissolve");

            var overlayGo = new GameObject("GlowOverlay");
            overlayGo.transform.SetParent(root.transform, false);
            var overlay = overlayGo.AddComponent<SpriteRenderer>();
            overlay.sharedMaterial = mats.GlowSprite;
            overlay.color = new Color(SpiritWhite.r, SpiritWhite.g, SpiritWhite.b, 0f);
            overlay.sortingOrder = 5;
            overlay.enabled = false;

            ParticleSystem glowEmbers = CreateSystem(root.transform, new LayerSpec
            {
                Name = "GlowEmbers",
                Material = mats.Ember,
                Lifetime = new Vector2(1f, 2f),
                Speed = new Vector2(0.2f, 0.8f),
                Size = new Vector2(0.08f, 0.2f),
                Color = FireGradient(),
                SizeOverLife = Curve(0f, 1f, 1f, 0f),
                Rise = new Vector2(0.6f, 1.8f),
                Noise = 0.6f,
                RandomRotation = true,
                SpriteShape = true,
                SortingOrder = 12,
                MaxParticles = 800,
            });

            // 소멸: 보스 모양 그대로 찍힌 뒤 → 위로 떠오르며 흩어짐 → 점점 성겨짐
            // 보스 본체는 영혼(푸른색)으로 흩어지고, 일부 불티(Cinders)가 섞여서 "불에 타 사라지는" 느낌을 낸다
            Gradient dissolveColor = SpiritGradient();

            // 처음엔 거의 안 흔들려서 보스 실루엣이 유지되고, 수명 후반으로 갈수록 크게 흩어진다
            AnimationCurve dissolveNoise = Curve(0f, 0.05f, 0.35f, 0.4f, 1f, 1f);

            var burstRoot = new GameObject("DissolveBurst");
            burstRoot.transform.SetParent(root.transform, false);

            ParticleSystem dots = CreateSystem(burstRoot.transform, new LayerSpec
            {
                Name = "Dots",
                Material = mats.Ember,
                Lifetime = new Vector2(2.5f, 4f),
                Speed = new Vector2(0f, 0.25f),
                Size = new Vector2(0.06f, 0.16f),
                Color = dissolveColor,
                SizeOverLife = Curve(0f, 1f, 1f, 0.4f),
                Rise = new Vector2(0.1f, 0.7f),
                Noise = 0.9f,
                NoiseFrequency = 0.6f,
                NoiseOverLife = dissolveNoise,
                RandomRotation = true,
                SpriteShape = true,
                SortingOrder = 13,
                MaxParticles = 2000,
            });

            ParticleSystem rings = CreateSystem(burstRoot.transform, new LayerSpec
            {
                Name = "Rings",
                Material = mats.Ring,
                Lifetime = new Vector2(2.5f, 4f),
                Speed = new Vector2(0f, 0.3f),
                Size = new Vector2(0.12f, 0.3f),
                Color = dissolveColor,
                SizeOverLife = Curve(0f, 1f, 1f, 0.6f),
                Rise = new Vector2(0.1f, 0.7f),
                Noise = 0.9f,
                NoiseFrequency = 0.6f,
                NoiseOverLife = dissolveNoise,
                SpriteShape = true,
                SortingOrder = 13,
                MaxParticles = 600,
            });

            ParticleSystem flash = CreateSystem(burstRoot.transform, new LayerSpec
            {
                Name = "Flash",
                Material = mats.Glow,
                Lifetime = new Vector2(0.4f, 0.4f),
                Size = new Vector2(5f, 5f),
                Color = MakeGradient(new[] { (0f, SpiritWhite), (1f, SpiritCyan) }, new[] { (0f, 1f), (1f, 0f) }),
                SizeOverLife = Curve(0f, 0.8f, 1f, 1.4f),
                SortingOrder = 14,
            });

            ParticleSystem cinders = CreateSystem(burstRoot.transform, new LayerSpec
            {
                Name = "Cinders",
                Material = mats.Ember,
                Lifetime = new Vector2(1.2f, 2.5f),
                Speed = new Vector2(0.2f, 1.2f),
                Size = new Vector2(0.06f, 0.15f),
                Color = FireGradient(),
                SizeOverLife = Curve(0f, 1f, 1f, 0f),
                Rise = new Vector2(0.4f, 1.4f),
                Noise = 0.7f,
                NoiseOverLife = dissolveNoise,
                RandomRotation = true,
                SpriteShape = true,
                SortingOrder = 13,
                MaxParticles = 400,
            });

            var burst = burstRoot.AddComponent<ImpactBurst>();
            burst.EditorSetLayers(new[]
            {
                new ImpactBurst.Layer(dots, 700, 700),
                new ImpactBurst.Layer(rings, 160, 160),
                new ImpactBurst.Layer(cinders, 140, 140),
                new ImpactBurst.Layer(flash, 1, 1),
            });

            // 산산조각 애니메이션 프레임마다 "지금 조각 모양"에서 흩날리는 영혼 가루
            ParticleSystem wisps = CreateSystem(root.transform, new LayerSpec
            {
                Name = "DeathWisps",
                Material = mats.Ember,
                Lifetime = new Vector2(1.2f, 2.4f),
                Speed = new Vector2(0.3f, 1.5f),
                Size = new Vector2(0.05f, 0.14f),
                Color = dissolveColor,
                SizeOverLife = Curve(0f, 1f, 1f, 0.3f),
                Drag = 1.5f,
                Rise = new Vector2(0.3f, 1.2f),
                Noise = 0.7f,
                NoiseFrequency = 0.6f,
                NoiseOverLife = dissolveNoise,
                RandomRotation = true,
                SpriteShape = true,
                SortingOrder = 13,
                MaxParticles = 1500,
            });

            root.AddComponent<BossDissolveEffect>()
                .EditorSetup(overlay, glowEmbers, burst, wisps, new[] { glowEmbers, dots, rings, cinders, wisps });

            SavePrefab(root);
        }

        // ───────────────────────── Particle System 생성 ─────────────────────────

        private static ParticleSystem CreateSystem(Transform parent, LayerSpec s)
        {
            var go = new GameObject(s.Name);
            go.transform.SetParent(parent, false);
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = ps.main;
            main.duration = 1f;
            main.loop = true;              // Rate over Time 으로 쓰는 시스템(GlowEmbers) 대비. Burst 는 Emit() 으로 직접 처리
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(s.Lifetime.x, s.Lifetime.y);
            main.startSpeed = new ParticleSystem.MinMaxCurve(s.Speed.x, s.Speed.y);
            main.startSize = new ParticleSystem.MinMaxCurve(s.Size.x, s.Size.y);
            main.startRotation = s.RandomRotation
                ? new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f)
                : new ParticleSystem.MinMaxCurve(0f);
            main.startColor = Color.white;
            main.gravityModifier = s.Gravity;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;   // 프리팹 스케일로 전체 크기 조절
            main.maxParticles = s.MaxParticles;

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = ps.shape;
            shape.enabled = true;
            if (s.SpriteShape)
            {
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
            }
            else
            {
                // Circle 은 로컬 XY 평면에서 방사 → 2D 화면 기준 사방으로 퍼짐
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = Mathf.Max(0.0001f, s.ShapeRadius);
                shape.radiusThickness = 1f;
                shape.arc = 360f;
            }

            ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
            col.enabled = s.Color != null;
            if (s.Color != null)
            {
                col.color = new ParticleSystem.MinMaxGradient(s.Color);
            }

            ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
            sol.enabled = s.SizeOverLife != null;
            if (s.SizeOverLife != null)
            {
                sol.size = new ParticleSystem.MinMaxCurve(1f, s.SizeOverLife);
            }

            if (s.Drag > 0f)
            {
                // 처음엔 빠르게 터지고 금방 감속 → "터지는" 느낌의 핵심
                ParticleSystem.LimitVelocityOverLifetimeModule limit = ps.limitVelocityOverLifetime;
                limit.enabled = true;
                limit.limit = 1000f;
                limit.drag = s.Drag;
                limit.multiplyDragByParticleSize = false;
                limit.multiplyDragByParticleVelocity = false;
            }

            if (s.Rise != Vector2.zero)
            {
                ParticleSystem.VelocityOverLifetimeModule vel = ps.velocityOverLifetime;
                vel.enabled = true;
                vel.space = ParticleSystemSimulationSpace.World;
                // x/y/z 는 같은 커브 모드여야 함
                vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
                vel.y = new ParticleSystem.MinMaxCurve(s.Rise.x, s.Rise.y);
                vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);
            }

            if (s.Noise > 0f)
            {
                ParticleSystem.NoiseModule noise = ps.noise;
                noise.enabled = true;
                noise.quality = ParticleSystemNoiseQuality.Medium;   // 2D 노이즈
                noise.strength = s.NoiseOverLife != null
                    ? new ParticleSystem.MinMaxCurve(s.Noise, s.NoiseOverLife)
                    : new ParticleSystem.MinMaxCurve(s.Noise);
                noise.frequency = s.NoiseFrequency;
                noise.scrollSpeed = 0.4f;
                noise.damping = true;
            }

            if (s.AngularVelocity != Vector2.zero)
            {
                ParticleSystem.RotationOverLifetimeModule rot = ps.rotationOverLifetime;
                rot.enabled = true;
                rot.z = new ParticleSystem.MinMaxCurve(s.AngularVelocity.x * Mathf.Deg2Rad, s.AngularVelocity.y * Mathf.Deg2Rad);
            }

            if (s.SheetTiles.x > 0 && s.SheetTiles.y > 0)
            {
                // 아틀라스에서 타일 1장을 랜덤으로 골라 고정 (애니메이션 없음)
                int tiles = s.SheetTiles.x * s.SheetTiles.y;
                ParticleSystem.TextureSheetAnimationModule sheet = ps.textureSheetAnimation;
                sheet.enabled = true;
                sheet.mode = ParticleSystemAnimationMode.Grid;
                sheet.numTilesX = s.SheetTiles.x;
                sheet.numTilesY = s.SheetTiles.y;
                sheet.animation = ParticleSystemAnimationType.WholeSheet;
                sheet.frameOverTime = new ParticleSystem.MinMaxCurve(0f);
                sheet.startFrame = new ParticleSystem.MinMaxCurve(0f, tiles - 0.001f);
                sheet.cycleCount = 1;
            }

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = s.Material;
            renderer.sortingOrder = s.SortingOrder;
            renderer.renderMode = s.Stretch ? ParticleSystemRenderMode.Stretch : ParticleSystemRenderMode.Billboard;
            if (s.Stretch)
            {
                renderer.velocityScale = s.StretchVelocityScale;
                renderer.lengthScale = 1f;
            }

            return ps;
        }

        // ───────────────────────── Demo Scene ─────────────────────────

        [MenuItem("Tools/Ending FX/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            var hitPrefab = LoadPrefab<ImpactBurst>("FX_HitBurst");
            var finalPrefab = LoadPrefab<ImpactBurst>("FX_FinalBurst");
            var slashPrefab = LoadPrefab<FireSlashEffect>("FX_FireSlash");
            var dissolvePrefab = LoadPrefab<BossDissolveEffect>("FX_BossDissolve");
            if (hitPrefab == null || finalPrefab == null || slashPrefab == null || dissolvePrefab == null)
            {
                Debug.LogError("[EndingFX] 먼저 Tools/Ending FX/Build Assets 를 실행해줘.");
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 3.5f;   // 원본 엔딩처럼 보스 클로즈업
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.035f, 0.06f);
            cam.allowHDR = true;
            camGo.transform.position = new Vector3(0f, 0f, -10f);

            BossClips bossClips = BuildBossClips();
            if (bossClips.Damage.FrameCount == 0)
            {
                Debug.LogError($"[EndingFX] 보스 프레임이 없음 → {BossDamageDir}");
                return;
            }

            var boss = new GameObject("Boss");
            var bossRenderer = boss.AddComponent<SpriteRenderer>();
            bossRenderer.sprite = bossClips.Damage.GetFrame(0);
            bossRenderer.sortingOrder = 0;

            // 피격 번쩍임: 보스와 같은 스프라이트를 가산으로 한 번 더 그림
            var hitFlashGo = new GameObject("HitFlash");
            hitFlashGo.transform.SetParent(boss.transform, false);
            var hitFlash = hitFlashGo.AddComponent<SpriteRenderer>();
            hitFlash.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialDir}/M_FX_GlowSprite.mat");
            hitFlash.color = new Color(1f, 1f, 1f, 0f);
            hitFlash.sortingOrder = 4;
            hitFlash.enabled = false;

            var bossAnimator = boss.AddComponent<BossEndingAnimator>();
            bossAnimator.EditorSetup(bossClips.Intro, bossClips.Damage, bossClips.Death, hitFlash);

            var dissolve = InstantiatePrefab(dissolvePrefab, boss.transform);
            var hit = InstantiatePrefab(hitPrefab, null);
            var final = InstantiatePrefab(finalPrefab, null);

            var poolGo = new GameObject("FireSlashPool");
            var pool = poolGo.AddComponent<FireSlashPool>();
            SetRef(pool, "prefab", slashPrefab);

            var demo = new GameObject("EndingFxDemo").AddComponent<EndingFxDemo>();
            SetRef(demo, "slashPool", pool);
            SetRef(demo, "hitBurst", hit);
            SetRef(demo, "finalBurst", final);
            SetRef(demo, "bossDissolve", dissolve);
            SetRef(demo, "bossAnimator", bossAnimator);
            SetRef(demo, "target", boss.transform);

            EnsureFolder(Root + "/Scenes");
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), Root + "/Scenes/EndingFxDemo.unity");
        }

        // ───────────────────────── Helpers ─────────────────────────

        private static Gradient SpiritGradient() => MakeGradient(
            new[] { (0f, SpiritWhite), (0.3f, SpiritCyan), (0.65f, SpiritBlue), (1f, SpiritDeep) },
            new[] { (0f, 1f), (0.35f, 1f), (1f, 0f) });

        private static Gradient FireGradient() => PaletteGradient(BurstPalette.Fire);

        private static Gradient PaletteGradient(BurstPalette p) => MakeGradient(
            new[] { (0f, p.Hot), (0.25f, p.Mid), (0.6f, p.Low), (1f, p.Tail) },
            new[] { (0f, 1f), (0.6f, 0.9f), (1f, 0f) });

        private static Gradient MakeGradient((float time, Color color)[] colors, (float time, float alpha)[] alphas)
        {
            var g = new Gradient();
            g.SetKeys(
                colors.Select(c => new GradientColorKey(c.color, c.time)).ToArray(),
                alphas.Select(a => new GradientAlphaKey(a.alpha, a.time)).ToArray());
            return g;
        }

        /// <summary>(time, value) 쌍으로 커브 생성. 예: Curve(0, 1, 1, 0)</summary>
        private static AnimationCurve Curve(params float[] timeValuePairs)
        {
            var keys = new Keyframe[timeValuePairs.Length / 2];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = new Keyframe(timeValuePairs[i * 2], timeValuePairs[i * 2 + 1]);
            }

            return new AnimationCurve(keys);
        }

        private static void SavePrefab(GameObject root)
        {
            EnsureFolder(PrefabDir);
            PrefabUtility.SaveAsPrefabAsset(root, $"{PrefabDir}/{root.name}.prefab");
            Object.DestroyImmediate(root);
        }

        private static T LoadPrefab<T>(string name) where T : Component
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{name}.prefab");
            return go != null ? go.GetComponent<T>() : null;
        }

        /// <summary>프리팹 연결을 유지한 채 씬에 배치하고 같은 타입의 컴포넌트를 반환.</summary>
        private static T InstantiatePrefab<T>(T prefabComponent, Transform parent) where T : Component
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabComponent.gameObject, parent);
            return instance.GetComponent<T>();
        }

        private static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static IEnumerable<string> FindAssetPaths(string filter, string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                return Enumerable.Empty<string>();
            }

            return AssetDatabase.FindAssets(filter, new[] { folder }).Select(AssetDatabase.GUIDToAssetPath).Distinct();
        }

        /// <summary>
        /// PNG 헤더(IHDR)에서 원본 가로 해상도를 읽는다. Max Size 로 축소되기 전 값이 필요해서
        /// 임포트된 Texture2D 가 아니라 파일을 직접 읽는다.
        /// </summary>
        private static int ReadPngWidth(string assetPath)
        {
            using (var stream = File.OpenRead(assetPath))
            {
                var header = new byte[24];
                if (stream.Read(header, 0, header.Length) < header.Length)
                {
                    throw new IOException($"PNG 헤더를 읽을 수 없음: {assetPath}");
                }

                return (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
