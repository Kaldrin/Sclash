// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/Bloom01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_KernelSize("Kernel Size (N)", Int) = 3
		_Spread("St. dev. (sigma)", Float) = 5.0
		_Treshold("Bloom treshold", Range(0, 1)) = 0.5
		_SrcTex("Source camera texture", 2D) = "" {}
	}


	CGINCLUDE
		#include "UnityCG.cginc"
		#pragma vertex vert_img
		#pragma fragment frag
		

		sampler2D _CameraDepthTexture;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float2 _MainTex_TexelSize;
		int	_KernelSize;
		float _Spread;
		float _Treshold;
		sampler2D _SrcTex;

		static const float TWO_PI = 6.28319;
		static const float E = 2.71828;


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
        Tags { "RenderType" = "Opaque" }


        Pass
        {
			Name "ThresholdPass"


            CGPROGRAM
				float4 frag(v2f_img i) : SV_TARGET
				{
					float4 tex = tex2D(_MainTex, i.uv);
					float brightness = rgb2hsv(tex).y;
					return (brightness > _Treshold) ? tex : float4(0.0, 0.0, 0.0, 1.0);

					
					return tex;
				}
            ENDCG
        }

		UsePass "ImageEffect/GaussianBlur01/SINGLEBLURPASS"
			
		
		Pass
		{
			Name "AddPass"

			
			CGPROGRAM
				float4 frag(v2f_img i) : SV_TARGET
				{			
					float3 originalTex = tex2D(_SrcTex, i.uv);
					float3 blurredTex = tex2D(_MainTex, i.uv);

					return float4(originalTex + blurredTex, 1.0);
				}
			ENDCG
		}		
    }
}
