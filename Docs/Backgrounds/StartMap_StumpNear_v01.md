# 시작 숲 그루터기 v01

2026-09-23. 이동로 뒤에 놓는 작은 부러진 나무 장식이다.

[PNG](../../Assets/04.Sprites/Backgrounds/StartMap/BG_Start_StumpNear_v01.png) · [합성 v22](StartMap_Placement_v22.html) · [합성 캡처](StartMap_Placement_v22.png) · [캐릭터 앞뒤 확인](StartMap_StumpNear_PlayerCheck_v01.png)

## 용도와 사양

- 원본 컨셉 왼쪽 시작 구역의 낮은 부러진 나무를 참고했다. 기존 TreeLeft_v01은 나무껍질 질감 참조이며 오른쪽 고목을 복원한 것이 아니다.
- 나무 몸통과 짧은 뿌리만 포함. 눈·지면·바위·충돌·발판은 제외.
- 요청1024×1024, 실제1254×1254 RGBA. 확대 가공 없음.
- alpha32 이상 범위 x39..1221, y322..1103. 좌상단·우하단 모서리alpha0.
- PPU100, 중앙pivot, FullRect, Max2048, Trilinear, Mipmap On, 무압축, 자동 Physics Shape Off. 고유 GUID·spriteID의 .meta 동반. Scale1 캔버스12.54×12.54유닛이며 최종 Scale 미정.

## 컨셉 차이와 배치 검토

컨셉의 그루터기는 더 높고 주변 가지·눈과 뒤섞여 있다. 이번 후보는 분리 배치와 작은 화면 가독성을 위해 낮고 단순한 형태로 만들었다. 눈은 함께 굽지 않았다. 축소 시 미세한 껍질 결보다 비대칭 실루엣이 주로 읽힌다.

HTML 후보: width11%, left36%, top76.8%, brightness(.75). PNG 밝기는 유지. 지면과 캐릭터보다 먼저 그려 뿌리 끝을 지면에 겹쳤다. 현재 그림 높이는 프레임 높이의 약9.15%, 비교 캐릭터5.42%의 약1.69배다. 캐릭터 위치41.5%에서 앞을 지나는 모습과 표시 토글, 전체 이미지 로드를 확인했다. 저장 PNG는 화면 범위 캡처다.

정지 아트 합성 확인이며 Unity 이동·충돌·최종 카메라·플레이 검증은 하지 않았다. 오른쪽 동굴 방향과 현재 캐릭터 크기 기준을 유지했다. 다음 구역은 제작하지 않았다.

## 실제 프롬프트

내장 imagegen 사용. 참조 순서: Bg_Concept, TreeLeft_v01. 생성 PNG를 프로젝트에 복사했다.

```text
Use case: stylized-concept. Create ONE isolated small broken tree stump for the starting forest of a side-view 2D platformer. Image1 is the master concept: match the low broken snag behind the small player in its upper-left forest area. Image2 is existing bark material reference only, DO NOT reproduce its giant tree. Transparent PNG requested 1024x1024. A squat asymmetric weathered stump with two blunt snapped prongs, jagged worn splinter ends, thick twisted bark and short exposed root feet. Silhouette width about 700px from x160 to860, height about580px from y240 to820; root feet rest on y830, all edges with transparent padding. Side elevation, no top-down perspective. Dark charcoal blue and desaturated gray-brown bark, restrained cold silver-blue upper-left moonlight, painterly detailed texture consistent with supplied art. No bright glow. It is a LOW ground decoration, not a tall tree, arch, doorway, spikes, platform or obstacle. ONLY wood; no snow, ice, rocks, ground slab, grass, sky, fog, background, character, architecture, text or shadow rectangle. Real alpha transparent background and gaps between roots. Keep broad readable silhouette at small display size, avoid spindly upward branches.
```
