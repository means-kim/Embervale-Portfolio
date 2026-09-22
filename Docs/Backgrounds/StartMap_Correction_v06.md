# 시작맵 컨셉 차이 점검과 수정 v06

2026-09-22. 사용자 요청: 차이와 해결 방법을 스스로 점검하고 계획 후 실행한다.

## 수정 계획

| 차이 | 해결 방법 | 검토 기준 |
|---|---|---|
| 숲이 좌우 대칭의 U자, 높은 밀도와 밝은 눈 | v02로 비대칭 군집·중앙 빈틈·완만한 눈 명암 수정 | 달·성·다리 노출, 절벽 하단 겹침, 나무 높이 변화 |
| 고목이 전체적으로 밧줄처럼 꼬임 | v02로 넓은 줄기 면·거친 균열·제한된 감긴 뿌리 | 오른쪽 경계 역할, 성을 가리지 않는 가지, 뿌리 투명 영역 |
| 산 능선이 원본보다 또렷함 | 숲 수정 후 재평가, 필요하면 기존 독립 안개를 숲 뒤에도 낮은 불투명도로 배치 | 산·성·숲 간 거리 분리, 달 가시성 |
| 왼쪽 고목·지면·눈 덮개 부재 | 현재 수정 소재와 미제작 요소를 구분 | 없는 요소를 이유로 기존 에셋을 과도하게 키우지 않음 |

원경 v02와 모든 이전 버전은 보존한다. 수정물마다 실제 PNG를 기존 레이어에 합성하고 원본과 비교한다. Unity 씬과 코드는 수정하지 않는다. 최종 월드 크기와 플레이 품질을 이번 정지 화면 검토로 확정하지 않는다.

## 실행 결과

1. [숲 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestMid_v02.png) 제작. 오른쪽 높은 군집이 낮아지고 중앙 틈과 높이 변화가 생겼다. [생성 직후 합성 v06](StartMap_Placement_v06.png)에서 달·성·다리 가시성과 절벽 하단 겹침을 확인했다. 밀도를 정량 측정한 것은 아니며, 왼쪽 눈 표현은 여전히 원본보다 선명하다.
2. [고목 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeRight_v02.png) 제작. 반복된 감긴 뿌리를 줄이고 큰 줄기 면과 세로 균열을 확보했다. [생성 직후 합성 v07](StartMap_Placement_v07.png)에서 성을 가리지 않는 위치와 바깥·뿌리 틈의 투명성을 확인했다. 원본 가지 방향·기울기까지 완전히 일치한 것은 아니다.
3. 기존 Mist v01을 추가 생성 없이 성 뒤·산 앞에 재사용했다. 배경 기준 너비 100%, 왼쪽 0%, 위쪽 16%, 불투명도 22%. 기존 숲 앞 안개 28%는 유지했다. 원경 PNG를 수정하지 않고 산 윤곽을 약하게 완화했다. 산의 형상 차이는 남는다.
4. [최종 비교 화면 v08](StartMap_Placement_v08.html)에 원본·수정 전 v05·수정 후를 같은 크기로 배치했다. [화면 캡처](StartMap_Placement_v08.png)를 프로젝트에 저장했다. 레이어 켜기/끄기로 각 소재의 영향을 비교할 수 있다.

## 실제 사양과 보존

| 수정 소재 | 실제 PNG | Import | Scale 1 캔버스 |
|---|---|---|---|
| ForestMid v02 | 1448×1086 RGBA | PPU 100 | 14.48×10.86유닛 |
| TreeRight v02 | 1024×1536 RGBA | PPU 100 | 10.24×15.36유닛 |

Single, Full Rect, 중앙 Pivot, Trilinear, Mip Maps On, 압축 None, Max Size 2048, Clamp, Alpha Is Transparency On, 자동 Physics Shape Off. 각 신규 PNG에 고유 GUID의 .meta를 보존했다. v01과 이전 합성 자료는 덮어쓰지 않았다. 생성 도구는 내장 image_gen이며 확대 처리 없음.

숲에는 침엽수와 정적 눈 표현만 포함하며 지면·독립 안개·배경을 제외한다. 고목은 몸통·부착 가지·뿌리이며 눈·지면·발판을 제외한다. 숲은 성 앞, 고목은 숲과 안개 앞에 연결한다. 배치 비율은 v05를 유지해 형태 수정의 효과를 비교했다. 실수로 다른 소재를 변경하지 않도록 Unity 씬에는 적용하지 않았다.

