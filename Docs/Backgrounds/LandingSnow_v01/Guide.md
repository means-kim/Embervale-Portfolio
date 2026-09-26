# 착지 눈먼지 소재와 사용자 구현 안내

## 준비한 아트

- 새 가루눈: ../../../Assets/04.Sprites/Backgrounds/StartMap/FX_Start_LandingPowder_v01.png
- 튀는 알갱이: 기존 ../../../Assets/04.Sprites/Backgrounds/StartMap/FX_Start_SnowParticle_v01.png 재사용. 두 소재의 재생 시간과 양을 독립 조절한다.
- 가루눈은 내장 image_gen 제작, 단일 정지 이미지이며 애니메이션 시트가 아니다. 실제 원본 1983×793 RGBA, Unity 가져오기 512×205, PPU100, Bilinear, Clamp, mipmap off, Uncompressed. Sprite pivot=(0.5,0.11)로 가시 하단에 맞췄다. 원본 PNG/알파 보존. 프롬프트는 Prompt.txt.
- 임시 960×540 정지 합성: Preview_0 효과 없음, Preview_1 실제 카메라 크기(Size12), Preview_2 확대(Size3). 시작 바닥 Raycast 지면Y0, 플레이어 콜라이더 발Y0 기준. 가루눈 폭2.8, alpha0.6, 알갱이8개 폭0.15. Sorting GA1_World/90으로 지형 앞/플레이어 뒤. 실제 플레이어의 최종 발 너비에 맞춰 다시 조정할 예시다.
- 원래 플레이어 위치 복원/임시 오브젝트 제거. 게임 스크립트, 셰이더, 씬, 프리팹을 저장/변경하지 않았다. 이번 검토는 정지 화면이며 실제 낙하/수명/발생 횟수 검증이 아니다.

## 권장 구성 — 사용자가 직접 작성할 부분

처음부터 큰 효과 관리 시스템을 만들 필요 없이 기존 플레이어의 지면 판정과 효과 재생을 분리한다. 이번 파일 탐색에서는 실제 게임 플레이어 이동 스크립트를 특정하지 못했다. 아래 이름은 새로 생성된 코드가 아닌 역할 예시다.

1. 기존 이동/지면 판정: 물리적으로 공중→지면 접촉이 되는 한 순간에 착지 정보를 만든다. 코요테 타임 등 점프 허용 상태와 실제 바닥 접촉을 구별한다. 발밑 지면 판정을 재사용해 벽/천장 접촉을 제외한다. 복수 콜라이더/복수 접점에도 한 번만 알린다. 시작 시 이미 바닥에 있는 캐릭터는 기본적으로 착지로 취급하지 않도록 초기 상태를 맞춘다.
2. 착지 정보: 접촉 위치, 표면 법선, 충돌 직전 하강 속도, 접촉한 지형의 표면 종류를 전달한다. 정지 지면에서는 직전 Y속도의 하강 성분으로 충분하다. 이동 발판은 지면과 플레이어의 상대 속도를 법선 방향으로 계산하는 편이 낫다. 충돌 처리 후 Rigidbody2D 속도가0이 된 값을 사용하지 않는다.
3. 표면 판별: map Layer는 충돌 대상 분류이며 눈 여부를 의미하지 않는다. 예를 들어 사용자가 작성할 GroundSurface 컴포넌트의 Snow/Rock 구분, 또는 현재 맵 데이터로 판별한다. 눈 제거 기능이 생기면 접촉 위치에 실제 눈이 남아 있는지도 조회한다. 이번 단계에서 눈 제거/발자국을 구현할 필요는 없다.
4. LandingSnowEffect: 눈 표면이고 충격이 최소 기준 이상일 때만 효과를 꺼내 접촉점에 배치한다. 약한/강한 착지에 따라 크기/알갱이 개수를 제한 범위 내에서 보간한다. 낙하가 길어져도 무한히 커지지 않도록 clamp한다. 임계값은 최종 점프/중력 설정을 보고 사용자가 정한다.
5. 재생이 끝나면 비활성화/풀 반환. 풀 재사용 시 이전 입자를 비우고 색/크기/애니메이션 시간을 초기화한다. 처음에는 한 프리팹으로 동작을 확인한 뒤 필요한 만큼 풀을 적용하면 된다.

