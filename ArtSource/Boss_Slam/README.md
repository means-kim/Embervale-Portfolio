# Boss Slam

설산 거인의 한손 내려찍기용 스프라이트 시트와 재생 클립입니다.

## 결과물

- 시트: `Assets/04.Sprites/Boss/Boss_Slam.png`
- 클립: `Assets/05.Animation/Boss/Boss_Slam.anim`
- 규격: RGBA PNG 2048×1024, 4열×2행, 512×512 셀 8개
- 피봇: Bottom Center `(0.5, 0)` / 픽셀 `(256, 0)`
- PPU: 50, Max Size: 2048, Bilinear, Mipmap 비활성화
- 재생 순서: 위 행 왼쪽부터 0–3, 아래 행 왼쪽부터 4–7
- 공격 팔: 화면 왼쪽의 팔. 모든 공격 프레임에서 같은 팔을 사용합니다.
- 프레임 0과 7은 기존 `Boss_Idle_0` 픽셀을 그대로 사용한 연결 포즈입니다.
- 생성된 중간 포즈는 동일 배율로 맞춘 다음, 앞발 중심 X와 하단 접지선을 정렬했습니다.
- 하단 여백은 알파가 0보다 큰 픽셀 기준으로 8프레임 모두 0px입니다.

## 타이밍

| 프레임 | 시작 시각 | 유지 시간 | 동작 |
|---|---:|---:|---|
| 0 | 0.00초 | 0.10초 | Idle에서 준비 |
| 1 | 0.10초 | 0.20초 | 공격 팔 들어 올리기 |
| 2 | 0.30초 | 0.20초 | 머리 위로 올리기 |
| 3 | 0.50초 | 0.20초 | 최대 전조 유지 |
| 4 | 0.70초 | 0.10초 | 빠른 하강 |
| 5 | 0.80초 | 0.05초 | 지면 충돌 |
| 6 | 0.85초 | 0.50초 | 낮은 자세의 후딜 |
| 7 | 1.35초 | 0.30초 | Idle 복귀 |

총 1.65초: 전조 0.70초 + 공격 동작 0.15초 + 회복 0.80초.
클립은 20fps 시간 눈금의 비반복 클립이며, 마지막 포즈 유지 시간을 위해
1.60초에 프레임 7의 참조 키를 한 번 더 둡니다. 시트의 프레임 수는 8개입니다.
기획서의 다음 공격까지 대기하는 Cooldown 0.5초는 전투 상태 머신에서 처리할 항목입니다.

Animator 상태 및 전이 연결, 공격 판정, VFX, SFX, 카메라 효과는 이후 전투 구현에서 연결합니다.
이 클립의 0.80초 충돌 프레임을 연결 기준으로 사용할 수 있습니다.
재생 미리보기 GIF는 반복해서 표시하지만 Unity 클립의 Loop Time은 꺼져 있습니다.

## 제작 및 검수

내장 ImageGen으로 Idle/Walk를 참조해 동작을 생성했습니다.
첫 생성에서 팔이 바뀌는 문제를 발견해 같은 팔이 이어지도록 한 차례 수정했습니다.
사용자가 선택한 코드 방식으로 최종 크기, 배치, 접지선과 메타데이터를 처리했습니다.

- 최초 프롬프트: [generation-prompt.txt](generation-prompt.txt)
- 최종 수정 프롬프트: [correction-prompt.txt](correction-prompt.txt)
- 채택한 생성 원본: [generation-source.png](generation-source.png)
- 최종 프레임별 수치: [measurements.json](measurements.json)
- 최종 시트 검수 이미지: [contact-sheet.png](contact-sheet.png)
- 애니메이션 미리보기: [preview.gif](preview.gif)
- 조립 스크립트: [build_slam.py](build_slam.py)

Unity MCP 검증: 실제 로드된 텍스처 2048×1024, Sprite 8개 모두 Rect 512×512,
Pivot (256, 0), PPU 50. 클립 길이 1.65초, Loop false, 스프라이트 참조 누락 0개.
기존 Idle/Walk 이미지, 클립, Animator Controller는 수정하지 않았습니다.
