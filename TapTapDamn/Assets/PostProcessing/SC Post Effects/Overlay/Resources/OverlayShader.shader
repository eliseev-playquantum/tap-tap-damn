Shader "Hidden/SC Post Effects/Overlay"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_OverlayTex, sampler_OverlayTex);
	float _Intensity;

	float4 Frag(VaryingsDefault i): SV_Target
	{
		float2 uv = i.texcoordStereo;
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		float4 overlay = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, uv);

		float3 color = lerp(screenColor.rgb, overlay.rgb, overlay.a * _Intensity);

		return float4(color.rgb, screenColor.a);
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