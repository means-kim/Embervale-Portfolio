# Boss06 조명 머티리얼 적용 — 2026-09-30

승인 범위: HyunseoScene의 06_Boss/Background 및 Terrain 하위 전체 표시 소재.

- 총 19 MeshRenderer. 기존 UpperSky Lit 1개 유지, 18개 변경.
- Boss06 전용 머티리얼 14개를 Assets/08.Materials/Map/LightShaders/Boss06에 생성. 기존 공용 머티리얼 변경 없음.
- 기존 PortalMask → PortalMaskLit2D. 기존 UnlitAlpha(바닥/눈) → 새 MeshLit2D. 원래 가림이 없던 소재에는 가림을 새로 추가하지 않음.
- 원본 텍스처, 메시, Transform, Sorting Layer/Order, 활성 상태, Collider 및 게임 코드 변경 없음. 씬 저장 완료. 사용자가 만든 기존 Shader Graph/테스트 머티리얼 보존.
- 임시 Preview Scene에서 실제 메시/소재를 개별 렌더. Global Light2D 광도 1과 0.2로 비교해 19/19 소재에서 유효 알파 및 밝기 감소 확인. 임시 씬/광원/카메라는 테스트 후 제거.
- 실제 Play 동선, 전체 구역의 겹침, 보스방 게이트 왕복은 이번 검증 범위에 포함하지 않음.
- 현 씬 조회 시 활성 Light2D는 Moon_Light 하나이며 GA1_BG_Far와 GA1_World 포함. 기본 조명이 없으면 광원 범위 밖은 어두울 수 있음. 기존 조명 값 변경 없음.
- 달 이미지도 사용자 요청의 Background 전체 범위에 포함해 Lit 처리함.
