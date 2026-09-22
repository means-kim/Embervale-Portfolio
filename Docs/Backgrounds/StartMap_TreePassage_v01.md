# 고목 통로 앞뒤 분리 v01

2026-09-22. 사용자가 오른쪽으로 진행하며 고목 사이를 통과할 입구가 빠졌음을 지적했다. 기존 장식용 통짜 고목 제작 방향을 수정한다. 새 눈 추가보다 통로 구조 검토를 우선한다.

## 결과물

- [뒤쪽 고목](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreePassage_Back_v01.png): 줄기 하부를 열린 뿌리 아치로 변경. 중앙 (620,1200)의 alpha 0 확인. 모서리 (0,0)는 alpha 1로 미세 잔여값이 있어 완전한 무잔여 추출로 부르지 않는다.
- [앞쪽 입구 테두리](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreePassage_Front_v01.png): 아치 상단의 가까운 뿌리만 별도 레이어. 이동대 y1220~1535 전체의 alpha 최댓값 0 확인.
- 두 이미지 요청·실제 크기 1024×1536 RGBA. 고유 .meta, PPU100, 중앙 pivot, Full Rect, 최대2048. 동일 Transform으로 등록하여 사용한다. 기존 고목·눈 PNG는 보존했다.
- [인터랙티브 합성 v11](StartMap_Placement_v11.html), [검토 이미지](StartMap_Placement_v11.png).

## 배치와 확인 범위

원경·중경 → 뒤쪽 고목 → 플레이어 → 앞쪽 테두리 순서로 합성했다. 입구 너머로 숲이 보이며, 앞쪽 테두리는 캐릭터 보행대보다 위에 놓인다. 두 고목 레이어는 서로 다른 패럴랙스를 적용하지 않는다. 기존 TreeRightSnow v01은 이전 고목 전용이므로 새 고목에는 겹치지 않는다.

HTML은 기존 Run_00 프레임을 그대로 참조한다. 프레임512/PPU200, Scale1 가정으로 256px 캔버스 크기를 사용하고 발 기준 y410을 임시 바닥선에 맞췄다. 원경1448×1086, Scale1, 고목 캔버스 너비60%로 고목 Scale 약0.8484375를 가정했다. 눈에 보이는 플레이어 높이 약1.69유닛은 참고값이다. 실제 프리팹·카메라 크기 확정이 아니다.

원본 컨셉보다 입구가 크고 아치 형태가 강조되었다. 이는 통과 구조를 읽기 쉽게 만든 후보이며 원본의 정확한 복제가 아니다. 앞 테두리와 뒤 고목은 생성된 독립 이미지라 정밀한 원본 분해본은 아니지만, 동일 좌표 확대 합성에서 연결을 육안 확인했다. 기존 고목 비교와 앞 테두리 토글을 확인했다.

점선은 임시 발 기준선이며 뿌리와 지면을 잇는 실제 바닥은 아직 없다. 고목 전체 실루엣에 자동 콜라이더를 씌우면 통과 의도와 어긋날 수 있다. 사용자가 씬을 조립할 때 통로·바닥 기준으로 충돌을 별도 설계한다. 지금은 Unity 씬·코드·콜라이더를 수정하지 않았다. 정지 합성과 위치 전환은 실제 이동·점프·충돌 검증을 대신하지 않는다. 최종 접지, 플레이 크기, 다음 등반 구간 연결은 미검증이다.

## 실제 생성 프롬프트

내장 imagegen 사용. 참조: 기존 오른쪽 고목 v02와 Bg_Concept → 새 뒤쪽 고목과 Bg_Concept. 생성한 두 PNG를 프로젝트에 복사했다.

### 뒤쪽 고목

```text
Use case: precise-object-edit. Create a production transparent sprite for a side-view 2D platformer. Image 1 is the existing right tree to redesign; Image 2 is the master winter landscape style and traversal reference. Output 1024x1536 RGBA. Preserve the dark blue-gray bark, upper tall trunk on the right side, moonlit hand-painted texture, and sparse branches of image 1. CRITICAL CHANGE: replace its solid lower trunk with a large natural hollow arch between split buttress roots, visibly open to the landscape behind it. The arch opening roughly spans x=470..850 and y=1120..1500, with an irregular rounded arch ceiling, genuine transparent empty interior, not a black painted hole. The tree's upper weight forks into two weathered root supports framing this opening. This sprite is the REAR TREE layer: all its wood will render behind a player walking horizontally left-to-right across the bottom of the opening. Keep the opening broad and readable, about 2 character heights tall. Roots must not form a horizontal bar across the opening. Keep canvas positioning of upper trunk stable. No snow, no ground, no platforms, no castle, no sanctuary, no character, no background, no text, no checkerboard. A single tree cutout with an actually transparent hollow bottom entrance. Do not turn it into a constructed doorway. Keep it ancient and organic like the large starting-area tree in image 2.
```

### 앞쪽 테두리

```text
Use case: background-extraction. Produce ONLY a separate foreground arch-rim overlay registered to Image 1, the supplied 1024x1536 hollow tree. Image 2 is palette reference. Full output canvas 1024x1536 transparent PNG. Imagine a narrow organic raised bark root tracing the UPPER edge of the hollow arch of image 1: beginning around x340 y1030, rising to x560 y900, curving to x760 y990 and ending x865 y1150. Export ONLY that near-facing raised root lip and a few short dangling bark fibers; remove ALL the remaining tree, trunk, branches, lower roots, and background to genuine transparency. Keep this lip at exactly those original canvas coordinates; do not center, crop, resize or pack the isolated fragment. The lip must attach visually to the existing arch and use the same dark desaturated blue bark. Its empty interior and ALL canvas below y1220 must be transparent, leaving the horizontal player walking lane unobscured. This is the foreground ceiling edge for a side-scrolling walk-through tree passage, NOT a full tree, NOT a door, NOT an enclosed ring. No snow, no ground, no landscape, no character, no text, no checkerboard. Single isolated curved bark lip with alpha transparency, exact full reference canvas registration.
```

