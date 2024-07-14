Shader "VertPaint/2-Way Vertex Color Blending (Height Based Smooth)" 
{
	Properties 
	{
		_Color1("Color 1", Color) = (1, 1, 1, 1)
		_Albedo1("Albedo 1 (RGB)", 2D) = "white" {}
		_NormalMap1("Normal map 1", 2D) = "white" {}
		_MSHAO1("Metallic (R) Smoothness (G) Height (B) AO (A) 1", 2D) = "black" {}
		_Smoothness1("Smoothness 1", Range(0, 2)) = 0.5
		_NormalMap1Strength("Normal Map 1 Strength", Range(0, 5)) = 1
		_Height1Shift("Height 1 Shift", Range(-3, 3)) = 0

		_Color2("Color 2", Color) = (1, 1, 1, 1)
		_Albedo2("Albedo 2 (RGB)", 2D) = "white" {}
		_NormalMap2("Normal map 2", 2D) = "white" {}
		_MSHAO2("Metallic (R) Smoothness (G) Height (B) AO (A) 2", 2D) = "black" {}
		_Smoothness2("Smoothness 2", Range(0, 2)) = 0.5
		_NormalMap2Strength("Normal Map 2 Strength", Range(0, 5)) = 1
		_Height2Shift("Height 2 Shift", Range(-3, 3)) = 0
			
		_TransitionSmoothness("Transition Smoothness", Range(0.0001, 1)) = 0.05
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		#pragma target 4.0

		#include "Assets/VertPaint/Shaders/CG include files/heightblend.cginc"

		struct Input 
		{
			float2 uv_Albedo1;
			float2 uv_Albedo2;
			float2 uv_NormalMap1;
			float2 uv_NormalMap2;
			float2 uv_MSHAO1;
			float2 uv_MSHAO2;

			float4 vertColor: COLOR;
		};

		float4 _Color1;
		float4 _Color2;

		sampler2D _Albedo1;
		sampler2D _Albedo2;
		sampler2D _NormalMap1;
		sampler2D _NormalMap2;
		sampler2D _MSHAO1;
		sampler2D _MSHAO2;

		float _Smoothness1;
		float _Smoothness2;
		float _NormalMap1Strength;
		float _NormalMap2Strength;
		float _Height1Shift;
		float _Height2Shift;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float3 albedo1 = tex2D(_Albedo1, IN.uv_Albedo1).rgb;
			float3 albedo2 = tex2D(_Albedo2, IN.uv_Albedo2).rgb;

			float4 defaultNormal = normalize(float4(1, 1, 1, 1));
			float3 normal1 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap1, IN.uv_NormalMap1), _NormalMap1Strength));
			float3 normal2 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap2, IN.uv_NormalMap2), _NormalMap2Strength));

			float4 mshao1 = tex2D(_MSHAO1, IN.uv_MSHAO1);
			float4 mshao2 = tex2D(_MSHAO2, IN.uv_MSHAO2);

			float height1 = mshao1.b + _Height1Shift;
			float height2 = (mshao2.b + _Height2Shift) * IN.vertColor.r;

			o.Albedo = heightblend(albedo1, height1, albedo2, height2);

			o.Normal = heightblend(normal1, height1, normal2, height2);
			
			o.Metallic = heightblend(mshao1.r, height1, mshao2.r, height2);
			
			o.Smoothness = clamp(heightblend(mshao1.g * _Smoothness1, height1, mshao2.g * _Smoothness2, height2), 0, 1);
			
			o.Occlusion = heightblend(mshao1.a, height1, mshao2.a, height2);
		}

		ENDCG
	}
	
	FallBack "Glitched Polygons/2-Way Vertex Color Blending (Height Based)"
	FallBack "Diffuse"

	CustomEditor "VertexColorBlendingEditor_2Way_HeightBasedSmooth"
}

// Copyright (C) Glitched Polygons | Raphael Beck & Maksym Yakymchuk, 2017