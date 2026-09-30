Shader "Map/MeshLit2D"
{
    Properties { _MainTex ("Source PNG", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float2 world:TEXCOORD1; float2 lightingUV:TEXCOORD2; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            Output vert(Input v) { Output o; o.vertex=TransformObjectToHClip(v.vertex.xyz); o.uv=v.uv; o.color=v.color; o.world=TransformObjectToWorld(v.vertex.xyz).xy; o.lightingUV=ComputeScreenPos(o.vertex/o.vertex.w).xy; return o; }
            float4 frag(Output i):SV_Target
            {
                float4 c=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv)*i.color;
                // Mesh vertex positions/colors are used directly: no SpriteRenderer-only inputs.
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(c.rgb, c.a, half4(1,1,1,1), surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);
                c = CombinedShapeLightShared(surfaceData, inputData);

                return c;
            }
            ENDHLSL
        }
    }
}
