Shader "Hidden/SC Post Effects/Double Vision"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Amount;

	//Structs
	struct v2f
	{
		float4 vertex: POSITION;
		float2 texcoord[3]: TEXCOORD0;
	};


	float4 blurDouble(TEXTURE2D_ARGS(tex, samplerTex), v2f i) {

		float4 color = float4(0, 0, 0, 0);
		color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[1]);
		color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[2]);
		return color/2;
		
	}

	v2f Vert(VaryingsDefault v)
	{
		v2f o;
		o.vertex = float4(v.vertex.xy, 0, 1);
		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

		//OpenGL
#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.texcoord[0] = uv;
		o.texcoord[1] = uv - float2(_Amount, 0);
		o.texcoord[2] = uv + float2(_Amount, 0);
		return o;
	}

	float4 Frag(v2f i): SV_Target
	{
		float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);
		float4 blurredColor = blurDouble(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i);

		screenColor = lerp(screenColor, screenColor + blurredColor, _Amount);

		return float4(blurredColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}