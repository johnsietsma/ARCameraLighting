Shader "ARCore/TextureProjectionLambert"
{
    Properties
    {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ProjectionStrength("Projection Lighting Strength", Float) = 0.5
		_ProjectionWrapAmount("Wrap Amount", Float) = 0.8
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        CGPROGRAM
		#pragma target 3.0
        #pragma surface surf BlinnPhong noforwardadd// finalcolor:textureProjectionLighting

		sampler2D _MainTex;

		sampler2D _ProjectionTexture;
		float _ProjectionWrapAmount;
		float _ProjectionStrength;
		fixed _GlobalLightEstimation;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
        };

		#include "AREnvironmentSphereLighting.cginc"

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
