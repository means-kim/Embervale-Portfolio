# GA1 플레이어 Idle v3

확정한 불꽃 원소 캐릭터의 Idle을 다시 제작했습니다. 범위는 Idle 한 동작입니다.

## 제작 방식

- 승인 원화를 이미지 편집 도구로 투명화한 다음, 그 한 장을 기준으로 제어된 2D 변형 애니메이션을 만들었습니다.
- 서로 다른 캐릭터 그림을 매 프레임 새로 생성하지 않았습니다. 얼굴의 크기·방향·눈 간격은 고정하고 머리 전체에 작은 수직 이동만 적용했습니다.
- 몸통의 호흡은 최대 약 1.8px, 불꽃 끝은 부분별로 시간 차이를 둔 작은 흔들림입니다. 발끝은 고정했습니다.
- GIF나 MP4를 재추출한 PNG가 아닙니다. 같은 연속 애니메이션에서 PNG 프레임과 미리보기를 출력했습니다.
- 8프레임으로 샘플링하면 선택한 불꽃 제어점의 프레임 간 이동이 최대 약 6.31px, 32프레임은 약 1.76px여서 최종 출력은 32프레임으로 정했습니다.
- 손그림 원화 32장을 새로 그렸다는 뜻은 아닙니다. 단일 원화에 적용한 변형을 32개의 스프라이트로 출력한 방식입니다.
- 붙어 있는 불꽃을 포함한 캐릭터 외형을 유지하고, 원본 배경·바닥·분리된 작은 불티는 캐릭터 본체에서 제외했습니다. 검은 없습니다.

## 규격

| 항목 | 값 |
|---|---|
| 프레임 수 | 32 |
| 프레임당 시간 | 0.05초 |
| 재생 속도 | 20fps |
| 한 주기 | 1.6초 |
| 개별 PNG | 512×512, RGBA |
| 시트 | 4096×2048, 8열×4행 |
| 순서 | 왼쪽 위에서 오른쪽으로 00~07, 다음 행 08~15 |
| Pivot | Custom (0.5, 0.19921875) |
| 발 기준선 | 이미지 위에서 410px |
| 권장 시작 PPU | 200. 기존 프로젝트의 기준이 있으면 그 값 사용 |

원화는 전체에 같은 배율을 한 번 적용했습니다. 프레임별 외곽 높이 맞추기나 개별 확대/축소는 없습니다. PPU 200일 때 불꽃 끝을 포함한 정지 실루엣 높이는 약 1.76 Unity unit입니다.

## 가장 간단한 Unity 적용

1. ZIP을 풀어 `GA1_Player_Idle_v3` 폴더를 Unity 프로젝트의 `Assets` 아래로 복사합니다.
2. 스크립트 컴파일이 끝나면 `Tools > GA1 > Build Player Idle v3`를 엽니다.
3. PPU를 확인하고 `Build Idle Assets`를 누릅니다.
4. `Generated` 폴더에 `.anim`, Idle 검수용 Animator Controller, 미리보기 Prefab이 만들어집니다.
5. Prefab을 씬에 놓고 재생하면 Idle을 확인할 수 있습니다.

이 자동 설정은 `Frames`의 개별 PNG를 사용하므로 Slice가 필요 없습니다. 시트는 같은 프레임을 묶어 제공한 별도 선택지입니다. 기존 플레이어의 다른 동작이나 코드는 변경하지 않습니다.

기존 플레이어에 연결할 때에는 검수용 Controller로 교체하지 말고, 기존 Animator의 Idle 상태 Motion에 생성된 `.anim`을 넣으세요. 클립은 Animator와 같은 GameObject에 있는 SpriteRenderer의 Sprite를 교체합니다. 두 컴포넌트가 다른 오브젝트에 있다면 시각 자식 오브젝트에 Animator와 SpriteRenderer를 함께 두거나 클립의 바인딩 경로를 맞춰야 합니다.

