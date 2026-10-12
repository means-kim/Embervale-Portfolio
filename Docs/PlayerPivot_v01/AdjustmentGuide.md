# 플레이어 몸통 피벗 적용 및 수동 조정 안내

2026-10-12. 사용자 승인: 몸통 기준으로 피벗을 정렬하고, 관련 기준점은 사용자가 직접 조정.

## 적용 범위

현재 프로젝트의 AnimationClip에서 참조하는 플레이어 이미지 137개 / Sprite 187개.
Idle v3 32프레임, Motions v4 101프레임, Attack/Attack2/Attack3/Attack4 시트의 54프레임.
공유 애니메이션과 Z_HyunseoTest 애니메이션이 같은 이미지를 쓰므로 두 곳에 함께 반영된다.
원본 PNG, 애니메이션 타이밍/참조, PPU, 잘린 영역, GUID와 Sprite local file ID는 유지했다.
씬, 프리팹, 게임 코드, 콜라이더, 공격 DATA는 수정하지 않았다.
미사용 과거 후보와 제작 원본/참고용 시트까지 일괄 재해석하지 않았다.

피벗은 실제 그림을 대조하여 선택한 몸통 내부의 시각 기준점이다. 이미지의 알파 외곽 중심이나 불꽃 중심이 아니다.
자세가 다른 프레임은 몸통 위치에 맞춰 다른 좌표를 사용하고, 불꽃만 흔들리는 루프는 같은 피벗을 유지한다.
DoubleJump의 회전 구간은 제작 당시의 회전 중심(512셀 좌상단 기준 256,286)을 유지한다.
최종 미세 위치 평가는 실제 Play Mode 피드백이 필요하다.

## 먼저 조정할 값

아래 값은 **Assets/03.Prefabs/Player/Player.prefab의 현재 설정**을 기준으로 계산했다.
Idle의 피벗은 512셀에서 기존 (256,410) → 새 (270,286), Unity 정규화 좌표 (0.52734375,0.44140625).
PPU 200에서 그림 전체가 오른쪽을 볼 때 Local X -0.07, Y -0.62 이동한다.

| 항목 | 기존 값 | 조정 시작값 | 설명 |
|---|---|---|---|
| Player의 CapsuleCollider2D Offset | (0,0.7) | **(-0.07,0.08)** | Idle 그림에 대한 기존 충돌 영역 위치를 보존하는 값. Size (1,1.5)는 우선 유지 |
| FootstepPoint Local Position | (0,0,0) | **(-0.07,-0.62,0)** | 기존 발밑 표시 기준을 보존. 그림 발 끝에 맞춰 미세 조정 |
| PlayerLight 등 몸을 따라야 하는 자식 | 오브젝트별 현재 값 | 기존 값 + (-0.07,-0.62,0) | 기존 그림과의 상대 위치를 유지하려는 자식에만 적용 |

이 값은 Idle에서 기존 관계를 보존하는 시작값이다. **대시·앉기 등 모든 자세에 콜라이더를 완전히 맞추는 값은 아니다.**
발 자체의 애니메이션 변형은 유지된다. 이동/착지 중 발과 콜라이더 하단을 보며 최종 조정한다.
PlayerMove는 Player 루트의 X Scale을 반전하므로 위 Local X는 왼쪽 방향에서 자동 반전된다.
콜라이더 Offset/자식 Local Position에는 Scale 1.5를 다시 곱하지 않는다.

### 시작 위치가 내려간 경우

피벗만 바꿨기 때문에 같은 Transform Position에서 Idle 그림은 Scale 1.5 기준 World X -0.105 / Y -0.93 이동한다.
같은 장소에 같은 그림을 유지하려면, 위 콜라이더/자식 조정을 한 뒤 오른쪽을 보는 Player의 Transform Position을
**X +0.105, Y +0.93** 올린다. 왼쪽을 보는 경우 X 보정의 부호는 반대다.
실제 씬 인스턴스의 부모 Transform/Scale/회전이 있으면 그 TransformVector 기준으로 계산한다.
이동 직전 저장된 리스폰/체크포인트 위치도 새 몸통 원점에 맞춰 확인한다.

## 지면·벽 감지

현재 공유 PlayerJump는 콜라이더 접촉을 사용한다. 별도의 GroundCheck Transform을 조정하는 구조가 아니다.
PlayerWall도 Rigidbody2D/콜라이더 기반 감지이므로, 먼저 콜라이더 Offset을 맞춘 뒤 접지·벽붙기를 확인한다.
발밑 감지점을 별도로 둔 다른 플레이어/씬 변형이 있다면 새 Idle 발 위치에 맞춰 같은 Local 이동량을 적용한다.

