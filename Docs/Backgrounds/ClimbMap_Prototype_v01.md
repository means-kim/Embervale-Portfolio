# 등반 구역 전체 아트 프로토타입 v01

2026-09-23. 사용자 요청으로 작업 단위를 ‘소재 하나씩 확인’에서 ‘구역 전체 프로토타입 제작 후 피드백’으로 변경했다. 이번 범위는 동굴 등반 구역의 하단 진입부터 상단 출구까지다.

[프로토타입](ClimbMap_Prototype_v01.html) · [배치 데이터](ClimbMap_Prototype_v01.json)

## 전체 구성

하단 진입 바닥 → 좌우를 번갈아 오르는 발판 → 넓은 중간 쉼턱 → 상단 등반 → 오른쪽 실내 출구. 모든 구간은 불투명 동굴 후면으로 채워 하늘·설산을 제외했다. 이전 야외 원경 후보를 사용하지 않는다.

기존 후면에 새 좌벽·우벽·발판3개를 더했다. 입구 바닥 포함9개 지면/발판 배치와 별도 천장1개를 구성했다. 발판 형태를 크기만 달리해 재사용했고 천장은 발판을180도 돌린 임시 조립이다. 출입구는 HTML에서 벽 표시 범위를 잘라 만든 구도 후보이며 실제 연결부 스프라이트의 완성 형태가 아니다.

## 소재 사양

내장 image_gen 사용. 원본 컨셉 지하 영역과 현재 내부 후면을 실제로 열어 참조했다. PNG와 고유 GUID·spriteID의.meta를 함께 저장했다. PPU100, 중앙pivot, Sprite Single, 압축 없음. 실제 Unity Transform Scale은 미정.

| 소재 | 요청 크기 | 실제 PNG | Import Max |
|---|---|---|---|
| [좌벽](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_WallLeft_v01.png) | 1024×1536 | 1024×1536 RGBA | 2048 |
| [우벽](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_WallRight_v01.png) | 1024×1536 | 1024×1536 RGBA | 2048 |
| [발판](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ledge_v01.png) | 1536×512 | 2172×724 RGBA | 4096 |

벽의 빈 쪽 모서리alpha0과 발판 모서리alpha0 확인. 벽 가장자리에는 생성된 옅은 반투명 빛 번짐이 일부 남아 있으며 합성에서 확인했다. 완벽한 불투명 벽 경계로 보증하지 않는다. 발판 Max4096은 실제2172px 폭이 Import에서2048로 줄어들지 않도록 해당 신규 소재에만 지정한 값이며 원본을4K로 확대한 것이 아니다.

## 검토와 수정

첫 합성에서 왼쪽 발판 일부가 벽에서 떨어져 보여 X를0, 너비540으로 수정했다. 전체 배치도와 카메라 창에 같은 값을 반영했다. 후면 밝기65%, 좌우 벽80%, 발판85%, 천장55%는 HTML 비교값이며 Unity와 수치 동일성을 검증하지 않았다.

배치도1000×1600, 카메라1000×562.5로16:9. 캐릭터 보이는 높이 약9.00% 확인. 다섯 비교 위치 모두 캐릭터가 창 안에 있고 이미지 로드 실패0, 두 합성의 발판좌표 일치 확인. 동선 표시와 구간 전환, 자동 둘러보기를 검토했다.

- [하단 진입](ClimbMap_Prototype_v01_Entry.png)
- [첫 등반](ClimbMap_Prototype_v01_Lower.png)
- [중간 쉼턱](ClimbMap_Prototype_v01_Middle.png)
- [상단 등반](ClimbMap_Prototype_v01_Upper.png)
- [출구](ClimbMap_Prototype_v01_Exit.png)

## 한계와 피드백

이것은 구역 전체 아트 시안이며 실제 플레이 프로토타입은 아니다. 구간 버튼은 캐릭터 참고 위치를 바꾸고, 자동 둘러보기는 장면을 순서대로 전환한다. 이동·점프·충돌·게이트·카메라 추적·패럴랙스 코드는 구현하지 않았다. Unity 씬·게임 코드·기존 소재는 변경하지 않았다.

