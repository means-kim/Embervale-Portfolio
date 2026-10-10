# YuchanScene 얼음 투사체·발사기 조립 안내 v01

## 현재 범위

새 런타임 스크립트는 Assets/02.Scripts/Shooter/IceProjectile.cs와 IceShooter.cs 두 개다. 에디터와 프리팹 조립은 사용자가 직접 진행한다. 현재는 주기적인 투사체 생성·직선 이동·지형 충돌 소멸·수명 소멸까지다. 플레이어 피해, 우클릭 근접 선택, 시간 감속, 조준 화살표, 플레이어 발사 기능은 후속 단계다.

현재 Shooter/IceBall.jpg 및 IceShooter.jpg는 확장자와 달리 실제 PNG 인코딩이며 알파가 있다. 실제 크기는 둘 다 1254×1254. 현재 Import의 PPU는 각각 1000과 500이다. 본 작업에서 이미지와 Import를 변경하지 않았다.

## 1. 투사체 프리팹

Hierarchy에서 빈 오브젝트를 만들어 IceProjectile로 이름 지정. Position (0,0,0), Rotation (0,0,0), Scale (1,1,1)로 시작한다. IceProjectile 스크립트를 루트에 붙이면 Rigidbody2D와 CircleCollider2D가 자동으로 추가된다.

- Rigidbody2D: Body Type Kinematic, Simulated 체크, Interpolate Interpolate, Freeze Rotation Z 체크. Gravity Scale 0. 스크립트도 실행 시 Kinematic/중력 0/회전 고정을 설정한다. 이동은 linearVelocity 대신 스크립트의 Collider.Cast + MovePosition으로 처리한다.
- CircleCollider2D: Is Trigger 체크. 현재 PPU 1000 및 Visual Scale 1 기준 Radius 0.35~0.4부터 시작하고 실제 보이는 둥근 몸체에 맞춰 Edit Collider로 조절한다. 투명 여백까지 판정에 포함하지 않는다. 스크립트도 실행 시 Trigger로 설정한다.
- IceProjectile: Life Time 6, Terrain Layers Everything.
- 루트 아래 빈 자식 Visual 생성. SpriteRenderer를 붙이고 IceBall 스프라이트 지정. 자식의 Position/Rotation은 0, Scale은 1. Color는 흰색/Alpha 1. Sorting Layer는 GA1_Actors, Order in Layer는 20부터 시작하고 실제 맵에서 조절한다.

크기를 조절할 때 Visual만 줄이거나 키웠다면 루트 CircleCollider2D의 Radius/Offset도 직접 맞춰야 한다. 처음에는 루트와 Visual 모두 Scale 1을 유지해 PPU 기준으로 비교한다. 루트 Tag는 Untagged, Layer는 Default로 시작해도 된다. Rigidbody2D와 CircleCollider2D는 Visual이 아닌 루트에 둔다.

Project 창의 사용자가 원하는 프리팹 폴더(예: Assets/03.Prefabs/Shooter)를 만들고 루트를 끌어 프리팹으로 저장한다. 프리팹 루트의 Active 체크와 IceProjectile.enabled, Collider.enabled를 유지한다. 임시로 만든 씬 투사체는 삭제하여 발사기에서 생성되는 것만 확인한다.

## 2. 발사기 프리팹

Hierarchy에 빈 루트 IceShooter 생성. Rotation 0, Scale 1. 루트에 IceShooter 스크립트를 붙인다. 루트 아래에 자식 두 개를 만든다.

```text
IceShooter (IceShooter 스크립트)
  Visual (SpriteRenderer: IceShooter)
  FirePoint (빈 Transform)
```

Visual은 Local Position/Rotation 0, Scale 1. SpriteRenderer에 IceShooter 스프라이트를 연결한다. Sorting Layer GA1_Actors, Order in Layer 10부터 시작한다. 현재 PPU 500 기준 전체 이미지 프레임 너비는 2.508 월드 단위지만 실제 보이는 몸체의 크기와는 다르다.

FirePoint는 Local Position (1.4,-0.4,0), Rotation (0,0,0), Scale (1,1,1)부터 시작한다. 이 좌표는 현재 이미지/PPU 기준 시작 제안이며 실제 Scene에서 입구 앞의 발사 중심으로 맞춘다. 투사체의 원형 판정이 지형에 겹쳐 태어나지 않도록 여유를 둔다. FirePoint의 로컬 +X(빨간 축)가 발사 방향이다.

IceShooter Inspector:

| 항목 | 값/연결 |
|---|---|
| Projectile Prefab | Project 창의 IceProjectile 프리팹 루트 |
| Fire Point | 같은 발사기 루트 아래의 FirePoint |
| Fire Interval | 2 |
| Initial Delay | 0.5 |
| Projectile Speed | 4 |

