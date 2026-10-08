# Player_Hyunseo 복귀·부활 테스트

사용자 요청에 따라 메인 Player 적용 전에 테스트 플레이어에 먼저 적용했다.

## 적용 범위

- 테스트 플레이어: [Player_Hyunseo.prefab](../../Assets/02.Scripts/Z_HyunseoTest/Prefabs/Player_Hyunseo.prefab).
- HyunseoTest의 Player, PlayerController, PlayerHit, PlayerMove, PlayerJump, PlayerWall, PlayerDash, PlayerAttack에 복귀/부활 상태 초기화 추가.
- 기존 [Respone.cs](../../Assets/02.Scripts/Map/Respone.cs)와 [ResponeZone.cs](../../Assets/02.Scripts/Map/ResponeZone.cs)는 지금 HyunseoTest.Player만 대상으로 한다. 메인 Player 연동은 아직 하지 않았다.
- Respone을 테스트 플레이어 프리팹 루트에 추가했다. 기존 씬 인스턴스도 프리팹을 통해 적용된다.
- 실제 체력 컴포넌트는 global::Health다. 기존 사용자가 추가한 ReviveFull을 사용하며 공용 Health와 메인 Player 코드는 변경하지 않았다.
- 씬 배치/체크포인트 위치, DeathCanvas 버튼 연결, 몬스터 초기화는 변경하지 않았다.

## 사용법

1. HyunseoScene을 Play로 실행하고 Player_Hyunseo의 HyunseoTest.Player 컴포넌트 메뉴를 연다.
2. Respone Test/Take 100 Damage: 피해 100.
3. Respone Test/Die: 사망 상태 진입.
4. Respone Test/Return With Damage: 피해 100 후 체크포인트 복귀. 치명적 피해라면 사망 유지.
5. Respone Test/Restart At Checkpoint: 체력 최대 회복, 사망/피격/이동 잠금 및 대시/공격/점프/벽 상태 초기화 후 재시도.

체크포인트가 지정되지 않으면 Play 시작 위치를 사용한다. 테스트 플레이어의 Border 충돌은 이제 Respone을 통해 피해 100 후 복귀한다. ResponeZone은 Is Trigger인 Collider2D가 있는 구역에 붙이고 _checkPoint를 지정해 사용한다. 해당 구역 복귀 시 재시도 체크포인트도 갱신된다. Border와 Trigger 구역을 같은 위치에 중복 배치하지 않는다.

버튼에는 Player_Hyunseo의 Respone.RestartTestPlayer() 또는 HyunseoTest.Player.RestartAtCheckpoint()를 연결할 수 있다. 이 메서드는 DeathCanvas 숨김이나 Time.timeScale 복원을 담당하지 않는다.

## 확인 결과

Unity에서 스크립트 컴파일 성공. Play 모드에서 직접 메서드/콜백을 실행하여 다음을 확인했다.

- 생존 복귀: 피해 100, 체크포인트 위치, 선속도/각속도 0.
- 치명적 복귀: 사망 상태 유지. 재시도 시 체력 최대/IsDead false/Idle/사망 이동 잠금 해제.
- 별도 소유자의 이동 잠금은 유지.
- 대시 중 재시도: 대시 종료 및 원래 중력 복원.
- ResponeZone.OnTriggerEnter2D 콜백이 테스트 플레이어를 찾고 복귀 피해 적용.
- 확인 후 Play 모드 종료, Console 오류 0.

실제 물리 트리거 통과, 전체 맵 낙하 동선, 엘리트·보스 초기화는 미검증/미연결이다.

## 사망 후 조작 불가 확인 및 재시도 UI 연결

후속 사용자 보고로 재확인했다. 사망 시 Health.IsDead=true, PlayerController.Death, 사망 이동 잠금이 남는 것은 확인됐지만, RetryButton의 부활 호출과 DeathCanvas 표시 처리가 연결되어 있지 않았다. 위치 이동 자체는 부활이 아니다.

