# GA1 Player Motions v4

기존 **Idle v3에 추가하는 플레이어 모션 팩**입니다. 확정된 불꽃 정령의 원화를 기준으로 달리기, 점프·착지, 몸을 말아 회전하는 더블 점프, 대시, 공격, 벽 동작, 피격을 제작했습니다.

## 달리기 경계 수정본 v4.2

기존 Run 8장은 원본 그대로 복원하고 마지막과 처음 사이에 연결 프레임 2장만 추가했습니다. 기존 v4를 사용 중이면 `README_RUN_LOOP_FIX_KO.md` 순서대로 덮어쓴 뒤 **Update Run Only**를 실행할 수 있습니다.

## 바로 사용하는 방법

1. ZIP을 풀어 `GA1_Player_Motions_v4` 폴더 전체를 Unity 프로젝트의 `Assets` 아래에 넣습니다. 기존 `GA1_Player_Idle_v3`는 그대로 사용합니다.
2. 스크립트 컴파일이 끝나면 **Tools > GA1 > Build Player Motions v4**를 실행합니다.
3. `Pixels Per Unit`을 기존 Idle과 동일하게 지정합니다. Idle v3 기본값을 사용했다면 **200**입니다.
4. Animator와 SpriteRenderer가 같은 GameObject에 있으면 `SpriteRenderer Path`는 비웁니다. Animator 아래 자식 `Visual`에 SpriteRenderer가 있다면 `Visual`을 입력합니다. 이 경우 기존 Idle 클립의 바인딩 경로도 동일해야 합니다.
5. **Build Motion Assets**를 누릅니다. 생성 후 실제 플레이어 Animator의 각 상태에 `Generated/Clips`의 `.anim`을 넣습니다.

| 생성 항목 | 용도 |
|---|---|
| `Generated/Clips/Player_Run_v4.anim` 등 16개 `.anim` | 실제 게임의 각 Animator State → Motion에 지정 |
| `Generated/Player_Motions_v4_Preview.controller` | 모션 확인용 상태 모음. 게임 전환 조건은 포함하지 않음 |
| `Generated/Player_Motions_v4_Preview.prefab` | Scene에 넣고 Play하면 모션을 순서대로 확인하는 프리팹 |

미리보기 프리팹의 `Player Motion Preview V4`에서 **Auto Advance**를 끄고 **Motion**을 선택하면 한 동작만 확인할 수 있습니다. 1회성 동작은 재생 후 마지막 프레임을 유지합니다. 컴포넌트 메뉴의 **Replay Selected Motion**으로 다시 재생할 수 있습니다. 이 컴포넌트는 실제 플레이어에 붙이는 이동·전투 컨트롤러가 아닙니다.

## 모션 구성

총 **16개 클립, 101개 출력 프레임**입니다. 출력 프레임 수는 새로 그린 포즈 수와 다릅니다. 유지 동작에는 같은 자세의 불꽃 움직임을, 더블 점프에는 하나의 웅크린 자세를 회전한 프레임을 사용했습니다.

| 클립 / Animator 상태 이름 | 출력 프레임 | 길이 | 재생 |
|---|---:|---:|---|
| Run | 10 | 0.500초 | 기존 8장 + 경계 연결 2장 / 반복 |
| JumpStart | 3 | 0.100초 | 1회 |
| JumpRise | 8 | 0.333초 | 상승 중 반복 |
| JumpApex | 4 | 0.167초 | 정점에서 1회 |
| Fall | 8 | 0.333초 | 낙하 중 반복 |
| Land | 4 | 0.133초 | 1회 |
| DoubleJump | 16 | 0.444초 | 웅크림 → 360° 회전 → 펴기 |
| DashStart | 2 | 0.050초 | 1회 |
| DashLoop | 8 | 0.200초 | 대시 유지 중 반복 |
| DashEnd | 3 | 0.100초 | 1회 |
| Attack | 8 | 0.360초 | 프레임별 시간 배분 / 1회 |
| WallCling | 8 | 0.400초 | 벽에 붙은 상태 반복 |
| WallClimb | 4 | 0.500초 | 오르기 반복 |
| WallSlide | 8 | 0.400초 | 미끄러지기 반복 |
| WallJump | 3 | 0.100초 | 벽에서 밀어내기 |
| Hurt | 4 | 0.167초 | 1회 |

