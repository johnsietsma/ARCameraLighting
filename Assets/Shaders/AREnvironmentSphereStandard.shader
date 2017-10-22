Shader "ARCore/TextureProjectionStandard"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_ProjectionStrength("Projection Lighting Strength", Float) = 0.5
		_ProjectionWrapAmount("Wrap Amount", Float) = 0.8
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        CGPROGRAM
		#pragma target 3.0
		#pragma surface surf TextureProjection_Standard noforwardadd// finalcolor:textureProjectionLighting

		sampler2D _MainTex;
		float4 _Color;
		half _Glossiness;
		half _Metallic;

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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}
