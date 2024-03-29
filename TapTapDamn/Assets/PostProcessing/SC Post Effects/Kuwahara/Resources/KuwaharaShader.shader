﻿Shader "Hidden/SC Post Effects/Kuwahara"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	uniform float4 _MainTex_TexelSize;
	float _Radius;
	float _FadeDistance;
	float _FadeFalloff;

	TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

	struct Window {
		int x1, y1, x2, y2;
	};

	//Based on the Kuwahara method as described in GPU Pro chapter 5.1, translated from GLHSL
	inline float4 OilPaint(float2 uv, float4 inputColor) {

		//float jitter = (sin(_Time.y * 10));
		float n = float((_Radius + 1) * (_Radius + 1));

		float3 m[4];
		float3 s[4];

		int k = 0;
		UNITY_LOOP
		for (k = 0; k < 4; ++k) {
			m[k] = float3(0, 0, 0);
			s[k] = float3(0, 0, 0);
		}

		Window W[4] = {
			{ -_Radius, -_Radius,       0,       0 },
			{ 0, -_Radius, _Radius,       0 },
			{ 0,        0, _Radius, _Radius },
			{ -_Radius,        0,       0, _Radius }
		};

		UNITY_UNROLL
		for (k = 0; k < 4; ++k) {

			for (int j = W[k].y1; j <= W[k].y2; ++j) {
				for (int i = W[k].x1; i <= W[k].x2; ++i) {

					//Shader warning in 'Hidden/SC Post Effects/Oil Paint': gradient instruction used in a loop with varying iteration, attempting to unroll the loop
					float3 c = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv + float2(i * _MainTex_TexelSize.x * 1, j * _MainTex_TexelSize.y), 0).rgb;
					m[k] += c;
					s[k] += c * c ;
				}
			}
		}

		float min_sigma2 = 1e+6;
		float s2;
		UNITY_LOOP
		for (k = 0; k < 4; ++k) {
			m[k] /= n;
			s[k] = abs(s[k] / n - m[k] * m[k]);

			s2 = s[k].r + s[k].g + s[k].b;
			if (s2 < min_sigma2) {
				min_sigma2 = s2;
				inputColor.rgb = m[k].rgb;
			}
		}

		return inputColor;
	}


	float4 Frag(VaryingsDefault i): SV_Target
	{

		float2 uv = i.texcoordStereo;
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

		float4 paintColor = OilPaint(uv, screenColor);

		return paintColor;
	}

	float4 FragDepthAware(VaryingsDefault i) : SV_Target
	{

		float2 uv = i.texcoordStereo;
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

		float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv));
		depth = pow(depth,_FadeDistance * 2) * _FadeFalloff;
		depth = saturate(depth);

		//return float4(depth, depth, depth, 1);
		float4 paintColor = lerp(OilPaint(uv, screenColor), screenColor, depth);

		return paintColor;
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

			Pass
		{
			HLSLPROGRAM

		#pragma vertex VertDefault
		#pragma fragment FragDepthAware

			ENDHLSL
		}
	}
}