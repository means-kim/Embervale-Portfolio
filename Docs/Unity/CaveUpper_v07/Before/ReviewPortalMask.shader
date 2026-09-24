Shader "MapReview/PortalMask"
{
    Properties { _MainTex ("Source PNG", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float2 world:TEXCOORD1; };
            sampler2D _MainTex;
            float _PortalEnabled, _PortalX, _PortalInside, _PortalEntrance, _PortalRole;
            float _ExitEnabled, _ExitOutside, _ExitRole;
            float Mouth(float2 uv)
            {
                float2 p=float2(uv.x,1-uv.y)*1254;
                const float2 edge[16]={float2(535,931),float2(593,881),float2(685,848),float2(748,803),float2(794,808),float2(835,832),float2(895,814),float2(976,831),float2(1254,838),float2(1254,1062),float2(1100,1058),float2(850,1058),float2(603,1058),float2(565,1015),float2(545,975),float2(537,950)};
                bool hit=false;
                for(int a=0,b=15;a<16;b=a++)
                {
                    if((edge[a].y>p.y)!=(edge[b].y>p.y))
                        if(p.x<(edge[b].x-edge[a].x)*(p.y-edge[a].y)/(edge[b].y-edge[a].y)+edge[a].x)hit=!hit;
                }
                return hit?1:0;
            }
            Output vert(Input v) { Output o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.color=v.color; o.world=mul(unity_ObjectToWorld,v.vertex).xy; return o; }
            float4 frag(Output i):SV_Target
            {
                float4 c=tex2D(_MainTex,i.uv)*i.color;
                float interior=step(_PortalX,i.world.x);
                float hidden=lerp(interior,1-interior,_PortalInside)*_PortalEnabled;
                if(_PortalInside<.5 && _PortalEnabled>.5)
                {
                    // Exterior scenery is never clipped by the entrance's vertical plane.
                    if(_PortalRole==1) hidden=0;
                    // The interior extension is not an exterior cliff silhouette.
                    if(_PortalRole==2){hidden=1;c.a*=1-smoothstep(6,10,i.world.y);}
                    // Preserve the rock shell above the opening, hide the room below it.
                    if(_PortalRole==3) hidden=1-smoothstep(6,10,i.world.y);
                    if(_PortalRole==4)c.a=0;
                }
                if(_PortalEntrance>.5)hidden=Mouth(i.uv)*(1-_PortalInside)*_PortalEnabled;
                if(_ExitEnabled>.5)
                {
                    if(_ExitRole==1 && _ExitOutside>.5)hidden=1;
                    if(_ExitRole==2 && _ExitOutside<.5)hidden=1;
                }
                c.rgb*=1-hidden;
                return c;
            }
            ENDHLSL
        }
    }
}
