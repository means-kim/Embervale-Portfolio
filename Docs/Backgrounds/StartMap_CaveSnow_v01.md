# 동굴 외부 암벽 눈 v01

2026-09-23. 동굴 외부의 위를 향한 바위 면에 얹을 눈을 별도 제작했다.

[PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CaveSnow_v01.png) · [합성 v21](StartMap_Placement_v21.html) · [합성 캡처](StartMap_Placement_v21.png) · [확대 정렬](StartMap_CaveSnow_Alignment_v01.html)

요청 및 실제 출력1254×1254 RGBA. CaveEntrance_v01과 동일 캔버스·중앙 pivot·PPU100, Scale1 캔버스12.54×12.54유닛. Sprite Single, Full Rect, Max2048, Trilinear, Mipmap On, 무압축, 자동 Physics Shape Off. 고유 GUID와 spriteID의 .meta를 함께 저장했다. 최종 Unity Transform Scale은 미정이다.

포함: 외부 경사 암벽의 불규칙한 눈 패치. 제외: 암벽 자체·동굴 내부 눈·보행면 눈·고드름·캐릭터. 하단 y900 이후에는 alpha1인 잔여 픽셀5개가 있어 완전 투명 조건을 엄밀히 충족하지는 않지만 합성에서 보이지 않는다. 원본 PNG를 가공하지 않고 보존했다.

합성에서 동굴과 동일 width70%, left46%, top9%, 눈 표시 밝기70%를 사용했다. PNG 자체 밝기는 변경하지 않았다. 정지 합성과620px 확대에서 주요 바위 턱과 눈의 정렬, 입구·캐릭터 가시성을 확인했다. 눈 토글, 동굴left52% 이동 시 눈 동행, 이미지 로드를 확인하고 기본46%로 복귀했다. 저장 캡처는 화면 범위 캡처다.

컨셉의 두꺼운 뿌리 눈과 비교하면 이 후보는 얇은 암벽 눈으로 읽힌다. 오른쪽 고목을 동굴로 바꾼 사용자 방향에 맞춘 조정이며 컨셉 전체 일치·실제 카메라·Unity 접합 검증 완료를 뜻하지 않는다. 사용자가 씬에서 배치할 때 동일 위치·Scale로 겹치며, 다른 동굴 형태에는 자동 호환되지 않는다. 지면 눈과 별도 소재이고 눈 제거·발자국·충돌 기능은 구현하지 않았다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: CaveEntrance_v01(좌표 및 암벽 형태), Bg_Concept(설경 스타일). 생성 결과를 프로젝트로 복사했다.

```text
Use case: background-extraction. Output ONLY a snow overlay registered exactly to Image1, the 1254x1254 cave entrance sprite. Image2 is master snow style. Full 1254x1254 transparent canvas, identical position and geometry, no recentering. Imagine lightly dusting the upward-facing exposed EXTERIOR rock ledges of image1, then hiding ALL stone. Export only irregular thin muted blue-white snow patches along the stepped outer left slope rising from approximately x400,y810 through x500,y680, x660,y530, x790,y420, x820,y180 to x1000,y100 and the top right. Follow ACTUAL image1 surfaces, approximate coordinates are guidance. Thin 5-15px caps with natural taper, charcoal-blue undershadow, no brilliant glow, no thick snowbank. Keep vertical cliff faces mostly bare. Crucial: NO snow inside the cave opening or along the underside roof, and NO snow on the bottom horizontal walking floor: all y900..1254 must be transparent. All rocks, ground, sky, trees, particles, icicles, character and text removed to alpha. The result is scattered snow-only caps in original source coordinates, not a complete cliff image, not an outline, no checkerboard.
```
