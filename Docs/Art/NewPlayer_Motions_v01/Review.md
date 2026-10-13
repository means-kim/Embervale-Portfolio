# 플레이어 모션 v01 — 제작 및 적용 결과

2026-10-13. 사용자가 나머지 모션을 승인했고, 일반 점프는 **Rise → Apex → Fall**로 확정했다. Start·Land는 활성 Animator에서 제외했다. 원본 이미지와 해당 원본 클립은 보존했다.

[모션 재생 화면](Motions_Review.html) · [전체 비교 이미지](Overview.png) · [현재 적용 목록](FinalManifest.json)

## 적용 상태

새 모션 **13종, 248프레임**. 승인된 Idle·Run을 합치면 **15종, 296프레임**이다. 새 PNG는 [NewPlayer/Motions_v01](../../../Assets/04.Sprites/NewPlayer/Motions_v01)에 있다.

| 이름 | 이전 고유 프레임 | 적용 프레임 | 역할 / 제작 방법 |
|---|---:|---:|---|
| Idle | 승인본 32 | 32 유지 | 고정 자세, 국소 불꽃 변형 |
| Run | 승인본 16 | 16 유지 | 달리기 몸동작 |
| JumpRise | 8 | 32 | 상승 자세 유지, 불꽃만 변형 |
| JumpApex | 4 | 8 | 올라간 무릎을 내리고 팔을 올려 Fall로 연결 |
| Fall | 8 | 32 | Apex 마지막 자세를 원화로 채택, 불꽃만 변형 |
| DoubleJump | 16 | 16 | 몸 말기 원화 + 동일 자세 회전 + 펼치기 |
| Dash | 8 | 16 | 수평 돌진 자세, 불꽃 흐름 |
| WallCling | 8 | 32 | 두 손으로 매달린 자세 유지, 불꽃만 변형 |
| Glide | 8 | 32 | 불꽃 활공막과 몸의 불꽃 변형, 손·얼굴 고정 |
| Hit | 4 | 8 | 뒤로 젖혔다가 회복 |
| FirstAttack | 8 | 12 | 전방 대각선 베기, 칼날 회수 |
| SecondAttack | 15 | 16 | 올려베기 |
| ThirdAttack | 15 | 16 | 머리 위에서 내려베기 |
| HorizontalAttack | 15 | 16 | 전방 수평 베기 |
| FireBallAttack | 9 | 12 | 손바닥 방출 → 반동 → 회수 |

Death는 기존에 연결된 모션이 없어 이번 제작 대상에 추가하지 않았다. 미사용 WallSlide·WallClimb·WallJump 등으로 범위를 확장하지 않았다.

## 현재 Unity 경로와 연결

진행 도중 프로젝트가 정리되어 최신 경로를 다시 확인했다. 기존 GUID와 연결은 유지했다.

- 이동·점프·피격: [Player_Motion](../../../Assets/05.Animation/Player_Motion)
- 공격 몸동작: [Player_AttackMotion](../../../Assets/05.Animation/Player_AttackMotion)
- 컨트롤러: [Player.controller](../../../Assets/05.Animation/Player_Motion/Player.controller)
- 공격 데이터: [Player](../../../Assets/07.Datas/Player)

공격 이름은 FirstAttack / SecondAttack / ThirdAttack / HorizontalAttack / FireBallAttack으로 연결되어 있다. 이미 정리된 파일 이름과 현재 데이터 값을 그대로 사용했다. 공격 프리팹·데미지·체력 비용·오프셋·선딜·회복 시간은 **적용 직전 상태와 비교해 동일**하다. 클립의 원래 공격 재생 길이와 Animator state speed도 보존했다.

일반 점프는 즉시 Rise로 들어가며, Apex 프레임 진행은 수직 속도에 연동된다. 착지 후에는 Land 표시나 대기 잠금 없이 Idle/Run으로 돌아간다. DoubleJump는 별도로 유지하며 표시 시간에 맞춰 전체 회전을 재생한다. 기존 차지 점프의 Dash 표시도 새 Dash 프레임을 사용한다.

## 제작 방법과 실제 사양

내장 image_gen을 사용했다. 프롬프트는 이 폴더의 `Prompt_*.txt`, 더미는 `*_PoseGuide.png`, 생성 결과는 `*_Generated.png`, 선택 보정본은 `*_Repaired.png`에 보존했다.

