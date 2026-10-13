// EndingFX 공용 파티클/스프라이트 셰이더.
// - Built-in / URP(Forward, 2D Renderer) 모두에서 동작하도록 LightMode 태그 없는 Unlit 패스로 작성.
// - Blend 를 프로퍼티로 노출해서 Additive / Alpha Blend 를 머티리얼에서 고른다. (셰이더 1개로 통일)
// - _Intensity 로 HDR 값을 만들어 Bloom 에 걸리게 한다. (카메라 HDR + Bloom 켜져 있을 때 효과)
Shader "EndingFX/Particle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint", Color) = (1, 1, 1, 1)
        _Intensity ("Intensity (HDR)", Range(0, 8)) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5   // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 1   // One = Additive, OneMinusSrcAlpha(10) = Alpha
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
        Blend [_SrcBlend] [_DstBlend]
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _TintColor;
            half _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _TintColor;   // 파티클 Color over Lifetime / SpriteRenderer.color
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 c = tex2D(_MainTex, i.uv) * i.color;
                c.rgb *= _Intensity;
                return c;
            }
            ENDCG
        }
    }
}
