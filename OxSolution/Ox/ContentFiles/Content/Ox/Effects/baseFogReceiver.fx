#ifndef BASE_FOG_RECEIVER_FX
#define BASE_FOG_RECEIVER_FX

#include "baseStandard.fx"

float3 xFogColor;
float xFogStart;
float xFogEnd;
bool xFogEnabled;

float3 ApplyFog(float3 color, float lerpValue)
{
	return lerp(color, xFogColor, lerpValue);
}

#endif
