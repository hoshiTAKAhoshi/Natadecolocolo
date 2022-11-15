Shader "Nose"
{
    Properties
    {
        _Amplitude("Amplitude",Vector) = (2.0,0.0,1.5,0)
        _LightDire("LightDire",Vector) = (0.5,-1.0,0.2,0)
        _LightColor("LightColor",Color) = (0.0,0.0,0.5,1)
        _Expansion("Expansion", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Stencil{
                Ref 2
                Comp always
                Pass replace
            }
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "UnityCG.cginc" 
            #include "UnityLightingCommon.cginc"
            #include "Lighting.cginc" 
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                fixed3 normal:NORMAL;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                //float4 localPos : TEXCOORD1;
                half3 normal:TEXCOORD2;
                fixed4 diff : COLOR0;
                SHADOW_COORDS(1)
            };

            fixed4 _Amplitude;
            float _Expansion;

            v2f vert(appdata v) {
                v2f o;
                
                float3 vWorld = mul(unity_ObjectToWorld,v.vertex);
                fixed4 amp = mul(unity_WorldToObject,_Amplitude);
                
                // 水平方向の揺れ 
                v.vertex.xyz += amp*(vWorld.y+0.5);

                float4x4 mat = unity_ObjectToWorld;
                float3 pos = float3(mat._m03,mat._m13,mat._m23);

                // 膨らみ 
                v.vertex.xyz = float3(v.vertex.x*(1+_Expansion),v.vertex.y*(1+_Expansion),v.vertex.z*(1+_Expansion));

                float3 moveY = mul(unity_WorldToObject,float3(0,-_Amplitude.x*(vWorld.x-pos.x)-_Amplitude.z*(vWorld.z-pos.z),0)*(vWorld.y+0.5)+float3(0,_Expansion*0.5,0));
                // 垂直方向の揺れ 
                v.vertex.xyz += moveY;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.grabPos = ComputeGrabScreenPos(o.pos);

                //o.localPos = v.vertex;
                o.normal = UnityObjectToWorldNormal(v.normal);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half NdotL = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = NdotL * _LightColor0;
                TRANSFER_SHADOW(o)

                return o;
            }

            //sampler2D _GrabTexture;
            //fixed4 _GrabTexture_TexelSize;
            //float _Blur;

            half3 _LightDire;
            fixed4 _LightColor;

            half4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                half3 normal = i.normal;


                fixed4 col_after_white = fixed4(0.9,0.9,0.9,1);//col*(1-len_from_center) + (len_from_center);

                float shine_ratio = dot(normal, _LightDire)/2.0 +0.3;
                fixed4 col_after_lighting = col_after_white - shine_ratio*_LightColor*1.6;
                return col_after_lighting;// * i.diff * SHADOW_ATTENUATION(i);
            }


            ENDCG
        }

        
        // 影の描画
        Pass {
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 grabPos : TEXCOORD0;
                //float4 pos : SV_POSITION;
                float4 localPos : TEXCOORD1;
                half3 normal:TEXCOORD2;
                V2F_SHADOW_CASTER;
            };

            fixed4 _Amplitude;
            float _Expansion;

            v2f vert (appdata v) {
                v2f o;
                float3 vWorld = mul(unity_ObjectToWorld,v.vertex);
                fixed4 amp = mul(unity_WorldToObject,_Amplitude);
                
                // 水平方向の揺れ 
                v.vertex.xyz += amp*(vWorld.y+0.5);

                float4x4 mat = unity_ObjectToWorld;
                float3 pos = float3(mat._m03,mat._m13,mat._m23);

                // 膨らみ 
                v.vertex.xyz = float3(v.vertex.x*(1+_Expansion),v.vertex.y*(1+_Expansion),v.vertex.z*(1+_Expansion));

                float3 moveY = mul(unity_WorldToObject,float3(0,-_Amplitude.x*(vWorld.x-pos.x)-_Amplitude.z*(vWorld.z-pos.z),0)*(vWorld.y+0.5)+float3(0,_Expansion*0.5,0));
                // 垂直方向の揺れ 
                v.vertex.xyz += moveY;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.grabPos = ComputeGrabScreenPos(o.pos);

                o.localPos = v.vertex;
                o.normal = UnityObjectToWorldNormal(v.normal);

                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}
