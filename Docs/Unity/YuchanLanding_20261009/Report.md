# YuchanScene 착지 파티클 표시 복구 — 2026-10-09

## 원인 및 수정

원본 LandParticle은 Sprite-Lit-Default 머티리얼과 Default Sorting Layer/Order 0을 사용했다. YuchanScene의 기존 조명은 Default를 비추지 않으며 GA1_World 지형은 Default보다 앞에 그려진다. 이 조합은 착지 효과를 검게 만들거나 지형 뒤에 가린다.

원본 프리팹을 보존하고 `Assets/03.Prefabs/VFX/LandParticle_Yuchan.prefab` 변형을 생성했다. ParticleSystemRenderer만 GA1_Actors, Order 1로 설정했다. 원본의 파티클 모양, 머티리얼, 발산 수, 수명 및 재생 동작은 그대로 상속한다.

YuchanScene의 EffectManager `_landVfx` 참조 한 곳만 이 변형으로 교체했다. 광원 설정이나 착지 판정 코드는 변경하지 않았다.

## 검증

- 두 번째 Play 검사에서 **자동 착지**로 파티클 60개, 재생 시간 0.103초를 관측했다. 수동 PlayLand 검증 호출을 하기 전에 자동 착지를 포착했다.
- [실제 Play 렌더](play-landing.png)에서 발 주변 착지 파티클 표시 확인.
- 런타임 ParticleSystemRenderer: GA1_Actors, Order 1, Sprite-Lit-Default.
- 저장 후 씬 재열기에서 변형 참조 유지 및 dirty=False 확인.
- LandParticle 원본, HyunseoScene, MainTestScene, 공용 Player.prefab은 작업 시작 시점과 SHA-256 동일.
- 씬의 이번 변경은 `_landVfx` 참조 한 줄이다. 변형 PNG 없이 prefab/meta만 추가했다.
- git diff --check 통과. 임시 검증 코드는 프로젝트에서 제거하고 .cs.txt로만 보존했다.

첫 자동 검사는 짧은 효과를 관측하지 못했으나 재검사에서 실제 자동 착지 렌더를 확보했다. 최초 timeout 기록은 재검사 성공을 대체하지 않는다.
