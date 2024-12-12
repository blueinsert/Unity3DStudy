

namespace Dest.Math
{
	public struct Line3Box3Dist
	{
		public Vector3D ClosestPoint0;
		public Vector3D ClosestPoint1;
		public double LineParameter;
	}

	public static partial class Distance
	{
		private static void Face(ref Box3 mBox, ref double mLineParameter,
			int i0, int i1, int i2, ref Vector3D pnt, ref Vector3D dir, ref Vector3D PmE, ref double sqrDistance)
		{
			Vector3D PpE = new Vector3D();
			double lenSqr, inv, tmp, param, t, delta;

			PpE[i1] = pnt[i1] + mBox.Extents[i1];
			PpE[i2] = pnt[i2] + mBox.Extents[i2];

			if (dir[i0] * PpE[i1] >= dir[i1] * PmE[i0])
			{
				if (dir[i0] * PpE[i2] >= dir[i2] * PmE[i0])
				{
					// v[i1] >= -e[i1], v[i2] >= -e[i2] (distance = 0)
					pnt[i0] = mBox.Extents[i0];
					inv = 1f / dir[i0];
					pnt[i1] -= dir[i1] * PmE[i0] * inv;
					pnt[i2] -= dir[i2] * PmE[i0] * inv;
					mLineParameter = -PmE[i0] * inv;
				}
				else
				{
					// v[i1] >= -e[i1], v[i2] < -e[i2]
					lenSqr = dir[i0] * dir[i0] + dir[i2] * dir[i2];
					tmp = lenSqr * PpE[i1] - dir[i1] * (dir[i0] * PmE[i0] + dir[i2] * PpE[i2]);

					if (tmp <= 2f * lenSqr * mBox.Extents[i1])
					{
						t = tmp / lenSqr;
						lenSqr += dir[i1] * dir[i1];
						tmp = PpE[i1] - t;
						delta = dir[i0] * PmE[i0] + dir[i1] * tmp + dir[i2] * PpE[i2];
						param = -delta / lenSqr;
						sqrDistance += PmE[i0] * PmE[i0] + tmp * tmp + PpE[i2] * PpE[i2] + delta * param;

						mLineParameter = param;
						pnt[i0] = mBox.Extents[i0];
						pnt[i1] = t - mBox.Extents[i1];
						pnt[i2] = -mBox.Extents[i2];
					}
					else
					{
						lenSqr += dir[i1] * dir[i1];
						delta = dir[i0] * PmE[i0] + dir[i1] * PmE[i1] + dir[i2] * PpE[i2];
						param = -delta / lenSqr;
						sqrDistance += PmE[i0] * PmE[i0] + PmE[i1] * PmE[i1] + PpE[i2] * PpE[i2] + delta * param;

						mLineParameter = param;
						pnt[i0] = mBox.Extents[i0];
						pnt[i1] = mBox.Extents[i1];
						pnt[i2] = -mBox.Extents[i2];
					}
				}
			}
			else
			{
				if (dir[i0] * PpE[i2] >= dir[i2] * PmE[i0])
				{
					// v[i1] < -e[i1], v[i2] >= -e[i2]
					lenSqr = dir[i0] * dir[i0] + dir[i1] * dir[i1];
					tmp = lenSqr * PpE[i2] - dir[i2] * (dir[i0] * PmE[i0] + dir[i1] * PpE[i1]);

					if (tmp <= 2f * lenSqr * mBox.Extents[i2])
					{
						t = tmp / lenSqr;
						lenSqr += dir[i2] * dir[i2];
						tmp = PpE[i2] - t;
						delta = dir[i0] * PmE[i0] + dir[i1] * PpE[i1] + dir[i2] * tmp;
						param = -delta / lenSqr;
						sqrDistance += PmE[i0] * PmE[i0] + PpE[i1] * PpE[i1] + tmp * tmp + delta * param;

						mLineParameter = param;
						pnt[i0] = mBox.Extents[i0];
						pnt[i1] = -mBox.Extents[i1];
						pnt[i2] = t - mBox.Extents[i2];
					}
					else
					{
						lenSqr += dir[i2] * dir[i2];
						delta = dir[i0] * PmE[i0] + dir[i1] * PpE[i1] + dir[i2] * PmE[i2];
						param = -delta / lenSqr;
						sqrDistance += PmE[i0] * PmE[i0] + PpE[i1] * PpE[i1] + PmE[i2] * PmE[i2] + delta * param;

						mLineParameter = param;
						pnt[i0] = mBox.Extents[i0];
						pnt[i1] = -mBox.Extents[i1];
						pnt[i2] = mBox.Extents[i2];
					}
				}
				else
				{
					// v[i1] < -e[i1], v[i2] < -e[i2]
					lenSqr = dir[i0] * dir[i0] + dir[i2] * dir[i2];
					tmp = lenSqr * PpE[i1] - dir[i1] * (dir[i0] * PmE[i0] + dir[i2] * PpE[i2]);

					if (tmp >= 0f)
					{
						// v[i1]-edge is closest
						if (tmp <= 2f * lenSqr * mBox.Extents[i1])
						{
							t = tmp / lenSqr;
							lenSqr += dir[i1] * dir[i1];
							tmp = PpE[i1] - t;
							delta = dir[i0] * PmE[i0] + dir[i1] * tmp + dir[i2] * PpE[i2];
							param = -delta / lenSqr;
							sqrDistance += PmE[i0] * PmE[i0] + tmp * tmp + PpE[i2] * PpE[i2] + delta * param;

							mLineParameter = param;
							pnt[i0] = mBox.Extents[i0];
							pnt[i1] = t - mBox.Extents[i1];
							pnt[i2] = -mBox.Extents[i2];
						}
						else
						{
							lenSqr += dir[i1] * dir[i1];
							delta = dir[i0] * PmE[i0] + dir[i1] * PmE[i1] + dir[i2] * PpE[i2];
							param = -delta / lenSqr;
							sqrDistance += PmE[i0] * PmE[i0] + PmE[i1] * PmE[i1] + PpE[i2] * PpE[i2] + delta * param;

							mLineParameter = param;
							pnt[i0] = mBox.Extents[i0];
							pnt[i1] = mBox.Extents[i1];
							pnt[i2] = -mBox.Extents[i2];
						}
						return;
					}

					lenSqr = dir[i0] * dir[i0] + dir[i1] * dir[i1];
					tmp = lenSqr * PpE[i2] - dir[i2] * (dir[i0] * PmE[i0] + dir[i1] * PpE[i1]);

					if (tmp >= 0f)
					{
						// v[i2]-edge is closest
						if (tmp <= 2f * lenSqr * mBox.Extents[i2])
						{
							t = tmp / lenSqr;
							lenSqr += dir[i2] * dir[i2];
							tmp = PpE[i2] - t;
							delta = dir[i0] * PmE[i0] + dir[i1] * PpE[i1] + dir[i2] * tmp;
							param = -delta / lenSqr;
							sqrDistance += PmE[i0] * PmE[i0] + PpE[i1] * PpE[i1] + tmp * tmp + delta * param;

							mLineParameter = param;
							pnt[i0] = mBox.Extents[i0];
							pnt[i1] = -mBox.Extents[i1];
							pnt[i2] = t - mBox.Extents[i2];
						}
						else
						{
							lenSqr += dir[i2] * dir[i2];
							delta = dir[i0] * PmE[i0] + dir[i1] * PpE[i1] + dir[i2] * PmE[i2];
							param = -delta / lenSqr;
							sqrDistance += PmE[i0] * PmE[i0] + PpE[i1] * PpE[i1] + PmE[i2] * PmE[i2] + delta * param;

							mLineParameter = param;
							pnt[i0] = mBox.Extents[i0];
							pnt[i1] = -mBox.Extents[i1];
							pnt[i2] = mBox.Extents[i2];
						}
						return;
					}

					// (v[i1],v[i2])-corner is closest
					lenSqr += dir[i2] * dir[i2];
					delta = dir[i0] * PmE[i0] + dir[i1] * PpE[i1] + dir[i2] * PpE[i2];
					param = -delta / lenSqr;
					sqrDistance += PmE[i0] * PmE[i0] + PpE[i1] * PpE[i1] + PpE[i2] * PpE[i2] + delta * param;

					mLineParameter = param;
					pnt[i0] = mBox.Extents[i0];
					pnt[i1] = -mBox.Extents[i1];
					pnt[i2] = -mBox.Extents[i2];
				}
			}
		}

