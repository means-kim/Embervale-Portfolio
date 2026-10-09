# 선택 시안 투명 에셋 v01

2026-10-08. Built-in image_gen. TitleFire_v02/02_Face_FlameCurve를 기준으로 제작. 최종 PNG는 Assets/04.Sprites/UI/EmbervaleFire_v01에 저장. 두 파일 모두 원본 title.png/UI1.png와 같은 2172x724 RGBA. 타이틀 Embervale와 b 내부 얼굴 유지, 버튼 문구 없음. 외부 배경 투명, 버튼 내부 어두운 면 유지. 원본 파일 보존. 각 기존 임포터 설정을 새 GUID의 meta로 복사. Unity 표시 확인은 별도.

## 실제 프롬프트

### Title_Embervale_Face_v01

Extract and faithfully recreate ONLY the Embervale title logo from reference 1 as an isolated transparent PNG asset. Keep exact spelling Embervale and exact selected flowing flame serif design, pale warm gold letter faces, restrained orange glowing edges, short flame terminals, elegant lower swashes, actual little ivory player flame FACE inside the round enclosed counter of lowercase b, two tapered dark eyes. Do not redesign or intensify flames. Reference 2 is the original title asset for canvas scale only: target EXACT 2172x724 canvas, wordmark centered occupying approx 94% width and 70% height, preserving natural letter proportions with padding and no clipping. Reference 1 controls design and face, reference 2 controls overall original asset scale. Remove ALL landscape/background/buttons; real transparent alpha outside letters and between letters, including counters except the face in b. Keep wordmark lettering, no added captions.

transparent_background: true

### Button_Flame_Blank_v01

Extract and faithfully recreate ONLY ONE blank horizontal button from reference 1 as an isolated transparent PNG UI asset. Use the selected refined flame-curve button with dark burnt burgundy/charcoal center, thin warm copper-orange contour, restrained short flowing flame endcaps at left and right, no central diamond ornaments, no long flames or particles. REMOVE ALL TEXT, absolutely no words, letters, numbers or symbols in center. Reference 2 is original UI1 asset for canvas scale/padding only: target EXACT 2172x724 canvas, single button centered spanning approx 99% canvas width and 74% height, same wide slim roughly 4:1 visible silhouette as original UI1. Preserve selected design, no additional redesign. Outside silhouette is genuine transparent alpha; dark button interior remains filled for placing text later. No logo, scenery, extra buttons or captions.

transparent_background: true