- 사용자는 재시도 버튼을 눌러 부활하는 방식을 선택했다.
- HyunseoScene의 기존 DeathCanvas에 테스트 전용 PlayerDeathUI를 추가하고 Player_Hyunseo와 RetryButton을 연결했다. 기존 배치를 보존하여 씬을 저장했다.
- 사망 시 약 0.4초 페이드 후 재시도 버튼을 활성화한다. 투명도 0이었던 자식 CanvasGroup도 런타임에 표시 가능하게 한다.
- 재시도는 RestartAtCheckpoint를 호출한다. 체력·사망 상태·이동 잠금 복구, 이 UI가 비활성화했던 입력 복구, 오버레이 숨김을 함께 처리한다.
- Player.OnRestarted 이벤트로 Inspector 재시도도 같은 UI 정리를 거친다.
- Time.timeScale을 변경하지 않는다. 메인 Player와 보스/엘리트 초기화에는 적용하지 않는다.
- Play 모드에서 사망/페이드/버튼 활성화, 버튼 중앙 UI 레이캐스트의 첫 대상이 RetryButton임을 확인했다. 포인터 클릭 이벤트로 부활 후 체력 최대/입력 활성/이동 잠금 해제/오버레이 숨김 및 수평 이동 속도 발생을 확인했다. 반복 사망 후 두 번째 재시도도 통과했다.

## 전투방 부활 지점과 카메라 연결

후속 요청에 따라 HyunseoScene의 기존 CheckPoint 자식 마커에 테스트 전용 RespawnPoint를 추가했다. 위치는 변경하지 않았다.

| 전투방 | 진입 게이트 | 부활 마커 | 부활 카메라 |
| --- | --- | --- | --- |
| 엘리트 | Gate_EliteMonster | Elite (162, 137.4, 0) | Camera_Start |
| 보스 | Gate_Boss | Boss (262, 137.4, 0) | Camera_EliteMonster |

두 마커는 각각 해당 방의 왼쪽 게이트 바깥에 있으므로 게이트 왼쪽 카메라와 배경 상태를 복구한다. Camera_Start 이름이 전체 동굴까지 추적하는 현재 배치와 혼동될 수 있으나 새 카메라를 임의로 만들지 않았다.

- GateSwitch는 오른쪽으로 통과한 HyunseoTest.Player에 설정된 테스트 부활 지점을 등록한다. 메인 Player에는 적용하지 않는다.
- 플레이어 이동 시 Cinemachine에 순간이동을 알리고, 해당 부활 마커의 카메라·MapVisibility를 복구한다. 카메라 이전 상태를 무효화해 이전 위치의 추적 지연을 제거한다.
- AreaController.ResetForRetry로 해당 방의 등장 연출 취소 및 장벽 열기/Waiting 복구를 처리한다. 처치 완료 이력은 유지한다. 몬스터의 체력·패턴·남은 공격 초기화를 구현한 것은 아니다.
- 기존 진입금지 ResponeZone의 Cave/Secret/BossSecondPhase 마커에도 해당 위치의 카메라 정보를 연결했다. Vector3 복귀는 등록된 부활 문맥을 지우지 않는다.
- Play 모드에서 실제 GateSwitch 진입/이탈 콜백을 호출하여 각 방의 체크포인트 등록을 검증했다. 사망 후 RetryButton 이벤트를 통해 마커 위치로 복귀, 체력 회복, 이동 잠금 해제, 해당 카메라 활성화를 확인했다.
- 이후 실제 CinemachineBrain의 활성 카메라와 플레이어 뷰포트 위치를 확인했다. 엘리트 부활: Camera_Start, 약 (0.60, 0.50). 보스 부활 시점도 화면 내부로 확인했다.
- 물리 이동으로 게이트를 직접 통과한 전체 동선 및 실제 몬스터 피격 사망은 별도 플레이 검증 대상이다.

## RespawnPoint 폴더·타입 정리

사용자 요청으로 RespawnPoint.cs와 .meta를 Assets/02.Scripts/Map으로 이동했다. GUID는 유지했다. 이어서 HyunseoTest 네임스페이스를 제거하고 GateSwitch의 _testRespawnPoint를 _respawnPoint로 변경했다. FormerlySerializedAs로 기존 Inspector 연결을 보존하며, Respone과 테스트 Player의 참조도 새 타입에 맞췄다. RestoreView는 Transform을 받아 테스트 Player 타입에 의존하지 않는다. 실제 플레이어 부활 연동 대상은 계속 Player_Hyunseo다. Unity 컴파일 및 부활 마커 5개/엘리트·보스 게이트 연결 유지 확인, Console 오류 0.

## 몬스터 초기화 변경 롤백 (2026-10-06)

사용자 지시로 직전 작업의 몬스터 스크립트 11개 변경, MonsterRetryReset.cs/meta 및 AreaController의 몬스터 연동을 되돌렸다. 기존 플레이어/카메라/장벽 재시도 처리는 유지한다. 몬스터와 보스 관련 스크립트는 이후 직접 수정하지 않는다.
