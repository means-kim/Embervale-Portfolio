# AirBoss Throw IceBlock 32프레임 제작 기록

2026-10-06. 내장 image_gen으로 제작. 최종 사양 및 검증 범위는 [ArtSource 제작 기록](../../../ArtSource/AirBoss/cast_onearm_updown_throw_iceblock/Cast_OneArm_UpDown_Throw_IceBlock_Production_v01.md)을 따른다.

- 최종 셀 1152×1152, 두 시트 각 4608×4608, Multiple 총32개, PPU24, Center576.
- 기존1024좌표의 신체를 유지하고 사방64px 공통 패딩 추가. 원래 Idle 마스터 보존.
- 왼팔(화면 오른쪽) 머리 위 홀드 후 사선 전방 투척. 손에 구체/발사체 없음.
- [제안 타이밍 미리보기](../../../ArtSource/AirBoss/cast_onearm_updown_throw_iceblock/preview_v01.html), [전체 포즈 비교](../../../ArtSource/AirBoss/cast_onearm_updown_throw_iceblock/contact_final_v01.png).
- [전체 프롬프트](AirBoss_Cast_OneArm_UpDown_Throw_IceBlock_prompts_v01.json).
- 픽셀·여백·Unity 슬라이스 검증 완료. 브라우저 동적 재생과 Unity 실제 게임 재생은 미검증. 클립·씬·프리팹·게임 코드 변경 없음.
