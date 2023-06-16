using System;
using System.Runtime.InteropServices;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class BringWindowToFront
    {
        internal static void ForWindowsProcess(int processId)
        {
            IntPtr handle = FindMainWindowForProcess(processId);

            if (IsIconic(handle))
                ShowWindow(handle, SW_RESTORE);

            SetForegroundWindow(handle);
        }

        static IntPtr FindMainWindowForProcess(int processId)
        {
            IntPtr result = IntPtr.Zero;

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                uint windowProcessId = 0;
                GetWindowThreadProcessId(wnd, out windowProcessId);

                if (windowProcessId == processId &&
                    IsMainWindow(wnd))
                {
                    result = wnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return result;
        }

        static bool IsMainWindow(IntPtr handle)
        {
            return GetWindow(new HandleRef(null, handle), GW_OWNER) == IntPtr.Zero
                && IsWindowVisible(new HandleRef(null, handle));
        }

        // Delegate to filter which windows to include
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool IsWindowVisible(HandleRef hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

        const int GW_OWNER = 4;
        const int SW_RESTORE = 9;
    }
}
