# 맵 아트 작업 인계

최종 정리: 2026-09-23. 이 문서는 현재 맵 아트 작업의 상태를 정리한다. 다른 PC에서도 프로젝트 파일과 함께 가져간다.

## 먼저 읽을 자료

| 자료 | 역할 |
|---|---|
| [AGENTS.md](../AGENTS.md) | Codex의 역할과 현재 작업 규칙 |
| [Bg_Concept.png](../Assets/04.Sprites/Bg_Concept.png) | 사용자가 지정한 맵 아트 기준 |
| [스프라이트 폴더 설명](../Assets/04.Sprites/README.md) | 현재 보유한 맵 소재와 삭제된 과거 작업 |
| [배경 제작 규칙](../Assets/04.Sprites/Backgrounds/README.md) | 분리 제작과 일관성 유지 방식 |
| [시작맵 소재 상태](../Assets/04.Sprites/Backgrounds/StartMap/README.md) | 현재 결과물, 실제 사양, 남은 검토 |
| [시작맵 상세 제작 기록](Backgrounds/StartMap_v01.md) | 원본과 결과 비교, 프롬프트, 수정 이력 |
| [캐릭터 크기 참고](../Assets/05.Animation/PLAYER_SCALE_REFERENCE.md) | 캐릭터 PPU와 배경 스케일 비교 |

## 프로젝트 방향

할로우나이트·실크송·오리 시리즈를 참고하는 2D 플랫포머 메트로바니아다. 달빛 아래의 설산, 고목 숲, 얼음 동굴로 이어지는 하나의 큰 맵을 탐험하며 보스를 만난다.

구역 순서와 원본의 위치 관계:

1. 시작맵: 원본 왼쪽의 눈 덮인 숲. 먼 성과 다리, 설산, 거대한 나무가 보인다.
2. 등반맵: 시작맵 오른쪽의 큰 고목과 절벽 사이를 올라가는 구간.
3. 보스전 통로: 등반 위쪽에서 오른쪽으로 이어지는 좁은 통로.
4. 보스전맵1: 상부 오른쪽의 설산을 배경으로 한 전투 구역.
5. 낙하 통로: 상부 구역에서 아래 동굴로 이어지는 수직 공간.
6. 보스전맵2: 하부의 넓은 얼음 동굴 전투 구역.

원본에 있는 성소 건축물은 사용자 요청으로 제외했다. 귀환 리프트도 필요 없다. 배경 제작에 캐릭터·보스를 구워 넣지 않는다.

## 현재 작업 방식

- 사용자 추가 지시: 눈 제거는 실제 캐릭터 발 크기·접촉 범위에 맞춰 작은 단위로 촘촘하게 이루어져야 한다. v16의 넓은 구간 전환과 잔설 덩어리 간격은 구현 기준이 아니다. 구체적인 해상도와 간격은 실제 배치 크기에서 검토한다.

- [룸 구조와 플레이 지형·아트 제작 합의](Room_Art_Workflow.md)를 따른다. 장식 소재와 플레이 충돌을 구분하고, 지형 아트는 캐릭터·카메라·임시 플레이 지형 검증 후 맞춘다.

- Unity 씬 배치는 사용자가 담당한다.
- Codex는 합의된 소재를 하나씩 제작하고 원본 및 기존 소재와 비교한다.
- 원경 → 원경 구조물 → 중경 숲·안개 → 근경 고목·가지 → 지형·별도 눈 덮개 순으로 시작맵 기준을 잡는다.
- 다른 구역으로 넘어갈 때 원본과 사용자 확인을 받은 이전 소재를 계속 참조한다.
- 전체 최후방 배경이 이어지는 구도를 유지하되, 작은 PNG 한 장을 전체 맵으로 확대해서 해결하지 않는다.
- 밟으면 눈이 사라지는 표현, 배경 움직임과 조명은 장래 요구다. 현재 구현 작업은 하지 않는다.

## 현재 파일 상태

**최신 사용자 선택 — 카메라폭80%:** 현재 배치도X20~100%, Y40~100%, 캐릭터 높이약9.0%를 아트 검토 기준으로 확정했다. [선택 화면v02](Backgrounds/StartMap_CameraWindow_v02.html), [기준과 한계](Backgrounds/StartMap_CameraWindow_v02.md). 초기값·복원 버튼에 반영했다. 이전55%는 탐색 기록이며 실제 Unity 월드 카메라 수치는 아직 적용하지 않았다.

