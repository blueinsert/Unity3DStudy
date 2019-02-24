using UnityEngine;
using System.Collections;

namespace zEditorWindow
{

	public class zTool
	{

		/// <summary>
		/// 判断两矩形是否相交
		/// </summary>
		/// <returns><c>true</c> if this instance is rect intersect the specified _r1 _r2; otherwise, <c>false</c>.</returns>
		/// <param name="_r1">R1.</param>
		/// <param name="_r2">R2.</param>
		public static bool IsRectIntersect (Rect _r1, Rect _r2)
		{
			bool xPass = false;
			;
			bool yPass = false;
			if (!(_r2.xMin > _r1.xMax || _r2.xMax < _r1.xMin))
				xPass = true; 
			if (!(_r2.yMin > _r1.yMax || _r2.yMax < _r1.yMin))
				yPass = true;

			if (xPass && yPass)
				return true;
			else
				return false;
		}
	}

}