# 숲 바위 분리 눈 v01

2026-09-23. ForestRocks_v01에 겹치는 독립 장식 눈.

[PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocksSnow_v01.png) · [합성 v25](StartMap_Placement_v25.html) · [전체 캡처](StartMap_Placement_v25.png) · [확대 접합](StartMap_ForestRocksSnow_Alignment_v01.html) · [확대 캡처](StartMap_ForestRocksSnow_Alignment_v01.png)

## 사양과 연결

요청 및 실제1774×887 RGBA, 바위와 동일 캔버스·중앙pivot·PPU100. Scale1 캔버스17.74×8.87유닛, 최종 Transform Scale 미정. 고유 GUID·spriteID의 .meta 동반. SpriteSingle, FullRect, Max2048, Trilinear, Mipmap On, 무압축, 자동 Physics Shape Off.

바위 윗면의 눈 패치만 포함한다. 바위·지면·충돌·눈 제거 기능을 포함하지 않는다. y800 이후 alpha1 잔여 픽셀29개가 있어 엄밀한 완전 투명 조건은 충족하지 않지만 합성에서 식별되지 않는다. PNG 원본은 가공 없이 보존했다.

검토 좌표: 바위와 동일 width19%, left19%, top78.7%, brightness(.7). 바위 바로 위, 그루터기·관목·이동 지면·플레이어 뒤에 배치한다. HTML 밝기만70%이며 PNG 자체 밝기는 그대로다. Unity에서도 바위와 위치·Scale을 함께 맞출 후보이며 임의의 다른 돌에는 호환되지 않는다.

## 컨셉 비교와 확인 범위

바위 윗면의 적설을 추가해 컨셉 설경과의 차이를 줄였다. 주요 윗면에 붙는지1150px 폭 확대에서 확인했고 어두운 옆면·돌 사이 틈은 남아 있다. 실제 출력의 주 바위 눈 일부는 요청8~20px보다 두껍게 내려오지만 바위 전체를 덮지 않고 접합을 유지해 이번 후보로 보존했다.

전체 축소 합성에서는 캐릭터 크기와 가시성을 유지한다. 작은 돌의 눈은 수 픽셀로 읽혀 최종 카메라에서 밝기·두께 재확인이 필요하다. 바위 높이 기준은 이전 약0.7배를 유지하며 눈 포함 전체 높이를 새로 확정하지 않았다.

눈만 숨기면 바위 유지, 바위를 숨기면 눈도 숨는 검토 화면 동작과 전체 이미지 로드를 확인했다. 정지 아트 검토이며 실제 Unity 접지·이동·충돌·눈 제거는 미구현·미검증이다. 캡처는 화면 범위다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: ForestRocks_v01(좌표와 형태), Bg_Concept(적설 스타일). 생성 결과를 프로젝트로 복사했다.

```text
Use case: background-extraction. Image1 is exact target 1774x887 rock cluster sprite. Image2 is master winter forest concept for snow appearance. Produce ONLY a registered separate SNOW overlay on the identical full 1774x887 transparent canvas. Do not recenter, resize, or recompose. Imagine snow settled on the UPWARD-FACING rock tops of image1, then remove all stone to transparency. Main broad asymmetric thin snow cap hugs the large left-center rock top around x510..970,y512..600, with restrained thickness 8-20px at these source dimensions. Smaller separated caps on small stones x170..330,y660..700, x380..470,y680..720, x1080..1220,y620..670, and x1440..1570,y695..725. Coordinates approximate, follow actual image1 surfaces exactly. A few irregular small dustings on other upward facing chips, do not join all snow into one stripe. Cold muted blue-white with charcoal blue undershadow, slightly brighter than bare stone so snow reads at small display size, yet no glow or bright neon. Painterly textured winter snow matching image2. Keep vertical stone faces, air gaps, lower contact edges and background entirely transparent. NO rocks, ground, ice, particles, trees, character, text, shadow rectangle or checkerboard. No snow bank underneath. All y800..887 transparent. Preserve canvas margins and sparse separated arrangement. Output transparent PNG requested 1774x887.
```
