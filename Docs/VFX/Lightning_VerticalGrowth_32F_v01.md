# 세로 성장 번개 32프레임 v01

2026-10-07. 사용자는 Attack_ShortLightning과 같은 디자인 계열의 다른 세로형 번개를 요청했다. 가는 시작에서 점차 굵어지는 32프레임, 중앙 피봇의 멀티 스프라이트만 제작하고 애니메이션 클립은 만들지 않는다. 회전과 콜라이더 설정은 사용자가 담당한다.

## 결과

- [Unity용 PNG](../../Assets/04.Sprites/AirBossAttack/Attack_Lightning_VerticalGrowth_AirBoss_32F_v01.png)와 같은 경로의 .meta.
- [재생·회전 미리보기](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/preview.html).
- [배경 위 전체 프레임 비교](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/contact_sheet_dark.png).
- 실제 PNG 3072×3072 RGBA, 8열×4행, 동일 셀 384×768. 좌상단부터 가로 방향으로 00~31.
- Unity Sprite Mode Multiple, 정확히 32개 Sprite, Center (0.5,0.5), 피봇 픽셀 (192,384), PPU 256.
- Bilinear, Clamp, Mipmap Off, NPOT None, Uncompressed, Max Size 4096, Full Rect. 생성형 물리 외곽선 비활성화.
- 애니메이션 클립, Animator Controller, 프리팹, 씬, 콜라이더, 게임 코드 변경 없음. 열려 있던 MinsuScene의 미저장 상태를 저장하거나 되돌리지 않았다.

## 디자인과 프레임 흐름

백색 중심선과 청보라색 발광을 유지했다. 기존과 다른 완만한 S형 주 줄기와 비대칭 가지를 사용했다. 위아래 끝은 가늘게 닫히고 지면·착탄 원·중력 방향 파편은 없어 사용자가 회전해 사용할 수 있다.

- 00~21: 가는 줄기에서 점점 굵어지는 성장.
- 22~25: 강한 방전, 최대 굵기까지의 변화와 분기 떨림.
- 26~31: 가지·중심선·잔광이 줄어드는 소멸. 마지막은 완전 빈 프레임이 아니라 희미한 잔광이다.

32장은 재생 시간과 별개다. HTML의 24fps / 약 1.33초는 비교용이며 Unity 재생 속도를 설정한 것이 아니다. 장식 가지와 잔광 폭은 충돌 범위 제안이 아니다.

## 실제 생성과 패킹

기본 내장 ImageGen을 사용했다. [1차 프롬프트](Lightning_VerticalGrowth_32F_v01_Prompt.txt), [8프레임 단위 보완 프롬프트 4개](Lightning_VerticalGrowth_32F_v01_RefinementPrompts.txt)를 보존한다.

참조는 [ShortLightning 원본](../../Assets/04.Sprites/AirBossAttack/Attack_Lightning_AirBoss_24F_v01.png)과 [공중 보스 번개 응축](../../Assets/04.Sprites/AirBoss/AirBoss_Cast_BothArms_LightningGather_Part2_4x2_1024_v02.png)이다. [현재 위치의 맵 컨셉](../../Assets/04.Sprites/UI/Bg_Concept.png)도 실제 이미지로 확인했다.

처음에는 4096×4096 전체 시트를 요청했으나 실제 반환은 1254×1254였다. 이후 2048×2048, 4열×2행 시트 4개를 요청했으며 실제 반환은 각각 1254×1254였다. 요청 해상도를 달성한 원본으로 표시하지 않는다.

각 생성 셀은 약 313~314×627 픽셀이다. 업스케일·회화적 수정 없이 원본 픽셀과 alpha를 복사하여 동일한 384×768 투명 캔버스에 패킹했다. 최종 3072×3072는 이 셀들을 조립한 시트의 해상도이며 네이티브 4K 생성본이 아니다. 넉넉한 투명 여백은 화질 증가가 아니다.

4개 묶음 사이에서 굵기가 작아지는 현상을 방지하기 위해 밝은 중심선 픽셀 면적을 기준으로 앞 26장의 순서를 정리했다. 이 수치는 가지 일부도 포함하는 보조 지표이며 모든 높이의 주 줄기 두께가 단조 증가한다는 뜻은 아니다. 번개 양 끝 부근의 alpha 가중 중심과 가시 영역의 세로 중심을 정수 픽셀로 등록했다. 생성된 길이·굴곡의 작은 차이는 남으며 균일 확대·축소나 왜곡으로 보정하지 않았다.

원본과 제작 자료는 [ArtSource 폴더](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/)에 있다. [패킹 스크립트](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/pack_atlas.ps1), [프레임 대응표](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/frame_mapping.csv), [정적 지표](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/atlas_metrics.csv), [Unity 임포트 스니펫](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/import_sprites.cs.txt)을 보존한다. 스니펫은 Editor 일회성 실행 기록이며 Assets의 게임 코드가 아니다.

## 확인과 한계

- Unity 6000.3.22f1에서 3072×3072 텍스처, 32개 서브 Sprite, 모든 rect·PPU·피봇을 실제 조회하여 확인했다. [Unity 검증 결과](../../ArtSource/AirBossAttack/lightning_verticalgrowth_v01/unity_validation.json).
- 전체 32장 배경 합성을 눈으로 확인했다. 얇은 시작, 굵은 후반, 희미한 소멸과 중심 정렬을 비교했다.
- 최종 모든 셀의 가장자리 4픽셀은 alpha=0이다. 내부의 매우 낮은 alpha 잔점은 원본 보존에 포함되며 색 제거 처리로 발광을 잘라내지 않았다.
- 임포트 후 Unity Console error 조회는 0건이었다.
- 실제 게임 카메라·최종 사용 Scale·소재가 동시에 다수 등장하는 상황·사용자 지정 재생 속도에서의 평가는 미완료다. 씬에 자동 배치하거나 기존 Attack 프리팹을 교체하지 않았다.
