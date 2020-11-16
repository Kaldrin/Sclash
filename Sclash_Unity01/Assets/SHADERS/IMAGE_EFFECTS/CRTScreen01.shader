// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/CRTScreen01"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorBleed("Color bleeding amount", Int) = 4
		_Brightness ("Brightness", Float) = 0
		_Contrast("Contrast", Float) = 0
	}


	CGINCLUDE
		#include "UnityCG.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		sampler2D _MainTex;
		float4 _MainTex_ST;
		int _ColorBleed;
		float _Brightness;
		float _Contrast;

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			float4 screenPos : TEXCOORD1;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.screenPos = ComputeScreenPos(o.vertex);
			o.uv = v.texcoord;
			return o;
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
			Name "CRTPass"


            CGPROGRAM
				float4 frag(v2f i) : SV_TARGET
				{
					float4 tex = tex2D(_MainTex, i.uv);
					float2 sp = i.screenPos.xy * _ScreenParams.xy;

					float3 r = float3(tex.r, tex.g / _ColorBleed, tex.b / _ColorBleed);
					float3 g = float3(tex.r / _ColorBleed, tex.g, tex.b / _ColorBleed);
					float3 b = float3(tex.r / _ColorBleed, tex.g / _ColorBleed, tex.b);

					float3x3 colorMap = float3x3(r, g, b);

					float3 wh = 1.0;
					float3 bl = 0.0;

					float3x3 scanlineMap = float3x3(wh, wh, bl);

					float3 returnVal = colorMap[(int)sp.x % 3] * scanlineMap[(int)sp.y % 3];

					returnVal += (_Brightness / 255);
					returnVal = saturate(returnVal);
					returnVal = returnVal - _Contrast * (returnVal - 1.0) * returnVal * (returnVal - 0.5);
					
					return float4(returnVal, 1.0);
				}
            ENDCG
        }
    }
}
