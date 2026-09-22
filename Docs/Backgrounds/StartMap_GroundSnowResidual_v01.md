# 잔설 아트 v01과 상태 비교 v16

2026-09-22. 사용자의 눈 흔적 연출 확인 이후 다음 진행 요청에 따라 잔설 이미지와 검토 자료를 제작했다. 게임 기능 구현 범위는 추가하지 않았다.

[잔설 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundSnowResidual_v01.png), [검토 v16](StartMap_Placement_v16.html), [검토 이미지](StartMap_Placement_v16.png).

요청/실제1774×887 RGBA. 모서리alpha0, 기존 GroundSnow와 동일PPU100·중앙pivot·FullRect·최대2048, 고유.meta. 긴 캔버스 안에 분리된 잔설 덩어리들이 있는 단일 스프라이트다. 바위와 분리되어 있고 기존 지면 좌표에 겹친다. 개별 발자국 모양이나 발 간격을 확정한 소재는 아니다.

첫 생성물은 연속된 눈 띠가 남아 차이가 약했으므로 사용하지 않았다. [첫 검토 원본](ReviewSources/GroundSnowResidual_Rejected01.png)을 보존했다. 두 번째 생성에서 빈 구간과 분리된 잔설을 확보했고 지면 위 확대 합성을 확인했다. 일부 눈 덩어리는 눌린 자국보다 작은 눈 뭉치로 읽히며, 축소 화면에서 변화는 미묘하다. 최종 카메라에서 가독성 조정이 필요하다.

HTML에서 밟기 전 / 잔설 남김 / 눈 제거를 선택할 수 있다. 화면20%부터 지정 끝 위치까지 CSS 표시 마스크로 기존 눈을 가리고, 잔설 모드일 때만 해당 구간의 새 소재를 표시한다. 바위는 유지한다. 세 상태의 표시 전환을 확인했다. 마스크는 검토 HTML에만 있으며 Unity 셰이더·접촉 감지·누적 흔적·발 간격·상태 저장·충돌은 구현하지 않았다. 흔적 끝 위치 숫자는 아트 비교용이며 캐릭터가 실제 이동하거나 발을 디뎠다는 검증이 아니다. 기존 컨셉 비율과 밝기80% 검토값을 유지한다. PNG 자체 밝기는 수정하지 않았다.

## 실제 프롬프트

내장 imagegen 사용. 생성 결과를 프로젝트에 복사했다.

### 첫 시도 (미채택)
```text
Use case: precise-object-edit. Image1 is the untouched snow-only overlay, Image2 its aligned rock ground, Image3 master palette. Produce a TRODDEN SNOW ONLY variant of image1 on exactly the SAME 1774x887 transparent canvas with no reframing. Preserve registration along y500..540 and rock-height variations from image2. Remove 65-75 percent of the pristine snow coverage to genuine transparency so underlying rock can show through. Remaining snow is thin flattened cool gray-blue slushy patches pressed into crevices, with irregular small ridges displaced to the sides of shallow side-view foot depressions. Depressions are irregularly spaced every roughly 35-60 pixels horizontally; do not draw top-down boot silhouettes or obvious repeated stamps. No tall fluffy unbroken white ridge: this must read distinctly lower, sparse and darker than pristine snow. Limit all visible material to the narrow existing surface band; no floating particles. No rock, landscape, character, text, checkerboard or colored background. Output ONLY residual compacted snow, not a snowy ground image. Genuine alpha transparency across missing snow and all surrounding canvas. Full 1774x887.
```

### 수정 시도 (채택 후보)
```text
Use case: precise-object-edit. Input is pristine snow overlay on 1774x887 canvas. Make a sparse residual-snow alternative at same coordinates. ESSENTIAL: do NOT output a continuous strip. There should be only about 18-24 SMALL DISCONNECTED isolated patches along y510..535 with broad true alpha0 empty gaps BETWEEN every patch. Each patch is only roughly 12-30px wide and 3-8px thick. Remove at least 80 percent of original snow area entirely to transparency. Flattened dull blue-gray compressed snow, not white fluffy mounds. Patches sit at the BOTTOM edge of the original snow band, retaining exact registration. Full 1774x887 canvas; all remaining 95 percent of canvas transparent, no recentering/cropping. This is the sparse leftover snow after footsteps swept most snow aside, side view. NO continuous horizontal line, NO ground or stones, NO shadows filling gaps, NO particles, NO text, NO checkerboard. It must visibly contain isolated scraps of snow separated by clear space rather than resemble the input long snow ribbon.
```

