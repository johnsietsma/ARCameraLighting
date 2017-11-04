Shader "AR/Lambert_ARCameraLit"
{
    Properties
    {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LightingStrength("Lighting Strength", Float) = 0.5
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        CGPROGRAM
		#pragma target 3.0
        #pragma surface surf Lambert noforwardadd finalcolor:ARCameraFinalColor

		sampler2D _MainTex;

		sampler2D _ARCameraRenderTexture;
		float _LightingStrength;
		fixed _GlobalLightEstimation;

        struct Input
        {
            float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
        };

		#include "ARCameraLighting.cginc"
		#include "Lighting.cginc"

		// This should be in an include file, but it's giving me build errors on Android :(
		float2 SphereMapUVCoords( float3 viewDir, float3 normal )
		{
			// Sphere mapping. Find reflection and tranform into UV coords.
			float3 reflection = reflect(viewDir, normal);
			float m = 2. * sqrt(
				pow(reflection.x, 2.) +
				pow(reflection.y, 2.) +
				pow(reflection.z + 1., 2.)
			);
			return reflection.xy / m + .5;
		}

		float4 ARCameraSphereMap(half3 viewDir, half3 normal)
		{
			float2 texcoord = SphereMapUVCoords(viewDir, normal);
			texcoord = 1 - texcoord;
			return tex2D(_ARCameraRenderTexture, texcoord);
		}

		void ARCameraFinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			color = color * ARCameraSphereMap(IN.viewDir, IN.worldNormal) * _LightingStrength * _GlobalLightEstimation;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}
