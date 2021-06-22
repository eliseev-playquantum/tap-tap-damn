Shader "Hidden/SC Post Effects/ScreenGradient"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"
	#include "../../Blending.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_Gradient, sampler_Gradient);
	float _Intensity;

	float4 Frag(VaryingsDefault i): SV_Target
	{
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);

		float2 gradientPos = float2((i.texcoordStereo.y*0.99)+0.01, 0);
		float4 gradient = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, gradientPos);

		float3 blendedColor = lerp(screenColor.rgb, gradient.rgb, gradient.a * _Intensity);
		return float4(blendedColor.rgb, screenColor.a);
	}

	float4 FragOverlay(VaryingsDefault i): SV_Target
	{
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
		float4 gradient = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, float2(i.texcoordStereo.y, 0));

		float3 blendedColor = BlendLighten(screenColor.rgb, gradient.rgb);

		float3 color = lerp(screenColor.rgb, blendedColor.rgb, gradient.a);
		return float4(color.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		//Normal
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			ENDHLSL
		}
		//Overlay
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment FragOverlay

			ENDHLSL
		}
	}
}