**최신 검토 — 부분 카메라 창:** [구간별16:9 화면](Backgrounds/StartMap_CameraWindow_v01.html), [결과와 한계](Backgrounds/StartMap_CameraWindow_v01.md). 전체 배치도의 일부를 시작·중간·동굴 진입으로 나누어 확인했다. 기본폭55%는 임시 탐색값이며 캐릭터 높이약13.1%도 확정값이 아니다. 전체 맵 맞춤 확장은 하지 않았고 신규 PNG·Unity 변경 없음.

**최신 사용자 지시 — 카메라는 전체 맵의 일부:**16:9는 뷰포트 비율이다. 전체 맵을 한 화면에 맞추는 확장안과5.42% 캐릭터 화면 높이 고정 가정은 철회했다. 이후 시작·중간·동굴 진입의 구간별 화면, 화면 밖 연결·카메라 이동 시 경계 노출을 우선 검토한다. 아래 원경 양옆 확장 우선 기록은 과거 가정이다. 실제 카메라 범위·플레이어Scale은 미정이며 Unity 설정은 변경하지 않았다.

**최신 사용자 확정 — 목표16:9:** 이후 배치 검토의 기준이다. 기존 캐릭터 화면 높이를 유지한 [좌우 범위 점검](Backgrounds/StartMap_16x9_Transition_v01.html)과 [확장 계획](Backgrounds/StartMap_16x9_Transition_v01.md)을 작성했다. 원경 양옆 확장이 우선이며16:9 완성 아트는 아직 제작 전. 캐릭터Scale·카메라 크기·출력 해상도는 미정. 아래 화면비 미확정 기록은 과거 상태다.

**최신 검토 — 플레이 화면비:** [4:3/16:9 비교](Backgrounds/StartMap_Framing_v01.html), [결과·필요 기준](Backgrounds/StartMap_Framing_v01.md). 너비 유지 시 캐릭터 높이 비율5.42→7.22% 및 상단 잘림, 높이 유지 시 좌우 표시 범위33.3% 증가를 확인했다. 목표 화면비는 사용자 확인 전이며 실제 카메라를 설정하지 않았다. 아트 배치는v28 유지.

**최신 검토 — 성 거리감 후보 미채택:** CastleBridge_v03(1448×1086 RGBA)을 제작·비교했으나 축소 합성에서 개선이 작고 성도 흐릿해져 **현재 성은v02, 최신 배치는v28 유지**. [전후 비교](Backgrounds/StartMap_CastleBridge_v03_Compare.html), [판단·사양·프롬프트](Backgrounds/StartMap_CastleBridge_v03.md). v03은 비교 후보로 보존하며 조립표에 자동 대체하지 않는다.

**최신 미세 조정 — 안개 v28:** 신규 PNG 없이 기존 안개 하단Y42→48%, 불투명도28→18%, 상단 불투명도22→16%로 조정했다. [최신 v28](Backgrounds/StartMap_Placement_v28.html), [전후 비교](Backgrounds/StartMap_MistPlacement_v28_Compare.html), [조립표](Backgrounds/StartMap_Assembly_v28.md). 숲 윤곽 차이는 작고 원경에 포함된 안개는 유지된다. 비교 토글·이미지 로드 확인, Unity 재현은 미검증.

**최신 수정 — 중경 숲 v03:** 1448×1086 RGBA, 큰 나무 높이와 중앙 밀도를 낮춰 성 아래 능선·골짜기의 여백을 넓혔다. [최신 v27](Backgrounds/StartMap_Placement_v27.html), [전후 비교](Backgrounds/StartMap_ForestMid_v03_Compare.html), [사양·프롬프트·한계](Backgrounds/StartMap_ForestMid_v03.md), [최신 조립표](Backgrounds/StartMap_Assembly_v27.md). v02와 다른 소재의 배치는 보존. 현재 합성은v03 후보이며 개별 승인·실플레이 검증은 남았다.

