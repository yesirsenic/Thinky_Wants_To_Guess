Shader "Custom/BrushShader"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}    // 기존 캔버스
        _BrushTex ("Brush", 2D) = "white" {} // 브러시 원
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
            sampler2D _BrushTex;
            float4 _Color;
            float2 _Coordinate;
            float _Size;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                // 기존 캔버스 색
                fixed4 baseCol = tex2D(_MainTex, uv);

                float2 diff = uv - _Coordinate;
                float dist = length(diff);

                if (dist > _Size)
                    return baseCol;

                float2 brushUV = diff / (_Size * 2) + 0.5;
                fixed4 brush = tex2D(_BrushTex, brushUV);

                fixed4 paint = brush * _Color;

                return lerp(baseCol, paint, brush.a);
            }
            ENDCG
        }
    }
}