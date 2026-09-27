# 보스 눈 효과 비대칭 v03

- 사용자 요청: 더 비대칭적으로. 왼쪽 높고 두꺼운 분출, 오른쪽 낮고 넓은 확산. 후반에도 좌우 눈 덩어리 높이와 밀도 차이 유지.
- Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamBurstFall_Sheet_v03.png: 1774×887 RGBA, 4열2행8프레임, 443×443 분할, PPU70. v02 크기/타이밍 유지.
- Assets/05.Animation/Snow/BossSlamBurstFall_v03.anim: 0.10초 정점, 1.30초 sprite=null/alpha0, 반복 꺼짐. SpriteRenderer와 Animator가 같은 오브젝트에 있어야 한다. 상태 Motion을 이 클립으로 교체한다.
- 원본 행별 기준선 보정 피벗: 위행(.5,.04), 아래행(.5,.135).
- 새 PNG와 클립만 추가. 기존 씬/프리팹/컨트롤러 변경 없음. 게임 스크립트 없음.
- 별도 Unity PreviewScene에서 Animator로 정점/낙하/최종 소멸 확인. 실제 보스 타격 연결은 미검증.
- 내장 image_gen 사용. Prompt.txt에 제작 프롬프트 보관.
