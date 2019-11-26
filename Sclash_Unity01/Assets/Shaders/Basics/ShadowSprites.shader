Shader "Custom/SpriteShadow"
{
	Properties
	{
		_Color("Tint", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
	}
		
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}


		LOD 200

		Cull Off


		CGPROGRAM
		// Lambert lighting model, and enable shadows on all light types
		#pragma surface surf Lambert addshadow fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		fixed _Cutoff;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			clip(o.Alpha - _Cutoff);
		}


		ENDCG
	}


	FallBack "Diffuse"
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
Shader "Custom/SpriteShadow"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
				"LightMode" = "ForwardBase"
			}

			Cull Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
			

				#pragma vertex vert
				#pragma fragment frag
				//#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc" // for UnityObjectToWorldNormal
				#include "UnityLightingCommon.cginc" // for _LightColor0

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					fixed4 diff : COLOR1;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata_base IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = _Color;
					// factor in the light color
					half3 worldNormal = UnityObjectToWorldNormal(float3(0,0,1));
					half nl = max(0, dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz));
					OUT.diff = nl * _LightColor0;

					// the only difference from previous shader:
					// in addition to the diffuse lighting from the main light,
					// add illumination from ambient or light probes
					// ShadeSH9 function from UnityCG.cginc evaluates it,
					// using world space normal
					OUT.diff.rgb += ShadeSH9(half4(float3(1,1,1), 1));

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);

	#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
	#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.a = IN.color.a;
					return c;
				}
			ENDCG
			}
		}
}*/
/*
Shader "Custom/SpriteShadow"
{
	Properties
	{
		//_Color("Color", Color) = (1,1,1,1)
		//_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness("Smoothness", Range(0,1)) = 0.5
		//_Metallic("Metallic", Range(0,1)) = 0.0
		_Color("Tint", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM

			#pragma surface surf Standard fullforwardshadows alpha:fade
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Standard"
}*/