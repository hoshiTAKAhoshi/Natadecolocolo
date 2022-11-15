// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Background"
{
   Properties
    {
        // マテリアルインスペクターの Color プロパティ、デフォルトを白に
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            // 頂点シェーダー
            // 今回は、 "appdata" 構造体の代わりに、入力を手動で書き込みます
            // そして v2f 構造体を返す代わりに、1 つの出力
            // float4 のクリップ位置だけを返します
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                return o;
            }
            
            // マテリアルからのカラー
            fixed4 _Color;

            // ピクセルシェーダー、入力不要
            fixed4 frag (v2f i) : SV_Target
            {
                if(i.vertex.x>=100)
                    return _Color; // 単に返します
                else
                    return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}