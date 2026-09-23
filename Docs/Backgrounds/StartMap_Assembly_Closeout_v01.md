# 시작맵 최신 조립 참고표 · 마감 검토 v01

2026-09-23. [검토 화면](StartMap_Closeout_v01.html) · [전체 데이터](StartMap_Closeout_v01.json). 현재 v04 구도를 통합한 자료이며, 과거 v28 조립표보다 이 문서를 우선한다.

## 조립 기준

19개 표시 항목, 고유 PNG18개(맵 아트17개 + 캐릭터 참고1개). 실제 PNG 헤더와 .meta의 PPU·피벗·GUID를 대조했다. 눈5쌍의 캔버스 크기·PPU·피벗·배치 좌표가 일치한다. 이는 투명 이미지의 모든 가장자리가 완벽히 맞는다는 의미는 아니다.

16:9 카메라 창은 전체 배치도 너비80%, 높이60%. 기준X20~100%, Y40~100%, 보이는 캐릭터 높이 약9.03%. 전체 배치도4:3은 상대 좌표 기준일 뿐 카메라 화면비가 아니다. 캐릭터 기본512×512 / PPU200과 실제 보이는 그림 높이를 구분한다.

Unity 배치는 사용자가 담당한다. 먼저 임시 플레이 지형·실제 플레이어·카메라를 확인하고 아래 아트를 맞춘다. HTML 좌상단 기준의 백분율은 Unity 중앙 피벗 Position이나 Scale이 아니므로 그대로 입력하지 않는다. 밝기 필터도 Unity 머티리얼과 동일한 수치로 보장하지 않는다.

## 뒤에서 앞으로 배치

| 순서 | 파일 | PNG | PPU | 너비% | X% | Y% | 밝기 / 불투명도 |
|---|---|---|---|---|---|---|---|
| 0 | [BG_Start_Far_v02.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png) | 1448×1086 | 100 | 100 | 0 | 0 | none / 1 |
| 1 | [BG_Start_Mist_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Mist_v01.png) | 2046×768 | 100 | 100 | 0 | 16 | none / .16 |
| 2 | [BG_Start_CastleBridge_v02.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v02.png) | 1448×1086 | 100 | 42 | 24.1 | 11.9 | none / 1 |
| 3 | [BG_Start_ForestMid_v03.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestMid_v03.png) | 1448×1086 | 100 | 100 | 0 | 9 | none / 1 |
| 4 | [BG_Start_Mist_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Mist_v01.png) | 2046×768 | 100 | 100 | 0 | 66 | none / .18 |
| 5 | [BG_Start_ForestRocks_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocks_v01.png) | 1774×887 | 100 | 19 | 19 | 78.7 | brightness(.7) / 1 |
| 6 | [BG_Start_ForestRocksSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocksSnow_v01.png) | 1774×887 | 100 | 19 | 19 | 78.7 | brightness(.7) / 1 |
| 7 | [BG_Start_StumpNear_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpNear_v01.png) | 1254×1254 | 100 | 11 | 36 | 76.8 | brightness(.75) / 1 |
| 8 | [BG_Start_StumpSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpSnow_v01.png) | 1254×1254 | 100 | 11 | 36 | 76.8 | brightness(.75) / 1 |
| 9 | [BG_Start_ShrubsNear_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ShrubsNear_v01.png) | 1774×887 | 100 | 12 | 24 | 82.2 | brightness(.8) / 1 |
| 10 | [BG_Start_CaveRubble_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubble_v01.png) | 1774×887 | 100 | 18 | 52 | 78 | brightness(.8) / 1 |
| 11 | [BG_Start_CaveRubbleSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubbleSnow_v01.png) | 1774×887 | 100 | 18 | 52 | 78 | brightness(.7) / 1 |
| 12 | [BG_Start_GroundApproach_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundApproach_v01.png) | 1774×887 | 100 | 100 | 0 | 51.2 | none / 1 |
| 13 | [BG_Start_GroundSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundSnow_v01.png) | 1774×887 | 100 | 100 | 0 | 51.2 | brightness(.8) / 1 |
| 14 | [BG_Start_CaveEntrance_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveEntrance_v01.png) | 1254×1254 | 100 | 70 | 46 | 9 | none / 1 |
| 15 | [BG_Start_CaveSnow_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveSnow_v01.png) | 1254×1254 | 100 | 70 | 46 | 9 | brightness(.7) / 1 |
| 16 | [BG_Start_CaveIcicles_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveIcicles_v01.png) | 1774×887 | 100 | 14 | 84 | 67 | brightness(.55) / 1 |
| 17 | [Run_00.png](../../Assets/05.Animation/player/GA1_Player_Motions_v4/Frames/Run/Run_00.png) | 512×512 | 200 | 6.190476 | 23.904762 | 83.090873 | none / 1 |
| 18 | [BG_Start_TreeLeft_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeLeft_v01.png) | 1024×1536 | 100 | 55 | -7 | -8 | none / 1 |

## 묶어서 조립할 소재

- 숲 바위 + 숲 바위 눈, 그루터기 + 그루터기 눈, 입구 잔돌 + 입구 잔돌 눈, 지면 + 지면 눈, 동굴 + 동굴 눈: 각각 같은 부모 아래 동일 위치·Scale. 눈은 별도 렌더러로 유지한다.
- 동굴을 이동할 때: 동굴 눈·고드름·입구 잔돌과 눈의 상대 위치를 함께 확인한다. 연결 바닥 길이는 별도로 점검한다.
- 장식 잔돌·관목·그루터기를 플레이 발판으로 자동 취급하지 않는다. 왼쪽 고목은 캐릭터보다 앞이므로 실제 이동 범위에서 가림 확인이 필요하다.
- CaveEntrance_v01은 내부 벽과 바닥까지 합쳐진 구도 후보다. 등반으로 이어지는 실제 연결부에서는 기존 파일을 늘여 쓰기보다 검증된 통로에 맞는 별도 소재를 제작한다.

## 적용 전후 확인

시작·중간·입구 카메라에서 캐릭터 크기와 발의 지면 접촉, 점프 시 고드름·천장 가림, 카메라 경계의 빈 부분, 패럴랙스 이동 때의 소재 끝을 확인한다. 성·달이 모든 화면에 들어갈 필요는 없다. 눈 제거는 나중에 실제 발 크기 기준으로 촘촘하게 처리하며 긴 눈 스프라이트 단위로 통째 제거하는 것으로 확정하지 않는다.
