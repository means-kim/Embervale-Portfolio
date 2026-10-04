# AirBoss 양팔 얼음 수직 투척 · 32프레임

2026-10-04. 기존 BothArms Raise를 참조해, 중심 결정 1개와 보조 결정 3개를 머리 위에서 제어하다 수직 아래로 던지는 멀티스프라이트를 만들었다. 00/31은 Idle 원본, 01~11 상승, 12~15 최고점 유지, 16~21 하강·투척, 22~30 복귀다.

- [실제 파일·제작 지침·프롬프트·검증 기록](../../../ArtSource/AirBoss/cast_botharms_icethrow/Cast_BothArms_IceThrow_Production_v01.md)
- [동일 FPS 미리보기](../../../ArtSource/AirBoss/cast_botharms_icethrow/preview_v01.html)
- [32장 전체 비교](../../../ArtSource/AirBoss/cast_botharms_icethrow/contact_final_v01.jpg)

1024×1024 셀, 4×4·4096×4096 PNG 두 장. Unity Multiple 총32개, PPU24, Center(512,512), Rect 확인. 첫·끝 Idle RGBA 동일. 별도 애니메이션 클립·Animator·씬·게임 코드 변경 없음.

눈에 보이는 그림은 사방64px 여백에 들어오지만 생성 프레임의 바깥 여백에 알파1/255 미세 픽셀이 남아 있어 엄격한 모든alpha>0 여백 검사는 미충족이다. 실제 Unity 재생·게임 화면 품질도 별도 확인이 필요하다.
