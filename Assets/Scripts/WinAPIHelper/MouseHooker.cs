using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NonsensicalKit.Windows.Hook;
using Debug = UnityEngine.Debug;

namespace NonsensicalKit.Windows.Hook
{
    /// <summary>
    /// 鼠标钩子
    /// </summary>
    public class MouseHooker
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public Action<HookMouseMessage> MouseEvent;
        private HashSet<HookMouseMessage> _blockList = new HashSet<HookMouseMessage>();

        private bool _hooking;

        public MouseHooker()
        {
            _proc = HookCallback;
        }

        ~MouseHooker()
        {
            StopHook();
        }

        public void StartHook()
        {
            if (_hooking == false)
            {
                _hooking = true;
                _hookID = SetHook(_proc);
            }
        }

        /// <summary>
        /// 不能只使用析构函数，unity不一定正常执行
        /// </summary>
        public void StopHook()
        {
            if (_hooking == true)
            {
                _hooking = false;
                UnhookWindowsHookEx(_hookID);
            }
        }

        public void AddBlock(params HookMouseMessage[] messages)
        {
            foreach (var message in messages)
            {
                _blockList.Add(message);
            }
        }

        public void RemoveBlock(params HookMouseMessage[] messages)
        {
            foreach (var message in messages)
            {
                _blockList.Remove(message);
            }
        }

        public void ClearBlock()
        {
            _blockList.Clear();
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx((int)HookID.WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (Enum.IsDefined(typeof(HookMouseMessage), (int)wParam))
                {
                    HookMouseMessage message = (HookMouseMessage)wParam;
                    if (_blockList.Contains(message))
                    {
                        return (IntPtr)1;
                    }

                    MouseEvent?.Invoke(message);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}