## 시트를 직접 Slice하는 경우

1. `Player_Idle_v3_Grid512.png` 선택.
2. Texture Type: Sprite (2D and UI), Sprite Mode: Multiple.
3. Max Size: 4096 이상. 2048이면 전체 시트가 축소될 수 있습니다.
4. PPU는 모든 상태에 동일하게 적용. Filter Mode: Bilinear, Compression: None, Mip Maps: Off, Wrap: Clamp.
5. Sprite Editor > Slice > Grid By Cell Size: 512×512. Offset/Padding: 0.
6. Custom Pivot: X=0.5, Y=0.19921875. Apply.
7. 시각적으로 왼쪽 위부터 행 순서대로 프레임을 배치합니다. 자동 생성된 이름의 정렬만 믿지 말고 확인하세요.
8. Animation Clip은 20 Samples, 0.05초 간격. 0.00~1.55초에 32장 배치하고 1.60초에 첫 그림을 루프 경계 키로 넣습니다. Loop Time 켜기.

마지막 그림과 첫 그림은 각각 인접한 시간의 자세입니다. 같은 그림을 끝에 한 프레임 더 오래 유지하면 반복 경계에서 멈칫할 수 있습니다.

## 이후 동작과의 연결 기준

- 이 원화의 얼굴 비율과 같은 PPU·Pivot을 다음 동작에도 사용합니다.
- 본체 이동은 플레이어 이동 코드가 담당하고, 이 클립에는 Transform Position/Scale 키가 없습니다.
- Run·Jump 등은 아직 이번 규격으로 재제작하지 않았습니다. 이전 시트와의 전환 품질을 이번 패키지가 보장하지는 않습니다.
- 공중 동작 종료 후에는 상승/낙하 상태로 복귀하도록 연결하며, 모든 상태의 시작과 끝에 Idle을 삽입하지 않습니다.

## 검수 범위

- 32개 PNG 크기와 알파 채널, 고정 발 영역, 얼굴 영역의 동일한 이동, 한 주기 끝과 시작의 연속성을 데이터로 확인했습니다.
- 주요 포즈와 남색 배경/축소 미리보기를 확인했습니다.
- 실제 Unity Editor에서의 컴파일·Play Mode 검증은 이 환경에서 수행하지 않았습니다. Editor 도구는 Unity Editor에서 실행하는 설치 보조 코드입니다.
- 본체 PNG에는 미리보기의 남색 배경, 선, 문구가 포함되지 않습니다.
- GIF는 확인용입니다. 게임에는 투명 PNG를 사용하세요.

`Idle_Timing.json`에는 순서·시간·기준점이, `Verification.json`에는 수치 확인 결과가 들어 있습니다. `Source`에는 기준 원화와 재생성 스크립트가 포함되어 있습니다.

## 원본 애니메이션 재생성

Unity 사용에는 이 과정이 필요 없습니다. 움직임을 수정하고 다시 출력하려는 경우에만 사용하세요.

Node.js 환경에서 `Source` 폴더의 `npm install`, `npm run build`를 실행하면 같은 변형식으로 PNG와 시트를 다시 만듭니다. `rebuild_idle.cjs`의 `field` 함수가 호흡·불꽃·팔의 움직임을 정의합니다. `RebuildPreviewFrames`에는 배경을 포함한 미리보기 PNG가 생성됩니다.

패키지 루트에서 FFmpeg를 사용하면 GIF도 다시 만들 수 있습니다.

```text
ffmpeg -framerate 20 -i RebuildPreviewFrames/preview_%02d.png -filter_complex "[0:v]split[a][b];[a]palettegen[p];[b][p]paletteuse" -loop 0 Preview_Idle_v3.gif
```

Windows 배치 파일(.bat) 안에 명령을 넣을 때에는 파일명 패턴의 `%`를 `%%`로 작성하세요.