		private static void CaseNoZeros(ref Box3 mBox, ref double mLineParameter,
			ref Vector3D pnt, ref Vector3D dir, ref double sqrDistance)
		{
			Vector3D PmE = new Vector3D(
				pnt.x - mBox.Extents[0],
				pnt.y - mBox.Extents[1],
				pnt.z - mBox.Extents[2]);

			double prodDxPy = dir.x * PmE.y;
			double prodDyPx = dir.y * PmE.x;
			double prodDzPx, prodDxPz, prodDzPy, prodDyPz;

			if (prodDyPx >= prodDxPy)
			{
				prodDzPx = dir.z * PmE.x;
				prodDxPz = dir.x * PmE.z;
				if (prodDzPx >= prodDxPz)
				{
					// line intersects x = e0
					Face(ref mBox, ref mLineParameter,
						0, 1, 2, ref pnt, ref dir, ref PmE, ref sqrDistance);
				}
				else
				{
					// line intersects z = e2
					Face(ref mBox, ref mLineParameter,
						2, 0, 1, ref pnt, ref dir, ref PmE, ref sqrDistance);
				}
			}
			else
			{
				prodDzPy = dir.z * PmE.y;
				prodDyPz = dir.y * PmE.z;
				if (prodDzPy >= prodDyPz)
				{
					// line intersects y = e1
					Face(ref mBox, ref mLineParameter,
						1, 2, 0, ref pnt, ref dir, ref PmE, ref sqrDistance);
				}
				else
				{
					// line intersects z = e2
					Face(ref mBox, ref mLineParameter,
						2, 0, 1, ref pnt, ref dir, ref PmE, ref sqrDistance);
				}
			}
		}

