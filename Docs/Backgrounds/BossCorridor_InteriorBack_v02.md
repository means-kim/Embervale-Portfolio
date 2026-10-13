# 통로 동굴 후면 v02

사용자 요청: BG_BossCorridor_InteriorBack_v01이 주변 동굴과 어색하게 보이는 문제 보완.

- 제작: 내장 imagegen 편집 도구. 기존 BossCorridor v01과 Climb04_BackRockTile_v01을 이미지 참조로 사용.
- 결과: [BG_BossCorridor_InteriorBack_v02.png](../../Assets/04.Sprites/Backgrounds/BossCorridor/BG_BossCorridor_InteriorBack_v02.png), 실제 1536×1024, 불투명 배경. Sprite Single, PPU 100, Max Size 2048, Uncompressed, Trilinear, mipmaps On.
- 큰 능선과 사선으로 솟는 암반을 줄이고 작은 후면 암벽 결로 변경. v01 원본 및 다른 구역의 v01 사용은 보존.
- MainTestScene의 03_Corridor 후면·상단 채움·하단 연장 3개 렌더러에 v02 전용 머티리얼 적용. 월드 좌표를 기준으로 UV와 색상을 통일하여 조각 접합에서 그림과 밝기가 이어지도록 구성.
- 실제로는 02_Climb/Cave_UpperBoundary/Cave_ExteriorUpperShell_v01이 앞쪽 레이어에서 통로 후면을 덮고 있었음. 이 오브젝트만 새 메시로 교체하여 X70~84에서 우측 겹침을 점차 투명하게 줄임. 원본 메시 보존.
- 기존 조명·입구 가림 셰이더 유지. 콜라이더 및 게임 코드는 변경하지 않음.
- 확인: 통로와 등반 진입부의 64×36 월드 영역을 각각 768×432(16:9) Unity 렌더로 확인. 하단 수평 밝기 경계를 보완한 최종 렌더 확인. 실제 이동·파괴벽 동작 검증은 이번 작업에 포함하지 않음.

## 생성 프롬프트

```text
Use case: precise-object-edit. Asset type: opaque 2D side-scrolling game cave rear background, wide landscape 3:2. Image 1 edit target BG_BossCorridor_InteriorBack_v01; image 2 supporting style reference BG_Climb04_BackRockTile_v01. Redesign image 1 into a calmer recessed cave back wall that seamlessly fits the reference game's blue-black layered slate rocks. Preserve the existing hand-painted detailed chipped rock brushwork, cool desaturated navy/charcoal palette, very dim blue rim light, dark subterranean atmosphere. Replace the tall sloping foreground-like rock ridges and repeating pointed summits with an irregular continuous recessed rocky wall: shallow broad craggy strata, small fissures, subtle pockets of depth. Moderate-small texture scale, restrained low contrast so actual foreground platforms and player remain clear. Wall fills entire image without sky, ground plane, silhouettes of peaks, a cave opening, architecture, platforms, spikes, snow, characters, text, glow sources. No framing, no vignette, no borders. Left/right and top/bottom edges have similar quiet rock texture and illumination so a cropped section blends into neighboring cave walls. Avoid vertical pillar forest, diagonal sweeping ridge silhouettes, mirror symmetry, black empty background. Opaque full image.
```
