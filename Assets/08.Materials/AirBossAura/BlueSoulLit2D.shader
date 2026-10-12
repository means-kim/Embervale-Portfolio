Shader "Game/AirBoss/BlueSoulLit2D"
{
    Properties
    {
        _Emission ("Subtle core emission", Range(0, 1)) = 0.18
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off Cull Off
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float2 lightingUV:TEXCOORD1; float seed:TEXCOORD2; };
            float _Emission;
            Output vert(Input v)
            {
                Output o;
                o.vertex=TransformObjectToHClip(v.vertex.xyz);
                o.uv=v.uv; o.color=v.color;
                o.lightingUV=ComputeScreenPos(o.vertex/o.vertex.w).xy;
                o.seed=v.color.r*13;
                return o;
            }
            float4 frag(Output i):SV_Target
            {
                float2 p=(i.uv-0.5)*2;
                float t=_Time.y*1.6+i.seed;
                float bend=sin(p.y*5+t)*0.10+sin(p.y*9-t*0.7)*0.035;
                float x=p.x-bend*smoothstep(-0.4,0.8,p.y);
                float head=exp(-dot(float2(p.x*6,(p.y+0.28)*6),float2(p.x*6,(p.y+0.28)*6)));
                float width=lerp(0.23,0.035,saturate((p.y+0.3)/1.2));
                float tail=exp(-x*x/(width*width))*smoothstep(-0.6,-0.15,p.y)*(1-smoothstep(0.1,0.9,p.y));
                float halo=exp(-(p.x*p.x*8+(p.y+0.1)*(p.y+0.1)*3))*0.18;
                float alpha=saturate(head+tail*0.6+halo)*(1-smoothstep(0.8,1,max(abs(p.x),abs(p.y))))*i.color.a;
                float3 rgb=lerp(float3(0.12,0.42,0.85),float3(0.8,0.95,1),saturate(head+tail*0.4))*i.color.rgb;
                SurfaceData2D surface; InputData2D input;
                InitializeSurfaceData(rgb,alpha,half4(1,1,1,1),surface);
                InitializeInputData(i.uv,i.lightingUV,input);
                float4 result=CombinedShapeLightShared(surface,input);
                result.rgb+=rgb*head*_Emission;
                return result;
            }
            ENDHLSL
        }
    }
}
