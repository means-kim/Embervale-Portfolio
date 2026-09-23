# 보스 바닥 v02

사용자 요청에 따라 기존 BG_Boss1_Ground_v01을 새 파괴 바닥 v03/오른쪽 암벽 v02와 같은 질감으로 수정했다. 내장 imagegen 편집 사용.

- 새 이미지: Assets/04.Sprites/Backgrounds/BossMap1/BG_Boss1_Ground_v02.png, 2172×724 RGBA. 기존 PNG 보존.
- 기존 Ground_v01 머티리얼의 mainTexture만 교체하여 이를 공유하는 씬/프리팹에 적용. 머티리얼 교체 전 내용은 이 폴더의 .before.txt로 보관.
- 현재 씬의 사용자 미저장 편집을 보존하기 위해 씬 저장하지 않음. 메시, Transform, Collider, 별도 눈 덮개 변경 없음.
- 중앙 파괴 바닥 및 오른쪽 암벽 접합을 전후 1280×720 렌더로 비교. 바위 표면 디테일과 명암 통일. 플레이 완주 검증은 아님.

## 실제 생성 프롬프트

Edit image1, the long horizontal rock ground sprite. Image2 revised breakable floor and image3 revised cliff are STYLE MATCH REFERENCES; image4 is overall game concept. Output ONLY a replacement horizontal terrain sprite for image1, same 3:1 canvas ratio, ideally2172x724 RGBA. Preserve EXACT silhouette footprint and normalized placement: flat walkable top around27% canvas height, irregular lower edge around72%-84%, rock spans full left and right canvas edges with no side caps, transparent above/below. Keep same contour so existing terrain mesh and separate snow layer stay aligned. Replace old blurry repetitive huge shield-shaped vertical slabs with the crisp richly textured irregular basalt of image2/image3: interlocking medium irregular vertical rocks, varied small filler stones and natural fissures, subdued dark cool blue-gray with soft moonlight on top-facing planes. Match contrast, material roughness and sharpness of image2; no excessively black featureless faces, no hard bright outlines, no repeated identical stones. As an extremely wide strip use enough rock clusters across it to match their apparent size in image2 when this is tiled/stretched horizontally. No new scenery, no characters, no text, no icicles, NO snow blanket (snow is a separate asset); only sparse frost-like rock highlights already on image1. Genuine transparent background, no opaque black matte or glow. Preserve the top edge alignment and bottom outline much more closely than the internal texture.
