using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NonsensicalKit.Windows.Hook
{
    /// <summary>
    /// 键盘钩子
    /// </summary>
    public class KeyboardHooker
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public Action<HookKeyboardMessage, VirtualKeys> KeyboardEvent;
        private HashSet<VirtualKeys> _blockList = new HashSet<VirtualKeys>();

        private bool _hooking;

        public KeyboardHooker()
        {
            _proc = HookCallback;
        }

        ~KeyboardHooker()
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

        public void AddBlock(params VirtualKeys[] keys)
        {
            foreach (var key in keys)
            {
                _blockList.Add(key);
            }
        }

        public void RemoveBlock(params VirtualKeys[] keys)
        {
            foreach (var key in keys)
            {
                _blockList.Remove(key);
            }
        }

        public void ClearBlock()
        {
            _blockList.Clear();
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx((int)HookID.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var keyCode = Marshal.ReadInt32(lParam);
                if (Enum.IsDefined(typeof(HookKeyboardMessage), (int)wParam)
                    && Enum.IsDefined(typeof(VirtualKeys), keyCode))
                {
                    HookKeyboardMessage message = (HookKeyboardMessage)wParam;
                    VirtualKeys key = (VirtualKeys)keyCode;
                    if (_blockList.Contains(key))
                    {
                        return (IntPtr)1;
                    }

                    KeyboardEvent?.Invoke(message, key);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
