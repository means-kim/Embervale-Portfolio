# 상승 눈가루 v02

- 사용자 피드백: 다섯 갈래로 규칙적으로 퍼지는 형태 수정.
- 내장 image_gen으로 기존 v01을 참조해 불규칙한 높낮이/밀도의 흩어진 눈가루로 변경. 지면 가루눈과 기존 v01은 보존.
- 새 파일: ../../../Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamUpSpray_v02.png
- 실제1536×1024 RGBA, Unity MaxSize1024, Sprite Single/PPU100/Pivot(0.5,0.055), alpha transparency, Clamp, mipmap off, Uncompressed.
- 정지 이미지 형태 확인. v02의 Unity 합성/애니메이션 검증은 아직 하지 않았다. 씬/프리팹/스크립트와 기존 참조를 수정하지 않음. 사용자는 상승 효과의 Sprite를 v02로 교체하면 된다. 이전 합성은 v01 결과이므로 v02 검증본으로 취급하지 않는다.

## 실제 프롬프트

Edit this 1536x1024 RGBA boss snow impact VFX sprite. Main change: REMOVE the five discrete evenly spaced fountain branches completely. Replace them with ONE organic irregular expanding cloud of airborne snow powder rising from a compact impact origin at x50%, y94.5%. No identifiable countable columns, no fan spokes, no central vertical jet, no five-lobed silhouette, no symmetry, no long straight spray trails. Snow is torn into many short scattered wisps and small granular clusters with uneven heights, one side slightly fuller, natural chaotic upward/outward momentum. Broken translucent patches and transparent holes through the middle keep the boss readable; avoid a dense solid cloud or smoke mushroom. Maintain pale cold blue-gray snow palette, fine texture, soft alpha feathering, fully transparent background, original canvas size and impact origin. Contain all snow inside margins, highest sparse flecks near y15%. No scenery, no character, no floor, no glow, no text or border. Single static isolated particle-effect sprite, not a sequence. Original is the EDIT TARGET for color, scale and origin only; its five-prong geometry is explicitly rejected.
