# AirBoss Move_Bank v01 — 멀티스프라이트

2026-09-30. 사용자 요청 범위는 Move_Bank 멀티스프라이트다. 클립·Animator·씬은 제작하거나 수정하지 않는다.

## 산출물

- [Unity용 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Move_Bank_4x4_1024_v01.png)와 같은 경로의 `.meta`.
- 전체 프레임 정적 비교: [어두운 바탕](AirBoss_Move_Bank_v01_contact_dark.png), [밝은 바탕](AirBoss_Move_Bank_v01_contact_light.png).
- [개별 1024 셀](../../../ArtSource/AirBoss/move/frames/v01/), [생성 원본](../../../ArtSource/AirBoss/move/source/Move_Bank_v01/), [ArtSource 시트](../../../ArtSource/AirBoss/move/sheets/AirBoss_Move_Bank_4x4_1024_v01.png).
- [제작 기준](../../../ArtSource/AirBoss/move/Move_Bank_Production_v01.md), [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md), [기획서](../../../ArtSource/AirBoss/Aerial%20Boss%20Design%20Document%20v1.md).

## 구성

오른쪽 이동을 기준으로 만든 16장이다. 상단부터 왼쪽→오른쪽 순으로 읽는다. 왼쪽 이동은 향후 좌우 반전하여 사용한다.

| 구간 | 셀 | 용도 |
| --- | --- | --- |
| 진입 | 00~03 | Idle에서 오른쪽으로 기울며 이동 자세로 진입 |
| 이동 유지 | 04~11 | 기울기를 유지하고 손목·천의 흐름이 변하는 반복 후보 |
| 감속·복귀 | 12~15 | 본체를 세우고 천이 뒤늦게 정리되며 Idle로 복귀 |

**00~15 전체를 반복하는 이동 루프가 아니다.** 향후 클립 구성은 `00~03 → 04~11 반복 → 12~15 → Idle`이다. 4/8/4장 배분은 이번 제작안이다. 기획서의 유지 루프 2.0초를 초기 검토값으로 보존하되 이번에는 노출 시간·클립 길이를 설정하지 않았다.

00은 현재 Idle 마스터의 PNG 파일을 그대로 복사했다. 01~15는 해당 동작을 새로 그린 프레임이며 중복·역순 복제나 반투명 교차로 대체하지 않았다. 일반 이동이므로 공격 시전 구체나 큰 휘두름 잔상을 추가하지 않고, 가면·손목의 기존 발광을 유지했다.

## 규격과 제작 이력

- 셀 1024×1024, 시트 4096×4096 RGBA, 4열×4행.
- PPU 24, 각 셀 Center (512,512). Unity Rect는 `X=(i%4)*1024`, `Y=(3-floor(i/4))*1024`, 크기 1024×1024.
- Multiple 16개, Full Rect, Max Size 4096, Bilinear, Clamp, Uncompressed, Mipmap Off.
- 내장 **image_gen**으로 마스터·연결 포즈를 참조하여 제작했다. [전체 프롬프트](AirBoss_Move_Bank_v01.prompts.json)에 참조 순서와 수정 요청을 보존한다.
- 요청은 1024×1024였으며 실제 생성 출력은 1254×1254였다. 전체 캔버스를 공통 비율 1024/1254로 정규화했다. 외곽 Trim, 프레임별 체격 맞춤 확대·축소, 좌표 재중앙정렬은 하지 않았다.
- 가시 영역에서 16px 넘게 떨어진 독립 alpha<8 덩어리만 정리한다. 연결된 천·발광 가장자리는 보존한다. 처리 수치·여백·해시는 [측정 JSON](AirBoss_Move_Bank_v01.metrics.json), 재현 절차는 [조립 스크립트](Build_AirBoss_Move_Bank_v01.ps1)에 있다.
- 02·12의 첫 생성본은 전환 포즈/여백 문제로, 09의 첫·두 번째 생성본은 상단·하단 여백 문제로 채택하지 않았다. 해당 원본은 `attempt01_not_selected`와 `attempt02_not_selected` 이름으로 보존했다.

## 검증 범위

- 16개 고유 PNG, 셀 1024×1024, 시트 4096×4096 RGBA 확인. 00과 마스터 파일 해시 일치.
- 모든 셀의 alpha>0 픽셀이 사방 64px 안전 여백 안에 있으며, 전체 최소 여백은 64px다. 손·천·발광을 경계에서 잘라서 통과시키지 않았다.
- 각 셀과 시트에서 다시 추출한 RGBA 픽셀 일치. ArtSource와 Assets 시트 해시도 일치한다: `6B004C0B3D7AFA016F358FC0D159A082212CA32047ACAB3A6E2E8C59B3028AF0`.
- 어두운·밝은 바탕의 정적 비교로 자세 방향과 투명 가장자리를 확인했다. 생성 그림 사이의 천 갈래·갑옷 세부 묘사 차이는 남아 있으며 실제 재생 속도에서의 자연스러움은 후속 검토 대상이다.
- Unity 6000.3.22f1에서 Multiple 16개, 실제 텍스처 4096×4096 RGBA32, PPU 24, 피벗 (512,512), 각 Rect, Physics Shape 0 확인. [Unity 검증 JSON](AirBoss_Move_Bank_v01.unity.json). Console Error 0개.
- 새 텍스처 GUID는 `a253665387b5a2a4d926577070f4d6a4`다. 기존 Idle 클립·Animator와 각 meta의 [전후 해시 일치](AirBoss_Move_Bank_v01.animation_unchanged.json)를 확인했다. Move 클립은 생성하지 않았다. 씬·기존 Idle 자산·전역 설정도 변경하지 않았다.

정적 자세 비교·임포트 검증과 실제 재생의 자연스러움은 별개다. 11→04 반복 연결의 실제 속도, 15→Idle 복귀, 왼쪽 반전 상태에서 비반전 Idle로 돌아오는 연결과 게임 화면 Scale은 미검증이다. 사용자 검토 전 최종 승인본으로 표시하지 않는다.
