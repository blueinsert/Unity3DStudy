Shader "SRPStudy/BaseLit"
{
	Properties
	{
		_Color("Color Tint", Color) = (0.5,0.5,0.5)
		[NoScaleOffset]_MainTex("MainTex",2D) = "white"{}
        _SpecularPow("SpecularPow",range(5,50)) = 20
	}

	HLSLINCLUDE

	#include "UnityCG.cginc"

	//这里定义了颜色和基本贴图。这边没有定义贴图的缩放偏移。
	fixed4 _Color;
	sampler2D _MainTex;
	//这边需要平行光参数用于计算
	half4 _DLightDir;
	fixed4 _DLightColor;
    half4 _CameraPos;
    fixed _SpecularPow;

	struct a2v
	{
		float4 position : POSITION;
		float2 uv : TEXCOORD0;
		//需要模型法线进行计算光照
		float3 normal : NORMAL; 
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
		//法线传入像素管线计算像素光照
		float3 normal : NORMAL;
        float3 worldPos : TEXCOORD1;
	};

	v2f vert(a2v v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		o.uv = v.uv;
		//这边仍然使用的Unity的内置矩阵。目前来看还可以用。如果以后不行了，可以考虑
		//管线自己往里面传矩阵。第一个是MVP矩阵。管线里面传camera.projectMatrix*
		//camera.worldToCameraMatrix之后，再传入一个物体的localToWorldMatrix即可。
		//法线转换可以根据是否统一缩放来进行转换，可以参考目前unitycg.cginc中相关
		//代码，传入M矩阵即可。
		o.position = UnityObjectToClipPos(v.position);		
		o.normal = UnityObjectToWorldNormal(v.normal);
        o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 fragColor = half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv);
		//获得光照参数，进行兰伯特光照计算
		half light = saturate(dot(i.normal, _DLightDir));

        half3 viewDir = normalize(_CameraPos - i.worldPos);
        half3 halfDir = normalize(viewDir + _DLightDir.xyz);
        fixed specular = pow(saturate(dot(i.normal, halfDir)), _SpecularPow);

        fragColor.rgb *= light * (1 + specular) * _DLightColor;
		return fragColor;
	}

		ENDHLSL

		SubShader
	{
		Tags{ "Queue" = "Geometry" }
			LOD 100
			Pass
		{
			//这边注意一下，Unity的默认光照模型中，是没有BaseLit这个类型的。在过去的默认管线中这么
			//写肯定不行，因为默认管线中的光照必须判断相关的宏定义来获取相关参数。但这边我们自己写
			//SRP，所以，这边其实可以随便起名字的。上一节的Unlit也是，默认应该是Always
			Tags{ "LightMode" = "BaseLit" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
	}
}