YuchanScene의 Player 프리팹이나 플레이어 공격 스크립트는 연결하지 않는다. Projectile Prefab에는 씬에 임시 배치한 투사체가 아니라 Project 창의 프리팹 에셋을 넣는다. FirePoint는 프리팹 자신의 자식이므로 프리팹 내부 참조로 저장할 수 있다. 발사기는 현재 Collider/Rigidbody가 없어도 동작한다. 지형 오브젝트 자식에 넣기보다 별도 루트로 두어 부모 Scale/Rotation의 영향을 피한다.

발사기 루트를 Project 창에 끌어 프리팹 저장 후 YuchanScene의 원하는 위치에 배치한다. 발사기를 선택하고 Scene의 Gizmos를 켜면 청록색 원이 생성 위치, 청록색 화살표가 방향을 표시한다.

왼쪽을 향하게 할 때는 Visual의 SpriteRenderer Flip X 체크, FirePoint Local X를 -1.4로 바꾸고 Z Rotation 180으로 설정한다. Local Y는 기존 높이를 기준으로 조정한다. Flip X만 변경하면 발사 방향은 바뀌지 않는다. 발사기 루트에 음수 Scale을 주는 방법 대신 FirePoint의 회전으로 방향을 명시한다. 위쪽은 FirePoint Z Rotation 90, 아래쪽은 -90이며 생성 위치도 해당 입구 앞으로 조절해야 한다.

## 3. 지형과 동작 확인

- 실제 Collider2D가 있는 오브젝트의 Tag가 Ground 또는 Platform인지 확인한다. 부모 Map의 태그가 아닌, 콜라이더가 달린 오브젝트의 태그를 본다. 현재 YuchanScene 지형은 이 태그를 사용한다.
- Terrain Layers는 태그와 별개인 Physics Layer 마스크다. 처음에는 Everything으로 둔다. 현재 지형의 StartMap 등 필요한 레이어를 제외하면 해당 지형에 닿아도 투사체가 사라지지 않는다.
- 플레이어/다른 투사체는 지형 태그가 아니므로 현재는 통과한다. 피해 처리는 없다. Platform 태그는 현재 위/아래 방향과 관계없이 닿으면 소멸한다.
- Time.timeScale 1 기준 시작 0.5초 후 첫 발사, 이후 약 2초마다 발사, 속도 초당 4 월드 단위. 수명은 6초이므로 충돌이 없으면 약 24단위까지 이동한다. 모든 타이머는 게임 시간 기준이다. 프레임 지연 시 밀린 발사를 한꺼번에 생성하지 않는다.
- 기본 Unity Physics 2D Layer Collision Matrix에서 Projectile의 Default와 지형 레이어 간 검사가 허용되는지 확인한다. 시범 설정은 기본값을 유지하고, 필요할 때만 직접 조절한다.
- Play에서 Hierarchy에 IceProjectile(Clone)이 생성되고 이동하는지 확인한다. 이후 지형 충돌 시 사라지는지, 빈 공간에서 6초 후 정리되는지 확인한다.
- 생성 직후 없어지면 FirePoint/투사체 반경과 지형의 겹침을 확인한다. 보이지 않으면 Visual의 Sprite, Color Alpha, Sorting Layer/Order, 카메라 Culling Mask, SpriteRenderer 머티리얼과 2D 조명을 확인한다. 조명 없는 시범 씬에서는 Sprite-Unlit-Default 머티리얼을 사용하면 비교가 쉽다.
- Projectile Prefab/Fire Point가 비어 있으면 콘솔에 구체적인 오류를 출력하고 발사기 스크립트를 비활성화한다. 참조를 연결한 뒤 컴포넌트를 다시 켜거나 Play를 다시 시작한다.

## 후속 차지 점프 연결점

IceProjectile.Velocity로 현재 이동 속도를 읽고, SetVelocity(Vector2.zero)로 이동을 정지시키거나 SetVelocity(-aimDirection.normalized * speed)로 반대 방향 속도를 설정할 수 있다. SetVelocity는 수명을 초기화하지 않는다. Launch(direction,speed)는 최초 발사용이며 수명 타이머를 초기화한다. Rigidbody2D.linearVelocity를 직접 바꿔서는 이 스크립트의 저장된 이동 방향이 바뀌지 않는다. 정지 중 수명 타이머는 계속 게임 시간으로 흐르므로 후속 차지 점프에서는 홀드 중 수명 처리도 함께 결정해야 한다.

## 검증

Unity 6000.3.22f1의 현재 Assembly-CSharp 참조/define을 사용하여 새 두 소스를 포함한 프로젝트 전체 C# 컴파일을 임시 출력 폴더에서 실행했고 exit 0. CLI 읽기 조회에서 compiling=false, compilationFailed=false, consoleErrors=0 확인. 실제 프리팹 조립/발사/충돌 Play 동작은 사용자 조립 이후 검증 대상이다.