## 공격 SpawnOffset

Assets/07.Datas/Player의 공격 DATA가 생성 위치를 갖고 있다. 현재 코드의 공식은:

`생성 월드 위치 = Player.position + (방향 × SpawnOffset.x × AttackScale, SpawnOffset.y × AttackScale)`

**Player Transform Scale 1.5를 자동으로 곱하지 않는다.** 현재 공유 프리팹의 AttackScale은 1이다.
기존 생성 위치가 새 Idle 몸에 대해 같은 위치를 유지하도록 계산한 시작값은
`SpawnOffset + (-0.105,-0.93) / AttackScale`이다. 이것은 생성 위치의 보정이지 공격 궤적/판정 크기의 최종 검증값은 아니다.

| DATA | 기존 SpawnOffset | AttackScale=1, Player Scale=1.5 기준 시작값 |
|---|---|---|
| LeftHorizontalAttack | (0,-2.6) | (-0.105,-3.53) |
| RightHorizontalAttack | (0,-4.4) | (-0.105,-5.33) |
| DownAttack | (0,-3) | (-0.105,-3.93) |
| FireBallAttack | (0,-1.6) | (-0.105,-2.53) |
| UpAttack | (1,-1) | (0.895,-1.93) |

공격 자세별 피벗도 몸통에 맞춰 정렬되어, 옛 발 원점에서 보던 모든 자세의 위치가 단순한 하나의 이동량으로 같아지지는 않는다.
위 값 적용 후 실제 공격 프레임의 손·불꽃·투사체와 독립 공격 프리팹의 판정 위치를 대조해 맞춘다.
공격 프리팹/VFX 이미지의 피벗이나 Collider Size는 이번에 수정하지 않았다.

차징 점프 조준/발사 이펙트의 원점은 현재 Collider.bounds.center를 사용하므로 콜라이더 Offset 조정 후 함께 확인한다.
대시 이펙트는 Player Transform 원점을 사용한다. 새 원점은 몸통이므로 표시 위치를 Play Mode에서 확인한다.
카메라 추적 대상이 Player라면 추적 기준도 발 원점에서 몸통 원점으로 바뀌므로 구도/Follow Offset을 확인한다.

## 검증과 검토 자료

- [변경 전후 회전 비교](Comparison.png): 같은 Transform 원점에서 기존/새 피벗을 비교한 이미지 기반 아트 검토. 실제 Unity Play 캡처가 아니다.
- Anchors_01~04.png: 분홍색 기존 피벗, 초록색 새 피벗. 자세별 선택 근거.
- [Unity 가져오기 검증](UnityVerification.json): 187개 피벗, Sprite ID/로컬 파일 ID, 영역/이름, PPU 확인.
- [파일 보존 검증](FileVerification.json): PNG 해시 유지, 피벗/정렬과 Unity가 채운 미지정 최상위 internalID/spriteID 외 .meta 항목 유지 확인. 채워진 기본 메타데이터 ID와 실제 애니메이션 참조용 Sprite local file ID는 별개이며, 후자는 변경되지 않았음을 Unity에서 검증했다.
- [프레임별 정확한 좌표](Plan.json), [원래 Unity 값](OriginalImporter.json), Before/ 원본 .meta 백업.

Play 검증은 하지 않았다. 사용자가 기준점 조정 후 Idle/Run 접지, 점프·착지, 좌우 8방향 공중 대시,
차징 점프 발사/종료, 벽붙기, 각 공격 생성 위치와 잔상을 확인한다.

## 복구와 후속 제작

Restore.cs는 새 스크립트 컴포넌트를 추가하는 도구가 아니라, 연결된 Editor의 eval_file로 실행할 복구 스니펫이다.
Play Mode가 아닌 상태에서 `unity command eval_file --project-path C:/Unity/2d-team08 --caller plugin --skill sprite-editor --file C:/Unity/2d-team08/Docs/PlayerPivot_v01/Restore.cs`로 원래 피벗/정렬만 복원한다.
모든 대상에서 피벗 편집 기능과 기존 Sprite ID를 먼저 확인한다. 복구 후 사용자가 조정한 기준점은 별도로 되돌려야 한다.
이전 제작 스크립트/Verification/Motions.json의 공통 발 피벗 기록은 과거 제작 사양이며 이번 import 결과를 나타내지 않는다.
소재를 다시 생성/등록할 경우에는 이 Plan.json의 자세별 피벗을 다시 반영해야 한다.
