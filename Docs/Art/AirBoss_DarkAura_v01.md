# 공중 보스 다크 오라 v01

2026-10-12 사용자 승인 컨셉을 바탕으로 구현.

- 적용: [AirBoss.prefab](../../Assets/03.Prefabs/AirBoss/AirBoss.prefab)의 `DarkAura` 자식. MainTestScene의 기존 프리팹 인스턴스에 자동 반영된다. 다른 씬에서 같은 프리팹을 사용해도 적용된다.
- 먹청색 후광, 낮은 채도의 보라색 노이즈 가장자리, 두 어깨의 상승 연기, 소수의 청백색 입자. 생성 샘플의 분위기를 실시간 셰이더와 파티클로 근사했으며 샘플 이미지를 그대로 붙인 방식은 아니다.
- 기존 보스 스프라이트·애니메이션·공격·충돌은 변경하지 않았다. 후광과 입자는 보스보다 뒤에 렌더링한다.
- Inspector: `DarkAura / AirBossDarkAura`의 Opacity, Halo Size, Halo Offset, Follow Lag, Phase Expansion을 조정한다. 연기량/속도는 두 어깨의 기본 ParticleSystem에서 수정한다. 색상은 [머티리얼 폴더](../../Assets/08.Materials/AirBossAura)에서 수정한다.
- 페이즈마다 후광 범위 9% 증가. 느린 호흡, 이동 지연, 순간이동 시 지연 초기화. 보스 SpriteRenderer가 숨겨지면 효과 즉시 제거. 사망 상태에서 후광 소멸 및 입자 정리. 컴포넌트 비활성화 시 전체 효과 정리.
- 최대 입자 96개(36+36+24), 후광 메시 1개. 성능 수치는 별도 프로파일링하지 않았다.

## 확인 범위

- Unity MCP에서 컴파일 및 셰이더 오류 없음 확인.
- MainTestScene Play 모드의 기존 등장/전투 실행 및 카메라 캡처 확인.
- 실행 중 보스 숨김→후광 비활성/입자 0개, 재표시→방출 재개 확인.
- 격리된 런타임 상태 설정으로 3페이즈 크기 증가 및 사망 상태의 후광/입자 소멸 확인. 전체 전투를 실제 조작으로 클리어한 테스트는 아니다.
- [실제 Unity 효과 캡처](AirBoss_DarkAura_v01.png)는 전신 확인을 위해 실행 중 보스 위치와 임시 카메라를 조정한 검토 화면이다. 기존 게임 카메라의 확정 구도가 아니다. 임시 변경은 Play 종료 시 폐기했다.
- Play 종료. MainTestScene 파일은 저장/변경하지 않았다.

## 파일

- [효과 제어 코드](../../Assets/02.Scripts/Monster/AirBoss/Animation/AirBossDarkAura.cs)
- [연기 셰이더](../../Assets/08.Materials/AirBossAura/DarkColdSmoke.shader)
- 위 폴더의 후광/연기/얼음빛 입자 머티리얼 및 Quad 메시와 각각의 .meta.
