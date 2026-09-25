# 등반 구역의 외부 동굴 외관

2026-09-25 사용자 요청: 분리된 벽 조립 대신 외부에서 등반 구역이 하나의 동굴로 보이도록 교체.

- 내장 image_gen으로 컨셉과 이전 Unity 화면을 참조해 새 외관 제작. 전체 프롬프트는 [Prompt.txt](Prompt.txt).
- 소재: ../../../Assets/04.Sprites/Backgrounds/ClimbMap/BG_Climb_ExteriorMountain_v01.png. 실제 1024×1536 RGBA, PPU 100, 무압축, mipmap off. 실제 표시는 독립 메시의 48×72 월드 영역 (X12.5~60.5, Y-3~69)이며 PPU가 메시 크기를 결정하지 않는다.
- Cave_ExteriorMountain_v01: GA1_World order35, entranceRole5. 출입구의 투명 부분 뒤에는 별도 Cave_ExteriorDoorDarkness(order34)를 배치해 외부 배경이 비치지 않게 했다.
- 이전 ExteriorUpperShell/ExteriorFacade 오브젝트는 비활성 보존. 내부 암벽/배경은 외부에서 숨기는 role4로 변경. 기존 입구/눈/고드름도 외부에서 숨기고 내부에서는 기존 표시를 유지.
- 입구 X34.5 및 히스테리시스, 플레이어, 카메라, 콜라이더, 지형 배치를 변경하지 않았다. 새로운 외관은 시각용이며 등반할 수 있는 지형을 추가하지 않는다.
- HyunseoScene, Map_CompleteLayout, Decoration_CaveUpperBoundary 저장. 단독 장식 프리팹의 외관과 출입구 가림을 다른 씬에서 사용하면 MapPortalVisibility surfaces 등록이 필요하다.
- BEFORE/AFTER 0 접근, 1 진입 직전, 2 진입 직후, 3 내부 상단, 4 외부 상단을 16:9로 확인. 내부 두 화면(2,3)은 수정 전후 PNG 해시가 동일하다. 플레이어 위치는 촬영 후 복원했다.
- 연속 이동 플레이 테스트는 미실시. 전체 맵 밖 카메라 위치나 임의로 변경한 줌에 대한 무한 범위 보장은 아니다.
- Before에 직전 씬/프리팹 백업 보관. 이후 사용자 배치를 덮어쓰는 전체 복원은 피한다.
