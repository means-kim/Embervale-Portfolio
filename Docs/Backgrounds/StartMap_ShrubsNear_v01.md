# 시작 숲 낮은 관목 v01

2026-09-23. 레이아웃과 배치 오브젝트 제작의 다음 소재로 시작 숲의 마른 관목 한 군집을 제작했다.

[관목 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_ShrubsNear_v01.png), [합성 v19](StartMap_Placement_v19.html), [검토 이미지](StartMap_Placement_v19.png).

요청1536×768, 실제1774×887 RGBA, 모서리alpha0. PPU100·중앙pivot·FullRect·최대2048, 고유.meta. 가지 군집이 한 PNG에 포함된다. 눈·바위·지면·콜라이더는 없다. 원본 컨셉의 시작 구역 마른 가지와 기존 동굴 바위 색을 참조했다. 생성본의 중앙 가지가 상대적으로 높아 배치 크기를 낮게 잡았다.

합성 후보: width12%, left24%, top82.2%, HTML brightness(.8). PNG 색상은 보존했다. 관목을 지면·눈·캐릭터보다 뒤에 두고 하단을 바닥에 묻었다. 기존 컨셉 캐릭터 비율을 유지한 채 관목 앞 위치의 가시성과 표시 토글을 확인했다. 작은 화면에서는 가느다란 가지가 은은하게 보이며 실제 카메라 해상도에서 가독성을 재검토해야 한다. 날카로운 가지가 위험물처럼 읽히는지도 실제 배치에서 확인한다.

이 소재는 장식이고 발판·가시·장애물로 확정하지 않았다. 기존 오른쪽 고목 제외, 동굴 진입 방향을 유지한다. Unity 씬·코드·게임 동작·충돌은 변경하지 않았다. 실제 플레이 접지·카메라·다음 구역 접합은 미검증이다.

## 실제 프롬프트

내장 imagegen 사용. 참조: Bg_Concept → CaveRubble_v01. 프로젝트에 생성 PNG를 복사했다.

```text
Use case: stylized-concept. Transparent PNG 1536x768, a single sparse LOW DEAD SHRUB CLUSTER for the near background of a side-view 2D winter forest platformer. Image1 master concept: reference the delicate leafless twigs beside the player in the left starting area, not the giant tree. Image2 palette reference for existing dark blue rocks. Several thin crooked woody stems branching in irregular directions, 2-3 small clumps with clear airy gaps, one slightly taller off-center stem, graceful natural tapering twigs. Overall wide low silhouette occupying x150..1400,y280..680, baseline y680. Keep fine branches readable at small display size without making thorn spikes. Dark desaturated blue-charcoal wood, very restrained cool moonlight on upper-left edges, painterly organic texture matching concept. These are harmless decorative shrubs BEHIND the player, not a hazard fence or tree wall. NO leaves, snow, ice, roots mass, rocks, ground strip, background, grass carpet, giant tree, architectural object, character, text, checkerboard. Genuine alpha transparent backdrop and gaps, no cast-shadow plane. Isolated shrub cluster only.
```

