# AirBoss 양손 얼음 밀기 v01

2026-10-04 사용자 요청으로 화면을 향해 양손으로 얼음을 밀어내는 24프레임 멀티 스프라이트를 제작했다. 손 원근감, 각진 얼음 덩어리·파편·서리, 몸통 반동과 천의 후행을 포함한다. 00/23은 Idle 원본이며 애니메이션 클립은 만들지 않았다.

- [제작·프롬프트·검증 기록](../../../ArtSource/AirBoss/cast_botharms_icepush/Cast_BothArms_IcePush_Production_v01.md)
- [재생 미리보기](../../../ArtSource/AirBoss/cast_botharms_icepush/preview_v01.html)
- [Part1 · 00~15](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_IcePush_Part1_4x4_1024_v01.png)
- [Part2 · 16~23](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_IcePush_Part2_4x2_1024_v01.png)

셀 1024×1024, PPU 24, Center (512,512), 시트 4096×4096 / 4096×2048. Unity 24분할·피봇·PPU·텍스처 해상도와 정적 PNG 검사를 완료했다. alpha=1 잔점 때문에 모든 alpha>0 기준 64px 완전 투명 검사 통과로 표시하지 않는다. 실제 게임 재생·발사체 연결과 최종 아트 승인은 별도다.
