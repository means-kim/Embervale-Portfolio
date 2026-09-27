# 보스 눈 분출 → 낙하 v01

- 원본: Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamUpSpray.png (사용자가 정리한 기존 v02 이름).
- 시트: Assets/04.Sprites/Backgrounds/Snow/FX_Boss_SlamBurstFall_Sheet_v01.png.
- 실제 PNG 1774×887 RGBA. 요청 크기와 다름. 원본 알파 보존.
- 4열×2행, 위에서 아래/왼쪽에서 오른쪽 순서 8프레임. 셀 443×443; 홀수 크기 때문에 경계 0~1px 여백 제외. PPU100, 피벗(.5,.05), MaxSize2048, 무압축.
- 클립: Assets/05.Animation/Snow/BossSlamBurstFall_v01.anim. 약1.13초, Loop Time 꺼짐. 1.05초에 sprite=null, alpha=0.
- 키 시간: 0 / .075 / .15 / .25 / .38 / .52 / .68 / .84초. .52초 이후 알파 감소.
- Animator와 SpriteRenderer가 같은 오브젝트에 있을 때 사용. 기존 ExplodeSnowAC에 사용할 상태로 클립을 직접 연결. 원래 이미지 비율과 다르므로 부모 Transform Scale로 실제 분출 폭을 맞춘다. 애니메이션에는 Transform Scale 키가 없다.
- 기존 원본, 씬, 프리팹, 컨트롤러는 수정하지 않음. 새 게임 스크립트 없음.
- Unity PlayableGraph + Animator로 프레임 순서/알파/마지막 숨김 확인. 별도 PreviewScene에서 5시점 렌더 확인. 보스 실제 타격 이벤트 및 실게임 크기/위치 연결은 사용자 작업.
- 렌더: Preview_0.png 정점, Preview_1.png 낙하 시작, Preview_2.png 하강, Preview_3.png 잔설, Preview_4.png 소멸.
- 미리보기 HTML은 이미지 검토용이며 게임 구현이 아님.
- 내장 image_gen 사용. 생성 프롬프트는 Prompt.txt.

