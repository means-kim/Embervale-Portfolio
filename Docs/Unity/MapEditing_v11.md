> **v13 우선**: 현재 아트는 기본 MeshFilter/MeshRenderer, 충돌은 기본 Collider로 편집합니다. ReviewArt/JSON/자동 생성기 설명은 과거 내용입니다. [최신 안내](../../Assets/03.Prefabs/Map/README.md).

> **v12 경로 변경**: 현재 작업 씬은 Assets/01.Scenes/HyunseoScene.unity, 공유 프리팹은 Assets/03.Prefabs/Map입니다. MapReview는 로컬 전용입니다. 아래 과거 경로 대신 [최신 팀 공유 안내](../../Assets/03.Prefabs/Map/README.md)를 따르세요.

# 맵 배치·추가·삭제 안내

현재 편집 씬은 [MapReview_v02](../../Assets/MapReview/MapReview_v02.unity)입니다. 정리 전 백업은 [Before_Organization_v11](../../Assets/MapReview/Before_Organization_v11.unity)입니다. 기존 메시 합성 방식과 충돌선을 보존하면서 편집 부모와 재사용 프리팹을 추가했습니다.

## 어디에서 편집하나요?

각 구역 `01_Start`, `02_Climb`, `03_Corridor`, `04_Boss`, `05_Drop` 아래에서 찾습니다.

| 폴더 | 내용 |
|---|---|
| Background | 하늘, 먼 풍경, 숲, 동굴 후면 |
| Terrain | 바닥, 발판, 오른쪽 경계벽과 대응 충돌 |
| Decoration | 장식, 천장·외벽 아트, 별도 눈 |
| Hazards | 함정 배치용 폴더. 현재 비어 있으면 아직 함정이 없는 것 |
| Transitions | 동굴 입구·보스 출구 가림 기준점 |
| 00_ReviewSystems | 임시 플레이어·카메라·체크포인트 |
| 99_Archive | 비활성 이전 소재·빈 과거 컨테이너. 복원할 때만 참고 |

## 발판을 추가하고 움직이기

1. Project의 `Assets/MapReview/Prefabs`에서 `Platform`을 검색합니다.
2. 프리팹을 해당 구역의 `Terrain`으로 끌어 놓습니다. 또는 기존 발판 부모를 `Ctrl+D`로 복제합니다.
3. **Platform 부모**를 이동하면 `RockArt`와 `Collision`이 함께 이동합니다. Scene 도구를 Pivot 모드로 두면 발판 착지선 중앙의 기준점을 사용할 수 있습니다.
4. 폭과 간격은 배치 후 조정합니다. 부모 X Scale은 충돌 폭도 함께 바꾸지만 그림도 늘어나므로, 비슷한 폭의 프리팹부터 고르는 편이 좋습니다.

추천 시작 소재: 좁은 발판 `Terrain_Platform_Ledge_p3`, 중간 발판 `Terrain_Platform_Ledge_p2`, 넓은 쉼턱 `Terrain_Platform_Ledge_rest`. 등반 8개·보스 3개의 독립 발판이 있습니다.

## 빼기·눈만 숨기기·그림 바꾸기

- 발판 전체를 빼려면 부모를 비활성화하거나 씬의 해당 인스턴스를 삭제합니다. Project의 프리팹 원본을 삭제할 필요는 없습니다.
- 눈이 분리된 바닥·장식은 `SnowArt`만 끌 수 있습니다. 바위와 충돌은 유지됩니다. 눈이 없는 발판에 빈 눈 소재를 추가하지는 않았습니다.
- 그림 교체는 하위 `Review Art`의 `Texture`, 앞뒤 순서는 `Order`입니다. 변경 시 미리보기가 갱신됩니다.
- Texture 교체는 기존 메시·UV 형태를 유지합니다. 종횡비나 투명 여백이 크게 다른 그림은 별도 메시 또는 일반 SpriteRenderer 오브젝트로 추가하는 것이 맞습니다.
- `Geometry`, `Batch Index`, 생성된 Render는 일반 배치 시 수정하지 않습니다. 자동 렌더 자식은 하이어라키에서 숨겼습니다.
- 이번 씬의 전용 카메라는 31번 레이어를 사용합니다. 새 SpriteRenderer를 직접 만들면 해당 레이어와 렌더 순서를 맞추세요. 일반 SpriteRenderer는 현재 ReviewArt 가림에 자동 등록되지 않습니다.
- 프리팹 인스턴스 수정은 해당 배치에만 남습니다. 모든 복사본에 반영하려는 변경만 Apply 하세요. 개별 자식 삭제가 제한되면 비활성화하거나 해당 인스턴스를 Unpack한 후 편집할 수 있습니다.

