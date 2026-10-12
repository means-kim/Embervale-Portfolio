# NewPlayerJump 정렬 및 PlayerJump 프리팹 적용

2026-10-12. 사용자 승인 범위: 이미지 중심 정렬, 애니메이션 생성, PlayerJump 프리팹 연결.

- PNG: Assets/04.Sprites/VFX/NewPlayerJump.png, 1774×887 → 2048×1024. 기존 그림의 픽셀을 이동했으며 재생성/늘이기 없음.
- 4열×2행, 총 8개 512×512 셀. 원래 발화점 1~4번 y429, 5번 y789(셀 내부345) 기준을 정렬. 소멸 단계는 후반 프레임 공통 좌표로 이동.
- 공통 피벗 (256,82), 정규화 (0.5,0.16015625), PPU100, 압축 없음. 이미지·텍스처 GUID 및 기존 0~7 spriteID 유지. 불필요한 8~27 슬라이스 제거.
- Assets/05.Animation/VFX/PlayerJumpFire.anim: 24fps, 8프레임. 0.333333초에 빈 Sprite, Unity에서 clip.length 0.375초. 반복 없음.
- PlayerJumpFire.controller 기본 상태 연결. PlayerJump.prefab에 Animator 추가. 기존 AudioSource/OneShotVFX/VFXOffset(-0.5)/머티리얼/Sorting 유지.
- 원본 및 적용 전 prefab/meta는 BeforeAlignment에 보존. 기존 씬/Player 프리팹 및 사용자 삭제 파일 미수정.
- 검증: Unity 6000.3.22f1 에디터 샘플링 8프레임 순서·공통 피벗·마지막 빈 Sprite 확인. Play Mode에서 별도 인스턴스의 실제 0~7 프레임, 빈 Sprite, Animator 재생, 약0.626초 자동 파괴 관찰. 검증 종료 후 Edit Mode 복귀.
- 실제 플레이어 점프 중 발 위치/게임 카메라에서의 크기와 최종 타이밍은 별도 시각 확인 필요. 현재 YuchanScene/MainTestScene의 EffectManager는 이 프리팹 GUID를 참조함; 다른 씬의 연결 상태는 변경하지 않음.

