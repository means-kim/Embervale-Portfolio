# 플레이어 글라이더 v01

2026-10-12. 사용자 요청에 따라 스프라이트, 애니메이션, 공용 Player 프리팹과 활공 코드를 연결했다.

## 사용

스페이스 첫 번째 입력은 점프, 두 번째는 공중 점프, 그 이후 입력은 활공 시작이다. 활공 중 다시 스페이스를 누르면 종료한다. 버튼을 누르고 있기만 해서는 발동하지 않는다. 좌우 이동 입력과 방향 전환은 유지한다.

바닥 또는 벽(Ground/Platform의 실제 접촉 또는 벽 탐지)에 닿으면 종료한다. 착지까지 공중에서 다시 켤 수 있다. 벽에 매달린 상태에서는 기존 벽 점프 입력을 우선한다. 피격, 사망, 대시, 차지 점프, 공격 전환, 리스폰, 컴포넌트 비활성화, 앱 포커스 상실에서도 활공을 정리한다. 활공 입력은 점프 버퍼에 들어가지 않는다.

Player 루트의 **Player Glide**에서 Horizontal Speed Multiplier와 Max Fall Speed를 조절한다. 기본값 1.5 / 5. 공용 프리팹의 이동 속도 20 기준 활공 수평 속도는 30이다. 하강 속도 제한은 초당 월드 단위이며, 상승 중에는 기존 상승 속도를 유지한다. Rigidbody2D 중력 설정 자체는 변경하지 않는다.

## 파일과 이미지 제작

- [원본 시트](../../../Assets/04.Sprites/Player/Glide_v01/Player_Glide_v01.png): 내장 image_gen으로 제작. 기존 Idle Master512와 Fall 시트를 이미지 참조로 사용.
- [실제 프롬프트](Prompt.txt): 불꽃 정령의 정체성과 양손, 얼굴을 유지하고 불꽃 글라이더를 추가. 4×2 배열, 8프레임, 미세한 불꽃·덮개 움직임.
- 요청 2048×1024 / 실제 PNG **1772×886**, 셀 **443×443**, 투명 RGBA. 실제 생성 해상도를 512 프레임으로 표현하지 않는다.
- Unity PPU 200, Multiple Sprite, Bilinear, 무압축, mipmap 없음. 기존 Fall과 얼굴 중심을 비교하여 프레임별 피벗을 정렬했다. Transform Scale은 변경하지 않았다.
- [Glide.anim](../../../Assets/05.Animation/player/Glide.anim): 12fps, 8프레임 반복. Player.controller의 Glide 상태에서 재생.
- [PlayerGlide.cs](../../../Assets/02.Scripts/Player/Movement/PlayerGlide.cs): 별도 컴포넌트. PlayerInput / PlayerMove / PlayerController / PlayerAnimator / PlayerState 및 대시·차지 점프·리스폰 연결.
- [Player.prefab](../../../Assets/03.Prefabs/Player/Player.prefab)에 컴포넌트 저장. 현재 YuchanScene의 Player 프리팹 인스턴스에서도 연결 확인. 별도 Hyunseo 테스트 플레이어 코드는 수정하지 않았다.

## 검증

[Play 모드 검사 기록](Verification.txt): Unity 6000.3.22f1, 공용 프리팹을 분리된 임시 PhysicsScene2D에 복제하여 21개 항목 통과. 입력 라우팅과 같은 메서드를 호출하고 실제 물리 시뮬레이션과 충돌 콜백을 실행했다. 키보드 하드웨어 입력으로 현재 씬 전체를 완주한 검사가 아니다.

측정 수평 속도 30.000 / 하락 속도 -5.000. 첫·두 번째 점프, 세 번째 스페이스 발동, 재입력 종료, 재발동, 좌우 조작, 속도 복구, 실제 벽·바닥 충돌 종료, 착지 후 오발 점프 방지, 대시·차지 점프·피격·사망·리스폰·비활성화 종료, 실제 Animator 상태 및 8개 고유 프레임을 검사했다.

[기존 Fall](Fall.png), [활공 오른쪽](Glide_Right.png), [왼쪽](Glide_Left.png), [중간 프레임](Glide_Frame4.png)은 동일 카메라·PPU로 Unity에서 렌더했다. 캡처에는 아트 비교용 임시 Unlit 머티리얼을 사용했다. 현재 씬 조명 아래 전체 이동의 최종 시각 검증은 별도다.

컴파일 오류 없음. 테스트와 캡처 임시 오브젝트 0개, Play 모드 종료, YuchanScene 미저장 변경 없음을 확인했다. 기존 사용자 변경·삭제 파일은 보존했다. 검증 도구 Setup.cs / Verify.cs / Capture.cs는 Docs 안에 보관해 게임 어셈블리에 포함되지 않는다.
