# 등반 하단 동굴 후면 v01

2026-09-23. 최신 사용자 수정: 동굴 진입 후 하늘·설산을 보이지 않게 하고 내부 배경으로 구성한다. 위로 열린 야외 암벽 틈 안은 철회했다. 등반 방향은 유지하되 별도 외부 출구 합의 전까지 실내다.

[검토 화면](ClimbMap_InteriorBack_v01.html) · [하단 캡처](ClimbMap_InteriorBack_v01_Entry.png) · [소재](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_InteriorBack_v01.png)

## 사양과 분리

내장 image_gen으로 제작. 요청 및 실제1024×1536, RGB 불투명 PNG. PPU100, 중앙pivot, 고유 GUID·spriteID의 .meta. 기존 원경 Import를 참고했고 최종 월드 Scale은 미정이다.

컨셉의 지하 암벽 부분과 기존 동굴 입구를 실제 이미지로 확인·참조했다. 사용자 예시는 외부/내부 배경 차단 원칙의 참고이며 해당 게임의 실제 렌더링 구현을 분석한 것은 아니다.

포함: 동굴 내부 후면 암벽, 깊은 균열, 옅은 냉색 내부 안개. 일부 돌출 윤곽은 뒤쪽 장식이며 발판이 아니다. 제외: 하늘·달·설산·외부 개구부·캐릭터·바닥·플레이 발판·눈·성소·리프트. 안개는 이번 PNG 내부에 합쳐져 있어 독립 이동하지 않는다.

## 화면 검토

16:9로 PNG 높이37.5%만 표시하고 Y0~62.5%를 탐색한다. 캐릭터 실제 그림 높이는 약9.00%로 측정했다. 하단과 상단의 정지 화면에서 하늘 없이 암벽이 배경을 채우며 캐릭터가 구분된다. 이미지 로드 실패0. 점선은 비교용 발 높이로 바닥이나 충돌이 아니다.

하단은 외부 원경보다 어둡고 닫힌 공간으로 읽힌다. 암벽의 세부 무늬가 남아 있으므로 실제 앞쪽 암벽과 발판을 겹친 뒤 거리감과 가독성을 재평가한다. 현재는 후면 단독 후보이며 실제 접합·Unity 카메라 추적·패럴랙스·플레이 검증 전이다.

## 연결 및 다음 소재

외부의 하늘은 시작맵 밖에 유지한다. 입구 경계에서는 천장·암벽·전이 통로로 외부 풍경을 가리고, 내부에서는 이 불투명 후면이 화면을 채우도록 배치한다. 검토 화면은 외부와 내부를 나란히 보여주는 아트 비교이며 자동 전환 기능을 구현하지 않았다.

다음은 하단 진입부 앞쪽 암벽 한 소재. 후면과 별도로 제작하며 실제 통로 폭·발판 간격은 이동·점프 검증에 맞춰 결정한다. 실내 눈은 자동 추가하지 않는다.

이전 BG_Climb_LowerFar_v01.png와 비교 자료는 과거 야외 후보로 보존하고 실내 배치에서 제외한다.

## 실제 프롬프트

```text
Use case: stylized-concept. Produce ONE opaque full-bleed portrait1024x1536 background sprite, a CLOSED CAVE INTERIOR rear wall for the lower entry of a vertical ascending cave in a hand-painted 2D metroidvania. Image1 is master world concept: use ONLY its underground cavern stone language and cold charcoal/slate-blue palette, never its outdoor sky, mountains or sanctuary. Image2 is existing entrance rock: match rough angular layered cracked stone and dark value range, but softer lower contrast because this is the farther back wall. Interior enclosed on every edge. Entire canvas must be subterranean rock and shadow, NO sky, stars, moon, clouds, snowy mountains, horizon, outdoor opening, trees or exterior view. Overlapping recessed rock planes and narrow deep shadow fissures suggest vertical cavern depth. Broad subdued wall shapes on upper third, receding rock surfaces in center and lower thirds. Very faint dusty blue interior haze only, not bright cyan glow. Soft indirect cold light from off-canvas upper left, no literal light source or hole. Keep central mid-lower regions quiet enough to read a small orange player added later, avoid huge black circles or bright center. No playable floor, flat horizontal platform, path, front arch or foreground frame, hanging icicle props, snow caps, crystals, vegetation, ruins, building, shrine, characters, HUD, text, border. No forced perspective walkway. This is only the REAR scenic layer behind separately assembled walls, ground and platforms, not a complete map or playable cave illustration. Portrait coverage is for a moving16:9 crop, not to fit a whole map on screen. Fine hand-painted stone texture, restrained dark blue atmospheric depth, no photographic look.
```

