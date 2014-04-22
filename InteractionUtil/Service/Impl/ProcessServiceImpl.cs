using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InteractionUtil.Service.Interface;

namespace InteractionUtil.Service.Impl
{
    internal class ProcessServiceImpl : IProcessService
    {
        private static int GWL_STYLE = -16;
        private static uint WS_MINIMIZE = 0x20000000;

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public void SendKeyToProcess(String name, String key)
        {
            using (Process process = Process.GetProcessesByName(name).FirstOrDefault())
            {
                if (null != process)
                {
                    IntPtr h2 = process.MainWindowHandle;
                    SetForegroundWindow(h2);

                    int style = GetWindowLong(process.MainWindowHandle, GWL_STYLE);
                    if ((style & WS_MINIMIZE) == WS_MINIMIZE)
                    {
                        ShowWindow(h2, 4);
                    }

                    SendKeys.SendWait(key);
                    SendKeys.Flush();
                }
            }
        }
    }
}