# 동굴 천장 고드름 v01

2026-09-23. 시작맵 동굴 입구에 붙일 별도 천장 장식 한 소재를 제작했다.

[고드름 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveIcicles_v01.png), [최신 합성 v20](StartMap_Placement_v20.html), [검토 이미지](StartMap_Placement_v20.png).

요청1536×768, 실제1774×887 RGBA, 모서리alpha0. PPU100·중앙pivot·FullRect·최대2048, 고유.meta. 암벽·눈·캐릭터·콜라이더를 포함하지 않는 얼음 군집이다. 원본 컨셉과 현재 동굴 입구를 이미지 참조로 사용했다.

합성 후보: width14%, left84%, top67%, HTML brightness(.55). PNG 밝기는 그대로 보존했다. 암벽 다음, 플레이어 이전에 배치하고 천장 접합부에 겹쳤다. 동굴 가로 이동 시 검토 화면의 고드름은 동굴left+38%로 함께 움직인다. Unity에서는 독립 소재이므로 따로 배치할 수 있다.

현재 화면에서 천장 접합과 서 있는 캐릭터 가시성, 표시 토글을 확인했다. 전체 페이지 캡처가 비정상적으로 줄바꿈되어 현재 화면 범위 캡처로 결과를 저장했다. HTML 표시 자체는 정상 너비로 확인했다. 최종 해상도에서 접합, 점프 여유, 위험물로 오인되는지는 미검증이다. 장식 후보이며 낙하·피해·충돌 기능은 없다. 실제 씬·코드·캐릭터 설정은 변경하지 않았다.

## 실제 프롬프트

내장 imagegen 사용, 참조: Bg_Concept → CaveEntrance_v01. 생성 PNG를 프로젝트로 복사했다.

```text
Use case: stylized-concept. Transparent PNG 1536x768 of ONE small irregular fringe of hanging icicles, isolated decorative sprite for a side-view winter cave entrance. Image1 master concept ice texture and palette, Image2 existing cave to match. A narrow rough frozen attachment seam near y180 spanning x170..1380, with 7-11 uneven downward tapered icicles, mostly short, a few medium ones ending y520, maximum one ends y600. Airy asymmetric gaps, no regular teeth, no huge centerpiece. Restrained deep blue translucent-looking painted ice, muted cold silver-blue edge light from upper left; NO bright white or neon cyan glow. Top seam organic and thin so it can overlap rock ceiling. This asset contains ONLY ice, genuine alpha transparent background and gaps. NO rock ceiling, snow cap, water, particles, background, ground, trees, architecture, character or text. Hand-painted texture matching the reference, not photoreal glass. Full fringe inside canvas with transparent padding. A decorative ceiling element, no implied trap mechanism.
```

