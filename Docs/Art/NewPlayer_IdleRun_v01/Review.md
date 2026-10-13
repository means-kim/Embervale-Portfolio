# Idle · Run 디자인 통일 v01

2026-10-12. 사용자의 `Idle 기준 확정 → Idle·Run 통일` 요청에 따른 확인용 적용본이다. 새 디자인의 최종 미적 승인은 사용자 확인 대기다.

## 결과

| 모션 | 이전 | 이번 적용 | 역할 |
|---|---:|---:|---|
| Idle | 32장 | 12장, 1.65초 반복 | 서 있는 자세를 유지하며 불꽃 중심으로 작은 변화 |
| Run | Run_v02 16장 | 16장, 약 0.521초 반복 유지 | 기존 개선된 달리기 동작 유지 |

Run_v02의 얼굴, 몸 비율, 불꽃 색과 화풍을 기준으로 새 Idle 마스터와 프레임을 제작했다. Run의 이미지와 재생 설정은 변경하지 않았다. Idle의 고유 이미지는 12장이며 루프 닫기용 첫 이미지 키까지 포함하면 13키다. 프레임 수와 AnimationClip의 sampleRate는 다른 값이다. Idle sampleRate는 기존 20을 유지했다.

- [전환·속도·좌우 반전 비교 미리보기](Idle_Run_Review.html)
- [기존/개선 전환 비교 GIF](Idle_Run_Comparison.gif): 왼쪽은 기존 Idle, 오른쪽은 새 Idle. 양쪽 Run은 같은 Run_v02다.
- [12프레임 모아보기](Idle_ContactSheet.png)
- [기존 Idle / 새 마스터 / Run 비교](Master_Comparison.png)
- [새 Idle 마스터](../../../Assets/04.Sprites/NewPlayer/Design_v01/Player_Idle_Master_v01.png)
- [새 Idle 스프라이트시트](../../../Assets/04.Sprites/NewPlayer/Idle_v01/Sheets/Idle_Grid512_v01.png)

## 제작과 정렬

내장 image_gen으로 실제 기존 Idle·Run 이미지를 참조해 마스터를 만들고, 마스터 4칸 템플릿을 바탕으로 불꽃 변화 프레임을 생성했다. Node 이미지 처리는 셀 분리, 떨어진 알파 조각 제거, 크기·위치 정렬, 시트 조립에 사용했다.

16개 후보 중 다리 길이가 다른 첫 4개를 재생성했으나, 보정본도 길이 차이가 커서 제외했다. 나머지 12개를 최종 선택했다. 제외한 후보와 수정본은 이 문서 폴더에 제작 이력으로 보존했다. 마스터는 디자인 참조이며 별도 애니메이션 프레임으로 추가하지 않았다.

최종 프레임은 모두 투명 RGBA 512×512, PPU 200, 피벗 (270, 226) 픽셀이다. 밝은 얼굴 영역을 기준으로 크기와 위치를 정렬했다. 몸을 크게 움직이지 않는 방향이지만, 생성 이미지 특성상 윤곽과 불꽃 발끝의 작은 차이는 남아 있다. 완전히 동일한 몸 픽셀에 불꽃만 교체한 방식은 아니다.

실제 프롬프트: [마스터](Prompt_IdleMaster.txt), [그룹 1](Prompt_IdleGroup_1.txt), [그룹 2](Prompt_IdleGroup_2.txt), [그룹 3](Prompt_IdleGroup_3.txt), [그룹 4](Prompt_IdleGroup_4.txt), [그룹 1 보정 시도](Prompt_IdleGroup_1_Repair.txt). 좌표·해시·프레임 선택은 [Manifest](Manifest.json)에 기록했다.

## 적용·검증

- Unity API로 기존 `Idle.anim`의 스프라이트 참조만 교체했다. 길이 1.65초, 반복 설정과 기존 이벤트를 유지했다.
- Play 모드에서 실제 플레이어의 이동 이벤트에 방향 값을 전달해 Idle → 오른쪽 Run → 왼쪽 Run → Idle을 확인했다. 실제 이동·상태·물리 로직을 사용했으며 키보드 입력 검사는 아니다.
- Idle 12장과 Run 16장의 각 스프라이트 키가 실제 Renderer에 표시되는 것을 별도로 확인했다. 방향 이동 구간의 접지 상태도 유지됐다.
- [Play 검증 데이터](PlayVerification.json), [Unity 에셋 검증](UnityVerification.json), 실제 카메라 캡처: [Idle 전](Unity_Idle_Before.png), [오른쪽 Run](Unity_Run_Right.png), [왼쪽 Run](Unity_Run_Left.png), [Idle 후](Unity_Idle_After.png).
- 검사 후 Play를 종료했고, 임시 입력 상태를 복구했으며 씬에 미저장 변경이 없음을 확인했다. 전체 플레이 동선·공격·점프 품질 검증은 이번 범위가 아니다.

## 보존 범위

원본 `Assets/04.Sprites/Player` 635개 파일을 포함한 보호 대상 838개 파일의 작업 전후 SHA-256을 비교했다. 변경된 기존 파일은 `Assets/05.Animation/player/Idle.anim` 하나다. 기존 Player, Run 버전, 다른 애니메이션, 플레이어 코드, 씬과 프리팹은 그대로다. [파일 검증](FileVerification.json).

이번 신규 자산은 `NewPlayer/Design_v01`과 `NewPlayer/Idle_v01`에 저장했다. 작업 전 클립은 [Idle 원본 직렬화 사본](Idle_Before.anim.txt), [Run 원본 직렬화 사본](Run_Before.anim.txt), [Idle 스프라이트 키 백업](Idle_BeforeCurve.json)에 보존했다. 되돌릴 때는 이 기록을 바탕으로 Unity API에서 Idle 참조를 복원할 수 있다.

공격 모션 이름 변경, FireBallAttack 재제작, 점프 단계 축소와 나머지 모션 제작은 이번에 진행하지 않았다.
