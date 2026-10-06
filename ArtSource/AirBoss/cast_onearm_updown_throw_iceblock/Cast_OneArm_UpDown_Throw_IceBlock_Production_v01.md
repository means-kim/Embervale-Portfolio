# AirBoss Cast OneArm UpDown — Throw IceBlock v01

2026-10-06. 사용자 요청: 캐릭터 왼팔(정면 화면 오른쪽)을 머리 위로 올려 잠깐 힘을 모은 뒤, 몸통을 동반해 사선 전방으로 던진다. 손에 구체·발사체·얼음 덩어리를 그리지 않는다. 32프레임 멀티스프라이트이며 파일 이름 끝에 `Throw_IceBlock`을 붙였다.

## 최종 결과

- 고유 RGBA 그림 **32개**: 첫 셀은 Idle 마스터를 보존한 공통 확장 셀, 나머지 31개는 내장 image_gen으로 생성한 개별 포즈.
- **1152×1152 RGBA 셀**, Center (576,576), **PPU 24**.
- **4×4, 4608×4608 시트 두 장**. Part1 00~15, Part2 16~31.
- PNG 위에서부터 왼쪽→오른쪽 순서. Unity Rect는 `(j%4)*1152, (3-floor(j/4))*1152, 1152, 1152`.
- Unity Sprite Multiple 각 16개, Full Rect, Bilinear, Uncompressed, mipmap off, NPOT None, Max Size 8192. 실제 임포트 해상도는 **4608**, 8192는 상한이며 원본 확대가 아니다.
- 새 클립·Animator·씬·프리팹·게임 코드·공격 이벤트를 생성하거나 수정하지 않았다.

| 자료 | 링크 |
| --- | --- |
| Part1, 00~15 | [PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Part1_4x4_1152_v01_Throw_IceBlock.png) |
| Part2, 16~31 | [PNG](../../../Assets/04.Sprites/AirBoss/AirBoss_Cast_OneArm_UpDown_Part2_4x4_1152_v01_Throw_IceBlock.png) |
| 개별 최종 셀 | [frames/final_v01](frames/final_v01/) |
| 재생 미리보기 | [preview_v01.html](preview_v01.html) |
| 전체 이미지 비교 | [어두운 배경](contact_final_v01.png), [밝은 배경](contact_light_v01.png) |
| 픽셀·여백·해시 | [validation_v01.json](validation_v01.json) |
| Unity 실제 읽기 결과 | [unity_validation_v01.json](unity_validation_v01.json) |
| 생성 번호 → 최종 순서 | [frame_map_v01.json](frame_map_v01.json) |
| 제안 타이밍 | [timing_v01.json](timing_v01.json) |
| 원본과 수정 프롬프트 49개 | [Docs 기록](../../../Docs/Boss/AirBoss/AirBoss_Cast_OneArm_UpDown_Throw_IceBlock_prompts_v01.json) |

## 1024에서 공통 1152로 확장한 이유

처음에는 기존 1024 셀을 목표로 제작했다. 일부 긴 천 끝이 64px 안전 여백에 들어왔고, 국소적으로 천을 접는 보정은 인접 프레임과 길이 차이가 커져 최종에서 제외했다. 공통 제작 규칙의 최대 포즈·공통 캔버스 검토 항목에 따라 이 신규 동작 전체에 동일한 확장 셀을 적용했다.

생성 요청은 1024×1024, 실제 신규 도구 출력은 1254×1254였다. 모든 신규 그림의 **전체 캔버스**를 동일한 방식으로 1024×1024 정규화한 뒤, 상하좌우에 각각 **64px 투명 패딩**을 더했다. 실루엣 fit, Trim, 개별 몸 축소, 가로세로 비균등 변형, 재중앙 정렬은 적용하지 않았다.

