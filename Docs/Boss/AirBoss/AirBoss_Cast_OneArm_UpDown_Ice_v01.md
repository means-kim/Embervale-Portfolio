# AirBoss 한팔 얼음 시전 32프레임 v01

2026-10-04. 기존 OneArm UpDown의 얼음 변형 멀티스프라이트를 제작했다. 애니메이션 클립은 만들지 않았다.

- [제작·사양·프롬프트·검증 기록](../../../ArtSource/AirBoss/cast_onearm_updown_ice/Cast_OneArm_UpDown_Ice_Production_v01.md)
- [프레임 미리보기](../../../ArtSource/AirBoss/cast_onearm_updown_ice/preview_v01.html)
- [전체 프레임](../../../ArtSource/AirBoss/cast_onearm_updown_ice/contact_final_v01.jpg)

32장, 셀 1024×1024, PPU 24, 중앙 피벗. 4096×4096 시트 2장에 00~15 / 16~31을 나눠 Unity Multiple 임포트와 32개 Sprite 조회를 확인했다. 00·31은 원본 Idle과 픽셀이 같다.

alpha>2의 그림은 사방64px 여백 안에 있다. 01~30 바깥 여백의 alpha1~2 미세 점은 보존되어 엄격한 모든-alpha 여백 기준은 미충족이다. 실제 게임 재생·최종 동작 승인은 별도다.