정확한 각 프레임의 시작 시각, 반복 여부, 원화 수는 `Motions.json`에 있습니다. Run 단독 GIF는 48fps, 전체 및 나머지 GIF는 20fps로 샘플링하며, Unity 클립에는 JSON의 정확한 타이밍이 적용됩니다.

## 크기·방향 기준

- 모든 사용 프레임: **512 × 512 RGBA PNG**.
- 공통 피벗: **X 0.5 / Y 0.19921875**, 이미지 아래에서 `(256, 102)`px.
- 접지 기준: 이미지 위에서 **Y 410px**. PPU 기본값 **200**.
- 오른쪽을 바라보는 원화입니다. 왼쪽 이동은 게임 쪽에서 SpriteRenderer의 `flipX`로 처리할 수 있습니다. 더블 점프는 한 자세 전체가 회전하므로 회전 도중 얼굴 각도가 달라지는 것이 정상입니다.
- 웅크림·구르기·대시의 바운딩 박스를 Idle 높이로 늘리지 않았습니다. 원화 준비 단계에서 머리 영역을 기준으로 배율을 교정하고, 해당 원화에서 파생되는 프레임은 같은 배율로 출력했습니다. 수정된 Run은 기존 8장을 그대로 사용하고, 마지막과 처음 사이에만 연결 원화 2장을 사용합니다.
- 공중/벽 동작은 다리의 펴짐에 따라 발끝이 접지선 위아래로 움직일 수 있습니다. Rigidbody/Collider의 실제 궤적은 게임 코드가 담당합니다.
- 클립에는 SpriteRenderer의 Sprite 교체만 기록됩니다. Transform Scale이나 Position, 물리 이동, 충돌체 크기를 애니메이션하지 않습니다.

`Previews/Scale_Comparison.png`에서 Idle과 동작별 대표 포즈를 같은 배율로 비교할 수 있습니다. `Previews/Motions_Overview.gif`의 이동 궤적은 동작을 설명하기 위한 예시입니다.

## 기존 Animator에 연결

기존 Idle 상태는 `Player_Idle_v3.anim`을 계속 사용하고, 새 상태의 Motion에 대응하는 v4 클립을 넣습니다. 자세한 분기 기준은 `Animation_Connections_KO.md`를 참고하세요.

- Run: 바닥에 있고 이동 중일 때. 이동을 멈추면 Idle.
- 점프: `JumpStart → JumpRise → JumpApex → Fall → Land`. 상승·정점·낙하는 실제 수직 속도에 따라 전환하며 착지 시 Land로 전환합니다.
- 더블 점프: 공중 입력 시 DoubleJump를 처음부터 재생하고, 끝나면 수직 속도에 맞는 JumpRise/Fall로 연결합니다.
- 대시: DashStart 후 DashLoop. 실제 대시 타이머가 끝나면 DashEnd와 현재 지상/공중 상태로 연결합니다.
- 벽: 이동 입력과 벽 접촉 여부에 맞춰 WallCling/WallClimb/WallSlide를 선택합니다. WallJump 원화는 오른쪽 벽을 보고 왼쪽으로 밀어내는 자세입니다.
- 공격/피격: 게임 로직에서 진입시키고 종료 후 현재 이동 상태로 복귀합니다.

스프라이트 교체 애니메이션의 Transition Duration을 늘려도 중간 그림이 만들어지지 않습니다. 이 세트의 기본 전환은 Duration 0으로 시작해서 조작감에 맞춰 조정하세요. 같은 상태를 재생하려고 매 Update마다 `Animator.Play(..., 0, 0)`을 호출하면 첫 프레임만 보이므로, 상태가 바뀔 때만 호출합니다.