**최신 정리 — 통합 배치 v26:** 신규 소재 추가 없이 18개 표시 항목(고유17파일)을 실제 PNG·meta와 대조했다. 눈 4쌍의 해상도·PPU·피벗·좌표 일치. [최신 검토 화면](Backgrounds/StartMap_Placement_v26.html), [Unity 조립 참고표·남은 차이](Backgrounds/StartMap_Assembly_v26.md), [상대 배치 데이터](Backgrounds/StartMap_Placement_v26.json). 확대 보기·참고선·위치 버튼·숲 바위와 눈 동시 숨김·동굴 상대 이동을 확인했다. 현재 위치는 v25 그대로이며 Unity 좌표·최종 Scale이 아니다. 후속 아트 검토 우선 항목은 중경 숲 밀도와 원경 여백이다.

**최신 추가 — 숲 바위 눈:** ForestRocksSnow_v01(1774×887 RGBA)을 바위와 동일 캔버스의 독립 눈으로 제작했다. [최신 v25](Backgrounds/StartMap_Placement_v25.html), [사양·프롬프트·한계](Backgrounds/StartMap_ForestRocksSnow_v01.md). 확대 접합·전체 합성·표시 전환 확인. 최종 카메라·Unity·눈 제거 동작은 미검증이다.

**최신 추가 — 숲의 낮은 바위:** ForestRocks_v01(실제1774×887 RGBA)을 이동로 뒤의 독립 장식으로 제작했다. [최신 v24](Backgrounds/StartMap_Placement_v24.html), [사양·프롬프트·검토](Backgrounds/StartMap_ForestRocks_v01.md). 캐릭터 약0.7배 높이, 정지 합성·확대 접합·가시성·표시 토글 확인. 눈·충돌 제외, 최종 플레이 검증은 남았다.

**최신 추가 — 그루터기 분리 눈:** StumpSnow_v01(1254×1254 RGBA)을 StumpNear_v01과 같은 캔버스로 제작했다. [최신 v23](Backgrounds/StartMap_Placement_v23.html), [사양·프롬프트·차이](Backgrounds/StartMap_StumpSnow_v01.md). 확대 접합과 정지 합성·표시 전환 확인. 컨셉보다 얇은 적설이며 실제 카메라 재검토가 필요하다. Unity·눈 제거 동작은 미구현이다.

**최신 추가 — 시작 숲 그루터기:** StumpNear_v01(실제1254×1254 RGBA)을 별도 장식으로 제작했다. [최신 v22](Backgrounds/StartMap_Placement_v22.html), [사양·프롬프트](Backgrounds/StartMap_StumpNear_v01.md). 캐릭터 약1.69배 높이의 정지 합성·가림·표시 토글을 확인했다. 눈·충돌·발판은 없으며 Unity 검증은 미완료다.

**최신 추가 — 동굴 외부 암벽 눈:** CaveSnow_v01(1254×1254 RGBA)을 동굴과 같은 캔버스의 독립 소재로 제작했다. [최신 v21](Backgrounds/StartMap_Placement_v21.html), [사양·프롬프트·한계](Backgrounds/StartMap_CaveSnow_v01.md). 정지 합성·확대 정렬·표시 토글·동굴 이동 동행을 확인했다. Unity 배치와 눈 제거 동작은 미구현이다.

**최신 추가 — 동굴 천장 고드름:** CaveIcicles_v01(실제1774×887 RGBA)을 암벽과 분리해 제작했다. [최신 v20](Backgrounds/StartMap_Placement_v20.html), [사양·프롬프트](Backgrounds/StartMap_CaveIcicles_v01.md). 천장 접합과 캐릭터 가시성을 정지 합성에서 확인했다. 장식 후보이며 점프 여유·실제 충돌 검증과 낙하·피해 기능은 없다.

**최신 추가 — 낮은 관목:** ShrubsNear_v01(실제1774×887 RGBA)을 제작해 시작 숲의 지면·캐릭터 뒤에 배치했다. [최신 v19](Backgrounds/StartMap_Placement_v19.html), [사양·프롬프트·검토](Backgrounds/StartMap_ShrubsNear_v01.md). 장식용이며 가시·발판이 아니다. 축소 화면 가독성은 최종 카메라에서 재확인이 필요하다. 눈 동작 구현은 계속 보류한다.

