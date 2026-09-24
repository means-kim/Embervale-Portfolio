# 등반 프로토타입 v03 · 동굴 전용 천장

2026-09-23. [전체 구역 검토](ClimbMap_Prototype_v03.html) · [배치 데이터](ClimbMap_Prototype_v03.json) · [상단 출구 캡처](ClimbMap_Prototype_v03_Exit.png)

## 변경

상단에서 기존 긴 발판을 뒤집어 쓴 직선형 천장을 별도 동굴 천장으로 교체했다. 중앙은 더 들어가고 양쪽에 불규칙한 돌출부를 둬 폐쇄된 동굴 통로로 읽히게 했다. 좌우 벽과 같은 거친 암석 질감을 사용한다.

기존 발판3종, 폭·간격 샘플, 개별 소재·너비·X·Y 조절을 그대로 유지했다. 새 천장은 발판이나 배경에 합치지 않은 독립 PNG다. HTML의 ‘이전 천장 비교’는 천장만 v02로 전환하며 발판 편집값을 바꾸지 않는다.

## 사양

[BG_Climb_Ceiling_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ceiling_v01.png) 및 고유 GUID·spriteID의.meta.

내장 image_gen. 요청1536×768, 실제1774×887 RGBA. PPU100, 중앙pivot, Max2048. 하단 빈 영역의alpha0 확인. 컨셉과 기존 오른쪽 암벽을 실제 이미지로 확인·참조했다. 눈·고드름·하늘·바닥·캐릭터는 포함하지 않는다.

배치도1000×1600 기준 X-50, Y-100, 캔버스 너비1100, 회전0도, HTML 밝기80%. 기존 천장은 Y-96,180도,밝기55%였다. HTML 비교값이며 Unity Position·Scale·머티리얼 수치가 아니다.

## 확인

상단 출구에서 천장과 벽이 겹쳐 닫힌 동굴로 읽히고 정지 캐릭터를 가리지 않는 것을 시각 확인했다. 전후 토글은 전체 배치도·카메라 창 모두 반영된다. 다섯 구간의 이미지 로드 실패0, 캐릭터 보이는 높이약9%,16:9 유지.

검토는 아트 합성이다. 점프 중 머리 여유·충돌·실제 출구 통과·인접 보스 통로 접합은 미검증. 출입구 벽의 표시 범위는 아직 HTML clip-path에 의한 시안이며 Unity에 동일하게 구현한 것이 아니다. 이번 수정은 그 구조를 완성된 통로 모듈로 선언하지 않는다.

## 실제 프롬프트

```text
Use case stylized-concept transparent game sprite. Image1 master cave style, image2 current near cave sidewall stone style. Create ONE dedicated overhead cave ceiling rock slab, landscape1536x768 transparent PNG, side-on 2D hand-painted cave art. Opaque rock mass solidly touches entire TOP border and left/right borders in upper portion, deep cracked rough angular charcoal slate-blue rock. Bottom edge is organically jagged and undulating, NOT a straight horizontal platform: left hanging buttress ends near y520, center recess rises near y280, uneven right tooth aroundx1150 descends to y560, right edge aroundy420. All pixels BELOW the irregular edge truly transparent, minimum lowest margin100px. One connected cave roof, no isolated debris. Match reference2's stone density and muted cold lighting, darker top, very subtle blue reflected light on underside but no glowing rim. No snow, icicles, crystals, sky, clouds, moon, floor, lower ledge, arch pillars extending to bottom, buildings, characters, fog, text, border or checkerboard. This is an overhanging ceiling with broken downward silhouettes, explicitly not an upside-down flat ledge. Keep central upper passage roomy, varied large shapes readable at game scale.
```

