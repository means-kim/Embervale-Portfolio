# 차지 점프 불꽃 화살표 시안 4종

생성일: 2026-10-10 (Asia/Seoul)

사용자 요청: 기존 조준 화살표를 불 느낌의 이미지로 교체하기 위해 시안 4개를 생성하고 사용자가 선택한다. 현재 파일은 검토 후보이며 Assets 임포트·스프라이트 설정·프리팹 변경은 사용자가 수행한다.

생성 방식: built-in image_gen 도구, 시안별 독립 호출 4회, transparent_background=true. CLI/API 대체 경로를 사용하지 않았다. 실제 참조 이미지 [프로젝트 컨셉](../../../Assets/04.Sprites/UI/Bg_Concept.png)과 [얼음 투사체](../../../Assets/04.Sprites/Shooter/IceBall.jpg)를 열어 화풍·질감·명암을 확인했다. 참조는 눈으로 확인하고 프롬프트에 묘사했으며 원본 이미지를 편집하지 않았다.

## 파일과 실제 사양

| 번호 | 후보 | 실제 PNG 해상도 | 시각적 차이 |
| --- | --- | --- | --- |
| 1 | [ChargeArrow_01_FlameSpear_v01.png](Candidates/ChargeArrow_01_FlameSpear_v01.png) | 2172 × 724 | 날렵한 불꽃 창: 가는 중심선과 간결한 날개 |
| 2 | [ChargeArrow_02_SpiritChevron_v01.png](Candidates/ChargeArrow_02_SpiritChevron_v01.png) | 1774 × 887 | 정령 불꽃: 부드러운 리본과 말리는 꼬리 |
| 3 | [ChargeArrow_03_EmberRune_v01.png](Candidates/ChargeArrow_03_EmberRune_v01.png) | 1983 × 793 | 불씨 창: 결정형 날과 어두운 균열 |
| 4 | [ChargeArrow_04_FlameComet_v01.png](Candidates/ChargeArrow_04_FlameComet_v01.png) | 1983 × 793 | 불꽃 혜성: 두꺼운 불꽃과 넓은 유기적 날개 |

네 파일 모두 PNG RGBA(32bpp ARGB), 알파 채널이 있으며 좌상단 픽셀 Alpha=0을 확인했다. 네 변의 모든 경계 픽셀도 검사했고 파일마다 최대 Alpha=1로 사실상 투명한 여백이 유지된다. 원본 생성 파일을 그대로 복사했으며 크기·알파·색을 후처리하지 않았다. 요청 픽셀 해상도를 도구 인자로 고정하지 않았으며 위 수치는 실제 출력 파일 기준이다. Unity PPU·Transform Scale은 아직 설정하지 않았다.

## 검토 상태

- 네 시안을 실제 이미지로 검토: 하나의 수평 오른쪽 화살표, 따뜻한 불꽃·흰 중심부, 뾰족한 오른쪽 날, 투명 바탕.
- 기존 얼음 투사체와 대비되는 따뜻한 오렌지/금색 계열, 회화적 세부 질감 유지.
- Unity 화면 크기에서의 판독성·조준 회전·팝 애니메이션·실제 배경 합성은 미검증이며 사용자 선택 후 에디터에서 확인한다.
- 사용자가 4개 중 선택하기 전 자동 채택하지 않는다. 원본 PNG의 투명 여백을 포함한 해상도와 실제 화살표 폭은 다르므로 적용 스크립트의 가시 폭 설정으로 조정한다.

## 실제 생성 프롬프트

### 1. FlameSpear

```text
Use case: stylized-concept.
Asset type: transparent 2D game aiming-arrow sprite, candidate for a dark painterly frozen fantasy metroidvania.
Context and style reference: the project has moonlit icy blue-grey mountains, twisted dark forest, rocky caves and detailed crystalline ice projectiles. Make this warm fire magic contrast clearly with that cold world; high quality hand-painted game VFX with fine ember texture, bright ivory fire core, warm golden orange body and deep red wisps, sophisticated atmospheric fantasy rendering.
Composition: one isolated horizontal arrow pointing RIGHT, centered, straight horizontal centerline, a clear continuous shaft and only one unmistakable pointed arrowhead. Side-on orthographic flat game asset; silhouette readable when scaled to 150 pixels across. Asset should occupy roughly 75 percent of image width and no more than 40 percent of image height, generously padded on all sides so flames and glow are not cut off. Balanced vertical visual weight above and below horizontal axis, preserve readable forward direction.
Background: genuine transparent alpha, only the arrow and a few compact nearby sparks; glow should fade into transparency.
Constraints: no background, no environment, no player, no weapon held by a character, no labels, no letters, no logos, no watermark, no frame, no grid, no multiple arrows, no diagonal tilt, no pointing left. Original design inspired by fluid magical platformer readability, not a direct recreation of an existing game asset.
Variant 1 - Slender flame spear. A long tapered straight white-gold core shaft, a crisp broad triangular flame arrowhead with two swept-back orange barbs, fine red-orange flames hugging the shaft and tapering behind its left end. Elegant, clean and fast-looking; small contained flamelets instead of a large fire cloud.
```

