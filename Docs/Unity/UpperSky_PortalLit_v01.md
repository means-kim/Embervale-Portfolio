# UpperSky PortalLit v01 — 2026-09-30

사용자 승인: 기존 가림을 유지하는 조명 대응 셰이더와 테스트 머티리얼 제작(2단계).

## 새 자산
- [PortalMaskLit2D.shader](../../Assets/08.Materials/Map/LightShaders/PortalMaskLit2D.shader): MapReview/PortalMaskLit2D
- [MAT_Boss1_UpperSky_PortalLit_v01.mat](../../Assets/08.Materials/Map/LightShaders/MAT_Boss1_UpperSky_PortalLit_v01.mat)

기존 PortalMask의 입구/출구 역할과 Mouth 영역 계산, UV/정점 색/알파를 유지한다. Universal2D 패스에서 URP Shape Light를 샘플링한다. Cull Off, ZWrite Off를 유지하고 SpriteRenderer 전용 반전/색상 입력을 사용하지 않는다. 가림은 조명 뒤에 적용하여 추가형 조명으로 검정 가림이 드러나지 않게 한다. 수평 다각형 변의 사용되지 않는 나눗셈도 0 분모가 되지 않도록 처리했다.

## 검증
Unity 6000.3.22f1에서 컴파일 오류/경고 없음. 임시 Preview Scene에 실제 _33 하늘 메시와 새 머티리얼, URP 카메라, Global Light2D를 구성하여 렌더하고 즉시 정리했다. 256x128 렌더 기준:

| 상태 | 표시 알파 픽셀 | RGB 합 |
|---|---:|---:|
| 진입 전, 광도 1 | 15996 | 0 |
| 진입 후, 광도 1 | 15996 | 6801.529 |
| 진입 후, 광도 0.2 | 15996 | 2333.039 |
| 복귀, 광도 1 | 15996 | 0 |

진입 조건은 _PortalEnabled=1, _PortalInside=1, _PortalX=34.5, _ExitEnabled=1, _ExitRole=2에 _ExitOutside를 0/1로 전환했다. RGB는 색 공간과 블렌딩을 거친 판독값이며 광도에 선형 비례하는 수치로 해석하지 않는다.

실제 씬의 Play 이동/게이트 통과 검증은 아직 하지 않았다. 기존 동굴 모든 역할에 대한 시각 회귀 검증도 아직 하지 않았다. 현재 일반 조명용이며 노멀맵용 NormalsRendering 패스와 법선 기반 입체 표현은 포함하지 않는다.

## 적용 범위와 다음 단계
원본 셰이더/머티리얼, 사용자가 만든 Shader Graph/테스트 머티리얼, 게임 코드, 배치, 조명은 변경하지 않았다. 씬의 두 UpperSky는 기존 PortalMask를 유지한다.

다음에는 _33 메시를 쓰는 보스방 UpperSky의 MeshRenderer Element 0에 새 머티리얼 하나만 연결한다. 기존 MapVisibility 등록을 유지한다. 실제 Scene에서 광원 Target Sorting Layers와 범위를 맞추고 진입/복귀를 검증한 후 확대 적용한다. 다른 맵 소재에 일괄 적용하지 않는다.
