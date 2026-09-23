# 등반 프로토타입 v02 · 발판 폭과 간격 다양화

2026-09-23. 사용자 피드백: 넓은 발판만 반복하지 말고 좁음·넓음·간격을 직접 유동적으로 배치할 수 있게 한다.

[프로토타입](ClimbMap_Prototype_v02.html) · [기본 배치 JSON](ClimbMap_Prototype_v02.json)

## 반영

좁은 착지용과 중간 이동용 PNG를 새로 만들고 기존 넓은 소재와 함께3종으로 구성했다. 새 PNG는 긴 그림을 가로로 찌그러뜨린 파일이 아니다. 각 발판의 위치와 크기를 독립적으로 지정하며, 벽이나 후면에 합쳐 넣지 않았다.

샘플 배치: 좁은 발판 캔버스 너비180/200/220/240, 중간300/350, 쉼턱620. 이는1000폭 배치도상의 비교값이며 Unity 월드 단위가 아니다. 투명 여백을 제외한 실제 바위 폭은 이보다 작다. 발판 높이와 가로 간격에도 변화를 주었다. 일부는 후면 앞의 독립 발판으로 보이며 벽 부착·독립 배치의 최종 형태는 사용자가 정한다.

## 새 소재

내장 image_gen으로 생성. 실제 컨셉과 기존 긴 발판을 이미지로 확인·참조했다. PPU100, 중앙pivot, 각각 고유 GUID·spriteID의.meta. 실제 Transform Scale 미정.

| 소재 | 요청 | 실제 | 용도 |
|---|---|---|---|
| [LedgeSmall_v01](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_LedgeSmall_v01.png) | 1024×768 | 1448×1086 RGBA | 좁은 착지점 |
| [LedgeMedium_v01](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_LedgeMedium_v01.png) | 1536×768 | 1774×887 RGBA | 중간 폭 이동 발판 |
| [기존 Ledge_v01](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ledge_v01.png) | 기존 파일 | 2172×724 RGBA | 넓은 쉼턱·바닥·통로 |

두 신규 PNG는 모서리alpha0 확인. 이미지 여백이 서로 다르므로 동일 Transform Scale이 동일한 보이는 폭을 의미하지 않는다. 비율을 유지한 크기 변경은 두께도 함께 바꾼다. 길이만 자유롭게 바꾸는9-slice나 타일 지형 시스템은 구현하지 않았다.

## 검토 화면 조작

‘발판 개별 배치’에서 대상을 선택하고 소재·너비·X·Y를 바꾼다. ‘선택 발판 보기’는 해당 발판으로 카메라와 비교 캐릭터를 이동한다. ‘기본 배치 복원’은 샘플 배치를 복원한다. 전체 배치도와 카메라 양쪽에 동일하게 반영된다.

편집값은 페이지 메모리에만 있으며 새로고침하면 초기화된다. ‘배치 JSON 저장’ 기능은 검토 좌표를 내려받도록 작성했다. 기본 JSON은 프로젝트에 저장되어 있다. 브라우저 다운로드 파일의 실제 저장 위치·완료는 검증하지 않았다. JSON을 Unity에 자동 적용하지 않는다.

## 확인 및 한계

[하단](ClimbMap_Prototype_v02_Entry.png), [중간 쉼턱](ClimbMap_Prototype_v02_Middle.png)에서 좁음·중간·넓음의 차이를 확인했다. p3를 중간 소재·너비260·X300·Y940으로 바꿔 두 화면에 동일 반영되는지 확인하고 기본값으로 복원했다. 다섯 구간 이미지 로드 실패0, 캐릭터 실제 그림 높이약9%, 16:9 유지.

소재와 간격의 다양화를 위한 아트 시안이다. 큰 간격이 실제로 점프 가능한지, 작은 착지점에서 충분히 조작 가능한지는 아직 검증하지 않았다. 현재 배치를 고정 레벨 설계로 취급하지 않는다. 실제 Unity 배치·충돌은 사용자 담당이며 이번에는 씬·게임 코드를 바꾸지 않았다.

## 실제 프롬프트

### 좁은 발판

```text
Create ONE isolated narrow rock landing platform sprite for a dark hand-painted2D side-view cave platformer. Image1 master underground cave style, image2 existing WIDE platform style/material reference only. New platform must be a compact chunky small ledge, NOT the long platform squeezed horizontally. Requested1024x768 transparent PNG. One connected asymmetric rock mass occupies x180..840,y250..600, top surface roughly y250..290, broad enough for a small character to stand, visibly irregular but nearly horizontal top. Only3 or4 large broken angular stone blocks, dark charcoal slate-blue with restrained cool blue top rim and deep cracked sides. Jagged thick underside with one offset taper. Real alpha empty all around. Same rough painted stone texture as reference2, similar stone detail scale, no snow, ice, sky, haze, vegetation, crystals, structures, characters, text, checkerboard, borders, other platforms. Designed to use independently or embed partly in sidewall. Keep actual small boulder-like mass rather than making a flat thin beam.
```

### 중간 발판

```text
Create ONE isolated MEDIUM rock platform sprite for a dark hand-painted2D cave platformer. Reference1 master cave concept for stone mood, reference2 existing wide ledge for material continuity. Requested1536x768 transparent PNG. One connected asymmetric ledge x130..1400,y230..610; mostly horizontal irregular landing top y230..280, several large cracked angular blocks with varied undersides, left underside deeper, right edge broken blunt. More compact and deeper relative to width than the long reference, but wider than a single small landing stone. Do not simply scale or squash reference2. Cool charcoal slate-blue, restrained upper-left light and bluish rim, rugged painterly rock texture. True transparent margins. No snow, ice, mist, sky, characters, vegetation, buildings, text, checkered background, extra platforms or detached debris.
```

