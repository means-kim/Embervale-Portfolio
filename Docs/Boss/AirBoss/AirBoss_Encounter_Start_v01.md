# AirBoss Encounter_Start v01

2026-10-01. 사용자 승인에 따라 추천한 공중 각성 동작의 **16장 멀티 스프라이트와 Unity Multiple 등록**을 제작했다. Animation Clip·Animator·씬·프리팹·게임 코드는 생성하거나 변경하지 않았다.

[제작 기준](../../../ArtSource/AirBoss/encounter_start/Encounter_Start_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [기획서 ANIM-02](../../../ArtSource/AirBoss/Aerial%20Boss%20Design%20Document%20v1.md)

## 동작

이미 공중에 떠 있는 보스가 몸을 숙이며 팔을 모은 뒤, 몸을 일으키고 양팔을 옆으로 크게 펼쳐 위세를 드러낸다. 망토가 뒤따라 움직이고 손목과 천의 관성을 정리하며 Idle로 가까워진다. 기존 BothArms_Raise의 머리 위 V자·구체 시전과 구분했다. 화면 밖에서 날아오는 이동이나 순간이동은 없다.

- 00: 고정 Idle 마스터 원본 파일 그대로.
- 01~03: 고개·몸통을 숙이고 양팔을 몸 가까이 모음.
- 04~06: 몸통을 일으킨 뒤 팔을 바깥으로 펼침.
- 07~08: 넓은 펼침 유지. 기존 눈·갑옷 틈새 발광만 약하게 강조.
- 09: 유지 종료, 팔을 조금 내리며 손목 방향 전환.
- 10~12: 팔을 회수하고 손바닥을 위로 돌렸다가 아래로 풀기 시작.
- 13~15: 팔의 하강 관성을 수습하고 마스터의 낮은 손·비대칭 천 흐름으로 접근.

기획의 3.6초는 후속 클립 참고값이다. 이번에 노출 시간·타임라인·클립·재생 파일을 만들지 않았다.

## 파일

- [Unity 멀티 스프라이트 PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Encounter_Start_4x4_1024_v01.png) 및 같은 이름의 .meta.
- [제작 시트](../../../ArtSource/AirBoss/encounter_start/sheets/AirBoss_Encounter_Start_4x4_1024_v01.png).
- [개별 셀](../../../ArtSource/AirBoss/encounter_start/frames/v01/) · [생성 원본과 제외 후보](../../../ArtSource/AirBoss/encounter_start/source/v01/).
- [주요 자세](AirBoss_Encounter_Start_v01_keyposes.png) · [전체 어두운 바탕](AirBoss_Encounter_Start_v01_contact_dark.png) · [전체 밝은 바탕](AirBoss_Encounter_Start_v01_contact_light.png) · [동일 좌표 겹침 비교](AirBoss_Encounter_Start_v01_alignment.png).
- [실제 프롬프트](AirBoss_Encounter_Start_v01.prompts.json) · [픽셀 검사](AirBoss_Encounter_Start_v01.metrics.json) · [Unity 확인](AirBoss_Encounter_Start_v01.unity.json) · [기존 파일 보존 검사](AirBoss_Encounter_Start_v01.protected_after.json).
- [시트 조립·검사 스크립트](Build_AirBoss_Encounter_Start_v01.cjs) · [정적 검토 자료 스크립트](Inspect_AirBoss_Encounter_Start_v01.cjs).

## 제작 방식과 수정

내장 ImageGen을 사용했다. 21회 생성·수정에서 15장을 채택하고 6개 제외 후보를 보존했다. 고정 마스터를 그대로 사용하는 00과 합쳐 총 16장이다. 생성 요청은 1024×1024이며 실제 생성 원본은 모두 1254×1254였다.

전체 정사각형 캔버스를 공통 1024로 정규화했다. 실루엣 맞춤 축소·자동 Trim·재중앙정렬·좌우 반전·역순 복제는 적용하지 않았다. 시트는 검수한 셀 픽셀을 정수 위치에 그대로 복사했다. 각 프롬프트의 requestedFrame과 실제 채택 finalFrame을 구분했다. 일부 생성 자세는 실제 팔의 펼침·회수 정도에 따라 06·10·11·12에 배정했다.

손끝과 천의 좌우·하단 안전 여백 부족은 ImageGen에서 관절을 굽히거나 천 끝을 안쪽으로 접어 수정했다. 도형을 잘라내거나 프레임만 축소하지 않았다. sharp는 전체 캔버스 정규화·정적 비교·픽셀 검사·조립에 사용했다. alpha≥8 내용에서 16px 이상 떨어진 완전히 분리된 alpha 1~7 성분만 잡점으로 정리했으며 최종 15장 합계 6959px다. 연결된 반투명 천과 발광에는 일괄 문턱을 적용하지 않았다.

마스터와 고정 v02의 SHA-256 일치를 시작 전에 확인했고, 마스터·콘티·기존 BothArms_Raise 시트를 실제 이미지로 확인했다. 최초 마스터의 72% 축소 이력은 다시 적용하지 않았다.

## 검증 결과

- 1024×1024 RGBA 16장, 각 파일 SHA-256 서로 다름. 00은 마스터와 파일 해시까지 완전히 같다.
- 모든 alpha>0에 사방 64px 투명 여백 통과. 전체 최소 여백 64px, 범위 밖 픽셀 0개.
- 4×4·4096×4096 시트에서 재추출한 16개 셀이 개별 프레임과 픽셀 완전 일치. 제작 시트와 Unity 복사본 동일.
- 밝은·어두운 바탕에서 같은 셀 좌표로 전 프레임 정적 비교. 마스터/주요 자세의 겹침 자료는 검토 전용이며 실제 프레임에 합성하지 않았다.
- Unity 6000.3.22f1에서 Multiple 16개, 00~15 이름·순서·1024 Rect·Center (512,512)·PPU 24 확인.
- 실제 텍스처 4096×4096, Max Size 4096, Uncompressed, Bilinear, Clamp, Mipmap Off.
- 임포트 후 콘솔 Error 조회 0개.
- 작업 전부터 있던 씬·애니메이션·프리팹·스크립트·설정·Boss/AirBoss 등 1077개 파일의 전후 해시 일치.
- Unity PNG GUID: `cdd49980366245968dca69ac3178b6bc`.
- 시트 SHA-256: `2af875705368ec0a96c5e1ad7c6d784a16cccea97648fda1034797a00046eba8`.

정적 자세와 임포트 확인을 실제 게임 재생 검증이나 사용자 최종 아트 승인으로 취급하지 않는다. 프레임 사이의 미세한 가면 균열·갑옷 선·손 원근·천 접힘은 생성 그림의 정적 검토 범위다. 실제 속도의 자연스러움, Idle 전환, 표시 크기·카메라·게임 통합은 후속 검토 대상이다. 승인 범위에 따라 Animation Clip, GIF/APNG, Animator, 씬 또는 게임 동작을 만들지 않았다.

