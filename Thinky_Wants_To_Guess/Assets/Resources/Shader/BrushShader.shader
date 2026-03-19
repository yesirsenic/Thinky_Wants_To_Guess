Shader "Custom/BrushShader"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _BrushTex ("Brush", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Coordinate ("Coordinate", Vector) = (0,0,0,0)
        _Size ("Size", Float) = 0.05
        _IsEraser ("IsEraser", Float) = 0
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
            float _IsEraser;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                // 기존 캔버스
                fixed4 baseCol = tex2D(_MainTex, uv);

                // 중심 거리 계산
                float2 diff = uv - _Coordinate;

                // 🔥 비율 보정 (원 깨짐 방지)
                float aspect = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
                diff.x *= aspect;

                float dist2 = dot(diff, diff);

                // =========================
                // 🔥 지우개
                // =========================
                if (_IsEraser > 0.5)
                {
                    if (dist2 <= (_Size * _Size))
                    {
                        baseCol = _Color;
                    }
                }
                else
                {
                    // =========================
                    // 🔥 브러시 (연필/펜 동일 구조)
                    // =========================
                    if (dist2 <= (_Size * _Size))
                    {
                        float2 brushUV = diff / _Size * 0.5 + 0.5;
                        brushUV.x /= aspect;

                        brushUV = saturate(brushUV);

                        fixed4 brush = tex2D(_BrushTex, brushUV);

                        // 🔥 완전 불투명 덮어쓰기 (핵심)
                        baseCol.rgb = lerp(baseCol.rgb, _Color.rgb, brush.a);
                        baseCol.a = 1.0;
                    }
                }

                return baseCol;
            }
            ENDCG
        }
    }
}