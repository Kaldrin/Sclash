Shader "Custom/TestShader"
{
    Properties
    {
		_color ("Color", Color) = (1, 1, 1, 1);
    }


    SubShader
    {
		Pass
		{
			
			Lighting Off
			SetTexture [_color]  {}
		}
    }


    FallBack "Diffuse"
}