**최신 작업 — 오브젝트 제작 복귀:** 눈 제거의 상세 연출 검토는 중단했다. 동굴 입구 뒤쪽 장식 바위 CaveRubble_v01(실제1774×887 RGBA)을 제작하고 [최신 배치 v18](Backgrounds/StartMap_Placement_v18.html)에서 확인했다. [사양·프롬프트](Backgrounds/StartMap_CaveRubble_v01.md). 눈은 분리 준비만 유지하고 실제 동작은 배치·크기 확정 후 진행한다.

**최신 검토 2026-09-23 — 발 폭 기준:** [v17](Backgrounds/StartMap_Placement_v17.html)에 작고 촘촘한 타원 눈 표시 마스크를 적용했다. Run_00 오른발 접지 부근 폭45px 표본을 컨셉 비교폭 약3.48px로 환산했다. [측정·한계](Backgrounds/StartMap_SnowContact_v17.md). 폭과 간격은 검토 후보이며 게임 접촉·흔적 구현은 아니다. PNG·Unity 씬은 변경하지 않았다.

**최신 추가 — 잔설 아트:** GroundSnowResidual_v01(1774×887 RGBA)을 제작하고 [v16](Backgrounds/StartMap_Placement_v16.html)에서 밟기 전·잔설·눈 제거 상태를 구간별로 비교한다. [프롬프트·확인 범위](Backgrounds/StartMap_GroundSnowResidual_v01.md). HTML 표시 마스크만 있으며 Unity 눈 제거·발 접촉·흔적 저장 기능은 미구현이다. 축소 화면의 잔설 차이는 작아 실제 카메라에서 재검토가 필요하다.

**최신 추가 — 별도 지면 눈:** GroundSnow_v01(1774×887 RGBA)을 연결 바위와 동일 캔버스로 제작했다. [최신 v15](Backgrounds/StartMap_Placement_v15.html), [사양·프롬프트·한계](Backgrounds/StartMap_GroundSnow_v01.md). 확대 겹침과 눈 토글을 확인했고 HTML 밝기80%로 검토했다. 단일 눈 스프라이트이며 부분 제거 기능·실제 충돌·접지는 미검증이다.

**최신 추가 — 숲·동굴 연결 지면:** 별도 바위 지면 후보 GroundApproach_v01(실제1774×887 RGBA)을 제작했다. [최신 합성 v14](Backgrounds/StartMap_Placement_v14.html), [실제 사양·프롬프트·검토](Backgrounds/StartMap_GroundApproach_v01.md). 동굴 입구를 오른쪽으로 이동한 합성도 확인했다. 현재 위치는 최종 배치가 아니며 Unity에서는 연결 바닥과 충돌을 함께 맞춘다. 눈 덮개는 분리 예정이고 실플레이 검증은 미완료다.

**최신 방향 변경 — 오른쪽 고목 제외, 동굴 진입:** 사용자 요청으로 오른쪽 고목·뿌리·눈을 현재 배치에서 제외했다. 파일은 보존한다. 암벽 동굴 입구 후보(실제1254×1254 RGBA)를 제작했다. [최신 v13](Backgrounds/StartMap_Placement_v13.html), [사양·프롬프트·남은 검토](Backgrounds/StartMap_CaveEntrance_v01.md). 캐릭터 컨셉 비율은 유지. 숲 바닥 접합과 동굴 이후 구역 연결은 미정이며 고목 추가 수정은 중단한다.

**최신 수정 — 컨셉 비율 재검토:** 사용자가 통로 v01과 캐릭터 크기를 거부했다. 큰 중앙 아치를 작은 비대칭 뿌리 틈으로 수정한 Back/Front v02를 제작했다. 비교 캐릭터는 기존 약74.3px에서 컨셉 기준 약26px/480px 높이로 수정했다(약2.86배 과대 표시 오류 수정). [기록·프롬프트](Backgrounds/StartMap_TreePassage_v02.md), [최신 v12](Backgrounds/StartMap_Placement_v12.html). Unity 캐릭터 설정은 변경하지 않았으며 지면·눈·발판과 실플레이는 여전히 미검증이다. 아래 v01은 과거 미승인 시안이다.

**최신 수정 — 고목 통로 우선:** 사용자가 오른쪽 진행 시 고목 사이를 지나는 입구가 누락됐음을 지적했다. 뒤쪽 고목과 앞쪽 입구 테두리를 각각 1024×1536 RGBA로 제작했다. [제작·프롬프트·한계](Backgrounds/StartMap_TreePassage_v01.md), [최신 합성 v11](Backgrounds/StartMap_Placement_v11.html). 기존 고목과 눈은 보존하되 새 통로 합성에서 제외했다. 캐릭터 크기 참고와 앞뒤 레이어 합성을 확인했으며 실제 이동·충돌·접지는 미검증이다. 왼쪽 눈보다 통로 구조 검토가 우선한다.

