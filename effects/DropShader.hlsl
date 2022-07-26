sampler2D implicitInput : register(s0);

static float1 factor = 0.4;
static float1 blueFactor = 1;

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(implicitInput, uv);
    color.rgb = float3(color.r * factor, color.g * factor, color.b * blueFactor);
    
    return color;
}
