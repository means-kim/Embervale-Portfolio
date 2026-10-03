// 보스 포효 화면 이펙트 (충격파 링 + 화면 틴트/비네트 + 플래시)
// - 풀스크린 UI Image에 이 머티리얼을 씌워 사용 (Built-in / URP 모두 동작, 화면 읽기 없음)
// - 링 데이터는 BossRoarScreenEffect가 매 프레임 _Rings 배열로 전달
// - 블렌딩: 프리멀티플라이드 (rgb = 더할 빛 + 틴트*알파, a = 틴트로 어둡게 할 양)
Shader "Game/BossRoarRings"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _CoreColor ("Core Color (선두, 냉기)", Color) = (0.92, 0.97, 1.0, 1)
        _EdgeColor ("Edge Color (꼬리, 불씨)", Color) = (1.0, 0.42, 0.12, 1)
        _TintColor ("Screen Tint", Color) = (0.03, 0.06, 0.12, 1)
        _FlashColor ("Flash Color", Color) = (1.0, 0.85, 0.65, 1)

        // UI 마스킹 호환
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            #define MAX_RINGS 8
            #define TWO_PI 6.28318530718

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };

            float4 _CoreColor, _EdgeColor, _TintColor, _FlashColor;

            // 스크립트에서 설정
            float4 _Rings[MAX_RINGS];     // xy: 뷰포트 중심(0~1), z: 반지름(화면 높이 단위), w: 세기
            float  _RingSeeds[MAX_RINGS];
            float  _RingCount;
            float  _Aspect;               // 화면 가로/세로
            float  _Thickness;            // 링 띠 반치폭(FWHM), 화면 높이 단위
            float  _SpikeCount;           // 원주 방향 스파이크 칸 수
            float  _SpikeLength;          // 스파이크가 링 바깥으로 뻗는 길이(높이 단위)
            float  _Breakup;              // 0=매끈한 원, 1=많이 끊김
            float  _TintAmount;           // 화면 어둡게 (0~1)
            float  _Vignette;             // 가장자리 추가 어둡게 (0~1)
            float  _Flash;                // 플래시 세기
            float  _RingOcclusion;        // 0=빛만 더함, 1=링이 화면을 가림(검은 링)

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float hash11(float n) { return frac(sin(n * 12.9898) * 43758.5453); }

            // 각도 방향 값 노이즈 (원주를 cells칸으로 나눠 이웃 칸과 부드럽게 보간, 360도에서 이어짐)
            float angularNoise(float a01, float cells, float seed)
            {
                float f = a01 * cells;
                float i = floor(f);
                float fr = frac(f);
                float h0 = hash11(fmod(i, cells) + seed * 31.7);
                float h1 = hash11(fmod(i + 1.0, cells) + seed * 31.7);
                return lerp(h0, h1, fr * fr * (3.0 - 2.0 * fr));
            }

            // 링 하나의 빛 (rgb)
            // rgb: 더할 빛, a: 가릴 양
            float4 ring(float2 uv, float4 R, float seed)
            {
                float2 p = (uv - R.xy) * float2(_Aspect, 1.0);
                float d = length(p);
                float a01 = atan2(p.y, p.x) / TWO_PI + 0.5;
                float x = (d - R.z) / max(_Thickness, 1e-4);            // 띠 단면 좌표 (0=중심, +는 바깥)

                // 1) 두꺼운 띠: 가우시안, 반치폭 = _Thickness
                float band = exp(-2.7726 * x * x);
                // 2) 끊김: 각도 노이즈로 밝기 변조
                float brk = lerp(1.0, angularNoise(a01, 23.0, seed) * 1.25, _Breakup);
                // 3) 스파이크: 칸마다 확률적으로 바깥으로 뻗는 가시 (삼각형 프로파일)
                float f = a01 * _SpikeCount;
                float cellRnd = hash11(fmod(floor(f), _SpikeCount) + seed * 57.3);
                float tri = 1.0 - abs(frac(f) - 0.5) * 2.0;
                float spikeOn = saturate((cellRnd - 0.35) * 1.8);
                float len = _SpikeLength * (0.4 + 0.6 * hash11(floor(f) + seed * 91.1));
                float outward = max(d - R.z, 0.0);
                float spike = spikeOn * pow(tri, 6.0) * exp(-outward / max(len, 1e-4)) * saturate(x + 0.6);

                // 색: 선두(바깥) = 냉기 흰빛, 꼬리(안쪽) = 불씨 주황
                float lead = saturate(x * 0.8 + 0.35);
                float3 col = lerp(_EdgeColor.rgb, _CoreColor.rgb, lead);
                float mask = band * brk + spike * 1.2;
                float occ = saturate(mask * R.w * _RingOcclusion * 1.8 * (1.0 - lead * 0.4));   // 꼬리 쪽이 더 짙게 가림
                return float4((col * band * brk + _CoreColor.rgb * spike * 1.2) * R.w, occ);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 light = 0;
                float keep = 1.0;                          // 원래 화면이 남는 비율
                [unroll]
                for (int k = 0; k < MAX_RINGS; k++)
                {
                    if (k < (int)_RingCount)
                    {
                        float4 r = ring(i.uv, _Rings[k], _RingSeeds[k]);
                        light = light * (1.0 - r.a) + r.rgb;   // 뒤 링 빛도 앞 링이 가림
                        keep *= 1.0 - r.a;
                    }
                }

                // 화면 틴트 + 비네트 (프리멀티플라이드 알파로 어둡게)
                float2 c = (i.uv - 0.5) * float2(_Aspect, 1.0);
                float v = saturate(length(c) / (0.5 * sqrt(_Aspect * _Aspect + 1.0)));
                float darken = saturate(_TintAmount * (1.0 - _Vignette + _Vignette * (0.35 + 1.3 * v * v)));

                float a = 1.0 - (1.0 - darken) * keep;
                float3 rgb = _TintColor.rgb * darken * keep + light + _FlashColor.rgb * _Flash;
                return float4(rgb, a) * i.color.a;
            }
            ENDCG
        }
    }
}
