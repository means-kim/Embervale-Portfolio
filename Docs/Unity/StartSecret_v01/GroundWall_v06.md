# 시작 바닥 소재 기반 ㄴ자 벽 v06

사용자는 BG_Start_GroundApproach_v01의 질감을 사용해 바닥에서 왼쪽 벽으로 이어지는 아트를 만드는 안을 승인했다. 원본을 회전하지 않고 같은 비스듬한 암반 결을 참조하여 새 연결 이미지를 제작했다.

- PNG: Assets/04.Sprites/Backgrounds/StartMap/BG_Start_SecretGroundWall_v01.png. 요청 2:3, 실제 1024×1536 RGBA. 원본 바닥과 Bg_Concept.png를 실제로 확인하고 이미지 참조. Built-in image_gen 사용.
- 같은 이름의 전용 Mesh 및 Material: Assets/07.Datas/Map/Meshes, Assets/08.Materials/Map/Materials. Unity 무압축, maxTextureSize 2048. 실제 표시 크기 20×30, 위치 (-28.7,10.35,0), Scale 1. 오른쪽 30%에서 메시 버텍스 알파를 줄여 기존 바닥과 연결.
- Wall_SecretLeftBoundary 아래 배치. 이전 석조 벽과 반전 보스 벽은 비활성 보존하고 Portal surfaces 대상에서 제외. 새 Renderer를 entranceRole=1, exitRole=1로 등록.
- 새 아트 SortingGroup 28. 눈을 PNG에 합치지 않음. 기존 눈 아트 유지; 올라가는 벽 밑동에서는 새 암반이 가리며 평지로 이어짐.
- PolygonCollider2D를 거의 수직인 벽과 완만한 밑동에 맞춤. 높이 1/8/16/24에서 경계 검출. 실제 이동 및 점프 완주 검증 아님.
- HyunseoScene 및 Terrain_Wall_SecretLeftBoundary_v01.prefab 저장. 사용자 바닥 Transform, 나무, 카메라 변경 없음.
- 실제 16:9 구도 검토: LeftBoundary_v06.png.

## 정확한 프롬프트

Use case: precise-object-edit. Input1 is the EXACT ground rock texture to extend. Input2 concept is secondary style reference only. Create a transparent portrait 2:3 2D terrain sprite: an L-shaped connected rock cross section, solid rock on LEFT 30 percent for entire height, solid rock along BOTTOM 15 percent across entire width, everything upper-right genuinely transparent. A nearly vertical right boundary of left wall, tiny irregularities only, rounds gently through an inside concave corner into the horizontal flat ground at 85 percent canvas height. Ground and wall are ONE continuous rock mass. Use EXACT dark low-contrast small diagonal interlocking basalt scales and blue-gray surface detail of input1, same rock size in both wall and floor. NO large boulders, no protruding shelves, no footholds, no masonry blocks, no bright bevels, no perspective front ledge. Preserve subdued top-right moonlight as in input1; do not rotate lighting. Top edge of wall extends beyond canvas; left and bottom filled solid to edges. Bottom floor extends beyond right edge. NO snow, vegetation, sky, mist, objects, outlines, text, pillars, or ground slope. Close texture match to input1 is more important than ornate detail. Wall should blend into the existing floor rather than look like foreground.