		private static void Case0(ref Box3 mBox, ref double mLineParameter,
			int i0, int i1, int i2, ref Vector3D pnt, ref Vector3D dir, ref double sqrDistance)
		{
			double PmE0 = pnt[i0] - mBox.Extents[i0];
			double PmE1 = pnt[i1] - mBox.Extents[i1];
			double prod0 = dir[i1]*PmE0;
			double prod1 = dir[i0]*PmE1;
			double delta, invLSqr, inv;

			if (prod0 >= prod1)
			{
				// line intersects P[i0] = e[i0]
				pnt[i0] = mBox.Extents[i0];

				double PpE1 = pnt[i1] + mBox.Extents[i1];
				delta = prod0 - dir[i0]*PpE1;
				if (delta >= (double)0)
				{
					invLSqr = ((double)1)/(dir[i0]*dir[i0] + dir[i1]*dir[i1]);
					sqrDistance += delta*delta*invLSqr;
					pnt[i1] = -mBox.Extents[i1];
					mLineParameter = -(dir[i0]*PmE0 + dir[i1]*PpE1)*invLSqr;
				}
				else
				{
					inv = ((double)1)/dir[i0];
					pnt[i1] -= prod0*inv;
					mLineParameter = -PmE0*inv;
				}
			}
			else
			{
				// line intersects P[i1] = e[i1]
				pnt[i1] = mBox.Extents[i1];

				double PpE0 = pnt[i0] + mBox.Extents[i0];
				delta = prod1 - dir[i1]*PpE0;
				if (delta >= (double)0)
				{
					invLSqr = ((double)1)/(dir[i0]*dir[i0] + dir[i1]*dir[i1]);
					sqrDistance += delta*delta*invLSqr;
					pnt[i0] = -mBox.Extents[i0];
					mLineParameter = -(dir[i0]*PpE0 + dir[i1]*PmE1)*invLSqr;
				}
				else
				{
					inv = ((double)1)/dir[i1];
					pnt[i0] -= prod1*inv;
					mLineParameter = -PmE1*inv;
				}
			}

			if (pnt[i2] < -mBox.Extents[i2])
			{
				delta = pnt[i2] + mBox.Extents[i2];
				sqrDistance += delta*delta;
				pnt[i2] = -mBox.Extents[i2];
			}
			else if (pnt[i2] > mBox.Extents[i2])
			{
				delta = pnt[i2] - mBox.Extents[i2];
				sqrDistance += delta*delta;
				pnt[i2] = mBox.Extents[i2];
			}
		}
		
