# 보스 지형 질감 수정 v14

- Built-in imagegen edit 사용. 원본 컨셉과 기존 소재, 실제 Unity 전후 렌더 참조.
- BG_Boss1_BreakFloor_v02 오브젝트 이름 유지, 그림은 BG_Boss1_BreakFloor_v03.png(1254×1254 RGBA)로 교체. 세로 암반 결로 양옆 지형과 맞춤.
- Boss right rock wall: 동굴 입구 공용 그림 대신 BG_Boss1_RightRockWall_v02.png(1024×1536 RGBA) 전용 그림 사용. 돌 크기 축소. 원래 동굴 입구 소재는 변경하지 않음.
- 새 PNG는 Assets/04.Sprites/Backgrounds/BossMap1, 새 .mat은 Assets/08.Materials/Map/Materials에 저장. 기존 PNG/머티리얼 보존.
- HyunseoScene과 Ground_Arena_BreakableFloor_Center, Terrain_Wall_RightBoundary, Map_CompleteLayout 프리팹에 적용.
- 위치, 메시, UV, 충돌, 표시 순서 및 가림 역할은 변경하지 않음. 눈은 기존 별도 소재 유지. 균열 상태 BG_Boss1_BreakFloorCracked_v02는 이번 변경 대상이 아님.
- Before_191/After_191: 중앙, Before_239/After_239: 오른쪽 벽. 1280×720, 직교 크기12의 고정 카메라 비교. 실제 플레이 완주 검증과 구분.
- 현재 MapReview/PortalMask는 Unlit. 조명 색/밝기 반응은 Lit 셰이더로 전환해야 하며 기존 vertex tint/alpha와 가림 연산을 보존해야 함. 2D Light와 일반 3D Light 경로를 혼용하지 말고 렌더러/셰이더 호환 확인 필요. 노멀맵은 입체 음영을 위한 선택적 추가 작업. 이번에 조명 기능 구현하지 않음.

## 실제 프롬프트

### 1

Edit target image 1: isolated breakable platform rock sprite. Image 2 is adjacent ground style reference, image 3 actual Unity placement showing the mismatched central insert, image 4 overall art concept. Produce ONLY revised image1 sprite with genuine transparent background and SAME square canvas framing: horizontal top at about 31% height, rock bottom about 76%, left/right reach 2%-98%. Preserve flat walkable top alignment and dark cold blue moonlit palette. Redesign internal rock texture to match the adjacent ground: predominantly vertical chunky irregular slate basalt masses, less diagonal slashes, larger varied vertical stones like image2. Reduce conspicuous triangular sawteeth at bottom to irregular rounded fractured masses; avoid isolated-looking ornamental slab. Left/right ends should be near vertical rock continuations for overlap with adjacent ground, no bright outlines or cut border. Retain very thin frosty top highlights but DO NOT add a thick snow cover (snow is separate). Similar subdued brightness to image2, no blue glow. Keep transparent outside rock. No text, no landscape, no characters. Professional painterly 2D game asset, crisp texture.

### 2

Edit target image1 isolated cave cliff sprite into an OUTDOOR frozen basalt boundary wall matching image2 ground and image3 concept. Keep SAME tall 2:3 canvas and original LEFT-side solid rock / RIGHT-side transparent negative-space orientation, same broad silhouette footprint with ledges around 20%,50%,80% height and bottom near88%. This will be mirrored and enlarged in Unity; use much SMALLER individual stones (roughly 3 times as many irregular stones per unit area), layered vertical strata with natural fractured seams, not huge repetitive triangular scales. Dark muted blue-gray rock, soft moonlit edge highlights, subtle frost on occasional horizontal shelves but no large snow shapes. Dense rock to left image edge continuing beyond top, completely transparent negative space right and below, no fog glow, no backdrop. Avoid giant protruding steps; integrate shelves subtly into cliff. Preserve outer footprint for existing UV mapping as closely as possible. Image4 actual current oversized wall placement for context, do not reproduce whole landscape. Output only revised transparent wall sprite. No text or characters.

