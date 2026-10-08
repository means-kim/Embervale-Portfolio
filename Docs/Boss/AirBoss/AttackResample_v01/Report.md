# AirBoss 공격 보간 업스케일 v01

2026-10-08. AI 재묘사 없이 원본 디자인과 동작을 유지하는 사용자 요청에 따라 적용.

- 클립에 연결된 AirBossAttack 시트 11개를 적용. 미사용 IceCylinder_4x3_v01은 유지.
- 최대 변2048px 이하 시트는4배, 나머지는2배. 최종 최대8192px. Premultiplied-alpha Lanczos 보간으로 투명 경계의 검은 테두리를 방지.
- PPU와 Sprite rect/border를 동일 배율로 조정. GUID/localID/name, 월드 크기/피벗 보존. Transform, Collider, 데미지, 타이밍 변경 없음.
- 본체 업스케일은 그대로 유지.

## Scale 조사
- IceBlock50×50 → IceMass 시트4배.
- LargeIcePillar50×50 → 256셀 구버전 시트4배, Tall_v04 시트2배.
- Blizzard50×30 →4배.
- LargeLightning50×80, SmallLightning10×100, Horizontal 루트150×20(회전 후 축 달라짐) → VerticalGrowth 시트2배.
- LightningFlight35×35 →4배.
- SmallIcePillar20×20 → 흰색 IcePillar 시트2배. 기본 상태 IcePillar_White 확인. 별도 과거 Attack_IcePillar 상태의 외부 aseprite 원본은 변경하지 않음.
- 자세한 프리팹-클립-시트 연결은 PrefabLinks.txt.

## 검증
Unity 재임포트 후 확대된 원본 해상도로 로드됨. Sprite256개 GUID/localID/name/월드 크기/피벗 오류0. 애니메이션 키369개 참조·순서·시간 전후 동일. 보호 파일479개(씬/프리팹/클립/컨트롤러/타임라인/본체 PNG/meta) 해시 변경0.

Comparison.png는 대표 프레임을 동일 표시 크기로 비교한 정적 자료(왼쪽 이전/오른쪽 이후). 실제 카메라에서의 전투·Timeline 전체 재생 검증은 미실시. 보간은 원본에 없는 세부 정보를 복원하지 않으므로 극단적인 확대에서 개선에는 한계가 있다. 픽셀 수와 동일 포맷 텍스처 메모리 비용은2배 시트에서4배,4배 시트에서16배 증가한다. 실제 실행 성능은 미측정.

## 보존
Original에 원본 PNG/meta, Prepared에 적용 시트, Dimensions.json에 실제 치수. Paths.txt의 경로에 Original PNG/meta를 함께 복원하고 Unity 재임포트하면 롤백 가능. Apply.cs.txt는 완료 마커를 사용하는 적용 이력으로 임의 재실행 금지.
