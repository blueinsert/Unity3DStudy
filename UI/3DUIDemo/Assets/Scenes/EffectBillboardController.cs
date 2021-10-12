using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Bluebean.ThreeDUI
{
    /// <summary>
    /// 编辑模式下让game object 始终朝向scene中的摄像头
    /// </summary>
    [ExecuteInEditMode]
    public class EffectBillboardController : MonoBehaviour
    {
        /// <summary>
        /// 运行时先自动从父节点中查找到ThreeDSceneLayer并获取LayerCamera，设置给自己的FaceToCameraInRuntime
        /// </summary>
        public void Start()
        {
            if (FaceToCameraInRuntime == null && Application.isPlaying)
            {
                FaceToCameraInRuntime = GetLayerCameraFromParent();
            }
        }

        /// <summary>
        /// 从父节点中查找到ThreeDSceneLayer并获取LayerCamera
        /// </summary>
        /// <returns></returns>
        protected Camera GetLayerCameraFromParent()
        {
            return GetComponentInParent<Canvas>().worldCamera;
        }

        /// <summary>
        /// Update is called every frame
        /// 非编辑模式下有效
        /// </summary>
        public void Update()
        {
            if (FaceToCameraInRuntime != null && Application.isPlaying)
            {
                if (RotationFaceMode == FaceMode.ReverseCameraRotation)
                {
                    var quat = FaceToCameraInRuntime.transform.rotation;
                    transform.rotation = quat * Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    //transform.rotation = Quaternion.LookRotation(transform.position - FaceToCamera.transform.position);
                    transform.LookAt(FaceToCameraInRuntime.transform);
                }

            }

            if (IsKeepSize)
            {
                KeepSize();
            }
        }

        /// <summary>
        /// OnRenderObject is called after camera has rendered the scene.
        /// 编辑模式下有效
        /// </summary>
        public void OnRenderObject()
        {
            if (FaceToCameraInEditor != null && !Application.isPlaying) //SceneView.lastActiveSceneView.camera
            {
                //transform.LookAt(FaceToCameraInEditor.transform);
                //Vector3 camEulerAngles = FaceToCameraInEditor.transform.rotation.eulerAngles;
                if (RotationFaceMode == FaceMode.ReverseCameraRotation)
                {
                    var quat = FaceToCameraInEditor.transform.rotation;
                    transform.rotation = quat * Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    transform.LookAt(FaceToCameraInEditor.transform);
                }
            }
        }

        protected void KeepSize()
        {
            if (Render == null || FaceToCameraInRuntime == null)
            {
                return;
            }         

            var keepScreenPortion = Width / Screen.currentResolution.width;
            var currScreenPortion = GetScreenPortion();

            var localScale = transform.localScale;
            var t = keepScreenPortion / currScreenPortion;
            transform.localScale = localScale * t;
        }


        /// <summary>
        /// 得到对象包围盒在屏幕上的比例
        /// </summary>
        /// <returns></returns>
        private float GetScreenPortion()
        {
            var bounds = Render.bounds;
            var objectSize = bounds.size.magnitude;
            //计算离屏幕一个单位长度的世界坐标所对应的像素长度
            var p0 = FaceToCameraInRuntime.ScreenToWorldPoint(new Vector3((Screen.width - 100f) / 2f, 0, 1f));
            var p1 = FaceToCameraInRuntime.ScreenToWorldPoint(new Vector3((Screen.width + 100f) / 2f, 0, 1f));
            var pixelsPerMeter = 1f / (Vector3.Distance(p0, p1) / 100f);

            var distance = Vector3.Distance(FaceToCameraInRuntime.transform.position, transform.position);
            var pixelSize = objectSize * pixelsPerMeter;
            var screenPortion = pixelSize / distance / Screen.width;
            return screenPortion;
        }

        /// <summary>
        /// 编辑状态下朝向的Camera
        /// </summary>
        [Header("编辑状态下朝向的Camera")]
        public Camera FaceToCameraInEditor = null;

        /// <summary>
        /// 运行状态下朝向的Camera
        /// </summary>
        [Header("运行状态下朝向的Camera")]
        public Camera FaceToCameraInRuntime;

        /// <summary>
        /// 特效对象Rotation是与摄像机朝向相反或朝向摄像机
        /// </summary>
        [Header("特效对象Camera朝向模式")]
        public FaceMode RotationFaceMode = FaceMode.ReverseCameraRotation;

        public bool IsKeepSize = false;

        public float Width;

        public Renderer Render;

        /// <summary>
        /// 特效对象的Rotation模式
        /// </summary>
        public enum FaceMode
        {
            ReverseCameraRotation,    // 摄像机朝向反转180度
            FaceToCamera,             // 朝向摄像机位置
        }
        
    }
}
