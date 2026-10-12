# 차지 점프 이미지·연출 연결 v02

2026-10-10. 이번 작업은 이미지 시안과 스크립트 편집이다. 씬·프리팹·Animator Controller·프로젝트 설정은 사용자가 연결한다. 이전 [핵심 이동 설정](ChargeJump_Setup_v01.md)은 유지하며, 아래 설정을 추가한다. C# 컴파일과 실제 Play 동작 검증은 구분한다.

## 1. 불꽃 화살표 선택과 임포트

[시안과 실제 생성 프롬프트](../Art/ChargeJump_20261010_v02/prompts.md)에 네 개의 원본 RGBA PNG를 보관했다. 아직 선택하지 않았으며 자동 채택하지 않았다.

| 번호 | 디자인 | 원본 |
|---|---|---|
| 1 | 날렵한 불꽃 창 | [PNG](../Art/ChargeJump_20261010_v02/Candidates/ChargeArrow_01_FlameSpear_v01.png) |
| 2 | 부드러운 정령 불꽃 | [PNG](../Art/ChargeJump_20261010_v02/Candidates/ChargeArrow_02_SpiritChevron_v01.png) |
| 3 | 균열이 있는 결정 창 | [PNG](../Art/ChargeJump_20261010_v02/Candidates/ChargeArrow_03_EmberRune_v01.png) |
| 4 | 두꺼운 불꽃 혜성 | [PNG](../Art/ChargeJump_20261010_v02/Candidates/ChargeArrow_04_FlameComet_v01.png) |

선택한 원본을 Assets/04.Sprites/Shooter로 복사하고 다음 Import 설정을 적용한다.

- Texture Type: Sprite (2D and UI), Sprite Mode: Single.
- Alpha Source: Input Texture Alpha, Alpha Is Transparency: 켜기.
- Pivot: Center. 그림이 오른쪽을 가리키는 원본 방향을 유지한다.
- Mesh Type: Full Rect, Filter Mode: Bilinear, Compression: 우선 None, Generate Mip Maps: 끄기.
- PPU는 우선 100을 사용할 수 있다. 스크립트가 Length에 맞춰 렌더러의 Scale을 조절하므로 PPU만으로 화면 크기를 맞추지 않아도 된다.
- Max Size는 원본을 유지하려면 4096(1번 너비 2172), 나머지는 2048을 사용할 수 있다.

## 2. 기존 ChargeJumpArrow 프리팹 수정

1. 기존 Assets/03.Prefabs/Shooter/ChargeJumpArrow.prefab을 연다.
2. 루트 아래 빈 자식 `Visual`을 생성한다. Position/Rotation은 0, Scale은 1로 두고 SpriteRenderer를 추가한다.
3. Sprite에 선택한 화살표를 연결한다. Color는 흰색, Flip X/Y는 끈다.
4. Material은 `Sprite-Unlit-Default` 또는 Shader가 `Universal Render Pipeline/2D/Sprite-Unlit-Default`인 머티리얼을 사용한다. 이전 LineRenderer의 Particles/Unlit 머티리얼을 SpriteRenderer에 재사용하지 않는다.
5. Sorting Layer는 `GA1_Actors`, Order in Layer는 -20. 투사체 Visual은 같은 레이어의 -10, 플레이어는 0으로 설정한다. 카메라 Culling Mask에 포함되는 GameObject Layer를 사용한다. 기존 LineRenderer의 Order도 -20으로 맞춘다.
6. 루트 ChargeJumpArrow의 `Sprite Renderer`에 Visual의 SpriteRenderer를 드래그한다. 루트와 Visual은 활성 상태로 유지한다.
7. 기존 LineRenderer는 제거하지 않아도 된다. 이미지가 연결되면 스크립트가 선 표시를 끈다. 이미지가 없으면 기존 선 화살표로 표시한다.

