# B slime Idle multi-sprite — 2026-10-13

- [Sprite sheet](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle/SlimeB_Idle_Sheet_v01.png): RGBA 3072×512, 4 cells of 768×512, Multiple, PPU 200, pixel pivot (384,64), normalized pivot (0.5,0.125).
- Default and Standalone Max Size 4096, NPOT None, no mipmaps, bilinear, uncompressed RGBA32 on Standalone.
- [Animation](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle/SlimeB_Idle_v01.anim): 6fps, 4 sprite keys, 0.666667 seconds, Loop Time on. Binds SpriteRenderer on the animated root. No controller or scene objects created.
- Separate frames retained next to the sheet. Frame 00's existing file and metadata were not edited. Frames 01 and 03 are identical.
- [Interactive preview](Idle_Preview.html), [contact sheet](Idle_Sheet_Review.png), [generated source](Idle_Breathing_generated.png), [packing script](PackIdleSheet.ps1).

## Measured geometry

Alpha >=32 bounds, image coordinates from top left:

| Frame | X | Y | Width | Height | Bottom exclusive |
|---|---:|---:|---:|---:|---:|
| 00 | 159 | 168 | 450 | 280 | 448 |
| 01 | 165 | 151 | 438 | 297 | 448 |
| 02 | 171 | 134 | 426 | 314 | 448 |
| 03 | 165 | 151 | 438 | 297 | 448 |

Maximum height increase: 34/280 = 12.14%. All pivots and contact baselines match; 64px bottom margin. The generated poses were packed to the target breathing dimensions; source sheet alone did not meet the requested dimensions. The neutral frame uses the approved PNG, not the regenerated neutral candidate.

## Verification

Unity MCP confirmed actual imported Texture2D 3072×512, all 4 Sprite.rect values 768×512, all Sprite.pivot values (384,64), PPU 200. Sampled the actual clip on a temporary preview-scene SpriteRenderer and Animator: times 0.01 / 0.18 / 0.35 / 0.52 / 0.68 produced 00 / 01 / 02 / 03 / 00. Preview scene closed afterwards. Frames 01 and 03 SHA256 match; last-to-first transition is the reverse of first-to-second, with the same 17px height difference as all other transitions. Contact sheet visually inspected on dark background. This is asset/clip validation, not MainTestScene gameplay validation.

## Image Gen prompt

Create a production sprite sheet from the attached approved snow slime idle sprite. Edit target is this exact single sprite, not a new design. Four equal-size horizontal cells, one row, total canvas requested 3072x512 with 768x512 cells. TRUE transparent alpha outside each character. Four frames numbered in description only, NO drawn text or guides. Frame 0 EXACT same approved neutral pose and artwork. Frame 1 halfway into slow inhale, body 6% taller and subtly narrower. Frame 2 inhalation peak 12% taller than neutral and subtly narrower. Frame 3 EXACT copy of frame 1 so sequence 0-1-2-1 smoothly loops back to 0. Keep contact baseline identical in all frames at 64 px above bottom of cell, centered horizontally. Neutral creature including crystal is 280px high within cell; peak about314px. Preserve identical blue-gray colors, painted ice facets, snow lobe outlines, cyan eyes, tiny horizontal mouth, SINGLE small crystal. NO blinking, no facial expression change, no perspective change, no new details. Snow and face gently follow breathing. The ice crystal translates upward with head but remains rigid and same size. Small amplitude soft breathing, never jump, never lift ground contact. Match every frame's light, texture and features tightly as one character. Generous transparent margins in each cell. No background, shadow, glow, floor or particles.
