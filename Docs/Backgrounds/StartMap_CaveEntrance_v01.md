# 시작맵 동굴 입구 v01

2026-09-22 사용자 방향 변경: 오른쪽 고목을 제외하고 오른쪽 동굴로 들어간다. 고목 수정과 고목 눈 추가를 중단한다. 과거 고목 소재는 삭제하지 않고 보존하며 최신 합성에서 제외한다. 기존 전체 맵 구역 순서를 모두 동굴로 바꾸는 지시는 아니므로 동굴 뒤 연결은 미정이다.

[새 소재](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveEntrance_v01.png), [최신 합성 v13](StartMap_Placement_v13.html), [합성 이미지](StartMap_Placement_v13.png).

## 사양·용도

요청1024×1024, 실제1254×1254 RGBA. 좌상단 alpha0 확인. PPU100, 중앙pivot, FullRect, 최대2048, 고유.meta 동반. 원본 컨셉의 어두운 중앙 암벽 질감을 참조한 동굴 입구 구도 후보다. 암벽·어두운 내부 벽·얇은 바닥 턱이 현재 한 이미지에 포함된다. 눈·얼음·전경 가림은 포함하지 않는다. 최종 모듈 지형이나 콜라이더 에셋으로 확정하지 않는다.

HTML 배치는 너비70%, left46%, top9%. 첫 배치에서 보인 암벽 상단 사각 경계를 화면 밖으로 옮겼다. 캐릭터는 이전에 수정한 컨셉 기준26px/480px 높이를 유지한다. 발 기준선89.7%는 이 합성의 임시값이다. 고목 레이어를 모두 뺀 상태에서 입구 및 내부 위치의 캐릭터 가시성을 확인했다. 실제 씬·이동·충돌 검증이 아니다.

숲에서 입구까지 이어지는 바닥이 아직 없어 입구 턱이 떠 보이는 구간이 남는다. 후속 지형 배치와 플레이 크기 검증이 필요하다. 새 동굴 내부와 다음 구역 연결은 미제작이다. Unity 씬, 캐릭터 설정, 게임 코드는 변경하지 않았다.

## 실제 프롬프트

내장 imagegen으로 생성, 프로젝트에 복사. 참조는 Bg_Concept 실제 이미지.

```text
Use case: stylized-concept. Create a transparent PNG 1024x1024 game art placement candidate: a dark rugged rock cliff with a SIDE-VIEW cave entrance for a 2D platformer walking from LEFT to RIGHT. Reference image is the master palette and angular rock texture, especially its central dark rocky mass. This replaces the giant right-hand tree of the starting area. The rock mass occupies the upper right and right border. Its ragged left edge slopes down to an overhang around x400 y670. Below this overhang a horizontal dark tunnel extends from x400 toward the RIGHT image edge, with floor near y880 and roof near y670: a sideways passage continuing offscreen right, NOT a front-facing oval hole or symmetrical stone doorway. Include a dark painted recessed interior back wall in the tunnel so distant forest does not show through it. Overall left upper area outside the rock silhouette must be true transparent alpha; below the floor only a thin jagged rock ledge to y1024. Cold desaturated charcoal blue, restrained moonlit edges, hand-painted dense weathered angular stone matching reference, no bright outline. Keep foreground entry open and readable, no freestanding arch, no trees, roots, timber, castle, sanctuary, lift, character, text, snow or ice (separate layers later). One rock cave entry/back-wall art asset, no background landscape. This is a local entrance candidate, not an entire level.
```

