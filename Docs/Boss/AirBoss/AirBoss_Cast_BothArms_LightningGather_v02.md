# AirBoss 양손 번개 응축 v02 — 천·몸통 반응과 1024 셀

2026-10-04 사용자 피드백을 반영해 천의 바깥 흐름·안쪽 말림·복귀 후행과 몸통/고개의 작은 반응을 추가했다. 팔 모으기 흐름과 24프레임을 유지하며 00/23은 Idle 원본이다. 애니메이션 클립은 만들지 않았다.

- [제작 사양·수정·프롬프트·검증 기록](../../../ArtSource/AirBoss/cast_botharms_lightninggather/Cast_BothArms_LightningGather_Production_v02.md)
- [전후 비교 미리보기](../../../ArtSource/AirBoss/cast_botharms_lightninggather/preview_v02.html)
- [Part1 · 00~15](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_LightningGather_Part1_4x4_1024_v02.png)
- [Part2 · 16~23](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_LightningGather_Part2_4x2_1024_v02.png)

셀 1024×1024, PPU 24, Center (512,512), 두 시트 4096×4096 / 4096×2048. Unity에서 24개 Sprite와 실제 해상도를 확인했다. v01의 추가 64px 패딩을 넣지 않아 셀이 작아졌으며, 여백 변화 자체가 몸체를 축소한 것은 아니다. 보이는 그림에는 여유가 있지만 alpha=1 잔점이 있어 모든 alpha>0에 대한 64px 완전 투명 통과로 표시하지 않는다.

정적 이미지와 임포트 검증 완료. 자동 브라우저 로컬 파일 정책으로 연속 HTML 재생 검증은 미실시, 실제 게임 재생과 최종 아트 승인은 별도다. 이전 v01은 보존했다.
