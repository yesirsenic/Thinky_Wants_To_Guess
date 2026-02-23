Shader "Custom/BrushShader"
{
    Properties
    {
        _MainTex ("Brush", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Coordinate ("Coordinate", Vector) = (0,0,0,0)
        _Size ("Size", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float2 _Coordinate;
            float _Size;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                float2 diff = uv - _Coordinate;
                float dist = length(diff);

                if (dist > _Size)
                    return tex2D(_MainTex, uv);

                float2 brushUV = diff / (_Size * 2) + 0.5;
                fixed4 brush = tex2D(_MainTex, brushUV);

                return brush * _Color;
            }
            ENDCG
        }
    }
}