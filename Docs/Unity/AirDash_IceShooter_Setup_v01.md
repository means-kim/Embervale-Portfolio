# 공중 8방향 대시와 발사기 배치

2026-10-10. 스크립트만 수정했다. 기존 사용자 씬·프리팹·Import·전역 설정은 보존했다.

## 공중 대시

기존 Shift 대시 입력·속도·지속시간·쿨타임·시작/종료 이벤트를 유지한다. 공중에서는 Shift를 누른 순간 WASD를 읽어 방향을 고정한다. 좌우와 상하의 반대 입력은 서로 상쇄하고, 방향이 0이면 마지막 PlayerMove.FacingDirection을 사용한다. 대각선 벡터는 정규화하여 속도가 증가하지 않는다.

| 입력 | 공중 대시 방향 |
|---|---|
| W / A / S / D | 위 / 왼쪽 / 아래 / 오른쪽 |
| WA / WD / AS / SD | 왼쪽 위 / 오른쪽 위 / 왼쪽 아래 / 오른쪽 아래 |
| WAD / ASD / WAS / WSD | 위 / 아래 / 왼쪽 / 오른쪽 |
| 없음 / AD / WS / WASD | 마지막 좌우 방향 |

지상에서는 기존 좌우 대시를 유지한다. 공중 대시 중 표시용 캐릭터와 잔상은 발사 방향을 향하고, 종료·피격·사망·비활성화 때 원래 표시로 복원된다. Rigidbody2D와 Collider2D가 붙은 루트를 회전시키지 않는다. 기존 차지 점프의 표시용 스프라이트 복사를 재사용하며 MaterialPropertyBlock은 Awake/메인 스레드에서 생성한다.

Editor에서 새 필드를 연결할 필요는 없다. PlayerDash의 기존 Dash Speed/Duration/Cooldown과 PlayerDashGhost의 Target Renderer만 기존대로 사용한다. 대시 VFX가 방향성이 있다면 +X를 오른쪽 진행 방향으로 제작한다. 지상 대시 VFX의 기존 좌우 반전 방식은 유지했다.

Play 확인: 점프 후 W, AS, WD, WAD, 입력 없음, WASD 각각과 Shift를 조합한다. 마지막 좌우 방향을 바꾼 뒤 입력 없는 대시도 확인한다. 일반 지상 대시·차지 점프·피격 후 복귀와 잔상 방향을 함께 확인한다.

## IceShooter

기존 방식은 FirePoint.right로 방향을 결정했기 때문에 SpriteRenderer의 Flip과 음수 Scale이 발사 방향과 어긋날 수 있었다. 이제 Visual의 실제 변환과 Flip을 함께 사용한다.

- 원본 이미지의 입구 방향인 Visual 로컬 +X를 기본 방향으로 사용한다.
- FirePoint의 위치는 입구 오프셋 기준이다. Visual 중심부터 X 거리는 절댓값으로 사용해 기존 왼쪽 FirePoint도 그대로 재사용한다. FirePoint의 Rotation은 사용하지 않는다.
- Visual의 Flip X/Y를 발사 위치에 적용한다. Flip X는 발사 방향도 뒤집는다.
- 루트/부모의 회전과 양수·음수 Scale을 TransformPoint/TransformVector로 반영한다.
- 발사 속도는 정규화한 월드 방향에 적용하므로 오브젝트 크기를 바꿔도 투사체 속도는 유지된다.
- Scene Gizmos의 청록 원과 화살표가 실제 발사 위치와 방향이다.

현재 프리팹의 Visual Flip X=true, FirePoint=(-1,-0.4)도 수정 없이 왼쪽 발사 구성으로 해석한다. Visual 필드를 비워두면 자식 SpriteRenderer를 자동으로 찾는다. SpriteRenderer가 여러 개라면 원하는 Visual을 한 번 직접 연결한다.

### Editor 사용

1. 기존 IceShooter 프리팹을 맵에 배치한다. Projectile Prefab과 Fire Point 연결은 유지한다.
2. 각 배치에서는 루트 IceShooter의 Position과 Rotation Z만 조절한다. 자식 FirePoint나 Visual을 따로 돌리지 않는다.
3. 상하좌우 반전은 루트 Scale X/Y의 부호를 바꾸면 이미지·발사 위치·방향이 같이 바뀐다. SpriteRenderer의 Flip X/Y를 써도 같은 계산이 적용된다.
4. 현재 기본 프리팹은 왼쪽을 향한다. Z=90은 아래, Z=-90은 위, Z=180은 오른쪽이다. Flip X를 끄거나 Scale X를 반전하면 기준 방향도 반대가 된다.
5. 입구 위치를 세밀하게 맞춰야 할 때만 원본 프리팹 FirePoint를 한 번 조정한다. 이후 회전·반전 배치마다 변경하지 않는다. 청록 Gizmo가 입구 바깥을 가리키는지 확인한다.

2D에서는 Rotation Z로 평면 방향을 조절한다. X/Y 회전으로 정면 이미지가 옆으로 눕는 배치는 권장하지 않는다. 변환된 발사 축이 화면 평면에서 0이 되면 발사하지 않는다. 중복된 반전(Scale X 음수와 Flip X)을 동시에 쓰면 서로 상쇄되는 것이 정상이다.

컴파일과 분리된 Unity 오브젝트의 방향 계산 검증은 실제 YuchanScene Play 검증과 구분한다.

검증 결과: 전체 C# 컴파일 통과. Unity 내부의 분리 Preview Scene에서 WASD 16조합×좌우 fallback 2개=32개 입력과, 8개 Z 각도×부모/루트 반전×Flip X/Y=256개 발사 위치·방향 조합을 검사해 모두 통과했다. 기존 IceShooter 프리팹은 위치 (-1,-0.4), 방향 (-1,0)으로 확인했다. 기존 사용자 씬을 Play하거나 저장하지 않았다. 공중 대시의 실제 충돌·애니메이션 전환·VFX는 사용자의 Play 확인이 필요하다.
