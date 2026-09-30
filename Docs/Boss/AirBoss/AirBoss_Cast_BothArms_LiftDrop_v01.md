# AirBoss Cast_BothArms_LiftDrop v01

2026-09-30. 사용자 요청으로 양팔 상승·강한 하강 시전의 멀티 스프라이트만 제작했다. 다른 동작, Animation Clip, Animator, 씬, 공격/피해 코드와 독립 공격 VFX는 제작하지 않았다.

## 산출물

- [Unity용 시트](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_LiftDrop_4x4_1024_v01.png) 및 동일 경로 `.meta`.
- [어두운 바탕 16장 비교](AirBoss_Cast_BothArms_LiftDrop_v01_contact_dark.png), [밝은 바탕 비교](AirBoss_Cast_BothArms_LiftDrop_v01_contact_light.png).
- [개별 프레임](../../../ArtSource/AirBoss/cast_botharms_liftdrop/frames/v01/), [생성 원본 및 수정 전 후보](../../../ArtSource/AirBoss/cast_botharms_liftdrop/source/v01/), [ArtSource 시트](../../../ArtSource/AirBoss/cast_botharms_liftdrop/sheets/AirBoss_Cast_BothArms_LiftDrop_4x4_1024_v01.png).
- [제작 기준과 포즈 구성](../../../ArtSource/AirBoss/cast_botharms_liftdrop/Cast_BothArms_LiftDrop_Production_v01.md).

## 실제 규격

16장, 셀 1024×1024 RGBA, 4열×4행, 시트 4096×4096. 00~03 / 04~07 / 08~11 / 12~15 순으로 위에서 아래, 왼쪽에서 오른쪽으로 읽는다. Unity Rect는 `X=(i%4)*1024`, `Y=(3-floor(i/4))*1024`, 크기는 1024×1024다. 각 셀의 Pivot은 Center (512,512), PPU 24다.

00은 기존 Idle 마스터 파일을 그대로 복사했다. 01~15는 내장 **image_gen**으로 생성한 서로 다른 포즈다. 양팔 준비 → 상승·구체 응축 → 홀드 → 하강·잔상 → 천의 후행과 잔광 감쇠 → Idle에 가까운 복귀를 표현했다. 구체와 손팔 잔상은 Body PNG에 포함했다. 지면·그림자·빨간 가이드·실제 지면 공격은 포함하지 않았다.

기획서의 3.4초는 향후 타이밍 검토를 위한 초기안이다. 이번 시트에는 재생 시간이나 공격 이벤트가 내장되어 있지 않으며 클립은 만들지 않았다. 전체 16장을 Idle처럼 반복하는 용도로 확정하지 않는다.

## 제작 및 수정 기록

현재 Idle 마스터의 SHA256은 `b67ebb82308ab8fa7d8a5c74c10f59fd7761e7335303aef3acf6c2f22587d722`이며 v02 보존본과 일치했다. 마스터와 콘티 PNG, 기존 Idle·좌우 Move 시트를 실제 이미지로 열어 확인했다. 콘티 3번은 동작 참조이며, 그 안의 12장/약 3초 및 VFX 별도 표기는 현재 규칙과 이번 16장 구성을 덮어쓰지 않는다.

요청 캔버스는 1024×1024였고, 생성 결과의 실제 크기는 **1254×1254**였다. 생성 결과는 캔버스 전체에 동일한 `1024/1254` 비율을 적용해 셀로 정규화했다. 실루엣 기준 Fit, Trim, 프레임별 배율 보정, 재중앙정렬, 전체 이미지를 회전·이동하는 방식의 포즈 생성은 하지 않았다. 실제 해상도와 alpha 측정값은 [metrics](AirBoss_Cast_BothArms_LiftDrop_v01.metrics.json)에 기록했다.

가시부 alpha≥8에서 16px 넘게 떨어진 **독립 alpha 1~7 성분만** 정리했다. 연결된 부분 투명 천·발광은 유지했으며 alpha를 일괄 문턱 처리하지 않았다. 손·천·발광이 64px 안전 여백을 넘으면 그림을 다시 생성했다. 02~08의 일부 천 끝/발광을 수정했고, 상승·하강 연결 포즈와 08의 머리 위 구체 재등장 문제를 보완했다. 05의 천은 인접 홀드 자세의 곡선과 비교해 재조정했다. 수정 전 생성 PNG는 `attemptNN` 파일로 남겼다.

전체 [프롬프트 기록](AirBoss_Cast_BothArms_LiftDrop_v01.prompts.json)과 [조립·검사 스크립트](Build_AirBoss_Cast_BothArms_LiftDrop_v01.cjs)를 보존했다. 생성 원본을 프로젝트에 복사했으며 `.codex` 저장 경로에만 의존하지 않는다.

## 검증 범위

- 16장 모두 1024×1024 RGBA이며 사방 64px 띠의 alpha>0 픽셀은 0개다. 최종 최소 여백은 67px다.
- 00은 마스터와 파일 해시가 일치한다. 16장 파일 해시는 모두 다르며, 시트에서 다시 추출한 각 셀은 개별 프레임과 픽셀 단위로 일치한다.
- 어두운 바탕과 밝은 바탕의 16장 비교 이미지를 실제로 열어 전조·하강·복귀 흐름 및 가장자리를 정적으로 확인했다.
- Unity MCP로 Multiple 16개, 실제 임포트 4096×4096, 1024 셀 Rect 순서, 각 Pivot (512,512), PPU 24를 확인했다. Max Size 4096, Mipmap Off, Uncompressed, Bilinear, Clamp다. [Unity 조회](AirBoss_Cast_BothArms_LiftDrop_v01.unity.json).
- 새 PNG GUID는 `69c0780de0064f2cab07183203c85807`이다. Assets와 ArtSource 시트의 SHA256은 `b41a52461f0b738f61d8a10b205b7804ba5862e65e1fb4c25f65f267673fa15d`다.

실제 게임 크기, 재생 속도, 프레임 전환의 자연스러움, Idle/Move 상태 전환은 **미검증**이다. 생성 프레임의 천 곡률·표면 질감과 손가락 가림에 미세한 차이가 있으며, 정적 검사를 재생 검증이나 사용자 최종 아트 승인으로 표시하지 않는다.

기존 씬·AirBoss 애니메이션 폴더·기존 AirBoss 시트 및 meta·Idle 제작 원본의 79개 파일은 작업 전후 해시가 모두 일치한다. [작업 전 해시](AirBoss_Cast_BothArms_LiftDrop_v01.protected_before.json), [작업 후 비교](AirBoss_Cast_BothArms_LiftDrop_v01.protected_after.json). 이번에 기존 파일의 추적 변경은 없다.
