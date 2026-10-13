# Slime B Move v01 — 2026-10-13

## Deliverables

- [Multi-sprite](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Move/SlimeB_Move_Sheet_v01.png): transparent RGBA, 4608×512, 6 cells of 768×512, left-facing crawl. Single-frame PNGs 00–05 retained alongside it.
- [Animation clip](../../../Assets/05.Animation/NormalMonster/SlimeB_Move_v01.anim): 10fps, duration 0.6s, Loop Time, sprite-only keys on root SpriteRenderer. No Transform curves or world movement.
- [Interactive preview](Move_Preview.html) includes step, speed and Idle comparison. [Review sheet](Move_Review.png), [measurements](Move_Metrics.json), [packing script](PackMoveSheet.ps1), [Image Gen source](Move_generated.png).

All slices use PPU 200, custom normalized pivot (0.5,0.125), pixel pivot (384,64). Default/Standalone Max Size 8192, NPOT None, no mipmaps, Bilinear, Full Rect, Standalone RGBA32. The strip is 4608 pixels wide and must not be imported with Max Size 4096.

## Poses / measured alpha bounds

| Frame | Pose | X | Y from top | Width | Height | Bottom exclusive |
|---|---|---:|---:|---:|---:|---:|
| 00 | Approved neutral / Idle connection | 159 | 168 | 450 | 280 | 448 |
| 01 | Compression | 144 | 190 | 480 | 258 | 448 |
| 02 | Front extension | 125 | 201 | 518 | 247 | 448 |
| 03 | Body follows | 139 | 178 | 490 | 270 | 448 |
| 04 | Rear gathers / recovery | 164 | 146 | 440 | 302 | 448 |
| 05 | Near-neutral return | 159 | 158 | 450 | 290 | 448 |

Bounds measured at alpha >=32. Height variation relative to neutral -11.8% to +7.9%, width growth about15.1%. Final→first changes height by10px and retains the450px width and baseline. Frame00 is byte-identical to the approved Idle00 PNG (SHA256 verified). Generated frames1–5 were packed to the target geometry; source output was1881×836 in3×2 layout, not the final game strip.

## Validation

Unity MCP verified imported Texture2D4608×512 and six Sprite.rect768×512, all pivots(384,64), PPU200. Sampled actual clip on a temporary preview-scene SpriteRenderer+Animator: Idle00→Move00→01→02→03→04→05→00→01→00→Idle00. Root Transform remained(0,0,0), and clip has zero float curves. Preview scene closed afterwards. Composited contact sheet visually inspected. Existing user Animator Controller, Idle clip and active MinsuScene were not edited. This validates sprites and clip sampling; it does not validate a runtime Animator transition, collision or actual travel-speed matching.

## Generation

Built-in Image Gen edit using the approved Idle00 PNG. Prompt:

Edit target: the attached APPROVED B snow slime idle sprite. Make a SIX FRAME 2D ground-crawling MOVE animation sprite sheet of this exact character. Six cells in a precise 3 columns x 2 rows grid, chronological left to right top row then bottom row. Requested canvas 2304x1024 with each cell 768x512. True transparent alpha. Identical fixed virtual camera and character facing slightly LEFT in every frame with both eyes visible, same painterly details, offwhite scalloped snow cap, blue-gray jelly, cyan eyes, tiny horizontal mouth, SINGLE short crown ice crystal. No redesign, no blinking, no changing perspective, no extra eyes/crystals. Animation IN PLACE with center anchored and identical ground contact baseline in every cell, 64 pixels above each cell bottom. NEVER hop or float. Six poses: 0 neutral matching approved sprite; 1 low compression preparing to crawl; 2 the LEFT front edge extends and leans LEFT as rear remains gathered, low wide body; 3 body mass catches up leftward while rear trails slightly; 4 rear draws in, slightly narrower taller recovery; 5 nearly neutral, bridging gently back to frame0. Motion is mild asymmetric jelly deformation, not whole-character translation. Keep original face and snow landmarks attached and consistent. Maximum height variation -12% to +8% of neutral and width growth at most15%. Crown crystal keeps rigid shape/size, moves/tilts only gently with cap. Neutral art approximately450x280px including crystal; fullest stretch ~518x247. Entire character inside each cell with generous transparent margins. No texts, labels, borders, ground plane, shadow, glow, trails, loose particles or background. Only six isolated sprite frames, one per cell.
