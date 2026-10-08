# 번개 구체 비행 루프 16프레임 v01

2026-10-07. 사용자 요청: 공중 보스와 기존 공격 이펙트에 맞는 번개 구체의 비행 루프 16장을 우선 제작하고, 회전과 방전 느낌을 보고 다음 단계 진행 여부를 판단한다.

## 결과와 사용
- [Unity용 PNG](../../Assets/04.Sprites/AirBossAttack/Attack_LightningOrb_FlightLoop_AirBoss_16F_v01.png)와 같은 경로의 .meta.
- [16fps GIF](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/flight_loop_16fps.gif).
- [반복 재생 HTML](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/preview.html): 파일을 브라우저로 직접 열면 동작한다. 12/16/20/24fps, 어두움/중간/밝음 배경, 프레임 수동 선택, 검토용 직선 이동.
- 실제 최종 PNG 2048×2048 RGBA, 4열×4행, 셀 512×512, 좌상단부터 가로 순서 00~15.
- Unity Multiple Sprite 16개, Center (0.5,0.5), PPU 256, Full Rect, Bilinear, Clamp, Mipmap Off, NPOT None, Uncompressed. 최종 게임 크기와 Transform Scale은 미확정.
- 비행 루프 초안이다. 생성/소멸/명중 프레임, 별도 이동 잔상, Animator/AnimationClip/Prefab/콜라이더/이동 코드/씬 배치는 이번 제작에 포함하지 않았다.

## 참조와 제작
내장 ImageGen을 사용했다. [공중 보스 CrossBurst](../../Assets/04.Sprites/AirBoss/AirBoss_Cast_CrossBurst_Part1_4x4_1152_v01.png), [기존 세로 번개](../../Assets/04.Sprites/AirBossAttack/Attack_Lightning_VerticalGrowth_AirBoss_32F_v01.png)를 실제 이미지로 읽고 참조했다.
청백색 핵, 청보라색 나선, 비대칭 주변 번개 가지를 사용했다. 1차 생성 뒤 셀 여백과 회전 표식을 보완했다.
- [초기 프롬프트](LightningOrb_Loop_16F_v01_Prompt.txt)
- [보완 프롬프트](LightningOrb_Loop_16F_v01_RefinementPrompt.txt)
- [생성 원본 및 제작 자료](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/)
요청 해상도는 2048×2048이었으나 두 생성 원본의 실제 크기는 각각 1254×1254였다. 최종 2048×2048은 원본 16개 영역을 512×512 투명 셀로 재배치한 결과이며, 각 프레임이 네이티브 512 해상도로 생성됐다는 뜻은 아니다. 원본에는 약 150~200px 규모의 주요 발광 형태가 들어 있다.
그림을 확대·회전·재채색하거나 alpha를 지우지 않았다. 발광 핵을 기준으로 정수 픽셀 평행 이동하고 여백만 추가했다. [등록 좌표](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/frame_registration.csv), [패킹 스크립트](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/pack_atlas.ps1) 참조.
원본 프레임 사이 절단선의 최대 alpha는 1/255, 최종 셀 가장자리 alpha는 0이다. 극히 낮은 alpha 잔점은 원본 그대로 보존했다.

## 검토 상태
- Unity 6000.3.22f1에서 실제 16 Sprite, 모든 rect와 피봇, PPU, PNG 해상도를 확인했다. [Unity 검증 결과](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/unity_validation.json).
- 임포트 후 Console error 조회 0건. MinsuScene은 저장하거나 변경하지 않았다.
- HTML을 헤드리스 Edge에서 실제 열어 프레임 진행, 일시정지, 01/16 수동 이동, 어두운/밝은 배경을 확인했다. 페이지 JS 오류 0건. [어두운 배경](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/preview_dark.png), [밝은 배경](../../ArtSource/AirBossAttack/lightning_orb_loop_v01/preview_light.png).
- GIF는 16페이지, 무한 반복, 프레임 지연 60/60/70/60ms를 4회 사용하여 전체 1초로 구성했다. GIF는 배경 합성과 확대가 포함된 검토용이며, Unity용 원본은 RGBA PNG다.
- 미리보기에는 추가 Transform 회전·프레임 보간이 없다. 생성된 나선의 형태 변화와 주변 가지 방전 자체를 보여준다.
- 프롬프트의 프레임당 22.5도는 목표이며, 생성 결과가 정확히 같은 표면을 등속으로 한 바퀴 회전한다고 검증한 값이 아니다. 나선 형태는 유지되지만 형태 변화·방전의 비중이 있으며 회전 강도와 루프 이음새는 사용자 피드백 대상이다.
- 실제 게임 카메라, 최종 크기, 다수 발사, 물리 판정, 플레이 중 표현은 미검증이다. 생성·소멸 제작으로 자동 확장하지 않는다.

