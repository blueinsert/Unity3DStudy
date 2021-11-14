using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtility {

	public static Color HSVToRGB(float h, float s, float v)
	{
		float r = 0, g = 0, b = 0;

		float Chroma = s * v;
		float Hdash = h / 60.0f;
		float X = Chroma * (1.0f - System.Math.Abs((Hdash % 2.0f) - 1.0f));

		if (Hdash < 1.0f)
		{
			r = Chroma;
			g = X;
		}
		else if (Hdash < 2.0f)
		{
			r = X;
			g = Chroma;
		}
		else if (Hdash < 3.0f)
		{
			g = Chroma;
			b = X;
		}
		else if (Hdash < 4.0f)
		{
			g = X;
			b = Chroma;
		}
		else if (Hdash < 5.0f)
		{
			r = X;
			b = Chroma;
		}
		else if (Hdash < 6.0f)
		{
			r = Chroma;
			b = X;
		}

		float Min = v - Chroma;

		r += Min;
		g += Min;
		b += Min;

		return new Color(r, g, b);
	}
}
