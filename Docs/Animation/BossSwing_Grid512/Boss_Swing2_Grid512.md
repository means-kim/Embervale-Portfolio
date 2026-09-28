# Boss_Swing2 — 2048×1024 / 512 셀 정렬 수정

> 후속 수정: [Idle·Walk 얼음 색 및 복귀 자세 보정](../BossSwing_IceMatch/Boss_Swing2_IceMatch.md). 아래 위치 비교는 정렬 당시 이력이며, 현재 0번·7번 프레임은 Idle_0과 동일한 픽셀이다. 시트·셀·피봇 규격은 유지한다.

## 결과
- [Boss_Swing2.png](../../../Assets/04.Sprites/Boss/Boss_Swing2.png): 실제 2048×1024 RGBA.
- 4열 × 2행, 8개 셀 모두 512×512.
- 모든 Sprite의 피봇은 Bottom Center (0.5, 0), 픽셀 좌표 (256, 0). PPU 50.
- Sprite Editor 기본 Slice 설정도 Grid by Cell Count 4×2 / Bottom Center / Offset 0 / Padding 0으로 저장.
- PNG와 meta의 GUID 및 8개 Sprite ID를 보존. [Boss_Swing2.anim](../../../Assets/05.Animation/Boss/Boss_Swing2.anim) 참조 유지.
- 기존 Swing/Idle/Walk PNG·meta, 기존 애니메이션·프리팹·씬·게임 코드는 수정하지 않음.

## 기준 확인
- 사용자 지정 [Idle 백업](../../../Backups/BossSprites_before_2048_grounded/Boss_Idle.png), [Walk 백업](../../../Backups/BossSprites_before_2048_grounded/Boss_Walk.png)을 실제 이미지로 확인.
- 백업 이미지는 각각 4096×2048, 셀 1024×1024, PPU 100.
- 백업의 개별 Sprite 피봇: Idle은 (0.5,0), Walk는 (0.5,0.5)로 서로 달랐음. 이 차이를 그대로 복제하지 않고 [기존 변환 보고서](../../../Backups/BossSprites_before_2048_grounded/conversion_report.json)와 현재 적용된 Idle/Walk의 공통 기준을 사용.
- 현재 Idle/Walk는 2048×1024 / 셀 512×512 / Bottom Center / PPU 50. 기준 Idle 0의 비투명 영역은 셀 (35,160)~(454,512).
- 수정 Swing2 0의 비투명 영역은 (33,160)~(459,512), 마지막 프레임은 (36,160)~(460,512). 기존 포즈·세부 형태 차이는 유지.
- [기준 위치 비교](Idle_Walk_Swing2_Alignment.png): 왼쪽부터 Idle 0, Walk 0, Swing2 0, Swing2 7. 모두 같은 셀과 피봇. 청록 세로선은 X256, 하단선은 발 기준선.

## 처리 방식
- 사용자 명시 선택: 새로 생성하지 않고 기존 그림을 프로그램으로 리사이즈·재배치.
- 최초 Swing2의 실제 크기 1774×887 및 개별 Rect/Pivot 방식은 이번 요청의 4×2 균등 분할에 맞지 않았으므로 대체.
- 기존 8개 포즈의 픽셀을 같은 배율 50/44로 등비 리샘플링하여 PPU 변경 후 월드상 크기를 유지. 재생성·프레임별 크기 정규화·가로 찌그러뜨림 없음.
- 비투명 경계 계산을 방해하던 alpha 8/255 미만의 미세 배경 노이즈만 제거하고, 경계 색 번짐을 피하도록 premultiplied-alpha 보간.
- 기존 몸 기준 X를 셀 중앙 피봇 X256에 반영. 셀 밖으로 나갈 팔/손끝을 보존하기 위해 2번 프레임은 왼쪽 12.80px, 5번 프레임은 오른쪽 10.45px만 이동.
- 각 프레임의 실제 그림 하단을 셀 하단에 정렬. 포즈에 따른 발 간격·몸 숙임은 유지.
- 위치 이동만큼 Swing2 클립의 Hurtbox/SwingHitbox 위치 키를 보정. 타격 시간·크기·각도·전체 길이·EndAttack 이벤트는 유지. 게임 코드·후딜레이 데이터는 미수정.
- 픽셀 확대는 세부 묘사를 새로 생성한 것이 아님.

## 검증
- PNG 실제 크기 2048×1024, 8개 Sprite Rect 512×512, 피봇 (256,0), PPU50 확인.
- 8개 셀 모두 투명 하단 여백 0px. 불투명 몸체 경계는 안티앨리어싱에 따라 하단 1~2px 위이며 Idle/Walk와 같은 접지 기준.
- 모든 셀 좌우에 최소 1px 투명 여백. 팔·손끝이 인접 셀로 넘어가지 않음.
- 스프라이트 GUID/로컬 ID 8개 보존. 13개 클립 시점의 참조 및 0.60~0.82초 공격 활성 확인.
- Unity 임시 프리팹 좌우 렌더 확인, 기존 활성 씬 dirty=false.
- 실제 플레이의 이동·피해·경사·점프 회피 검증은 범위 외.

## 자료
- [재생 미리보기](../BossSwing_GroundSweep_v01/Boss_Swing2_Preview.html)도 새 규격으로 갱신.
- [변환 수치](Conversion_Report.json), [클립 샘플](Swing2_Samples.json).
- 수정 전 PNG/meta/anim/meta는 [백업 폴더](../../../Backups/BossSwing2_before_grid_2048/)에 보존.