### 2. SpiritChevron

```text
Use case: stylized-concept.
Asset type: transparent 2D game aiming-arrow sprite, candidate for a dark painterly frozen fantasy metroidvania.
Context and style reference: the project has moonlit icy blue-grey mountains, twisted dark forest, rocky caves and detailed crystalline ice projectiles. Make this warm fire magic contrast clearly with that cold world; high quality hand-painted game VFX with fine ember texture, bright ivory fire core, warm golden orange body and deep red wisps, sophisticated atmospheric fantasy rendering.
Composition: one isolated horizontal arrow pointing RIGHT, centered, straight horizontal centerline, a clear continuous shaft and only one unmistakable pointed arrowhead. Side-on orthographic flat game asset; silhouette readable when scaled to 150 pixels across. Asset should occupy roughly 75 percent of image width and no more than 40 percent of image height, generously padded on all sides so flames and glow are not cut off. Balanced vertical visual weight above and below horizontal axis, preserve readable forward direction.
Background: genuine transparent alpha, only the arrow and a few compact nearby sparks; glow should fade into transparency.
Constraints: no background, no environment, no player, no weapon held by a character, no labels, no letters, no logos, no watermark, no frame, no grid, no multiple arrows, no diagonal tilt, no pointing left. Original design inspired by fluid magical platformer readability, not a direct recreation of an existing game asset.
Variant 2 - Spirit chevron. A luminous ivory-gold arrowhead with gently curved swept-back flame wings forming a clear > chevron at the right end, a slimmer translucent golden shaft, three flowing orange ribbons hugging the shaft and a soft curled flame tail at the left. Dreamlike fluid living fire, soft painterly glow but clear directional arrow outline.
```

### 3. EmberRune

```text
Use case: stylized-concept.
Asset type: transparent 2D game aiming-arrow sprite, candidate for a dark painterly frozen fantasy metroidvania.
Context and style reference: the project has moonlit icy blue-grey mountains, twisted dark forest, rocky caves and detailed crystalline ice projectiles. Make this warm fire magic contrast clearly with that cold world; high quality hand-painted game VFX with fine ember texture, bright ivory fire core, warm golden orange body and deep red wisps, sophisticated atmospheric fantasy rendering.
Composition: one isolated horizontal arrow pointing RIGHT, centered, straight horizontal centerline, a clear continuous shaft and only one unmistakable pointed arrowhead. Side-on orthographic flat game asset; silhouette readable when scaled to 150 pixels across. Asset should occupy roughly 75 percent of image width and no more than 40 percent of image height, generously padded on all sides so flames and glow are not cut off. Balanced vertical visual weight above and below horizontal axis, preserve readable forward direction.
Background: genuine transparent alpha, only the arrow and a few compact nearby sparks; glow should fade into transparency.
Constraints: no background, no environment, no player, no weapon held by a character, no labels, no letters, no logos, no watermark, no frame, no grid, no multiple arrows, no diagonal tilt, no pointing left. Original design inspired by fluid magical platformer readability, not a direct recreation of an existing game asset.
Variant 3 - Ember runic spear. A sharp polished amber-gold arrowhead shaped as an elongated triangular crystal blade, a straight molten ember core shaft, short symmetrical flame fins near the arrowhead shoulders, restrained ruby red cracks and very few floating glowing fragments along the shaft. Ancient dark-fantasy magic, slightly angular and structured, no actual written runes or symbols.
```

### 4. FlameComet

```text
Use case: stylized-concept.
Asset type: transparent 2D game aiming-arrow sprite, candidate for a dark painterly frozen fantasy metroidvania.
Context and style reference: the project has moonlit icy blue-grey mountains, twisted dark forest, rocky caves and detailed crystalline ice projectiles. Make this warm fire magic contrast clearly with that cold world; high quality hand-painted game VFX with fine ember texture, bright ivory fire core, warm golden orange body and deep red wisps, sophisticated atmospheric fantasy rendering.
Composition: one isolated horizontal arrow pointing RIGHT, centered, straight horizontal centerline, a clear continuous shaft and only one unmistakable pointed arrowhead. Side-on orthographic flat game asset; silhouette readable when scaled to 150 pixels across. Asset should occupy roughly 75 percent of image width and no more than 40 percent of image height, generously padded on all sides so flames and glow are not cut off. Balanced vertical visual weight above and below horizontal axis, preserve readable forward direction.
Background: genuine transparent alpha, only the arrow and a few compact nearby sparks; glow should fade into transparency.
Constraints: no background, no environment, no player, no weapon held by a character, no labels, no letters, no logos, no watermark, no frame, no grid, no multiple arrows, no diagonal tilt, no pointing left. Original design inspired by fluid magical platformer readability, not a direct recreation of an existing game asset.
Variant 4 - Organic flame comet arrow. A distinct bright pointed broad arrowhead at the right, curved sweeping fire tongues behind the shoulders, a straight bright orange shaft within a lively compact streaming tail on the left. White hot center, layered orange-red semi-transparent flame edge; dynamic asymmetrical flame contour while the arrow centerline remains exactly horizontal. Make it look like living fire condensed into a readable arrow, not a round fireball.
```

