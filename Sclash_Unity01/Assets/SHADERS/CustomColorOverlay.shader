Shader "custom/ColorOverlay"
{
	Properties
	{
		[HideInInspector]
		_MainTex("Texture", 2D) = "white" {}
		_OverlayColor("Overlay Color", Color) = (1,1,1,1)
		_OverlayOpacity("Overlay Opacity", Range(0,1)) = 1
	}
		SubShader
		{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
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
					fixed4 color : COLOR;
				};
				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
				};
				v2f vert(appdata v)
				{
					v2f o;
					o.color = v.color;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				fixed4 _OverlayColor;
				float _OverlayOpacity;
				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 mainTex = tex2D(_MainTex, i.uv);
					mainTex *= i.color;
					fixed4 overlay = lerp(mainTex, _OverlayColor, _OverlayOpacity);
					overlay.a = mainTex.a;
					return overlay;
				}
				ENDCG
			}
		}
}