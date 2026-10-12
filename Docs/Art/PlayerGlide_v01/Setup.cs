using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;

public static class GlideAssetSetup
{
    public static string Main()
    {
        const string path = "Assets/04.Sprites/Player/Glide_v01/Player_Glide_v01.png";
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 200;
        importer.maxTextureSize = 4096;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        var edit = provider.GetDataProvider<ISpriteFrameEditCapability>();
        if (edit == null) throw new Exception("Sprite edit capabilities unavailable");
        var caps = edit.GetEditCapability();
        if (!caps.HasCapability(EEditCapability.CreateAndDeleteSprite)
            || !caps.HasCapability(EEditCapability.EditPivot)) throw new Exception("Slicing/pivot unsupported");
        var pixels = new Texture2D(2, 2);
        pixels.LoadImage(File.ReadAllBytes(path));
        int w = pixels.width / 4, h = pixels.height / 2;
        var old = provider.GetSpriteRects();
        var rects = new SpriteRect[8];
        for (int i = 0; i < 8; i++)
        {
            int left = i % 4 * w, bottom = (1 - i / 4) * h;
            // Align the pale face rather than the moving canopy to the existing Fall pose.
            float sx = 0, sy = 0, count = 0;
            for (int y = (int)(h * .40f); y < h * .60f; y++)
            for (int x = (int)(w * .35f); x < w * .78f; x++)
            {
                var c = pixels.GetPixel(left + x, bottom + y);
                if (c.a < .5f || c.r < .85f || c.g < .65f || c.b < .35f) continue;
                sx += x; sy += y; count++;
            }
            var name = "Glide_" + i.ToString("00");
            var previous = old.FirstOrDefault(r => r.name == name);
            rects[i] = new SpriteRect {
                name = name, rect = new Rect(left, bottom, w, h),
                alignment = SpriteAlignment.Custom,
                pivot = count > 0 ? new Vector2((sx / count - 21f) / w, (sy / count - 78f) / h) : new Vector2(.56f, .33f),
                spriteID = previous != null ? previous.spriteID : GUID.Generate()
            };
        }
        provider.SetSpriteRects(rects);
        var names = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        if (names == null) throw new Exception("Stable sprite IDs unsupported");
        names.SetNameFileIdPairs(rects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)));
        provider.Apply(); importer.SaveAndReimport();
        UnityEngine.Object.DestroyImmediate(pixels);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();
        if (sprites.Length != 8) throw new Exception("Expected eight glide sprites");
        const string clipPath = "Assets/05.Animation/player/Glide.anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
        clip.name = "Glide"; clip.frameRate = 12;
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var keys = new ObjectReferenceKeyframe[9];
        for (int i = 0; i <= 8; i++) keys[i] = new ObjectReferenceKeyframe { time = i / 12f, value = sprites[i % 8] };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/05.Animation/player/Player.controller");
        var machine = controller.layers[0].stateMachine;
        var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Glide");
        if (state == null) state = machine.AddState("Glide", new Vector3(900, 400));
        state.motion = clip; EditorUtility.SetDirty(controller); EditorUtility.SetDirty(clip);
        const string prefabPath = "Assets/03.Prefabs/Player/Player.prefab";
        var root = PrefabUtility.LoadPrefabContents(prefabPath);
        try {
            if (root.GetComponent<PlayerGlide>() == null) root.AddComponent<PlayerGlide>();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        } finally { PrefabUtility.UnloadPrefabContents(root); }
        AssetDatabase.SaveAssets();
        return $"Glide setup: {w * 4}x{h * 2}, 8 frames {w}x{h}, PPU 200, 12fps; prefab connected";
    }
}
