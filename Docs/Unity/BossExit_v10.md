# 보스 출구 양방향 가림 v10

MapReview_v02 적용, 백업 Before_BossExit_v10.unity. 공유 스크립트·셰이더 변경은 씬 백업에 포함되지 않음.

[검토 화면](BossExit_v10.html) · [상태 기록](BossExit_v10_checks.txt)

ReviewPortalVisibility의 bossExitEnabled 활성화. bossExitX=120, 양쪽 .25 여유. 통로 내부에서는 보스 OutdoorFar/UpperSky/Moon/Ruins 검정, 보스 구역에서는 BossCorridor_InteriorBack 검정. 천장과 UpperRock 외벽 및 바닥 유지. 시작 상부 하늘의 같은 텍스처 재사용은 이름 역할로 구분해 제외.

입구 접근→보스 밖→점프 높이→통로 복귀 및 시작 입구 양쪽 8곳 고정 1600×900 렌더 확인. 이동/충돌 기능 변경 없음. 이번 작업에서는 Play 이동 완주 미검증. 셰이더 로드 후 최종 콘솔 오류 0.
