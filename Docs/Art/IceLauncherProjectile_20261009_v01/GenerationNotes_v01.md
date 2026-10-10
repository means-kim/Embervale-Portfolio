# 얼음 발사기·투사체 시안 v01

2026-10-09. 사용자가 발사기 4종과 투사체 4종을 고른 뒤 직접 추가하는 범위. built-in image_gen으로 생성한 개별 PNG를 원본 그대로 복사. Unity 씬, 프리팹, 코드, Import 설정은 변경하지 않음. 시안이며 게임 화면 크기에서의 최종 가독성과 사용 크기는 미확정.

## 이미지 참고

- 기존 지침의 Assets/04.Sprites/Bg_Concept.png는 현재 없음. 실제 파일 [Bg_Concept.png](../../../Assets/04.Sprites/UI/Bg_Concept.png)을 열어 확인.
- [기존 고드름 공격](../../../Assets/04.Sprites/AirBossAttack/Attack_Icicle_Fall_AirBoss_4x3_v02.png), [YuchanScene 검토 캡처](../../Unity/YuchanLighting_20261009/reloaded-player.png)를 열어 확인.
- 발사기 1·2는 확인된 화풍을 텍스트로 명세하여 신규 생성. 발사기 3·4와 투사체 1~4는 Bg_Concept.png 및 발사기 1 PNG를 모델 이미지 참조로 입력.

## 실제 파일 사양과 확인

- 요청 프롬프트: 정사각형 1024×1024 선호. 실제 생성 PNG 8종은 모두 1254×1254.
- System.Drawing으로 실제 픽셀 크기와 Format32bppArgb 확인. 16픽셀 간격 알파 샘플에서 모든 파일에 Alpha 0인 투명 영역이 있음.
- 원본과 검토 폴더 복사본의 SHA-256 일치 확인. 리사이즈, 색 보정, 잘라내기 없이 원본 그대로 보존.
- 8종의 생성 결과를 실제 이미지로 검토. 캐릭터 대비 크기, 게임 화면 내 작은 크기의 가독성과 배치 합성은 미검증.

## 선택 번호

- 발사기 1 · 수정 광맥: [Launcher_01_Geode_v01.png](Launcher_01_Geode_v01.png)
- 발사기 2 · 서리 꽃: [Launcher_02_FrostBloom_v01.png](Launcher_02_FrostBloom_v01.png)
- 발사기 3 · 석조 분출구: [Launcher_03_RuinSpout_v01.png](Launcher_03_RuinSpout_v01.png)
- 발사기 4 · 나선 얼음뿔: [Launcher_04_SpiralHorn_v01.png](Launcher_04_SpiralHorn_v01.png)
- 투사체 1 · 얼음 구슬: [Projectile_01_FrozenOrb_v01.png](Projectile_01_FrozenOrb_v01.png)
- 투사체 2 · 고드름 창: [Projectile_02_IcicleDart_v01.png](Projectile_02_IcicleDart_v01.png)
- 투사체 3 · 눈꽃 결정: [Projectile_03_SnowCrystal_v01.png](Projectile_03_SnowCrystal_v01.png)
- 투사체 4 · 서리 씨앗: [Projectile_04_FrostSeed_v01.png](Projectile_04_FrostSeed_v01.png)

## 실제 생성 프롬프트

### 발사기 1 · 수정 광맥

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: compact natural ice-crystal geode emitter, a low squat cluster of deep indigo rock and asymmetrical icy quartz spikes rooted in one small integral rock foot. Its prominent short horizontal faceted crystal throat points RIGHT, with a dark circular opening visible slightly from the side and a bright icy cyan core recessed inside. Broad stable bottom, uneven tallest crystals at the rear left, frost on upper surfaces. Organic magical geological launcher, no metal gun, no modern technology. Nothing is being shot; emitter idle.
```

### 발사기 2 · 서리 꽃

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: a frost flower emitter, an elegant curved icy stalk arising from a compact dark rock root, with 5 broad translucent ice petals forming a trumpet flower that opens horizontally RIGHT. A small bright pale-blue seed core inside the throat marks the firing point. Strong organic crescent silhouette, thick sturdy frosted petals rather than delicate lace. Silver frost veins, small icicles underneath, muted cyan internal light. Natural enchanted cavern plant, no face, no metal gun. Flower and root are one isolated asset. Nothing is being shot; emitter idle.
```