**최신 추가(아래 과거 상태보다 우선):** 오른쪽 고목의 별도 장식 눈 v01을 제작했다. 실제 1024×1536 RGBA, 고목 v02와 같은 캔버스·PPU·pivot이다. [기록 및 프롬프트](Backgrounds/StartMap_TreeRightSnow_v01.md), [최신 합성 v10](Backgrounds/StartMap_Placement_v10.html). 주요 가지·뿌리 정렬을 확대 합성으로 확인했고, 전체 합성에서 눈 표시 밝기를 75%로 낮췄다(PNG 자체는 그대로). 왼쪽 고목 눈과 플레이 지형은 미제작, Unity 검증은 미완료다.

**최신 추가:** [왼쪽 고목 v01](../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeLeft_v01.png)을 제작했다. 1024×1536 RGBA, PPU 100. [생성·합성 기록](Backgrounds/StartMap_TreeLeft_v01.md), [최신 비교 v09](Backgrounds/StartMap_Placement_v09.html). 달·성·다리 가시성과 왼쪽·상단 연결을 정지 합성에서 확인했다. 지면·발판·눈 덮개는 미제작이고 접지·캐릭터 가림은 미검증이다.

**최신 수정:** 사용자가 차이 분석·해결 계획·실행을 맡겼다. 숲 v02(비대칭 군집·중앙 틈), 오른쪽 고목 v02(줄기 면 확대·반복 꼬임 축소)를 제작하고 매번 합성 확인했다. 기존 안개를 산 앞에도 22%로 배치한 [최종 비교 v08](Backgrounds/StartMap_Placement_v08.html)과 [실행 기록](Backgrounds/StartMap_Correction_v06.md)을 따른다. 원본·수정 전·수정 후를 같은 화면에 비교했다. 아래 v05의 사용자 판단 대기 상태는 이번 자율 수정으로 대체한다. 개별 아트의 최종 승인 및 플레이 검증은 여전히 미완료다.

**최신 상태(아래 이전 기록보다 우선):** 중경 숲 v01, 성 명도 수정 v02, 독립 안개 v01, 오른쪽 고목 v01을 제작하고 생성할 때마다 원본과 합성 비교했다. [최신 합성 v05 및 사양·프롬프트·판단](Backgrounds/StartMap_LayerReview_v05.md), [비교 이미지](Backgrounds/StartMap_Placement_v05.png). 고목의 굵기와 숲의 밀도를 사용자에게 확인받을 지점이다. 새 소재는 사용자 개별 승인 전이며, 지형·고목 눈 덮개·다른 구역은 미제작이다. Unity 씬은 변경하지 않았다.

사용자 최신 작업 방식: 일상적인 제작·수정마다 질문하지 않고 시작맵 소재를 순서대로 진행한다. 매번 컨셉 참조와 기존 레이어 합성을 확인하며, 판단이 필요한 지점에서 비교 자료와 함께 알린다. 자동으로 아트 승인이나 씬 작업 권한을 얻은 것으로 해석하지 않는다.

### 이전 작업 기록

배치 검토 최신: [컨셉과 원경·성 합성 비교](Backgrounds/StartMap_Placement_v01.md)를 완료했다. 두 레이어의 배치 후보를 확인했고, 성 받침 절벽의 폭·밝기가 두드러지는 점을 기록했다. 숲 포함 합성·구간 접합·실제 플레이 검증은 미완료이며 Unity 씬은 수정하지 않았다.

최신 추가: [먼 성·다리·받침 절벽 v01](../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v01.png) 제작 완료, 사용자 확인 전. 실제 1448×1086 RGBA 투명 PNG, PPU 100. 원경 v02와의 합성·최종 배치 크기는 미검증이다. [구조물 제작 기록](Backgrounds/StartMap_CastleBridge_v01.md)

