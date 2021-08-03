#include "baseStandard.fx"

void NormalVertexShader(
	float4 inPosition : POSITION0,
	inout float4 ioColor : COLOR0,
    out float4 outPosition : POSITION0)
{
    outPosition = mul(inPosition, xWorldViewProjection);
}

void NormalPixelShader(inout float4 ioColor : COLOR0) { }

technique Normal
{
    pass Normal
    {       
        VertexShader = compile vs_1_1 NormalVertexShader();
        PixelShader = compile ps_1_1 NormalPixelShader();
    }
}
