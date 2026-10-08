Shader "Game/Map/Phase3PlatformFlash"
{
    Properties
    {
        _MainTex ("Platform texture", 2D) = "white" {}
        _FlashOpacity ("Flash opacity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; };
            sampler2D _MainTex;
            float _FlashOpacity;
            Output vert(Input v) { Output o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; return o; }
            float4 frag(Output i):SV_Target { return float4(0.8,0.92,1,tex2D(_MainTex,i.uv).a*_FlashOpacity); }
            ENDHLSL
        }
    }
}
