# GuidePanel FrostSlate sprite v01

2026-10-13. Sample 1 copied without image resampling to [sprite](../../../Assets/04.Sprites/UI/GuidePanel/GuidePanel_FrostSlate_v01.png).

- Source PNG: 2058 × 764; transparent exterior, opaque center.
- Sprite Single, Full Rect, center pivot, PPU 6.
- Native local size: 343 × 127.333. PPU chosen for MainTestScene's existing SpriteRenderer panels with local sizes in the hundreds and parent Scale 0.02–0.025; not a general world-sprite PPU standard.
- 9-slice border pixels: left 240, bottom 190, right 300, top 140. Fixed border totals: width 90, height 55 local units.
- Max Size 4096 retains source dimensions (does not upscale to 4096). Uncompressed, Bilinear, Clamp, no mipmaps.

## Apply to existing GuidePanel

Assign sprite to SpriteRenderer. Set Color to white RGBA (1,1,1,1), otherwise the existing dark tint obscures the artwork. Keep existing material and sorting. Use Draw Mode Sliced; retain existing SpriteRenderer Size, usually 340 × 92, with 340 × 115 and 520 × 220 variants. RectTransform Size Delta alone does not resize this SpriteRenderer.

At parent Scale 0.02, Size 340 × 92 gives 6.8 × 1.84 world units before any other ancestor scale. Native aspect ratio is different; Sliced adapts the quiet center and retains corner dimensions. Existing nonuniform Y scale on the large movement panel still compresses its art vertically.

PNG dimensions, alpha samples, copy hash and importer file checked. Scene unchanged. Unity import and in-game rendering not verified in this pass.
