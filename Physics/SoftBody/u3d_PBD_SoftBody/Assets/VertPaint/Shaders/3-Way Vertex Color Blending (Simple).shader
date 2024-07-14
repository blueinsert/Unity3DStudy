Shader "VertPaint/3-Way Vertex Color Blending (Simple)"
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

		_Color3("Color 3", Color) = (1, 1, 1, 1)
		_Albedo3("Albedo 3 (RGB)", 2D) = "white" {}
		_NormalMap3("Normal map 3", 2D) = "white" {}
		_MSHAO3("Metallic (R) Smoothness (G) Height (B) AO (A) 3", 2D) = "black" {}
		_Smoothness3("Smoothness 3", Range(0, 2)) = 0.5
		_NormalMap3Strength("Normal Map 3 Strength", Range(0, 5)) = 1
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
		float4 _Color3;

		sampler2D _Albedo1;
		sampler2D _Albedo2;
		sampler2D _Albedo3;
		sampler2D _NormalMap1;
		sampler2D _NormalMap2;
		sampler2D _NormalMap3;
		sampler2D _MSHAO1;
		sampler2D _MSHAO2;
		sampler2D _MSHAO3;

		struct Input
		{
			float2 uv_Albedo1;
			float2 uv_Albedo2;
			float2 uv_Albedo3;
			float2 uv_NormalMap1;
			float2 uv_NormalMap2;
			float2 uv_NormalMap3;
			float2 uv_MSHAO1;
			float2 uv_MSHAO2;
			float2 uv_MSHAO3;

			float4 vertColor : COLOR;
		};

		float _Smoothness1;
		float _Smoothness2;
		float _Smoothness3;
		float _NormalMap1Strength;
		float _NormalMap2Strength;
		float _NormalMap3Strength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float3 albedo1 = tex2D(_Albedo1, IN.uv_Albedo1) * _Color1;
			float3 albedo2 = tex2D(_Albedo2, IN.uv_Albedo2) * _Color2;
			float3 albedo3 = tex2D(_Albedo3, IN.uv_Albedo3) * _Color3;

			o.Albedo = lerp(lerp(lerp(albedo1, albedo3, IN.vertColor.g), albedo2, IN.vertColor.r), albedo1, IN.vertColor.b).rgb;

			float4 defaultNormal = normalize(float4(1, 1, 1, 1));

			float4 normal1 = lerp(defaultNormal, tex2D(_NormalMap1, IN.uv_NormalMap1), _NormalMap1Strength);
			float4 normal2 = lerp(defaultNormal, tex2D(_NormalMap2, IN.uv_NormalMap2), _NormalMap2Strength);
			float4 normal3 = lerp(defaultNormal, tex2D(_NormalMap3, IN.uv_NormalMap3), _NormalMap3Strength);

			o.Normal = UnpackNormal(lerp(lerp(lerp(normal1, normal3, IN.vertColor.g), normal2, IN.vertColor.r), normal1, IN.vertColor.b));

			float4 mshao1 = tex2D(_MSHAO1, IN.uv_MSHAO1);
			float4 mshao2 = tex2D(_MSHAO2, IN.uv_MSHAO2);
			float4 mshao3 = tex2D(_MSHAO3, IN.uv_MSHAO3);
						
			o.Metallic = lerp(lerp(lerp(mshao1.r, mshao3.r, IN.vertColor.g), mshao2.r, IN.vertColor.r), mshao1.r, IN.vertColor.b);

			float smoothness1 = clamp(mshao1.g * _Smoothness1, 0, 1);
			float smoothness2 = clamp(mshao2.g * _Smoothness2, 0, 1);
			float smoothness3 = clamp(mshao3.g * _Smoothness3, 0, 1);

			o.Smoothness = lerp(lerp(lerp(smoothness1, smoothness3, IN.vertColor.g), smoothness2, IN.vertColor.r), smoothness1, IN.vertColor.b);
			
			o.Occlusion = lerp(lerp(lerp(mshao1.a, mshao3.a, IN.vertColor.g), mshao2.a, IN.vertColor.r), mshao1.a, IN.vertColor.b);
		}

		ENDCG
	}

	FallBack "Diffuse"
	CustomEditor "VertexColorBlendingEditor_3Way_Simple"
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com