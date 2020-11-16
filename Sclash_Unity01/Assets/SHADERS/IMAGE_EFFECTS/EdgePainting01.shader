// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/EdgesPainting01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_KernelSize("Kernel Size (N)", Int) = 7
		_RandomValue("Random value", Float) = 0.5
		_PowX("x Pow", Int) = 2
		_PowY("Y Pow", Int) = 2
		_Spread("St. dev. (sigma)", Float) = 5.0
	}


	CGINCLUDE
		#include "UnityCG.cginc"
		#pragma vertex vert_img
		#pragma fragment frag


		sampler2D _MainTex;
		float2 _MainTex_TexelSize;
		int	_KernelSize;
		float _RandomValue;
		int _PowX;
		int _PowY;
		float _Spread;

		static const float TWO_PI = 6.28319;
		static const float E = 2.71828;


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
					float3 tex = tex2D(_MainTex, uv + offset);

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
	ENDCG


    SubShader
    {
		Cull Off
		ZWrite Off
		ZTest Always

		Cull Off ZWrite Off ZTest Always


        Tags
		{
			"RenderType" = "Opaque"
		}


        Pass
        {
			Name "PaintingPass"


            CGPROGRAM
				float4 frag(v2f_img i) : SV_TARGET
				{
					float3 tex = tex2D(_MainTex, i.uv);

					int tempKernelSize = getKernelSize(i.uv) * _Spread;

					// Kernel limits
					int upper = ((tempKernelSize) - 1) / 2;
					int lower = -upper;


					// Kernel sample size (x * y)
					int samples = (upper + 1) * (upper + 1);



					



					// Calculate kuwahara's 4 regions
					region regionA = calcRegion(int2(lower, lower), int2(0, 0), samples, i.uv);
					region regionB = calcRegion(int2(0, lower), int2(upper, 0), samples, i.uv);
					region regionC = calcRegion(int2(lower, 0), int2(0, upper), samples, i.uv);
					region regionD = calcRegion(int2(0, 0), int2(upper, upper), samples, i.uv);



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




					return float4(col, 1.0);
				}
            ENDCG
        }
    }
}
