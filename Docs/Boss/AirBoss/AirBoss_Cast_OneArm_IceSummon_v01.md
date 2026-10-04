# AirBoss 단팔 상승 얼음 소환 v01

2026-10-04. 사용자 요청에 따라 손 궤적이 포함된 24프레임 바디 멀티스프라이트를 built-in image_gen으로 제작했다.

- [제작·수정·검증 상세](../../../ArtSource/AirBoss/cast_onearm_icesummon/Cast_OneArm_IceSummon_Production_v01.md)
- [전체 프레임과 재생 미리보기](../../../ArtSource/AirBoss/cast_onearm_icesummon/preview_v01.html)
- [프롬프트 기록](AirBoss_Cast_OneArm_IceSummon_Prompts_v01.json)
- [시트 00~15](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_IceSummon_Part1_4x4_1152_v01.png)
- [시트 16~23](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_IceSummon_Part2_4x2_1152_v01.png)

실제 생성 원본 1254×1254 → 전체 캔버스 1024 정규화 → 공통 64px 투명 패딩 → 1152 셀. 체격별 fit·Trim·클리핑 없음. 00은 마스터 RGBA를 중앙에 그대로 보존. 새 시트 실제 4608×4608 및 4608×2304, Unity 24 Sprite·PPU 24·Center 확인. 전 셀 64px alpha 안전 여백·시트 재추출 일치 검사 통과.

이번 결과는 검토용 v01이다. 정적 순서 비교와 일부 중간 포즈 보완을 수행했으며 프레임별 손·갑옷·천의 미세 차이, 최종 속도와 게임 화면 품질은 사용자 검토 대상이다. 클립·Animator·씬·프리팹·게임 코드 및 기존 보스 스프라이트는 변경하지 않았다. 독립 얼음 VFX는 포함하지 않았다.
