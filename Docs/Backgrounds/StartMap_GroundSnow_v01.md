# 연결 지면 눈 덮개 v01

2026-09-22 사용자 '다음' 요청으로 기존 연결 지면의 별도 눈 한 소재를 제작했다.

[눈 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundSnow_v01.png), [정렬 대상 바위](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_GroundApproach_v01.png), [최신 합성 v15](StartMap_Placement_v15.html), [합성 이미지](StartMap_Placement_v15.png).

요청/실제1774×887 RGBA, 좌상단alpha0. PPU100, 중앙pivot, FullRect, 최대2048, 고유.meta. 지면과 같은 캔버스·좌표·Scale로 겹친다. PNG에 바위·캐릭터·눈 제거 기능은 포함하지 않는다. 단일 긴 눈 스프라이트이며 부분 제거용 조각 분리나 마스크는 아직 없다.

원본 컨셉과 실제 연결 지면을 이미지 참조로 사용했다. 확대 합성에서 눈이 바위 요철을 따르는 것을 육안 확인했다. 표시 크기에서 강한 하이라이트를 줄이기 위해 HTML brightness(.8)을 적용했고 PNG 원본은 보존했다. Unity 표시색과 1:1 대응하는 확정 수치는 아니다. 눈 토글을 확인했다. 지면 → 눈 → 동굴 → 플레이어 순서로 겹쳐 동굴 바닥이 눈을 가리게 했다. 동굴 이동 시 가려지는 구간도 달라진다. 입구 경계의 최종 눈 분포와 접합은 실제 배치에서 조정해야 한다.

캐릭터 비율은 기존 컨셉 기준26px/480px를 유지한다. 현재 표시상 바위·눈의 작은 높이 차이는 실제 충돌선 검증이 아니다. Unity 씬·코드·콜라이더는 변경하지 않았다. 실플레이 접지·점프·인접 구간 접합·최종 카메라는 미검증이다. 다른 구역이나 동굴 내부는 이번에 확장하지 않았다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: GroundApproach_v01 → Bg_Concept. 생성 결과를 프로젝트로 복사했다.

```text
Use case: background-extraction. Create ONLY a thin snow overlay registered exactly to image1, the 1774x887 bare rock ground sprite. Image2 is master snow texture/palette. Output full 1774x887 transparent PNG, SAME canvas, no reframing, no crop, no scaling. Imagine adding settled moonlit blue-white snow along the actual upper rock surface in image1, around y505..530, then hiding ALL stone and exporting ONLY the snow. Thin natural uneven cap approximately 8..20 pixels thick resting on the tops of rocks, small rounded drapes down upper edges, soft cool gray-blue shadow below, muted silver highlights. Follow actual little rises and dips of reference surface, keep upper silhouette near y490..520; do not produce a straight ruler stripe. Snow runs from left image edge to right image edge, same ground registration, no floating detached chunks. All areas above and below the narrow cap are genuine alpha transparency. No rocks, ground, landscape, falling flakes, particles, icicles, tree, character, text, checkerboard. Reference concept has restrained fine snow at walking scale; do not make a huge snowbank or brilliant white glow. One independent snow-only overlay, not a snowy ground image.
```

