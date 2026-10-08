# 공격 VFX 리마스터 v01 — 2026-10-08

## 적용

- Up / LeftHorizontal / RightHorizontal / Down: 각각 고유 프레임 7개, 기존 반복 끝 키 포함 8키.
- FireBallAttack의 StabAttackVFX: 고유 프레임 10개, 끝 키 포함 11키.
- 총 38개 PNG: [Assets/04.Sprites/VFX/AttackRemaster_v01](../../../Assets/04.Sprites/VFX/AttackRemaster_v01).
- 원래 Aseprite의 96×96 캔버스 기준 → 384×384 RGBA PNG. PPU 100→400, 투명 배경, Bilinear, 무압축, mipmap 없음.
- 원래 스프라이트는 잘린 rect와 외부 피벗을 사용했다. 각 그림의 rect 크기/위치를 4배로 환산하여 전체 384 캔버스에 배치하고 공통 피벗 (0.5,0)을 사용했다. 따라서 각 이미지의 원래 월드 위치/영역을 유지한다. 새 그림의 세부 실루엣과 입자 모양은 재제작되어 달라진다.
- 해당 5개 .anim의 Sprite 키 참조만 교체. 시간과 마지막 반복 키는 유지. Controller 연결은 유지했다.
- 제공된 3개 PNG와 Aseprite는 수정하지 않았다. 실제 애니메이션은 PNG가 아니라 Aseprite subasset을 참조하고 있었으며 Unity에서 실제 사용 프레임을 추출해 제작했다. 사용하지 않는 다른 효과는 변경하지 않았다.
- 씬·전투 코드·공격 판정·전역 설정은 수정하지 않았다.

## 제작과 보존

- 내장 image_gen으로 실제 프레임 참조 시트 5개를 각각 편집. [정확한 프롬프트](Prompts.txt).
- 요청 생성 크기: 검격 2048×1024, 투사체 2560×1024. 실제 생성 크기는 [Assembly.csv](Assembly.csv)의 sourceWidth/sourceHeight 참고. 최종 PNG는 모두 384×384. 생성 요청 크기와 실제 저장 크기는 다르다.
- [원본 프레임/클립 백업](Original), [원본 참조 정보](Manifest.tsv), 생성 원본 *_Generated.png를 함께 보관했다.
- [Assemble.ps1](Assemble.ps1)은 생성 그림의 분할·크기 맞춤 재현용이다. 그림 자체는 이미지 생성 도구로 제작했다.

## 확인

- Unity에서 38개 PNG 임포트, 43개 Sprite 키의 참조/원본 시간/크기/PPU/피벗 검증 완료.
- [Unity 실제 렌더](Unity_ContactSheet.png): 위부터 Up, LeftHorizontal, RightHorizontal, Down, FireBall. 현재 작업 씬과 분리된 임시 preview scene에서 렌더 후 정리했다.
- [검사 기록](Unity_Validation.txt), [전후 애니메이션 비교](Review.html).
- 실제 플레이에서 공격 발생/캐릭터와의 합성/전투 가독성은 미검증. 정지 프레임 렌더를 실제 플레이 검증으로 취급하지 않는다.
