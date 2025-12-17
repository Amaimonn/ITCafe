Shader "UI Toolkit/UIHelmetDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BulgeAmount ("Bulge Amount", Range(0.0, 2.0)) = 1.25
        _BulgeRadius ("Bulge Radius", Range(0.1, 1.0)) = 1.0
        _BulgeStrength ("Bulge Strength", Range(0.0, 3.0)) = 0.1
        _Refraction ("Refraction", Range(0.0, 0.5)) = 0.2
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

            #include "UnityCG.cginc"
            #include "UnityUIEFilter.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint rectIndex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _BulgeAmount;
            float _BulgeRadius;
            float _BulgeStrength;
            float _Refraction;

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

            float2 BulgeDistortion(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float2 d = uv - center;
                float r = length(d);
                float normalizedR = r / _BulgeRadius;
                
                float bulge = 0; // выпуклость: в центре = 0, на краю радиуса = _BulgeAmount
                
                if (normalizedR < 1.0)
                {
                    float t = 1.0 - normalizedR;
                    
                    bulge = _BulgeAmount * t * t * _BulgeStrength; // квадратичная кривая для плавного выпуклого эффекта
                    bulge += _Refraction * sin(normalizedR * 3.14159 * 2.0) * 0.1; //рефракция (искажение краев)
                }
                
                return uv + d * bulge;
            }

            float2 SphericalBulge(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float2 d = uv - center;
                float r = length(d);
                
                if (r < _BulgeRadius)
                {
                    float t = r / _BulgeRadius;
                    float sphereHeight = sqrt(1.0 - t * t);
                    float bulge = _BulgeAmount * sphereHeight * _BulgeStrength;
                    
                    float2 direction = d / (r + 0.0001);
                    
                    return uv + direction * bulge;
                }
                
                return uv;
            }

            float2 FisheyeBulge(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float2 d = uv - center;
                float r = length(d);
                
                if (r < _BulgeRadius)
                {
                    float theta = atan2(r, _BulgeRadius);
                    float newR = _BulgeRadius * theta * _BulgeAmount;
                    
                    newR *= _BulgeStrength;
                    
                    float2 direction = d / (r + 0.0001);
                    return center + direction * newR;
                }
                
                return uv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                float2 uv = NormalizeUVs(i.uv, uvRect);
                float2 originalUV = uv;

                float2 finalUV = MapToUVRect(originalUV, uvRect);
                fixed4 originalColor = tex2D(_MainTex, finalUV);

                uv = BulgeDistortion(uv);
                // uv = SphericalBulge(uv); // альтернатива
                // uv = FisheyeBulge(uv);   // другая альтернатива
                
                uv = clamp(uv, 0.0, 1.0);
                
                // плавное затухание к краям
                float2 d = uv - float2(0.5, 0.5);
                float r = length(d);
                float edgeFade = 1.0 - smoothstep(_BulgeRadius * 0.8, _BulgeRadius * 1.2, r);
                
                float2 distortedUV = MapToUVRect(uv, uvRect);
                fixed4 distortedColor = tex2D(_MainTex, distortedUV);
                
                // эффект отражения/освещения
                float3 lightDir = normalize(float3(0.3, 0.5, 1.0));
                float3 normal = normalize(float3(d * 0.5, 1.0));
                float lighting = saturate(dot(normal, lightDir)) * 0.3 + 0.7;
                
                fixed4 col = lerp(originalColor, distortedColor, edgeFade);
                col.rgb *= lighting;

                return col;
            }
            ENDCG
        }
    }
}