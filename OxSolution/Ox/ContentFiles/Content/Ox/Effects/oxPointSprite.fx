#include "baseFogReceiver.fx"

int xViewportHeight;

void NormalVertexShader(
	float4 inPosition : POSITION0,
	float1 inSize : PSIZE,
	float4 inColor : COLOR0,
	out float4 outPosition : POSITION0,
	out float1 outSize : PSIZE,
	out float4 outColor : COLOR0,
	out float1 outFogLerp : COLOR1)
{
	// position
	outPosition = mul(inPosition, xWorldViewProjection);
	
	// color
	outColor = inColor;
	
	// size
	float4 cameraPositionLocal = mul(xCameraPosition, xWorldInverse);
	float d = distance(inPosition, cameraPositionLocal);
	outSize = xViewportHeight * inSize / (1 + d);
	
	// fog
	float4 positionWorld = mul(inPosition, xWorld);
	float distance = distance(positionWorld, xCameraPosition);
	float fogRange = xFogEnd - xFogStart;
	float distanceIntoFogStart = distance - xFogStart;
	outFogLerp = saturate(distanceIntoFogStart / fogRange);
}

void NormalPixelShader(
	float1 inSize : PSIZE,
#ifdef XBOX
	float4 inTexCoord : SPRITETEXCOORD,
#else
	float2 inTexCoord : TEXCOORD0,
#endif
	float4 inColor : COLOR0,
	float1 inFogLerp : COLOR1,
	out float4 outColor : COLOR0)
{
	// color
	float2 texCoord;	
#ifdef XBOX
	texCoord = abs(inTexCoord.zw);
#else
	texCoord = inTexCoord.xy;
#endif
	outColor = tex2D(xDiffuseMapSampler, texCoord);	
	outColor *= inColor;	
	
	// apply fog
	if (xFogEnabled) outColor.rgb = ApplyFog(outColor.rgb, inFogLerp);
}

technique Normal
{
	pass Normal
	{
		VertexShader = compile vs_2_0 NormalVertexShader();
		PixelShader = compile ps_2_0 NormalPixelShader();
	}
}
