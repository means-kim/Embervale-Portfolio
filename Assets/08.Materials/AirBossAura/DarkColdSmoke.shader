Shader "Game/AirBoss/DarkColdSmoke"
{
    Properties
    {
        _Color ("Ink color", Color) = (0.012,0.018,0.045,1)
        _RimColor ("Cold violet edge", Color) = (0.10,0.09,0.19,1)
        _Opacity ("Opacity", Range(0,1)) = 0.72
        _Speed ("Rise speed", Float) = 0.12
        _Detail ("Smoke detail", Float) = 4
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
            float4 _Color, _RimColor;
            float _Opacity, _Speed, _Detail;
            float hash(float2 p) { return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453); }
            float noise(float2 p)
            {
                float2 i=floor(p), f=frac(p); f=f*f*(3-2*f);
                return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);
            }
            float fbm(float2 p) { return noise(p)*0.57+noise(p*2.03+7.1)*0.28+noise(p*4.07+13.2)*0.15; }
            Output vert(Input v) { Output o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.color=v.color; return o; }
            float4 frag(Output i):SV_Target
            {
                float2 p=(i.uv-0.5)*2;
                float t=_Time.y*_Speed;
                float2 q=i.uv*_Detail-float2(0,t);
                float2 warp=float2(fbm(q+float2(t*0.2,0)),fbm(q+9.4));
                float n=fbm(q+warp*2.6);
                float radius=length(p);
                float edge=radius+(n-0.5)*0.32;
                float envelope=1-smoothstep(0.53,0.98,edge);
                // Fade to zero before quad edges, including displaced noise peaks.
                envelope*=1-smoothstep(0.84,1, max(abs(p.x),abs(p.y)));
                float wisps=smoothstep(0.22,0.72,n);
                float veins=pow(saturate(1-abs(n-0.52)*9),3);
                float rim=smoothstep(0.35,0.78,edge)*veins;
                float3 rgb=lerp(_Color.rgb,_RimColor.rgb,rim)*i.color.rgb;
                return float4(rgb,envelope*(0.45+wisps*0.55)*_Opacity*_Color.a*i.color.a);
            }
            ENDHLSL
        }
    }
}
