# 시작맵 조립 참고표 v27

2026-09-23. [통합 검토 화면](StartMap_Placement_v27.html) · [전체 캡처](StartMap_Placement_v27.png) · [파일·배치 데이터](StartMap_Placement_v27.json).

[중경 수정 사양과 프롬프트](StartMap_ForestMid_v03.md) · [전후 비교](StartMap_ForestMid_v03_Compare.html)

## 확인한 현재 상태

중경 숲을 ForestMid_v03으로 교체했다. 다른 소재와 상대 배치는 v26에서 유지했다. 18개 표시 항목, 고유 파일17개(맵 아트16개와 플레이어 참고1개)이며 안개 PNG를 두 번 사용한다. 과거 버전과 제외된 오른쪽 고목은 이 표에 포함하지 않으며 삭제하지 않았다.

원본/눈 4쌍: ForestRocks/ForestRocksSnow, StumpNear/StumpSnow, GroundApproach/GroundSnow, CaveEntrance/CaveSnow. 각 쌍은 PNG 크기·PPU·피벗·HTML 너비와 좌표가 일치한다. 전체 이미지 로드와 표시 전환은 브라우저에서 확인한다. Unity 씬에서의 결과를 보증하지 않는다.

## 사용자가 조립할 때의 순서

1. 실제 플레이어 Scale과 카메라 범위를 먼저 정한다. 512px 캔버스 전체와 보이는 그림 높이를 혼동하지 않는다. 현재 HTML은 Run_00 보이는 높이336px를 화면 높이5.42%로 표시한 비교다.
2. 원경·성·중경 숲·안개를 놓고 캐릭터를 함께 비교한다. 소재의 투명 여백도 캔버스 크기에 포함된다.
3. 바닥과 동굴을 임시 플레이 지형에 맞춘다. CaveEntrance는 외벽·내부 벽·바닥이 합쳐진 후보이므로 최종 지형 모듈로 확정하지 않는다.
4. 각 원본과 눈을 같은 부모 아래 두고 동일 위치·Scale로 맞춘다. 현재 쌍의 피벗은 중앙이다. 눈은 원본보다 앞에서 표시하되 지면 장식은 캐릭터 뒤에 둔다.
5. 관목·그루터기·숲 바위·입구 바위를 추가하고 왼쪽 고목의 캐릭터 가림을 확인한다. 아래 순서는 HTML의 그리기 순서이며 기존 프로젝트 Sorting Layer/Order 체계에 맞춰 적용할 참고다.
6. 동굴을 오른쪽으로 옮길 때 외벽 눈·입구 바위·고드름의 상대 위치와 연결 바닥을 함께 점검한다. HTML에서는 상대 이동만 표현하며 새 지면이나 충돌을 생성하지 않는다.

## 상대 배치 목록

너비와X는 프레임 너비 대비%, Y는 높이 대비%. X·Y는 PNG 캔버스 좌상단 기준으로 Unity 중앙 피벗 위치와 다르다. 수치를 Unity Position/Scale에 그대로 복사하지 않는다. 밝기는 HTML CSS값이며 Unity 색상·머티리얼과의 수치 동등성은 검증하지 않았다.

