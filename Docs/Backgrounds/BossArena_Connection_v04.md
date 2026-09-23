# 달 높이 조정 v04

[검토](BossArena_Connection_v04.html) · [배치](BossArena_Connection_v04.json) · [내장 이미지 생성 프롬프트](BossArena_Connection_v04_Prompts.md)

원경에서 기존 달을 제거하고 독립 달 PNG로 교체했다. 달 중심 로컬Y 약124→50으로74단위 상승(캐릭터 보이는높이 약1.46배). X중심3229.6 유지. 새 달 캔버스x3167.6,y-12,width124,brightness0.72. 화면에 보이는 달 지름은 약74로 기존약78과 유사하다. 프레임마다 위치를 고정하는 UI가 아니라 월드 내 독립 장식이다.

- BG_Boss1_OutdoorFar_NoMoon_v01.png:1774×887 RGB,PPU100,Max2048. 기존 달/주변광 제거 편집본. 산과 구름의 주요 구도는 유지했으나 생성 편집이므로 원본과 픽셀 단위 동일을 보장하지 않는다.
- BG_Boss1_Moon_v01.png:요청512×512,실제1254×1254 RGBA,PPU100,Max2048,중앙pivot. 독립 달과 부드러운 광륜.

파일은 [BossMap1](../../Assets/04.Sprites/Backgrounds/BossMap1/README.md)에 PNG+.meta로 보관. 기존 파일과v03보존. 설산/지형/석주/눈/캐릭터의Transform과카메라배율 유지. v03상부확장 및마스크 유지. 달을중복사용하지 않는다.

[지상](BossArena_Connection_v04_Ground.png)과 [5회+추가여유](BossArena_Connection_v04_MaxJump.png)에서 높이·달1개·상단배경 연속성 확인. 이미지로드누락없음. Unity실제카메라/패럴랙스/이음마스크는미검증.
