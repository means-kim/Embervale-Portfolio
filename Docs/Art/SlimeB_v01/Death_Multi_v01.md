# Slime B Death v01 — 2026-10-13

- [Sheet](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Death/SlimeB_Death_Sheet_v01.png): transparent RGBA3072×512, four768×512 cells. Individual PNGs00–03 retained alongside.
- [Clip](../../../Assets/05.Animation/NormalMonster/SlimeB_Death_v01.anim): 8fps,0.5seconds, Loop Time off. Holds final remnant. SpriteRenderer sprite keys only; no Transform or alpha curves.
- [Interactive preview](Death_Preview.html), [review](Death_Review.png), [metrics](Death_Metrics.json), [packing script](PackDeathSheet.ps1), [generated source](Death_generated.png).

PPU200, normalized pivot(0.5,0.125), pixel pivot(384,64), Full Rect, Bilinear, no mipmaps, NPOT None, Default/Standalone Max Size4096, Standalone RGBA32.

## Geometry

Alpha>=32 bounds, image coordinates from top-left:

|Frame|Pose|X|Y|Width|Height|Bottom exclusive|
|---|---|---:|---:|---:|---:|---:|
|00|Weakening, dim half-closed eyes|154|198|460|250|448|
|01|Collapse, eyes closed, crystal tipped|129|268|510|180|448|
|02|Faceless snow / fading blue body|194|343|380|105|448|
|03|Small snow heap / two crystal fragments|279|363|210|85|448|

Monotonically decreasing height, identical contact line,64px transparent bottom margin. Final pile packed to210×85 to preserve generated shard proportions. Source1536×1024 in2×2 arrangement; final strip3072×512.

## Verification / scope

Unity MCP verified imported Texture2D3072×512, all four Sprite.rect768×512, pivot(384,64), PPU200. Sampled actual clip on temporary preview-scene SpriteRenderer+Animator at0.01/0.14/0.26/0.39/0.51/1.0seconds:00/01/02/03/03/03. No root position changes, no float curves, Loop Time false. Preview scene closed. Composited contact sheet visually inspected. Existing controller, scenes, gameplay code and previous clips were not edited. Death priority, collision disable, fade-out, pooling/destruction and runtime Animator transitions remain unimplemented.

## Image Gen prompt

Built-in Image Gen edit using approved Idle00 as reference:

Edit the attached approved B snow slime sprite into FOUR consecutive DEATH animation frames, presented in a precise2x2 grid (top left, top right, bottom left, bottom right). Same painted offwhite scalloped snow cap, blue-gray jelly body, ONE small crown ice crystal, left-facing three-quarter view. True transparent alpha. A gentle fantasy game creature dissipating into snow, no gore. Frame0 loses strength: mildly drooping body, cyan eyes half-closed and light fading. Frame1 collapses: eyes fully closed/dark, body flattening sideways with snow cap sinking, single rigid crystal tilts with cap. Frame2 dissipates: much smaller low pile of snow, remaining small amount of blue jelly almost gone, no face or eye glow, original crystal now resting at shallow angle. Frame3 final remnant: compact very low little heap of snow with TWO small blue fragments from the original crystal, NO living face, no jelly silhouette, no eyes, no mouth. Monotonically decreasing height; never rebound, never return to idle, no jumping or airborne particles. Each pose centered on fixed ground contact at same cell baseline; generous transparent margin. Suggested cell768x512, full sheet1536x1024. Pose sizes approx460x250,510x180,380x105,280x65, always contact64px above cell bottom. Keep original palette and rendering coherent. No background, floor plane, cast shadow, halo, motion lines, labels or drawn grid; four isolated frames only. Snow debris stays in the compact mound, no scattered distant particles.
