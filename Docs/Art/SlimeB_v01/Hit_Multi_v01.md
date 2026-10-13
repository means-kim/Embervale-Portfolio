# Slime B Hit v01 — 2026-10-13

- [Sheet](../../../Assets/04.Sprites/NormalMonster/SlimeB_v01/Hit/SlimeB_Hit_Sheet_v01.png): 2304×512 transparent RGBA, three 768×512 cells; individual PNGs retained alongside.
- [Clip](../../../Assets/05.Animation/NormalMonster/SlimeB_Hit_v01.anim): 12fps, 0.25 seconds, Loop Time off, root SpriteRenderer sprite keys only. Final frame holds after completion.
- [Interactive preview](Hit_Preview.html), [review image](Hit_Review.png), [measurements](Hit_Metrics.json), [packing script](PackHitSheet.ps1), [generated source](Hit_generated.png).

PPU200; normalized pivot(0.5,0.125), pixel pivot(384,64); Full Rect, Bilinear, NPOT None, no mipmaps, default/Standalone Max Size4096, Standalone RGBA32.

## Geometry

Alpha>=32, coordinates from top left:

| Frame | Pose | X | Y | Width | Height | Bottom exclusive |
|---|---|---:|---:|---:|---:|---:|
|00|Impact / eyes closed|125|224|518|224|448|
|01|Recoil / eyes closed|177|140|414|308|448|
|02|Recovery / approved Idle00|159|168|450|280|448|

Impact height -20%, recoil +10%, relative to280px neutral. All contact baselines448 from top with64px transparent bottom margin. Recovery PNG SHA256 matches Idle00. Original single crystal, snow cap and blue jelly design retained; generated hit poses packed to target dimensions. Source generated1881×836; final sheet is2304×512.

## Verification and scope

Unity MCP verified actual Texture2D2304×512, three Sprite.rect768×512, pivot(384,64), PPU200. Actual clip sampled in a temporary preview scene at0.01 /0.09 /0.18 /0.249 /0.3s gave00 /01 /02 /02 /02. Root Transform unchanged; zero float curves. Loop Time false, 12fps, duration0.25s. Preview scene closed. Composited contact sheet inspected. Existing Animator Controller, other clips, scenes and damage code untouched. Animator transitions, damage, repeated-hit policy, death priority and knockback remain unimplemented.

## Generation prompt

Built-in Image Gen edit of approved Idle00 PNG:

Edit this approved snow slime B into a THREE-frame HIT reaction sprite sheet. Exact same character identity, painterly snow lobes and blue-gray jelly, ONE small rigid ice crystal on crown, slight left three-quarter view. Transparent alpha. Three isolated frames evenly spaced left-to-right on identical cells, no text, no ground, no effects, no glow or particles. Frame 0 IMPACT: both eyes squeezed tightly shut into clear dark curved eyelids, tiny tense mouth, body flattened with height 20% below neutral and width 15% wider. Frame 1 RECOIL: body rebounds upward 10% taller and 8% narrower than neutral, eyelids still shut or only slightly reopening, not a happy expression. Frame 2 RECOVER: original neutral idle pose with both original cyan eyes fully open and original tiny flat mouth. Keep the B snow cap design and facial landmarks consistent; crystal rigid, just follows cap vertically with tiny tilt, never elongated rubber. Same ground contact baseline and horizontal contact center in all three cells; do not lift or jump, do not displace whole character sideways. Suggested sheet2304x512, cells768x512, contact y448 from top (64px transparent margin below). Neutral art450x280px, impact518x224px, recoil414x308px. This is short restrained damage feedback, NOT death, shattering, attacking, melting or turning. Preserve lighting, textures and silhouette family from reference. Keep all characters inside own cell and background truly transparent.
