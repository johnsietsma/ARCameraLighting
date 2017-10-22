#define PI 3.1415926535897932384626433832795
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

/*fixed3 GetNormal(half3 viewDir, fixed3 worldNormal)
{
	float3 refl = reflect(viewDir, worldNormal);

	return -refl;
}*/


// Look up the the color of a texture projected onto a sphere.
// Texture is wrapped from the positive z-axis and can be partially wrapped using 'wrapAmount'.
float4 SphereProjectionColor(fixed3 viewDir, fixed3 worldNormal)
{
	/*
	viewDir = -viewDir;

	fixed3 normal = GetNormal(viewDir, worldNormal);

	float oneMinusWrapAmount = 1 - _ProjectionWrapAmount * -sign(viewDir.z);

	float2 nXZ = normal.xz;
	float2 nYZ = normal.yz;

	// Find angles of the normals
	//float angleXZ = (atan2(nXZ.x, nXZ.y) / (2 * PI)) + 0.5;
	//float angleYZ = (atan2(nYZ.x, nYZ.y) / (2 * PI)) + 0.5;

	//float2 uv = float2(angleXZ, 1-angleYZ);

	float u = dot(normal, fixed3(1, 0, 0)) * 0.5 + 0.5;
	float v = dot(normal, fixed3(0, -1, 0)) * 0.5 + 0.5;
	float2 uv = float2(u, v);

	if (sign(viewDir.z)<0) {
		//uv = float2(0, 0);
	}

	// Stretch UVs to allow partial coverage
	uv = uv * (1 + (oneMinusWrapAmount * 2));
	uv = uv - oneMinusWrapAmount;


	//return fixed4(uv, 0, 1)* step(dot(viewDir, worldNormal), _ProjectionWrapAmount);
	return tex2D(_ProjectionTexture, uv);// *_ProjectionStrength * _GlobalLightEstimation * step(dot(nXZ, float2(0, -1)), _ProjectionWrapAmount);
	*/

	return fixed4(1, 0, 0, 1);
}


fixed4 LightingTextureProjection_Standard(SurfaceOutputStandard o, half3 viewDir, UnityGI gi)
{
	//gi.light.color = SphereProjectionColor(viewDir, o.Normal).xyz;
	//gi.light.dir = GetNormal(viewDir, o.Normal);
	return LightingStandard(o, viewDir, gi);
	//return SphereProjectionColor(viewDir, o.Normal);  //LightingStandard(o, viewDir, gi);
}


void LightingTextureProjection_Standard_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
{
	LightingStandard_GI(s, data, gi);
	//gi.indirect.diffuse += SphereProjectionColor(s.Normal).xyz;
}

inline fixed4 LightingBlinnPhong2(SurfaceOutput s, half3 viewDir, UnityGI gi)
{
	return LightingBlinnPhong(s, viewDir, gi) + SphereProjectionColor(viewDir, s.Normal);

}


inline void LightingBlinnPhong2_GI(
	SurfaceOutput s,
	UnityGIInput data,
	inout UnityGI gi)
{
	LightingBlinnPhong_GI(s, data, gi);
}
