# AirBoss Cast_BothArms_Raise v01

2026-09-30. Raise 단일 동작의 멀티 스프라이트 16장과 Unity Multiple 등록을 완료했다. 클립은 만들지 않았다.

[제작 기준](../../../ArtSource/AirBoss/cast_botharms_raise/Cast_BothArms_Raise_Production_v01.md) · [공통 규칙](../../../ArtSource/AirBoss/AirBoss_Multisprite_Production_Rules_v1.md) · [콘티](../../../ArtSource/AirBoss/references/AirBoss_Storyboard_v01.png)

LiftDrop은 양손을 가까이 모아 응축한 뒤 강하게 내리친다. Raise는 높은 V자로 양손을 넓게 벌리고 손가락을 펼쳐 시전을 유지하며, 구체와 양손 사이 에너지가 서서히 약해진 후 팔을 천천히 내린다. 프레임 05~09는 유지, 10~15는 종료·복귀로 역할을 구분했다. 다른 동작의 프레임이나 역순 복사를 사용하지 않았다. 00만 공통 Idle 마스터 그대로다.

## 제작 및 출력

- 내장 ImageGen으로 프레임별 생성·수정. 매번 고정 Idle 마스터와 인접 키포즈를 참조.
- 요청 1024×1024, 실제 생성 원본 크기는 프레임별 metrics.json에 기록.
- 전체 정사각형 캔버스만 1024로 정규화. 실루엣에 따른 Fit·Trim·재중앙정렬·개별 체격 보정 없음.
- Node sharp로 alpha 검사, 분리된 alpha 1~7 잡점 정리, PNG 셀·시트 조립. 본체·연결된 천·발광은 alpha 임계값으로 잘라내지 않음.
- 셀 1024×1024 RGBA 16장, 4×4 시트 4096×4096, Center (512,512), PPU 24.
- 최초 최대 포즈는 발광이 상단 64px 여백을 침범해 구체·발광을 수정. 최초 시도도 source/v01에 보존.
- 14번의 손이 높게 남은 초기안을 보존하고 낮은 손 자세로 다시 제작했다. 최종 비교에서 05번 구체의 크기·밝기를 04·06에 가깝게 수정했으며 이전 유지 기준 그림도 보존했다.

## 파일

- [개별 프레임](../../../ArtSource/AirBoss/cast_botharms_raise/frames/v01/)
- [제작 시트](../../../ArtSource/AirBoss/cast_botharms_raise/sheets/AirBoss_Cast_BothArms_Raise_4x4_1024_v01.png)
- [Unity PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_Raise_4x4_1024_v01.png)
- [어두운 바탕 검토](AirBoss_Cast_BothArms_Raise_v01_contact_dark.png) · [밝은 바탕 검토](AirBoss_Cast_BothArms_Raise_v01_contact_light.png)
- [실제 프롬프트](AirBoss_Cast_BothArms_Raise_v01.prompts.json) · [픽셀 검사](AirBoss_Cast_BothArms_Raise_v01.metrics.json)
- [LiftDrop과 동일 셀 크기 비교](AirBoss_Cast_BothArms_Raise_v01_vs_LiftDrop.png) · [Unity 조회 결과](AirBoss_Cast_BothArms_Raise_v01.unity.json)

## 완료 검증

- 00: 고정 Idle 마스터와 SHA-256 및 원본 파일 동일. 마스터를 재생성·재보간하지 않았다.
- 01~15: 실제 생성 원본은 모두 1254×1254. 전체 캔버스를 동일 비율 1024/1254로 정규화했다.
- 16개 셀 모두 RGBA 1024×1024, 고유 그림. alpha>0 기준 사방 64px 바깥 픽셀 0. 가장 작은 여백은 07번 위쪽 64px다.
- 시트는 4096×4096. 16개 셀을 다시 추출해 개별 PNG의 RGBA 픽셀과 동일함을 검사했다. 어두운·밝은 바탕에서 전체 순서와 외곽을 정적으로 검토했다.
- Unity 6000.3.22f1에서 Multiple 16개, 모든 셀 1024×1024, Center (512,512), PPU 24, 실제 텍스처 4096×4096 확인. Mipmap Off, Bilinear, Clamp, Uncompressed. 콘솔 오류 조회 0개.
- 새 PNG GUID: `54c9c1ab850d4d11a1edc67e2e1fc401`. 시트 SHA-256: `df62bfeb14ff992dd58e3a330ce2837e5c72b5c06c83dd619f18068c0c59ff1e`.
- 기존 씬·AirBoss 애니메이션·스프라이트·제작 원본 206개 파일의 해시가 시작 전과 같다. [보존 확인](AirBoss_Cast_BothArms_Raise_v01.protected_after.json).
- 정적 스프라이트와 임포트 확인이며 실제 게임 재생·최종 아트 승인·Transform 크기 검증을 뜻하지 않는다. 클립·Animator·이벤트·피해 판정·다른 동작·씬은 변경하지 않았다.

## 재생 의도와 범위

기획서의 3.6초를 참고하면 준비·상승 00~04에 1.2초, 유지 05~09에 1.2초, 종료·복귀 10~15에 1.2초를 배분할 수 있다. 이는 향후 클립 제작 시 검토할 배분이며, 이번에는 클립·이벤트·Animator·게임 코드·씬을 만들거나 변경하지 않는다. 실제 재생의 자연스러움과 게임 화면에서의 크기·밝기는 미검증이다.
