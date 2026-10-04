# AirBoss 한팔 올리기·내리기 얼음 변형 v01

2026-10-04. 사용자 요청: 기존 OneArm UpDown과 같은 방향의 동작, 얼음 조각 3~5개, 형태·크기 유지, 하강 트레일, 몸통·천의 후행, Idle 시작·복귀, **32프레임 멀티스프라이트만 제작**.

## 결과

- 시전 팔: 캐릭터 오른팔, 화면 왼쪽. 반대 손은 낮게 유지.
- 주요 얼음은 긴 다이아몬드형 1개, 쐐기 1개, 짧은 다면체 1개, 작은 삼각형 1개로 총 4개를 기준으로 제작했다. 투명한 푸른 면·흰 모서리·서리 질감은 [IcePush 12번](../cast_botharms_icepush/frames/v01/AirBoss_Cast_BothArms_IcePush_v01_12.png)을 참고했다. 프레임별 가림·회전·소멸로 보이는 면적은 다르며, 생성 그림의 형태·크기가 수치상 완전히 고정된다는 의미는 아니다.
- 00: 원본 Idle → 01~08: 준비·상승 → 09~12: 홀드 → 13~17: 하강 → 18~23: 서리 소멸·후행 → 24~30: 복귀 → 31: 원본 Idle.
- [미리보기](preview_v01.html), [전체 어두운 바탕](contact_final_v01.jpg), [밝은 바탕](light_background_review_v01.jpg), [주요 연결부](transitions_review_v01.jpg).
- Unity 애니메이션 클립, Animator, 씬, 프리팹, 게임 코드는 제작·수정하지 않았다. 기존 사용자 스크립트 변경도 보존했다.

## 실제 사양

### 재생 시간 v02 — 미리보기만 수정했던 과거 이력

사용자 정정에 따라 실제 PNG 프레임을 수정한 최신 결과는 [스프라이트 v02](Cast_OneArm_UpDown_Ice_Production_v02.md)다. 아래는 요청을 잘못 이해하고 미리보기만 수정했던 기록이다.

사용자가 팔을 올리고 내리는 앞부분을 늘리고 최고점에서 잠시 멈추길 요청했다. 현재 이 새 시트를 참조하는 Unity AnimationClip은 파일 검색과 Unity AssetDatabase 조회에서 없었다. 기존 요청의 멀티스프라이트 범위를 유지하고 제작 미리보기의 기본 구간별 시간만 수정했다. PNG·슬라이스·기존 클립은 그대로다.

| 구간 | 프레임 | 이전 | 변경 |
|---|---|---|---|
| 올리기 | 01~08 | 0.92초 | 1.40초 |
| 최고점 유지 | 09~12 | 0.48초 | 0.80초 |
| 내리기 | 13~17 | 0.29초 | 0.50초 |
| 앞부분 합계 | 01~17 | 1.69초 | 2.70초 |
| 전체 미리보기 | 00~31 | 3.255초 | 4.265초 |

10번 자세는 0.35초 유지한다. 18~31의 후행·Idle 복귀는 1.445초로 동일하다. 미리보기에서 ‘구간별 속도’를 선택하면 적용되며, 고정 fps 옵션은 별도의 균등 재생 비교용이다. [프레임별 시간표](timing_v02.json)에 이전·변경 시간을 함께 보존했다. 초기 로딩 후 첫 Idle의 표시 시간도 보장하도록 재생 시계 시작점을 맞췄다. 실제 Unity 클립 재생 타이밍을 변경한 것은 아니다.

| 항목 | 결과 |
|---|---|
| 개별 셀 | 32장, 1024×1024 RGBA |
| Part1 | 00~15, 4×4, 4096×4096 |
| Part2 | 16~31, 4×4, 4096×4096 |
| 순서 | 각 시트 왼쪽 위부터 행 우선 |
| Unity | Multiple, PPU 24, Center (512,512), Max Size 4096, Uncompressed |
| 생성 도구 | 내장 imagegen |
| 요청 / 생성 실제 해상도 | 1024×1024 / 1254×1254 |
| 조립 처리 | 생성 캔버스 전체를 1024로 균등 정규화. 프레임별 실루엣 fit, Trim, 재중앙 정렬, 개별 확대·축소 없음 |
| 양 끝 | 00·31 모두 기존 1024 Idle 마스터와 RGBA 픽셀 동일 |

