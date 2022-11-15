Shader "StencilOutlinePostEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass {
            Stencil{
                Ref 1
                Comp Equal
            }

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _MainTex;
            half4 _OutlineColor;

            fixed4 frag(v2f_img i) : COLOR {
                //fixed4 c = _Color;
                return _OutlineColor;
            }

            ENDCG
        }
    }
}