발판 간격, 천장 높이, 출구 통과 가능 여부는 실제 캐릭터 이동 능력으로 검증해야 한다. 캐릭터 발 높이도 캔버스상의 대략적 표면을 기준으로 맞춘 비교이며 충돌 접지 검증이 아니다. 후면 속 돌출 암벽은 밟는 지형이 아니다.

한 발판의 반복과 직선적인 천장 윤곽은 아직 보인다. 다음 수정은 전체 분위기·공간감·동선 피드백 이후 발판 변형 및 출입구 전용 소재를 우선 검토한다. 외부 시작맵에서 실내로 넘어오는 실제 전환과 다음 보스 통로는 이번 구역 바깥의 연결 검증으로 남는다.

사용자에게 받을 피드백: 실내 밝기, 통로의 넓이와 답답함, 중간 쉼턱·상단 출구의 자연스러움. 지금 결과를 자동 승인으로 간주하지 않는다.

## 실제 생성 프롬프트

### 좌벽

```text
Use case: stylized-concept, transparent game sprite. Create a LEFT SIDE near rock wall for a closed interior vertical climbing cave, matching image1 master concept's underground stone (no architecture) and image2 existing dark rear cave wall. Portrait1024x1536 transparent PNG. Opaque rock occupies leftmost 50-65% and runs completely through TOP and BOTTOM canvas borders; rugged broken right edge projects irregularly into transparent space, with rightmost25% entirely transparent. Solid full left border, no transparent holes in rock mass. No tunnel opening. Visually continuous tall wall with 3 subtle outward rock buttresses, not regular stairs or baked playable platforms. Larger chunky jagged blocks, charcoal slate blue, sharper than background, subtle cold light on right-facing edges and upper ledges. Mid-dark readable forms, not black flat silhouette. Actual empty alpha to right. No floor across bottom, no ceiling across top, snow, icicles, crystals, vegetation, sky, lamps, people, structures, HUD, text, extra isolated fragments. One connected sidewall sprite only. The wall boundaries offscreen don't need to tile seamlessly.
```

### 우벽

```text
Use case: stylized-concept, transparent game sprite. Create a RIGHT SIDE near rock wall for a closed interior vertical climbing cave. Image1 is master underground stone visual reference, image2 existing cave rear wall. Portrait1024x1536 transparent PNG. Opaque rock occupies rightmost50-65% and runs through TOP and BOTTOM edges, rugged irregular LEFT edge projects toward center, leftmost25% entirely transparent. Solid right border. Different silhouette from left wall: broad upper shoulder, recessed middle edge, heavy lower buttress. No tunnel opening, no holes in rock. Large irregular angular cracked stone with cold charcoal slate-blue coloration, subtle upper-left cool light, sharper and darker than rear background. Not stairs or regular platforms. No floor across bottom, ceiling across top, snow, icicles, crystals, vegetation, sky, characters, buildings, text, border or extra floating fragments. One connected right wall with true transparent left margin.
```

### 발판

```text
Use case: stylized-concept, transparent sprite. Image1 master underground cave rock style; image2 existing cave rear wall palette. Create ONE modular bare rock ledge for a 2D side-view platformer. Landscape1536x512 transparent PNG. A broad roughly horizontal but irregular top surface across x80..1450 at y120..165, jagged thick fractured underside ending aroundy400, isolated ledge not attached to any visible wall. Large dark charcoal/slate-blue angular blocks, detailed hand painted cracks, a thin subtle cold bluish rim along the top makes landing surface readable against dark cave. No snow, ice, sky, mist, greenery, crystals, characters, lighting props, shadow cast outside sprite, checkerboard, text. Real transparency on all sides, 60px minimum margins left right, 90px top,80px bottom. Not floating debris collection: one connected solid ledge. Side-on terrain art only; no perspective path or multiple platforms.
```

