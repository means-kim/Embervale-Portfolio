# 시작맵 최후방 배경 v02 — 밝기 수정

2026-09-22. 사용자 요청: “더 어둡게 해줘”.

- 결과: [BG_Start_Far_v02.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png). v01과 해당 .meta는 보존했다.
- 내장 image_gen 편집 사용. v01을 편집 대상으로, Bg_Concept를 색감·분위기 참조로 전달했다.
- 하늘·산·안개의 밝기를 낮추고 청색 계열과 달빛을 유지했다.
- 생성 결과를 시각적으로 확인했다. 주요 달·구름·산 배치는 유지되어 보이나, 생성 편집이므로 픽셀 단위의 형상 동일성을 검증한 것은 아니다.
- 수정 결과 제시 후 사용자가 “그래 다음”으로 후속 진행을 요청했다. v02를 확인받은 아트 기준으로 사용한다. 이웃 구간 접합·레이어 합성·플레이 화면 화질은 미검증이다. 씬에 배치하지 않았다.
- 포함: 하늘·달 하나·구름·먼 설산·안개. 성·다리·숲·고목·지형·성소·캐릭터는 제외한다.
- PNG 실제 크기와 Import 사양은 [시작맵 README](../../Assets/04.Sprites/Backgrounds/StartMap/README.md)의 v02 항목을 따른다.

## 실제 사용한 프롬프트

```text
Use case: lighting-weather.
Asset type: opaque far-background sprite for a 2D hand-painted snowy metroidvania.
Input image 1 is the EDIT TARGET: BG_Start_Far_v01. Input image 2 is Bg_Concept, authoritative palette and atmosphere reference ONLY; do not copy its objects or layout.
Primary request: make image 1 noticeably darker. Apply a restrained nighttime exposure adjustment, approximately one stop darker in sky, mountain midtones and mist. Retain readable mountain layers and delicate snow detail; do not crush shadows. Keep cool slate blue and silver moonlight, with a gently luminous moon and subdued clouds and mist closer to the reference's nighttime atmosphere.
STRICT INVARIANTS: preserve the exact original composition, framing, single moon position and size, cloud shapes, every mountain silhouette and ridge location, painted textures, aspect ratio 4:3. Change only illumination and tonal values, not geometry or content. No new trees, forest, buildings, castle, bridge, cliffs, platforms, ground, characters, sanctuary, text or second moon. Full-bleed opaque PNG, preserve 1448x1086 resolution if supported; no upscaling or crop. Produce one edited image.
```
