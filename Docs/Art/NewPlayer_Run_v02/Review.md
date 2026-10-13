# Run v02 상태와 후속 제작 제안

2026-10-12 사용자 피드백: 동작은 괜찮지만 원본과 디자인 차이가 있어, 계속 제작한다면 Idle부터 사용 중 모션의 디자인을 통일할 필요가 있음. 전체 재생성은 아직 실행하지 않음.

- 적용: `Assets/05.Animation/player/Run.anim`에 `Assets/04.Sprites/NewPlayer/Run_v02/Frames`의 신규 16장 연결. 마지막 첫 프레임 반복 키 포함 17키, 48 sample rate, 클립 길이 0.5208333초.
- 제작: 내장 image_gen. 동작 가이드 → 16장 저해상도 초안 → 4장씩 고해상도 재생성 → 03번 중복 팔과 08번 팔 방향 개별 수정. 프롬프트는 이 폴더의 Prompt 파일에 보관.
- 분리/정렬: 생성 원본의 지배적 알파 실루엣 분리, 단독 잡점 제외, 원본 Run_00 얼굴 면적 기준 크기 정렬, 같은 몸통 피벗과 3px 상하 움직임. 최종 512×512, PPU 200.
- 보존: 기존 Player 635개 파일의 SHA-256 일치. 다른 기존 애니메이션 파일 보존. Run_v01도 유지.
- 검증: Unity Editor API로 16 스프라이트 참조/루프/길이 저장 확인, 저장 당시 씬 dirty=false. v02의 자동 Play 검증은 추가 수행하지 않았으며 v01 검증을 v02 결과로 인용하지 않음. 사용자가 직접 확인 후 위 피드백을 전달함.
- 비교: [이전 개선안/원본/v02 비교](Run_Comparison_v02.html), [이전 개선안 대비 GIF](Run_v01_vs_v02.gif).

## 제안 프레임 수 — 아직 미실행

종료 중복 키를 제외한 실제 스프라이트 수. CurrentAnimationInventory.json은 실제 저장된 controller와 anim 파일을 읽어 작성했다. 인벤토리 조회 시점에는 Editor 연결이 종료되어 정적 파일 검사를 사용했다.

| 모션 | 현재 | 제안 |
| --- | ---: | ---: |
| Idle | 32 | 32 |
| Run | 16 (원본 10) | 16 |
| JumpStart | 3 | 6 |
| JumpRise | 8 | 12 |
| JumpApex | 4 | 6 |
| Fall | 8 | 12 |
| DoubleJump | 16 | 16 |
| Land | 4 | 6 |
| Dash | 8 | 12 |
| Hit | 4 | 6 |
| WallCling | 8 | 12 |
| Glide | 8 | 12 |
| LeftHorizontalAttack | 8 | 12 |
| RightHorizontalAttack | 15 | 15 |
| UpAttack | 15 | 15 |
| DownAttack | 15 | 15 |
| FireBallAttack | 9 | 12 |

Death 상태에는 본체 애니메이션이 연결되어 있지 않으며 별도 사망 VFX와 구분한다. WallSlide 8장, WallClimb 4장, WallJump 3장, DashStart 2장, DashEnd 3장 원본은 있으나 현재 Player.controller의 독립 상태/클립으로 연결되어 있지 않아 우선 제작에서 제외한다. 차지점프 발사 표시는 Dash를 재사용한다.

제안 순서: Idle 기준 디자인 확정 → Idle 32장과 Run 외형 일치 → Fireball → 점프/낙하/착지/대시 → 벽/활공/피격 → 나머지 공격. 매 동작에 동작 가이드→플레이어 그림→오류 프레임 개별 재생성→루프/전환 비교 방식을 적용한다.

Fireball은 플레이어의 시전 동작을 뜻하며 투사체 이펙트는 별도다. 현재 0.75초 클립을 Animator state speed 3으로 재생하므로 클립 유효 길이는 약 0.25초. Attack DATA는 WindupDuration 0, RecoveryDuration 0.22이다. 게임의 판정/입력 복귀 타이밍은 그림 수와 구분하며, 변경 승인 전에는 즉시 발사→반동→복귀 구성으로 현재 타이밍을 보존한다.
