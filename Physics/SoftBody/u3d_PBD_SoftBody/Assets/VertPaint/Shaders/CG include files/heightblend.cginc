
//		Expose the following property to shaders that include this cginc 
//		to control the smoothness of the height transition blending:
//
//		_TransitionSmoothness("Transition Smoothness", Range(0.0001, 1)) = 0.05
//
// -------------------------------------------------------------------------------------------------------------------

float _TransitionSmoothness;


// FLOAT BLENDING ----------------------------------------------------------------------------------------------------
float heightblend(float input1, float height1, float input2, float height2)
{
	float height_start = max(height1, height2) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float heightlerp(float input1, float height1, float input2, float height2, float lerp)
{
	lerp = clamp(lerp, 0, 1);
	return heightblend(input1, height1 * (1 - lerp), input2, height2 * lerp);
}

float heightblend(float input1, float height1, float input2, float height2, float input3, float height3)
{
	float height_start = max(max(height1, height2), height3) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3)) / (b1 + b2 + b3);
}

float heightblend(float input1, float height1, float input2, float height2, float input3, float height3, float input4, float height4)
{
	float height_start = max(max(height1, height2), max(height3, height4)) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	float b4 = max(height4 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3) + (input4 * b4)) / (b1 + b2 + b3 + b4);
}
// -------------------------------------------------------------------------------------------------------------------



// FLOAT2 BLENDING ---------------------------------------------------------------------------------------------------
float2 heightblend(float2 input1, float height1, float2 input2, float height2)
{
	float height_start = max(height1, height2) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float2 heightlerp(float2 input1, float height1, float2 input2, float height2, float lerp)
{
	return heightblend(input1, height1 * (1 - lerp), input2, height2 * lerp);
}

float2 heightblend(float2 input1, float height1, float2 input2, float height2, float2 input3, float height3)
{
	float height_start = max(max(height1, height2), height3) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3)) / (b1 + b2 + b3);
}

float2 heightblend(float2 input1, float height1, float2 input2, float height2, float2 input3, float height3, float2 input4, float height4)
{
	float height_start = max(max(height1, height2), max(height3, height4)) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	float b4 = max(height4 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3) + (input4 * b4)) / (b1 + b2 + b3 + b4);
}
// -------------------------------------------------------------------------------------------------------------------



// FLOAT3 BLENDING ---------------------------------------------------------------------------------------------------
float3 heightblend(float3 input1, float height1, float3 input2, float height2)
{
	float height_start = max(height1, height2) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float3 heightlerp(float3 input1, float height1, float3 input2, float height2, float lerp)
{
	return heightblend(input1, height1 * (1 - lerp), input2, height2 * lerp);
}

float3 heightblend(float3 input1, float height1, float3 input2, float height2, float3 input3, float height3)
{
	float height_start = max(max(height1, height2), height3) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3)) / (b1 + b2 + b3);
}

float3 heightblend(float3 input1, float height1, float3 input2, float height2, float3 input3, float height3, float3 input4, float height4)
{
	float height_start = max(max(height1, height2), max(height3, height4)) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	float b4 = max(height4 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3) + (input4 * b4)) / (b1 + b2 + b3 + b4);
}
// -------------------------------------------------------------------------------------------------------------------



// FLOAT4 BLENDING ---------------------------------------------------------------------------------------------------
float4 heightblend(float4 input1, float height1, float4 input2, float height2)
{
	float height_start = max(height1, height2) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float4 heightlerp(float4 input1, float height1, float4 input2, float height2, float lerp)
{
	return heightblend(input1, height1 * (1 - lerp), input2, height2 * lerp);
}

float4 heightblend(float4 input1, float height1, float4 input2, float height2, float4 input3, float height3)
{
	float height_start = max(max(height1, height2), height3) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3)) / (b1 + b2 + b3);
}

float4 heightblend(float4 input1, float height1, float4 input2, float height2, float4 input3, float height3, float4 input4, float height4)
{
	float height_start = max(max(height1, height2), max(height3, height4)) - _TransitionSmoothness;
	float b1 = max(height1 - height_start, 0);
	float b2 = max(height2 - height_start, 0);
	float b3 = max(height3 - height_start, 0);
	float b4 = max(height4 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3) + (input4 * b4)) / (b1 + b2 + b3 + b4);
}

float4 heightblendalpha(float4 input1, float height1, float4 input2, float height2)
{
	float height_start = max(input1.a + height1, input2.a + height2) - _TransitionSmoothness;
	float b1 = max(input1.a + height1 - height_start, 0);
	float b2 = max(input2.a + height2 - height_start, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float4 heightlerpalpha(float4 input1, float height1, float4 input2, float height2, float lerp)
{
	return heightblendalpha(input1, height1 * (1 - lerp), input2, height2 * lerp);
}

float4 heightblendalpha(float4 input1, float height1, float4 input2, float height2, float4 input3, float height3)
{
	float height_start = max(max(input1.a + height1, input2.a + height2), input3.a + height3) - _TransitionSmoothness;
	float b1 = max(input1.a + height1 - height_start, 0);
	float b2 = max(input2.a + height2 - height_start, 0);
	float b3 = max(input3.a + height3 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3)) / (b1 + b2 + b3);
}

float4 heightblendalpha(float4 input1, float height1, float4 input2, float height2, float4 input3, float height3, float4 input4, float height4)
{
	float height_start = max(max(input1.a + height1, input2.a + height2), max(input3.a + height3, input4.a + height4)) - _TransitionSmoothness;
	float b1 = max(input1.a + height1 - height_start, 0);
	float b2 = max(input2.a + height2 - height_start, 0);
	float b3 = max(input3.a + height3 - height_start, 0);
	float b4 = max(input4.a + height4 - height_start, 0);
	return ((input1 * b1) + (input2 * b2) + (input3 * b3) + (input4 * b4)) / (b1 + b2 + b3 + b4);
}
// -------------------------------------------------------------------------------------------------------------------