| ChargeJumpArrow 필드 | 시작값 | 역할 |
|---|---:|---|
| Length | 1.6 | 투명 여백을 포함한 이미지 전체 폭의 월드 크기 |
| Start Scale | 0.25 | 처음 등장 크기 |
| Peak Scale | 1.3 | 순간 최대 크기 |
| Grow Duration | 0.1 | 최대 크기에 도달하는 실제 시간 |
| Settle Duration | 0.12 | 100%로 돌아오는 실제 시간 |

팝 연출은 슬로모션의 영향을 받지 않는다. 매 조준 시작에 한 번만 재생한다. 크기는 Visual의 Scale을 직접 바꾸기보다 Length로 조절한다. 프리팹 루트 Scale은 1로 둔다.

후속 사용자 요청으로 화살표 중앙을 선택한 투사체의 Collider2D 중심에 맞췄다. 마우스 조준과 회전도 이 중심을 기준으로 한다. 크기 팝 중에도 중앙은 고정된다. 이전 Start Offset 필드는 제거했으며 새 에디터 연결은 필요 없다. 중앙은 투명 여백을 포함한 스프라이트 사각형 기준이다.

## 3. 검은 그라데이션 테두리

별도 PNG나 Volume 없이 UI Image 하나를 사용한다. ChargeJumpVignette가 시작할 때 256×144 검정 알파 그라데이션을 생성한다. 중심은 투명하고 바깥으로 갈수록 어두워지는 타원형 테두리다.

1. YuchanScene에 Canvas를 생성하고 Render Mode를 Screen Space - Overlay로 둔다. 이름 예: `ChargeJumpCanvas`.
2. Sorting Order는 우선 20으로 둔다. 기존 HUD Canvas보다 낮게 두면 HUD는 어두워지지 않는다. 같은 Canvas를 사용하면 테두리 Image를 HUD 요소보다 앞선 sibling으로 둔다.
3. Canvas 아래 UI > Image를 만들고 `ChargeJumpVignette`로 이름을 지정한다.
4. RectTransform의 Anchor Min을 (0,0), Max를 (1,1)로 바꾸고 Left/Right/Top/Bottom을 모두 0으로 설정한다. 전체 화면을 채워야 한다.
5. Image에 ChargeJumpVignette 컴포넌트를 추가한다. CanvasGroup은 자동으로 추가된다. Source Image는 비워도 된다. Image Material은 기본 None을 사용한다.
6. Player 필드에 씬 플레이어 루트의 PlayerChargeJump를 연결한다.
7. 시작값: Opacity 0.75, Fade In Duration 0.12, Fade Out Duration 0.18, Clear Radius 0.45.

페이드는 실제 시간 기준이다. 조준 때 페이드 인, 발사·취소 때 페이드 아웃한다. Raycast Target과 CanvasGroup의 Blocks Raycasts는 스크립트가 꺼서 우클릭을 가로채지 않는다. Clear Radius의 생성 결과 변경은 Play를 다시 시작해 확인한다.

## 4. 대시 애니메이션과 잔상

발사 순간 기존 Animator의 Dash 상태를 재생하고 발사 유지 시간 동안 표시한다. 실제 PlayerDash 이동이나 대시 쿨타임을 호출하지 않는다. 발사 종료 또는 피격·취소 시 일반 상태로 돌아간다. 좌우 발사 방향에 맞춰 플레이어의 Facing Direction을 갱신하며, 수직 발사에서는 기존 방향을 유지한다.

후속 사용자 요청으로 차지 점프의 Dash 연출 중에만 표시용 스프라이트를 발사 방향으로 기울인다. 물리 루트·충돌체·카메라 타깃의 Transform은 회전하지 않는다. 기존 루트 SpriteRenderer의 Animator 연결을 유지하기 위해 런타임 자식 ChargeJumpLaunchVisual에 현재 프레임을 복사하여 표시하고 원본 렌더링만 잠시 숨긴다. 종료·피격·사망·컴포넌트 비활성화 시 원본 표시를 복원한다. 일반 대시·조준·일반 점프에서는 기울이지 않는다. 잔상도 기울어진 실제 월드 축과 좌우 반전을 복사한다. 새 Inspector 연결은 필요 없다.

