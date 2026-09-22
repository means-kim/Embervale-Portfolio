# 시작맵 레이어 제작·합성 검토 v05

2026-09-22. 사용자 지시: 소재 하나마다 컨셉·기존 소재와 대조하며 자율적으로 이어서 작업하고, 사용자 판단이 필요한 지점에서 알린다. 이번에는 중경 숲 → 성 명도 수정 → 독립 안개 → 오른쪽 고목 순서로 각각 제작 직후 합성 검토했다. 내장 image_gen을 사용했다.

## 결과와 실제 사양

| 소재 | 실제 PNG | Import PPU | Scale 1 캔버스 크기 | 상태 |
|---|---|---|---|---|
| [중경 숲 v01](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ForestMid_v01.png) | 1448×1086 RGBA | 100 | 14.48×10.86 | 내부 합성 검토, 사용자 개별 확인 전 |
| [성 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v02.png) | 1448×1086 RGBA | 100 | 14.48×10.86 | 눈·절벽 하이라이트 낮춤, v01 보존 |
| [안개 v01](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Mist_v01.png) | 2046×768 RGBA | 100 | 20.46×7.68 | 독립 레이어, 검토 배치 opacity 0.28 |
| [오른쪽 고목 v01](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeRight_v01.png) | 1024×1536 RGBA | 100 | 10.24×15.36 | 몸통·부착 뿌리·가지, 눈·지형 제외 |

공통 Import: Single, Full Rect, 중앙 Pivot, Trilinear, Mip Maps On, 압축 None, Max Size 2048, Clamp, Alpha Is Transparency On, 자동 Physics Shape Off. PNG 원본 확대 없음. 생성 요청의 비율과 실제 출력은 구분한다. 안개는 4:1을 요청했으나 실제 약 2.66:1이다. RGBA와 투명 표본을 확인했다. 안개 중앙 alpha는 242이므로 원본 자체가 전체적으로 엷은 것은 아니며, 검토 화면에서 28%로 낮춰 사용했다.

## 소재별 포함·제외와 확인

- 숲: 침엽수와 나무 위의 정적인 눈 표현 포함. 독립 안개·지면·하늘·성은 제외. 높이가 다른 나무들이 한 소재에 들어 있어 개별 나무별 이동은 불가하다. 원본보다 좌우 대칭과 밀도가 강하고, 가까운 나무의 눈이 선명하다.
- 성 v02: 구조와 배치 후보를 유지하면서 밝은 눈 윤곽·암벽 대비를 낮췄다. 시각상 원경과 조화가 개선됐지만 원본 건축과 완전히 동일하지 않다. 하단의 기존 옅은 안개 표현도 남아 있으며 독립적으로 움직이지 않는다.
- 안개: 나무 줄기 높이를 통과하는 별도 레이어다. 숲에 구워 넣지 않았다. 0.28 불투명도에서 성과 나무 가시성을 유지함을 화면에서 확인했다.
- 고목: 원본 오른쪽 거대 고목을 참조한 나무 몸통·부착 뿌리·가지. 뿌리 사이와 바깥 배경의 alpha 0 표본 및 합성에서 투명 배경을 확인했다. 눈·얼음·땅·발판을 포함하지 않는다. 원본보다 곧게 서고 가지 방향이 달라 그대로 재현한 것은 아니다.

## 생성 직후 검토 기록

1. [숲 합성 v02](StartMap_Placement_v02.png): 절벽 하단이 일부 가려져 거리감이 생겼으나 성 윤곽이 밝아 수정 결정.
2. [성 수정 합성 v03](StartMap_Placement_v03.png): 밝은 성·암벽 윤곽 감소 확인.
3. [안개 합성 v04](StartMap_Placement_v04.png): 나무 줄기 주변을 부드럽게 연결하고 달·성·다리 가시성 확인.
4. [최신 고목 합성 v05](StartMap_Placement_v05.png): 처음보다 고목을 키워 컨셉의 오른쪽 비중에 가깝게 배치. 성과 달을 가리지 않음. [레이어를 켜고 끌 수 있는 화면](StartMap_Placement_v05.html).

