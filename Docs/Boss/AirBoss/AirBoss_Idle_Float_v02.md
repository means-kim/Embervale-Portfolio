# AirBoss Idle v02 — 16장 연결 보완

2026-09-30. 사용자 요청으로 기존 8장 사이에 중간 동작 8장을 새로 제작했다. 이전 8장과 v01 시트는 보존했다.

## 결과

- [Unity 시트 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Idle_Float_4x4_1024_v02.png), 같은 경로의 `.meta`.
- [기존 Idle 클립](../../../Assets/05.Animation/AirBoss/AirBoss_Idle.anim): 새 16장으로 연결. 기존 GUID `e4f303def1b4e8b499900ad08009f649` 유지.
- [16장 실제 속도 APNG](AirBoss_Idle_Float_v02_loop.png), [8장/16장 비교 HTML](AirBoss_Idle_Float_v02_preview.html), [전체 정적 비교](AirBoss_Idle_Float_v02_contact_dark.png).
- [개별 셀](../../../ArtSource/AirBoss/idle/frames/v02/), [원본 생성 결과](../../../ArtSource/AirBoss/idle/source/Idle_Float_v02/), [ArtSource 시트](../../../ArtSource/AirBoss/idle/sheets/AirBoss_Idle_Float_4x4_1024_v02.png).

## 확정 사양과 순서

| 항목 | 값 |
| --- | --- |
| 고유 그림 | 16장: 기존 8장 + 새 중간 동작 8장 |
| 셀 / 시트 | 1024×1024 / 4096×4096 RGBA, 4열×4행 |
| Unity | Sprite Multiple 16개, PPU 24, 각 셀 Center (512,512) |
| 임포트 | Max Size 4096, 실제 4096×4096, Bilinear, Uncompressed, Mipmap Off, Clamp, Full Rect |
| 노출 / 루프 | 각 0.075초 / 전체 1.2초, Loop On |
| 클립 Sample Rate | 120: 0.075초를 9틱으로 정확히 표현하기 위한 타임라인 단위. 고유 그림 120장이 아님 |
| 안전 여백 | 모든 alpha>0 기준 64px 통과. 실제 최소 70px |

짝수 번호 00,02,…,14에는 기존 v01의 00,01,…,07을 파일 그대로 복사했다. 홀수 번호는 바로 앞뒤 기존 포즈 사이의 새 중간 동작이며, 15는 기존 07→00의 루프 연결이다. v02 프레임 `i`의 시작 시간은 `i × 0.075초`다.

```text
00 기존00   01 새00→01   02 기존01   03 새01→02
04 기존02   05 새02→03   06 기존03   07 새03→04
08 기존04   09 새04→05   10 기존05   11 새05→06
12 기존06   13 새06→07   14 기존07   15 새07→00
```

Unity Rect는 아래 원점 기준 `X=(i%4)×1024`, `Y=(3-floor(i/4))×1024`, 크기 1024×1024다. 15번도 0.075초 유지하도록 143/120초에 동일 Sprite의 종료 유지 키를 넣었다. 이는 17번째 고유 그림이나 별도 셀이 아니다. 1.2초에서 00으로 돌아간다.

## 제작과 보존

새 8장은 **내장 image_gen**을 각 구간마다 호출해 제작했다. 양쪽 기존 포즈를 실제 이미지로 참조했고, 프롬프트·참조 순서는 [prompts JSON](AirBoss_Idle_Float_v02.prompts.json)에 기록했다. 복제 프레임이나 반투명 교차로 중간 동작을 대신하지 않았다.

요청 캔버스는 1024×1024였으나 실제 생성 출력은 모두 1254×1254였다. 생성 캔버스 전체를 공통 비율 1024/1254로 정규화했다. 체격에 맞춘 개별 확대·축소나 외곽 Trim은 하지 않았다. 고정 가면 영역을 기준으로 생성 위치 오차만 최대 2px 정수 이동으로 등록했다. v01과 동일하게 가시 영역에서 16px 넘게 떨어진 독립 alpha<8 덩어리만 정리했다. 처리 수치·여백·파일 해시는 [metrics JSON](AirBoss_Idle_Float_v02.metrics.json), 재현 절차는 [조립 스크립트](Build_AirBoss_Idle_Float_v02.ps1)에 있다.

클립 변경 전 내용은 [before16 기록](AirBoss_Idle_before16_v02.anim.txt)에 보존했다. 기존 v01 PNG/meta, 마스터, TEST.anim, 씬, Animator Controller, 전역 설정은 변경하지 않았다.

## 검증 범위

- 기존 8장과 짝수 8장의 파일 해시 일치, 00과 마스터 일치, 고유 파일 16개 확인.
- 각 셀 RGBA와 저장 시트에서 추출한 픽셀 일치, 64px 안전 여백, 실제 해상도 확인.
- 정적 비교에서 전신·손·천 실루엣과 루프 연결 그림을 확인했다. 기존 그림을 보존했으므로 기존 포즈 사이의 형태 차이는 남으며, 추가 그림에도 세부 묘사 차이가 있다. 완전히 끊김 없는 최종 승인본으로 표시하지 않는다.
- Unity Preview Scene의 AnimationClipPlayable에서 두 루프의 32개 노출 구간을 샘플링하여 모두 기대한 Sprite와 일치함을 확인했다. 실제 Rect·PPU·Pivot·클립 길이·반복 결과는 [Unity 검증 JSON](AirBoss_Idle_Float_v02.unity.json)에 있다. Console Error 0개 확인.
- APNG는 16개 PNG의 압축 프레임 데이터를 그대로 포장하고 동일성을 검사했다. [APNG 포장 스크립트](Pack_AirBoss_Idle_Float_v02_APNG.cjs) 참조.
- 비교 HTML은 제작했으나 앱 브라우저가 file URL을 허용하지 않아 브라우저 재생 화면 검증은 하지 못했다. 사용자 실제 게임 화면에서의 자연스러움·카메라·Scale 검토는 별도다.
