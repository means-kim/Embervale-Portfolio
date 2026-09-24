# 입구 간소화

- 내장 image_gen으로 v01의 입구 주변 작은 돌/잔해를 넓은 암반 면으로 정리. 생성본은 Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_ExteriorMountain_v02.png (1024×1536 RGBA).
- 생성본의 불필요한 외곽 알파 변화 때문에 전체 교체하지 않음. 기존 v01 전체 아트 위에 UV X0.16~0.72, Y0.015~0.285의 입구 부분만 가장자리 알파 보간된 네이티브 메시로 합성. 별도 서브메시/머티리얼로 같은 렌더러와 포털 전환을 사용.
- 외벽 전체 실루엣, Transform, 충돌, 전환 지점을 유지. 입구 주변만 형태 단순화. 기존 v01 소재와 메시 보존.
- HyunseoScene 및 CompleteLayout/CaveUpperBoundary 프리팹에 저장. BEFORE/AFTER 5개 렌더 화면 기록. 직접 플레이 테스트는 미실시.
- 롤백 시 Cave_ExteriorMountain_v01의 MeshFilter를 기존 Cave_ExteriorMountain_v01.asset으로, 머티리얼 배열을 기존 단일 머티리얼로 돌리면 됨.
