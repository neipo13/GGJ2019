sampler _mainTex;
bool flashing;

float4 frag(float2 coords : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(_mainTex, coords);
    if (flashing && textureColor.a == 1)
        return float4(1, 1, 1, 1);
    return textureColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 frag();
    }
}