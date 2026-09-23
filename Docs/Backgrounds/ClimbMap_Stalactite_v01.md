# 낙하 종유석 아트 v01

[배치 검토](ClimbMap_Stalactite_v01.html) · [배치 캡처](ClimbMap_Stalactite_v01_Placement.png) · [낙하선](ClimbMap_Stalactite_v01_Guide.png) · [배치 데이터](ClimbMap_Stalactite_v01.json).

## 소재

[BG_Climb_FallingStalactite_v01.png](../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_FallingStalactite_v01.png)와 고유.meta. 내장 image_gen 사용. 요청/실제1024×1536 RGBA. PPU100, custom pivot(0.5,0.92), FullRect, Max2048, 무압축, Clamp, mipmap/Trilinear. 자동 Physics Shape는 비활성. 천장·뒤벽과 합쳐지지 않은 돌 종유석 한 개다. 얼음·파편·바닥 없음.

## 배치

등반 단독HTML 좌상단X565,Y60,너비80,높이120,밝기1. 수직 낙하 중심X605, 상단exit 발판까지 참고선Y180~370. 화면당 실제 보이는 크기는 투명 여백을 제외한 실루엣 기준으로 확인했다. 길이는 캐릭터보다 길고 폭은 한 캐릭터 정도로 읽힌다. 위치는 플레이 검증 전 제안이다.

Unity 예시: HTML100단위=1월드단위, 좌상단원점(0,0), Y반전일 때 Scale0.078125. 상단pivot 위치는(6.05,−0.696). 높이120의 상단8% 지점이pivot이다. 최종 월드 크기/캐릭터Scale을 확정하는 값은 아니다. 천장 부착부에 상단을 조금 겹치고 종유석은 독립 오브젝트로 유지한다. 동일한 PNG를 낙하 전·중에 사용할 수 있다. 캐릭터 뒤/장식벽 앞에 보이도록 정렬하되 실제 플레이 피격 가독성을 확인한다.

미래 함정 동작용 물리 오브젝트에 종유석 SpriteRenderer를 붙이거나 시각 자식으로 두면 된다. 구현 시 패럴랙스 배경에 종속시켜 낙하축이 흔들리지 않도록 한다. 파괴 파편, 흔들림, 경고, 트리거, 충돌, 피해, 재생성은 이번에 만들지 않았다.

## 확인

천장 접합, 캐릭터 대비 크기, 예상 낙하선이exit 발판 안으로 향함을 실제 브라우저 화면에서 확인. 이미지 로드 실패0, 낙하선 토글 표시 확인. 한 개의 맵 오브젝트를 전체도와 카메라 화면에 각각 표시한 것이며 두 개를 배치한 것이 아니다. 낙하선은 고정 아트 가이드여서 발판 편집 후에는 새 위치에 맞춰 재검토해야 한다. 기존 발판·벽은 변경하지 않았다. Unity 씬 배치와 실제 낙하 기능은 미구현이다.

## 생성 프롬프트 원문

Reference1: Assets/04.Sprites/Bg_Concept.png. Reference2: Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_Ceiling_v01.png.

Create ONE separate falling stalactite hazard sprite for a 2D side scrolling cave game. Reference1 is style/mood, reference2 exact cave rock material. Genuine transparent RGBA background. Requested1024x1536 portrait. Single long downward pointing jagged tapered STONE stalactite, broad uneven broken attachment at top, narrowing to sharp bottom tip. Centered, occupies x30%-70% and y8%-92%. Dark charcoal blue rock, detailed handpainted angular fissures, faint cold blue upper left edge highlights for readable hazard silhouette. Not ice, not a cluster. Entire object independent, no ceiling slab attached, no backdrop, no floating dust, no glow, no snow, no ground, no shadow, no text. Top should fit tucked into existing ceiling. Fully opaque rock interior and clean natural alpha silhouette. This exact rock will later detach and drop vertically as one object; do not show motion or fragments.
