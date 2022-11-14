Shader "Custom/OutlineStencilWrite"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        //輪郭の描画
        Pass
        {
            Tags { "Queue" = "Transparent"}

            Stencil{
                Ref 1
                Comp GEqual
                Pass replace
            }
            CGPROGRAM
            //Ztest
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
                float4 pos : SV_POSITION;
                float4 localPos : TEXCOORD3;
                half3 normal:TEXCOORD2;
            };

            fixed4 _Amplitude;

            v2f vert(appdata v) {
                v2f o;
                
                v.vertex *= 1.1;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.localPos = v.vertex;
                o.normal = UnityObjectToWorldNormal(v.normal);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half NdotL = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                return o;
            }


            half4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                return float4(0.1,1,1,0.1);// * i.diff * SHADOW_ATTENUATION(i);
            }


            ENDCG
            Cull Front

        }
    }
}
