# 동굴 입구 뒤쪽 바위 군집 v01

2026-09-23. 사용자 합의에 따라 눈 제거 검토를 중단하고 레이아웃·오브젝트 제작으로 돌아왔다.

[바위 군집 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveRubble_v01.png), [배치 v18](StartMap_Placement_v18.html), [검토 이미지](StartMap_Placement_v18.png).

요청1536×768, 실제1774×887 RGBA, 모서리alpha0. PPU100·중앙pivot·FullRect·최대2048, 고유.meta. 작은 돌에서 오른쪽 큰 바위로 이어지는 단일 장식 군집. 눈·지면·콜라이더는 포함하지 않는다. 이동 바닥과 플레이어보다 뒤에 놓아 숲과 동굴 절벽의 시각적 연결을 보조한다.

원본 컨셉과 동굴 입구를 이미지 참조했다. 첫 합성은 절벽에 대부분 가려져 left60%/width16%에서 left52%/width18%로 바꿨다. 최종 top78%, HTML 밝기80%로 합성했다. PNG 밝기는 그대로다. 입구 앞 위치의 캐릭터 가시성과 바위 표시 토글을 확인했다. 바위는 플레이어를 가리지 않지만 실제 카메라에서 장애물처럼 오인되는지는 추가 확인이 필요하다.

동굴 가로 위치를 조절하면 검토 화면에서 바위도 상대 위치+6%로 함께 움직인다. 실제 Unity에서 두 소재는 독립 스프라이트이며 함께 또는 각각 배치할 수 있다. 장식 바위에 자동 충돌을 부여하지 않는다. 실제 씬, 코드, 충돌, 카메라 설정은 변경하지 않았다. 최종 접지·구역 연결은 미검증이다.

## 실제 프롬프트

내장 imagegen 사용. 참조: Bg_Concept → CaveEntrance_v01. 생성 결과를 프로젝트로 복사했다.

```text
Use case: stylized-concept. Create one separate transparent PNG 1536x768 sprite of a LOW ASYMMETRIC CLUSTER of weathered angular rocks for a 2D side-scrolling winter forest cave approach. Image1 is master art palette, Image2 is existing cave entrance whose stone material must match. This cluster sits BEHIND the player walking plane and softens the transition from open forest to right-hand cliff. Side elevation orthographic hand-painted appearance. Mostly small loose rock chips on left, 3-4 overlapping modest jagged rocks rising on right, peak around 40 percent canvas height; cluster base around 85 percent canvas height. Width 80 percent of canvas with transparent margins. Flat-ish contact base, no broad ground slab. Dark charcoal-blue muted material and sparse cool upper-left moonlit highlights, low contrast so it doesn't look like a bright obstacle. Not a tall wall or platform, no circular doorway, NO snow or ice (separate later), no trees, roots, plants, characters, ground, terrain strip, landscape, particle or text. Genuine alpha transparent background. Keep jagged stratified stone matching cave, avoid smooth round boulders or symmetrical pyramid.
```

