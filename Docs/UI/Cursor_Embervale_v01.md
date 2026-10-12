# Embervale mouse cursor v01

Generated 2026-10-12 with the built-in image_gen tool. Existing Embervale title was opened and supplied as a style reference. No scene, code, or Player Settings were changed.

Assets: ../../Assets/04.Sprites/UI/Cursor_Embervale_v01/

- Cursor_Embervale_Source_v01.png: actual generated size 1254 x 1254, RGBA.
- Cursor_Embervale_32_v01.png: 32 x 32, suggested hotspot (4, 3).
- Cursor_Embervale_64_v01.png: 64 x 64, suggested hotspot (7, 5).

The two small exports were resampled with System.Drawing HighQualityBicubic. Transparent alpha was preserved. Suggested hotspots are the first pixel with alpha >= 128 near the upper-left arrow tip, measured from the top-left. Final click alignment should be checked in a build.

Import metadata: Cursor texture type, no mipmaps, input alpha, alpha transparency, clamp, bilinear, uncompressed, readable. The Editor's actual import and runtime cursor display have not been verified.

Apply before building: Edit > Project Settings > Player > Default Cursor, choose the 32 px texture, set Cursor Hotspot to X=4, Y=3. The 64 px export is an optional larger alternative with hotspot X=7, Y=5. If a build profile overrides Player Settings, verify its Default Cursor too. Check pointer size and clicking in the built application.

## Exact generation prompt

Use case: stylized-concept. Asset type: single usable mouse cursor texture for a Unity dark fantasy game. Generate ONE isolated mouse pointer on genuinely transparent background, square canvas. Reference image is style reference only: existing Embervale title uses hand painted ivory gold with restrained orange flame edges. Design a crisp standard arrow mouse pointer pointing toward UPPER LEFT, unmistakable sharp click tip, ivory gold filled triangular arrowhead, dark brown charcoal thin contrasting border, a short diagonal stem toward lower right ending in two small elegant orange ember flame tongues. Strong clean silhouette readable at 32x32 and 64x64 pixels. Compact simple painted fantasy shape, minimal interior detail, subtle gold bevel, no floating particles, no smoke, no diffuse glow, no cast shadow, no face, no text, no sheet of alternatives. Pointer fills about 80% of square canvas with small transparent padding. Tip at upper left of object. Keep alpha transparency outside silhouette.
