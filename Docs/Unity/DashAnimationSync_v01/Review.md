# 대시·차징 점프 회전/스프라이트 전환 동기화

2026-10-14. 대시 또는 차징 점프 종료 시 각도가 먼저 원래대로 돌아오고 대시 스프라이트가 잠시 남는 문제를 재현하고 수정했다.

## 원인과 수정

`PlayerJumpPhaseAnimator.Reset()`이 이전 점프 클립 이름을 지우지 않았다. 예를 들어 JumpRise → Dash → JumpRise 순서에서 점프 담당 컴포넌트는 여전히 JumpRise를 재생 중이라고 판단해 `Animator.Play`를 생략했다. 정점 부근에서는 최소 공중 시간 유예도 새 포즈 선택을 막을 수 있었다. [수정 전 재현](Before.json)에서 상승·낙하·정점 세 경우 모두 회전 표시를 해제하고 Animator를 평가한 뒤에도 Dash_00이 남았다.

또한 시작 시 회전 렌더러가 이전 스프라이트를 복사한 다음에 대시 재생을 요청하고 있었다. 수정 전 시작 직후에는 JumpRise 또는 Fall이 복사되었으며 다음 Animator 평가에서 Dash로 바뀌었다.

- [PlayerJumpPhaseAnimator.cs](../../../Assets/02.Scripts/Player/Main/PlayerJumpPhaseAnimator.cs): Reset 시 현재 클립 캐시도 무효화한다. 정점 유예는 유효한 점프 포즈가 있을 때만 적용해 대시 복귀 시 Rise/Fall/Apex를 바로 선택한다. 일반 점프 시작의 유예는 유지한다.
- [PlayerAnimator.cs](../../../Assets/02.Scripts/Player/Main/PlayerAnimator.cs): 시작 시 Dash를 재생·평가한 후 회전 렌더러를 활성화한다. 종료와 피격·사망 중단에서도 새 스프라이트를 같은 호출 안에서 평가한다. 이때 0초 평가를 사용하므로 애니메이션 시간을 추가로 진행하지 않는다. 비활성/미초기화 Animator에는 강제 평가를 실행하지 않는다.

공개 메서드·직렬화 필드·Inspector 값·대시 및 발사 시간·물리 이동·콜라이더·스프라이트·클립·씬·프리팹은 변경하지 않았다. 물리 루트 회전도 유지했다.

## 확인 결과

- Unity 6000.3.22f1 컴파일 완료, compilationFailed=false.
- [After.json](After.json): YuchanScene의 실제 플레이어 컴포넌트로 Play 검증 18건 통과. 대시/차징 × 좌우 × 상승/낙하/정점 12건에서 시작 즉시 Dash, 종료 즉시 JumpRise/Fall/JumpApex를 확인했다. 종료 검증 전에 추가 Animator 평가나 다음 프레임 대기를 넣지 않았다. 일반 점프의 정점 유예, 2단 점프 중단, Idle/Hit/Death 복귀, 컴포넌트 비활성화 6건도 통과했다. 기록 중 경고·오류·예외 없음.
- [Runtime.json](Runtime.json): 운영 PlayerInput 이벤트를 전달해 실제 대시와 차징 점프를 시작하고 Update 타이머의 자연 종료를 확인했다. 두 동작 모두 약 0.20초 후 회전 표시가 해제된 첫 관측에서 Dash가 아닌 점프 스프라이트를 표시했다. 오류·예외 없음. 충돌 방해를 피하기 위해 Play 중 플레이어 위치만 위로 옮겼으며 씬에 저장하지 않았다.
- [FinalEditor.json](FinalEditor.json): 검증 후 Play 종료, 원래 YuchanScene 유지, 씬 미저장 변경 없음.
- `git diff --check` 통과. Assets 변경은 위 두 스크립트뿐이다.

사용자가 체감한 약 0.05초를 실측한 결과는 아니다. 대시 이미지가 남는 원인과 호출 직후 표시 전환을 검증했다. 물리 키·마우스 입력 주입, 전체 동선 플레이, 모든 충돌 상황의 조작감 확인은 포함하지 않는다.

재현/검증 코드: [Reproduce.cs](Reproduce.cs), [Verify.cs](Verify.cs), [VerifyRuntime.cs](VerifyRuntime.cs). Unity CLI eval_file로 Play 모드에서 실행한 문서용 자료이며 게임 빌드에 포함되는 런타임 스크립트가 아니다.
