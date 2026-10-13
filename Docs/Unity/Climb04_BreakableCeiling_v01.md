# 등반 구간 6회 공중 타격 천장

최신은 [스프라이트·파괴 피드백 v02](Climb04_BreakableCeiling_v02.md). 아래는 최초 구현 이력이며, 공중 공격권 반환 연동은 사용자 요청으로 제거되었다.

- 대상: MainTestScene의 Map/04_Climb/BreakableFloor_Center. 보스방의 동명 오브젝트와 원본 프리팹은 변경하지 않았다.
- 기존 상태: 정상·균열·파편 아트와 BoxCollider2D만 존재. 공격으로 파괴하는 컴포넌트는 없었다.
- 교체 이미지: [SP_Climb04_BreakableCeiling_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/Climb04/SP_Climb04_BreakableCeiling_v01.png). 내장 imagegen 도구로 기존 BG_Climb_Ceiling_v01을 참조해 제작. 실제 PNG 2172×724, 투명 배경. 기존 눈과 보스방 장식은 대상 인스턴스에서 제거했다.
- 불투명 영역 Rect(4,236,2164,256)을 별도 Sprite 에셋으로 참조. 원본 PNG는 자르거나 늘여 저장하지 않았다. 표시 폭 20.44, 높이 약 2.418. 실제 충돌은 양쪽 벽 사이 X138~156, 하단 Y60.25에 맞췄다. 기존 얇은 바닥 충돌을 암반 두께에 맞춘 천장 충돌로 변경했다.
- 새 컴포넌트: [AirComboBreakableCeiling.cs](../../Assets/02.Scripts/Map/AirComboBreakableCeiling.cs), Collision/Central break floor collision에 부착. Required Hits=6.
- 판정: 활성화된 PlayerAttackInstance의 실제 공격 Trigger만 계산. 플레이어가 공중일 때 누적하며, 한 공격 인스턴스의 복수 콜라이더·Trigger Stay는 중복 계산하지 않는다. 바닥/발판 착지 또는 사망 시 미완성 타격 횟수와 균열 표시를 초기화한다. 벽 접촉과 추가 점프는 초기화하지 않는다.
- 여섯 번째 명중에서 CeilingVisual과 충돌을 비활성화한다. 이후 착지해도 부서진 천장은 복구하지 않는다. 씬을 다시 로드하면 초기 상태로 시작한다.
- 기존 플레이어는 공중 공격 3회 제한이므로, 이 천장 명중 시 현재 실행 중인 공중 공격권 1회를 반환하도록 PlayerAttack에 좁은 연동 메서드를 추가했다. 일반 공격의 최대 횟수 설정은 바꾸지 않았다.
- 시각 피드백: 타격 시 짧은 밝기 변화와 누적 균열 선. 조명과 구역별 가림 등록 유지.

## 검증

Unity Play 모드에서 실제 UpAttack 프리팹을 천장에 겹쳐 Physics2D Trigger를 발생시켜 확인했다. 자동 테스트 동안 플레이어 위치를 고정하고 입력을 끈 상태이며, 자연스러운 이동 경로 완주 검증은 아니다. Play 상태는 저장하지 않았다.

- 한 공격이 지속해서 겹쳐도 타격 1회만 계산, 공격권 1회 반환.
- 2회 명중 후 임시 Ground 충돌에 실제 착지: Grounded=true, 타격 0회.
- 지상에서의 공격은 계산하지 않음.
- 다시 공중에서 실제 공격 6회: 1~5회 유지, 6회째 그림과 충돌 제거.
- 파괴 후 착지해도 천장이 복구되지 않음.
- 천장이 없는 상태에서 일반 공중 공격 3회 후 4번째 공격 거부: 기존 제한 유지.
- 주변과 함께 16:9 렌더 확인. Edit 모드 복귀 후 정상 천장과 충돌이 초기 상태로 복원되었는지 확인.

## 이미지 생성 프롬프트

```text
Use case: stylized-concept. Asset type: transparent 2D side-scrolling cave breakable ceiling slab. Reference 1 existing cave ceiling defines the exact hand-painted fractured blue-black rock art style. Create ONE independent horizontal thin broad rock slab for a breakable cave roof, approximately SIX TIMES as wide as it is thick. Landscape canvas; slab spans most of canvas width and sits centered with transparent space above/below. Broad near-flat top and nearly-flat underside, subtly jagged chipped edges no long stalactites. Interlocking charcoal/navy slate chunks with a few readable hairline fractures suggesting a destructible weak ceiling, no huge central crack/gap yet. Same dense layered craggy dark stones as reference, dim cool bluish facets, subdued lighting, no emissive lines, no white highlights. Front-on orthographic side view, no perspective, no ground/background, no snow, ice, foliage, structures, characters, text, frames, multiple variations or debris floating outside. Actual transparent alpha, clean isolated single slab. Stones have natural irregular size; continuous rock across full width, not a row of identical blocks.
```