		private static void Case00(ref Box3 mBox, ref double mLineParameter,
			int i0, int i1, int i2, ref Vector3D pnt, ref Vector3D dir, ref double sqrDistance)
		{
			double delta;

			mLineParameter = (mBox.Extents[i0] - pnt[i0])/dir[i0];

			pnt[i0] = mBox.Extents[i0];

			if (pnt[i1] < -mBox.Extents[i1])
			{
				delta = pnt[i1] + mBox.Extents[i1];
				sqrDistance += delta*delta;
				pnt[i1] = -mBox.Extents[i1];
			}
			else if (pnt[i1] > mBox.Extents[i1])
			{
				delta = pnt[i1] - mBox.Extents[i1];
				sqrDistance += delta*delta;
				pnt[i1] = mBox.Extents[i1];
			}

			if (pnt[i2] < -mBox.Extents[i2])
			{
				delta = pnt[i2] + mBox.Extents[i2];
				sqrDistance += delta*delta;
				pnt[i2] = -mBox.Extents[i2];
			}
			else if (pnt[i2] > mBox.Extents[i2])
			{
				delta = pnt[i2] - mBox.Extents[i2];
				sqrDistance += delta*delta;
				pnt[i2] = mBox.Extents[i2];
			}
		}
		
		private static void Case000(ref Box3 mBox, ref double mLineParameter,
			ref Vector3D pnt, ref double sqrDistance)
		{
			double delta;

			if (pnt.x < -mBox.Extents[0])
			{
				delta = pnt.x + mBox.Extents[0];
				sqrDistance += delta * delta;
				pnt.x = -mBox.Extents[0];
			}
			else if (pnt.x > mBox.Extents[0])
			{
				delta = pnt.x - mBox.Extents[0];
				sqrDistance += delta * delta;
				pnt.x = mBox.Extents[0];
			}

			if (pnt.y < -mBox.Extents[1])
			{
				delta = pnt.y + mBox.Extents[1];
				sqrDistance += delta * delta;
				pnt.y = -mBox.Extents[1];
			}
			else if (pnt.y > mBox.Extents[1])
			{
				delta = pnt.y - mBox.Extents[1];
				sqrDistance += delta * delta;
				pnt.y = mBox.Extents[1];
			}

			if (pnt.z < -mBox.Extents[2])
			{
				delta = pnt.z + mBox.Extents[2];
				sqrDistance += delta * delta;
				pnt.z = -mBox.Extents[2];
			}
			else if (pnt.z > mBox.Extents[2])
			{
				delta = pnt.z - mBox.Extents[2];
				sqrDistance += delta * delta;
				pnt.z = mBox.Extents[2];
			}
		}


		public static double Line3Box3(ref Line3 line, ref Box3 box, out Line3Box3Dist info)
		{
			return System.Math.Sqrt(SqrLine3Box3(ref line, ref box, out info));
		}

		public static double Line3Box3(ref Line3 line, ref Box3 box)
		{
			Line3Box3Dist info;
			return System.Math.Sqrt(SqrLine3Box3(ref line, ref box, out info));
		}

