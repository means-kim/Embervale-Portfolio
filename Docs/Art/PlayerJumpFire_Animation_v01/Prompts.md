# 불꽃 점프 8프레임 초안 v01

2번 Curl 원본을 참조해 built-in image_gen으로 제작. 실제 PNG 1774x887, 투명 RGBA, 4열 2행 구성. 생성 요청 2048x1024와 실제 해상도는 다름. 위쪽 왼쪽부터 오른쪽, 이어서 아래쪽 왼쪽부터 오른쪽 순서.

발화 → 성장 → 확산 → 정점 → 잔류 → 분리 → 불티 → 소멸. 검토용 초안. 칸 치수가 정수로 나뉘지 않아 고정 셀 정규화 및 공통 피벗 정렬이 필요하며, Unity 재생 품질은 미검증. 씬/기존 PlayerJump.png/애니메이션은 변경하지 않음.

## 생성 프롬프트

Use case: stylized-concept. Create a production-oriented 2D game VFX animation SPRITE SHEET based on the supplied curled fire jump takeoff effect. The reference is the design anchor, not a sheet to copy verbatim.
Output ONE image with EXACTLY EIGHT animation frames arranged in a mathematically regular 4 COLUMN x 2 ROW grid. Canvas 2:1 aspect ratio, ideally 2048x1024; each cell square and equal size. Reading order left-to-right top row, then left-to-right bottom row. NO drawn grid, NO cell borders, NO text or frame numbers. REAL transparent RGBA background everywhere outside the painted flames; never draw checkerboard or black background.
Every frame uses the same fixed view, identical coordinate system and common origin at horizontal center / 78 percent down within each square cell. No automatic recentering or magnification per frame. Entire widest effect fits inside its own cell with transparent gutters. Effects are wide and low within square cells, leaving clear space above the center where the character will be. No character, terrain, shadows, props or UI.
DESIGN: hand-painted orange and crimson outer flames with buttery pale yellow hot cores, elegant organically curling flame tongues, just like the input game effect. Two low waves spring outwards from the foot contact point then curl upwards at left and right tips. Painterly but bold readable silhouettes, no photorealism, no pixel art. Minimal bloom and small restrained embers. This is a SHORT ONE-SHOT JUMP TAKEOFF, NOT a continuously burning fire. Maintain temporal continuity: same tongues develop, spread and break apart. Do NOT draw eight unrelated similar fire bursts.
FRAME 1 top left: tiny low ignition, two short orange tongues, width 15 percent cell; no big wings.
FRAME 2: tongues rapidly push outward, width 35 percent cell, shallow V, early curl tips.
FRAME 3: growing paired curls, width 60 percent cell, bright cream hot cores, moderate height.
FRAME 4 top right: peak matching the reference's twin curled flame waves, width 88 percent cell, clear open center above base.
FRAME 5 bottom left: waves roll farther outward with thinner cores, width 90 percent cell, inner base begins to disappear, same coherent curls travel outwards.
FRAME 6: the wave silhouettes tear into a few thin curling orange ribbons and separated ember tips at the SAME outer positions, noticeably less filled area; center is transparent.
FRAME 7: only a few tiny reddish-orange embers remain near the outer tips, no connected flames, no fresh burst.
FRAME 8 bottom right: almost fully transparent, ONLY two or three very faint tiny ember specks at the prior outward positions, no central ignition.
The fade in frames 6-8 is essential. The reference is the maximum frame only. Keep painterly flame styling consistent across ALL eight frames.
Critical: Use invisible layout only. Every cell boundary is PURE ZERO ALPHA. No thin lines of ANY color anywhere, no horizontal baseline connecting cells. Make flames smaller with 15 percent blank margins on all sides. Frame 7 sparse tiny sparks, frame 8 barely visible specks. Each frame is a separate floating cutout. Transparent gaps between all cutouts. Do not put them on a ruled sheet.

