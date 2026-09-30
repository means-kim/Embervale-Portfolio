# AirBoss Cast_Spin360 v01

2026-09-30 착수, 2026-10-01 완료. 사용자 요청에 따라 세로축 회전 멀티 스프라이트 제작과 Unity 16분할 임포트 확인을 완료했다. Animation Clip·Animator·씬·프리팹·게임 코드는 만들거나 변경하지 않았다.

[제작 기준](../../../ArtSource/AirBoss/cast_spin360/Cast_Spin360_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [기획서 ANIM-07](../../../ArtSource/AirBoss/Aerial%20Boss%20Design%20Document%20v1.md)

## 동작과 참조

몸을 세운 채 정면 → 화면 오른쪽을 보는 측면 → 후면 → 화면 왼쪽을 보는 측면 → 정면으로 돌아온다. 세로축은 사용자 확정이며 회전 방향은 이번 제작의 선택이다. 이미지 평면 회전·좌우 반전으로 자세를 대신하지 않았다.

- 00: 고정 Idle 마스터 원본.
- 01~02: 팔과 손을 몸 쪽으로 모으는 준비.
- 03~05: 오른쪽을 보는 측면으로 회전.
- 06~08: 등과 뒤통수가 드러나는 후면으로 진행.
- 09~11: 반대쪽 측면으로 진행.
- 12~13: 정면 복귀, 천과 손목의 짧은 후행광.
- 14~15: 손을 풀고 천의 움직임을 정리하며 Idle로 연결.

[마스터](../../../ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png)와 [콘티](../../../ArtSource/AirBoss/references/AirBoss_Storyboard_v01.png)를 실제 이미지로 확인했다. 콘티의 화면 평면 회전 예시는 이번 세로축 지시로 대체했다. 측면·후면 주요 자세를 먼저 만든 뒤 이를 마스터와 함께 연결 자세의 이미지 참조로 사용했다. 후면은 기존 목 고리·갑옷 판·천 부착점의 연속 구조로 보완했으며 얼굴과 눈을 후면에 복제하지 않았다.

양쪽 측면은 각각 생성한 그림이다. 높은 비대칭 천은 회전하며 앞뒤 가림이 달라진다. 회전 중 작은 푸른 후행광은 본체 PNG에 포함했다. 계획서의 각도는 그림의 방향 목표이며 3D 모델로 측정한 각도가 아니다. 기존 3.6초 기획을 이번에 클립이나 재생 파일로 구현하지 않았다.

## 파일

- [Unity 멀티 스프라이트 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_Spin360_4x4_1024_v01.png) 및 같은 이름의 `.meta`.
- [제작 시트](../../../ArtSource/AirBoss/cast_spin360/sheets/AirBoss_Cast_Spin360_4x4_1024_v01.png).
- [개별 셀](../../../ArtSource/AirBoss/cast_spin360/frames/v01/) · [생성 원본과 제외 후보](../../../ArtSource/AirBoss/cast_spin360/source/v01/).
- [주요 자세](AirBoss_Cast_Spin360_v01_keyposes.png) · [어두운 바탕](AirBoss_Cast_Spin360_v01_contact_dark.png) · [밝은 바탕](AirBoss_Cast_Spin360_v01_contact_light.png).
- [실제 프롬프트](AirBoss_Cast_Spin360_v01.prompts.json) · [픽셀 측정](AirBoss_Cast_Spin360_v01.metrics.json) · [Unity 확인](AirBoss_Cast_Spin360_v01.unity.json) · [기존 파일 보존 검사](AirBoss_Cast_Spin360_v01.protected_after.json).

## 제작 방식

총 27회 생성·수정 중 15장을 채택했고 제외 후보 12개를 보존했다. 마스터를 그대로 사용하는 00을 합쳐 총 16장이다.

내장 ImageGen으로 자세를 생성·수정했다. 00은 마스터 파일을 그대로 복사했다. 생성 요청 1024×1024와 실제 생성 원본 1254×1254를 구분한다. 원본 전체 캔버스를 같은 비율로 1024에 정규화했으며 외곽 fit·Trim·재중앙 정렬·프레임별 확대축소·좌우 반전·평면 회전은 하지 않았다.

오른쪽 측면의 본체 위치와 상하 천 끝, 준비 자세와 회전 중 넓게 흐르는 천의 여백을 수정했다. 손이나 천을 잘라내지 않고 ImageGen에서 천 끝을 안쪽으로 접어 그렸다. 제외된 후보 원본도 보존했다. 최종 그림의 미세한 선·균열과 원근 변화는 정적 아트 검토 범위이며 3D 리깅의 동일 형상 보증과 구분한다.

Node sharp는 전체 캔버스 정규화, 정적 비교 자료, 픽셀 검사와 시트 조립에만 사용했다. alpha≥8 내용에서 16px 이상 떨어진, 완전히 분리된 alpha 1~7 성분만 잡점으로 정리했다. 연결된 반투명 천·발광은 보존했다. 프레임별 정리 수량과 사방 여백은 측정 파일에 기록한다.

## 검증 결과

- 1024×1024 RGBA 16장, 모두 서로 다른 SHA-256. 00은 고정 마스터 파일과 완전히 동일하다.
- 모든 alpha>0에 사방 64px 검사 통과. 전체 최소 여백 64px, 범위 밖 픽셀 0개.
- 분리된 희미한 잡점 정리 수량은 최종 채택 15장 합계 4043px다. 연결된 천과 발광에 일괄 alpha 문턱을 적용하지 않았다.
- 4×4·4096×4096 시트에서 재추출한 16개 셀은 개별 파일과 픽셀 완전 일치. 제작 시트와 Unity 복사본 내용 동일.
- 밝은·어두운 바탕에서 전체 16장과 주요 방향을 동일한 셀 좌표로 정적 비교했다. 자동 재정렬은 하지 않았다.
- Unity 6000.3.22f1에서 Multiple 16개, 순서·이름·Rect 1024×1024·Center (512,512)·PPU 24 확인. 실제 4096×4096, Max Size 4096, Uncompressed, Bilinear, Clamp, Mipmap Off.
- 임포트 후 콘솔 Error 조회 0개. 기존 씬·애니메이션·Boss/AirBoss 원본 등 1007개 파일의 전후 해시 일치.
- Unity PNG GUID: `08bb4a3d122f4866949412ce1c02a34e`.
- 시트 SHA-256: `15dc807d526fa31dd377097bdcd26db8092f8350a6772fd5d4f941bbe46cef90`.

정적 자세 검토와 Unity 임포트를 실제 게임 재생 검증으로 취급하지 않는다. 애니메이션 클립, GIF/APNG, 타임라인은 생성하지 않았다. 실제 표시 크기·재생 시간·게임 내 자연스러움과 사용자 최종 아트 승인은 후속 검토 대상이다.
