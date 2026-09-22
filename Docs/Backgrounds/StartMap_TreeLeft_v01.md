# 시작맵 왼쪽 고목 v01

2026-09-22. 룸/아트 제작 합의 기록 후 장식 소재 제작을 계속했다.

- [PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeLeft_v01.png): 1024×1536 RGBA, PPU 100. Scale 1 캔버스 10.24×15.36유닛, 투명 여백 포함. 확대 없음.
- Import: Sprite Single, Full Rect, 중앙 Pivot, Trilinear, Mip Maps On, 압축 None, Max Size 2048, Clamp, Alpha Is Transparency On, 자동 Physics Shape Off. 고유 GUID .meta 보존.
- 포함: 왼쪽 고목 몸통·부착 상단 가지·뿌리. 제외: 눈·얼음·지면·발판·안개·다른 나무·배경·캐릭터·성소.
- 내장 image_gen 사용. Bg_Concept의 왼쪽 끝 나무를 형태 기준으로, 오른쪽 고목 v02를 질감·팔레트 참조로 전달했다.
- 사용 목적: 시작 구역 왼쪽과 상단을 연결하는 장식. 실제 BG_Near/FG 역할은 캐릭터 가림을 보고 결정한다. 충돌 지형이 아니다.

## 합성 확인

[컨셉 비교 화면 v09](StartMap_Placement_v09.html), [캡처](StartMap_Placement_v09.png).

배경 1448×1086 기준 이미지 너비 55%, 왼쪽 -7%, 위쪽 -8%, 종횡비 유지. 투명 캔버스를 포함한 배치 비율이며 최종 Unity Scale이 아니다. 숲·안개·성·오른쪽 고목을 함께 놓고 달·성·다리를 가리지 않음을 확인했다. 중앙 투명 표본 alpha 0 및 브라우저 합성에서 배경 투과를 확인했다.

원본보다 나무껍질의 밝은 균열과 옹이가 두드러지고 가지 방향도 완전히 동일하지 않다. 현재 양쪽 나무에는 눈이 없고 지면도 없어 뿌리 접지가 미완료다. 최종 가림·플레이 크기·카메라 품질은 미검증. 사용자 개별 승인 전이다. 지형·눈 덮개는 [룸/아트 합의](../Room_Art_Workflow.md)에 따라 실제 플레이 범위를 확인한 뒤 맞춘다.

## 실제 프롬프트

```text
Use case: background-extraction. Asset: one transparent LEFT EDGE ancient-tree framing sprite for a 2D hand-painted snowy metroidvania.
Image 1 authoritative reference Bg_Concept: use ONLY the dark tree hugging the far LEFT border and its branches sweeping inward across the upper-left corner of the starting forest. Image 2 right-tree v02: match broad bark plates, deep vertical fissures, restrained roots and dark slate-blue palette, but do not mirror its shape.
Portrait 2:3 canvas. Leftmost 15-25% contains a tall irregular trunk leaning gently outward left, partial trunk allowed to run off left/top edges. One main crooked attached branch extends RIGHT across the upper 10-20% toward x=85%, with sparse fine twigs below. The central/right lower 70% of canvas must stay almost entirely TRANSPARENT to keep moon, castle and gameplay view open. Roots at bottom-left curve slightly right but occupy at most the lower-left third. This is an asymmetric corner frame, NOT a freestanding centered tree and NOT an arch around the entire scene. Very dark blue-gray wood with subtle cool rim light, broad irregular aged bark not braided cables. Match the original concept's hand-painted texture. Bare wood only, sparse small attached twigs, no leafy crown.
Real transparent RGBA background including branch gaps. No sky, moon, mountains, other tree, forest, fog, ground, soil, rock, walkable ledge, snow, ice, buildings, sanctuary, character, text or border. Near-tree snow and terrain must remain separate future assets. One PNG 1024x1536 if supported; root tips and right branch tip have clear padding. Preserve useful empty center.
```
