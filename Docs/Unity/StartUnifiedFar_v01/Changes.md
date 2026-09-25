# 시작 원경 통합

요청: Start upper sky와 BG_Start_Far_v02를 하나의 큰 원경으로 관리. 기존 설산/달 구도와 비밀방 접합 유지.

## 최종 적용 방식

새 PNG 한 장으로의 교체는 적용하지 않았다. Built-in image_gen으로 두 번 시도했으나 달과 산의 위치 보존에 실패했다. 사용자에게 변경 이유를 알리고 기존 두 원본 텍스처를 단일 MeshRenderer 오브젝트로 통합했다. 따라서 이미지 파일은 두 개를 참조하며 실제 드로우콜 1회라는 뜻도 아니다.

- 오브젝트: 01_Start/Background/BG_Start_FarUnified_v01.
- 프리팹: Assets/03.Prefabs/Map/Background_Start_FarUnified_v01.prefab.
- 메시: Assets/07.Datas/Map/Meshes/Start_FarUnified_v01.asset. 두 submesh와 원래 Material 두 개를 사용한다. 새 셰이더/런타임 스크립트 없음.
- 크기: 원래 geometry 그대로 통합. 전체 bounds X=-4~44, Y=-3.21~67. 중심 (20,31.895,0). 원본 PNG 해상도와 기존 하늘 배치 비율 유지.
- SortingGroup GA1_BG_Far / 14. 원래 Far 상단 Y24~27.91만 vertex alpha로 부드럽게 연결. 달과 산의 위치/크기/텍스처는 원본 유지.
- 원래 Start upper sky, BG_Start_Far_v02 Renderer만 비활성화해 보존. Portal surfaces에서 원본 두 항목 대신 통합 Renderer를 entranceRole=1, exitRole=1로 등록.
- HyunseoScene과 Map_CompleteLayout.prefab에 해당 원경 통합만 반영. 다른 배치, 비밀방 확장, 충돌, UI, 카메라는 수정하지 않음.

## 검토와 롤백

BEFORE/AFTER_0 비밀방, 1 지상, 2 상단 접합부, 3 높은 하늘. 임시 16:9 카메라 촬영 후 플레이어 원위치 복원. 실제 5회 점프/패럴랙스 검증은 아님.

Before 폴더에 작업 전 씬과 전체 맵 프리팹 저장. 이전 SortingLayers_v01 백업과 구별한다. 이후 배치 변경이 있으면 전체 파일 복원 대신 통합 오브젝트 제거, 원래 두 Renderer 재활성, Portal surfaces 원본 참조 복구만 한다.

## 미채택 이미지 생성 기록

Built-in image_gen. RejectedLayout.png 및 UnusedGeneratedCandidate_v02.png는 비교용 미채택 이미지이며 런타임에서 참조하지 않음. 첫 요청 약2048×2992, 실제 첫 결과1038×1515. 실제 출력이 요청 해상도와 다름.

첫 프롬프트:

Use case: precise-object-edit / outpainting. Make ONE tall seamless 2D game far background by expanding image1 UPWARD with clouds styled from image2. Image3 overall concept style only. CRITICAL preserve image1 mountain landscape and single moon composition, NOT a new composition. Target portrait about 2048x2992 pixels, width:height=48:70.21. Original image1 sits in bottom 44.32 percent of final canvas, spanning horizontal x8.33% to94.75%. Keep its scene at that size/position: original moon center ends at final x37.8%, y64.5%, moon diameter approx4.8% canvas width; highest mountain peaks around y67-71%; bottom remains misty foothills. Upper 55.68 percent is new dark midnight cloud sky seamlessly joining original sky, no straight horizontal join. Fill narrow extra left/right margins by continuing clouds/mountains naturally. DO NOT move or enlarge moon, do not add another moon, stars, mountains in upper sky, castle, trees, terrain, characters, text. No stretching original landscape. Match original dark navy painterly brushwork and subtle blue moonlight; extension cloudy atmospheric but subdued. Opaque full image, no blank margins. Preserve recognizable original lower landscape and avoid changing its brightness.

위치 수정 프롬프트:

Edit target image1, exact same 1038x1515 portrait framing. The lower landscape and moon are TOO HIGH. Move the ENTIRE existing lower mountain+moon composition DOWN by 176 pixels and RIGHT by 30 pixels WITHOUT SCALING or changing its textures. The single moon center must be at pixel (391,977) [37.7% canvas width,64.5% canvas height], diameter 51 pixels. It is currently around (361,801). Mountains move by same translation; high right mountain summit ends around y1025. The bottommost fog may crop beyond bottom edge, that is intended. Keep the upper 650pixels cloud sky unchanged; naturally outpaint/blend the cloud band between existing upper sky and moved lower composition, no horizontal seam or repeated moon. Fill right/left edge gaps naturally. Do not shrink mountains, do not move moon relative to mountains. Preserve colors and texture. Exactly ONE moon, no objects/foreground/text. This is a precise layout correction to match an existing Unity scene.
