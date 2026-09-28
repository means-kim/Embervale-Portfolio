# Boss_Swing2 — 낮은 쓸기 동작

> 현재 적용: [털·눈·얼음 디테일 수정 v02](../BossSwing_DetailMatch_v02/Boss_Swing2_DetailMatch_v02.md). 2048×1024·512 셀·공통 피봇과 기존 클립을 유지한다.

> 최신 색 보정: [Idle·Walk 얼음 색 및 복귀 자세 보정](../BossSwing_IceMatch/Boss_Swing2_IceMatch.md). 아래 생성·연결 상태는 최초 작업 이력이다.

> 최신: [2048×1024 / 512 셀 정렬 수정](../BossSwing_Grid512/Boss_Swing2_Grid512.md). 아래 1774×887·PPU44는 최초 제작 이력이며 현재 PNG는 2048×1024·PPU50·공통 하단 중앙 피봇으로 변경됨.


- 최신 사용자 요청: 코드를 수정하지 않고 기존 Swing 애니메이션·스프라이트를 보존하며 Swing2로 별도 제작.
- 앞서 수정했던 BossSwingAttack.cs는 작업 전 백업과 바이트 단위로 동일하게 복구. 임시 효과음도 제거.
- [새 스프라이트](../../../Assets/04.Sprites/Boss/Boss_Swing2.png), [새 애니메이션](../../../Assets/05.Animation/Boss/Boss_Swing2.anim).
- [재생 검토](Boss_Swing2_Preview.html), Unity 임시 프리팹 렌더 [왼쪽](Swing2_Left.png) / [오른쪽](Swing2_Right.png).
- 기존 Animator Controller, 보스 프리팹, 씬, 공격 데이터는 이 작업에서 수정하지 않음. 자동으로 기존 Swing을 교체하지 않음.

## 이미지 / Import
- built-in imagegen으로 원본 Boss_Swing.png를 참조하여 제작.
- 요청 크기 2048×1024, 실제 출력 **1774×887 RGBA**. 네이티브 512 프레임으로 표기하지 않음.
- 실제 출력의 여백에 맞춰 8개 Sprite Rect와 개별 Pivot을 지정. PPU **44**, 원본 PPU 50과 구별.
- 실체 높이와 발 기준선을 원본과 비교하여 맞춤. Transform Scale은 변경하지 않음.
- Sprite 이름: Boss_Swing2_0 ~ Boss_Swing2_7.
- 완전한 픽셀 단위 자세 일치는 아님. 같은 팔의 낮은 휘두르기와 동일한 캐릭터 형태를 우선.

## 클립
- 전체 1.5초 / Samples 20. 스프라이트 키: 0, 0.10, 0.25, 0.40, 0.60, 0.70, 0.82, 1.10, 1.45초(마지막 유지).
- 기존 Colliders/SwingHitbox 계층을 사용. 위치·회전·크기·Enabled를 새 클립에 저장.
- 타격 0.60~0.82초. 준비/회수 때 OFF. Slam/Charge 판정 OFF.
- 0.60초: 중심(-2.8,1.6), 크기(3.6,1.2), 각도30도.
- 0.70초: 중심(-3.1,1.45), 크기(3.8,1.15), 각도24도.
- 몸을 낮추는 프레임에서 Hurtbox 높이·중심도 별도 조정.
- 원본의 EndAttack 이벤트 1.45초를 보존. 새 이벤트 함수나 VFX·오디오 코드는 추가하지 않음.
- 코드의 후딜레이·쿨타임·거리·데미지는 변경하지 않음.

## 확인
- Unity 6000.3.22f1에서 8개 Sprite 참조와 13개 시점의 판정 활성 상태 확인.
- 프리팹의 임시 복제본에서 좌우 방향 0.70초 렌더 확인. 활성 씬 dirty=false 유지.
- 원본 Swing 애니메이션·스프라이트 및 게임 코드 보존 확인.
- 기존 Console에 UnityEditor.Graphs.Edge.WakeUp의 NullReferenceException 1건이 있었음. 게임 스크립트 컴파일 오류는 확인되지 않음.
- 실제 플레이의 경사·발판 끝·피해 처리·점프 회피는 미검증. 현재 씬 테스트 플레이어에는 Health가 없음.

