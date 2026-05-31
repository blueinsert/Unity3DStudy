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