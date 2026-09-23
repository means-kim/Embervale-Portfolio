# 동굴 진입 잔돌 눈 v01

2026-09-23. 내장 image_gen으로 제작한 독립 눈 소재 후보.

- 소재: [BG_Start_CaveRubbleSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubbleSnow_v01.png), 고유 GUID와 spriteID의 .meta 동반.
- 참조: [기존 잔돌](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubble_v01.png), [원본 컨셉](../../Assets/04.Sprites/Bg_Concept.png). 두 이미지를 실제로 열어 확인한 뒤 제작했다.
- 요청 및 실제 크기: 1774×887, RGBA, 투명 배경. PPU100, 중앙 pivot, Max2048, 압축 없음, Trilinear, MipMap On. 원본 PNG 밝기는 유지했다.
- 포함: 위를 향한 바위 면의 얇고 끊어진 눈. 제외: 바위 본체·지면·파티클·눈 제거 기능.

## 배치와 확인

[현재 카메라 합성 v03](StartMap_CameraWindow_v03.html)은 전체 배치 v28을 바탕으로 잔돌 눈만 추가했다. 전체 배치 v28 파일 자체는 과거 상태로 보존한다.

전체 배치도 기준 width18%, left52%, top78%, HTML 밝기70%. 잔돌과 동일 좌표·크기로 잔돌 뒤가 아닌 바로 위에 겹치고, 지면보다 먼저 그린다. Unity에서도 본체와 동일 Transform·Scale을 기준으로 조립하되 표시 밝기와 정렬 순서를 실제 씬에서 확인해야 한다.

사용자 선택인 16:9, 카메라폭80%, X20~100%, Y40~100%, 캐릭터 보이는 높이 약9.0%를 그대로 유지했다. 플레이어 위치만 동굴 진입으로 옮겨 비교했다.

[확대 접합](StartMap_CaveRubbleSnow_v01_Alignment.html)에서 눈이 주요 바위 윗면에 붙는 것을 확인했다. 축소 합성에서는 작은 눈 테두리로 보이며 주변 지면 눈과 이어진다. 변화는 은은하고, 눈 추가만으로 전체 컨셉 차이가 해소된 것은 아니다. 관찰한 정지 화면에서 이동로와 캐릭터를 가리지 않았다. 이미지 로드 실패0, 두 합성의 눈 동시 표시 전환을 확인했다.

[확대 캡처](StartMap_CaveRubbleSnow_v01_Alignment.png) · [동굴 진입 캡처](StartMap_CameraWindow_v03_Entry.png)

Unity 씬·실제 이동·점프·패럴랙스·접촉에 따른 눈 제거는 미검증이다. 장식 바위용 눈이며 촘촘한 발자국 마스크를 확정한 파일이 아니다. 다음 검토에서는 동일 카메라 크기로 이동 구간의 연결과 입구 주변 가독성을 우선 확인한다.

## 실제 프롬프트

내장 image_gen, 첫 참조는 잔돌, 두 번째 참조는 컨셉.

```text
Use case: background-extraction. Image1 is the exact target cave-approach rubble sprite,1774x887. Image2 is master snowy forest/cave concept for snow texture. Output ONLY a separate snow overlay registered precisely to Image1, identical full1774x887 transparent canvas, no resizing/recentering. Imagine a light irregular dusting of settled snow on the UPWARD FACING ledges of these rocks, then hide ALL stone. Sparse small snow patches on low left chips; a few broken thin caps across central stepped rock tops; slightly wider but still thin patches on large right high ledges. Keep dark cracks and vertical faces bare, don't trace the whole silhouette with a white outline. Main highest rock near x1380,y380 gets a narrow cap. Follow actual reference geometry, no new rock shapes. Muted cold blue-white painterly snow with charcoal blue undershadow, matching snowy approach in image2, no brilliant glow. Preserve empty gaps and transparent margins. ONLY snow, no rocks, soil, ground slab, sky, mist, particles, ice, trees, characters, text or checkerboard. Snow should blend with existing separate ground and cliff snow without becoming a continuous horizontal floor. No snow below y850. Output transparent PNG requested1774x887.
```

