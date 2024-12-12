
using System;

namespace Dest.Math
{
	public static class Quaternionex
	{
		/// <summary>
		/// Calculates difference from this quaternion to given target quaternion. I.e. if you have quaternions Q1 and Q2,
		/// this method will return quaternion Q such that Q2 == Q * Q1 (remember that quaternions are multiplied right-to-left).
		/// </summary>
		public static Quaternion DeltaTo(this Quaternion quat, Quaternion target)
		{
			return target * Quaternion.Inverse(quat);
		}

		/// <summary>
		/// Returns string representation (does not round components as standard Quaternion.ToString() does)
		/// </summary>
		public static string ToStringEx(this Quaternion quat)
		{
			return string.Format("[{0}, {1}, {2}, {3}]", quat.x.ToString(), quat.y.ToString(), quat.z.ToString(), quat.w.ToString());
		}


        /// <summary>
        /// 返回Rotation的double欧拉角表示数据
        /// </summary>
        /// <param name="rotationInt"></param>
        /// <returns></returns>
        public static double GetRotationDouble(UInt32 rotationInt)
        {
            return (rotationInt * 360) / UInt16.MaxValue;
        }

        /// <summary>
        /// 返回用UInt16编码的Rotation数据
        /// </summary>
        /// <param name="rotationValue"></param>
        /// <returns></returns>
        public static UInt16 GetRotationUInt16(double rotationValue)
        {
            return (UInt16)(rotationValue * UInt16.MaxValue / (360));
        }
	}
}
