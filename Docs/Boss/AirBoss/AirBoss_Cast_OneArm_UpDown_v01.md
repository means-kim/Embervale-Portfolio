# AirBoss Cast_OneArm_UpDown v01

2026-09-30. 사용자가 요청한 단팔 상승·하강 시전 멀티 스프라이트 v01 제작·Unity 임포트 확인 완료. 최종 아트 승인과 실제 게임 재생 검토는 별도다.

[제작 기준](../../../ArtSource/AirBoss/cast_onearm_updown/Cast_OneArm_UpDown_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [콘티](../../../ArtSource/AirBoss/references/AirBoss_Storyboard_v01.png)

시전 팔은 **캐릭터 오른팔 = 화면 왼쪽 팔**이다. 반대팔은 낮게 유지한다. 한쪽 손을 모아 올리고 짧게 응축한 뒤, 같은 팔을 내려 시전하고 정면 Idle로 복귀한다. 양팔을 모아 내리치는 LiftDrop 및 넓은 V자를 유지하는 Raise와 팔 사용·실루엣·힘의 방향을 구별했다.

## 출력

- 16개 셀: 1024×1024 RGBA. 00은 고정 Idle 마스터를 그대로 복사.
- 시트: 4×4·4096×4096. 위에서부터 00~03 / 04~07 / 08~11 / 12~15.
- Unity 기준: Sprite Multiple 16개, PPU 24, 셀 Center (512,512).
- 구체·손과 팔의 잔상은 Body PNG에 포함. 독립 투사체·위험 가이드·바닥 공격 자산 없음.

## 제작 방식과 수정 이력

내장 ImageGen으로 각 프레임을 생성·수정하고 매번 고정 마스터를 이미지 참조로 사용했다. 요청 해상도 1024×1024와 실제 도구 출력 크기는 구분해 [측정 기록](AirBoss_Cast_OneArm_UpDown_v01.metrics.json)에 보존한다. 생성 원본 전체 캔버스만 같은 비율로 1024에 정규화하며 실루엣 Fit·Trim·개별 체격 조정·외곽 중앙정렬은 하지 않는다.

Node sharp로 캔버스 정규화·검사·시트 조립을 수행한다. alpha≥8 본체/효과에서 16px 이상 떨어져 있고 완전히 분리된 alpha 1~7 성분만 잡점으로 정리하고 개수를 기록한다. 연결된 천·발광·부분 투명도는 보존한다. 바깥 64px 검사는 alpha>0 전체에 적용한다.

최대 상승 05번은 초기 구체가 상단 여백을 침범했다. 작은 구체로 수정하는 과정에서 본체가 아래로 밀린 후보도 제외하고, 원본 마스터에서 본체 위치를 고정해 다시 제작했다. 하강 시작 07번의 초기안은 손이 너무 낮아 어깨 높이를 지나는 자세로 다시 제작했다. 초기 시도는 `source/v01/frameNN_attemptNN.png`에 남겼다.

01번의 초기 준비 손이 너무 높아 낮은 위치에서 팔을 당기는 자세로 수정했다. 11번의 초기 복귀 후보가 손을 다시 올려 재시전처럼 보였으므로, 10·12번을 참조해 손을 골반 아래에 둔 복귀 자세로 다시 제작했다. 총 21회 생성·수정 중 15개를 채택하고, 00번은 마스터 원본을 사용했다. 생성 원본은 모두 1254×1254였고 전체 캔버스를 1024×1024로 정규화했다.

## 최종 확인

- 셀 16개 모두 1024×1024 RGBA이며 각 PNG의 SHA-256이 서로 다르다. 00번은 Idle 마스터 파일과 완전히 동일하다.
- 모든 alpha>0 픽셀의 사방 여백 검사 통과. 전체 최소 여백은 65px이며 64px 안전 영역 밖 픽셀은 0개다. 연결된 반투명 천·발광을 잘라내지 않았다.
- 4096×4096 시트에서 16개 셀을 재추출해 각 원본 셀과 픽셀 완전 일치를 확인했다.
- 밝은 바탕·어두운 바탕의 전체 프레임 정적 비교와 주요 자세 비교를 확인했다. 화면 왼쪽 팔만 시전하고, 반대팔은 낮게 유지한다.
- Unity MCP로 실제 임포트 후 Sprite Mode Multiple, 16개 이름·1024 사각형·Center (512,512), PPU 24를 전부 확인했다. 최대 크기 4096, Uncompressed, Bilinear, Clamp, Mipmap Off. [Unity 확인 값](AirBoss_Cast_OneArm_UpDown_v01.unity.json).
- 임포트 후 Unity 콘솔 Error 조회 결과 0개. 새 클립·Animator·프리팹·씬·게임 코드는 만들지 않았다.
- 기존 씬·AirBoss 애니메이션·아트 참조 등 보호 대상 245개 파일의 전후 SHA-256 일치. [전후 검사](AirBoss_Cast_OneArm_UpDown_v01.protected_after.json).
- Unity PNG GUID: `750d927fb1ce47ba98ec978eb722fa4c`.
- 시트 SHA-256: `18b60d0f1154d85f026f6dc63a81fa9122af4104d392df3919984214c8f15ef7`.

## 파일

- [개별 프레임](../../../ArtSource/AirBoss/cast_onearm_updown/frames/v01/)
- [제작 시트](../../../ArtSource/AirBoss/cast_onearm_updown/sheets/AirBoss_Cast_OneArm_UpDown_4x4_1024_v01.png)
- [Unity PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_4x4_1024_v01.png)
- [어두운 바탕](AirBoss_Cast_OneArm_UpDown_v01_contact_dark.png) · [밝은 바탕](AirBoss_Cast_OneArm_UpDown_v01_contact_light.png)
- [주요 자세 비교](AirBoss_Cast_OneArm_UpDown_v01_keyposes.png)
- [프롬프트](AirBoss_Cast_OneArm_UpDown_v01.prompts.json) · [픽셀 검사](AirBoss_Cast_OneArm_UpDown_v01.metrics.json)

## 범위

준비·상승 00~06, 하강 07~09, 후행·복귀 10~15로 구성했다. 기획서의 1.2초 / 0.5초 / 1.5초는 향후 타이밍 검토용이며 이번 제작에 클립은 포함하지 않는다. 실제 게임 재생·최종 표시 크기·아트 승인은 별도 검토 대상이다. 다른 동작·Animator·씬·게임 코드는 변경하지 않는다.
