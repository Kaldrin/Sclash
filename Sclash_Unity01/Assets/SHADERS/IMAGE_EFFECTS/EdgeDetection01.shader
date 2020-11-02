// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/EdgeDetection01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainColor("Main color", Color) = (0.5, 0.1, 0.5, 1)
		_SecondColor("Main color", Color) = (0.5, 0.1, 0.5, 1)
		_EdgeDetectionTreshold("Edge detection treshold", Float) = 0.4
		_ColorTreshold("Color treshold", Float) = 0.4
	}


	CGINCLUDE
		#include "UnityCG.cginc"
		


		sampler2D _CameraDepthTexture;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float2 _MainTex_TexelSize;
		float _EdgeDetectionTreshold;
		float4 _MainColor;
		float4 _SecondColor;
		float _ColorTreshold;


		float3 sobel(float2 uv)
		{
			float x = 0;
			float y = 0;
			float2 texelSize = _MainTex_TexelSize;



			x += tex2D(_MainTex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
			x += tex2D(_MainTex, uv + float2(-texelSize.x, 0)) * -2.0;
			x += tex2D(_MainTex, uv + float2(-texelSize.x, texelSize.y)) * -1.0;

			x += tex2D(_MainTex, uv + float2(texelSize.x, -texelSize.y)) *  1.0;
			x += tex2D(_MainTex, uv + float2(texelSize.x, 0)) *  2.0;
			x += tex2D(_MainTex, uv + float2(texelSize.x, texelSize.y)) *  1.0;

			y += tex2D(_MainTex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
			y += tex2D(_MainTex, uv + float2(0, -texelSize.y)) * -2.0;
			y += tex2D(_MainTex, uv + float2(texelSize.x, -texelSize.y)) * -1.0;

			y += tex2D(_MainTex, uv + float2(-texelSize.x, texelSize.y)) *  1.0;
			y += tex2D(_MainTex, uv + float2(0, texelSize.y)) *  2.0;
			y += tex2D(_MainTex, uv + float2(texelSize.x, texelSize.y)) *  1.0;


			float result = sqrt(x * x + y * y) * _EdgeDetectionTreshold;
			if (result > _ColorTreshold)
				result = _ColorTreshold;

			return result;
		}
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
				#pragma vertex vert_img
				#pragma fragment frag


				float4 frag(v2f_img i) : SV_TARGET
				{
					float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
					depth = Linear01Depth(depth);

					float4 col = float4(sobel(i.uv), 1.0);
					col = lerp(_SecondColor, _MainColor, col.r);
					//col = col * _MainColor;

					//return float4(_EdgeDetectionTreshold / sobel(i.uv), 1.0);
					return col;
				}
            ENDCG
        }
    }
}
