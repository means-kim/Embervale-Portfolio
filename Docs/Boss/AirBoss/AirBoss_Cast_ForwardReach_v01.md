# AirBoss Cast_ForwardReach v01

2026-09-30. 사용자 요청에 따른 정면 지정 시전 멀티 스프라이트 v01 제작·Unity 임포트 확인 완료. 실제 게임 재생과 최종 아트 승인은 별도다.

[제작 기준](../../../ArtSource/AirBoss/cast_forwardreach/Cast_ForwardReach_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [기획서](../../../ArtSource/AirBoss/Aerial%20Boss%20Design%20Document%20v1.md)

캐릭터 오른팔(화면 왼쪽)을 몸 쪽으로 모았다가 관객 쪽으로 뻗는다. 전완의 단축과 가까운 손바닥의 원근으로 깊이를 표현하며 얼굴을 가리지 않는다. 반대팔은 낮게 유지한다. 단팔 상승·하강 및 양팔 시전과 구별되는 지정 시전 동작이다.

## 산출물

- [Unity PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_ForwardReach_4x4_1024_v01.png) 및 같은 이름의 `.meta`
- [제작 시트](../../../ArtSource/AirBoss/cast_forwardreach/sheets/AirBoss_Cast_ForwardReach_4x4_1024_v01.png)
- [개별 셀](../../../ArtSource/AirBoss/cast_forwardreach/frames/v01/) · [생성 원본](../../../ArtSource/AirBoss/cast_forwardreach/source/v01/)
- [어두운 바탕](AirBoss_Cast_ForwardReach_v01_contact_dark.png) · [밝은 바탕](AirBoss_Cast_ForwardReach_v01_contact_light.png) · [주요 자세](AirBoss_Cast_ForwardReach_v01_keyposes.png)
- [프롬프트](AirBoss_Cast_ForwardReach_v01.prompts.json) · [픽셀 측정](AirBoss_Cast_ForwardReach_v01.metrics.json)

셀 1024×1024 RGBA 16장, 4×4·4096×4096. PNG 위쪽부터 00~03 / 04~07 / 08~11 / 12~15. PPU 24, Center (512,512). 00은 고정 Idle 마스터 원본이다.

## 제작·검토 방식

내장 ImageGen을 사용하며 각 생성·수정에서 고정 마스터와 관련 주요 자세를 참조한다. 실제 요청 프롬프트와 사용한 참조 파일을 보존한다. 생성 출력 크기와 최종 셀 크기를 구분해 기록한다.

Node sharp로 전체 캔버스 정규화·정적 비교·조립·검사를 수행한다. 실루엣 Fit·Trim·재중앙정렬·개별 체격 보정은 하지 않는다. 완전히 분리된 alpha 1~7 성분 중 alpha≥8 본체·효과에서 16px 이상 떨어진 잡점만 정리하고 수량을 기록한다. 연결된 반투명 천·발광은 보존하며 바깥 64px 검사는 모든 alpha>0에 적용한다.

총 19회 생성·수정 중 15개 프레임을 채택하고 00번은 마스터 파일을 그대로 사용했다. 요청은 1024×1024, 실제 생성 원본은 모두 1254×1254였다. 원본 전체 캔버스를 같은 비율로 1024×1024에 정규화한 뒤 얼굴·몸통과 관절 위치를 정적 비교했다.

03번 초기 준비 자세는 천의 상·하단 여백이 47px·41px여서 제외하고 마스터를 다시 참조해 제작했다. 08·09번 초기 발동·잔광 자세의 하단 여백은 55px·60px였다. 보스 전체를 줄이거나 끝을 자르지 않고 천 끝을 안쪽으로 말아 수정했으며, 최종 하단 여백은 104px·93px다. 15번은 양손의 휴식 자세를 마스터에 더 가깝게 맞추도록 다시 제작했다. 제외한 원본과 수정에 사용한 1024 참조 스냅샷을 `source/v01/frameNN_attempt01*.png`에 보존했다.

## 최종 확인

- 16개 셀은 1024×1024 RGBA이며 서로 다른 SHA-256을 갖는다. 00번은 고정 Idle 마스터 파일과 완전히 동일하다.
- 모든 alpha>0에 사방 64px 여백 검사 통과. 전체 최소 여백은 65px이고 안전 범위 밖 픽셀은 0개다.
- 4096×4096 시트에서 16개 셀을 재추출해 개별 PNG와 픽셀 완전 일치를 확인했다. 제작 시트와 Unity 복사본의 파일 내용도 일치한다.
- 밝은·어두운 바탕의 전체 프레임과 주요 자세를 정적 비교했다. 단팔 손 모으기 → 정면 뻗기 → 손바닥 방출광 → 회수·Idle 복귀로 구분된다. 얼굴은 손과 시전광에 가려지지 않는다.
- Unity MCP 실제 임포트 후 Multiple 16개, 각 이름·Rect 1024×1024·Center (512,512)·PPU 24를 확인했다. 실제 텍스처 4096×4096, Max Size 4096, Uncompressed, Bilinear, Clamp, Mipmap Off. [Unity 측정](AirBoss_Cast_ForwardReach_v01.unity.json).
- 임포트 후 Unity 콘솔 Error 조회 결과 0개. Play 모드 재생은 진행하지 않았다.
- 기존 씬·AirBoss 애니메이션·아트·참조 288개 파일의 전후 해시 일치. [보존 검사](AirBoss_Cast_ForwardReach_v01.protected_after.json).
- Unity PNG GUID: `c5f092a9ad1c43bb8edc0515bf5be74a`.
- 시트 SHA-256: `c5587c31f4121c0bdae38d465bdc52baf8432d371f71b9a7b19c6b5f815d5af8`.

## 범위

멀티 스프라이트와 Unity 슬라이스만 제작한다. 클립·Animator·씬·프리팹·게임 코드를 생성하거나 변경하지 않는다. 실제 게임 재생, 최종 표시 크기와 최종 아트 승인은 별도다. 손에 붙은 응축·방출·소멸광을 포함하지만 독립 투사체·지면 공격·위험 가이드는 만들지 않는다.
