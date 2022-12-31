Shader "Custom/NoseAttachLight"
{
    Properties
    {
        _AlphaOfs ("AlphaOfs", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent+1" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        ZWrite Off

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
                float4 vertex : SV_POSITION;
                float4 local_pos : TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.local_pos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);                

                return o;
            }

            float _AlphaOfs;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
                col.a = saturate((0.15 - abs(i.local_pos.x+i.local_pos.z)*0.3) - (i.local_pos.y*(2)) + _AlphaOfs + 0.15*sin(_Time*40));
                return col;
            }
            ENDCG
        }
    }
}
