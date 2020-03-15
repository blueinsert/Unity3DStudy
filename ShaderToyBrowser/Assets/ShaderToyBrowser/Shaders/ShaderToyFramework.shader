﻿Shader "ShaderToyFramework"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		iChannel0("iChannel0", 2D) = "white" {}
	    iChannel1("iChannel0", 2D) = "white" {}
	    iChannel2("iChannel0", 2D) = "white" {}
	    iChannel3("iChannel0", 2D) = "white" {}
	    iResolution("iResolution",Vector) = (320,240,0,0)
		iMouse("iMouse",Vector) = (0,0,0,0)
		iTime("iTime",float) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry" "LightMode" = "ForwardBase" }
		Pass
	    {
		    GLSLPROGRAM

			uniform sampler2D _MainTex;
			uniform sampler2D iChannel0;
		    uniform sampler2D iChannel1;
		    uniform sampler2D iChannel2;
		    uniform sampler2D iChannel3;
		    uniform vec4 iResolution;
			uniform vec4 iMouse;
			uniform float iTime;
#ifdef VERTEX  
	        out vec4 textureCoordinates;

		    void main()
	        {
				textureCoordinates = gl_MultiTexCoord0;
		        gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	        }
#endif  

#ifdef FRAGMENT
	        in vec4 textureCoordinates;

	        void main()
	        {
				gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
				//mainImage(gl_FragColor, (textureCoordinates*iResolution).xy);
	        }
#endif   

	        ENDGLSL
	    }
	}
}