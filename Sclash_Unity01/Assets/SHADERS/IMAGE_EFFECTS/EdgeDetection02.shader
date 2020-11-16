// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/EdgeDetection02"
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


		float3 rgb2hsv(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
			float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}


		float3 hsv2rgb(float3 c)
		{
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
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

					float4 tex = tex2D(_MainTex, i.uv);
					float4 col = float4(sobel(i.uv), 1.0);
					float3 hsvTex = rgb2hsv(tex);
					hsvTex.y = 1.0;
					hsvTex.z = 1.0;
					col *= float4(hsv2rgb(hsvTex), 1.0);
					

					//return float4(_EdgeDetectionTreshold / sobel(i.uv), 1.0);
					return col;
				}
            ENDCG
        }
    }
}
