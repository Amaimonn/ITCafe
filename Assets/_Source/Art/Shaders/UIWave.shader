Shader "UI Toolkit/UIWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_ST;

            v2f vert(FilterVertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.rectIndex = GetFilterRectIndex(v);
                return o;
            }

            float2 RotateUV90(float2 uv)
            {
                return float2(uv.y, 1.0 - uv.x);
            }

            float2 MirrorUV(float2 uv)
            {
                return float2(1.0 - uv.x, 1.0 - uv.y);
            }

            float2 WaveUV(float2 uv, float time)
            {
                float wave = sin(uv.y * 10.0 + time * 3.0) * 0.02;
                return uv + float2(wave, 0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Получаем UV регион по индексу
                float4 uvRect = GetFilterUVRect(i.rectIndex);
                // uvRect = ExpandRect(uvRect, 0.05);
                // Конвертируем UV атласа в диапазон [0,1]
                float2 uv = NormalizeUVs(i.uv, uvRect);

                // === НАЧАЛО: манипуляции с UV ===

                // Пример 1: Инвертирование по оси Y
                // uv.y = 1.0 - uv.y;

                // Пример 2: Вращение UV (раскомментировать при необходимости)
                // uv = RotateUV90(uv);

                // Пример 3: Зеркальное отражение (раскомментировать при необходимости)
                // uv = MirrorUV(uv);

                // Пример 4: Wave эффект с использованием времени
                // uv = (uv - 0.5) * 1.05 + 0.5;
                uv = WaveUV(uv, _Time.y);

                float borderWidth = 1e-16; // Adjust as needed
                clip(uv.x - borderWidth);
                clip(uv.y - borderWidth);
                clip(1.0 - uv.x - borderWidth);
                clip(1.0 - uv.y - borderWidth);
                //


                // Пример 5: Масштабирование UV
                // float2 center = float2(0.5, 0.5);
                // uv = (uv - center) * 0.8 + center;

                // Пример 6: Повторение текстуры (tiling)
                // uv = frac(uv * 2.0);

                // === КОНЕЦ: манипуляции с UV ===

                // Конвертируем UV обратно в регион атласа
                uv = MapToUVRect(uv, uvRect);
                uv = clamp(uv, 0.0, 1.0);

                // Получаем цвет из текстуры
                fixed4 col = tex2D(_MainTex, uv);

                // === НАЧАЛО: эффекты цвета ===

                // Пример 1: Инверсия цвета
                // col.rgb = 1.0 - col.rgb;

                // Пример 2: Оттенки серого
                // float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                // col.rgb = float3(luminance, luminance, luminance);

                // Пример 3: Сепия
                // float3 sepia = float3(1.2, 1.0, 0.8);
                // float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                // col.rgb = float3(gray, gray, gray) * sepia;

                // Пример 4: Добавление шума
                // float noise = frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
                // col.rgb += noise * 0.1;

                // === КОНЕЦ: эффекты цвета ===

                return col;
            }
            ENDCG
        }
    }
}