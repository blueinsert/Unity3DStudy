using UnityEngine;

/// <summary>
/// 轨道相机控制器：围绕目标点旋转、缩放，并始终看向目标点。
/// 鼠标拖拽旋转视角，滚动滚轮缩放距离。
/// </summary>
public class OrbitCameraController : MonoBehaviour
{
    [Header("目标设置")]
    public Transform target;           // 要围绕并看向的目标物体
    public Vector3 targetOffset = Vector3.zero;  // 目标点的偏移量，例如想看向角色的头部时可设置

    [Header("旋转参数")]
    public float horizontalSpeed = 120f;   // 水平旋转速度（度/秒）
    public float verticalSpeed = 80f;      // 垂直旋转速度（度/秒）
    public float verticalMinLimit = -30f;   // 垂直角度下限（避免从下方翻转）
    public float verticalMaxLimit = 60f;    // 垂直角度上限

    [Header("缩放参数")]
    public float zoomSpeed = 2f;           // 滚轮缩放速度
    public float minDistance = 1f;          // 最近距离
    public float maxDistance = 20f;         // 最远距离
    public float initialDistance = 5f;      // 初始距离

    [Header("其他")]
    public bool needMouseButton = true;     // 是否按住鼠标右键才旋转（false 则直接拖拽旋转）
    public MouseButton rotateButton = MouseButton.Right;   // 旋转需要的鼠标按键

    // 内部变量
    private float currentDistance;          // 当前相机到目标点的距离
    private float currentAngleX = 0f;       // 绕 Y 轴的水平角度（度）
    private float currentAngleY = 20f;      // 绕 X 轴的垂直角度（度）

    // 枚举：支持鼠标左、中、右键
    public enum MouseButton { Left = 0, Right = 1, Middle = 2 }

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("OrbitCameraController: 未设置 target 目标点，请在 Inspector 中指定。");
            enabled = false;
            return;
        }

        // 初始化距离
        currentDistance = Mathf.Clamp(initialDistance, minDistance, maxDistance);

        // 尝试从当前相机位置推算初始角度，使相机与现有位置保持一致
        Vector3 fromTarget = transform.position - GetTargetPosition();
        if (fromTarget != Vector3.zero)
        {
            // 根据当前偏移向量计算出角度
            currentDistance = fromTarget.magnitude;
            currentAngleX = Mathf.Atan2(fromTarget.x, fromTarget.z) * Mathf.Rad2Deg;
            currentAngleY = Mathf.Asin(fromTarget.y / currentDistance) * Mathf.Rad2Deg;
        }
        // 限制角度范围
        currentAngleY = Mathf.Clamp(currentAngleY, verticalMinLimit, verticalMaxLimit);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        if (target == null) return;
        //Debug.Log($"{Input.mousePosition} {Screen.width} {Screen.height}");
        var mousePosition = Input.mousePosition;
        if(mousePosition.x <0 || mousePosition.x>=Screen.width
            || mousePosition.y < 0 || mousePosition.y >= Screen.height)
        {
            return;
        }
        // 处理旋转输入
        if (Input.GetMouseButton((int)rotateButton) && needMouseButton ||
            (!needMouseButton && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))))
        {
            // 获取鼠标移动增量
            float deltaX = Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
            float deltaY = Input.GetAxis("Mouse Y") * verticalSpeed * Time.deltaTime;

            currentAngleX += deltaX;
            currentAngleY -= deltaY;   // 减去 deltaY 使得向上拖拽时视角向上看
            currentAngleY = Mathf.Clamp(currentAngleY, verticalMinLimit, verticalMaxLimit);
        }

        // 处理缩放输入（鼠标滚轮）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        // 根据当前角度和距离计算相机位置
        Vector3 targetPos = GetTargetPosition();
        Quaternion rotation = Quaternion.Euler(currentAngleY, currentAngleX, 0);
        Vector3 offset = rotation * Vector3.back * currentDistance;  // 相机应该在目标的后方（负Z方向）并通过旋转指向目标
        transform.position = targetPos + offset;
        transform.LookAt(targetPos);
    }

    /// <summary>
    /// 获取目标点世界坐标（含偏移量）
    /// </summary>
    private Vector3 GetTargetPosition()
    {
        return target.position + targetOffset;
    }
}