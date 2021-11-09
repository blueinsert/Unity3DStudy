using UnityEngine;
using System.Collections.Generic;
using System;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Various vector math utility functions.
	/// Version: A lot of functions in the Polygon class have been moved to this class
	/// the names have changed slightly and everything now consistently assumes a left handed
	/// coordinate system now instead of sometimes using a left handed one and sometimes
	/// using a right handed one. This is why the 'Left' methods in the Polygon class redirect
	/// to methods named 'Right'. The functionality is exactly the same.
	///
	/// Note the difference between segments and lines. Lines are infinitely
	/// long but segments have only a finite length.
	///
	/// \ingroup utils
	/// </summary>
	public static class Math {

		/// <summary>
		/// Returns the closest point on the segment.
		/// The segment is NOT treated as infinite.
		/// See: ClosestPointOnLine
		/// See: ClosestPointOnSegmentXZ
		/// </summary>
		public static Vector3 ClosestPointOnSegment (Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
			var dir = lineEnd - lineStart;
			float sqrMagn = dir.sqrMagnitude;

			if (sqrMagn <= 0.000001) return lineStart;

			float factor = Vector3.Dot(point - lineStart, dir) / sqrMagn;
			return lineStart + Mathf.Clamp01(factor)*dir;
		}


		/// <summary>
		/// Normalize vector and also return the magnitude.
		/// This is more efficient than calculating the magnitude and normalizing separately
		/// </summary>
		public static Vector3 Normalize(Vector3 v, out float magnitude)
		{
			magnitude = v.magnitude;
			// This is the same constant that Unity uses
			if (magnitude > 1E-05f)
			{
				return v / magnitude;
			}
			else
			{
				return Vector3.zero;
			}
		}

		/// <summary>
		/// Complex number multiplication.
		/// Returns: a * b
		///
		/// Used to rotate vectors in an efficient way.
		///
		/// See: https://en.wikipedia.org/wiki/Complex_number<see cref="Multiplication_and_division"/>
		/// </summary>
		public static Vector2 ComplexMultiply(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
		}

		/// <summary>
		/// Converts an HSV color to an RGB color.
		/// According to the algorithm described at http://en.wikipedia.org/wiki/HSL_and_HSV
		///
		/// @author Wikipedia
		/// @return the RGB representation of the color.
		/// </summary>
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
}
