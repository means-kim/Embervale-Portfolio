# AirBoss Move_Bank_Left v01

2026-09-30. 사용자 요청: 기존 비대칭 Move_Bank를 Flip해서 쓰는 대신 왼쪽 전용 멀티스프라이트를 제작한다. 새 클립은 만들지 않는다.

## 산출물과 방향

- [Unity용 시트](../../../Assets/04.Sprites/AirBoss/AirBoss_Move_Bank_Left_4x4_1024_v01.png)와 동일 경로 `.meta`.
- [어두운 바탕 전체 비교](AirBoss_Move_Bank_Left_v01_contact_dark.png), [밝은 바탕 전체 비교](AirBoss_Move_Bank_Left_v01_contact_light.png).
- [개별 셀](../../../ArtSource/AirBoss/move_left/frames/v01/), [생성 원본](../../../ArtSource/AirBoss/move_left/source/Move_Bank_Left_v01/), [ArtSource 시트](../../../ArtSource/AirBoss/move_left/sheets/AirBoss_Move_Bank_Left_4x4_1024_v01.png).
- [제작 기준](../../../ArtSource/AirBoss/move_left/Move_Bank_Left_Production_v01.md).

왼쪽 전용 포즈를 새로 그렸다. 몸은 화면 왼쪽으로 기울고 천의 자유단은 오른쪽으로 흐르며, 원래 비반전 Idle의 가면 균열·갑옷·양손·천 부착점·광원 방향을 기준으로 제작했다. 원래 왼쪽 어깨 뒤의 높은 천 다발은 같은 부착점에서 뒤쪽으로 휘어진다. 완성 자산은 `flipX=false`로 사용한다. 이번에 씬이나 Animator의 Flip 설정을 바꾸지는 않았다.

## 배열과 사양

| 구간 | 셀 | 역할 |
| --- | --- | --- |
| 진입 | 00~03 | 원래 비반전 Idle에서 왼쪽 이동 자세로 진입 |
| 이동 유지 | 04~11 | 왼쪽 기울기를 유지하고 천·손목의 흐름이 변하는 반복 후보 |
| 감속·복귀 | 12~15 | 본체와 천이 원래 비반전 Idle 쪽으로 정리 |

00은 현재 Idle 마스터 파일 그대로다. 01~15는 새 그림이며 기존 오른쪽 그림의 좌우 반전이나 기계적 전체 회전으로 대신하지 않는다. 전체 16장을 한 루프로 반복하는 규격이 아니며 후속 반복 대상은 04~11이다.

셀 1024×1024, 시트 4096×4096 RGBA, 4열×4행. PPU 24, Center (512,512), 사방 64px 완전 투명 여백. Unity Rect는 `X=(i%4)*1024`, `Y=(3-floor(i/4))*1024`, 크기 1024×1024다. 일반 이동이므로 시전 구체나 큰 공격 잔상을 추가하지 않고 원래 손목·가면의 발광을 유지한다.

## 제작 기록

내장 **image_gen**을 사용했다. [프롬프트와 참조](AirBoss_Move_Bank_Left_v01.prompts.json), [픽셀 검사 및 해시](AirBoss_Move_Bank_Left_v01.metrics.json), [조립 스크립트](Build_AirBoss_Move_Bank_Left_v01.ps1)를 보존한다.

요청 캔버스는 1024×1024였으나 실제 생성 출력은 1254×1254였다. 생성 출력은 캔버스 전체를 공통 비율 1024/1254로 정규화했으며, 프레임별 실루엣 Fit·Trim·재중앙정렬을 하지 않았다. 가시부에서 16px 넘게 떨어진 독립 alpha<8 덩어리만 정리하고, 연결된 천·발광은 보존했다. 여백을 넘는 천은 잘라내지 않고 다시 그렸다. 01·03·08·12는 여백과 초기 자세를, 03·13은 전환 포즈의 간격을 추가 수정했다. 미채택 원본도 source의 `attemptNN_not_selected` 파일에 보관한다.

## 검증 결과

- 16장 모두 서로 다른 파일이며 00은 Idle 마스터와 SHA256이 같다. 개별 셀과 시트의 해당 영역은 픽셀 단위로 일치한다.
- 모든 셀의 사방 완전 투명 여백이 64px 이상이다. 어두운 바탕과 밝은 바탕에서 전체 프레임을 정적으로 확인했다.
- ArtSource 시트와 Assets 시트의 SHA256은 모두 `4DA8598C13D81EF73B9A84504EC1A1EC5948DBEFB2F59DD9306CFCDE80707BB9`다.
- Unity MCP 재연결 후 16개 Sprite 등록을 확인했다. Multiple, PPU 24, 1024×1024 Rect, Center (512,512), Max Size 4096, Mipmap Off, Uncompressed, Bilinear, Clamp, Full Rect, 자동 Physics Shape 없음. [Unity 조회 결과](AirBoss_Move_Bank_Left_v01.unity.json).
- PNG GUID는 `4301e1341970ba74489935094bde801b`. 등록 후 Unity 콘솔 오류 조회 결과는 0건이다.
- 기존 오른쪽 PNG, Idle 마스터, 두 기존 클립과 meta, Animator Controller와 meta, 사용자 MinsuScene의 총 9개 파일이 작업 전후 해시와 일치한다. [보존 확인](AirBoss_Move_Bank_Left_v01.protected_after.json). 새 애니메이션 클립은 생성하지 않았다.

확인 범위는 정적 이미지·픽셀 규격·Unity 분할이다. 실제 재생, 04~11 반복 연결, Idle 전환 및 최종 게임 화면의 자연스러움은 아직 확인하지 않았다. 프레임 사이 천·갑옷 세부 묘사의 일관성은 재생 검토에서 추가 평가한다.
