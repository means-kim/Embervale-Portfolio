# 시작맵 먼 성·다리·받침 절벽 v01

2026-09-22. 원경 v02 확인 후 사용자의 “그래 다음” 요청으로 후속 소재 제작.

- [결과 PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_CastleBridge_v01.png)
- 내장 image_gen 사용. [Bg_Concept](../../Assets/04.Sprites/Bg_Concept.png)의 왼쪽 성·아치 다리·절벽을 형태 참조로, [원경 v02](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_Far_v02.png)를 색감 참조로 전달했다.
- 성·다리·받침 절벽을 하나의 고정 원경 구조물 스프라이트로 제작했다. 지형에서 사라지는 눈 덮개와는 별개인 원경 설경이다.
- 포함: 성, 왼쪽 아치 다리, 눈 덮인 받침 절벽, 하단의 옅은 안개 표현. 제외: 하늘·달·배경 산·숲·고목·플레이 지형·캐릭터·성소·귀환 리프트.
- 주요 실루엣을 시각 검토했다. 원본을 그대로 잘라낸 것이 아니라 가려진 부분을 생성 복원했으므로 세부 건축 형태는 달라질 수 있다.
- 실제 사양은 시작맵 README에 기록한다. 사용자 확인 전이며 원경 합성·접합·플레이 크기 검증은 아직 미완료다. 씬 변경 없음.

## 실제 사용한 프롬프트

```text
Use case: background-extraction.
Asset type: isolated transparent distant castle / bridge / supporting cliff sprite for a 2D hand-painted snowy metroidvania.
Input image 1: authoritative Bg_Concept reference. Extract and reconstruct ONLY the small distant castle, arched bridge extending left from it, and rocky supporting bluff visible in the upper-left starting forest region (roughly x 10-23%, y 7-32%). This is not the sanctuary in the lower-right cave. Input image 2: approved darker far-background v02, palette and moonlight reference only; do NOT include its mountains, sky, moon, clouds or mist as a backdrop.
Produce ONE coherent isolated structure group: tall narrow central Gothic castle tower, a few smaller roofs and spires clustered to its right, slender stone arch bridge leading left, and the distant snow-dusted rocky bluff supporting them. Faithfully retain the small modest castle silhouette, bridge rhythm and side-on distant perspective of the original. Reconstruct areas occluded by trees, but do not enlarge into a sprawling fantasy palace. Hand-painted soft slate-blue and muted silver tones, restrained contrast suitable for mid-far distance against the approved night background; fine detail with softened distant edges, no black outline. Snow is a static part of this distant inaccessible structure only.
Genuine transparent alpha background, including all openings between bridge piers; no sky rectangle, no background mountains, no opaque fog, no checkerboard painted into pixels. Keep the entire structure within frame with modest clear padding; supporting bluff tapers and softly fades to transparency at its lower edge for future forest overlap. Landscape 4:3 canvas, highest available native detail. No trees or vegetation, ground platform, foreground, characters, bosses, sanctuary, return lift, glowing windows, second structure group, text, guides or watermark.
```