## 공격 판정

Attack의 권장 타격 구간은 **0.105초 이상 ~ 0.175초 미만**입니다. 0.360초 클립의 정규화 시간으로는 약 **0.292 ~ 0.486**입니다. 검이 보이는 모든 시간을 판정 구간으로 사용하지 마세요. 준비 동작에서도 불꽃 검이 생겨납니다.

이 도구는 히트박스나 Animation Event를 자동으로 추가하지 않습니다. 기존 전투 스크립트에 맞춰 위 구간에서 판정을 켜고 끄고, 공격이 피격·대시 등으로 취소될 때도 반드시 끄세요. 같은 공격에서 같은 적을 중복 타격하는지 여부 역시 전투 로직에서 결정합니다.

## 스프라이트 시트를 직접 Slice하는 경우

`Sheets/*_Grid512.png`를 사용합니다. `Source`는 제작용 원화이며 Slice 대상으로 사용하지 마세요.

1. Texture Type = Sprite (2D and UI), Sprite Mode = Multiple.
2. PPU = Idle과 동일, Max Size = **4096 이상**, Compression = None, Filter Mode = Bilinear, Generate Mip Maps = Off.
3. Sprite Editor → Slice → **Grid By Cell Size**, Pixel Size **512 × 512**, Offset **0**, Padding **0**.
4. Pivot = Custom **(0.5, 0.19921875)**. Mesh Type은 Full Rect를 사용합니다.
5. 좌상단부터 오른쪽, 다음 행 순서로 사용합니다. Attack은 프레임별 표시 시간이 다르므로 `Motions.json`의 times를 따릅니다.

생성 도구는 이미 잘린 `Frames` PNG를 Single Sprite로 임포트합니다. 직접 Slice한 결과와 원본 픽셀은 같으며, 도구는 피벗·PPU·클립 순서·표시 시간을 자동으로 맞춰줍니다. 두 방식 모두 한 Animator의 해당 상태에는 한 세트만 연결하면 됩니다.

## 파일 구성과 검증

- `Frames/`: 실제 사용할 101개 PNG.
- `Sheets/`: 16개 모션별 그리드 시트.
- `Previews/`: 상태별 GIF 16개, 전체 GIF, 크기 비교 PNG.
- `Editor/`, `Runtime/`: Unity 생성 도구와 미리보기 전용 컴포넌트.
- `Motions.json`: 클립의 순서·타이밍·셀·피벗 정보.
- `Source/`, `Keyposes/`: 제작 원화, 배율 교정 기록, 재생성 코드. 원화는 내장 이미지 생성 도구로 제작했고 프롬프트는 `Source/prompts.json`에 있습니다.
- `Verification.json`: 프레임·시트 픽셀 일치, 투명 여백, 순서·타이밍 검사 결과.

PNG와 GIF 및 그리드의 정합성을 검사했습니다. **Unity Editor에서의 컴파일·플레이, 사용자의 게임 이동/전투 컨트롤러와의 연결은 이 환경에서 실행하지 못했습니다.** 생성 후 Console 오류와 실제 이동 속도·점프 높이·공격 판정의 일치를 확인해야 합니다.

생성 버튼을 다시 누르면 이 팩의 동명 Generated 에셋을 갱신합니다. 기존 Idle 폴더와 사용자의 플레이어 프리팹은 수정하지 않습니다. 생성 클립을 직접 편집해 유지하려면 복사본을 만들어 사용하세요.

## 제작 스크립트 재실행

Node.js와 `sharp`, `@napi-rs/canvas`를 설치한 환경에서 폴더 안의 `Source/build_motions.cjs`를 실행하면 PNG와 JSON을 재생성합니다. `Source/build_previews.cjs`는 추가로 ffmpeg가 필요합니다. `Source/verify.cjs`로 결과를 검사할 수 있습니다. Unity 임포트와 사용에는 Node.js나 ffmpeg가 필요하지 않습니다.
