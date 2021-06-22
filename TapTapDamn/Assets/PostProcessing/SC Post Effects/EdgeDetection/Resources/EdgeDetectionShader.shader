Shader "Hidden/SC Post Effects/Edge Detect" { 

	HLSLINCLUDE

	#include "PostProcessing/Shaders/StdLib.hlsl"
	//#include "../../../Shaders/StdLib.hlsl"

	#pragma fragmentoption ARB_precision_hint_fastest 

	//Screen color
	TEXTURE2D_SAMPLER2D (_MainTex, sampler_MainTex);
	//Unused but required by UnityStereoScreenSpaceUVAdjust
	float4 _MainTex_ST;
	uniform float4 _MainTex_TexelSize;

	//Camera depth textures
	TEXTURE2D_SAMPLER2D (_CameraDepthTexture, sampler_CameraDepthTexture);
	TEXTURE2D_SAMPLER2D (_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture);

	//Parameters
	uniform half4 _Sensitivity; 
	uniform half4 _BgColor;
	uniform half _BgFade;
	uniform half _SampleDistance;
	uniform float _Exponent;
	uniform float _Threshold;
	uniform float4 _EdgeColor;

	//Structs
	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord[5] : TEXCOORD0;
	};
	
	struct v2fd {
		float4 vertex : POSITION;
		float2 texcoord[2] : TEXCOORD0;
	};

	struct v2flum {
		float4 vertex : POSITION;
		float2 texcoord[3] : TEXCOORD0;
	};

	inline float DecodeFloatRG(float2 enc)
	{
		float2 kDecodeDot = float2(1.0, 1 / 255.0);
		return dot(enc, kDecodeDot);
	}

	inline half CheckSame(half2 centerNormal, float centerDepth, half4 theSample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		half2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		half isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG(theSample.zw);
		float zdiff = abs(centerDepth - sampleDepth);
		// scale the required threshold by the distance
		half isSameDepth = zdiff * _Sensitivity.x < 0.09 * centerDepth;

		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise

		return isSameNormal * isSameDepth;
	}

	//TRIANGLE DEPTH NORMALS METHOD

	v2f vertThin(VaryingsDefault v)
	{
		v2f o;
		o.vertex = float4(v.vertex.xy, 0, 1);

		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.texcoord[0] = uv;

		//TODO: Create seperate struct with smaller array
		o.texcoord[3] = uv;
		o.texcoord[4] = uv;

		// offsets for two additional samples
		o.texcoord[1] = UnityStereoScreenSpaceUVAdjust(uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);
		o.texcoord[2] = UnityStereoScreenSpaceUVAdjust(uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);

		return o;
	}

	half4 fragThin(v2f i) : SV_Target
	{
		half4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);

		half4 center = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[0].xy);
		half4 sample1 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[1].xy);
		half4 sample2 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[2].xy);

		// encoded normal
		half2 centerNormal = center.xy;
		// decoded depth
		float centerDepth = DecodeFloatRG(center.zw);

		half edge = 1;

		edge *= CheckSame(centerNormal, centerDepth, sample1);
		edge *= CheckSame(centerNormal, centerDepth, sample2);

		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BgFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeColor.a);

		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);

	}

	//ROBERTS CROSS DEPTH NORMALs METHOD

	v2f vertRobert(VaryingsDefault v )
	{
		v2f o;
		o.vertex = float4(v.vertex.xy, 0, 1);
		
		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);


#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.texcoord[0] = uv;
			
		// calc coord for the X pattern
		// maybe nicer TODO for the future: 'rotated triangles'
		
		o.texcoord[1] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,1) * _SampleDistance, _MainTex_ST);
		o.texcoord[2] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,-1) * _SampleDistance, _MainTex_ST);
		o.texcoord[3] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,1) * _SampleDistance, _MainTex_ST);
		o.texcoord[4] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,-1) * _SampleDistance, _MainTex_ST);
				 
		return o;
	} 

	half4 fragRobert(v2f i) : SV_Target
	{
		half4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);

		half4 sample1 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[1].xy);
		half4 sample2 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[2].xy);
		half4 sample3 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[3].xy);
		half4 sample4 = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord[4].xy);

		half edge = 1.0;

		edge *= CheckSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= CheckSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BgFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeColor.a);

		//return original;
		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}
	
	//SOBEL DEPTH METHOD
	 
	v2fd vertD(VaryingsDefault v )
	{
		v2fd o;
		o.vertex = float4(v.vertex.xy, 0, 1);
		
		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);
		
#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.texcoord[0] = uv;
		o.texcoord[1] = uv;
		
		return o;
	}

	float4 fragDCheap(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"
		half4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist)); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(-1,1))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(-1,1))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist)); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(0,1))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(1,0))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(1,0))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(0,1))); // B

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);

		float edge = 1 - Sobel;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BgFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeColor.a);

		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	// pretty much also just a sobel filter, except for that edges "outside" the silhouette get discarded
	//  which makes it compatible with other depth based post fx

	float4 fragD(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"
		half4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist)); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(-1,1))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(-1,1))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist)); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(0,1))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(1,0))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]+uvDist*float2(1,0))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord[1]-uvDist*float2(0,1))); // B

		// make it work nicely with depth based image effects such as depth of field:
		depthsDiag = (depthsDiag > centerDepth.xxxx) ? depthsDiag : centerDepth.xxxx;
		depthsAxis = (depthsAxis > centerDepth.xxxx) ? depthsAxis : centerDepth.xxxx;

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);

		float edge = 1 - Sobel;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BgFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeColor.a);

		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	//TRIANGLE LUMINANCE VARIANCE METHOD

	v2flum vertLum(VaryingsDefault v)
	{
		v2flum o;
		o.vertex = float4(v.vertex.xy, 0, 1);
		float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

#if UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
		//UNITY_SINGLE_PASS_STEREO
		uv = TransformStereoScreenSpaceTex(uv, 1.0);

		o.texcoord[0] = uv;
		o.texcoord[1] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		o.texcoord[2] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;

		return o;
	}

	float4 fragLum(v2flum i) : SV_Target
	{
		float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[0]);

		// a very simple cross gradient filter

		half3 p1 = original.rgb;
		half3 p2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[1]).rgb;
		half3 p3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord[2]).rgb;

		half3 diff = p1 * 2 - p2 - p3;
		half edge = dot(diff, diff);
		edge = step(edge, _Threshold);
		//if(len >= _Threshold)
		//	original.rgb = 0;

		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BgFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeColor.a);

		//return original;
		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	
		ENDHLSL
	
		//Pass determined by EdgeDetectionMode enum value
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

	  HLSLPROGRAM
      #pragma vertex vertThin
      #pragma fragment fragThin
	 ENDHLSL
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

	  HLSLPROGRAM
      #pragma vertex vertRobert
      #pragma fragment fragRobert
	 ENDHLSL
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

	  HLSLPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragDCheap
	 ENDHLSL
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

	  HLSLPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragD
	 ENDHLSL
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

	  HLSLPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertLum
      #pragma fragment fragLum
	 ENDHLSL
  }
}

Fallback off
	
} // shader
