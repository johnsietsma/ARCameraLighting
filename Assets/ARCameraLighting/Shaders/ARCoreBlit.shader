Shader "ARCore/ARCoreBlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			// GLSL required for samplerExternalOES support
			GLSLPROGRAM

			#pragma only_renderers gles3

			#ifdef SHADER_API_GLES3
			#extension GL_OES_EGL_image_external_essl3 : require
			#endif

			uniform vec4 _MainTex_ST;

			#ifdef VERTEX

			// Orientation handling code from inbuilt shader AR/TangeARRender
			#define kPortrait 1.0
			#define kPortraitUpsideDown 2.0
			#define kLandscapeLeft 3.0
			#define kLandscapeRight 4.0

			varying vec2 textureCoord;
			uniform float _ScreenOrientation;

			void main()
			{
				textureCoord = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

				if (_ScreenOrientation == kPortrait)
				{
					float origX = textureCoord.x;
					textureCoord.x = 1.0 - textureCoord.y;
					textureCoord.y = 1.0 - origX;
				}
				else if (_ScreenOrientation == kPortraitUpsideDown)
				{
					float origX = textureCoord.x;
					textureCoord.x = textureCoord.y;
					textureCoord.y = origX;
				}
				else if (_ScreenOrientation == kLandscapeLeft)
				{
					textureCoord.y = 1.0 - textureCoord.y;
				}
				else if (_ScreenOrientation == kLandscapeRight)
				{
					textureCoord.x = 1.0 - textureCoord.x;
				}
			}

			#endif

			#ifdef FRAGMENT

			varying vec2 textureCoord;
			uniform samplerExternalOES _MainTex;

			void main()
			{
				gl_FragColor = texture2D(_MainTex, textureCoord);
			}

			#endif
			ENDGLSL
		}
	}

	SubShader
	{

		Pass
		{
			// CG version for testing in the Editor

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
