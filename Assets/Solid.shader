// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Solid"
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
            
            // 頂点シェーダー
            // 今回は、 "appdata" 構造体の代わりに、入力を手動で書き込みます
            // そして v2f 構造体を返す代わりに、1 つの出力
            // float4 のクリップ位置だけを返します
            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
            
            // マテリアルからのカラー
            fixed4 _Color;

            // ピクセルシェーダー、入力不要
            fixed4 frag () : SV_Target
            {
                return _Color; // 単に返します
            }
            ENDCG
        }
    }
}