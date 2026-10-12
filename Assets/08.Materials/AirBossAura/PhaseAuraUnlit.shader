Shader "Game/AirBoss/PhaseAuraUnlit"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _SoftLine ("Soft line edges", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off Cull Off
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            float4 _Tint;
            float _SoftLine;
            Output vert(Input v) { Output o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.color=v.color*_Tint; return o; }
            float4 frag(Output i):SV_Target
            {
                float4 c=i.color;
                c.a*=lerp(1,1-smoothstep(0.1,0.5,abs(i.uv.y-0.5)),_SoftLine);
                return c;
            }
            ENDHLSL
        }
    }
}
