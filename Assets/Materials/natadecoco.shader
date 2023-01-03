Shader "Custom/natadecoco"
{
    Properties
    {
        _Blur("Blur", Float) = 10
        _Amplitude("Amplitude",Vector) = (2.0,0.0,1.5,0.0)
        _LightDire("LightDire",Vector) = (0.5,-1.0,0.5,0.0)
        _LightColor("LightColor",Color) = (0.0,0.0,0.5,1)
        _Expansion("Expansion", Float) = 0
    }

    SubShader
    {

        // Draw after all opaque geometry
        Tags { "Queue" = "Transparent" }
        LOD 200
        //ZWrite Off
        //ZTest Always
        //#pragma target 3.0
        // Grab the screen behind the object into _GrabTexture

        /*
        GrabPass
        {
            //"_GrabTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            Cull Front

            Stencil{
                Ref 2
                Comp always
                Pass replace
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                //fixed3 normal:NORMAL;
            };
            
            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 localPos : TEXCOORD1;
                //half3 normal:TEXCOORD2;
            };

            fixed4 _Amplitude;
            float _Expansion;

            v2f vert(appdata_base v) {
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
                //o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }
            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _Blur;

            half4 frag(v2f i) : SV_Target
            {
                float blur = 0;//_Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0,0,0,0);
                float weight_total = 0;

                float4 local_pos = i.localPos;

                
                for (float x = -blur; x <= blur; x += 1)
                {
                    float distance_normalized = abs(x/blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2)*5);
                    weight_total += weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(x*_GrabTexture_TexelSize.x,0,0,0))*weight;
                }
                col /= weight_total;

                return col;
            }
            ENDCG
        }
        */
        
        GrabPass
        {
        //    "_GrabTexture"
        }
        // 裏面描画
        Pass
        {
            Cull Front

            Stencil{
                Ref 2
                Comp always
                Pass replace
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                float4 localPos : TEXCOORD1;
                half3 normal:TEXCOORD2;
            };

            fixed4 _Amplitude;
            float _Expansion;

            v2f vert(appdata_base v) {
                v2f o;
                
                float3 vWorld = mul(unity_ObjectToWorld,v.vertex);
                fixed4 amp = mul(unity_WorldToObject,_Amplitude);
                
                // 水平方向の揺れ 
                v.vertex.xyz += amp*(vWorld.y+0.5);
                //v.vertex.xyz += amp*(v.vertex.y+0.5);

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

                return o;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _Blur;
            half3 _LightDire;
            fixed4 _LightColor;

            half4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                float blur = 0;//_Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0,0,0,0);
                float weight_total = 0;
                float4 local_pos = i.localPos;
                half3 normal = i.normal;

                for (float y = -blur; y <= blur; y += 1)
                {
                    float distance_normalized = abs(y/blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2)*5);
                    weight_total += weight;
                    //col += tex2Dproj(_GrabTexture, i.grabPos + float4(0,y/**(5-abs(local_pos.x)*10)*/*_GrabTexture_TexelSize.y,0,0))*weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(0,y*_GrabTexture_TexelSize.y,0,0))*weight;
                }
                col /= weight_total;
                //if(length(local_pos.xyz)>0.6)
                //if(local_pos.x>0)
                //    col = fixed4(1,1,1,1);
                //half4 bgcolor = tex2Dproj(_GrabTexture, i.grabPos);

                float len_from_center = saturate(length(local_pos.xyz)*0.5);

                fixed4 col_after_white = col*(1-len_from_center) + (len_from_center);

                float shine_ratio = dot(normal, _LightDire)/3.0 +1.0;
                fixed4 col_after_lighting = col_after_white - shine_ratio*_LightColor;
                //if(facing>0)
                    return col_after_lighting;
                //else
                //    return half4(1,0.5,0.5,0.5);
                //return col*(1) + (0.0);
            }


            ENDCG
        }
        GrabPass
        {
            //"_GrabTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        // 表面描画1回目 
        Pass
        {
            Cull Back

            Stencil{
                Ref 2
                Comp always
                Pass replace
            }
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlights
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
                float4 localPos : TEXCOORD3;
                //half3 normal:TEXCOORD2;
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

                o.localPos = v.vertex;
                //o.normal = UnityObjectToWorldNormal(v.normal);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half NdotL = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = NdotL * _LightColor0;
                TRANSFER_SHADOW(o)


                return o;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _Blur;

            half4 frag(v2f i) : SV_Target
            {
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0,0,0,0);
                float weight_total = 0;

                float4 local_pos = i.localPos;

                

                for (float x = -blur; x <= blur; x += 1)
                {
                    float distance_normalized = abs(x/blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2)*5);
                    weight_total += weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(x*_GrabTexture_TexelSize.x,0,0,0))*weight;
                }
                col /= weight_total;
                //half4 bgcolor = tex2Dproj(_GrabTexture, i.grabPos);
                return col;//* i.diff * SHADOW_ATTENUATION(i);
            }
            ENDCG
        }
        
        GrabPass
        {
        //    "_GrabTexture"
        }
        // 表面描画2回目
        Pass
        {
            Cull Back

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
                float4 localPos : TEXCOORD3;
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

                o.localPos = v.vertex;
                o.normal = UnityObjectToWorldNormal(v.normal);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half NdotL = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = NdotL * _LightColor0;
                TRANSFER_SHADOW(o)
                return o;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _Blur;
            half3 _LightDire;
            fixed4 _LightColor;

            half4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0,0,0,0);
                float weight_total = 0;
                float4 local_pos = i.localPos;
                half3 normal = i.normal;

                for (float y = -blur; y <= blur; y += 1)
                {
                    float distance_normalized = abs(y/blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2)*5);
                    weight_total += weight;
                    //col += tex2Dproj(_GrabTexture, i.grabPos + float4(0,y/**(5-abs(local_pos.x)*10)*/*_GrabTexture_TexelSize.y,0,0))*weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(0,y*_GrabTexture_TexelSize.y,0,0))*weight;
                }
                col /= weight_total;
                //if(length(local_pos.xyz)>0.6)
                //if(local_pos.x>0)
                //    col = fixed4(1,1,1,1);
                //half4 bgcolor = tex2Dproj(_GrabTexture, i.grabPos);

                float len_from_center = saturate(length(local_pos.xyz)*0.9);

                fixed4 col_after_white = col*(1-len_from_center) + (len_from_center);

                float shine_ratio = dot(normal, _LightDire)/2.0 +0.3;
                fixed4 col_after_lighting = col_after_white - shine_ratio*_LightColor;
                
                return col_after_lighting;// * i.diff * SHADOW_ATTENUATION(i);
            }


            ENDCG
        }


        // 影の描画
        Pass {
            Cull Back

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

}