본체 픽셀 좌표와 피벗 모두 같은 64px가 더해졌으므로 `(x+64-576)/24 = (x-512)/24`다. 패딩 때문에 신체의 월드 배율이 변하지 않는다. Unity의 새 셀 전체 크기와 실제 보이는 신체 크기는 구분한다.

현재 공통 Idle 마스터 자체는 바꾸지 않았다. [이 작업 전용 확장 기준](reference/AirBoss_Idle_Master_1152_v01_Throw_IceBlock.png)은 원래 마스터를 동일 픽셀로 중앙에 넣은 것이다. 00의 안쪽 1024 영역은 원본 RGBA와 완전히 같음을 검사했다. 원본 및 고정 버전 SHA-256은 모두 `B67EBB82308AB8FA7D8A5C74C10F59FD7761E7335303AEF3ACF6C2F22587D722`로 유지됐다.

## 동작과 제안 시간

- 00: 원래 Idle.
- 01~09: 화면 오른쪽의 왼팔 상승.
- 10~15: 머리 위 홀드와 힘 모으기. 구체 없이 빈손·어깨·몸통으로 표현.
- 16~19: 사선 전방 투척.
- 20~25: 팔·상체·천의 후행.
- 26~31: 대기 자세 복귀. 31은 새 그림이며 00의 복제본이 아니다.

생성 포즈의 손 높이를 비교해 전반부를 연결 순서로 재배치했다. 원본 번호와 최종 번호는 frame_map에 모두 남겼다. 연결 그림을 단순 역순 복제나 반투명 겹침으로 대신하지 않았다.

제안 타이밍은 총 **2.4초**: 00~07 각90ms, 08~15 각85ms, 16~19 각40ms, 20~25 각55ms, 26~31 각85ms. 실제 Unity 클립에 적용하지 않은 아트 검토용 시간표다. 15→16의 시트 경계는 시간 지연 없이 이어진다.

## 제작 및 검증 범위

- 내장 image_gen 사용. 실제 마스터와 인접 주요 포즈를 참조해 새 포즈 및 국소 팔 보정을 생성했다.
- 원본 그림/프롬프트/채택하지 않은 보정은 source/v01에 보존했다.
- 최초 1024 검토 셀은 frames/v01, 최종은 **frames/final_v01**이다. 두 폴더를 혼용하지 않는다.
- 초기 검토 셀 일부에서 분리된 alpha=1 테두리 노이즈만 정리해 본 이력이 있으나, 최종 셀은 생성 원본에서 다시 만들었다. **최종에는 알파 문턱 처리·테두리 자르기·국소 천 길이 보정을 사용하지 않았다.**
- 모든 최종 셀의 바깥64px 완전 투명, 32개 고유 RGBA 해시, 두 PNG 시트 저장 후 재추출 픽셀 완전 일치를 확인했다.
- 어두운/밝은 배경에서 전체 포즈와 손·천 흐름을 이미지로 비교했다.
- Unity 32개 실제 Sprite의 이름·Rect·Center (576,576)·PPU24 및 4608 해상도를 확인했다. PNG/meta는 Assets와 ArtSource 시트 폴더에 함께 보존했다.
- 브라우저 자동 검토 도구는 Windows sandbox 실행 오류로 열리지 않았다. HTML은 제공했지만 **브라우저 동적 재생, Unity 실제 애니메이션 재생, 게임 화면 자연스러움·최종 사용자 아트 승인은 미검증**이다.
- 생성 이미지의 천 주름·가면 세부·후행 포즈에는 그림별 차이가 있다. 숫자 검증을 디자인의 픽셀 동일성이나 동작의 최종 승인으로 해석하지 않는다.

[공통 지침](../AirBoss_Multisprite_Production_Rules_v1.md)과 [기존 오른팔 단팔 제작 기준](../cast_onearm_updown/Cast_OneArm_UpDown_Production_v01.md)을 참조했다. 이번 왼팔·무구체·32장 지시가 기존 오른팔·구체·16장 조건보다 우선한다.
