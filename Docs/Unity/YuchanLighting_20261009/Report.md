# YuchanScene 광원 최소 수정 — 2026-10-09

## 진단

- 이전 에이전트가 추가한 GlobalLight(0.1), GlobalPlayerLight(1.0), 맵/더미의 GA1_World 및 CheckPoint 두 개의 GA1_Actors 설정은 디스크와 에디터에 모두 존재했다.
- 현재 열린 씬은 dirty=False였다. 디스크와 에디터 복사본 SHA-256은 동일했다. 과거 충돌 여부는 확인할 수 없지만 작업 시점의 저장 불일치는 없었다.
- Player SpriteRenderer만 Default여서 두 광원의 대상에서 제외됐다. 수정 전 에디터 카메라 렌더에서 검은 실루엣으로 확인했다.
- 공용 Player.prefab의 Idle_00 스프라이트 변경은 작업 시작 전부터 존재했다. 광원 수리와 별개이므로 유지했다.

## 적용

- Unity 에디터에서 Player SpriteRenderer의 Sorting Layer를 GA1_Actors로 설정하고 Prefab 인스턴스 override로 저장했다.
- 실제 이번 디스크 변경은 YuchanScene의 m_SortingLayer=5, m_SortingLayerID=1722300004 두 항목뿐이다.
- 기존 맵/CheckPoint 레이어, 광원 프리팹/강도, 배치, 코드, 프로젝트 설정은 유지했다.
- HyunseoScene, MainTestScene 및 Player.prefab SHA-256은 시작 시점과 동일하다.

## 확인

- [비교 화면](Comparison.html): before-player는 검은 실루엣, after-player 및 reloaded-player는 밝은 원래 스프라이트. CheckPoint는 밝고 지형/더미는 어두운 상태를 유지한다.
- 씬 저장 후 재열기 성공. editor-inspect.txt에서 dirty=False 및 Player=GA1_Actors 확인.
- Play 진입/종료와 play-report.txt에서 Player 및 두 CheckPoint=GA1_Actors, 맵/더미=GA1_World, 광원 강도 0.1/1.0 유지 확인.
- Play 자동 카메라 렌더에는 Player가 잡히지 않았다. 이 캡처를 Play 시각 검증으로 사용하지 않는다. 실제 이동/점프/대시 중 화면 확인은 미완료이며, 게임 이동/카메라 코드는 변경하지 않았다.
- git diff --check 통과. 임시 에디터 도구는 제거하고 사용한 소스만 .cs.txt로 보존했다.

## 보존 파일

- YuchanScene.disk.before.unity.txt / YuchanScene.editor.before.unity.txt: 수정 전 디스크/열린 씬.
- Player.prefab.before.txt: 기존 수정까지 포함한 공용 프리팹 사본.
- protected-hashes.json: 보호 파일 시작 시점 해시.
- before/after/reloaded PNG: 에디터 카메라 렌더 비교.
- play-report.txt, editor-inspect.txt: 런타임 설정 및 최종 에디터 상태.
