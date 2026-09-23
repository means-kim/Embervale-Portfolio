# 등반맵 통합 조립표 v01

[마감 화면](ClimbMap_Closeout_v01.html) · [배치 JSON](ClimbMap_Closeout_v01.json) · [실제 PNG/Import 점검](ClimbMap_Closeout_v01_AssetAudit.json).

## 사용 소재

모두 Assets/04.Sprites/Backgrounds/ClimbMap 안에 PNG와.meta가 있다.

|소재|실제 크기|형식|PPU|MaxSize|
|---|---|---|---:|---:|
|[BG_Climb_InteriorBack_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_InteriorBack_v01.png)|1024×1536|RGB|100|2048|
|[BG_Climb_Ledge_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ledge_v01.png)|2172×724|RGBA|100|4096|
|[BG_Climb_LedgeSmall_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_LedgeSmall_v01.png)|1448×1086|RGBA|100|2048|
|[BG_Climb_LedgeMedium_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_LedgeMedium_v01.png)|1774×887|RGBA|100|2048|
|[BG_Climb_Ceiling_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ceiling_v01.png)|1774×887|RGBA|100|2048|
|[BG_Climb_EntryWall_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_EntryWall_v01.png)|1024×1536|RGBA|100|2048|
|[BG_Climb_ExitUpper_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_ExitUpper_v01.png)|1024×1536|RGBA|100|2048|
|[BG_Climb_ExitLower_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_ExitLower_v01.png)|1024×1536|RGBA|100|2048|
|[BG_Climb_JoinGround_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_JoinGround_v01.png)|1774×887|RGBA|100|2048|

9종 GUID 고유, 중앙pivot·무압축 확인. Max4096은 긴 발판 다운샘플 방지 설정이며 원본4K 의미가 아니다.

## 기준 배치

HTML 1000×1600, 좌상단 원점. 너비는 투명 여백 포함, 균일 확대. 최종 월드 좌표 아님.

|항목|소재|X|Y|너비|비고|
|---|---|---:|---:|---:|---|
|back|BG_Climb_InteriorBack_v01.png|0|0|1000|cover: scaled PNG1066.667×1600, centered crop33.333 each side|
|BG_Climb_EntryWall_v01.png|BG_Climb_EntryWall_v01.png|-450|0|1000|independent alpha, no mask|
|BG_Climb_ExitUpper_v01.png|BG_Climb_ExitUpper_v01.png|450|-130|1000|independent alpha, no mask|
|BG_Climb_ExitLower_v01.png|BG_Climb_ExitLower_v01.png|450|70|1000|independent alpha, no mask|
|entry|BG_Climb_Ledge_v01.png|-300|1311.08|1600|adjustable wide|
|p1|BG_Climb_LedgeSmall_v01.png|190|1230|220|adjustable small|
|p2|BG_Climb_LedgeMedium_v01.png|495|1100|350|adjustable medium|
|p3|BG_Climb_LedgeSmall_v01.png|245|960|180|adjustable small|
|rest|BG_Climb_Ledge_v01.png|365|830|620|adjustable wide|
|p5|BG_Climb_LedgeMedium_v01.png|170|710|300|adjustable medium|
|p6|BG_Climb_LedgeSmall_v01.png|555|560|200|adjustable small|
|p7|BG_Climb_LedgeSmall_v01.png|255|445|240|adjustable small|
|exit|BG_Climb_Ledge_v01.png|440|310|650|adjustable wide|
|ceiling|BG_Climb_Ceiling_v01.png|-50|-100|1100|draw above walls, behind actor|

표는 등반 단독 기준. 시작맵과 함께 조립할 때는 [연결 v05 데이터](StartToClimb_Joined_v05.json)를 쓴다. 그 배치에서는 등반 원점X1250, 뒤벽 확장, entry Y+15와 연결바닥X1160,Y1410,너비380이 추가된다. 두 좌표계를 혼용하지 않는다.

## Unity 조립 순서

1. Gameplay와Art 부모를 분리한다. Art 안에 Back/Wall/Platform/Ceiling/Transition을 구분한다. 이는 권장 계층이며 새 게임 코드를 요구하지 않는다.
2. 뒤벽을 먼저 놓고 입구·출구 벽3개를 각각 배치한다. 세 벽은 전체 캔버스 중앙pivot이므로 보이는 바위 중심으로 맞추지 않는다.
3. 발판은9개 인스턴스로 조립한다. 이동·폭·간격은 자유롭게 조정한다. 가로만 늘려 질감을 찌그러뜨리지 않는다.
4. 천장을 앞에 겹치되 캐릭터보다 뒤로 표시한다. 시작맵 연결은 승인된 바닥만 쓰는 방향을 유지한다. JoinWall·JoinSnowEnd를 추가하지 않는다.
5. 실제 캐릭터 발 위치와 충돌 지형에 맞춰 발판 아트를 조정한다. 배경 바위나 장식 벽에 임의로 충돌을 부여하지 않는다.
6. 같은 지형에 붙은 조각들은 같은 이동/패럴랙스 기준으로 묶는다. 실제 카메라 이동 범위에서 배경 여백을 다시 확인한다.

상대 정렬: Back < Wall < Platform < Ceiling < Player. 이 값은 상대 관계이며 기존 프로젝트 Sorting Layer/Order를 존중한다. 실제 조명과 CSS 밝기는 다를 수 있다.

좌표 변환: 원본 W×H, HTML 너비w라면 표시높이h=w×H/W. HTML1단위당k월드단위로 정했을 때 Scale=w×k×PPU/W, 중앙위치X=(x+w/2)×k,Y=−(y+h/2)×k. 후면 cover는 예외이므로 표의 실제 확대 크기와 카메라 가림을 참고한다. 캐릭터 Scale과 Orthographic Size는 이 표만으로 확정하지 않는다.

## 상단 출구 인계

다음 통로 접합 기준은 등반 HTML X1000, 상단exit 발판의 참고 표면Y371.75. 이는 계산된 근삿값이며 실제 PNG 표면을 보고 맞춘다. 출구 위·아래 벽은 각각 이동할 수 있다. 다음 구역이 없으므로 오른쪽 끝을 막는 마감 벽은 만들지 않는다. 다음 통로도 외부 출구가 합의되기 전까지 실내 후면을 유지한다. 현재 카메라 오른쪽 끝 밖 공간은 제작/검증하지 않았다.

## 보존하지만 제외하는 소재

LowerFar 야외후보, 기존 WallLeft/WallRight 전체벽, JoinWall, JoinSnowEnd는 현재 조립에서 제외. 삭제하지 않는다. 천장에는 전용Ceiling을 사용하며 뒤집은 발판은 과거 비교용이다.
