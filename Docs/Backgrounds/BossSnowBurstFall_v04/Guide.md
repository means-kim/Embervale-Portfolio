# 미세 눈 조각 v04

- 요청: 눈 알갱이가 너무 크고 동그람. 큰 구형 눈뭉치를 잘고 불규칙한 조각/가루로 교체. 왼쪽 높고 오른쪽 낮은 비대칭 유지.
- 시트: Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamBurstFall_Sheet_v04.png. 4열2행8프레임, 443×443 분할, PPU70, 무압축. 이전 버전 보존.
- 클립: Assets/05.Animation/Snow/BossSlamBurstFall_v04.anim. v03 타이밍 유지, 0.10초 정점, 1.30초 sprite=null/alpha0, 반복 꺼짐. 같은 오브젝트의 SpriteRenderer를 대상으로 하는 Animator 상태 Motion에 연결.
- 씬/프리팹/컨트롤러 변경 및 게임 스크립트 추가 없음.
- 별도 Unity PreviewScene에서 Animator 프레임/정점/하강/소멸 확인. 실게임 보스 타격 연동은 별도.
- 내장 image_gen 제작. Prompt.txt에 프롬프트 보관.
