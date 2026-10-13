# 동굴 천장 아트와 파괴 피드백 v02

- 대상은 MainTestScene의 Map/04_Climb/BreakableFloor_Center. 사용자가 옮긴 위치 (147.2,47.6)를 유지했다.
- 이미지: [SP_Climb04_BreakableCeiling_v02.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/Climb04/SP_Climb04_BreakableCeiling_v02.png). 내장 imagegen 사용. BG_Climb_Ceiling_v01과 BG_Climb04_BackRockTile_v01을 실제 이미지 참조로 사용해, 둥근 돌이 늘어선 형태를 작은 자연 암반 층과 불규칙한 단면으로 수정했다.
- 실제 PNG 2172×724 RGBA. 불투명 Rect(21,208,2123,306)을 별도 Sprite에서 참조한다. 폭 20.44, 높이 약2.946의 원본 비율로 표시. 기존 천장 충돌의 하단은 유지하고 새 그림 두께에 맞춰 위쪽으로 조정했다. 원본 v01은 보존했다.
- PlayerAttack 코드의 공격 횟수 설정·초기화는 변경하지 않는다. 맵의 RefundAirAttackForEnvironmentHit 호출도 제거했다. 현재 PlayerAttack.cs에는 이번 변경의 Git diff가 없다.
- [AirComboBreakableCeiling](../../Assets/02.Scripts/Map/AirComboBreakableCeiling.cs)는 천장에 맞은 서로 다른 공격 인스턴스만 센다. 6회 파괴와 미완성 시도 착지 초기화는 유지한다.
- 명중: 그림만 흔들림(기본0.18유닛, 누적에 따라 증가), 0.16초 밝기 변화, 0.025초 hitstop, 짧은 카메라 충격, 돌가루와 작은 암반 조각4개.
- 누적 손상: 15개의 불규칙한 균열 경로를 5단계로 노출. [CeilingFracture.shader](../../Assets/08.Materials/Map/CeilingFracture.shader)에서 암반 PNG의 실제 알파로 잘라 공중에 선이 떠 보이지 않게 하고, 구역별 가림을 유지한다.
- 최종 파괴: 즉시 충돌을 해제하고, 0.1초의 강한 흔들림·0.065초 hitstop·카메라 충격 뒤 원본 그림을 지그재그 경계의 메시9개로 분리한다. 조각이 퍼지고 회전·낙하한 뒤 1.7초 내 사라지며, 여러 곳에서 회색 남청색 연무가 퍼진다.
- 파편은 [CeilingRockFragment](../../Assets/02.Scripts/Map/CeilingRockFragment.cs)로 움직이며 충돌·피해 판정이 없다. 메시와 파티클은 종료 시 제거된다. 연무는 프로젝트의 기존 CFXR smoke cloud x4 blurred 텍스처를 사용했다. 별도 사운드는 추가하지 않았다.

## 확인

Play 모드에서 테스트용 UpAttack 프리팹을 직접 생성해 천장 판정과 시각 효과를 확인했다. 플레이어 공격 예산을 바꾸거나 초기화하지 않았다. 자연스러운 조작으로 6회 공격하는 동선 완주 테스트는 아니다.

- 실제 Trigger 접촉으로 타격을 계산하며, 같은 인스턴스의 지속 접촉은 중복 계산하지 않음.
- 타격 후 그림의 위치 흔들림과 돌가루·파편 생성 확인. 충돌은 흔들지 않음.
- 3회 명중 시 균열 경로9개가 보이는 16:9 렌더 확인. 알파 마스킹 적용 후 공중으로 튀어나온 선을 제거했다.
- 임시 Ground에 실제 착지하면 타격0, 균열0, 그림 위치 복원.
- 다시 1~5회 유지, 6회째 충돌 해제와9개 큰 파편 생성. 파괴 장면16:9 렌더 확인.
- 2.1초 뒤 천장 파편·연무 모두0개로 자동 정리되는 것을 확인.
- 테스트 중 PlayerAttack.CurrentAirAttackCount는 변경되지 않았다. Play 상태와 테스트 오브젝트는 저장하지 않았다.

## 생성 프롬프트

```text
Use case: stylized-concept. Make ONE transparent horizontal destructible natural cavern roof patch for a 2D game. References 1 existing ceiling, 2 rear wall are exact material/style references. Shape: an irregular elongated slab approximately 7 times wider than tall, with near-flat upper boundary and shallow jagged underside. Dense layered chipped slate rock, small organically irregular crags and fissures matching the reference, desaturated dark charcoal/navy blue, very restrained cool facets. It MUST look like a fragment of NATURAL solid cave rock, not masonry, not a row of large rounded cobblestones/bricks, not a floating platform. Avoid polished geometric blocks, repeated triangular tooth pattern, snowy/white edges, bright shiny rim, deep dangling stalactites. Interwoven small angular rock strata and grain with 5 irregular branching subtle structural fracture paths spaced across the width; intact closed hairline cracks, not transparent gaps yet. Width nearly fills a wide landscape canvas, centered vertically with genuine transparent space surrounding the complete slab. Orthographic front-on, no perspective, no sky, no background, no text, no extra objects, no labels. Maintain low contrast and small texture scale so it fits existing cave walls. One single image, actual transparent alpha.
```
