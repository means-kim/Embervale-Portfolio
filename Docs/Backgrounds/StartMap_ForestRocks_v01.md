# 시작 숲 낮은 바위 군집 v01

2026-09-23. 숲 이동로 뒤에 놓을 장식 바위 한 소재.

[PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestRocks_v01.png) · [최신 합성 v24](StartMap_Placement_v24.html) · [합성 캡처](StartMap_Placement_v24.png) · [접합 확대](StartMap_ForestRocks_Detail_v01.html) · [확대 캡처](StartMap_ForestRocks_Detail_v01.png)

## 용도와 사양

- 작은 각진 바위와 돌 조각의 낮은 군집. 눈·지면·풀·충돌·발판을 포함하지 않는다. 개별 돌마다 분리된 파일이 아니라 군집 단일 스프라이트다.
- 요청1536×768, 실제1774×887 RGBA. alpha32 이상 세로 범위512..780. 좌상단·우하단alpha0 확인. 확대 가공 없음.
- PPU100, 중앙pivot, FullRect, Max2048, Trilinear, Mipmap On, 무압축, 자동 Physics Shape Off. 고유 GUID·spriteID의 .meta 동반.
- Scale1 캔버스17.74×8.87유닛. 최종 Transform Scale은 미정.
- 연결: 왼쪽 고목 뿌리 이후의 숲 이동로, 관목·그루터기 근처. 기존 CaveRubble_v01은 동굴 연결용으로 유지하며 새 소재는 별도로 이동 가능.

## 비교 및 검토

컨셉 시작 구역의 흩어진 낮은 돌을 참고했다. 기존 동굴 바위는 오른쪽으로 높아지는 모양이므로 그대로 복제하지 않고 낮은 중앙 바위와 양옆 작은 돌로 변화를 주었다. 프롬프트보다 작은 돌의 개수가 많게 나왔으며 최종 결과는 위 PNG 기준이다.

HTML width19%, left19%, top78.7%, brightness(.7). PNG 원래 밝기는 유지. 바위는 지면과 플레이어 뒤에 두어 밑부분이 지면에 묻히도록 했다. alpha 기준 보이는 원본 높이269px를 환산하면 화면 높이 약3.84%, 캐릭터5.42%의 약0.71배다. 지면에 가려지는 부분 때문에 합성의 실제 노출 높이는 이보다 약간 작다.

정지 전체 합성과1150px 폭 확대에서 바닥 접합·관목 겹침·캐릭터 앞뒤를 육안 확인했다. 표시 토글 및 이미지 로드 확인. 컨셉보다 돌 배치가 성기고 바위에 적설이 없어 설경 전체 일치가 아니다. 눈 덮개는 필요 시 별도 제작한다. 장식이 장애물처럼 읽히는지, 최종 카메라에서 가독성이 적절한지는 실플레이 검토가 남았다.

Unity 씬·코드·충돌은 변경하지 않았다. 다음 구역으로 확장하지 않았다. 캡처는 화면 범위이며 확대 자료는 실제 카메라 범위가 아니다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: Bg_Concept(원본), CaveRubble_v01(기존 바위 재질). 생성 PNG를 프로젝트로 복사하고 원본을 보존했다.

```text
Use case: stylized-concept. Create ONE low loose cluster of 4-6 weathered angular forest stones, isolated sprite for side-view 2D platformer, transparent PNG requested 1536x768. Image1 master concept: match scattered low rocks on the left forest ground around the player's approach, Image2 established stone texture only. Unlike image2's tall rising cliff rubble, keep a VERY LOW broken irregular horizontal silhouette with one squat broad stone left of center and separated smaller chips towards either side. No pyramid or wall. Entire stones fit x140..1390,y380..650, bottom contact around y650, generous transparent padding. Large stone much wider than tall. Clear small pockets of transparent space between stones. Deep charcoal slate blue with muted cool gray-blue upper-left moonlight, hand-painted layered chipped facets, subtle weathered edges, low contrast. Side elevation, no top-down platform or ground slab. ONLY rocks, no snow or ice (separate material later), no roots/plants/soil/background/fog/shadow plane/characters/architecture/text. True alpha transparency. Distinct from cave rubble through low scattered shape, but same art style. Decorative background-to-walking-path object, not a climbable obstacle.
```
