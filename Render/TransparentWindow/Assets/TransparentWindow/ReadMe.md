# Unity无边透明窗口(Windows平台)

# 特别说明

下述方法测试于Unity6.3、Unity6.4。

# **环境配置**

## **相机的参数**

- **Background Type**: Solid Color
- **Background**: RGBA(0, 0, 0, 0)
- **Post Processing**: 关掉
- **HDR Rendering**: 关掉

## **Project Settings:**

**Player->Resolution And Presentation:**

- Run In Background: 打开
- Full Screen Mode: Windowed
- Resizable Window: 关掉
- Visible In Background: 打开
- Use DXGI flip model swapchain for D3D11: 关掉

**Player->Other Settings:**

- Auto Graphics API for Windows: 关掉
- Graphics API for Windows: 将Direct3D11拖到第一个

# **代码配置**

将下面的TransparentWindowController挂到场景里任意一个Game Object上即可。

## **关键代码解释**

- 在ApplyTransparentWindowCoroutine里使用一个循环去不断地获取GetActiveWindow()是因为一开始窗口没有创建完成，只在Start里获取一次的话，一般会返回IntPtr.Zero这样的无效值，当然延迟一秒去获取也是可以的，这里为了稳定性选择了循环获取，超时时间默认是5秒。
- 下面这段代码是为了去掉窗口的非客户区，比如标题栏、边框。

```csharp
var style = GetWindowLong(currentWindowHandle, GWL_STYLE);
style &= ~WS_CAPTION;
SetWindowLong(currentWindowHandle, GWL_STYLE, style);
```

- 下面这段代码是为了修复Windows 客户区坐标错位，如果没有这段代码的话，鼠标点击UI会有一定程度上的向上错位。

```csharp
SetWindowPos(currentWindowHandle, IntPtr.Zero, 0, 0, 0, 0,
SetWindowPositionNoMove | SetWindowPositionNoSize | SetWindowPositionNoZOrder |
SetWindowPositionFrameChanged);
```

- 下面这段是为了实现背景透明。
  MARGINS margins = new MARGINS()
  {
    cxLeftWidth = -1,
  };

DwmExtendFrameIntoClientArea(currentWindowHandle, ref margins);

## **TransparentWindowController代码**

普通协程版：

```csharp
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindowController : MonoBehaviour
{
    [SerializeField]
    private float timeoutDuration = 5f;

    private const int GWL_STYLE = -16;
    private const int WS_BORDER = 0x00800000;
    private const int WS_DLGFRAME = 0x00400000;
    private const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

    private const uint SetWindowPositionNoSize = 0x0001;
    private const uint SetWindowPositionNoMove = 0x0002;
    private const uint SetWindowPositionNoZOrder = 0x0004;
    private const uint SetWindowPositionFrameChanged = 0x0020;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr windowHandle, IntPtr windowInsertAfter, int x, int y, int width,
        int height, uint flags);

    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr windowHandle, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    private void Start()
    {
#if UNITY_EDITOR
        return;
#endif
#if UNITY_STANDALONE_WIN
        StartCoroutine(ApplyTransparentWindowCoroutine());
#endif
    }

    private IEnumerator ApplyTransparentWindowCoroutine()
    {
        IntPtr currentWindowHandle = IntPtr.Zero;
        float elapsedTime = 0f;

        while (currentWindowHandle == IntPtr.Zero && elapsedTime < timeoutDuration)
        {
            currentWindowHandle = GetActiveWindow();

            if (currentWindowHandle != IntPtr.Zero)
            {
                break;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (currentWindowHandle == IntPtr.Zero)
        {
            Debug.LogError("Failed to get active window handle.");
            yield break;
        }

        var style = GetWindowLong(currentWindowHandle, GWL_STYLE);
        style &= ~WS_CAPTION;
        SetWindowLong(currentWindowHandle, GWL_STYLE, style);

        SetWindowPos(currentWindowHandle, IntPtr.Zero, 0, 0, 0, 0,
            SetWindowPositionNoMove | SetWindowPositionNoSize | SetWindowPositionNoZOrder |
            SetWindowPositionFrameChanged);

        MARGINS margins = new MARGINS()
        {
            cxLeftWidth = -1,
        };

        DwmExtendFrameIntoClientArea(currentWindowHandle, ref margins);
    }
}
```

UniTask版：

```CSHAR
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransparentWindowControllerUniTask : MonoBehaviour
{
    [SerializeField]
    private float timeoutDuration = 5f;

    private const int GWL_STYLE = -16;
    private const int WS_BORDER = 0x00800000;
    private const int WS_DLGFRAME = 0x00400000;
    private const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

    private const uint SetWindowPositionNoSize = 0x0001;
    private const uint SetWindowPositionNoMove = 0x0002;
    private const uint SetWindowPositionNoZOrder = 0x0004;
    private const uint SetWindowPositionFrameChanged = 0x0020;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr windowHandle, IntPtr windowInsertAfter, int x, int y, int width,
        int height, uint flags);

    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr windowHandle, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    private void Start()
    {
#if UNITY_EDITOR
        return;
#endif
#if UNITY_STANDALONE_WIN
        ApplyTransparentWindow().Forget();
#endif
    }

    private async UniTaskVoid ApplyTransparentWindow()
    {
        IntPtr currentWindowHandle;

        var timeoutCancellationTokenSource = new CancellationTokenSource();
        timeoutCancellationTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeoutDuration));

        try
        {
            await UniTask.WaitUntil(() => GetActiveWindow() != IntPtr.Zero,
                cancellationToken: timeoutCancellationTokenSource.Token);
            currentWindowHandle = GetActiveWindow();
        }
        catch (OperationCanceledException)
        {
            Debug.LogError("Failed to get active window handle.");
            return;
        }

        var style = GetWindowLong(currentWindowHandle, GWL_STYLE);
        style &= ~WS_CAPTION;
        SetWindowLong(currentWindowHandle, GWL_STYLE, style);

        SetWindowPos(currentWindowHandle, IntPtr.Zero, 0, 0, 0, 0,
            SetWindowPositionNoMove | SetWindowPositionNoSize | SetWindowPositionNoZOrder |
            SetWindowPositionFrameChanged);

        MARGINS margins = new MARGINS()
        {
            cxLeftWidth = -1,
        };

        DwmExtendFrameIntoClientArea(currentWindowHandle, ref margins);
    }
}
```