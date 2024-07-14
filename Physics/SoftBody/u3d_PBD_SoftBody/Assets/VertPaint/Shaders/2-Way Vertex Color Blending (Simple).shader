Shader "VertPaint/2-Way Vertex Color Blending (Simple)"
{
	Properties
	{
		_Color1("Color 1", Color) = (1, 1, 1, 1)
		_Albedo1("Albedo 1 (RGB)", 2D) = "white" {}
		_NormalMap1("Normal map 1", 2D) = "white" {}
		_MSHAO1("Metallic (R) Smoothness (G) Height (B) AO (A) 1", 2D) = "black" {}
		_Smoothness1("Smoothness 1", Range(0, 2)) = 0.5
		_NormalMap1Strength("Normal Map 1 Strength", Range(0, 5)) = 1

		_Color2("Color 2", Color) = (1, 1, 1, 1)
		_Albedo2("Albedo 2 (RGB)", 2D) = "white" {}
		_NormalMap2("Normal map 2", 2D) = "white" {}
		_MSHAO2("Metallic (R) Smoothness (G) Height (B) AO (A) 2", 2D) = "black" {}
		_Smoothness2("Smoothness 2", Range(0, 2)) = 0.5
		_NormalMap2Strength("Normal Map 2 Strength", Range(0, 5)) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		#pragma target 4.0

		float4 _Color1;
		float4 _Color2;

		sampler2D _Albedo1;
		sampler2D _Albedo2;
		sampler2D _NormalMap1;
		sampler2D _NormalMap2;
		sampler2D _MSHAO1;
		sampler2D _MSHAO2;

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

		float _Smoothness1;
		float _Smoothness2;
		float _NormalMap1Strength;
		float _NormalMap2Strength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 mshao1 = tex2D(_MSHAO1, IN.uv_MSHAO1);
			float4 mshao2 = tex2D(_MSHAO2, IN.uv_MSHAO2);

			o.Albedo = lerp(tex2D(_Albedo1, IN.uv_Albedo1) * _Color1, tex2D(_Albedo2, IN.uv_Albedo2) * _Color2, IN.vertColor.r).rgb;

			float4 defaultNormal = normalize(float4(1, 1, 1, 1));
			o.Normal = UnpackNormal(lerp(lerp(defaultNormal, tex2D(_NormalMap1, IN.uv_NormalMap1), _NormalMap1Strength), lerp(defaultNormal, tex2D(_NormalMap2, IN.uv_NormalMap2), _NormalMap2Strength), IN.vertColor.r));

			o.Metallic = lerp(mshao1.r, mshao2.r, IN.vertColor.r);

			o.Smoothness = clamp(lerp(mshao1.g * _Smoothness1, mshao2.g * _Smoothness2, IN.vertColor.r), 0, 1);

			o.Occlusion = lerp(mshao1.a, mshao2.a, IN.vertColor.r);
		}

		ENDCG
	}

	FallBack "Diffuse"
    CustomEditor "VertexColorBlendingEditor_2Way_Simple"
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com