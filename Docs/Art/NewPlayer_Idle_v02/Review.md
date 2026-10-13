# Idle v02 — 한 장의 원화를 이용한 불꽃 애니메이션

2026-10-12 제작, 2026-10-13 재개 및 Play 확인. 사용자 컨펌 대기.

## 결과

- 기존 새 Idle 마스터 1장을 그대로 사용했다. 프레임별 AI 이미지 재생성은 하지 않았다.
- 얼굴·손·발을 보호하고 머리 위, 목 뒤, 등과 엉덩이 뒤 불꽃 끝에 작은 주기적 변형을 적용했다. 전체 캐릭터의 위치·크기·회전은 고정했다.
- 512×512 RGBA, PPU 200, 피벗 (270, 226), **32장 / 20fps / 1.6초 반복**이다.
- 16장 샘플링의 불꽃 끝 최대 프레임 이동량 약 3.55px와 비교해 32장은 약 1.81px로 줄어든다. 32장 모두 한 원화에서 만든 서로 다른 샘플이다.
- 이전 Idle v01의 마지막 중복 프레임 0.05초를 없앴다. 새 클립은 32키이며 마지막 31번 프레임 이후 첫 프레임으로 바로 순환한다.

## 확인 자료

- [인터랙티브 비교](Idle_Review.html): 원본 / 이전 12장 생성본 / 새 32장 변형본, 16장 대 32장, 새 Idle·Run 전환, 좌우 반전, 속도 조절, 프레임 이동.
- [원본(왼쪽)과 새 Idle(오른쪽) GIF](Idle_Comparison.gif).
- [변형 영역](MotionMask.png): 청록색은 완전 보호 영역, 주황색은 변형 영향 영역. 표시용 반투명 오버레이이며 실제 프레임에는 들어가지 않는다.
- [대표 프레임](Keyframes.png), [실제 브라우저 확인 화면](Preview_UI.png).
- [프레임 폴더](../../../Assets/04.Sprites/NewPlayer/Idle_v02/Frames), [스프라이트시트](../../../Assets/04.Sprites/NewPlayer/Idle_v02/Sheets/Idle_Grid512_v02.png), [사용한 원화 사본](../../../Assets/04.Sprites/NewPlayer/Idle_v02/Source/Idle_Master512.png).

## 검증

- 모든 프레임의 보호 영역과 발 고정 구간에서 원화 대비 변경 바이트 0. 주기 종료 시점과 시작 프레임 사이 변경 바이트 0. [픽셀 검증](PixelVerification.json).
- Unity API 적용 후 32개 스프라이트 참조, 1.6초 길이, 20fps, 반복 설정과 피벗을 확인했다. [Unity 검증](UnityVerification.json).
- 2026-10-12 최초 적용 요청의 CLI 응답은 타임아웃이었지만 Editor 내부 작업은 완료됐다. 2026-10-13 저장 결과와 실제 재생을 확인했으며 다시 중복 적용하지 않았다.
- YuchanScene Play 모드에서 Idle 32장과 Run 16장의 각 키가 SpriteRenderer에 표시되는지 확인했다. 실제 플레이어 이동 이벤트를 주입하여 Idle → 오른쪽 Run → 왼쪽 Run → Idle 전환을 검사했다. 키보드 입력 자체를 자동 검사한 것은 아니다.
- 345개 관찰 샘플 모두 접지 상태였다. 검사 후 Play를 종료하고 임시 입력 상태를 복구했다. [Play 결과](PlayVerification.json), 실제 카메라 [Idle 전](Unity_Idle_Before.png), [오른쪽 Run](Unity_Run_Right.png), [왼쪽 Run](Unity_Run_Left.png), [Idle 후](Unity_Idle_After.png).
- 비교 HTML을 실제 브라우저에서 열어 렌더링, 16/32 선택, 프레임 이동, Run 전환, 좌우 반전, 속도 선택을 확인했다. 검사 중 브라우저 오류 로그는 없었다.
- 이 검증은 연결·보존·고정 영역 검사다. 불꽃 움직임의 미적 만족도는 사용자 확인 대상이며 다른 모션의 품질 검증은 포함하지 않는다.

## 보존과 범위

원본 Player 635개 파일을 포함한 기존 보호 대상 877개 파일의 해시를 비교했다. 기존 파일의 변경은 `Assets/05.Animation/player/Idle.anim` 하나이며, 원본 Player, 기존 Run 버전, Idle v01, 디자인 마스터, 다른 애니메이션, 플레이어 코드, 씬과 프리팹은 보존했다. [파일 검증](FileVerification.json).

작업 전 클립은 [직렬화 사본](Idle_Before.anim.txt)과 [스프라이트 참조 백업](Idle_BeforeCurve.json)에 보관했다. 제작·시트 조립 코드는 [build.cjs](build.cjs), Unity 연결은 [Apply.cs](Apply.cs), 미리보기는 [preview.cjs](preview.cjs)에 있다. 제작 코드는 Docs에만 두어 게임 실행 코드에 추가하지 않았다.

Run은 사용자가 괜찮다고 평가한 Run_v02 16장을 유지했다. 나머지 모션과 공격 이름 변경은 사용자 컨펌 후 별도 진행한다.