- 몸동작: 더미 → 캐릭터 원화 → 프레임별 분리/검토 → 필요한 보정.
- 자세 유지: 한 원화를 기준으로 국소 불꽃 변형. 마스크·검은 팔/손 보호 영역의 픽셀 변경 0, 반복 끝과 처음의 차이 0.
- WallCling의 긴 아래 다리와 Apex 팔 진행을 이미지 생성으로 보정했다. Fall은 보정된 Apex 끝 원화를 이어 사용했다.
- SecondAttack에서 팔이 얼굴을 가리는 구간은 가려진 얼굴 면적 때문에 몸이 확대되지 않도록 크기 계산을 보정했다.
- 출력 프레임은 **512×512 RGBA, PPU 200**, Bilinear, mipmap 없음, FullRect, 무압축이다.
- 생성 원본은 포즈당 1254×1254, 12프레임 시트 1448×1086, 16프레임 시트 1254×1254, 8프레임 시트 1774×887이다. 분리 후 512 프레임에 맞춘 것으로, 모든 프레임이 원래부터 512 해상도로 생성된 것은 아니다.
- 토르소 기준 피벗을 유지한다. 프레임 안에서 큰 활공막/칼날 여백을 확보한 경우 그림과 피벗을 함께 이동해 월드 기준 몸 위치를 보존했다.
- Rise/Fall/WallCling/Glide는 각 1.6초 반복, Dash는 0.2초 반복이다. 공중 체류 시간은 게임 로직이 결정한다.

## 확인한 것

- [Unity 적용 검사](UnityApplyVerification.json): 248개 스프라이트의 크기·PPU·피벗, 13개 클립, GUID·이벤트·공격 데이터 보존.
- [Play 바인딩 검사](PlayBindingVerification.json): 승인 Idle/Run 포함 296개 고유 프레임 모두 실제 SpriteRenderer에 표시됨. 점프 단계 선택, DoubleJump 진입, 차지 발사 Dash 참조 통과.
- [YuchanScene Play 검사](ScenePlayVerification.json): 11.5초 동안 실제 물리·플레이어 입력 이벤트를 사용. 일반 점프 Rise → Apex → Fall → Idle, 2단 점프, 활공 진입/해제, 대시, 공격 5종 시작 및 공격 인스턴스 생성 확인. 1259개 관측, 오류 0, 최종 Idle/접지 상태.
- 실제 카메라 캡처: [Apex](Unity_JumpApex.png), [FireBallAttack](Unity_FireBallAttack.png), [Glide](Unity_Glide.png).
- [파일 보존 검사](FileVerification.json): 원본 Player·Boss·승인 Idle/Run·기준 원화의 757개 파일 변경 0. 씬·플레이어 프리팹도 이번 적용에서 변경하지 않았다.
- [최종 Editor 상태](FinalEditorStatus.json): 컴파일 실패 없음, Play 종료, YuchanScene 미저장 변경 없음.

키보드 자체 입력 경로, 실제 벽 접촉 중 WallCling, 전체 레벨 이동과 최종 손맛은 이번 검사에 포함하지 않았다. WallCling/Hit은 실제 Animator의 모든 프레임 바인딩을 확인했다.

## 보존과 재현

`Before/`는 제작 시작 시점, `ResumeBefore/`는 경로 정리 이후 적용 직전의 클립·데이터 사본이다. 다른 작업에서 변경 중인 플레이어 스크립트는 되돌리지 않았다. 해당 파일 차이는 `FileVerification.json`의 `externalChangesSinceResume`에 분리 기록했다.

최종 적용 데이터는 `FinalManifest.json`이다. 제작용 Moving/HeldManifest의 초기 경로보다 이를 우선한다. Apply.cs는 현재 컨트롤러 GUID와 state.motion에서 클립을 찾아 적용한다.

미리보기는 HTML을 직접 열거나 `node Docs/Art/NewPlayer_Motions_v01/serve-preview.cjs` 후 로컬 포트 8794에서 볼 수 있다. Unity 적용 도구는 Docs 아래의 일회성 eval 파일이며 Assets에 별도 테스트 컴포넌트를 추가하지 않았다.

