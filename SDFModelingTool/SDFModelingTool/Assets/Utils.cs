using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class Utils
    {
        /// <summary>
        /// 返回在摄像机坐标系中定义的由摄像机位置指向近裁剪面的四个顶点的四个向量组成的矩阵
        /// 第一行：指向左上顶点的向量
        /// 第二行：指向右上顶点的向量
        /// 第三行：指向右下顶点的向量
        /// 第四行：指向左下顶点的向量
        /// </summary>
        public static Matrix4x4 GetFrustumCorners(Camera cam)
        {
            float camFov = cam.fieldOfView;
            float camAspect = cam.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            float fovWHalf = camFov * 0.5f;

            float tan_fov = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 toRight = Vector3.right * tan_fov * camAspect;
            Vector3 toTop = Vector3.up * tan_fov;

            Vector3 topLeft = (-Vector3.forward - toRight + toTop);
            Vector3 topRight = (-Vector3.forward + toRight + toTop);
            Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
            Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            return frustumCorners;
        }
    }
}
