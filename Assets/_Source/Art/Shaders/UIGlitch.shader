Shader "UI Toolkit/UIGlitch"
{
    Properties
    {
        _MainTex ("UI Texture", 2D) = "white" {}

        _GlitchIntensity ("Glitch Intensity", Range(0, 5)) = 1
        _GlitchSpeed ("Glitch Speed Multiplier", Range(0, 5)) = 1
        _GlitchShift ("Glitch Shift", Range(0, 5)) = 0.1
        _GlitchBlockSize ("Block Size", Range(0.01, 5.00)) = 1
        _GlitchPulseFreq ("Pulse Frequency", Range(0, 1)) = 0.2
        
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 5.0

        _RGBShift ("RGB Shift", Range(0, 0.02)) = 0.0015
        _ColorTint ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)

        _ScanLineSpeed ("Scan Line Speed", Range(0, 20)) = 3.0
        _ScanLineDensity ("Scan Line Density", Range(1, 200)) = 50.0
        _ScanLineIntensity ("Scan Line Intensity", Range(0, 0.5)) = 0.03

        _BloomThreshold ("Bloom Threshold", Range(0, 1)) = 0.7
        _BloomIntensity ("Bloom Intensity", Range(0, 2)) = 0.5

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
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local_fragment _ GLITCH_ON
            #pragma multi_compile_local_fragment _ SCANLINE_ON
            #pragma multi_compile_local_fragment _ BLOOM_ON

            #include "UnityCG.cginc"
            #include "UnityUIEFilter.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint rectIndex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _UnscaledTime;

            float _GlitchIntensity;
            float _GlitchSpeed;
            float _GlitchShift;
            float _GlitchBlockSize;
            float _GlitchPulseFreq;

            float _NoiseScale;

            float _RGBShift;
            float4 _ColorTint;

            float _ScanLineSpeed;
            float _ScanLineDensity;
            float _ScanLineIntensity;

            float _BloomThreshold;
            float _BloomIntensity;

            float _Contrast;

            v2f vert(FilterVertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.rectIndex = GetFilterRectIndex(v);
                return o;
            }

            float2 NormalizeUVs(float2 uv, float4 uvRect)
            {
                return float2(
                    (uv.x - uvRect.x) / uvRect.z,
                    (uv.y - uvRect.y) / uvRect.w
                );
            }

            float2 MapToUVRect(float2 uv, float4 uvRect)
            {
                return float2(
                    uv.x * uvRect.z + uvRect.x,
                    uv.y * uvRect.w + uvRect.y
                );
            }

            float GetFastRandom(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float GetFastNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = GetFastRandom(i);
                float b = GetFastRandom(i + float2(1.0, 0.0));

                return lerp(a, b, f.x);
            }

            float GetGlitchPulse(float t, float freq)
            {
                float pulse = sin(t * freq * 10.0) * 0.5 + 0.5;
                float random_val = GetFastRandom(float2(t * 0.1, 0.0));
                return step(1.0 - freq * 0.5, pulse * random_val);
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                float2 uv = NormalizeUVs(i.uv, uvRect);

                float4 original_col = tex2D(_MainTex, MapToUVRect(uv, uvRect));
                float4 col = original_col;

                float time = _UnscaledTime * _GlitchSpeed;

                float glitchNoise = GetFastNoise(uv * _NoiseScale + time);

                float glitchPulse = GetGlitchPulse(time, _GlitchPulseFreq);

                float2 glitchUV = uv;

                float horizontalShift = (glitchNoise - 0.5) * _GlitchShift * glitchPulse * _GlitchIntensity;
                glitchUV.x += horizontalShift;

                float blockY = floor(uv.y / _GlitchBlockSize) * _GlitchBlockSize;
                float blockGlitch = GetFastRandom(float2(blockY, time * 0.3));

                float blockEffect = saturate(blockGlitch - 0.7) * glitchPulse;
                glitchUV.x += (GetFastRandom(float2(blockY, time)) - 0.5) * 0.02 * blockEffect * _GlitchIntensity;

                glitchUV = saturate(glitchUV);

                float4 shiftedCol;
                shiftedCol.r = tex2D(_MainTex, MapToUVRect(glitchUV + float2(_RGBShift, 0.0), uvRect)).r;
                shiftedCol.g = tex2D(_MainTex, MapToUVRect(glitchUV, uvRect)).g;
                shiftedCol.b = tex2D(_MainTex, MapToUVRect(glitchUV - float2(_RGBShift, 0.0), uvRect)).b;

                col.rgb = lerp(col.rgb, shiftedCol.rgb, _GlitchIntensity);

                float scanLine = sin(uv.y * _ScanLineDensity + _UnscaledTime * _ScanLineSpeed) * 0.5 + 0.5;
                scanLine = pow(scanLine, 4.0) * _ScanLineIntensity * _GlitchIntensity;
                col.rgb += scanLine;

                float flicker = 0.95 + GetFastRandom(float2(_UnscaledTime * 0.05, 0.0)) * 0.1;
                col.rgb *= flicker;

                col.rgb *= _ColorTint.rgb;

                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float bloom = smoothstep(_BloomThreshold, 1.0, brightness);
                col.rgb += bloom * _BloomIntensity * _ColorTint.rgb * _GlitchIntensity;

                col.rgb = pow(col.rgb, _Contrast);

                col.a = original_col.a;

                return col;
            }
            ENDCG
        }
    }
}