# 비밀방 왼쪽 경계

사용자 요청으로 기존 BossMap1 암벽 메시/Material을 재사용해 왼쪽을 막았다. 새 PNG 생성 없음. 기존 보스 암벽은 변경하지 않았다.

- 씬: HyunseoScene / 01_Start / 00_SecretLeft / Wall_SecretLeftBoundary
- 부모 위치 (-38,7,0), RockArt와 Collision 분리. 원본 메시 중심을 원점으로 옮기고 X 반전 및 0.4 균일 배율을 적용한 독립 메시 SecretLeftBoundary_v01.asset.
- 표시 순서 Default 32. MapPortalVisibility entranceRole=1, exitRole=1 등록.
- BoxCollider2D 우측 경계 X=-35.8, Y=-8~36. 기존 보스 벽과 같은 물리 레이어. 플레이어 레이어와 충돌 무시되지 않음 확인.
- 높이 1/8/16/24에서 왼쪽으로 Linecast하여 경계 검출. 실제 이동 완주 검증은 아님.
- 프리팹 Assets/03.Prefabs/Map/Terrain_Wall_SecretLeftBoundary_v01.prefab.
- 16:9 화면 확인 후 암벽 아트를 하단으로 6.8 내려, 향후 바닥 아래까지 이어지도록 보완. 충돌의 월드 범위 유지.
- 시작 바닥은 Ground_Start 부모 전체를 복제해 아트/눈/충돌을 함께 옮길 수 있음. 이번에는 사용자가 배치하도록 바닥 복제는 하지 않음. 실제 고정 카메라 로직과 상호작용 기능은 여전히 미구현.

이 문서는 Changes.md의 과거 '좌측 경계 없음' 상태를 대체한다.

## v02 — 막힌 벽으로 보이도록 보완

- 사용자 피드백: 기존 벽이 막힌 것으로 보이지 않음. 고정 화면에서 가느다란 암벽 끝만 보이는 점 확인.
- RockArt를 부모 기준 (3.7,-1.5,0)로 옮기고 1.4 균일 확대. 부모 위치는 유지. 두꺼운 벽면이 화면 왼쪽과 바닥 아래까지 이어짐. 사용자 복제 Ground_Start는 변경하지 않음.
- 기존 직사각형 충돌을 PolygonCollider2D로 교체해 확대된 벽의 주요 굴곡에 맞춤. 이전 X=-35.8 고정 경계는 더 이상 현재값이 아님. 높이 1/8/16/24에서 새 벽 충돌 검출 확인. 실제 이동/점프 완주 검증은 아님.
- HyunseoScene 및 Terrain_Wall_SecretLeftBoundary_v01.prefab 저장. 최종 고정 화면: LeftBoundary_v02.png.

## v03 — 전경처럼 튀어나오는 느낌 완화

- 사용자 피드백에 따라 RockArt의 Renderer/SortingGroup 순서를 32에서 25로 낮춰 시작 바닥(26)/눈(27) 뒤에 표시.
- 전용 메시의 버텍스 색상을 (0.66,0.70,0.77,1)로 조정해 밝은 암반 모서리를 억제. 원본 PNG 및 공유 Material 변경 없음.
- 벽 위치와 충돌, 사용자 바닥 배치는 유지. HyunseoScene과 벽 프리팹 저장. 16:9 실제 렌더 LeftBoundary_v03.png 확인.
