# Boss_Swing2 — Idle·Walk 얼음 색 일치 보정

> 후속 적용: [털·눈·얼음 디테일 수정 v02](../BossSwing_DetailMatch_v02/Boss_Swing2_DetailMatch_v02.md). 아래 색 보정은 이전 제작 이력이며, 현재 중간 프레임은 Idle 재질을 기준으로 다시 편집했다.

2026-09-28 사용자 요청에 따라 현재 Swing2의 등 얼음 색과 복귀 자세를 수정했다.

## 적용 결과
- 적용 파일: [Boss_Swing2.png](../../../Assets/04.Sprites/Boss/Boss_Swing2.png).
- [Idle·Walk 및 수정 전후 비교](Idle_Walk_Swing2_Ice_Comparison.png).
- [전체 시트 보존본](Boss_Swing2_IceMatch_v01.png).
- 실제 크기 2048×1024, 4열×2행, 각 셀 512×512, Bottom Center (0.5,0), PPU50 유지.
- 중간 1~6번 프레임의 쓸기 자세·실루엣·발 위치·알파를 그대로 보존하고 파란 얼음의 채도·명암을 보정했다.
- 시작 0번과 복귀 7번 프레임은 현재 Boss_Idle_0의 픽셀을 그대로 사용했다. 기본 자세로 돌아올 때 등 얼음의 모양·눈 가장자리·털 표현이 기준과 일치한다.
- 공격 중간 프레임을 새로 그리지는 않았다. 원래 생성된 그림의 비교적 부드러운 질감은 남아 있으며, 모든 중간 포즈가 Idle의 세부 형태와 동일하다는 의미는 아니다.

## 기준과 방식
- 현재 적용 중인 Boss_Idle.png와 Boss_Walk.png의 8개 프레임씩을 기준으로 사용.
- 이전 사용자의 프로그램 기반 수정 선택을 유지했다. 생성형 재제작 없이 RGB 픽셀을 보정.
- 셀 내부 X65~414, 위에서 Y160~369의 불투명한 파란 영역에서 HSV 분포를 측정했다. 모든 파란 픽셀이 순수한 얼음만을 뜻하지는 않으므로 시각 비교도 병행했다.
- 기준 채도 중앙값 약 0.737. Swing2 중간 프레임의 기존 0.848~0.912를 보정 후 0.747~0.752로 낮췄다.
- 밝기 상위 90% 지점은 기존 0.769~0.788에서 0.855~0.863으로 조정했다. Idle·Walk 기준은 약 0.863.
- 파란 재질 마스크를 부드럽게 적용하고 명암을 구간별로 대응했다. 작은 면 경계의 대비만 제한적으로 보강했다. 중성 털과 하체를 넓게 색칠하지 않았다.
- 0번·7번과 Idle_0의 512×512 RGBA 픽셀 차이 0개를 확인했다.

## 검증·보존
- Unity 재임포트 완료. 8개 Sprite의 GUID·Local ID, Rect, Pivot, PPU 및 9개 애니메이션 참조 키를 확인.
- PNG .meta와 Boss_Swing2.anim은 작업 전후 해시가 동일하다. 콜라이더 키·타격 시간·이벤트도 변경하지 않았다.
- 기존 Swing·Idle·Walk 이미지, 코드, 프리팹을 수정하지 않았다. 사용자 수정 중인 MinsuScene과 Animator Controller의 파일 해시도 그대로 유지했다.
- 임시 프리팹으로 0.70초 좌우 렌더 생성: [왼쪽](Unity_Swing2_Left.png), [오른쪽](Unity_Swing2_Right.png). 활성 씬 dirty=false 유지.
- 실제 Play 모드에서 전환 타이밍과 모든 이동 상태를 검증한 것은 아니다.
- [수치 보고서](Correction_Report.json), [보존 파일 해시](Preserved_Before.json).
- 수정 전 PNG/meta/anim/meta: [백업](../../../Backups/BossSwing2_before_ice_match/).
- 기존 [클립 재생 미리보기](../BossSwing_GroundSweep_v01/Boss_Swing2_Preview.html)는 최신 PNG를 직접 참조한다.