그리기 순서(뒤→앞)는 배경→화살표→투사체→플레이어→UI다. 세 SpriteRenderer를 GA1_Actors로 맞춘 뒤 Order를 화살표 -20, 투사체 -10, 플레이어 0으로 설정한다. 배경은 기존 GA1_BG_Far/GA1_BG_Mid/GA1_World 등 GA1_Actors보다 앞에 나열된 Sorting Layer를 사용하며, UI는 Screen Space - Overlay를 사용한다. 다른 Sorting Layer 사이에서는 Order 값보다 Sorting Layer 순서가 우선한다. SortingGroup이 있으면 해당 그룹의 레이어·Order도 확인한다.

공통 Player 프리팹에는 PlayerDashGhost가 이미 있다. Target Renderer가 실제 캐릭터 SpriteRenderer인지 확인한다. 차지 점프에도 같은 잔상 풀을 사용한다.

- Charge Jump Spawn Delay: 0.02.
- Spawn Interval: 우선 0.04.
- Fade Time: 기존 값이 너무 짧으면 0.2~0.3으로 조절한다. 일반 대시에도 같은 잔상 설정이 적용된다.
- 기존 Animator Controller에 Dash 상태가 있으므로 새 상태나 Transition을 만들 필요는 없다.

## 5. EffectManager 슬롯

실제 씬에서 사용하는 EffectManager를 선택하면 `VFX 프리팹 (차지 점프)`에 다음 세 항목이 추가된다. 슬롯을 None으로 두어도 이동·화살표·테두리·대시 잔상은 작동한다.

| 필드 | 재생 시점 | 프리팹 설정 |
|---|---|---|
| Charge Jump Aim Vfx | 조준 시작부터 종료까지 | 지속 Loop 파티클, OneShotVFX 없이 |
| Charge Jump Launch Vfx | 플레이어 발사 위치에서 한 번 | Loop 끄기, 오른쪽 +X를 기준 방향으로 제작 |
| Charge Jump Projectile Launch Vfx | 투사체 반사 위치에서 한 번 | Loop 끄기, 오른쪽 +X를 기준 방향으로 제작 |

Aim 효과는 플레이어 중심을 따라가고 조준 종료 때 제거된다. 내부 ParticleSystem/Animator는 실제 시간 기준으로 재생한다. 발사 두 효과는 실제 발사 방향으로 Z 회전하며 OneShotVFX로 수명이 끝나면 제거한다(없으면 런타임에 추가). 기존 VFX 슬롯은 유지한다.

## 6. Play 확인

### 투사체 없는 차지 점프 (2026-10-12)

플레이어의 `PlayerChargeJump` Inspector에서 다음 값을 조절한다. 기존 Arrow Prefab 연결은 그대로 필요하다.

| 필드 | 기본값 | 동작 |
|---|---:|---|
| Allow Without Projectile | 켜기 | 주변에 선택 가능한 투사체가 없을 때 플레이어 중심에서 조준 허용 |
| Health Cost | 50 | 투사체 없이 실제 발사할 때만 소모. 0이면 무료 |
| Cooldown | 1초 | 투사체 유무에 관계없이 발사 시점부터 재사용까지의 게임 시간 |

투사체가 있으면 기존 반사를 우선하고 체력은 소모하지 않는다. 투사체 없이 사용할 때 체력이 비용 이하면 조준을 시작하지 않으며, 조준 중 체력이 부족해져도 발사를 취소한다. ESC·피격·포커스 상실 등 취소에는 체력 비용과 쿨타임이 발생하지 않는다. 체력 소모는 기존 `Health.SpendHealth`를 사용하여 피격·넉백 연출을 발생시키지 않는다. 게임 일시정지 중에는 쿨타임도 멈춘다.

Play에서 투사체 없는 발사/취소, 체력 50 이하 사용 차단, 투사체 반사의 무료 사용, 쿨타임 재사용 차단, 허용 옵션 해제를 확인한다. 코드 컴파일 확인과 실제 Play 동작 확인은 별개다.

