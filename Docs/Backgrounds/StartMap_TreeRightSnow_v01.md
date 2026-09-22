# 오른쪽 고목 장식 눈 v01

2026-09-22 제작. 사용자 자율 진행 요청에 따라 고목 장식용 눈 한 소재만 추가했다. 지면 눈·플랫폼·콜라이더·눈 제거 기능은 포함하지 않는다.

## 소재와 배치

- [눈 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeRightSnow_v01.png), 고유 GUID의 .meta 동반.
- 정렬 대상: [오른쪽 고목 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreeRight_v02.png). 원본 컨셉을 함께 실제 이미지로 참조했다.
- 요청 및 실제 PNG: 1024×1536, RGBA, 모서리 alpha 0 확인. 나무 자체는 포함하지 않는 별도 눈 레이어.
- Import: 대상 고목과 동일한 PPU 100, 중앙 pivot, Full Rect, 최대 2048. Transform Scale과 최종 월드 크기는 미확정.
- 배치 시 고목과 동일한 위치·회전·Scale로 겹치고 눈을 고목보다 앞에 표시한다. 개별 눈 조각을 재정렬하거나 캔버스를 자르면 이 정렬 기준이 깨진다.

## 생성 후 비교 및 해결

[확대 정렬 검토](StartMap_TreeRightSnow_Alignment_v01.html) / [정렬 이미지](StartMap_TreeRightSnow_Alignment_v01.png)에서 두 1024×1536 캔버스를 같은 좌표로 겹쳤다. 주요 가지와 감긴 뿌리, 하단 뿌리에 눈이 얹힌 것을 육안 확인했다. 픽셀 단위 완전 일치 판정이나 Unity 검증은 아니다.

전체 합성에서는 원본 컨셉보다 눈 하이라이트가 두드러졌다. [합성 v10](StartMap_Placement_v10.html)에서 PNG를 변경하지 않고 눈 표시 밝기를 CSS brightness(0.75)로 낮춰 어두운 배경과 비교했다. alpha는 유지했다. 50~100% 밝기 조절과 눈 표시 토글을 제공하고 토글 동작을 확인했다. Unity 색공간·재질에 따라 동일 숫자가 동일 결과를 내지는 않으므로 실제 배치 때 별도 조정한다.

원본 컨셉의 수평 발판·고드름은 이번 소재 범위에 포함하지 않았다. 왼쪽 고목은 아직 눈이 없으며, 지면과의 접지·캐릭터 가림·플레이 화면과 인접 구역 연결은 미검증이다. 다음 장식 작업은 왼쪽 고목의 별도 눈 후보이며, 플레이 지형은 임시 지형과 카메라 크기 확인 후 제작한다. 개별 아트의 사용자 최종 승인은 아직 아니다.

## 생성 방식 및 실제 프롬프트

내장 imagegen 사용. 생성 결과를 프로젝트에 복사했고, 기존 고목 PNG는 보존했다. 참조 순서: 오른쪽 고목 v02 → Bg_Concept.

```text
Use case: background-extraction.
Asset type: ONLY SNOW overlay registered exactly to the supplied right tree sprite. Image 1 is the alignment target BG_Start_TreeRight_v02, 1024x1536. Image 2 Bg_Concept is snow texture reference only.
Produce a 1024x1536 transparent PNG overlay that fits image 1 when placed at exactly the same position, scale and pivot. DO NOT move, rotate, crop, reframe or rescale the target geometry. Imagine adding a moderate layer of settled powder snow to the upward-facing surfaces of image 1, then hiding the entire wood layer and exporting ONLY the new snow. The result must contain ONLY disconnected patches of cool blue-white snow, with all wood, all bark, all branches and all roots removed to transparency. The snow patches must stay at the exact positions that the underlying target branches and raised roots would support: the upper left branch near y=140-240, the upper right branch near y=100-200, the short left spur near y=610, the broad wrapping root near y=1040-1150, and upper surfaces of the bottom spreading roots around y=1350-1490. Use the actual image rather than these approximate landmarks if needed. No vertical snow stripe down the trunk. Keep most of the trunk uncovered and the whole left open area transparent. No snow hovering away from wood. Restrained soft silver-blue light, hand-painted rounded snow caps with slightly uneven edges, like the source concept. No ground snowbank, no falling flakes, no floating particles, no added icicles, no platforms, no whole tree silhouette, no colored background, no checkerboard, no text. Genuine transparent alpha. Output one snow-only overlay in the SAME full portrait canvas; do not pack the patches together.
```

