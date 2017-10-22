Shader "Skybox/ARSkybox"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
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

	sampler2D _MainTex;
	float4x4 _ARViewMatrix;

	v2f vert(appdata v)
	{
		// We want to ignore the view direction. The physical camera is always directly in front of the virtual camera.
		// When we move the phone, there is no need to change the view direction.
		// When rendering a skybox, the view direction is altered for each face. Grab the view direction, reverse the camera's
		// view direction, leaving only the skybox direction.
		float3 viewDir = -normalize(WorldSpaceViewDir(v.position));
		viewDir = mul(_ARViewMatrix, float4(viewDir,0));

		v2f o;
		o.position = UnityObjectToClipPos(v.position);

		// The sphere mapping. Transform the view direciton in UV coords.
		float3 reflection = reflect(viewDir, v.normal);
		float m = 2. * sqrt(
			pow(reflection.x, 2.) +
			pow(reflection.y, 2.) +
			pow(reflection.z + 1., 2.)
		);
		o.texcoord = reflection.xy / m + .5;

		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return tex2D(_MainTex, i.texcoord);
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