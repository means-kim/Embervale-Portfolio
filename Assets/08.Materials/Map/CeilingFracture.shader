Shader "Map/CeilingFracture"
{
    Properties { _MainTex ("Rock alpha", 2D) = "white" {} }
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _RockBounds, _RockUVRect;
            float _PortalEnabled, _PortalInside, _PortalX;
            float _ExitEnabled, _ExitOutside, _ExitRole;
            struct Input { float4 vertex : POSITION; float4 color : COLOR; };
            struct Output { float4 vertex : SV_POSITION; float4 color : COLOR; float2 world : TEXCOORD0; };
            Output vert(Input v)
            {
                Output o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.world = TransformObjectToWorld(v.vertex.xyz).xy;
                o.color = v.color;
                return o;
            }
            float4 frag(Output i) : SV_Target
            {
                float2 local = (i.world - _RockBounds.xy) * _RockBounds.zw;
                clip(min(min(local.x, local.y), min(1-local.x, 1-local.y)));
                float2 uv = _RockUVRect.xy + local * _RockUVRect.zw;
                clip(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a - 0.5);
                float interior = step(_PortalX, i.world.x);
                clip(0.5 - lerp(interior, 1-interior, _PortalInside) * _PortalEnabled);
                if (_ExitEnabled > 0.5 && _ExitRole == 1 && _ExitOutside > 0.5) clip(-1);
                return i.color;
            }
            ENDHLSL
        }
    }
}
