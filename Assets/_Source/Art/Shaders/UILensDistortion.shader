Shader "UI Toolkit/UILensDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0.0, 1.0)) = 0.35
        _Power ("Power", Range(1.0, 3.0)) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
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
            #pragma multi_compile _ UIE_OUTPUT_LINEAR

            #include "UnityCG.cginc"
            #include "UnityUIEFilter.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint rectIndex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Distortion;
            float _Power;

            v2f vert (FilterVertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.rectIndex = GetFilterRectIndex(v);
                return o;
            }

            float2 NormalizeUVs(float2 uv, float4 uvRect)
            {
                return (uv - uvRect.xy) / uvRect.zw;
            }

            float2 MapToUVRect(float2 uv, float4 uvRect)
            {
                return uv * uvRect.zw + uvRect.xy;
            }

            // Вогнутая линза (helmet / visor)
            float2 HelmetDistortion(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float2 d = uv - center;

                float r = length(d);
                float k = 1.0 / (1.0 + _Distortion * pow(r, _Power));

                return center + d * k;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                float2 uv = NormalizeUVs(i.uv, uvRect);

                uv = HelmetDistortion(uv);
                uv = clamp(uv, 0.0, 1.0);

                float2 finalUV = MapToUVRect(uv, uvRect);
                fixed4 col = tex2D(_MainTex, finalUV);

                #if UIE_OUTPUT_LINEAR
                col.rgb = GammaToLinearSpace(col.rgb);
                #endif

                return col;
            }
            ENDCG
        }
    }
}
