# Boss_Swing2 — 털·눈·얼음 디테일 수정 v02

2026-09-28. 사용자 요청: Idle에 비해 Swing2 공격 중 털과 쌓인 눈의 디테일이 뭉개지는 이질감을 줄인다.

## 결과
- 적용: [Boss_Swing2.png](../../../Assets/04.Sprites/Boss/Boss_Swing2.png).
- [Idle·수정 전·수정 후 비교](Idle_Before_After_Comparison.png).
- [동일 시점 재생 비교](Swing2_Detail_Comparison.html): Idle/Walk 선택, 1배/0.35배속, 기준 동작과 공격 전환, 최대 타격 프레임 정지, 384/256px 셀 표시.
- [버전 보존 시트](Boss_Swing2_Detail_v02.png).
- 1~6번 공격 중간 프레임을 Idle 기준으로 개별 편집. 0번·7번은 기존의 Idle_0 동일 픽셀을 유지했다.
- 털: 둥글고 매끈한 큰 덩어리를 겹치는 뾰족한 털 다발과 가는 결로 수정.
- 눈: 어깨·팔의 흰 띠를 얇고 불규칙한 덮개, 작은 입자와 아래쪽 그림자로 구분.
- 얼음·손: 얼음의 균열·서리·밝은 모서리와 어두운 손의 갈라진 표면, 거친 발톱을 기준에 맞췄다.
- 새 그림의 회수 6번은 손을 가슴 쪽으로 조금 더 접는 자세다. 게임 코드나 클립 키는 변경하지 않았다.

## 제작
- 도구: built-in image_gen. 기존 포즈를 편집 대상으로 주고, 백업 Idle의 1024×1024 첫 셀을 화풍·재질 기준으로 제공했다.
- 최초 최대 타격 5번 수정 후, 그 결과를 다른 중간 프레임의 공통 참조로 추가했다.
- [최대 타격 프롬프트](Prompts.md), [나머지 프레임 프롬프트](Frame_Prompts.md).
- 생성 원본은 각 1254×1254 RGBA. 모든 원본과 기준 이미지를 이 폴더에 저장했다. 요청 해상도와 실제 생성 해상도는 다르다.
- 픽셀 정렬은 이전에 합의한 프로그램 방식으로 수행했다. 모든 원본을 동일한 512/1254 배율로 축소하고 각 발 하단을 Y512에 배치했다.
- RGBa premultiplied-alpha Lanczos로 축소. alpha 8/255 미만의 미세 투명 배경 노이즈만 제거했다. 전체 샤픈이나 새 모션 블러를 적용하지 않았다.
- [정렬 수치](Registration_Report.json).
- 수정 전 PNG/meta와 anim/meta는 [백업](../../../Backups/BossSwing2_before_detail_match_v02/)에 보존했다.

## 규격·검증
- 실제 2048×1024 RGBA, 4열×2행, 512×512 셀 8개.
- 하단 중앙 피봇 (256,0), PPU50, 8개 Sprite GUID/Local ID와 모든 스프라이트 키 참조 유지.
- 모든 셀의 그림 하단 Y512. 좌우에 투명 여백이 있어 인접 셀 침범 없음.
- 0번·7번과 현재 Idle_0의 RGBA 픽셀 동일 확인.
- Unity 재임포트 및 임시 프리팹의 0.70초 [왼쪽](Unity_Swing2_Left.png)·[오른쪽](Unity_Swing2_Right.png) 렌더 확인.
- Console 오류 0개, 활성 씬 dirty=false. [Unity 검사 기록](Unity_Verification.json).
- 기존 Swing/Idle/Walk 이미지·meta, Swing2 meta/anim, 사용자 MinsuScene/Animator Controller, 보스 프리팹은 작업 전후 해시가 동일하다. [이전 해시](Preserved_Before.json), [이후 비교](Preserved_After.json).
- 게임 코드·콜라이더 키·타격 시간·공격 이벤트는 수정하지 않았다.
- 실제 Play 모드의 상태 전환·이동·피해 판정은 미검증. 모든 털 다발을 일대일로 추적하는 리깅 기반 애니메이션은 아니다.
- 재생 비교 HTML의 문법과 참조 파일은 확인했다. 앱 브라우저의 로컬 file URL 제한으로 브라우저 재생 화면 자체는 직접 검증하지 못했다.

