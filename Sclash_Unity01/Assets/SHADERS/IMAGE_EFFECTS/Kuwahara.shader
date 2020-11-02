Shader "Hidden/Custom/Kuwahara"
{
	HLSLINCLUDE
		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		float _Blend;
		float2 _MainTex_TexelSize;
		int	_KernelSize;
		int _PowX;
		int _PowY;
		float _Spread;


		struct region
		{
			float3 mean;
			float variance;
		};


		region calcRegion(int2 lower, int2 upper, int samples, float2 uv)
		{
			region r;

			float3 sum = 0.0;
			float3 squareSum = 0.0;

			[loop]
			for (int x = lower.x; x <= upper.x; ++x)
			{
				[loop]
				for (int y = lower.y; y <= upper.y; ++y)
				{
					float2 offset = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
					float3 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);

					sum += tex;
					squareSum += tex * tex;
				}
			}

			r.mean = sum / samples;

			float3 variance = abs((squareSum / samples) - (r.mean * r.mean));
			r.variance = length(variance);
			return r;
		}


		float getKernelSize(float2 uv)
		{
			float distance = sqrt(pow(abs(uv.x - 0.5) * 2, _PowX) + pow(abs(uv.y - 0.5) * 2, _PowY));
			return min(_KernelSize * distance, 1.0);
		}	
	ENDHLSL

	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Tags { "RenderType" = "Opaque" }

		Pass
		{
			Name "PaintingPass"

			HLSLPROGRAM
				#pragma vertex VertDefault
				#pragma fragment Frag


				float4 Frag(VaryingsDefault i) : SV_Target
				{
					float3 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
					int tempKernelSize = getKernelSize(i.texcoord) * _Spread;

					// Kernel limits
					int upper = ((tempKernelSize)-1) / 2;
					int lower = -upper;

					// Kernel sample size (x * y)
					int samples = (upper + 1) * (upper + 1);

					// Calculate kuwahara's 4 regions
					region regionA = calcRegion(int2(lower, lower), int2(0, 0), samples, i.texcoord);
					region regionB = calcRegion(int2(0, lower), int2(upper, 0), samples, i.texcoord);
					region regionC = calcRegion(int2(lower, 0), int2(0, upper), samples, i.texcoord);
					region regionD = calcRegion(int2(0, 0), int2(upper, upper), samples, i.texcoord);


					// REGION COMPARISON
					// Define default comparison values
					float3 col = regionA.mean;
					float minVar = regionA.variance;
					float testVal = 0;

					// Test region B
					testVal = step(regionB.variance, minVar);
					col = lerp(col, regionB.mean, testVal);
					minVar = lerp(minVar, regionB.variance, testVal);

					// Test region C
					testVal = step(regionC.variance, minVar);
					col = lerp(col, regionC.mean, testVal);
					minVar = lerp(minVar, regionB.variance, testVal);

					// Test region D
					testVal = step(regionD.variance, minVar);
					col = lerp(col, regionD.mean, testVal);



					//float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
					//float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
					//color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
					//return color;

					return float4(col, 1.0);
				}
			ENDHLSL
		}
	}
}