v02~v04 화면 하단의 문구 일부는 v01 검토 템플릿에서 이어진 과거 메모다. 최신 판단과 상태는 이 문서 및 v05를 따른다. 합성은 기존 PNG를 HTML 레이어로 표시한 것이며, 생성 도구로 합성 결과를 다시 그리지 않았다.

## 최신 검토 배치

배경 캔버스 1448×1086 기준. 비율은 파일의 투명 여백을 포함한다.

| 앞뒤 순서 | 너비/배경 너비 | 왼쪽 | 위쪽 | 표시 불투명도 |
|---|---|---|---|---|
| 원경 v02 | 100% | 0% | 0% | 100% |
| 성 v02 | 42% | 24.1% | 11.9% | 100% |
| 중경 숲 | 100% | 0% | -9% | 100% |
| 안개 | 100% | 0% | 42% | 28% |
| 오른쪽 고목 | 60% | 33% | -18% | 100% |

모든 이미지는 원래 종횡비를 유지한다. 이 배치는 아트 검토 후보이며 최종 Unity Transform이 아니다. 원경과 개별 소재의 파일 크기가 다르므로 너비 비율을 Unity Scale로 그대로 복사하면 안 된다.

## 사용자에게 확인할 지점과 다음 작업

최신 합성에서 오른쪽 고목의 굵기·화면 점유와 중경 숲의 밀도를 확인받는다. 이 판단에 따라 앞으로 만들 지형 및 별도 눈 덮개와의 겹침이 달라진다. 자율 진행 지시를 모든 생성물의 아트 승인으로 간주하지 않는다.

지면·발판·고목 눈 덮개는 아직 없으므로 뿌리가 떠 보이는 것은 현재 합성의 한계다. 실제 카메라·맵 크기·캐릭터 대비 Scale, 구역 접합, 플레이 품질은 미검증이다. Unity 씬, 캐릭터, 게임 코드, 전역 설정은 변경하지 않았다.

## 중경 숲 v01 실제 프롬프트

```text
Use case: background-extraction.
Asset type: ONE transparent middle-distance conifer forest layer for the START region of a hand-painted 2D snowy metroidvania.
Reference 1 Bg_Concept: authoritative texture, tree shapes and layout of its LEFT starting forest. Reference 2: approved dark far background; match its cool blue nighttime palette. Reference 3: current placement comparison; right panel gives existing moon and small castle placement ONLY. Do not copy UI or produce a comparison sheet.
Generate just the middle-distance fir and spruce woods, isolated on genuine transparent alpha. Landscape 4:3 canvas. Think of the output as a full-area overlay: top 28% completely transparent. Tree groups start near y=30% on the left and y=38% on the right, descending toward a lower valley of smaller treetops around x=35-55%, y=51%. Leave the sky and distant castle visible above this central valley; the lower forest will overlap the bottom of the castle's cliff around y=50-60%. Dense irregular conifers fill much of the lower half, trunks and roots fade gently toward transparent below y=90%. Preserve transparent gaps between needles and upper branches. Hand-painted dark slate-blue needles, muted light blue snow accents, softer silhouettes and restrained contrast; middle distance, not sharply lit foreground Christmas trees. Small alternating tree heights and organic clusters, not a repeated flat fence. One depth band only: no separate far mountains or fog bands baked in. Faint painted atmospheric softness on the trees is okay, but no independent fog object.
No sky, moon, clouds, castle, buildings, bridge, rock ridge, terrain, walkable ground, giant foreground tree, characters, sanctuary, text, borders, watermark, or checkerboard pixels. Snow on these distant noninteractive trees is static surface detail; no thick ground snow. Output one RGBA PNG with actual transparency, no opaque rectangular backdrop.
```

## 성 명도 수정 v02 실제 프롬프트

