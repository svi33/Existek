using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Existek
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    public class MousePosArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public uint MouseData { get; set; }
        public uint Flags { get; set; }
        public uint Time { get; set; }
        public MouseMessages Message { get; set; }

        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_WBUTTONDOWN = 0x0207,
            WM_WBUTTONUP = 0x0208
        }
    }

    public static class MouseHook
    {
        public static void Start()
        {
            _mouseHookID = SetLowLevelHook(_llMouseproc, WH_MOUSE_LL);
        }

        public static void stop()
        {
            UnhookWindowsHookEx(_mouseHookID);
        }
        private delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static event EventHandler<MousePosArgs> MouseAction = delegate { };
        private static LowLevelHookProc _llMouseproc = MouseHookCallback;
        private static IntPtr _mouseHookID = IntPtr.Zero;
        private const int WH_MOUSE_LL = 14;

        private static IntPtr SetLowLevelHook(LowLevelHookProc proc, int hookId)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                IntPtr hook = SetWindowsHookEx(hookId, proc, GetModuleHandle("user32"), 0);
                if (hook == IntPtr.Zero) throw new System.ComponentModel.Win32Exception();
                return hook;
            }
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 /*&& MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam*/)
            {
                var hookStruct = (MSLLMOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLMOUSEHOOKSTRUCT));
                MouseAction(null, new MousePosArgs() { X = hookStruct.pt.x, Y = hookStruct.pt.y, MouseData = hookStruct.mouseData, Flags = hookStruct.flags, Time = hookStruct.time, Message = (MousePosArgs.MouseMessages)wParam });
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLMOUSEHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
