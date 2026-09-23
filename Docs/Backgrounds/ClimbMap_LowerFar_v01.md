# 등반맵 하단 먼 배경 v01

2026-09-23. 사용자 승인: 시작맵 1차 아트 기준 고정, 동굴 입구→짧은 전이 통로→위로 열린 암벽 틈 등반 방향으로 착수.

## 범위와 구도

하단 진입부의 먼 풍경 한 장만 제작했다. 왼쪽 전이 통로에서 같은 높이로 들어온 뒤 위로 진행한다. 양쪽 중경 암벽이 세로 틈을 만들고 뒤로 이 원경이 보이도록 구성한다. 실제 통로 폭·발판·점프 거리는 미정이며 그림에 맞춰 확정하지 않는다.

레이어 계획: 먼 배경(이번 소재) → 중경 암벽(다음 소재) → 진입 지면·발판 → 별도 눈·고드름. 각자의 위치·밝기·가림은 소재가 준비될 때 합성해서 확인한다. 전체 등반맵을 한 이미지나 한 화면에 넣지 않는다.

## 파일과 사양

- [BG_Climb_LowerFar_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_LowerFar_v01.png), 고유 GUID·spriteID의 .meta 동반.
- 내장 image_gen 사용. 요청1024×1536, 실제1024×1536 RGB 불투명 PNG. 확대 저장하지 않았다.
- Import: PPU100, 중앙pivot, Sprite Single. 기존 Far_v02 Import를 참고해 동일 기본값을 복사했다. 최종 Transform Scale과 월드 크기는 미정.
- 참조 순서: [컨셉](../../Assets/04.Sprites/Bg_Concept.png), [시작맵 원경v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png), [동굴 입구](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveEntrance_v01.png). 실제로 연 뒤 참조했다.
- 포함: 먼 산, 하늘·구름, 계곡 안개. 요청과 달리 작은 먼 숲 실루엣도 일부 포함되어 있다. 이 수목과 구름·안개는 독립 레이어가 아니며 원경과 함께 움직인다.
- 제외: 근거리 암벽·바닥·발판·캐릭터·성소·리프트·새 달 원반·장식 눈 덮개. 기존 PNG와 씬은 변경하지 않았다.

## 검토

[16:9 부분 보기·시작맵 비교](ClimbMap_LowerFar_v01.html). [하단 캡처](ClimbMap_LowerFar_v01_Entry.png), [위쪽 탐색 캡처](ClimbMap_LowerFar_v01_Upper.png).

실제 원경1024×1536에서 가로1024 전체와 세로576px만 보이는16:9 창으로 탐색한다. 창 높이는 소재의37.5%, 세로 탐색은Y0~62.5%. 하단은Y62.5~100%, 중간은31.25~68.75%다. 이것은 배경 크롭 실험이며 실제 카메라 속도·추적·패럴랙스 시뮬레이션이 아니다.

새 캔버스에 시작맵의80% 좌표를 그대로 재사용하지 않았다. 16:9와 실제 그림 기준 캐릭터 높이약9%를 유지했고 DOM 측정값은약9.00%였다. 캐릭터는 화면상의 크기 참고이며 점선은 바닥이나 충돌 지형이 아니다.

시작맵과 비교해 차가운 청색과 명암 분위기는 이어진다. 새 원경의 하단은 더 좁고 깊은 계곡처럼 보이며, 위로 탐색하면 높은 능선이 드러난다. 하단의 어두운 배경 위에서 캐릭터 실루엣을 구분할 수 있었다. 전체 이미지 로드 실패0, 수직 탐색과 암벽 영역 가이드 토글 확인.

현재 양쪽 암벽과 전이 통로는 미제작이므로 실제 접합·가림·산의 체감 거리·Unity 플레이는 미검증이다. 시작맵 원경과 직접 맞붙는 무봉제 이미지가 아니다. 다음에는 진입부 암벽 한 소재를 겹쳐 원경이 지나치게 가까워 보이는지 판단한다. 밝기 수정이 필요하면 그 합성 근거로 진행한다.

## 실제 프롬프트

```text
Use case: stylized-concept. Create ONE background-only game sprite for the LOWER ENTRY SECTION of a vertically ascending open rock cleft in a hand-painted 2D metroidvania. Reference1 is the master world concept: retain its charcoal/cold slate blue palette, painterly fine rock texture and atmospheric depth. Reference2 is the existing start-map distant mountain sky: match its restrained dark blue luminance, distant jagged snowy mountain language and moonlight direction from upper left. Reference3 is the existing cave entrance: context for the foreground that will be separate, NOT to bake into this background. Requested portrait canvas1024x1536, opaque full-bleed PNG. This is distant scenery visible BEHIND separately assembled near rock walls and platforms, NOT an entire playable level. A subdued deep valley at bottom; overlapping distant sharp mountain ridges through thin blue haze in lower two thirds; sparse dark storm cloud sky in upper third, opening slightly brighter toward upper left. Keep central silhouette quiet, low contrast atmospheric depth. Mountains must feel far away, no huge close cliff. No sun or moon disc (the existing world's moon stays off-screen here), no stars as bright focal points. No castle, architecture, sanctuary, lift, bridge, trees, characters, enemies, near rock walls, cave arch, floor, ledges, snow caps for playable platforms, icicles, text, border or UI. Do not put a traversable path into image. Preserve tonal continuity with image2, not cyan glowing cave or daytime sky. Portrait is coverage for vertical camera movement, not a request to fit the whole ascent into one camera. Treat it as a new adjoining atmospheric view, not a guaranteed seamless tile.
```

