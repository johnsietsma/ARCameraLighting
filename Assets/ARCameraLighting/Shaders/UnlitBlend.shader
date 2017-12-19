Shader "Unlit/UnlitBlend"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlendAmount ("Blend Amount", Range(0,1)) = 0.2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha // Blend via alpha amount

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _BlendAmount;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a = _BlendAmount; // Alpha will determine how much we blend with the existing buffer
				return col;
			}
			ENDCG
		}
	}
}
