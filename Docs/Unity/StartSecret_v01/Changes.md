# 시작 구역 왼쪽 비밀방 — 배경과 전경 준비

사용자 요구: 시작 왼쪽 작은 비밀방, 같은 배경으로 연결, 나무 뒤로 통과, 추가 중경/근경 장식 없음, 상호작용 오브젝트 하나, 진입 후 고정 화면.

## 적용

- HyunseoScene의 01_Start/00_SecretLeft 아래 확장 원경과 배치 기준점 추가. 기존 바닥/콜라이더/이동/카메라 코드는 수정하지 않음.
- BG_Start_SecretFarLeft_v01.png: 1448×1086 RGB, 달 없는 왼쪽 설산 확장. 실제 메시 표시 41.4933×31.12 월드 단위, X 약 -38.993~2.5, 오른쪽 6% 버텍스 알파 블렌드. 원본 이미지 종횡비 유지.
- BG_Start_TreeLeftFull_v02.png: 1024×1536 RGBA, 잘린 왼쪽 몸통과 뿌리 보완. 표시 24×36, 중심 (2.4,14.8,0). SortingGroup Default/110으로 현재 플레이어 Default/100 앞에 표시. 충돌 없음.
- 원래 BG_Start_TreeLeft_v01 Renderer는 삭제하지 않고 비활성화. 새 나무는 같은 Piece 부모에 배치.
- 두 Renderer를 MapPortalVisibility surfaces에 entranceRole=1, exitRole=1로 등록.
- 배경 프리팹: Assets/03.Prefabs/Map/Background_Start_SecretLeft_v01.prefab. 나무 프리팹: Decoration_Start_TreeLeftFull_v02.prefab. 전체 맵 스냅샷 프리팹은 이번에 갱신하지 않음.
- PNG: Assets/04.Sprites/Backgrounds/StartMap. 메시/Material은 공유 Map 폴더에 각각 저장. MapReview 로컬 폴더 의존 없음. Assets/04.Sprites/Boss 미변경.

## 구도와 미구현 범위

- SecretRoom_FixedView_Size12_16x9: (-17,9,-10), 검토 Orthographic Size 12, 16:9. 빈 Transform이며 실제 카메라 전환 기능이 아님.
- InteractionObject_PlacementOnly: (-20,0,0). 빈 배치 기준점이며 상호작용 오브젝트 아트/기능 없음.
- 왼쪽 방의 바닥, 충돌, 좌측 폐쇄 경계와 실제 고정 카메라 진입/복귀 로직은 아직 없음. 바로 걸어서 진입할 수 있는 완성 방이라고 취급하지 않음.
- Unity 캡처 View_-17.png, View_0.png, View_13.png 확인. 임시로 플레이어를 X=-17/0/13, Y=1에 놓고 캡처 후 원위치 복원. X=0에서 나무가 플레이어를 가림. 전체 경로 플레이 검증 아님.
- 현재 고정 구도에 배경이 채워짐. 비밀방 외부로 수직 카메라 이동하거나 카메라를 더 크게 하면 별도 범위 검토 필요.

## 제작 기록

Built-in image_gen 사용. 원본 컨셉과 기존 원경/나무를 실제 이미지로 확인하고 참조. 새 PNG는 별도 버전으로 보존.

### 배경 프롬프트

Use case: precise-object-edit. Create a LEFT extension tile for the supplied mountain night background (image1), style reference concept image2. Output 4:3 opaque background, same cold dark navy painterly snow mountains and cloudy night sky. This new tile will sit LEFT of image1: the RIGHT edge should meet image1 LEFT edge with matching mountain elevations, cloud colors and low mist. No moon (existing moon stays only in original), no castle, no trees, no foreground or ground, no objects. Upper 30 percent cloud sky, distant mountain ridge around 30-45 percent height, progressively darker foggy foothills below, subdued bottom. Maintain original scale and contrast, no stretching. Quiet secret room backdrop.

### 나무 프롬프트

Use case: precise-object-edit. Edit image1 cropped ancient foreground tree into a complete free-standing trunk with both left and right silhouettes visible. Image2 overall style reference. Transparent background genuinely alpha. Keep dark blue charcoal twisted bark, upper right sweeping branch, flared roots at bottom and massive tall trunk. Add missing left half of trunk and modest left roots, no background, no fog, no ground slab, no extra trees, no snow baked in. Trunk slightly left of center, roots spread lower width, upper branches continue out top allowed; all side trunk/root silhouettes contained with transparent margin. Game 2D side-view foreground occluder, character can walk behind trunk. Portrait 2:3 composition. Match original bark detail and subdued moonlight, no bright outlines.
