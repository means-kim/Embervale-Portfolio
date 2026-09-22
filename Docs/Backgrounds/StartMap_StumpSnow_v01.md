# 그루터기 분리 눈 v01

2026-09-23. 시작 숲 StumpNear_v01에 겹치는 별도 장식 눈 한 소재.

[눈 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpSnow_v01.png) · [합성 v23](StartMap_Placement_v23.html) · [전체 캡처](StartMap_Placement_v23.png) · [확대 전후 비교](StartMap_StumpSnow_Alignment_v01.html) · [확대 캡처](StartMap_StumpSnow_Alignment_v01.png)

## 사양과 배치

요청 및 실제1254×1254 RGBA. 좌상단·우하단 모서리alpha0 확인. 나무와 같은 캔버스·중앙pivot·PPU100, Scale1 캔버스12.54×12.54유닛. 고유 GUID·spriteID의 .meta 동반. SpriteSingle, FullRect, Max2048, Trilinear, Mipmap On, 무압축, 자동 Physics Shape Off. 최종 Unity Scale은 미정.

부러진 윗면과 뿌리 능선의 눈만 포함. 나무·지면·캐릭터·눈 제거 기능 제외. StumpNear_v01 바로 위, 지면·플레이어 뒤에 동일 width11%, left36%, top76.8%로 겹친다. HTML brightness(.75), PNG 원본 밝기 유지. Unity에서도 원본 그루터기와 동일 위치·Scale로 맞출 후보이며 다른 나무와 자동 호환되지 않는다.

## 확인과 남은 차이

550px 확대 전후 비교에서 주요 윗면과 뿌리 굴곡의 접합을 육안 확인했다. 전체 합성에서는 캐릭터 크기와 가시성을 유지한다. 다만 컨셉의 두꺼운 적설보다 얇고 어두워 작은 화면에서는 서리처럼 은은하게 읽힌다. 적설량까지 컨셉과 동일하다고 보지 않는다. 실제 카메라 크기 확정 후 밝기·두께 재검토가 필요하다.

눈만 숨기면 나무가 유지되고, 그루터기를 숨기면 눈도 함께 숨도록 검토 화면을 구성하고 확인했다. 모든 이미지 로드를 확인했다. 캡처는 화면 범위다. Unity 배치·실플레이·눈 제거 동작은 구현하거나 검증하지 않았다. 오른쪽 고목 제외 및 동굴 진입 방향 유지.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: StumpNear_v01(좌표), Bg_Concept(눈 스타일). 생성 이미지를 프로젝트에 복사하고 원본을 보존했다.

```text
Use case: background-extraction. Image1 is the exact target stump sprite, 1254x1254; image2 is master winter forest concept for snow texture. Output ONLY a separate snow overlay registered exactly to image1 on the same 1254x1254 transparent canvas, no recentering or resizing. Imagine light settled snow on upward-facing surfaces of this stump, then hide ALL wood. Place thin irregular muted blue-white snow caps on the two highest broken tops around x270..455,y325..400, a few small deposits on right broken tips near x760..855,y535..615, subtle patches along upward-facing diagonal root ridges in lower third. Follow actual target surfaces precisely, coordinates only guidance. Snow predominantly at tops, with sparse short root dusting rather than a continuous outline. Leave vertical bark, cavity and gaps empty. Painted cold gray-blue shaded snow with soft silver moonlight, restrained highlights, not bright glowing white. ONLY snow on real alpha transparent background; no wood, stump, rocks, ground, floating particles, fog, sky, icicles, character, lettering or checkerboard. Keep original canvas including transparent margins and empty regions; do not generate a new stump or recompose it. No broad bottom snowbank. No snow below y1110.
```
