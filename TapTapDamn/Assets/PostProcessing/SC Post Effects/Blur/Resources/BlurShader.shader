Shader "Hidden/SC Post Effects/Blur"
{
	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	#include "PostProcessing/Shaders/Sampling.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"
	//#include "../../../Shaders/Sampling.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float4 _MainTex_TexelSize;

	float _Amount;
	float4 _Offsets;

	struct v2fGaussian {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;

		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};

	struct v2fBox
	{
		float4 vertex: POSITION;
		float2 uv : TEXCOORD1;
	};


	v2fGaussian VertGaussian(VaryingsDefault v) {
		v2fGaussian o;
		o.pos = float4(v.vertex.xy, 0, 1);

		o.uv.xy = TransformTriangleVertexToUV(o.pos.xy);

#if UNITY_UV_STARTS_AT_TOP
		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
		//UNITY_SINGLE_PASS_STEREO
		o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);

		o.uv01 = o.uv.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 = o.uv.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = o.uv.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1) * 6.0;

		return o;
	}

	//0
	float4 FragBlurGaussian(v2fGaussian i): SV_Target
	{
		half4 color = float4(0, 0, 0, 0);

		color += 0.40 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
		color += 0.15 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.xy);
		color += 0.15 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.zw);
		color += 0.10 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.xy);
		color += 0.10 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.zw);
		color += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.xy);
		color += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.zw);

		return color;
	}

	v2fBox VertBox(VaryingsDefault v)
	{
		v2fBox o;
		o.vertex = float4(v.vertex.xy, 0, 1);

		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.uv = uv;

		return o;
	}

	float4 FragBlurBox(v2fBox i) : SV_Target
	{
		return DownsampleBox4Tap(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i.uv, _MainTex_TexelSize.xy * _Offsets);
	}

	//Separate pass, because this shouldn't be looped
	float4 FragBlend(VaryingsDefault i) : SV_Target
	{
		return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass //0
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment FragBlend

			ENDHLSL
		}
		Pass //1
		{
			HLSLPROGRAM

			#pragma vertex VertGaussian
			#pragma fragment FragBlurGaussian

			ENDHLSL
		}
		Pass //2
		{
			HLSLPROGRAM

			#pragma vertex VertBox
			#pragma fragment FragBlurBox

			ENDHLSL
		}

	}
}