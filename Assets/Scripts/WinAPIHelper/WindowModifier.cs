using System;
using System.Runtime.InteropServices;
using NonsensicalKit.Core;
using UnityEngine;

namespace NonsensicalKit.Windows.Window
{
    public static class WindowModifier
    {
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("dwmapi.dll")]
        private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        //获取前台窗口句柄
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        private const int SW_Hide = 0; //隐藏任务栏图标  
        private const int SW_SHOWRESTORE = 1; //还原  
        private const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}  
        private const int SW_SHOWMAXIMIZED = 3; //最大化  

        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int LWA_ALPHA = 2;

        private const int SWP_FRAMECHANGED = 0x0020; //32
        private const int SWP_SHOWWINDOW = 0x0040; //64

        private const int WS_POPUP = 0x800000;
        private const uint WS_VISIBLE = 0x10000000;
        private const uint WS_EX_TRANSPARENT = 0x00000020;
        private const uint WS_EX_TOOLWINDOW = 0x00000080;
        private const uint WS_EX_LAYERED = 524288;

        private const int HWND_TOPMOST = -1;

        /// <summary>
        /// ！！！此方法不要在unity编辑器中执行！！！
        /// 使用winAPI将windows程序窗口透明化
        /// 摄像机应设置为纯色且颜色透明度设置为0
        /// 需要允许不安全代码
        /// 
        /// unity版本大于2018时，需要将UseDXGI flip model swapchain for d3d11 修改为false，否则背景无法透明
        /// 路径为 Edit > Project settings > Player > Resolution and Presentation
        /// 参考：https://discussions.unity.com/t/solved-transparent-window-in-unity-2020/859769/4
        /// </summary>
        public static void TransparentWindow()
        {
            if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
            {
                int fWidth = Screen.width;
                int fHeight = Screen.height;
                var margins = new MARGINS() { cxLeftWidth = -1 };
                var hwnd = GetActiveWindow();

                SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);

                SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_TRANSPARENT);

                SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
                SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
                DwmExtendFrameIntoClientArea(hwnd, ref margins);
            }
        }


        /// <summary>
        /// 最大化   
        /// </summary>
        public static void MaxSize(IntPtr hwnd)
        {
            if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
            {
                ShowWindow(hwnd, SW_SHOWMAXIMIZED);
            }
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="hwnd"></param>
        public static void MinSize(IntPtr hwnd)
        {
            if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
            {
                ShowWindow(hwnd, SW_SHOWMINIMIZED);
            }
        }

        /// <summary>
        /// 获取当前窗口句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetForegroundWindowHandle()
        {
            return GetForegroundWindow();
        }
    }
}
