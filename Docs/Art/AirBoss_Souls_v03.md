# 공중 보스 푸른 영혼 효과 v03

2026-10-12. 사용자 [참고 이미지](References/BlueSoul_UserReference_v01.png)의 푸른 연무와 작은 빛 입자를 바탕으로, 기존 고드름을 영혼 불씨로 교체했다. 참고 이미지 자체를 게임 스프라이트로 사용하지 않는다.

## 페이즈

- 1: 푸른 영혼 불씨, 작은 빛 입자, 옅은 연무. 각 입자는 불규칙한 간격으로 몸 둘레에서 발생하고 위·옆으로 떠오르다 사라진다.
- 2: 냉기 입자 없음, 기존 전기 아크 유지.
- 3: 냉기 입자가 돌아오고 전기 아크 발생 지점에서 밝은 불씨와 작은 입자가 추가로 발생한다.
- 기존 DarkAura 유지. 고드름 스프라이트 참조와 메시 생성은 현재 컴포넌트에서 제거했다. 사용자 요청으로 미사용 Crystal 머티리얼·FacetedCrystal 메시와 이전 v01/v02 검토 문서·캡처를 삭제했다. 공격용 원본 고드름은 보존한다.
- FrostMistLit2D와 PhaseAura_FrostMist는 최종 영혼 효과의 옅은 연무에서 계속 사용하는 파일이므로 유지한다. PhaseAuraUnlit도 최종 전기 머티리얼에서 사용한다.

## 구현 및 조절

[프리팹](../../Assets/03.Prefabs/AirBoss/AirBoss.prefab)의 PhaseAura / AirBossPhaseAura에서 조정한다.

- Soul Size 기본 1.8, Soul Spawn Gap 0.18~0.42초, Frost Opacity 0.8. Radius/Center는 기존 몸 둘레 범위를 유지한다.
- World 공간 ParticleSystem 3개를 재사용한다. 불씨 최대 32, 작은 입자 최대 64, 연무 최대 48개. 리깅 불필요, 데미지 없음.
- 생성 위치는 보스를 따라가고 이미 발생한 입자는 뒤에 남는다. 한 프레임 이동이 Teleport Clear Distance(12 월드 단위)를 넘으면 잔상을 정리한다.
- 불씨는 절차적 셰이더로 밝은 중심과 휘는 꼬리, 부드러운 테두리를 만든다. 별도 이미지/패키지 생성·설치 없음. 연무는 2D 조명에 반응하며 불씨 중심에만 약한 자체 발광(0.18)을 더한다.
- 전환 요청 즉시 전체 효과와 입자를 숨긴다. 전환 중/2페이즈 컷신 중 숨김을 유지하며, 사망·비활성화에도 정리한다.
- 구현: [제어 코드](../../Assets/02.Scripts/Monster/AirBoss/Animation/AirBossPhaseAura.cs), [불씨 셰이더](../../Assets/08.Materials/AirBossAura/BlueSoulLit2D.shader), [머티리얼](../../Assets/08.Materials/AirBossAura/PhaseAura_BlueSoul.mat).

## 검증

- MainTestScene을 Play에서만 로드. 원래 MinsuScene은 저장/변경하지 않고 Play 종료 후 복귀했다.
- 1페이즈 표본: 불씨 8, 작은 입자 16, 연무 5. 2페이즈 실제 컷신 전환 요청 직후 모든 입자 0 및 루트 비활성, 완료 후에도 입자 0.
- AI를 끄고 Health/CompletePhase로 3페이즈 진행: 불씨 20, 작은 입자 20, 연무 5 및 전기 표시 확인. 3페이즈 컷신 전체 검증은 아니다.
- 이동 직후 기존 입자 월드 위치 유지 확인. 30단위 순간 이동 후 입자 0 확인. 비활성화와 실제 사망 후 각각 루트 비활성/입자 0.
- 모든 Light2D intensity를 0으로 한 실제 렌더 확인: 연무는 어두워지고 중심 발광만 희미하게 남는다. 검사 후 조명을 복원했다.
- 새 셰이더 오류 0, AirBossPhaseAura 관련 콘솔 오류 0. 기존 Unity AI NoSubscription 메시지는 본 효과와 별개다.
- 전체 전투 완주·모든 카메라 거리·성능 프로파일링은 미검증.

## Unity 캡처

임시 전신 검토 카메라와 보스 위치로 촬영했다. 실제 게임 카메라 구도가 아니다. 2·3페이즈 캡처는 실제 2페이즈 컷신으로 달빛이 꺼진 이후 조명 상태다.

- [1페이즈](AirBoss_Souls_P1_v03.png)
- [2페이즈](AirBoss_Souls_P2_v03.png)
- [3페이즈](AirBoss_Souls_P3_v03.png)
