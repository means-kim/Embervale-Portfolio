Shader "UI/HealthFlameFill"
{
    Properties
    {
        [PerRendererData] _MainTex("Design artwork",2D)="white" {}
        _UseArtwork("Use silver frame artwork",Float)=0
        _ArtRect("Artwork UV bounds",Vector)=(0,0,1,1)
        _EmptyColor("Empty", Color)=(0.025,0.032,0.05,1)
        _BorderColor("Outline", Color)=(0.13,0.17,0.23,1)
        _FireNoise("Fire asset turbulence",2D)="gray" {}
        _DetailNoise("Fire asset detail",2D)="gray" {}
        _FireSpeed("Flame speed",Range(.1,3))=1
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata { float4 vertex:POSITION; float4 color:COLOR; float4 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; float4 color:COLOR; float4 uv:TEXCOORD0; float4 local:TEXCOORD1; };
            float4 _EmptyColor, _BorderColor, _ClipRect;
            sampler2D _FireNoise, _DetailNoise;
            sampler2D _MainTex;
            float _UseArtwork;
            float4 _ArtRect;
            float _FireSpeed;
            v2f vert(appdata v) { v2f o; o.local=v.vertex; o.vertex=UnityObjectToClipPos(v.vertex);o.color=v.color;o.uv=v.uv;return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float x=i.uv.x,y=i.uv.y,fill=i.uv.z;
                float t=_Time.y*_FireSpeed+i.uv.w;
                float2 uv=float2(x,y);
                float warp=tex2D(_DetailNoise,frac(uv*float2(1.7,1.1)-float2(.03,t*.18))).r-.5;
                float n1=tex2D(_FireNoise,frac(uv*float2(1.4,.85)+float2(warp*.24,-t*.42))).r;
                float n2=tex2D(_DetailNoise,frac(uv*float2(3.6,1.5)+float2(warp*.16,-t*.67))).r;
                float sway=sin(y*9-t*2)*.04+warp*.13*y;
                float core=1-smoothstep(.015,.34*(1-y)+.055,abs(x-.49-sway));
                float heat=saturate(n1*.64+n2*.42+core*.52+.08-y*.30);
                float ribbons=smoothstep(.35,.7,n1*.6+n2*.4+core*.22);
                float3 fire=lerp(float3(.24,.014,.004),float3(.95,.12,.012),smoothstep(.12,.40,heat));
                fire=lerp(fire,float3(1,.47,.045),smoothstep(.36,.64,heat));
                fire=lerp(fire,float3(1,.85,.36),smoothstep(.59,.82,heat));
                fire=lerp(fire,float3(1,.98,.79),smoothstep(.79,1,heat));
                fire*=.82+.18*ribbons;
                float lit=fill<=0 ? 0 : (fill>=1 ? 1 : 1-smoothstep(fill-.012,fill+.012,x));
                float3 col=lerp(_EmptyColor.rgb,fire,lit);
                float3 rim=lerp(_BorderColor.rgb,fire*.8,lit*.78);
                col=i.color.r<.2 ? rim : col;
                float alpha=i.color.a;
                if(i.color.r>.2 && i.color.r<.8)
                {
                    col=float3(1,.26,.025);
                    alpha*=lit*(.7+.3*sin(t*3+y*6));
                }
                if(_UseArtwork>.5)
                {
                    if(i.color.r<.5)
                    {
                        col=lerp(float3(.19,.23,.29),float3(.83,.89,.95),y);
                    }
                    else
                    {
                        float4 art=tex2D(_MainTex,_ArtRect.xy+uv*_ArtRect.zw);
                        // The production sprite has a solid black interior, separate
                        // from its silver frame; animate only this keyed interior.
                        float brightness=max(art.r,max(art.g,art.b));
                        float interior=1-smoothstep(.012,.055,brightness);
                        col=lerp(art.rgb,lerp(_EmptyColor.rgb,fire,lit),interior);
                        alpha*=art.a;
                    }
                }
                #ifdef UNITY_UI_CLIP_RECT
                alpha*=UnityGet2DClipping(i.local.xy,_ClipRect);
                #endif
                return float4(col,alpha);
            }
            ENDCG
        }
    }
}
