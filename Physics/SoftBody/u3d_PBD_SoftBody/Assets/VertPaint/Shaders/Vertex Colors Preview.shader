Shader "VertPaint/Unlit Vertex Colors" {
	SubShader {
		Tags { "RenderType" = "Opaque+100000" }
		LOD 50

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vertInput {
				fixed4 vertColor : COLOR;
				fixed4 vertPosition : POSITION;
			};

			struct vertOutput {
				fixed4 vertColor : COLOR;
				fixed4 vertPosition : SV_POSITION;
			};

			vertOutput vert(vertInput v) {
				vertOutput o;
				o.vertColor = v.vertColor;
				o.vertPosition = UnityObjectToClipPos(v.vertPosition);
				return o;
			}

			fixed4 frag(vertOutput v) : COLOR {
				return v.vertColor;
			}
			ENDCG
		}
	}
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com