개념 흐름: 실제 착지 발생 → 접촉점/직전 충격속도/표면 전달 → Snow 조건 검사 → 발 위치에서 효과 재생 → 완료 후 반환.

## 효과 오브젝트 구성 예시 (아직 만들지 않음)

LandingSnowEffect
- Powder: SpriteRenderer + 짧은 Animation/Animator. 새 가루눈 이미지가 이미 양옆으로 퍼지는 형태이므로 회전시키지 않고 접촉 법선에 맞춰 낮게 배치. X크기를 조금 늘리고 Color 알파를 줄여 소멸. 이는 Inspector/Animation에서 설정 가능하다.
- Grains: Particle System. 기존 단일 눈 알갱이로 6~10개 정도의 짧은 Burst를 시작점으로 시험. Loop/Play On Awake off, 연속 방출0. 약0.2~0.45초 수명, 좌우로 나뉘는 작은 속도와 짧은 상승 후 약한 낙하, 끝으로 갈수록 알파0. 수치는 검증된 완성 설정이 아닌 시각 조정 출발값이다.

가루눈 SpriteRenderer의 pivot과 Particle System 렌더러 pivot은 동일한 동작이라고 가정하지 않는다. 그래서 첫 구현은 Powder를 SpriteRenderer로, 알갱이만 Particle System으로 나누는 구성이 단순하다. 표면에 맞춘 작은 시각 오프셋을 노출해 경사면 파묻힘을 조정하되 충돌 지형 자체를 효과에 맞춰 옮기지 않는다.

발생 뒤 캐릭터를 따라가지 않도록 효과 루트를 플레이어 자식으로 계속 붙여두지 않는다. Grains는 World simulation space를 사용한다. 동굴 입구 근처에서 눈이 있는 표면이라면 효과가 발생할 수 있으므로 단순히 플레이어 X만으로 눈 여부를 결정하지 않는다.

알갱이는 프로젝트 렌더 파이프라인에 맞는 투명 비발광(Unlit/alpha blend) 입자 머티리얼을 사용하고 Sprite 입력은 Texture Sheet Animation의 Sprites 모드로 연결할 수 있다. 소재는 준비했지만 해당 머티리얼/Particle System은 아직 생성하지 않았다.

## 점검할 동작

- 정지/걷기 중 착지 효과가 반복되지 않는지, 낮은 착지/높은 착지의 양이 적절한지.
- 벽/천장/일방통행 발판의 아래쪽에 닿아도 착지로 오인하지 않는지.
- 눈 없는 바닥에는 눈먼지가 나오지 않는지, 발판 끝/경사에서도 접촉 위치가 맞는지.
- 재점프해도 이전 눈먼지가 플레이어를 따라 올라가지 않는지.
- 콜라이더 이음새, 여러 접촉점, 재사용한 효과에서 중복 재생/잔여 입자가 없는지.

## 공식 문서

- [ParticleSystem simulationSpace](https://docs.unity.cn/6000.0/Documentation/ScriptReference/ParticleSystem.MainModule-simulationSpace.html)
- [Texture Sheet Animation의 Sprites 모드](https://docs.unity.cn/6000.0/Documentation/ScriptReference/ParticleSystemAnimationMode.Sprites.html)
- [Collision2D.relativeVelocity](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/collision2d/relativevelocity) — 두 충돌 물체 간 상대 속도. magnitude에는 수평 운동도 포함되므로 착지 강도로 그대로 쓰지 말고 필요 성분을 선택한다.
