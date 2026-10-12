# Embervale app icon v01

Created 2026-10-12 with built-in image_gen using the existing Embervale title face as style and identity reference.

Assets: ../../Assets/04.Sprites/UI/AppIcon_Embervale_v01/

- Source: 1254 x 1254 generated PNG.
- Applied: AppIcon_Embervale_1024_v01.png, 1024 x 1024, opaque background.
- Small preview: AppIcon_Embervale_32_v01.png, 32 x 32.

Exports use System.Drawing HighQualityBicubic with TileFlipXY edge handling. Corners verified alpha 255. The applied texture was imported in Unity 6000.3.22f1 as Default, uncompressed, no mipmaps, max size 1024.

Applied through the connected Unity Editor to the Default Icon in both ProjectSettings/ProjectSettings.asset and the WindowsTest build profile. GUID: 2a4f4925387d59241b839efc5b830938. Editor readback and saved references verified. No executable was built; Windows executable/taskbar display remains unverified. Existing in-memory cursor assignment in WindowsTest was preserved when Unity serialized its Player Settings.

Unity serialization writes an empty default build-target name with trailing whitespace. This is the Editor-generated representation.

## Exact generation prompt

Use case: stylized-concept. Asset type: finished square desktop application icon for a dark fantasy 2D game. Reference is the existing game title; use the small ivory flame spirit face inside its letter b as the main character identity, and its gold-orange painted flame style. Create ONE app icon, a large centered ivory-gold flame spirit head with two unmistakable dark almond-shaped eyes, softly tapering mask-like face, orange flames rising above and curving around it. Bold simple silhouette, large face readable at 16,32,48 pixel sizes, minimal texture. Rich hand-painted fantasy illustration, cream white facial center, warm gold to orange and dark red flame perimeter. Deep charcoal blue solid square background with subtle radial warm illumination behind character, fully opaque edge-to-edge square, no rounded icon corners baked in. Subject occupies 80 percent of canvas, safe margin around flame tips. No text, no letter b, no logo lettering, no border, no tiny particles, no scenery, no weapons, no extra characters. Output one square image.
