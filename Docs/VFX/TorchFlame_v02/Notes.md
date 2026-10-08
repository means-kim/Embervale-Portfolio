# 횃불 불꽃 리마스터 v02 — 2026-10-08

- 대상: `../../../Assets/04.Sprites/Light/Flame_A_Soft_00.png` ~ `Flame_A_Soft_15.png`, 총 16프레임.
- 기존 160×240 → 실제 저장 256×384 RGBA. 가로·세로 각각 1.6배. 단순 보간 확대가 아닌 image_gen 내장 도구로 윤곽과 내부 불꽃을 다시 그렸다.
- 요청 시트 해상도 2048×3072, 실제 생성 해상도 1024×1536. 요청 해상도를 실제 결과로 표기하지 않는다.
- PPU 100 → 160으로 캔버스 월드 크기 1.6×2.4 유지. 정규화 피벗 (0.5, 0.05), GUID, Single Sprite 모드, Bilinear, 무압축 기본 설정 유지.
- Fire_Active.anim과 프리팹은 수정하지 않았다. 기존 16프레임 순서와 클립 타이밍 유지. 새 그림은 원본의 픽셀 단위 복원이 아니며 불꽃 모양은 달라졌다.
- 횃불 몸체, Glow/Ember, 씬 및 다른 사용자 변경은 수정하지 않았다.
- 생성 시트의 실제 행 간격을 확인하여 프레임 추출. 알파 >16 기준 바닥 y=373으로 맞췄다. 스크립트는 재현용이며 자동 실행되지 않는다.
- 원본 PNG와 meta: [Original](Original). 참조 합본: [Original_16Frames.png](Original_16Frames.png). 생성 원본: [Remastered_16Frames.png](Remastered_16Frames.png).
- [전후 애니메이션 비교](Review.html). PNG 크기·투명도·GUID·PPU 검사는 완료. Unity 실제 애니메이션 재생 및 게임 배치에서의 인상은 미검증.

## 프롬프트

내장 image_gen 사용. 최초 요청: 원본 4×4, 16프레임 불꽃 시트를 고해상도로 리마스터. 각 프레임의 위치·크기·바닥 기준·애니메이션 위상을 유지하고, 주황/빨강 외곽·노랑 중간·밝은 노랑 코어를 보존. 계단형 도트 윤곽과 저해상도 노이즈를 부드러운 곡선과 회화적 그라데이션으로 교체. 투명 PNG, 2048×3072 요청. 횃불 몸체·배경·문자·격자·프레임 사이 광휘 제외.

후속 투명도 편집의 정확한 프롬프트:

> Edit this 4x4 torch flame sprite sheet: REMOVE THE ENTIRE ORANGE BACKGROUND AND ALL BACKGROUND GLOW. Actual transparent alpha PNG is essential. Keep all 16 flames exactly positioned in their existing equal cells. Every pixel outside the actual flame silhouettes must be fully transparent including gaps inside tongues. No ambient glow between sprites. Preserve warm yellow cores and smooth curved orange flame edges. No dark red hard outline. Each row has four flames; 4 rows; preserve animation phases. Transparent game sprite atlas, no checkerboard baked in. Output at highest available resolution, ideally 2048x3072.
