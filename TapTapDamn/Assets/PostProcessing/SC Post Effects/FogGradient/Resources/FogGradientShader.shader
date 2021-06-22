Shader "Hidden/SC Post Effects/Fog Gradient"
{
	HLSLINCLUDE
	
	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	TEXTURE2D_SAMPLER2D(_Gradient, sampler_Gradient);

	float _Blend;
    float _MaxDistance;

	float4 Frag(VaryingsDefault i): SV_Target
	{
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);

		//Scene depth
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, float2(i.texcoordStereo.x, i.texcoordStereo.y));

		//Orthographic camera fix
		depth = lerp(Linear01Depth(depth), LinearEyeDepth(depth), unity_OrthoParams.w);

		//TODO:
		//Discard depth values that exceed 0.99 (exclude skybox functionality)

		depth = clamp(depth / _MaxDistance, 0, 1);

		//Discard very low depth values
		depth += 0.01;

		float4 fogGradient = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, float2(depth, 0));

		float3 fogColor;
		
		//Normal
		fogColor = fogGradient.rgb;
		
		//Screen
		//fogColor = 1.0 - (1.0 - screenColor) * (1.0 - fogGradient);

		//Overlay
		//fogColor = screenColor < .5 ? 2.0 * screenColor * fogGradient : 1.0 - 2.0 * (1.0 - screenColor) * (1.0 - fogGradient);

		float3 color = lerp(screenColor.rgb, fogColor.rgb, _Blend.xxx * fogGradient.a);
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