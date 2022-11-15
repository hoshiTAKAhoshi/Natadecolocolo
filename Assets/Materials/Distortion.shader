Shader "Custom/Distortion"
{
    Properties
    {
        _Diameter("Diameter", Float) = -0.3
        _Ratio("Ratio", Float) = 0.4
        _Alpha("Alpha", Float) = 1.0
        _ThicknessRatio("ThicknessRatio", Float) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha //重なったオブジェクトの画素の色とのブレンド方法の指定
        LOD 200
        Cull off
        //ZTest Always

        GrabPass
        {
            //"_GrabTexture"
        }

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

            float _Diameter;
            float _Ratio;
            float _ThicknessRatio;

            v2f vert(appdata_base v) {
                v2f o;

                float thickness_deff = 0.417;
                // 太さのどこの点か 
                float len_ratio = (1-(length(float3(v.vertex.x,0,v.vertex.z))-0.5)/thickness_deff);
                float rad = atan2(v.vertex.x,v.vertex.z)+3.141592/2;
                v.vertex.x -= (_Diameter+(len_ratio*thickness_deff*(1-_ThicknessRatio)))*cos(rad);
                v.vertex.z += (_Diameter+(len_ratio*thickness_deff*(1-_ThicknessRatio)))*sin(rad);
                v.vertex.y -= (_Diameter+(len_ratio*thickness_deff*(1-_ThicknessRatio)))*2;
                v.vertex.y *= -1.5;
                float len = length(float3(v.vertex.x,0,v.vertex.z));
                float a = 0.5 + _Diameter+thickness_deff*(1-_ThicknessRatio); //太さ始まり  
                float b = thickness_deff-thickness_deff*(1-_ThicknessRatio);   // 太さ終わり 
                //rad += _Ratio*sin((len-a)/b*3.141592);  
                rad += _Ratio*(sin((len-a)/b*3.141592*2 - 3.141592/2)+1)/2; 
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex*0+float3(-len*cos(rad),v.vertex.y,len*sin(rad))));
                o.localPos = v.vertex;

                return o;
            }

            sampler2D _GrabTexture;
            float _Alpha;

            half4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,0);

                col += tex2Dproj(_GrabTexture, i.grabPos)+fixed4(1,1,1,1)*0.2*_Ratio*_Ratio;
                col.a=_Alpha;
                return col; 
            }
            ENDCG
        }
    }
}