- 조준 시작: 불꽃 이미지가 25%→130%→100%로 변하고 테두리는 어두워지는지 확인한다.
- 조준 중: 마우스 회전, 선택 투사체 정지, UI 클릭 방해 없음, Aim VFX 유지.
- 발사: 플레이어는 조준 방향, 투사체는 반대 방향. Dash 자세·잔상·양쪽 발사 VFX와 테두리 페이드 아웃.
- 취소: ESC/포커스 상실/피격/리스폰 후 테두리·Aim VFX·시간 배율이 남지 않는지 확인한다.
- 일반 이동·점프·대시도 다시 확인한다. 이번 작업은 사용자 조립 전 C# 검증이며 실제 Play 확인은 별도다.

## 7. 카메라 추적 정지 조사와 수정

사용자는 Tracking Target에 Player가 남아 있는데 추적이 멈춘다고 확인했다. 현재 읽기 전용 조사에서는 실제 Player 참조, Brain 활성 상태, SmartUpdate/LateUpdate 설정이 정상이다. YuchanScene에는 Confiner·Bounds·카메라 게이트가 없으며, 차지점프 코드가 Follow를 변경하는 경로도 없다. 따라서 참조를 임의 재연결하는 카메라 패치는 적용하지 않았다.

확인한 코드 취약점은 PlayerChargeJump의 물리 시간 간격 복원 조건이다. 조준에서 `fixedDeltaTime`을 0.1배로 지정한 계산값을 저장한 뒤, 종료 때 실제 getter와 Mathf.Approximately가 같은 경우에만 복원했다. Unity 시간 값의 양자화로 계산값과 실제값 사이 오차가 있으면 이 조건이 실패할 수 있다.

현재 실제 기본 getter 값은 0.0199999921이다. Unity.DiscreteTime의 141120000 ticks/sec를 사용하는 절삭 모델에서 기본 .02f를 계산하면 이 값과 일치한다. 같은 모델의 차지 계산값 0.00199999916과 양자화 값 0.00199999288은 Mathf.Approximately가 false다. 이 경우 반복 조준마다 남은 물리 간격에 다시 0.1을 곱해 과도한 FixedUpdate로 이어지는 경로가 있다. 이 검사는 읽기와 순수 산술만 사용했으며 실제 시간 setter를 호출하거나 증상 순간을 재현한 검증은 아니다.

수정: Time.timeScale과 Time.fixedDeltaTime 지정 직후 실제 getter를 다시 읽어 적용값으로 저장한다. 종료 시 같은 실제값을 비교하므로 계산값/적용값 오차로 복원이 생략되는 경로를 막는다. 다른 일시정지 시스템이 시간값을 바꿨으면 기존 소유권 검사는 유지한다. 시간 배율에 맞춰 fixedDeltaTime을 조정하는 방식 자체는 [Unity Time.timeScale API](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/time/timescale)의 사용법을 따른다.

Editor 확인:

1. 새 스크립트 컴파일 후 Play를 종료했다가 다시 시작한다. 이전 실행에서 잘못 남은 간격을 새 실행으로 가져오지 않는다.
2. Edit > Project Settings > Time의 Fixed Timestep을 기존 프로젝트 값 0.02로 확인한다. Inspector 반올림과 실제 getter의 미세한 차이는 구분한다.
3. 30회 정도 조준·발사·ESC 취소를 반복하고 시간과 물리 간격이 누적 축소되지 않는지 확인한다.
4. 추적 정지가 재발하면 Pause 후 실제 플레이어가 계속 이동하는지, MainCamera Brain과 사용 중 CinemachineCamera가 활성인지, Console 오류가 있는지 확인한다. 직접 증상이 재현되지 않았으므로 이번 수정만으로 완전 해결됐다고 선언하지 않는다.
5. 현재 카메라에는 CinemachineFollow와 CinemachinePositionComposer 두 Body가 활성화되어 있으며 실제 사용 중 Body는 첫 번째 CinemachineFollow다. Position Control을 하나만 유지하도록 구성한다. 이것은 구성 정리이며 추적 정지의 확인된 직접 원인은 아니다.
