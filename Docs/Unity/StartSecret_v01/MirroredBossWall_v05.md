# 보스 오른쪽 벽 반전 배치 v05

사용자의 명시 요청에 따라 현재 씬의 Boss right rock wall을 자식 WallFootJoin과 함께 그대로 복제. 복제본 이름 Boss right rock wall_Mirrored. 원본 보스 벽과 공유 메시/Material은 수정하지 않음.

- 부모: Wall_SecretLeftBoundary. 복제본 월드 위치 (212,-34,0), localScale (-1,1,1). 원본 크기 유지, 좌우 반전만 적용. 원본 메시의 좌표가 월드형으로 구워져 있어 Transform 위치가 실제 보이는 위치와 다름.
- SortingGroup 25, MapPortalVisibility entranceRole 1 / exitRole 1. 석조 WallArt는 비활성 보존.
- 왼쪽 경계 충돌은 반전된 암벽의 주요 화면 윤곽을 따르는 PolygonCollider2D로 조정. 실제 이동/점프 완주 검증은 미실시.
- 기존 바닥 및 나무는 변경하지 않음. HyunseoScene과 Terrain_Wall_SecretLeftBoundary_v01.prefab 저장.
- 검토 화면 LeftBoundary_v05.png. 새 이미지 생성 없음.
