# 동굴 입구/내부 상단 보완

원인: 외부 상단에 불투명 외피가 부족해 시작 하늘이 드러남. 내부에서는 기존 셰이더의 X=34.5 수직 분할이 검정 직선으로 노출됨.

## 적용

- 02_Climb/Cave_UpperBoundary 아래 외부 전용 암벽 외피 Cave_ExteriorUpperShell_v01, 내부 전용 검정 가림 Cave_InteriorUpperOcclusion_v01 추가.
- 기존 InteriorBack 텍스처 재사용 및 독립 메시/Material 생성. PNG 생성/수정 없음. 관련 자산은 07.Datas/Map/Meshes, 08.Materials/Map/Materials에 보관.
- 외피: GA1_World Order19, Y8~90, 불규칙한 좌측 윤곽과 0.8유닛 경계 알파. 기존 지형 뒤/원경 앞. 원본 입구 암벽은 유지.
- 내부 가림: GA1_World Order80, Y8~90, 왼쪽을 검게 덮고 불규칙한 우측 경계 1.5유닛 알파로 직선 절단면을 숨김. Player Actors보다 뒤. 플레이 공간/충돌 미변경.
- 셰이더 ReviewPortalMask.shader에 entranceRole5 추가: 가림 활성 중 외부에서만 표시, 내부에서는 alpha0. 기존 role0~4 처리는 변경하지 않음.
- 내부 가림은 기존 role4 사용(외부 투명). 두 항목 exitRole1. MapPortalVisibility 코드 변경 없음.
- Decoration_CaveUpperBoundary_v01.prefab 신규, Map_CompleteLayout.prefab에 이 그룹/Surfaces만 추가. HyunseoScene 저장.

## 확인 및 복구

- BEFORE/AFTER_0~3은 외부 플레이어 X30 / 내부 X45, 카메라 Y22/38/25/43의 16:9 실제 렌더. 플레이어 위치 캡처 후 복원.
- 외부 상단 하늘 누출과 내부 수직 경계의 완화를 시각 확인. 전체 플레이/충돌/모든 카메라 위치 검증은 아님.
- Before/에 변경 전 씬, 전체 맵 프리팹, 셰이더 보존. 이후 사용자 편집이 있으면 전체 덮어쓰기 대신 추가 그룹과 Surfaces 두 항목을 제거하고 셰이더 role5 추가 줄만 복구.
- 향후 입구 Anchor/구역을 옮길 때 이 보완 그룹도 함께 맞춰야 함. 현재 메시의 월드 배치를 기준으로 제작됨.