## 남은 차이와 진행 방향

- 원본의 왼쪽 큰 나무와 가지가 아직 없어 화면 왼쪽이 열려 있다. 다음 근경 소재에서 별도 제작해 화면 경계를 연결할 수 있다.
- 지면·별도 눈 덮개가 없어 고목 뿌리가 떠 보인다. 지형과 접지 검토는 아직 미완료다.
- 숲 왼쪽 눈 표현, 성 건축 형태, 산 능선 및 고목 가지 방향은 원본과 차이가 남는다. 완전히 일치하거나 최종 승인된 것으로 기록하지 않는다.
- 더 넓은 레이어 조합이 준비되면 같은 비교 방식으로 조정한다. 구조적 문제는 에이전트가 먼저 분석·수정하고, 사용자에게 매번 선택을 떠넘기지 않는다.
- 실제 카메라·월드 크기·캐릭터 대비 Scale·이웃 접합·플레이 화면 검증은 미완료다.

## 숲 v02 실제 프롬프트

```text
Use case: precise-object-edit.
Image 1 EDIT TARGET: transparent middle-distance forest v01. Image 2 REFERENCE: original Bg_Concept, specifically the left starting forest below the distant castle. Image 3 palette reference: approved dark far background.
Correct ONLY the forest layer, retaining its full-area 4:3 layout and genuine alpha. The current image forms a symmetrical U-shaped fence of dense snow-bright trees. Make it an irregular asymmetric distant woodland like reference 2: keep the tallest cluster toward the LEFT quarter, reduce the height and size of the far-right cluster by about 25%, break uniform rows, remove about one quarter of the most crowded trees, leave a few narrow irregular transparent gaps between tree groups, especially around x=35-55% in the lower half. Do NOT create a central straight road or a neatly framed arch. Keep some smaller conifers beneath the castle region so they can hide the supporting cliff's bottom. Preserve the upper sky opening, left/right overall footprint and soft transparent lower edges. Entire upper 20% remains empty transparent. Central tree tips roughly y=50-58%; largest left tips y=25-32%, right tips y=35-43%.
Reduce the white snow highlights substantially; make the trees subdued slate blue-gray, less crisp and less black than foreground bark. Retain delicate hand-painted needles and soft moonlight, no gaussian blur wash. It must read as MIDDLE DISTANCE, not a foreground Christmas forest. Actual alpha between branches and outside trees. No opaque fog/background, no separate mountains or sky, no castle, ground, giant tree, path, rocks, characters, text or watermark. One transparent PNG, preserve 1448x1086 if supported. No stretch or crop.
```

## 고목 v02 실제 프롬프트

```text
Use case: precise-object-edit.
Image 1 EDIT TARGET: transparent right giant tree trunk v01. Image 2 authoritative Bg_Concept REFERENCE: the large trunk dividing left forest from climbing route, at approximately x24-31% of full concept.
Correct the tree's anatomy and surface to match the reference more closely. Current target is a bundle of endlessly braided ropes. Replace roughly two thirds of the looping vine-like coils with a single substantial continuous ancient trunk: broad irregular bark plates, deep long vertical fissures, asymmetric knots, only a few large exposed roots wrapping partway around the trunk. Keep the near-black blue-gray moonlit wood, delicate brushed texture and readable bark. No repeated spiral corkscrew or symmetrical woven pattern. Trunk leans subtly LEFT toward the upper edge, anchored by a broad spreading root base. Preserve approximate canvas footprint and root base position, portrait 2:3, top extending beyond image edge, sparse attached crooked branches with useful empty space on the left to keep the castle visible. Do not create a canopy. Preserve authentic transparent alpha throughout all background and root/branch holes. Keep roots free of surrounding terrain and root tips mostly within frame.
BARE WOOD ONLY: no snow, frost, ice, icicles, rock, soil, ground mound, ledge, steps, platform, buildings, fog, sky, other tree, characters or text. Snow and terrain are separate future layers. Output one transparent PNG, 1024x1536 if supported, no borders, no painted checkerboard or opaque backdrop.
```
