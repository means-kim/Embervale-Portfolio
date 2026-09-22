# 중경 숲 v03 — 원경 여백 보완

2026-09-23. 기존 ForestMid_v02의 큰 침엽수 높이와 중앙 밀도를 줄인 수정 후보.

[PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestMid_v03.png) · [전후 비교](StartMap_ForestMid_v03_Compare.html) · [비교 캡처](StartMap_ForestMid_v03_Compare.png) · [최신 배치 v27](StartMap_Placement_v27.html) · [조립표](StartMap_Assembly_v27.md)

## 변경과 실제 사양

요청 및 실제1448×1086 RGBA. PPU100, 중앙pivot, 기존v02 Import 설정을 유지하고 GUID·spriteID는 새로 발급했다. PNG와.meta를 함께 보존했다. Scale1 캔버스14.48×10.86유닛, 최종 월드 크기·Scale 미정. 원본v02는 덮어쓰지 않았다.

기존처럼 중경 침엽수만 포함하고 하늘·달·성·산·플레이 지면은 제외한다. 나뭇가지의 먼 눈 표현은 숲에 포함되어 있으며 발로 지우는 근경 눈 소재가 아니다. HTML은 기존 width100%, top-9%, left0, opacity1을 유지하고 PNG만 바꿨다. 캐릭터·동굴·지면·안개 위치와 크기도 유지했다.

## 컨셉 비교와 검토

v02에서는 높은 나무가 성 아래 능선과 골짜기를 가렸다. v03은 좌우 숲 덩어리의 높이를 낮추고 중앙 여백을 넓혔다. 같은 위치의 전후 합성에서 성·산의 가시성이 높아지고 근경과 중경 분리가 더 잘 읽히는 것을 육안 확인했다.

보조 측정: 원본 PNG에서 x400..1049,y300..799 영역을2px 간격으로 표본 추출했다. alpha32 이상 표본 비율은 v02 36.38%, v03 4.96%다. 이는 선택 영역의 불투명 픽셀 점유율이며 전체 나무 수나 실제 화면 가림 비율이 아니다. 프롬프트의 절반 밀도를 정확히 달성했다고 해석하지 않는다.

단독 소재에서는 하단 작은 나무 일부가 떨어져 보인다. 현재 합성에서는 기존 안개·뒤 풍경과 겹쳐 하단 경계가 약해지지만 다른 위치나 패럴랙스에서는 다시 검토해야 한다. 컨셉보다 중앙 공간이 넓어진 후보이며 동일 구도 완성본은 아니다. 여전히 평평한 이동 지형과 실제 카메라 검증은 남는다.

v27에 이전 숲v02 비교 토글과 나란히 비교 링크를 추가했다. 전환 결과 경로 및 전체 이미지 로드를 확인했다. Unity 씬·코드·캐릭터는 변경하지 않았다. 사용자의 개별 승인 전 수정 후보이며 정지 합성을 넘어선 플레이 품질을 확정하지 않는다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: ForestMid_v02(수정 대상), Bg_Concept(화풍·깊이 기준). 생성 결과를 프로젝트로 복사했다.

```text
Use case: precise-object-edit. Image1 is the existing transparent midground forest sprite (1448x1086). Image2 is master concept, especially the distant layered forest around the castle in upper-left starting area. Revise ONLY the forest into a less crowded, more distant forest layer on the same full 1448x1086 transparent canvas. Preserve cold blue-gray painterly conifer style, asymmetrical valley silhouette, upper-left moonlight. Reduce visually large foreground-like trees and strong white snow highlights. Keep 2 loose groups at the lower left and lower right with varied heights and generous irregular gaps between individual trees. Left tallest tip near x150,y390 (NOT y210); right tallest near x1320,y570; bottom fades to real transparency by y1030. Open a broad irregular transparent valley from x430..1000 above y780, with only a few small dim distant tips at its bottom, NOT a uniform hedge. About half the current visual tree density. Midground depth: lower contrast, desaturated slate blue, fine branches softer than original, restrained snow dusting acceptable as integral distant detail. Lower half should also have some transparent gaps rather than an opaque solid tree carpet. No fog sheet filling gaps, no new sky/mountains/moon/castle/bridge/ground/rocks/character/architecture. Output ONLY forest on real alpha background, keep canvas and outside margins, no framing or text. Do not reproduce the whole concept scene. Requested output1448x1086.
```
