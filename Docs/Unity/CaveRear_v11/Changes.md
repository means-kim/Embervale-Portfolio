# 기존 입구 뒤쪽 외관

- 기존 아치/고드름/눈/바닥의 형상과 Transform을 유지하고 Cave_ExteriorRearRock_v11을 World order27에 추가. 아치(order28) 뒤에서 보이는 외부 전용 장식이며 충돌 없음.
- 기존 ExteriorMountain_v01의 오른쪽 암반 영역을 사용. 새 PNG 생성 없음. 이전 전면 덮개들은 계속 비활성.
- PortalMask에 기본값0인 _MouthRearReveal 추가. 현재 입구에만 복제 머티리얼 Cave_EntranceRearReveal_v11로 활성화. 외부일 때 입구 Mouth 영역의 뒷부분만 알파를 줄여 뒤쪽 암벽이 보이게 함. 원래 아치 픽셀은 유지.
- 뒤쪽 열림은 기존 포털 X+2.5~6.5 범위에서 보간. 새 암벽은 entranceRole5로 진입 후 투명. 내부/충돌/전환 임계값 변경 없음.
- 현재 씬과 CompleteLayout/CaveUpperBoundary 프리팹 저장. 단독 기존 입구 프리팹은 수정하지 않음.
- 5개 카메라 렌더 기록. BEFORE 대비 내부2/3 및 상단4의 해시 비교를 수행. 실제 연속 이동 테스트는 미실시.
- 롤백: 새 rear rock 비활성/Surfaces 제거, 입구를 원래 머티리얼로 복구. 셰이더 새 옵션은 기본0이므로 남겨도 기존 동작 유지. 작업 전 씬/프리팹/셰이더는 Before에 보존.
