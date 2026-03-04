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

            float4 _MainTex_TexelSize;

            float4 _Color;
            float2 _Coordinate;
            float _Size;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                float2 texel = float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
                float2 pixelUV = floor(uv / texel) * texel + texel * 0.5;

                fixed4 baseCol = tex2D(_MainTex, pixelUV);

                float2 diff = uv - _Coordinate;
                float dist2 = dot(diff, diff);

                if (dist2 <= (_Size * _Size))
                {
                    float2 brushUV = diff / (_Size * 2) + 0.5;
                    brushUV = saturate(brushUV);

                    fixed4 brush = tex2D(_BrushTex, brushUV);

                    brush.a = step(0.5, brush.a);

                    fixed4 paint = brush * _Color;

                    baseCol = lerp(baseCol, paint, brush.a);
                }

                return baseCol;
            }
            ENDCG
        }
    }
}