Shader "UI/OutGameDarknessHole"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0, 0, 0, 0.7)
        _HolePos ("Hole UV", Vector) = (0.5, 0.5, 0, 0)
        _HoleRadius ("Hole Radius", Range(0, 0.8)) = 0.15
        _Feather ("Feather", Range(0.001, 0.5)) = 0.12
        _Hole2Pos ("Title Hole UV", Vector) = (0.5, 0.72, 0, 0)
        _Hole2Radius ("Title Hole Radii", Vector) = (0.57, 0.19, 0, 0)
        _Hole2Feather ("Title Feather", Range(0.01, 1.0)) = 0.3
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
            float4 _Hole2Pos;
            float4 _Hole2Radius;
            float _Hole2Feather;
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
                // 구멍1: 마우스 원
                float2 d1 = float2((i.uv.x - _HolePos.x) * _Aspect, i.uv.y - _HolePos.y);
                float h1 = smoothstep(_HoleRadius, _HoleRadius + _Feather, length(d1));
                // 구멍2: 타이틀 타원 (중심=1, 경계=1+feather)
                float2 d2 = float2((i.uv.x - _Hole2Pos.x) * _Aspect / _Hole2Radius.x,
                                   (i.uv.y - _Hole2Pos.y) / _Hole2Radius.y);
                float h2 = smoothstep(1.0, 1.0 + _Hole2Feather, length(d2));
                // 둘 중 하나 안에 있으면 투명 (min이라 겹쳐도 진해지지 않음)
                float hole = min(h1, h2);
                fixed4 c = _Color * i.color;
                c.a *= hole;
                return c;
            }
            ENDCG
        }
    }
}