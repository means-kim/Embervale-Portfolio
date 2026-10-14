# HorizontalAttack v02 — 불꽃 주먹 확인본

2026-10-13. 사용자가 HorizontalAttack 한 모션의 새 버전 제작과 검토를 요청했다. 게임 애니메이션 클립과 컨트롤러에는 적용하지 않았다.

- [0.25배속 재생](Preview.gif), [16프레임 비교](Contact.png)
- [최종 PNG와 시트](../../../Assets/04.Sprites/NewPlayer/HorizontalAttack_v02)
- 16개 고유 프레임, 각 512×512 RGBA, PPU 200, 토르소 기준 피벗. Bilinear, 무압축, mipmap 없음.
- 기존 1.25초 클립 / state speed 5 기준 게임 동작 길이 0.25초에 대응하는 후보. GIF는 동작 1초와 끝 대기 0.5초를 반복해 자세를 쉽게 확인한다.
- 검은 팔과 주먹의 수평 타격. 불길은 주먹에 붙은 짧은 불꽃으로 제한하고 긴 칼날·불꽃 궤적은 제거했다.
- 더미 자세표 → 내장 image_gen 원화 → 프레임 분리 → 오류 프레임 04·09·10만 내장 image_gen으로 재생성했다. 해당 프레임에서 공격하는 팔이 바뀌던 문제를 수정했다.
- [생성 프롬프트](Prompt.txt), [3프레임 보정 프롬프트](Prompt_Repair.txt), [더미](PoseGuide.png), [최초 생성 결과](Generated.png), [선택 보정 결과](Repaired.png).
- 생성 시트 실제 크기는 1448×1086, 보정 원화는 2172×724이다. 최종 512 프레임은 분리·정렬·크기 보정한 결과다.
- [프레임별 명세](Manifest.json), [Unity Import 검사](ImportVerification.json). 16개 스프라이트 규격·투명 여백을 확인했다. 이번 후보를 실제 게임에서 재생한 결과는 아니다.

기존 HorizontalAttack과 다른 공격 모션은 이번 작업에서 수정하지 않았다. 본 후보에 대한 사용자 확인 후 별도 적용한다.
