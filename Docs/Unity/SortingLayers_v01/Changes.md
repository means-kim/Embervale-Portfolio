# GA1 Sorting Layer 적용 및 롤백 기준

사용자 요청: 역할별 Sorting Layer 적용, 작업 직전 상태 보존. 기존 작업 씬의 저장되지 않은 변경은 없음을 확인한 뒤 백업했다.

## 롤백 자료

- Before/Assets/01.Scenes/HyunseoScene.unity 및 .meta: 작업 직전 씬.
- Before/Assets/03.Prefabs/Map: 작업 직전 맵 프리팹 폴더 전체.
- Before/ProjectSettings/TagManager.asset: 작업 직전 Layer/Sorting Layer 정의.
- BeforeSorting.tsv: 현재 씬 Renderer와 기존 SortingGroup의 GlobalObjectId, Layer, Order. 새 SortingGroup은 이 목록에 없으므로 정렬만 롤백할 때 이번에 추가된 그룹을 구분할 수 있다.
- 원본 씬 SHA256: B8B8E61BF5A032A8CA51D0901C0213D7DB22E5F9C64098A536EDE41D7CAF0264.
- 이후 사용자 배치가 바뀌었다면 백업 씬을 덮어쓰지 말고 정렬 값과 이번 추가 SortingGroup만 선택 복구한다. 전체 복원은 이후 배치 변경도 되돌린다.

## 적용

현재 씬 Renderer 총 94개(비활성/보관 오브젝트 포함): GA1_BG_Far 8, GA1_BG_Mid 13, GA1_World 70, GA1_Actors 1, GA1_Foreground 2.

- Far: 하늘, 산 원경, 달, 비밀방 확장 배경.
- Mid: 안개, 성/다리, 중경 숲, 동굴 후면, 유적.
- World: 바닥, 눈, 발판, 입구/천장/벽, 지형 장식. Right deep rock은 이름상 Background 아래 있지만 기존 지형 가림 역할과 Order 70을 보존하기 위해 World로 분류.
- Actors: Player SpriteRenderer.
- Foreground: 왼쪽 나무 원본 및 새 전경 나무.
- HD 계열 Sorting Layer는 그대로 보존하고 사용하지 않음. 일반 Layer map/UI, 물리, 카메라, UI Canvas 설정은 변경하지 않음.
- 기존 Order 숫자 유지. 기존 SortingGroup이 있으면 그룹 Order 유지. 그룹 없는 모든 MeshRenderer에 SortingGroup을 추가하고 기존 Renderer Order 복사. 각 실제 아트 오브젝트 Inspector에서 조정 가능. 맵 루트에 통합 SortingGroup은 추가하지 않음.
- Assets/03.Prefabs/Map 내 56개 프리팹에 같은 분류 적용. 전체 배치 프리팹은 그 파일의 기존 배치 그대로 정렬만 변경; 현재 씬으로 덮어쓰지 않음.

## 확인

- BEFORE_0~6 / AFTER_0~6: 비밀방, 시작, 입구, 등반, 보스 출구, 중앙 바닥, 오른쪽 벽의 임시 640×360 렌더. 캡처 후 플레이어 원위치 복구.
- 2~6은 PNG 해시 동일. 0은 2265/230400 픽셀, 1은 4713/230400 픽셀 차이. 전후 실제 이미지 비교에서 뚜렷한 가림 오류는 관찰되지 않음. 픽셀 완전 동일이라고 주장하지 않음.
- SortingGroup 없는 MeshRenderer 0개 확인. 전체 이동 경로/동적 카메라/파괴 전환 검증은 이번 범위 아님.
- HyunseoScene과 변경 프리팹 저장 완료. 백업은 Docs 아래라 Unity 런타임 자산에 포함하지 않음.
