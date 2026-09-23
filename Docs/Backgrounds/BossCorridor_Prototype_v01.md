# 보스전 통로 실내 초안 v01

사용자의 다음 진행 요청에 따라 예정 순서인 등반→보스전 통로를 착수했다. 방향 질문에 별도 답이 없는 동안 기존 계획을 따른 가정이며 새 구역의 최종 승인으로 간주하지 않는다.

[검토 화면](BossCorridor_Prototype_v01.html) · [통로 안쪽](BossCorridor_Prototype_v01_Inside.png) · [배치 데이터](BossCorridor_Prototype_v01.json).

## 소재

Assets/04.Sprites/Backgrounds/BossCorridor/BG_BossCorridor_InteriorBack_v01.png+.meta. 내장image_gen 사용. 요청2048×1024, 실제1774×887 RGB,PPU100,중앙pivot,Max2048,무압축. 뒤벽 전용이며 전경 천장·바닥·발판·함정·보스 없음. 그림 안의 돌출바위는 원경/후면 장식이며 플레이 충돌이 아니다.

## 조립

등반 상단 일부X0~1000과 오른쪽 통로X1000~2200을 같은 좌표계로 검토. 카메라1000×562.5,캐릭터 약9%. 기존 Ceiling과Ledge를 독립 재사용. 바닥X800,Y285,폭1000과X1500,Y300,폭1000. 천장X800/1500,Y−170,폭1000. 두꺼운 반복 바닥의 단차를 줄이도록 두번째를 낮췄다. 실제 바닥이 픽셀 단위로 완전히 평평한 것은 아니다.

등반 출구 접합과 통로 안쪽을 실제 이미지로 확인, 로드 실패0. 하늘 없는 실내,색감·캐릭터 가독성 유지. 플레이어 위치는 비교를 위한 근삿값이며 실제 접지나 이동은 아니다. 원근 후면은 새 PNG, 겹치는 등반 뒤벽은 임시 HTML 마스크 사용. Unity 재현 완료가 아니다. 기존 종유석/파괴소재는 등반맵에 보존되며 이 접합 검토에는 중복 배치하지 않았다.

다음 보완: 통로의 실제 길이와 지형에 맞춘 벽/바닥 반복 접합, 보스전맵 입구와의 연결. 보스전맵과 외부 출구는 이번에 제작하지 않았다. Unity 씬·코드·함정 구현 변경 없음.

## 생성 프롬프트

참조1 Bg_Concept.png,참조2 ClimbMap/BG_Climb_InteriorBack_v01.png.

Generate horizontal interior cave rear wall background for 2D side scrolling game passage leading from climbing cave toward boss approach. Reference1 is actual world concept style only, reference2 is approved adjoining cave rear wall, match its blue charcoal palette, detailed handpainted angular rock, dim cold ambient light. Requested2048x1024 landscape. Entire image opaque rock interior and subtle shadowed depth; absolutely no sky mountains exterior windows or bright openings. A long horizontally organized recess, layered distant rock formations, restrained contrast center lower area for readable player silhouette. No foreground ceiling slab, no walkable floor, no platforms, no snow, no stalactite trap, no characters, no architecture, no sanctuary, no text. Background layer only, both side edges low contrast rocky recesses suitable overlapping neighboring indoor wall. Preserve painterly natural scale, do not add door or boss.
