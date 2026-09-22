# 성·다리 v03 거리감 수정 후보 — 현재 미채택

2026-09-23. 내장 imagegen으로 CastleBridge_v02의 절벽 대비·하단 경계를 부드럽게 수정하고 같은 배치에서 비교했다.

[후보 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v03.png) · [전후 비교](StartMap_CastleBridge_v03_Compare.html) · [비교 캡처](StartMap_CastleBridge_v03_Compare.png) · [유지하는 최신 배치 v28](StartMap_Placement_v28.html)

## 결과와 결정

**현재 배치에는 CastleBridge_v02를 유지한다.** v03은 미채택 검토 후보이며 파일 이름의 높은 버전 번호가 최신 채택본이라는 뜻이 아니다. 조립표·배치 JSON의 성 경로를 변경하지 않았다.

컨셉과 비교했을 때 받침 절벽의 밝은 눈줄기와 선명한 면이 거리감을 약하게 만드는지 검토했다. 수정본은 하단 경계와 바위 세부가 약간 부드러워졌지만 전체 합성에서는 거리감 차이가 작고 성도 더 흐릿하게 읽힌다. 기대한 절벽 중심의 대비 개선이 충분하다고 보지 않아 기존 이미지를 교체하지 않았다.

단독 PNG에서 여전히 하단 안개 테두리와 밝은 눈줄기가 남는다. 프롬프트의 대비 약1/3 감소는 요청값이며 측정된 결과가 아니다. 원경 크기를 임의로 줄여 문제를 숨기지 않았고 캐릭터·동굴·지형 비율을 바꾸지 않았다.

## 실제 사양과 확인 범위

요청 및 실제1448×1086 RGBA. PPU100, 중앙pivot, SpriteSingle, FullRect, 기존v02 Import 설정 유지. GUID·spriteID는 새로 발급하고 PNG·.meta 함께 저장했다. Scale1 캔버스14.48×10.86유닛, 최종 Transform Scale 미정.

성·다리·받침 절벽과 자체 하단 안개를 포함한다. 하늘·숲·캐릭터·충돌은 제외. HTML width42%, left24.1%, top11.9% 동일 위치에서 전후 비교했다. 전체 이미지 로드 및 두 성의 표시 너비·위치 일치를 확인했다. 구조 실루엣의 완전한 픽셀 일치 검증은 하지 않았다. 캡처는 화면 범위다.

Unity 씬·게임 코드 변경 없음. 실제 카메라 크기가 확정된 후, 성 실루엣을 유지하면서 절벽만 수정할 필요가 있는지 다시 판단한다. 현재 단계에서는 미세한 거리감 수정을 반복하기보다 실제 배치에서의 가독성 확인이 더 유용하다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: CastleBridge_v02(수정 대상), Bg_Concept(거리감과 화풍). 출력 원본을 보존하고 프로젝트로 복사했다.

```text
Use case: precise-object-edit. Image1 is existing 1448x1086 transparent castle/bridge/cliff sprite, Image2 is the master winter concept (distant castle in upper-left forest). Preserve EXACT canvas, scale, position, castle silhouette, towers, bridge arches, stone masses and cool moonlight direction of Image1. This is ONLY an atmospheric distance/material contrast edit. The cliff pedestal currently has very sharp bright white snow veins and a conspicuous cloudy bottom edge which makes it appear foreground-like. Reduce contrast and small-scale detail of the cliff by about one third, soften bright snow streaks into muted slate blue-gray, gently blend the bottom quarter into real transparency with an irregular feathered alpha edge. Keep actual rock under main castle readable. Do not paint a white fog border or add a flat haze rectangle. Keep architecture slightly more legible than cliff, preserve dark tower silhouette and open transparent bridge arches. Overall distant cold gray-blue mood of image2; avoid brightening whole asset, neon, black crushing or strong blur. Preserve true alpha outside subject, don't insert sky/mountains/forest/background or a new landscape. No castle redesign, no moving or resizing, no new objects, no text. Output transparent PNG requested1448x1086. The saved sprite will be placed in the exact same coordinates as the input for direct comparison.
```
