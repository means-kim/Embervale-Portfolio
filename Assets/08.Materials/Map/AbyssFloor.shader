// 바닥에 깔리는 검은 심연(일렁이는 연기) 이펙트.
// - 텍스처 없이 노이즈로만 그리므로 어떤 크기의 Quad/Sprite에도 늘어나지 않고 붙는다.
// - 노이즈 좌표를 월드 X/Y 기준으로 잡아서 Quad를 여러 개 이어 붙여도 이음새가 맞는다.
// - LightMode 태그가 없는 Unlit 패스라서 Built-in / URP(2D Renderer 포함) 모두에서 렌더링된다.
Shader "Game/Effects/AbyssFloor"
{
    Properties
    {
        // SpriteRenderer가 스프라이트 텍스처를 넣는 슬롯. 노이즈로만 그리므로 샘플링하지 않는다.
        [PerRendererData][HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Abyss Color", Color) = (0.02, 0.015, 0.03, 1)
        [Toggle] _UseVertexColor ("Use Sprite Vertex Color (off for Quad)", Float) = 1

        [Header(Surface)]
        _SurfaceHeight ("Surface Height (UV Y)", Range(0, 1)) = 0.55
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.5)) = 0.16
        _WaveFrequency ("Wave Frequency (per world unit)", Float) = 0.35
        _WaveSpeed ("Wave Speed", Float) = 0.45
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.07

        [Header(Smoke)]
        _SmokeScale ("Smoke Scale", Float) = 1.3
        _SmokeRiseSpeed ("Smoke Rise Speed", Float) = 0.4
        _WispIntensity ("Wisp Intensity", Range(0, 1)) = 0.45
        _WispHeight ("Wisp Height (UV)", Range(0.01, 0.5)) = 0.18
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            // UV를 0~1로 가정하므로 아틀라스에 패킹된 스프라이트에서는 수면 높이가 틀어진다.
            "CanUseSpriteAtlas" = "False"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _UseVertexColor;
            float _SurfaceHeight;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _EdgeSoftness;
            float _SmokeScale;
            float _SmokeRiseSpeed;
            float _WispIntensity;
            float _WispHeight;

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;   // SpriteRenderer 색/투명도 연동
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 worldXY    : TEXCOORD1;
                fixed4 color      : COLOR;
            };

            float Hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            // 부드러운 Value Noise (0~1)
            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = Hash21(i);
                float b = Hash21(i + float2(1, 0));
                float c = Hash21(i + float2(0, 1));
                float d = Hash21(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // 4옥타브 FBM. 옥타브 수는 고정해서 루프 언롤이 되게 한다. (0~1)
            float Fbm(float2 p)
            {
                float sum = 0.0;
                float amp = 0.5;
                [unroll]
                for (int octave = 0; octave < 4; octave++)
                {
                    sum += ValueNoise(p) * amp;
                    p = p * 2.03 + float2(17.1, 9.2);
                    amp *= 0.5;
                }
                return sum / 0.9375;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.worldXY = mul(unity_ObjectToWorld, input.vertex).xy;
                output.color = input.color;
                return output;
            }

            fixed4 Frag(Varyings input) : SV_Target
            {
                // Height zero is fully hidden, including the animated wisps.
                if (_SurfaceHeight <= 0.0) return fixed4(0, 0, 0, 0);
                float time = _Time.y;
                float2 world = input.worldXY;

                // 1) 수면선: 느린 큰 물결 + 덩어리처럼 솟는 billow 노이즈
                float waveX = world.x * _WaveFrequency;
                float largeWave = Fbm(float2(waveX, time * _WaveSpeed)) - 0.5;
                float billow = abs(ValueNoise(float2(waveX * 3.1 + 17.3, time * _WaveSpeed * 1.7)) * 2.0 - 1.0);
                float surface = _SurfaceHeight + largeWave * _WaveAmplitude + billow * _WaveAmplitude * 0.5;

                // 2) 위로 흐르는 연기 노이즈로 경계를 흐트러뜨림
                float smoke = Fbm(world * _SmokeScale + float2(0.0, -time * _SmokeRiseSpeed));
                float distanceBelowSurface = surface - input.uv.y + (smoke - 0.5) * _EdgeSoftness * 2.0;
                float body = smoothstep(0.0, _EdgeSoftness, distanceBelowSurface);

                // 3) 수면 위로 피어오르는 옅은 연기 가닥
                float wispNoise = Fbm(world * _SmokeScale * 1.8 + float2(time * 0.1, -time * _SmokeRiseSpeed * 1.5));
                float heightFade = saturate(1.0 + distanceBelowSurface / _WispHeight);
                float wisp = smoothstep(0.5, 0.78, wispNoise) * heightFade * heightFade * _WispIntensity;

                float alpha = max(body, wisp);

                // Quad 윗변에서 잘리는 선이 보이지 않게 페이드
                alpha *= 1.0 - smoothstep(0.92, 1.0, input.uv.y);

                fixed4 color = _Color * lerp(fixed4(1, 1, 1, 1), input.color, _UseVertexColor);
                return fixed4(color.rgb, color.a * alpha);
            }
            ENDCG
        }
    }
}
