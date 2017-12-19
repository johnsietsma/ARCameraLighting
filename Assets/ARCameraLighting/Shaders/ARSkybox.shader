Shader "Skybox/ARSkybox"
{
	Properties
	{
		_LightingTex("Lighting Tex", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata
	{
		float4 position : POSITION;
		float3 normal : NORMAL;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float2 texcoord : TEXCOORD0;
	};

	// This relies on a RenderTexture of this name being created in ARCoreCameraRenderTexture.cs.
	sampler2D _LightingTex;
	float4x4 _WorldToCameraMatrix;

	float2 SphereMapUVCoords( float3 viewDir, float3 normal )
	{
		// Sphere mapping. Find reflection and tranform into UV coords.
		// Heavily inspired by https://www.clicktorelease.com/blog/creating-spherical-environment-mapping-shader/
		float3 reflection = reflect(viewDir, normal);
		float m = 2. * sqrt(
			pow(reflection.x, 2.) +
			pow(reflection.y, 2.) +
			pow(reflection.z + 1., 2.)
		);
		return reflection.xy / m + .5;
	}

	v2f vert(appdata v)
	{
		// Create a sphere map with a texture whose center is at the viewDir/sphere intersection.
		// The texture is wrapped around the sphere so that the corners meet directly behind the camera.
		// To do this we could operate in static viewDir (0,0,1) space. We always want to look at the center on the texture.
		// When we move the phone, there is no need to change the view direction.
		// When rendering a skybox, the view direction is altered for each face. Grab the world space view direction to each vert
		//  then reverse the camera's view direction, bringing it back to view space.
		float3 viewDir = -normalize(WorldSpaceViewDir(v.position));
		viewDir = mul(_WorldToCameraMatrix, float4(viewDir,0));

		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord = SphereMapUVCoords(viewDir, v.normal);

		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return tex2D(_LightingTex, i.texcoord);
	}

	ENDCG

	SubShader
	{
		Tags{ "RenderType" = "Background" "Queue" = "Background" }
			Pass
		{
			ZWrite Off
			Cull Off
			Fog{ Mode Off }
			CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag
			ENDCG
		}
	}
}