# 차지 점프 연결 방법

스크립트만 구현했다. 씬과 프리팹 연결 및 Play 검증은 사용자가 진행한다.

## 동작

- 가까운 IceProjectile이 있을 때 우클릭을 누르면 가장 가까운 투사체를 선택한다. 선택 거리는 플레이어 콜라이더 중심부터 투사체 콜라이더까지다.
- 시간 배율과 물리 시간 간격을 기존 값의 0.1배로 낮춘다. 플레이어와 선택된 투사체는 정지하고, 선택된 투사체의 수명도 정지한다.
- 플레이어 중심에서 마우스 방향으로 화살표를 표시한다. 버튼을 놓으면 시간을 복원하고 플레이어는 화살표 방향, 투사체는 반대 방향으로 발사한다.
- 기본 발사 속도 18, 유지 시간 0.2초다. 유지 중 중력과 일반 조작을 막고, 이후 기존 이동·중력을 복원한다. 기본 직선 이동 거리는 충돌이 없다면 약 3.6 월드 단위다.
- 투사체 반사 속도는 8이며 반사 시 수명을 초기화한다. 기존 공중 점프 횟수를 채워주지는 않는다.
- ESC, 창 포커스 상실, 타깃 소멸, 피격·사망·체크포인트 이동·시네마틱 시작은 차지를 취소한다. 공격·대시·피격 중에는 시작하지 않는다.
- 지형 충돌은 기존 프로젝트의 Ground/Platform 태그를 사용한다. 투사체 피해·적 피해·전용 애니메이션·이펙트는 이번 범위에 포함하지 않는다.

## 1. 화살표 머티리얼

1. Project에서 원하는 머티리얼 폴더에 Material을 생성하고 `MAT_ChargeJumpArrow`로 이름을 지정한다.
2. Shader를 `Universal Render Pipeline/Particles/Unlit`로 선택한다.
3. Surface Type은 Transparent, Blending Mode는 Alpha, Render Face는 Both로 설정한다.
4. Color Mode는 Multiply, Base Color는 흰색/Alpha 1로 두고 Base Map은 기본 흰색 상태를 사용한다. Soft Particles, Camera Fading, Distortion은 끈다.

이 머티리얼은 조명을 요구하지 않는다. 화살표의 실제 색은 ChargeJumpArrow의 Color에서 설정한다.

## 2. 화살표 프리팹

1. Hierarchy에서 활성화된 빈 GameObject를 생성하고 `ChargeJumpArrow`로 이름을 지정한다. Transform은 Position 0, Rotation 0, Scale 1로 둔다.
2. `ChargeJumpArrow` 컴포넌트를 추가한다. 필요한 LineRenderer가 자동으로 붙는다. SpriteRenderer는 필요 없다.
3. LineRenderer의 Materials / Element 0에 위 머티리얼을 연결한다. Alignment는 View로 둔다.
4. Sorting Layer는 현재 플레이어와 같은 `GA1_Actors`, Order in Layer는 우선 100으로 설정한다. 해당 레이어의 다른 오브젝트가 더 높은 Order를 사용하면 그보다 높게 조정한다. GameObject Layer는 카메라가 그리는 레이어로 둔다.
5. ChargeJumpArrow의 Length 1.6, Start Offset 0.25, Head Length 0.4, Head Width 0.3, Line Width 0.08을 시작값으로 사용한다.
6. 원하는 프리팹 폴더(예: Assets/03.Prefabs/Shooter)에 드래그하여 저장하고, 임시 Hierarchy 오브젝트는 제거한다. 프리팹 루트는 활성 상태로 유지한다.

스크립트가 Play 중 LineRenderer의 Positions, Width, Color, 표시 상태를 관리한다. 화살표는 플레이어의 좌우 반전 Scale 영향을 받지 않도록 런타임에 독립 생성된다.

## 3. 플레이어 연결

1. YuchanScene에서 실제 사용하는 플레이어 루트를 선택한다. Rigidbody2D, PlayerInput, PlayerMove, PlayerJump가 있는 오브젝트다. 별도의 Z_HyunseoTest 플레이어에는 연결하지 않는다.
2. `PlayerChargeJump`를 추가한다.
3. Arrow Prefab에 위 화살표 프리팹을 연결한다.
4. Aim Camera는 None이면 MainCamera 태그의 카메라를 사용한다. 필요하면 씬 플레이어의 필드에 실제 게임 카메라를 직접 연결한다. 프리팹 에셋에는 씬 카메라를 연결할 수 없다.
5. 다음 값으로 테스트한다.

| 필드 | 시작값 | 의미 |
|---|---:|---|
| Selection Radius | 1.8 | 투사체 탐색 거리 |
| Projectile Layers | Everything | 탐색할 물리 LayerMask. Sorting Layer와 다르다 |
| Slow Motion Scale | 0.1 | 기존 시간 배율에 곱할 값 |
| Player Launch Speed | 18 | 플레이어 발사 속도 |
| Projectile Launch Speed | 8 | 반사 투사체 속도 |
| Launch Duration | 0.2 | 중력 없이 직선 속도를 유지할 게임 시간 |

플레이어 Rigidbody2D는 기존 Dynamic, Simulated 상태를 사용한다. Gravity Scale을 수동으로 0으로 바꾸지 않는다. 차지 스크립트가 잠시 0으로 만들고 복원한다. 기존 발사기·투사체의 Inspector 연결을 다시 만들 필요는 없다.

## 4. Play 확인

1. 발사기 근처에서 투사체가 탐색 범위에 들어올 때 우클릭을 누른다. 범위 밖에서는 시작하지 않는다. Gizmos를 켜고 플레이어를 선택하면 탐색 원을 확인할 수 있다.
2. 누르고 있는 동안 전체 게임이 느려지고 플레이어와 선택된 투사체가 정지하는지 확인한다. 마우스를 움직여 상하좌우로 화살표가 회전하는지 확인한다.
3. 버튼을 놓고 두 물체가 반대 방향으로 이동하며 시간과 이후 일반 이동·중력이 복원되는지 확인한다.
4. ESC로 취소하거나 창을 벗어났을 때 원래 시간과 투사체 이동이 복원되는지 확인한다.
5. 벽을 향해 발사했을 때 지형에서 직선 발사가 종료되는지 확인한다. 공격·대시·피격 중 시작 방지와 체크포인트 이동도 확인한다.

화살표가 없으면 Arrow Prefab 연결, 머티리얼, 카메라 Culling Mask, Sorting을 확인한다. 차지가 시작되지 않으면 실제 플레이어에 컴포넌트가 붙었는지, 투사체의 IceProjectile/Collider2D가 활성 상태인지, Projectile Layers가 투사체의 물리 레이어를 포함하는지 확인한다. UI 위에 있는 포인터로 우클릭하면 차지를 시작하지 않는다.

## 변경 스크립트

- [PlayerChargeJump](../../Assets/02.Scripts/Player/Movement/PlayerChargeJump.cs): 선택·조준·시간 제어·발사·취소.
- [ChargeJumpArrow](../../Assets/02.Scripts/Player/Movement/ChargeJumpArrow.cs): LineRenderer 화살표.
- 기존 PlayerInput, PlayerJump, PlayerController, PlayerHit, Player, IceProjectile에 입력·점프 상태·공격 제한·취소·투사체 보류를 연결했다.
- HitImpactFeedback의 히트스톱 복귀 배율과 CinematicManager의 제어 잠금 시작에 취소 처리를 연결했다.

전체 C# 컴파일 확인과 실제 Play 검증은 구분한다. 이 문서는 실제 플레이 완료 기록이 아니다.
