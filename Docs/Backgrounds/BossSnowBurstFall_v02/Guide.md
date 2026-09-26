# 보스 내려치기 눈 효과 v02
사용자 요청: 보스 내려치기에 맞춰 더 크고 강하게.

- 새 시트: Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamBurstFall_Sheet_v02.png
- 새 클립: Assets/05.Animation/Snow/BossSlamBurstFall_v02.anim
- 실제 PNG1774×887 RGBA, 4열×2행, 8프레임. 443×443 분할, 무압축, MaxSize2048.
- PPU70: 같은 Transform Scale에서 셀 월드 크기가 기존 PPU100 대비 약1.43배. 실제 눈 실루엣 비율은 프레임마다 다르므로 가시폭이 정확히1.43배라는 의미는 아니다.
- 초반 분출량과 좌우 확산 증가. 긴 꼬리를 줄이고 눈 덩어리와 가루가 떨어지도록 수정.
- 키 시간0,.04,.10,.20,.36,.56,.80,1.04초.1.30초에 alpha0 및 sprite=null. 루프 꺼짐. frameRate30은 키 편집 기준이며 그림은8장이다.
- 원본 행별 바닥 위치 차이를 상쇄하기 위해 피벗 위행(.5,.04), 아래행(.5,.135). 크기/이동 애니메이션 키 없음.
- 기존 클립 대신 새 클립을 같은 SpriteRenderer 오브젝트의 Animator 상태 Motion에 넣는다. 실제 월드 배치 크기는 부모 Scale로 조절한다.
- 기존 이미지/씬/컨트롤러/프리팹 수정 없음. 스크립트 추가 없음.
- 별도 Unity PreviewScene에서 Animator/PlayableGraph로 정점, 낙하2시점, 최종 소멸 확인. 실제 보스 타격 연결/게임 카메라에서는 미검증.
- 내장 image_gen 제작. Prompt.txt에 생성 및 여백 보정 프롬프트 보관.

