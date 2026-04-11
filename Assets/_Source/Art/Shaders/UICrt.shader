Shader "UI Toolkit/UICrt"
{
    Properties
    {
        _MainTex ("UI Texture", 2D) = "white" {}

        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 1.0
        _ScanlineDensity ("Scanline Density", Range(100,600)) = 290
        _ScanlineSpeed ("Scanline Speed", Float) = 2.0

        _MaskIntensity ("Mask Intensity", Range(0,1)) = 0.14
        _MaskScale ("Mask Scale", Float) = 180

        _NoiseIntensity ("Noise Intensity", Range(0, 1.0)) = 0.4

        _Tint ("Tint", Color) = (0.6, 1.0, 0.6, 1)
        _OverlayAlpha ("Overlay Alpha", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        ZWrite Off
        ZTest Always
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityUIEFilter.cginc"
            #include "Filters.cginc"

            sampler2D _MainTex;
            float4 _Tint;

            float _ScanlineIntensity;
            float _ScanlineDensity;
            float _ScanlineSpeed;

            float _MaskIntensity;
            float _MaskScale;

            float _NoiseIntensity;
            float _OverlayAlpha;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint rectIndex : TEXCOORD1;
            };

            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            v2f vert(FilterVertexInput v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.rectIndex = GetFilterRectIndex(v);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                float2 uv = NormalizeUVs(i.uv, uvRect);

                fixed4 col = tex2D(_MainTex, i.uv);

                float time = _Time.y;

                // Scanlines
                float scan = sin((uv.y + time * _ScanlineSpeed) * _ScanlineDensity * 3.14159);
                scan = 1.0 - saturate(abs(scan)) * _ScanlineIntensity;

                // Shadow mask
                float2 grid = frac(uv * _MaskScale);
                float mask = step(0.5, grid.x);
                mask = lerp(1.0, mask, _MaskIntensity);

                // Noise
                float noise = (Hash(uv * 800 + time) - 0.5) * _NoiseIntensity;

                // Overlay color
                float3 crt = _Tint.rgb * scan * mask;

                crt += noise;

                // Alpha-independent overlay
                float overlayAlpha = _OverlayAlpha * saturate(_ScanlineIntensity + _MaskIntensity);

                col.rgb = lerp(col.rgb, col.rgb + crt, overlayAlpha);

                return col;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}