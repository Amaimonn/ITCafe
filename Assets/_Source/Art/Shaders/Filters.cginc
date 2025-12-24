float2 NormalizeUVs(float2 uv, float4 uvRect)
{
    return (uv - uvRect.xy) / uvRect.zw;
}

float2 MapToUVRect(float2 uv, float4 uvRect)
{
    return uv * uvRect.zw + uvRect.xy;
}