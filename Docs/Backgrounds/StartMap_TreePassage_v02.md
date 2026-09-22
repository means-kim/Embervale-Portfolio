# 고목 통로 v02 — 컨셉 비율 재검토

2026-09-22. 사용자 지적: v01이 컨셉과 너무 다르고 비교 캐릭터가 과대하며 입구가 부자연스럽다. v01은 승인된 기준이 아니다.

## 원인과 수정

이전 비교는 원경 Scale1 / PPU100과 캐릭터 Scale1 / PPU200을 임의로 조합했다. 이 값은 컨셉의 화면 비율을 보장하지 않는다. 컨셉 좌상단640×480에서 작은 인물은 육안상 약24~28px 높이이며 이번 비교 목표는26px다. 정확한 컨셉 인물 마스크 측정값은 아니다.

실제 Run_00 프레임을 alpha>=32로 조사하면 그림 세로 범위 y75~410, 높이336px다. 이전 시안의 그림 높이는 동일640×480 표시에서 336/2×640/1448 = 약74.25px, 새 목표26px의 약2.86배다. 수정 HTML은 프레임 표시폭 26/336×512 =39.619px, 즉640px 화면 너비의6.190476%를 사용한다. 발 기준 y410은 유지한다. 캐릭터 원본·PPU·프리팹 Scale은 변경하지 않았다. 장래에는 이 상대 크기를 기준으로 월드 크기와 카메라를 함께 결정해야 한다.

## 소재

- [뒤쪽 고목 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreePassage_Back_v02.png): 거대한 중앙 아치를 없애고 연속된 몸통을 복원했다. 오른쪽 하단 뿌리 사이에 작고 비대칭인 틈을 둔다.
- [앞쪽 뿌리 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_TreePassage_Front_v02.png): 전체 입구 테두리 대신 한쪽의 비스듬한 뿌리만 분리했다.
- 두 파일 요청/실제1024×1536 RGBA, PPU100, 중앙 pivot, 기존 설정과 같은 FullRect·최대2048, 각각 고유 .meta. 내부 표본 (840,1250)은 두 레이어 모두 alpha0 확인.
- [최신 비교 v12](StartMap_Placement_v12.html) / [이미지](StartMap_Placement_v12.png). 고목 캔버스 너비60%, left29%, top-18%; 앞뒤 동일좌표. 캐릭터 발 기준선은 임시87%로 입구 높이에 맞췄다.

## 확인과 남은 차이

컨셉 원본과 실제 합성을 열어 비교했다. 캐릭터 표시 비율, 작은 뿌리 틈의 투명 배경, 앞뒤 소재의 겹침을 확인했다. 자연스러움은 v01보다 개선했지만 컨셉과 동일한 추출물이 아니다. 고목의 뿌리 밀도·가지 실루엣·눈·지면·등반 발판 구성은 여전히 다르다. 앞 소재는 생성 편집본이라 완벽한 픽셀 분해라고 할 수 없다. 전체 지형 조립·통과 충돌·카메라·접합은 검증하지 않았다. 기존 눈 덮개는 새 고목과 호환되지 않아 합성에서 제외했다. v01과 기존 모든 소재는 보존했다.

## 실제 프롬프트 (내장 imagegen)

원본 컨셉을 우선 참조하고 실패한 v01을 수정 대상으로 사용했다. 생성 파일을 프로젝트에 복사했다.

### 뒤쪽
```text
Use case: precise-object-edit. Image 1 is the MASTER composition reference Bg_Concept: focus only on the huge narrow tree at x420..550,y0..500 in the left starting area. Image 2 is a FAILED tree sprite whose giant symmetric doorway MUST be removed. Output one 1024x1536 transparent PNG, same portrait canvas and upper trunk placement as image 2. Redesign the bottom 45% to closely follow the natural tree in Image1: keep a single massive continuous irregular trunk, overlapping crooked buttress roots, dark muted hand-painted blue bark, NOT a bifurcated gateway. Close nearly all of the huge arch with natural solid aged wood. At the LOWER RIGHT SIDE only, let one crooked root lift away from the trunk making a modest asymmetric slit-like walking opening, approximately x740..880 and y1300..1480. This small gap is about one third the height and width of the failed huge doorway, and surrounded by irregular tapered wood, without an arch rim, ring, carved door, left/right pillars, or symmetry. Genuine transparent alpha in that gap, background and between roots. It is a subtle gap underneath an elevated root, not the focal point. Upper branches should feel gnarled and irregular like master; keep desaturated, dark, low contrast bark, not polished 3D sculpt. No snow (separate later), ground, rock platforms, character, scenery, particles, text, checkerboard, sanctuary. Preserve original overall tall tree silhouette and avoid broad splayed legs. Player will render in front of rear tree and below a later sparse foreground root. This is the rear layer only.
```

### 앞쪽
```text
Use case: background-extraction. Image1 is registration target 1024x1536 tree. Produce a same-canvas transparent FOREGROUND overlay containing ONLY the small crooked diagonal root above its LOWER RIGHT hollow. Preserve the exact target coordinates: the root runs from approximately (754,1070) diagonally down-right across (865,1170) to (966,1260), with a short tapered hanging bark end stopping by y1320. Export only this narrow near-facing bark root, no other wood. The rest of the full 1024x1536 canvas must be genuine alpha0. Do not reframe or center the fragment. Do not create an arch, ring, doorway, or a pair of sides. Keep ALL pixels below y1350 transparent for player passage. Same dark weathered blue-gray wood as image1. Image2 is original concept palette only. No snow, background, ground, character, text or checkerboard. This is a small asymmetrical overlap detail, not a complete entrance border.
```

