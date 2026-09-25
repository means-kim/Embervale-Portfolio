# 비밀방 석조 벽 교체 v04

사용자가 돌출 암벽 대신 단순한 벽을 요청하여 새로운 석조 벽 아트로 교체했다. 기존 바위 아트 파일은 보존하되 현재 경계에서 사용하지 않는다.

- 새 PNG: Assets/04.Sprites/Backgrounds/StartMap/BG_Start_SecretMasonryWall_v01.png. 요청 portrait 2:3, 실제 1024×1536. 투명 배경 생성. Unity 무압축, 최대 2048. 메시 표시 24×36, Transform Scale 1; 픽셀 밀도 증가를 의미하지 않음.
- 전용 Mesh: Assets/07.Datas/Map/Meshes/SecretMasonryWall_v01.asset. Material: Assets/08.Materials/Map/Materials/BG_Start_SecretMasonryWall_v01.mat.
- HyunseoScene / 01_Start / 00_SecretLeft / Wall_SecretLeftBoundary / WallArt. 아트 중심 (-35.8,9,0), SortingGroup 25로 바닥과 눈 뒤에 표시.
- Collision은 BoxCollider2D로 교체. 우측 X=-31, Y=-13~31. 높이 1/8/16/24에서 벽 충돌 검출. 실제 이동 완주 검증 아님.
- 사용자 바닥/나무/배경/카메라 변경 없음. 씬 및 Terrain_Wall_SecretLeftBoundary_v01.prefab 저장.
- 검토: LeftBoundary_v04.png, 1280×720, Orthographic Size 12, 기존 비밀방 고정 구도 기준점. 임시 캡처 후 플레이어 위치 복원.

## 생성 방식과 정확한 프롬프트

Built-in image_gen. 참조: Assets/04.Sprites/Bg_Concept.png 및 Docs/Unity/StartSecret_v01/LeftBoundary_v03.png. 두 이미지를 실제로 확인 후 참조했다.

Use case: stylized-concept. Create ONE isolated 2D game boundary wall asset, portrait 2:3, transparent background. Reference1 is the art style and cold blue palette. Reference2 is the actual scene to fit: replace the protruding rocky LEFT cliff with a SIMPLE FLAT VERTICAL OLD MASONRY WALL. DO NOT copy the cliff shape. Wall built from understated rectangular dark aged stone blocks, shallow mortar, small weather cracks, flat frontal side-view surface, no perspective side face, no shelves, ledges, platforms, steps, jutting rocks, buttresses, arches, doorways, holes, windows or pillars. It must read as solid impassable wall, not climbable platforms. Wall occupies left 70 percent canvas all the way from top to bottom, nearly straight right silhouette at x70 percent with very small chips only; right 30 percent genuine transparency. Wall continues beyond TOP LEFT and BOTTOM canvas edges; NO visible wall top or bottom caps, no floating object. Low contrast dark slate navy, subtle hand-painted texture matching castle ruins in reference1, diffuse cold night ambient light, no bright rim or dramatic bevel. No snow baked in, no ground, sky, trees, text or background. Calm simple massive wall surface.
