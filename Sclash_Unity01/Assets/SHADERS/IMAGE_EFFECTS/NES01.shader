// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/NES01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorRange ("Color range reduction", Float) = 4.0
	}


	CGINCLUDE
		#include "UnityCG.cginc"

		#pragma vertex vert_img
		#pragma fragment frag
		

		sampler2D _CameraDepthTexture;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float2 _MainTex_TexelSize;
		int _ColorRange;

		static const float EPSILON = 1e-10;
	ENDCG


    SubShader
    {
		Cull Off
		ZWrite Off
		ZTest Always


        Tags
		{
			"RenderType" = "Opaque"
		}


        Pass
        {
			Name "Edge detection pass"


            CGPROGRAM
				float4 frag(v2f_img i) : SV_TARGET
				{
					float4 tex = tex2D(_MainTex, i.uv);

					int r = (tex.r - EPSILON) * _ColorRange;
					int g = (tex.g - EPSILON) * _ColorRange;
					int b = (tex.b - EPSILON) * _ColorRange;

					return float4(r / float(_ColorRange - 1), g / float(_ColorRange - 1), b / float(_ColorRange - 1), 1.0);
				}
            ENDCG
        }
    }
}
