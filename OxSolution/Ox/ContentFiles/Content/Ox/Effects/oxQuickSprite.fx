#include "baseStandard.fx"

void SpriteVertexShader(
	float4 inPosition : POSITION0,
	float4 inColor : COLOR0,
	float4 inEffectColor : COLOR1,
	float2 inTexCoord: TEXCOORD0,
	out float4 outPosition : POSITION0,
	out float4 outColor : COLOR0,
	out float2 outTexCoord : TEXCOORD0)
{
	outPosition = mul(inPosition, xWorldViewProjection);
   	outTexCoord = inTexCoord;
   	outColor = inColor * inEffectColor;
}

void SpritePixelShader(
	float4 inPosition : POSITION0,
	float4 inColor : COLOR0,
	float2 inTexCoord : TEXCOORD0,
	out float4 outColor : COLOR0)
{
	outColor = tex2D(xDiffuseMapSampler, inTexCoord) * inColor;
}

technique Normal
{
    pass Normal
    {
        VertexShader = compile vs_1_1 SpriteVertexShader();
        PixelShader = compile ps_1_1 SpritePixelShader();
    }
}
