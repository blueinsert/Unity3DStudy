// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI_FakeDepth"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

       _DepthLeft("DepthL", Float) = 0
	   _DepthRight("DepthR", Float) = 0
	   _DepthTop("DepthTop", Float) = 0
	   _DepthBottom("DepthBottom", Float) = 0
	   _HorizonDegree("HorizonDegree", Float)=0
	  [Toggle] _DebugDepth("DebugDepth", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite On
		ZTest[unity_GUIZTestMode]
		Blend One OneMinusSrcAlpha
		ColorMask[_ColorMask]

		CGINCLUDE

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			half2 texcoord  : TEXCOORD0;
			float4 worldPosition : TEXCOORD1;
		};

	    fixed4 _Color;
		fixed4 _TextureSampleAdd;
		float4 _ClipRect;
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		float _DepthLeft;
		float _DepthRight;
		float _DepthTop;
		float _DepthBottom;
		float _HorizonDegree;
		float _DebugDepth;

	    v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.worldPosition = IN.vertex;
			OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
			float depthH = (IN.texcoord.x)*(_DepthRight - _DepthLeft) + _DepthLeft;
			float depthV = (IN.texcoord.y)*(_DepthTop - _DepthBottom) + _DepthBottom;
			OUT.vertex.z = lerp(depthV, depthH, _HorizonDegree);

			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color * _Color;
			return OUT;
		}

		ENDCG

		Pass
		{
			Name "Normal"
			CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag

	        #include "UnityCG.cginc"
	        #include "UnityUI.cginc"

	        #pragma multi_compile __ UNITY_UI_ALPHACLIP
			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord);
				//IN.vertex.z = min(color.a, IN.vertex.z);
				color.rgb *= color.a;
				color = (color + _TextureSampleAdd) * IN.color;
				half z = IN.vertex.z;
				color = lerp(color, half4(z, z, z, 1), _DebugDepth);
				color.rgba *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                #ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
                #endif

			    return color;
			}
			ENDCG
		}//end pass normal

	}//end subshader
}
