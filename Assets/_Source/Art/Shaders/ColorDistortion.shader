Shader "UI Toolkit/ColorDistortion"
{
    Properties
    {
        _MainTex ("UI Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _DistortionAmount ("Distortion", Range(0,0.05)) = 0.01
        _DistortionSpeed ("Speed", Range(0,10)) = 1
        _NoiseScale ("Noise Scale", Range(1,50)) = 10
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0; // ← Layout UV
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _DistortionAmount;
            float _DistortionSpeed;
            float _NoiseScale;

            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = UnityObjectToClipPos(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float time = _Time.y * _DistortionSpeed;

                float2 uv = i.uv;

                float4 col = tex2D(_MainTex, uv);


                return col;
            }
            ENDHLSL
        }
    }
}
