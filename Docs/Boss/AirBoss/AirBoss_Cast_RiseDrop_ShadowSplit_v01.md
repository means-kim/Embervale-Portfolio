# AirBoss Cast_RiseDrop_ShadowSplit v01

2026-09-30. 사용자 요청에 따라 멀티 스프라이트 제작과 Unity 16분할 임포트 확인을 완료했다. 애니메이션 클립·Animator·씬·프리팹·게임 코드는 생성하거나 변경하지 않았다. 실제 게임 재생과 최종 아트 승인은 별도다.

[제작 기준](../../../ArtSource/AirBoss/cast_risedrop_shadowsplit/Cast_RiseDrop_ShadowSplit_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [기획서](../../../ArtSource/AirBoss/Aerial%20Boss%20Design%20Document%20v1.md)

## 동작

부유 → 상승 → 원래 부유 높이보다 낮은 최저점까지 하강 → 보스 형상 잔상 2개 좌우 분리 → 본체가 위로 올라가 원래 부유 높이·Idle 자세로 복귀한다. 잔상은 피해 없는 장식이며 본체와 같은 프레임·시트에 그렸다. 지면 타격·충격파·구체·수평 대시는 포함하지 않았다.

- 00: 고정 Idle 마스터 그대로.
- 01~05: 손을 몸 가까이 모으며 상승, 정점에서 후면 천을 아래로 접는다.
- 06~08: 하강과 천의 위쪽 후행. 08 최저점에서 잔상 분리 시작.
- 09~12: 본체가 위로 복귀하며 좌우 잔상 확산.
- 13: 잔상의 명암·윤곽을 흐리게 처리해 소멸로 연결. 본체의 후면 천은 유지.
- 14~15: 잔상 없이 원래 부유 높이와 Idle 자세로 정리.

셀 내부에 높이 변화가 그려져 있으므로 향후 월드 이동을 추가한다면 중복 이동 여부를 검토해야 한다. 실제 이동·타이밍은 이번에 구현하지 않았다.

## 산출물

- [Unity PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_RiseDrop_ShadowSplit_4x4_1024_v01.png) 및 같은 이름의 `.meta`.
- [제작 시트](../../../ArtSource/AirBoss/cast_risedrop_shadowsplit/sheets/AirBoss_Cast_RiseDrop_ShadowSplit_4x4_1024_v01.png).
- [개별 셀](../../../ArtSource/AirBoss/cast_risedrop_shadowsplit/frames/v01/) · [생성 원본과 제외 후보](../../../ArtSource/AirBoss/cast_risedrop_shadowsplit/source/v01/).
- [주요 자세](AirBoss_Cast_RiseDrop_ShadowSplit_v01_keyposes.png) · [어두운 바탕](AirBoss_Cast_RiseDrop_ShadowSplit_v01_contact_dark.png) · [밝은 바탕](AirBoss_Cast_RiseDrop_ShadowSplit_v01_contact_light.png).
- [실제 프롬프트 24회](AirBoss_Cast_RiseDrop_ShadowSplit_v01.prompts.json) · [픽셀 측정](AirBoss_Cast_RiseDrop_ShadowSplit_v01.metrics.json) · [Unity 확인](AirBoss_Cast_RiseDrop_ShadowSplit_v01.unity.json) · [기존 파일 보존 검사](AirBoss_Cast_RiseDrop_ShadowSplit_v01.protected_after.json).

## 제작과 수정

내장 ImageGen으로 24회 생성·수정해 15장을 채택했으며 00은 마스터 원본이다. 생성 요청 1024×1024와 실제 생성 원본 1254×1254를 구분한다. 전체 캔버스를 같은 비율로 1024에 정규화했고, 실루엣 fit·Trim·재중앙 정렬·개별 체격 보정은 하지 않았다. 사용한 기준 마스터, 콘티, 기존 캐스팅 비교 자료는 제작 기준에 링크했다.

초기 11번의 넓고 불투명한 잔상 후보를 제외하고 겹치는 푸른 윤곽형 잔상으로 수정했다. 03·06의 상단 천과 04의 하단 천은 끝을 자르거나 몸을 줄이지 않고 안쪽으로 접어 여백을 확보했다. 09·10은 잔상 간격과 복귀 연결을 보완했고, 13은 양옆 여백 및 잔상 소멸을 수정한 뒤 함께 옅어진 본체 후면 천을 복원했다. 제외 후보 9개도 보존했다.

Node sharp는 캔버스 정규화, 정적 비교 자료, 픽셀 검사와 시트 조립에 사용했다. alpha≥8 내용에서 16px 이상 떨어지고 완전히 분리된 alpha 1~7 성분만 잡점으로 정리했다. 총 22610px이며 프레임별 수량은 측정 파일에 있다. 연결된 천·잔상·발광은 보존했다.

## 검증 결과와 한계

- 1024×1024 RGBA 16장, 모두 서로 다른 SHA-256. 00은 고정 마스터 파일과 완전히 동일.
- 모든 alpha>0에 사방 64px 검사 통과. 전체 최소 여백 65px, 범위 밖 픽셀 0개.
- 4×4·4096×4096 시트에서 재추출한 16개 셀이 개별 파일과 픽셀 완전 일치. 제작 시트와 Unity 복사본 내용 동일.
- 밝은·어두운 바탕의 전체 프레임 및 주요 자세를 같은 셀 좌표로 정적 비교했다. 자동 재정렬은 하지 않았다.
- Unity 6000.3.22f1에서 Multiple 16개, 각 이름·Rect 1024×1024·Center (512,512)·PPU 24 확인. 실제 4096×4096, Max Size 4096, Uncompressed, Bilinear, Clamp, Mipmap Off.
- 임포트 후 콘솔 Error 조회 0개. Play 모드·애니메이션 재생은 진행하지 않았다. 정적 포즈 연결 검토를 실제 게임 재생 검증으로 취급하지 않는다.
- 기존 씬·애니메이션·Boss/AirBoss 원본 등 964개 파일의 전후 해시 일치.
- Unity PNG GUID: `ff4db2ba4c594cdb9ce6565ed01f7944`.
- 시트 SHA-256: `f39331b00bdb96053caf1e62400c6f5198a412f192bf852b559f3d6c80d413d2`.

이번 결과는 멀티 스프라이트 v01이다. 독립 잔상 자산·공격 판정·Animation Clip·Animator·GIF/APNG를 만들지 않았다. 실제 표시 크기, 재생 시간과 게임 안에서의 자연스러움은 후속 적용 때 확인한다.
