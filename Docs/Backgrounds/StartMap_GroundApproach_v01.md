# 숲에서 동굴로 이어지는 바위 지면 v01

2026-09-22. 사용자는 동굴 입구를 나중에 실제 배치에서 오른쪽으로 이동할 수 있는지 물었고, 가능하면 다음 작업 진행을 요청했다. 입구는 독립 스프라이트라 이동 가능하며 바닥 연결·충돌·구역 경계를 함께 맞춰야 한다. 이번 작업은 최종 플레이 지형 확정이 아닌 연결용 아트 후보다.

## 결과물

[BG_Start_GroundApproach_v01.png](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundApproach_v01.png), 고유 .meta 동반. 생성 요청1536×768, 실제1774×887 RGBA. 좌상단 alpha0. PPU100, 중앙pivot, FullRect, 최대2048 등 기존 동굴과 동일 Import 설정. 실제 월드 크기·Scale은 미정이다.

포함: 낮은 요철의 바위 표면과 그 아래 암벽. 제외: 눈·얼음·식물·배경·캐릭터·동굴 천장. 지면과 눈을 합치지 않았다. PNG에는 큰 상단 투명 여백이 있으므로 캔버스 상단이 보행면이 아니다. 요청한 y130 대신 실제 표면은 대략 y510 부근에 생성되어 실제 이미지 기준으로 위치를 맞췄다.

[합성 v14](StartMap_Placement_v14.html) / [이미지](StartMap_Placement_v14.png). 원본 컨셉과 기존 동굴 소재를 참조하고 생성 후 합성 확인했다. 지면 캔버스를 너비100%, left0%, top51.2%로 배치하고 동굴보다 뒤에 그려 겹쳤다. 화면 하단의 빈 부분을 메웠고 표면 높이를 동굴의 바닥과 근접하게 맞췄다. 질감 이음은 확대·실제 카메라에서 추가 조정이 필요하며 무봉제 타일이라고 검증하지 않았다.

동굴 가로 위치를46%에서54%로 이동한 시안도 확인한 뒤 기본46%로 저장했다. HTML 숫자 입력으로 이를 비교할 수 있다. 이 입력은 Unity 배치를 저장하지 않는다. 캐릭터 컨셉 비율26px/480px를 유지한다. 충돌·점프·캐릭터 접지·맵 경계는 실제 플레이에서 미검증이다. Unity 씬과 게임 코드는 변경하지 않았다.

다음 후보는 이 지면 위의 별도 눈 덮개이며, 지형 형상 변경이나 다른 구역 확장은 확정된 플레이 레이아웃에 맞춰 진행한다.

## 실제 프롬프트

내장 imagegen 사용. 생성 결과를 프로젝트로 복사. 참조 순서: Bg_Concept → CaveEntrance_v01.

```text
Use case: stylized-concept. Transparent PNG 1536x768. Create a separate horizontal bare rock ground strip for the starting forest of a side-view 2D platformer, connecting into the cave floor of Image2. Image1 is master art direction and its foreground rock ground texture; Image2 is the approved cave entrance material match. A continuous nearly level walkable upper surface around y130 across the entire width, with only low uneven chips (10px variation), NO hills, steps or gaps. Dark angular blue-charcoal stones underneath form a substantial vertical cliff face continuing to the BOTTOM edge, not a thin floating platform. Texture sizes should match the small rocks on image2's floor, not huge rounded boulders. Upper area above the ground is true transparent alpha. Left and right ends continue to canvas boundaries for overlapping placement, no isolated island silhouette or tapered ends. Restrained moonlight upper edge, dark low-contrast bottom. This is one modular art candidate, no collider. No snow or ice (must remain a separate later layer), plants, trees, roots, props, background, character, cave ceiling, architecture, text or checkerboard. Orthographic side view, hand-painted winter fantasy texture, keep simple horizontal silhouette suitable for a calm walking approach. No promised seamless tiling needed.
```