### 발사기 3 · 석조 분출구

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: a short ancient charcoal-blue carved stone pedestal emitter encrusted in translucent blue ice, with an angular horizontally projecting stone-and-ice spout facing RIGHT, chipped relic architecture and a recessed cyan crystal chamber. Compact sturdy weathered pedestal, a few abstract carved grooves without letters, snow dust along top ledges and slim icicles under spout. Eroded ruins matching a snowy ruined temple, no skull or face, no machinery, no barrels, no metal gun. Stable foot is part of the object, no surrounding environment. Nothing is being shot; emitter idle.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

### 발사기 4 · 나선 얼음뿔

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: natural spiral ice horn emitter. A single thick curling frozen formation growing from a dark fractured rock base, coiling in a C-shaped spiral and terminating in a short flared hollow ice nozzle that points horizontally RIGHT. Broad tapering glacier facets, dark blue thickness and pale edge highlights, subtle inner luminous cyan ice vein through the curve. Distinct hooked silhouette, dangerous yet ancient and natural. No animal, no shell animal, no bone, no machinery. One complete ground-placeable object. Nothing is being shot; emitter idle.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

### 투사체 1 · 얼음 구슬

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: a small round faceted ice projectile, near-spherical irregular ice crystal encasing one bright cyan-white heart. A few short chunky jagged crystal facets project slightly from its perimeter, a readable compact round silhouette. Dark translucent azure outer rim and white-blue front glint make it visible over both snow and dark stone. Mild localized translucent halo only, no long motion trail, no decorative orbit ring, no environment or stand. Symmetrical projectile usable in any travel direction.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

### 투사체 2 · 고드름 창

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: single horizontal icicle dart projectile pointing precisely RIGHT. A thick sharply tapering hand-painted crystalline spearhead with an asymmetric rough fractured heel at LEFT. Length about 2.7 times its width, one strong sharp right tip, 3 broad blue ice facets, cool silver-white edges, a small embedded cyan magical core near the heel. Minimal translucent cold vapor clings only at the heel, no long trail. Clear object silhouette suitable for rotation with velocity. No arrow shaft, no feathers, no weapon handle, no attached launcher.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

### 투사체 3 · 눈꽃 결정

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: chunky six-point snow crystal projectile, a solid compact frozen hexagonal central gem surrounded by six short broad spear-like translucent icy points. Clearly six arms with restrained small secondary facets; substantial readable shape, not delicate thin snowflake lace. Cold deep azure central volume, small luminous cyan heart and silver frosted edges, slight restrained tight halo. Symmetric radial silhouette, no motion trail, no ring, no background.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

### 투사체 4 · 서리 씨앗

transparent_background=true

```text
Use case: stylized-concept. Asset type: individual transparent PNG sprite concept for a 2D side-scrolling dark fantasy metroidvania. Match the project's inspected visual language: painterly textured fantasy, moonlit snowy black-blue rock caverns, ancient ruined stone, translucent blue-white ice, charcoal navy midtones, silver frost edges, subdued cyan glow in a small focal core. Readable substantial shapes at small gameplay size, finely painted facets but restrained microdetail, no thick black cartoon outlines, not pixel art, not photoreal studio rendering. Side-view orthographic sprite, depth painted only within object, all edges fully within canvas with 12 percent transparent padding. Single isolated object only, transparent alpha everywhere outside object. Soft cool upper-left key light, restrained internal glow, no opaque backdrop, no floor plane, no cast shadow outside asset, no border, no labels, no lettering, no watermark, no character, no arrow, no UI, no sprite sheet, no repeated views. Prefer square 1024x1024 output. Subject: ice spirit seed projectile traveling RIGHT, an oval pointed translucent frozen seed with its compact pale cyan-white core toward the right front and three short curled frosty ice ribbons wrapped around the rear LEFT. The ribbons remain close to the seed, total silhouette compact teardrop about twice as wide as high. Layered glassy blue ice, a little silver-white frost, very mild semitransparent inner glow. Mysterious magical shard, no face, no eyes, no creature, no fire, no orange, no long detached trail, no arrow.
Reference images: image 1 is the existing game concept for palette, lighting and material style ONLY; image 2 is the newly generated launcher for consistent ice material language ONLY. Do not copy their full composition or objects. Create only the subject specified above.
```