		public static double SqrLine3Box3(ref Line3 line, ref Box3 box, out Line3Box3Dist info)
		{
			// Compute coordinates of line in box coordinate system.
			Vector3D diff = line.Center - box.Center;
			Vector3D point = new Vector3D(
				diff.Dot(box.Axis0),
				diff.Dot(box.Axis1),
				diff.Dot(box.Axis2));
			Vector3D direction = new Vector3D(
				line.Direction.Dot(box.Axis0),
				line.Direction.Dot(box.Axis1),
				line.Direction.Dot(box.Axis2));

			// Apply reflections so that direction vector has nonnegative components.
			bool reflect0, reflect1, reflect2;
			if (direction.x < 0f)
			{
				point.x = -point.x;
				direction.x = -direction.x;
				reflect0 = true;
			}
			else
			{
				reflect0 = false;
			}
			if (direction.y < 0f)
			{
				point.y = -point.y;
				direction.y = -direction.y;
				reflect1 = true;
			}
			else
			{
				reflect1 = false;
			}
			if (direction.z < 0f)
			{
				point.z = -point.z;
				direction.z = -direction.z;
				reflect2 = true;
			}
			else
			{
				reflect2 = false;
			}

			double sqrDistance = 0f;
			double mLineParameter = 0f;

			if (direction.x > 0f)
			{
				if (direction.y > 0f)
				{
					if (direction.z > 0f)  // (+,+,+)
					{
						CaseNoZeros(ref box, ref mLineParameter, ref point, ref direction, ref sqrDistance);
					}
					else  // (+,+,0)
					{
						Case0(ref box, ref mLineParameter, 0, 1, 2, ref point, ref direction, ref sqrDistance);
					}
				}
				else
				{
					if (direction.z > 0f)  // (+,0,+)
					{
						Case0(ref box, ref mLineParameter, 0, 2, 1, ref point, ref direction, ref sqrDistance);
					}
					else  // (+,0,0)
					{
						Case00(ref box, ref mLineParameter, 0, 1, 2, ref point, ref direction, ref sqrDistance);
					}
				}
			}
			else
			{
				if (direction.y > 0f)
				{
					if (direction.z > 0f)  // (0,+,+)
					{
						Case0(ref box, ref mLineParameter, 1, 2, 0, ref point, ref direction, ref sqrDistance);
					}
					else  // (0,+,0)
					{
						Case00(ref box, ref mLineParameter, 1, 0, 2, ref point, ref direction, ref sqrDistance);
					}
				}
				else
				{
					if (direction.z > 0f)  // (0,0,+)
					{
						Case00(ref box, ref mLineParameter, 2, 0, 1, ref point, ref direction, ref sqrDistance);
					}
					else  // (0,0,0)
					{
						Case000(ref box, ref mLineParameter, ref point, ref sqrDistance);
					}
				}
			}

			// Compute closest point on line.
			Vector3D mClosestPoint0 = line.Center + mLineParameter * line.Direction;

			// Compute closest point on box.
			Vector3D mClosestPoint1 = box.Center;
			// Undo the reflections applied previously.
			if (reflect0)
			{
				point.x = -point.x;
			}
			mClosestPoint1 += point.x * box.Axis0;
			if (reflect1)
			{
				point.y = -point.y;
			}
			mClosestPoint1 += point.y * box.Axis1;
			if (reflect2)
			{
				point.z = -point.z;
			}
			mClosestPoint1 += point.z * box.Axis2;

			info.ClosestPoint0 = mClosestPoint0;
			info.ClosestPoint1 = mClosestPoint1;
			info.LineParameter = mLineParameter;

			return sqrDistance;
		}

		public static double SqrLine3Box3(ref Line3 line, ref Box3 box)
		{
			Line3Box3Dist info;
			return SqrLine3Box3(ref line, ref box, out info);
		}
	}
}
