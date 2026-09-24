# 종유석 지면 충돌 파괴 효과 v01

[4프레임 재생 검토](ClimbMap_StalactiteBreak_v01.html) · [시트 PNG](../../Assets/04.Sprites/Backgrounds/ClimbMap/FX_Climb_StalactiteBreak_v01.png) · [프레임/피벗 데이터](ClimbMap_StalactiteBreak_v01.json) · [기존 낙하 종유석](ClimbMap_Stalactite_v01.md).

## 제공

FX_Climb_StalactiteBreak_v01.png+.meta. 내장 image_gen 사용. 요청1536×1536, 실제1254×1254 RGBA.2×2, 각627×627의4키프레임. 읽는 순서는 좌상→우상→좌하→우하. Sprite Multiple,PPU100,FullRect,Max2048,무압축,Bilinear,mipmap꺼짐,자동PhysicsShape꺼짐. 기존 파일 변경 없음.

프레임 이름00_Impact,01_Burst,02_Settle,03_Debris. 피벗을 프레임별 실제 바닥 접촉에 맞췄다. Unity rect는 좌하단 기준이며 JSON의anchor는 각 프레임 좌상단 기준이다. 자동 재슬라이스/중앙pivot 적용 시 정렬 보정이 사라지므로 동봉.meta를 유지한다. 그림을 개별PNG로 다시 편집한 것이 아니라 한 시트의 Sprite rect를 분리했다.

## 구현 연결 기준

1. 낙하 중에는 기존 BG_Climb_FallingStalactite_v01을 사용한다.
2. 지면에 닿는 순간 낙하 이미지를 숨기고 접촉 위치에 파괴 효과를 표시한다. 기존 낙하 스프라이트는 상단pivot, 새 효과는 지면pivot이므로 같은 Transform.position에서 단순 Sprite 교체하면 위치가 어긋난다. 별도 시각 오브젝트로 표시하거나 위치를 보정한다.
3.00→01→02→03을 한 번 재생한다. 제안 시간은80/100/140ms, 잔해400ms이상. Unity에서 원하는 타이밍으로 설정한다. 미리보기는03까지 재생 후 계속 유지한다.
4. 마지막 잔해는 유지하거나 alpha를 줄여 제거한다. 완전히 사라지는 투명 프레임이나 별도 파편 물리 스프라이트는 제공하지 않았다.
5. 피해와 물리 판정, 파괴 중 재충돌 방지, 사운드 등은 사용자의 구현 범위다. 이 자료는 .anim/Animator/게임코드/프리팹/씬을 생성하지 않았다.

원본과 파괴 시트는 투명 여백과 그림 배율이 다르므로 같은Scale로 쓰면 안 된다. 현재 검토 종유석은 전체너비80 HTML단위, 보이는 길이약104단위. 파괴시트00의 알파128 높이약546px이므로 프레임 표시너비 약120단위부터 맞춘다. PPU100,HTML100단위=1월드단위 예시에서 원본Scale0.078125, 파괴효과Scale약0.1914. 최종 캐릭터 크기에 맞춰 시각 확인한다. 원본에서 분리된 동일 메시의 물리 시뮬레이션이 아니라 손그림4키프레임이라 파편 모양의 완전한 동일성은 보장하지 않는다.

## 확인

PNG RGBA1254×1254와 프레임별 alpha128 윤곽 측정.4개 rect가 시트 안에 있고 서로 겹치지 않도록 설정했다. 검토본에서 모든 프레임, 바닥 기준선 정렬, 재생 종료 후 잔해 표시를 확인했다. Unity 실제 import/애니메이션 재생/충돌 기능은 아직 검증하지 않았다. 낮은 프레임 수의 빠른 파괴 표현으로 사용하며 추가 중간 프레임은 현재 범위에 포함하지 않는다.

## 생성 프롬프트 원문

참조: Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_FallingStalactite_v01.png.

Create a production 2D sprite animation sheet showing THIS referenced stone stalactite shattering upon ground impact. Match its dark charcoal blue rock material, angular detail and subtle cold highlights. Genuine transparent RGBA. Square1536x1536 requested, exactly2 columns by2 rows, FOUR equal square frames, no separators no labels. Read order top-left, top-right, bottom-left, bottom-right. CRITICAL same fixed side camera, SAME scale and impact ground anchor in every cell: centerX50%, ground contactY82% of each cell. Invisible ground; no floor or scenery. Keep all particles and rock inside each cell with at least8% transparent margin. Frame1: immediate impact, pointed stalactite already splitting into4 chunky angular pieces, narrow vertical silhouette height65%cell, tiny stone powder at base. Frame2: same recognizable pieces burst outward and upward from impact, broad asymmetric spray about70%cellwidth, compact grey-blue stone dust at base. Frame3: same pieces falling and bouncing close to ground, dust fading, spread samewidth. Frame4: same broken pieces resting as a low pile on ground line, no floating particles, almost no dust. Ground anchor and object scale consistent throughout, no intact replacement stalactite regrowth. No ice, no snow, no magic, no flames, no flash, no text, no cast background shadow. Transparency outside rock and local translucent dust. Four clean handpainted game animation keyframes.
