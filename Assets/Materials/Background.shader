// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Background"
{
    Properties
    {
        // マテリアルインスペクターの Color プロパティ、デフォルトを白に
        _Color ("Main Color", Color) = (1,1,1,1)
        _NtdccScreenPos("NtdccScreenPos", Vector) = (0.0, 0.509, 0.0)
        _Radius0("Radius0", float) = 400.0
        _Radius1("Radius1", float) = 300.0
        //_Thickness("Thickness", float) = 20.0

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;

            };

            // 頂点シェーダー
            // 今回は、 "appdata" 構造体の代わりに、入力を手動で書き込みます
            // そして v2f 構造体を返す代わりに、1 つの出力
            // float4 のクリップ位置だけを返します
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.screenPos = ComputeScreenPos(o.vertex)*_ScreenParams;//0~1
                return o;
            }
            
            // マテリアルからのカラー
            fixed4 _Color;
            float4 _NtdccScreenPos;
            float _Radius0;
            float _Radius1;
            float _Thickness;
            // ピクセルシェーダー、入力不要
            fixed4 frag (v2f i) : SV_Target
            {
                _Radius0 *= _ScreenParams.y/100;
                _Radius1 *= _ScreenParams.y/100;
                _Thickness *= _ScreenParams.y/100;
                half2 npos = _NtdccScreenPos.xy;
                if(distance(i.screenPos,npos)>_Radius1 && distance(i.screenPos,npos)<_Radius0)
                    return fixed4(1,1,1,1);
                else
                    return _Color; // 単に返します
            }
            ENDCG
        }
    }
}