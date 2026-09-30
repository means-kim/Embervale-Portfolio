# Move_Bank_Left v01 제작 기준

2026-09-30. 사용자가 비대칭 디자인 때문에 Move_Bank의 좌우 반전 재사용을 철회하고 왼쪽 전용 멀티스프라이트 제작을 요청했다. 이 지시는 Move_Bank에 적용한다.

- [Idle 마스터](../idle/AirBoss_Idle_Master_image2.png)의 가면 균열·갑옷·양손·천 부착점·광원 방향을 비반전 상태로 보존한다. 몸은 화면 왼쪽으로 기울고 느슨한 천은 오른쪽으로 후행한다. 부착점과 바람에 휘어진 자유단을 구분한다.
- 기존 오른쪽 Move_Bank를 반전하거나 전체 회전한 그림으로 대신하지 않는다. 움직임은 관절과 천의 새 포즈로 그린다.
- 오른쪽 버전과 같은 16장 구성: 진입 00~03, 이동 유지 04~11, 감속·복귀 12~15. 00은 원래 Idle 마스터 파일 그대로이며 15는 같은 비반전 Idle로 돌아온다.
- 셀 1024×1024, 시트 4×4·4096×4096 RGBA, PPU 24, Center (512,512), 사방 64px 완전 투명 여백. 프레임별 Trim·체격 맞춤 확대축소·재중앙정렬 금지.
- `AirBoss_Move_Bank_Left_v01_00`~`15`로 분할한다. Unity용 PNG는 `Assets/04.Sprites/AirBoss/AirBoss_Move_Bank_Left_4x4_1024_v01.png`.
- Left 전용 이미지는 향후 `flipX=false` 상태로 사용한다. 실제 씬의 Flip 값이나 Animator는 이번에 수정하지 않는다.
- 기존 오른쪽 PNG·Idle·애니메이션 클립·사용자 씬 변경 사항은 보존한다. 새 애니메이션 클립은 만들지 않는다.

[공통 규칙](../AirBoss_Multisprite_Production_Rules_v1.md)과 [기획서](../Aerial%20Boss%20Design%20Document%20v1.md)를 함께 따른다. 일반 이동에는 시전 구체·큰 휘두름 잔상을 추가하지 않는다. 00~15 전체를 반복하지 않으며 실제 재생·최종 자연스러움은 스프라이트 검토 뒤 별도로 확인한다.

## v01 제작 상태

16장 제작과 Unity Multiple 분할을 완료했다. 모든 셀의 64px 투명 여백, PPU 24, 중앙 Pivot, 4096×4096 시트 등록을 확인했다. 기존 클립·Animator·씬은 보존했고 새 클립은 만들지 않았다. 정적 검토 완료이며 실제 재생과 상태 전환은 미검증이다. [결과와 검증 기록](../../../Docs/Boss/AirBoss/AirBoss_Move_Bank_Left_v01.md), [전체 프레임 미리보기](../../../Docs/Boss/AirBoss/AirBoss_Move_Bank_Left_v01_contact_dark.png)를 참고한다.
