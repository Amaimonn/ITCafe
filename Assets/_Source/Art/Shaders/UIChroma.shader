Shader "UI Toolkit/UIChroma"
{
    Properties
    {
        _MainTex("Texture2D", 2D) = "white" {}

        _BloomThreshold ("Bloom Threshold", Range(0, 1)) = 0.16
        _BloomIntensity ("Bloom Intensity", Range(0, 2)) = 0.5

        _RGBShiftPixels ("RGB Shift Pixels", Range(0, 100)) = 1
        _ColorTint ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _ShiftIntensity ("Shift Intensity", Range(0, 3)) = 1

        _Contrast ("Contrast", Range(0.5, 2)) = 1.2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Cull Off
        ZWrite False
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUIEFilter.cginc"
            #include "Filters.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint rectIndex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BloomThreshold;
            float _BloomIntensity;
            float _RGBShiftPixels;
            float4 _ColorTint;
            float _ShiftIntensity;
            float _Contrast;

            v2f vert(FilterVertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.rectIndex = GetFilterRectIndex(v);

                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                float2 uv = NormalizeUVs(i.uv, uvRect);

                float2 shiftUV = float2(_RGBShiftPixels, 0.0) * _MainTex_TexelSize.xy;

                float4 shiftedCol;
                shiftedCol.r = tex2D(_MainTex, MapToUVRect(uv + shiftUV, uvRect)).r;
                shiftedCol.g = tex2D(_MainTex, MapToUVRect(uv, uvRect)).g;
                shiftedCol.b = tex2D(_MainTex, MapToUVRect(uv - shiftUV, uvRect)).b;

                float4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, shiftedCol.rgb, _ShiftIntensity);

                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float bloom = smoothstep(_BloomThreshold, 1.0, brightness);
                col.rgb += bloom * _BloomIntensity * _ColorTint.rgb;

                col.rgb = pow(col.rgb, _Contrast);
                return col;
            }
            ENDCG
        }
    }
}