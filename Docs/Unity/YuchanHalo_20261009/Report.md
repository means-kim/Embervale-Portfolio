# YuchanScene 실제 발광·후광 — 2026-10-09

이 기록은 이전 YuchanLighting_20261009의 밝기 구분 보완 이후, 사용자가 요구한 실제 주변 발광을 추가한 최신 결과다.

## 변경

- 기존 PlayerLight.prefab을 참조한 Point Light2D 인스턴스 3개를 Player 및 CheckPoint 1/2의 자식 EmissionLight2D로 추가했다. 공용 프리팹은 수정하지 않았다.
- Player: 주황빛, Intensity 2, Inner Radius 0.6, Outer Radius 5.
- CheckPoint 1/2: 따뜻한 금빛, Intensity 2, Inner Radius 0.6, Outer Radius 4.
- 각 광원은 GA1_World를 실제로 비추며 Volumetric Enabled=true, Volume Intensity=0.18로 빈 배경에도 부드러운 후광을 표시한다. 별도 후광 스프라이트/색 틴트는 추가하지 않았다.
- 기존 GlobalLight 0.1과 GlobalPlayerLight 1은 주변 어둠 및 발광 대상의 가독성을 위한 기본 조명으로 유지했다.
- Camera Background Color만 어두운 청흑색(0.018, 0.022, 0.035)으로 바꿨다. 카메라 클리어 색은 Light2D 영향을 받지 않아 기존 파란 배경을 별도로 조정했다.
- 광원 위치는 실제 SpriteRenderer bounds 중심으로 설정했다. 부모를 따라 이동한다.

## 실제 확인

- [비교](Comparison.html): Unity URP 카메라 렌더에서 원형 후광과 체크포인트 아래 지형의 국소 밝기 변화 확인.
- Play 진입 후 Player가 Y=-7.46에서 -12.52로 낙하했고, 해당 광원도 Y=-6.31에서 -11.37로 같은 거리 이동했다.
- play-Player.png 및 play-CheckPoint_2.png에서 런타임 후광과 바닥 조명 확인. 임시 검증 카메라로 URP 렌더를 얻었으며, 기존 게임 카메라 배치는 변경하지 않았다.
- 저장 후 YuchanScene 재열기 및 광원 3개 유지, dirty=False/play=False 확인.
- 원본 HyunseoScene/MainTestScene 및 공용 Player.prefab은 이전 작업 시점 해시와 동일하다. 기존 공용 Player.prefab의 미커밋 스프라이트 변경은 유지했다.
- git diff --check 통과. 임시 에디터 도구는 제거하고 소스만 .cs.txt로 보존했다.
- 전체 이동 동선, 점프/대시/체크포인트 상호작용은 이번 조명 검증 범위가 아니다.

## Inspector 조정

Player 또는 Map/CheckPoints/CheckPoint 1·2 아래 EmissionLight2D의 Intensity, Outer Radius, Volumetric/Volume Intensity를 조절할 수 있다. Point 광원은 주변 지형을 밝히고 Volume은 공기 중 보이는 후광을 조절한다. 프리팹 Apply 없이 씬 override로 유지한다.

수정 전 디스크 및 열린 에디터 씬은 disk.before.unity.txt, editor.before.unity.txt에 각각 보존했다.