## 입구·출구와 새 배경

`01_Start/Transitions/Portal_CaveEntrance`는 입구 외벽·눈·고드름과 전환 기준점을 포함합니다. 부모 X를 옮기면 가림 기준도 따라갑니다. `03_Corridor/Transitions/Portal_BossExit`는 보스 출구 전환 기준점입니다. 통로 바닥·천장은 별도이므로 함께 옮길지 선택합니다.

이 가림은 현재 두 경계의 X 위치를 기준으로 합니다. 새로운 방을 임의로 만드는 범용 룸 시스템은 아닙니다. 입구 높이·모양을 바꾸면 셰이더의 상부 높이 6~10 및 입구 UV 영역도 별도로 맞춰야 합니다.

기존 아트의 가림 역할은 이름 대신 명시적인 값으로 저장했습니다. 같은 역할의 프리팹을 복제해 **맵 루트 아래**에 배치하면 최대 약 0.25초 이내 자동 검색됩니다. 다른 구역용 배경으로 재사용할 때는 아래 역할도 확인합니다.

| Review Art 항목 | 값 |
|---|---|
| Portal Role | -2 가림 제외, -1 이전 이름 기반, 0 일반, 1 시작 외부 풍경, 2 내부 후면, 3 동굴 외벽·천장, 4 내부 발판·종유석 |
| Exit Role | 0 출구 특수 처리 없음, 1 보스 통로 후면, 2 보스 야외 풍경 |
| Entrance Mouth | 현재 시작 동굴 입구 이미지의 입구 영역 마스크 |

## 중앙 파괴 바닥과 보스 지형

`04_Boss/Terrain/Ground_Arena` 아래 `BreakableFloor_Center`에 정상(Intact)·균열(Cracked)·파괴(Broken) 아트와 충돌이 함께 있습니다. 기존 임시 플레이어의 B키 참조를 유지했습니다. 새 파괴 바닥 복사본은 B키에 자동 연결되지 않습니다.

보스 양쪽 경사 바닥은 기존에 하나의 메시로 합성되어 있어 `Ground_Arena` 단위로 관리합니다. 좌우 경사 일부를 독립적으로 빼려면 추가 메시 분할이 필요합니다. 이 정리에서는 아트를 재제작하거나 임의의 절단선을 만들지 않았습니다.

낙하 양쪽 경계 충돌은 `05_Drop/Terrain`에 별도로 있고, 착지 바닥은 `Ground_Landing`입니다. 지형을 크게 옮기면 `00_ReviewSystems/Review checkpoints`와 낙하 시간도 함께 확인하세요.

## 보존과 검증

- 정리 직후 기존 오브젝트 월드 변환 차이 0, 충돌 직렬화 데이터 변경 0.
- 51개 편집 묶음, 재사용 프리팹 50개. 보스 전체 경사는 씬 묶음으로 유지했습니다.
- [전후 위치 검사](Organization_v11_checks.txt), [복제·이동·파괴 상태 및 렌더 검사](Organization_v11_validation.txt), [요소 목록](Organization_v11_inventory.txt).
- 전체 경로 이동·실제 보스 공격·눈 제거 기능 검증과는 별개입니다.
- 과거 `Apply ...`, 생성기, 이전 배치 정리 메뉴를 재실행하지 않습니다. 사용자 수동 배치 이후의 기본 작업은 프리팹 추가와 Transform 편집입니다.
- 씬 백업에는 공유 C#·셰이더·프리팹의 과거 버전까지 포함되지 않습니다.

최종 확인: 9개 고정 카메라 중 6개는 전후 픽셀 동일, 나머지 3개는 RGB 차이 최대 1/255. 등반 출구의 동일 Order 겹침을 46→47→48의 명시적 순서로 고정하여 기존 화면을 복원했습니다. 51개 묶음 이동·복원 오차 최대 0.00000382 유닛, 좁은/중간/넓은 발판 프리팹 생성 및 가림 대상 검색 통과, 파괴 바닥 3상태 참조 통과. Play 진입과 편집 모드 복귀 확인. 전체 동선 완주 검증은 포함하지 않습니다.