Unity 파일:
- [Part1 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Ice_Part1_4x4_1024_v01.png) / [meta](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Ice_Part1_4x4_1024_v01.png.meta)
- [Part2 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Ice_Part2_4x4_1024_v01.png) / [meta](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Ice_Part2_4x4_1024_v01.png.meta)

## 검증과 남은 제한

- Unity MCP 실제 조회에서 두 시트 각각 16개, 총 32개 Sprite 확인. 모든 이름·Rect·Pivot·PPU와 실제 텍스처 4096×4096 확인: [Unity 기록](unity_validation_v01.json).
- 시트에서 다시 추출한 셀과 개별 PNG의 RGBA 픽셀 일치 확인. 00·31은 마스터와 동일. 새로 그린 30장의 해시와 좌표는 [셀 검증](validation_v01.json).
- 어두운 바탕·밝은 바탕·주요 연결부를 정적으로 비교했다. 팔이 너무 일찍 올라가거나 복귀 중 다시 시전하던 02·19·20·25를 수정하고, 10번 홀드의 배치·위쪽 효과 여백을 보정했다.
- **엄격한 사방 64px 완전 투명 규칙은 미충족이다.** 01~30의 바깥 64px에 alpha 1~2/255의 미세 픽셀이 남아 있다. 생성 alpha를 보존했다. alpha>2인 그림은 모두 사방 64px 안에 들어온다. 이 구분을 전체 alpha 여백 통과로 해석하지 않는다. [원시 경계 측정](alpha_border_v01.json).
- 원본 Idle 체격을 참조했고 셀·PPU·피벗 때문에 생기는 일괄 배율 변경은 없다. 독립적으로 그린 포즈 간 세부 실루엣·천 갈래·얼음 회전의 작은 변화까지 없다고 보장하지 않는다.
- HTML 노출 시간은 검토용이다. 실제 Unity 재생, 사용자 제작 클립 타이밍, 임의 Idle 프레임에서의 전환, 게임 카메라 크기·최종 아트 승인은 미검증이다. 00·31이 같은 마스터라는 사실만으로 모든 Idle 위상의 연결이 검증되는 것은 아니다.

## 제작 원본과 프롬프트

- 원본 디자인: [Idle 마스터](../idle/AirBoss_Idle_Master_image2.png). PNG SHA-256: b67ebb82308ab8fa7d8a5c74c10f59fd7761e7335303aef3acf6c2f22587d722.
- 동작 참조: [기존 UpDown 시트](../cast_onearm_updown/sheets/AirBoss_Cast_OneArm_UpDown_4x4_1024_v01.png).
- 생성 원본은 source/v01, 정규화 셀은 frames/v01, 시트·meta 보관본은 sheets에 있다.
- [키 포즈 최초 요청](keyframe10_prompt.txt), [수정 1](keyframe10_revision_prompt.txt), [수정 2](keyframe10_revision2_prompt.txt).
- [프레임별 실행 프롬프트](prompts_final_v01.json). 26~30은 Idle만 참조하도록 바꿨다. 이전 계획은 prompts_v01.json에 보존.
- [실행한 복귀 보정](refinement_executed_v01.json), [10번 홀드 보정](frame10_final_prompt.txt), [10번 여백 보정](frame10_safety_prompt.txt). refinement_prompts_v01.json의 09·11 수정안은 원본 확대 검사에서 이미 4개인 것을 확인해 실행하지 않았다.
- 최종 선택 우선순위: safe → final → revised → generated. [조립 스크립트](build_sheet.py)는 픽셀 조립·정규화와 메타 작성용이며 자세를 절차적으로 생성하지 않는다.
