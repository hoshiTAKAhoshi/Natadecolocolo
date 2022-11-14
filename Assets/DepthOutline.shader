Shader "DepthOutline"
{
    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off

        Tags { "RenderType"="Opaque" }

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
            };
            
            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_ST;
            float4 _CameraDepthTexture_TexelSize;
            float _OutlineThreshold;
            float4 _OutlineColor;
            float _OutlineThick;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CameraDepthTexture);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick;
                float diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick;
                float col00 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + half2(-diffX, -diffY)).r);
                float col10 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + half2(0, -diffY)).r);
                float col01 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + half2(-diffX, 0)).r);
                float col11 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + half2(0, 0)).r);
                float outlineValue = (col00 - col11) * (col00 - col11) + (col10 - col01) * (col10 - col01);

                clip(outlineValue - _OutlineThreshold);

                return _OutlineColor;
            }
            ENDCG
        }
    }
}