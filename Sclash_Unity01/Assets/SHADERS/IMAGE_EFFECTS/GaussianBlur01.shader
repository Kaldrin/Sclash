// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/GaussianBlur01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_KernelSize("Kernel Size (N)", Int) = 3
		_Spread("St. dev. (sigma)", Float) = 5.0
	}


	CGINCLUDE
		#include "UnityCG.cginc"
		


		sampler2D _CameraDepthTexture;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float2 _MainTex_TexelSize;
		int	_KernelSize;
		float _Spread;

		static const float TWO_PI = 6.28319;
		static const float E = 2.71828;


		float gaussian(int x, int y)
		{
			float sigmaSqu = _Spread * _Spread;
			return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -((x * x) + (y * y)) / (2 * sigmaSqu));
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
			Name "SingleBlurPass"


            CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag_horizontal


				float4 frag_horizontal(v2f_img i) : SV_TARGET
				{
					float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
					depth = Linear01Depth(depth);

					float3 col = float3(0.0, 0.0, 0.0);
					float kernelSum = 0.0;

					int tmpKernelSize = _KernelSize;
					

					int upper = ((tmpKernelSize - 1) / 2);
					int lower = -upper;


					for (int x = lower; x <= upper; ++x)
					{
						for (int y = lower; y <= upper; ++y)
						{
							float gauss = gaussian(x, y);
							kernelSum += gauss;
							float2 offset = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
							col += gauss * tex2D(_MainTex, i.uv + offset);
						}
					}

					col = col / kernelSum;

					return float4(col, 1.0);
				}
            ENDCG
        }
    }
}
