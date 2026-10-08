Shader "UI/OutGameDarknessHole"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0, 0, 0, 0.7)
        _HolePos ("Hole UV", Vector) = (0.5, 0.5, 0, 0)
        _HoleRadius ("Hole Radius", Range(0, 0.8)) = 0.15
        _Feather ("Feather", Range(0.001, 0.5)) = 0.12
        _Aspect ("Screen Aspect", Float) = 1.777
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            sampler2D _MainTex;
            fixed4 _Color;
            float4 _HolePos;
            float _HoleRadius;
            float _Feather;
            float _Aspect;
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 d = float2((i.uv.x - _HolePos.x) * _Aspect, i.uv.y - _HolePos.y);
                float dist = length(d);
                float hole = smoothstep(_HoleRadius, _HoleRadius + _Feather, dist);
                // hole=0(구멍 안) -> 투명, hole=1(바깥) -> 어둡게
                fixed4 c = _Color * i.color;
                c.a *= hole;
                return c;
            }
            ENDCG
        }
    }
}