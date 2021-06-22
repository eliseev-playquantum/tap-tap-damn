Shader "Hidden/Custom/Fisheye"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Strength;

	float2 FisheyeUV(half2 uv, half amount, half zoom)
	{
		half2 center = uv.xy - half2(0.5, 0.5);
		half CdotC = dot(center, center);
		half f = 1.0 + CdotC * (amount * sqrt(CdotC));
		return f * zoom * center + 0.5;
	}


	float4 Frag(VaryingsDefault i): SV_Target
	{
		float2 uv = FisheyeUV(i.texcoordStereo, _Strength , lerp(1, 0.9, _Strength));

		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

		return float4(screenColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			ENDHLSL
		}
	}
}