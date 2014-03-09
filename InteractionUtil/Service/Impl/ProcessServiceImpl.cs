using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InteractionUtil.Service.Interface;


namespace InteractionUtil.Service.Impl
{
    public class ProcessServiceImpl : IProcessService
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);


        public void SendKeyToProcess(String name, String key)
        {
            Process process = Process.GetProcessesByName(name).FirstOrDefault();

            if (null != process)
            {
                IntPtr h2 = process.MainWindowHandle;
                SetForegroundWindow(h2);
                
                SendKeys.SendWait(key);
                SendKeys.Flush();
            }
        }
    }
}