| 뒤→앞 | 파일 | 실제 PNG | PPU | 너비% | X% | Y% | HTML 밝기/불투명도 |
|---|---|---|---|---|---|---|---|
| 0 | [BG_Start_Far_v02.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png) | 1448×1086 | 100 | 100 | 0 | 0 | none / 1 |
| 1 | [BG_Start_Mist_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Mist_v01.png) | 2046×768 | 100 | 100 | 0 | 16 | none / .22 |
| 2 | [BG_Start_CastleBridge_v02.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v02.png) | 1448×1086 | 100 | 42 | 24.1 | 11.9 | none / 1 |
| 3 | [BG_Start_ForestMid_v03.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestMid_v03.png) | 1448×1086 | 100 | 100 | 0 | -9 | none / 1 |
| 4 | [BG_Start_Mist_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Mist_v01.png) | 2046×768 | 100 | 100 | 0 | 42 | none / .28 |
| 5 | [BG_Start_ForestRocks_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocks_v01.png) | 1774×887 | 100 | 19 | 19 | 78.7 | brightness(.7) / 1 |
| 6 | [BG_Start_ForestRocksSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocksSnow_v01.png) | 1774×887 | 100 | 19 | 19 | 78.7 | brightness(.7) / 1 |
| 7 | [BG_Start_StumpNear_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpNear_v01.png) | 1254×1254 | 100 | 11 | 36 | 76.8 | brightness(.75) / 1 |
| 8 | [BG_Start_StumpSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpSnow_v01.png) | 1254×1254 | 100 | 11 | 36 | 76.8 | brightness(.75) / 1 |
| 9 | [BG_Start_ShrubsNear_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ShrubsNear_v01.png) | 1774×887 | 100 | 12 | 24 | 82.2 | brightness(.8) / 1 |
| 10 | [BG_Start_CaveRubble_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubble_v01.png) | 1774×887 | 100 | 18 | 52 | 78 | brightness(.8) / 1 |
| 11 | [BG_Start_GroundApproach_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundApproach_v01.png) | 1774×887 | 100 | 100 | 0 | 51.2 | none / 1 |
| 12 | [BG_Start_GroundSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundSnow_v01.png) | 1774×887 | 100 | 100 | 0 | 51.2 | brightness(.8) / 1 |
| 13 | [BG_Start_CaveEntrance_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveEntrance_v01.png) | 1254×1254 | 100 | 70 | 46 | 9 | none / 1 |
| 14 | [BG_Start_CaveSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveSnow_v01.png) | 1254×1254 | 100 | 70 | 46 | 9 | brightness(.7) / 1 |
| 15 | [BG_Start_CaveIcicles_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveIcicles_v01.png) | 1774×887 | 100 | 14 | 84 | 67 | brightness(.55) / 1 |
| 16 | [Run_00.png](../../Assets/05.Animation/player/GA1_Player_Motions_v4/Frames/Run/Run_00.png) | 512×512 | 200 | 6.190476 | 23.904762 | 83.090873 | none / 1 |
| 17 | [BG_Start_TreeLeft_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeLeft_v01.png) | 1024×1536 | 100 | 55 | -7 | -8 | none / 1 |

## 컨셉 차이와 후속 판단

| 차이 | 현재 판단과 처리 |
|---|---|
| 오른쪽 거대한 고목이 동굴로 바뀜 | 사용자 선택이므로 유지. 원본 고목 구도로 되돌리지 않는다. |
| 이동로가 더 평평함 | 현재 GroundApproach는 연결 후보. 장식 추가로 고저차를 해결하지 말고 이동·점프 범위 확정 후 지형 아트를 맞춘다. |
| 중경 침엽수가 더 크고 빽빽함 | 원경 성과 숲 사이 여백·깊이 차이. v03에서 큰 나무 높이와 중앙 밀도를 낮췄다. 전후 합성에서 원경 여백 증가를 확인했으며 최종 카메라 검토는 남았다. |
| 근경 눈 양이 소재마다 다름 | 그루터기 눈은 얇고 관목·동굴 입구 잔돌은 별도 눈 없음. 모든 소재에 자동으로 눈을 추가하지 말고 실제 표시 크기에서 읽히는 정도를 비교한다. |
| 플레이어·소품이 작은 화면에서 작게 읽힘 | 컨셉 비교 비율 유지. 확대 미리보기는 관찰용이며 최종 카메라 확대 확정값이 아님. |

캐릭터 Scale, 카메라, 접지·점프·충돌, 동굴 이후 경로, 개별 소재 사용자 승인은 여전히 별도 확인 대상이다. 이 정리는 Unity 변경이나 최종 완료 선언이 아니다. 눈 제거는 실제 발 크기 기준의 촘촘한 표현 요구를 유지하고 구현 범위가 주어질 때 진행한다.