## 사용
기존 보스의 동일한 Colliders 계층에서 사용할 수 있는 별도 클립이다. 채택할 때 기존 Animator의 Swing 상태 Motion에 Boss_Swing2를 연결하면 기존 IsSwing 트리거와 EndAttack 이벤트를 사용할 수 있다. 현재 연결은 원래 Boss_Swing으로 유지되어 있다.

## 생성 프롬프트
도구: built-in image_gen. 생성 후 프로젝트에 PNG와 meta를 보존.

### 최초 요청
Use case: precise-object-edit.
Edit target: the supplied Boss_Swing.png game sprite sheet. Create a replacement eight-frame low ground-sweeping claw attack for EXACTLY this ice-and-white-fur giant, matching the original painted rendering, colors, face, blue ice spines, black claws, proportions and camera angle.
Output a genuinely transparent PNG, preferably EXACTLY 2048x1024, arranged in a strict four-column by two-row grid of eight 512x512 cells. No grid lines, labels, shadows on background, VFX, snow, scenery or text. Each character must stay inside its own cell.
Critical registration: preserve the source's planted feet and identical scale in every cell, soles near y=508 within each cell, center between feet near x=290. Preserve the initial idle pose in cell 1 and return to the SAME pose in cell 8. All eight figures face LEFT like the source. Keep the same front/near arm (the arm on the viewer's right in idle) as the attacking arm throughout.
Action: a slight crouch and broad LOW sweeping claw movement grazing just above ground, NOT a straight chest-height punch and NOT an overhead slam. Bend elbow naturally; rotate shoulder and torso in a shallow low arc.
Reading order left-to-right top row then left-to-right bottom row:
1 idle as original.
2 begin windup, bend knees slightly, near arm pulls back.
3 clear anticipation, near arm back and LOWERED, torso leaning toward ground, feet remain planted.
4 end windup, claws low beside right/rear foot ready to sweep toward left.
5 active early sweep, large near hand moving LOW across front of knees toward left, claws roughly 30-80 pixels above soles.
6 active maximum sweep toward left, claws near left/front floor edge (x around 30-120), arm stretched diagonally DOWN from shoulder, hand roughly 15-70 pixels above soles; face and torso remain visible above the forearm.
7 follow-through and begin retracting arm upward from low sweep.
8 return to exact original idle stance.
Keep all feet anchored, same character identity, no extra limbs, no cropping or overlap between frames. Never shrink the character to fit action. Leave no motion blur so each claw and arm is readable.

### 공격 팔 연속성 수정
Use case: precise-object-edit.
Image 1 is the edit target: eight-frame ground-sweep sprite sheet. Image 2 is the original character identity and planted-feet reference.
Fix ONLY action anatomy and registration of image 1. Preserve eight frames in a strict 4 columns x 2 rows grid, transparent PNG, requested output 2048x1024.
CRITICAL correction in cell 6 (bottom row second column): it currently extends the WRONG arm. The near shoulder, on the viewer's RIGHT and covered by the large blue shoulder crystal, must connect through a long thick WHITE FUR upper arm and forearm that crosses DIAGONALLY DOWN-LEFT IN FRONT OF the chest/belly and ends in the extended claw hand near the floor at the left of the cell. Remove the current near/right hand tucked behind on the right. The far arm stays bent and close to the far/left knee, partially hidden behind the attacking arm. Exactly two arms total. This is the SAME near arm that crosses the body in cell 5 (bottom left) and returns in cell 7 (bottom third).
Cell 5: lower the attacking near forearm and claw tips another small amount, so it passes across shins/knees instead of across chest.
Cell 4: keep the near hand low pulled back ready to sweep forward. Cells 5-6 form ONE continuous LOW shallow arc near the floor, not a punch, no switching arms.
Keep soles of feet at the same local baseline, near the bottom edge of EVERY cell, keep feet positions stable, same body scale, original ice giant identity, face and colors. Cell 8 is exactly the neutral idle pose of cell 1. Maintain crisp clean alpha edges with NO bright blue outlines/fringe, no floor shadow, no VFX, no scenery, no text, no grid. Make cells uniform, do not crop any body part.

