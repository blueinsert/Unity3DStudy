Shader "VertPaint/4-Way Vertex Color Blending (Height Based)"
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

		_Color4("Color 4", Color) = (1, 1, 1, 1)
		_Albedo4("Albedo 4 (RGB)", 2D) = "white" {}
		_NormalMap4("Normal map 4", 2D) = "white" {}
		_MSHAO4("Metallic (R) Smoothness (G) Height (B) AO (A) 4", 2D) = "black" {}
		_Smoothness4("Smoothness 4", Range(0, 2)) = 0.5
		_NormalMap4Strength("Normal Map 4 Strength", Range(0, 5)) = 1

		_Blend1("Blend 1", Range(-3, 1)) = 0.5
		_Blend2("Blend 2", Range(-3, 1)) = 0.5
		_Blend3("Blend 3", Range(-3, 1)) = 0.5
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
		float4 _Color4;

		sampler2D _Albedo1;
		sampler2D _Albedo2;
		sampler2D _Albedo3;
		sampler2D _Albedo4;
		sampler2D _NormalMap1;
		sampler2D _NormalMap2;
		sampler2D _NormalMap3;
		sampler2D _NormalMap4;
		sampler2D _MSHAO1;
		sampler2D _MSHAO2;
		sampler2D _MSHAO3;
		sampler2D _MSHAO4;

		struct Input
		{
			float2 uv_Albedo1;
			float2 uv_Albedo2;
			float2 uv_Albedo3;
			float2 uv_Albedo4;
			float2 uv_NormalMap1;
			float2 uv_NormalMap2;
			float2 uv_NormalMap3;
			float2 uv_NormalMap4;
			float2 uv_MSHAO1;
			float2 uv_MSHAO2;
			float2 uv_MSHAO3;
			float2 uv_MSHAO4;

			float4 vertColor : COLOR;
		};

		float _Smoothness1;
		float _Smoothness2;
		float _Smoothness3;
		float _Smoothness4;
		float _NormalMap1Strength;
		float _NormalMap2Strength;
		float _NormalMap3Strength;
		float _NormalMap4Strength;

		float _Blend1;
		float _Blend2;
		float _Blend3;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 mshao1 = tex2D(_MSHAO1, IN.uv_MSHAO1);
			float4 mshao2 = tex2D(_MSHAO2, IN.uv_MSHAO2);
			float4 mshao3 = tex2D(_MSHAO3, IN.uv_MSHAO3);
			float4 mshao4 = tex2D(_MSHAO4, IN.uv_MSHAO4);
								
			float3 albedo1 = tex2D(_Albedo1, IN.uv_Albedo1) * _Color1;
			float3 albedo2 = tex2D(_Albedo2, IN.uv_Albedo2) * _Color2;
			float3 albedo3 = tex2D(_Albedo3, IN.uv_Albedo3) * _Color3;
			float3 albedo4 = tex2D(_Albedo4, IN.uv_Albedo4) * _Color4;

			float4 defaultNormal = normalize(float4(1, 1, 1, 1));

			float3 normal1 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap1, IN.uv_NormalMap1), _NormalMap1Strength));
			float3 normal2 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap2, IN.uv_NormalMap2), _NormalMap2Strength));
			float3 normal3 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap3, IN.uv_NormalMap3), _NormalMap3Strength));
			float3 normal4 = UnpackNormal(lerp(defaultNormal, tex2D(_NormalMap4, IN.uv_NormalMap4), _NormalMap4Strength));

			bool layer1 = (mshao1.b - IN.vertColor.r) - _Blend1 > (mshao2.b + IN.vertColor.r);

			float3 tempAlbedo1 = layer1 ? albedo1 : albedo2;
			float3 tempNormal1 = layer1 ? normal1 : normal2;
			float tempMetallic1 = layer1 ? mshao1.r : mshao2.r;
			float tempSmoothness1 = layer1 ? mshao1.g * _Smoothness1 : mshao2.g * _Smoothness2;
			float tempAO1 = layer1 ? mshao1.a : mshao2.a;

			float tempHeight1 = lerp(mshao1.b, mshao2.b, IN.vertColor.r); 

			bool layer2 = (tempHeight1 - IN.vertColor.g) - _Blend2 > (mshao3.b + IN.vertColor.g);

			float3 tempAlbedo2 = layer2 ? tempAlbedo1 : albedo3;
			float3 tempNormal2 = layer2 ? tempNormal1 : normal3;
			float tempMetallic2 = layer2 ? tempMetallic1 : mshao3.r;
			float tempSmoothness2 = layer2 ? tempSmoothness1 : mshao3.g * _Smoothness3;
			float tempAO2 = layer2 ? tempAO1 : mshao3.a;

			float tempHeight2 = lerp(tempHeight1, mshao3.b, IN.vertColor.g);

			bool layer3 = (tempHeight2 - IN.vertColor.b) - _Blend3 > (mshao4.b + IN.vertColor.b);

			o.Albedo = layer3 ? tempAlbedo2 : albedo4;
			o.Normal = layer3 ? tempNormal2 : normal4;
			o.Metallic = layer3 ? tempMetallic2 : mshao4.r;
			o.Smoothness = clamp(layer3 ? tempSmoothness2 : mshao4.g * _Smoothness4, 0, 1);
			o.Occlusion = layer3 ? tempAO2 : mshao4.a;
		}

	ENDCG

	}

	FallBack "Glitched Polygons/4-Way Vertex Color Blending (Simple)"
	FallBack "Diffuse"

	CustomEditor "VertexColorBlendingEditor_4Way_HeightBased"
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com