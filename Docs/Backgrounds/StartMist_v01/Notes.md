# 시작 구역 안개 소재 v01

## 역할과 범위

사용자가 게임 스크립트를 직접 작성한다. Codex는 필요한 기법과 설정 안내, 이미지 제작/수정과 시각 검토를 담당한다. 이번 작업은 PNG 두 개, Unity TextureImporter 설정 및 임시 정지 합성이다. 게임 스크립트/셰이더/프리팹을 작성하거나 기존 씬 배치를 저장하지 않았다.

## 파일

- ../../../Assets/04.Sprites/Backgrounds/StartMap/FX_Start_MistLong_v01.png : 실제 1536×1024 RGBA, 긴 낮은 안개.
- ../../../Assets/04.Sprites/Backgrounds/StartMap/FX_Start_MistSmall_v01.png : 실제 1254×1254 RGBA, 작은 비대칭 안개 덩어리. 요청한 1024 정사각형과 실제 출력 크기는 다름.
- 내장 image_gen 제작. 참조는 프로젝트 Bg_Concept.png, 작은 안개는 긴 안개 결과를 추가 참조. 프롬프트는 Prompts.txt.
- Sprite (2D and UI), Single, PPU100, Alpha Is Transparency on, Clamp, mipmap off, Uncompressed, Max Size2048. .meta 함께 보관.
- 원본 알파 보존. 8픽셀 간격 샘플링 알파 Long0~250, Small0~233; 좌상단 알파0. 모든 영역이 낮은 알파인 것은 아니므로 Renderer Color 알파로 밀도를 낮춰 사용.

## 검토 화면

Preview_0: 기존 맵. Preview_1: 추가 안개 알파0.18. Preview_2: 추가 안개 알파0.32.

16:9 960×540, Orthographic Size12, 카메라(12,9,-10). 임시 SpriteRenderer를 GA1_BG_Mid Order30/31에 두어 실제 지형 World와 플레이어 Actors 뒤에서 표시.

- Long 중심(12,3), 전체 투명 여백 포함 폭34월드 단위, 종횡비 유지 Scale 약2.214.
- Small 중심(24,3.5), 전체 폭11월드 단위, 종횡비 유지 Scale 약0.877.
- Color white, alpha0.18 또는0.32. 현재 정지 합성에서는0.32가 옅은 흐름을 확인하기 적합. 최종값이 아니므로 실제 배치에서 조정.
- 발판/캐릭터가 안개보다 앞에 표시됨을 시각 확인. 임시 오브젝트/카메라 제거, 촬영 전 플레이어 위치 복원. 씬 저장 안 함.

## 사용과 후속 구현

PNG를 Scene에 드래그해 일반 SpriteRenderer로 배치하고 Sorting Layer와 Color 알파를 조정하면 정적인 안개로 사용할 수 있다. 애니메이션/코드는 포함하지 않았다. 두 이미지는 반복 타일이나 스프라이트 시트가 아니라 양끝이 흐려지는 독립 덩어리다.

이동은 이후 사용자가 Transform의 느린 수평 이동과 알파 페이드 또는 Particle System의 긴 수명/낮은 속도로 구현할 수 있다. 현재 포털 가림은 커스텀 셰이더 방식이므로 일반 SpriteRenderer를 놓는 것만으로 내부 진입 시 자동으로 숨는다고 가정하지 않는다. 구역별 표시 전환은 사용자가 구현할 별도 연결 지점이다.
