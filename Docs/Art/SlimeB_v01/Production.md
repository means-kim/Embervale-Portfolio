# Slime B Idle 00

Built-in Image Gen; approved B from [comparison](Design_ABC.png). Source: [generated PNG](Idle_00_generated.png).

Final: [Idle 00](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle/SlimeB_Idle_00.png), RGBA 768x512, PPU 200, custom pivot (0.5,0.125). Visible bounds approximately 450x280; original proportions preserved instead of stretching to the proposed 480–520 width. Contact baseline: y=448 from top; bottom margin 64. Suggested uniform scene scale 1.5, not applied. No scene changes.

Generated at 1536x1024. Alpha compositing confirmed outside halo is transparent. Alpha >=32 silhouette bounds used for proportional cell packing, with generated alpha retained. A later cleanup generation was not selected. Unity asset import succeeded; scene placement and animation remain untested.

## Generation prompt

Edit the supplied design sheet into ONE isolated sprite: only the CENTER character labeled B. Preserve B's exact approved character design, low wide squishy blue-gray jelly body, broad white snow cap with scalloped lobes, ONE small pale blue crystal, two cyan eyes and tiny flat mouth, same facing direction and painterly rendering. This is neutral Idle frame 00, no motion, no redesign. Remove A and C, all text, all background, and the floor/cast shadow completely. True transparent alpha background, clean edges. Output a 768x512 pixel canvas. Place the entire B character including crystal in approximately x=144..624 and y=168..448 (image coordinates from top left), height about 280 pixels; its bottom contact line is y=448, horizontal contact center x=384. Keep 64 pixels completely transparent below the contact line. This gives normalized Unity pivot (0.5,0.125) measured from bottom left. Preserve original aspect ratio, no stretching. All other canvas space transparent; no labels, ruler, guides, glow outside character, checkerboard drawing, scenery or extra objects.

## Import correction — 2026-10-13

Original PNG was 768x512, but copied Default/Standalone Max Size 512 reduced the imported texture and Sprite.rect to 512x341. Corrected both limits to 1024. Live Unity MCP verified texture 768x512, Sprite.rect 768x512, pixel pivot (384,64), and PPU 200 after reimport. Preserved current NormalMonster folder and asset GUID. No scene edits.