```text
Use case: lighting-weather.
Input 1 is EDIT TARGET: transparent castle, bridge, supporting bluff. Input 2 Bg_Concept is authoritative distant nighttime palette reference (small castle at upper left). Make ONE targeted correction: suppress bright silver-white snow highlights and hard rock contrast on input 1 so it reads as a quiet DISTANT landmark in blue night haze, not a bright foreground structure. Reduce the light snow and illuminated edges by approximately 35%, lift the deepest local crevices slightly into cool slate blue to compress local contrast, retain readable shapes and muted detail. Do not uniformly make the whole silhouette pitch black. Maintain blue-gray hues. Keep EXACT same castle architecture, tower count, silhouette, bridge arches, supporting cliff contour, composition, size, canvas, placement, transparent bridge holes and genuine RGBA transparency. Preserve the lower alpha fade without adding a fog rectangle or new objects. No background, no sky, no trees, no new buildings, no text. Output one edited transparent PNG, same 4:3 canvas and 1448x1086 if supported.
```

## 독립 안개 v01 실제 프롬프트

```text
Use case: background-extraction.
Asset type: independent translucent mist ribbon sprite for a 2D hand-painted moonlit snow forest.
Reference 1 Bg_Concept: copy only the quiet blue-gray haze between its left starting-area conifers. Reference 2 approved dark far background: palette reference only.
Generate ONE wide, low, sparse veil of soft slate-blue silver mist on GENUINE transparent RGBA background. Landscape 4:1 aspect ratio preferred. Delicate irregular horizontal strands concentrated in the middle 40% of the canvas height, fading fully to alpha zero at all four edges with generous padding. Semi-transparent throughout, very low density, no opaque center. Smooth airy brushwork, slightly uneven breaks and wisps; calm low forest haze, not a thick storm cloud, smoke explosion, luminous magic or ground. Muted desaturated blue gray, not bright white. Suitable for placing across tree trunks to soften depth without hiding branch silhouettes. Absolutely no trees, terrain, ground, mountains, buildings, moon, sky, snowflakes, sparkles, text, frame, checkerboard pixels or flat background rectangle. Preserve real partial alpha. One isolated mist sprite only.
```

## 오른쪽 고목 v01 실제 프롬프트

```text
Use case: background-extraction.
Asset type: ONE isolated near-ground giant ancient tree (trunk, attached roots and attached branches) for the RIGHT edge of starting area in a 2D hand-painted metroidvania.
Reference 1 Bg_Concept is authoritative: reconstruct the huge deeply gnarled trunk at x=24-31% of the full original image, separating it from mountains, terrain and snow. Reference 2 is the accepted dark blue moonlit palette.
Portrait 2:3 composition, genuine transparent RGBA backdrop. Thick tall twisting weathered trunk runs through approximately the middle-right of canvas, rising beyond the upper edge so that its crown is out of view. Massive braided roots spread near the bottom, crooked attached branches reach modestly left and right with naturally tapering tips. Left side remains mostly transparent to reveal the distant castle behind it. Organic asymmetrical bark with deep vertical fissures, twisted rope-like grain and knot holes exactly in the source's hand-painted style. Very dark blue-gray bark, subtle silver-blue moonlit highlights on upper-left edges; keep details readable without bright high-contrast white stripes. No cartoon outlines, no photorealistic tree photograph.
IMPORTANT separation: BARE WOOD ONLY. No snow caps, no ice, no icicles, no moss, no ground rocks, no soil mound, no ledges, no walkable platforms, no ladder, no attached architecture. The snow on this near tree must be a separate later material, so absolutely do not paint white snow onto branches or roots. The roots themselves are allowed but their surrounding ground must be transparent. No sky, forest, other trees, moon, fog, castle, characters, sanctuary, text, watermark, painted checkerboard. Do not include a whole isolated leafy tree with a crown. Produce one tall root/trunk framing piece to overlay the right edge of the scene. Keep the root ends inside the canvas and clear padding on left/right/bottom; only upper trunk may meet the top edge.
```