2026-09-22 추가: 원경을 더 어둡게 수정한 [v02](../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png)를 제작했다. 실제 1448×1086 RGB, PPU 100이며 v01을 보존했다. 사용자가 “그래 다음”으로 후속 진행을 요청하여 v02를 현재 아트 기준으로 사용한다. 후속 소재인 먼 성·다리·받침 절벽 v01을 제작했으며 사용자 확인 전이다. 아래 v01 기록은 이전 제작 상태다. [v02 기록](Backgrounds/StartMap_v02.md)

| 항목 | 상태 |
|---|---|
| Bg_Concept | 사용자가 배치한 기준 이미지, 보존 |
| BG_Start_Far_v01.png | 시작맵 최후방 원경 1차본 제작됨 |
| 시작맵 v01 실제 사양 | 1448×1086 RGB PNG, PPU 100 |
| 시작맵 v01 사용자 확인 | 아직 확인되지 않음. 후속 질문을 아트 승인으로 간주하지 않음 |
| 다른 신규 레이어 | 미제작 |
| 이웃 구간 접합·레이어 합성 | 아직 미검증 |
| 전체 맵 최종 원경 | 미완성 |
| 이전 Frostbound 씬·소재·DATA | 사용자가 삭제함. 현재 작업의 기반으로 복원하지 않음 |

현재 PNG는 하늘·달 하나·구름·먼 설산·안개를 포함한다. 성, 숲, 고목, 지면, 발판, 성소는 포함하지 않는다. 첫 생성물에 섞였던 침엽수 실루엣은 수정 과정에서 제거했다.

## 확정하지 않은 값

- 전체 맵의 최종 Unity 월드 크기.
- 최종 카메라 Orthographic Size와 실제 표시 영역.
- 배경별 최종 파일 해상도, 분할 수, 배치 Scale 및 패럴랙스 범위.
- 시작맵 v01을 최종 기준 소재로 채택할지 여부.

이전에 논의한 캐릭터 1.2유닛, 카메라 Size 13.5/10.8, 다른 분석글의 Size 11.25 및 PPU 96/64는 검토 과정의 예시·후보값이다. 현재 캐릭터가 1.2유닛으로 확정되었다고 가정하거나 이 숫자들을 씬에 자동 적용하지 않는다. 캐릭터의 제공 사양은 PPU 200, 실제 그림 높이 예시 약 1.69유닛이다.

4K 화면을 고려한 제작 밀도가 논의되었지만 현재 배경의 실제 원본은 1448×1086이다. 요청한 크기와 생성 결과 크기를 혼동하지 않는다.

## 다음에 이어갈 일

1. 수정 v08을 현재 검토 기준으로 사용한다. 남은 차이를 스스로 분석·수정하며, 단순한 숲 밀도·고목 형태 판단마다 사용자 확인을 요구하지 않는다.
2. 지형·고목 눈 덮개 제작 전 실제 표시 범위와 크기 기준을 확인한다. 눈과 지형은 별도로 제작한다.
3. 이후에도 하나 제작할 때마다 컨셉 및 기존 소재와 겹쳐 확인하고, 필요하면 스스로 수정한다.
4. Unity 플레이 검증·이웃 구역 접합은 실제 준비가 된 뒤 별도로 진행하며 현재 완료로 표시하지 않는다.

현재 인계 문서 작성은 추가 스프라이트 제작 지시가 아니다.

## 다른 PC에서 이어가기

- Unity 버전 기록: [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt), 현재 6000.3.22f1.
- Assets, Packages, ProjectSettings, Docs, AGENTS.md를 프로젝트와 함께 가져간다. Unity 참조를 유지하도록 .meta 파일도 포함한다.
- Git으로 옮길 경우 미추적 파일도 포함됐는지 확인한다. 문서를 만든 것만으로 커밋·푸시·동기화가 완료되지는 않는다.
- Library, Temp, Logs, obj 등 재생성되는 로컬 캐시를 작업 기준 문서의 저장 위치로 사용하지 않는다.
- 새 PC에서 Unity 및 필요한 Codex 도구 연결을 확인한다.
- 새 Codex 작업에는 “AGENTS.md와 Docs/README.md를 읽고 Bg_Concept를 확인한 뒤 현재 맵 아트 작업을 이어가라”고 전달한다.
- 원본 대화를 그대로 이어갈 필요가 있으면 Codex 앱의 작업 이전 기능을 별도로 사용한다. 이 문서만으로 대화 기록이 이전되지는 않는다.

