Shader "Custom/NoseAttachLight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AlphaOfs ("AlphaOfs", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent-1" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        ZWrite Off
        //ZTest LEqual
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
                float3 centerScreenPos : TEXCOORD2;
                float4 local_pos : TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.local_pos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float4x4 mat = unity_ObjectToWorld;
                float3 center_pos = float3(mat._m03,mat._m13,mat._m23);
                //float2 vWorldScreen = ComputeScreenPos(center_pos);// * (_ScreenParams.y);
                //o.localPos = v.vertex;
                o.centerScreenPos = UnityObjectToClipPos(center_pos);

                o.screenPos = UnityObjectToClipPos(v.vertex);//*_ScreenParams.y/100;//0~1
                

                return o;
            }

            float _AlphaOfs;

            fixed4 frag (v2f i) : SV_Target
            {
                //float4 local_pos = i.localPos;

                // ï®ëÃÇÃÉèÅ[ÉãÉhç¿ïW
                //float4 vWorld = mul(unity_ObjectToWorld,i.vertex);
                //float2 vWorldScreen = ComputeScreenPos(vWorld);//*_ScreenParams;

                //float4x4 mat = unity_ObjectToWorld;
                //float4 center_pos = float4(mat._m03,mat._m13,mat._m23,1);
                //float2 vWorldScreen = ComputeScreenPos(center_pos);// * (_ScreenParams.y);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a = 1;//saturate(distance(i.screenPos.xy,vWorldScreen.xy)*0.00000004);
                //col.r = saturate(i.screenPos.x*0.0002);
                //col.r = (/*vWorldScreen.x-*/i.screenPos.x);
                //col.g = (vWorldScreen.x/*-i.screenPos.x*/);
                
                //col.r = (-500*(i.screenPos.x-vWorldScreen.x)*(i.screenPos.x-vWorldScreen.x)+0.9);
                
                //col.a = abs(i.screenPos.x-i.centerScreenPos.x);
                float2 screenPos = UnityObjectToClipPos(i.local_pos);
                //col.a = abs(i.local_pos.x+i.local_pos.z)*0.5-0.1 - saturate(i.local_pos.y*2-1 +abs(i.local_pos.x+i.local_pos.z));//abs((screenPos.x - i.centerScreenPos.x/19));
                col.a = saturate((0.6-abs(i.local_pos.x+i.local_pos.z)*0.3-0.15) - (i.local_pos.y*(2)+0.3)+ _AlphaOfs);
                return col;
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}
