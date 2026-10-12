# NewPlayer 달리기 v01 — 사용자 검토 대기

2026-10-12. 사용자가 기존 Player 폴더 보존, NewPlayer에 필요한 동작만 제작, 달리기 우선 적용을 요청했다. 나머지 동작은 이 결과의 사용자 승인 후 진행한다.

## 지금 확인하기

- [비교 화면](Run_Comparison_v01.html): 기존 10장/신규 16장 동시 재생, 0.5배/0.25배 속도, 프레임 탐색, 좌우 반전, 공통 기존 Idle 자세 비교. 현재 몸통 피벗을 기준으로 표시한다.
- [비교 GIF](Run_Comparison.gif): 빠른 육안 비교. GIF의 시간 해상도는 Unity 클립과 다르므로 타이밍의 최종 기준은 Run.anim이다.
- [Unity Play 카메라 캡처](Unity_Run_Play.png): YuchanScene에서 Run 상태를 직접 선택하여 캡처했다.
- Unity의 현재 YuchanScene에서 Play 후 A/D 또는 좌우 방향키로 달리면 공유 Run.anim의 신규 이미지가 표시된다. 이 클립을 공유하는 다른 씬에도 반영된다. Idle 등 다른 상태는 기존 Player 이미지를 사용한다.

## 파일과 사양

- 신규 프레임: ../../../Assets/04.Sprites/NewPlayer/Run_v01/Frames/Run_00.png ~ Run_15.png.
- 실제 프레임 PNG: 512×512 RGBA, PPU 200, Bilinear, 미프맵 없음, 압축 없음. 기존 자세별 몸통 피벗과 중간 좌표를 사용했다. 이동 속도/콜라이더/씬/프리팹/컨트롤러/다른 클립은 변경하지 않았다.
- 최종 생성 원본: 4장씩 포함한 1254×1254 RGBA 시트 4개, 셀 약 627×627. 요청의 선호 해상도 2048×2048와 실제 출력 해상도를 구분한다. 얼굴 영역의 크기와 위치를 기존 포즈와 대조하여 512셀에 배치한다. 개별 이미지마다 외곽에 맞춰 전체 크기를 자동으로 채우지 않았다.
- 정리 시트: ../../../Assets/04.Sprites/NewPlayer/Run_v01/Sheets/Run_Grid512_v01.png, 2048×2048, 4×4. 런타임 애니메이션은 이 시트 대신 개별 프레임을 참조한다.
- 적용 클립: ../../../Assets/05.Animation/player/Run.anim. 기존 GUID 유지, Samples 48 유지, 서로 다른 스프라이트 10→16장. 0~0.46875초를 0.03125초 간격으로 배치하고 0.5초에 첫 그림을 반복한다. 전체 길이 0.520833초 유지. 키는 반복 종료 키를 포함하여 17개다.
- 원본 Player 폴더는 파일 및 .meta를 보존했다. 기존 Run 클립 전체 텍스트는 Run_Original.anim.txt, 원래 곡선은 OriginalCurve.json에 보관했다.

## 제작 방식과 검증 범위

기본 imagegen 도구로 생성했다. 실제 프롬프트는 Prompt_Group_1~4.txt에 있다. 최초 혼합 중간 그림과 저해상도 16장 시트는 미채택 후보로 Docs에 보존했고 NewPlayer에서는 제외했다. 최종 원본 시트는 프로젝트 안 Source에 보관했다. build.cjs는 시트 셀의 주요 연결 알파 형상을 선택하여 인접 셀 조각/분리된 잡점을 제외하고, 얼굴 기준 크기를 맞추고 PNG를 분할·패킹한다. 생성 도구 외 별도 유료 API를 사용하지 않았다.

UnityVerification.json: 실제 Unity 가져오기, PPU/피벗 적용, 새 참조 16개, 기존 반복 길이와 루프 유지, 씬 미저장 변경 없음.

PlayVerification.json: 실제 Play 모드에서 Animator의 Run 상태를 각 키 시각에 직접 선택하여 16개 신규 이미지가 SpriteRenderer에 할당되고 카메라에 표시되는 것을 확인했다. 이 검사는 키보드로 달린 테스트가 아니다. 이동 중 발 접촉, Idle↔Run 전환, 왼쪽 이동의 실제 품질은 사용자 플레이 검토가 필요하다. 생성본의 얼굴/불꽃 색감은 원본과 차이가 남을 수 있으므로 채택 완료로 표시하지 않는다.

FileVerification.json: 원본 Player 파일 및 .meta 보존, 다른 애니메이션 보존, 새 프레임 해상도/알파와 고유 이미지 확인.

## 원래 달리기로 복구

Play 모드 종료 후 다음 명령으로 이전 참조와 타이밍만 복구한다. NewPlayer 이미지는 보존된다.

```powershell
unity command eval_file --project-path C:/Unity/2d-team08 --caller plugin --skill unity-cli --file C:/Unity/2d-team08/Docs/Art/NewPlayer_Run_v01/Restore.cs
```

제작 재실행은 최종 Source/Run_Group_1~4_Generated_v01.png 네 파일을 순서대로 build.cjs에 전달하고, 연결된 Editor에서 Apply.cs를 eval_file로 실행한다. 프로젝트 런타임 코드에 추가하는 스크립트가 아니다.
