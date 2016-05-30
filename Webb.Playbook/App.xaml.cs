using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Interop;

namespace Webb.Playbook
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex singleProcessMutex;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //bool bMutexWasCreated;

            //singleProcessMutex = new Mutex(true, "PlaybookLock", out bMutexWasCreated);

            //if (bMutexWasCreated)
            //{
            //    singleProcessMutex.ReleaseMutex();
            //}
            //else
            //{
            //    Process[] processes = Process.GetProcessesByName("Playbook");
            //    if (processes.Count() > 1)
            //    {
            //        foreach (Process process in processes)
            //        {
            //            if (process.MainWindowHandle != IntPtr.Zero)
            //            {
            //                InteropHelper.ShowWindowAsync(process.MainWindowHandle, 1);

            //                InteropHelper.SetForegroundWindow(process.MainWindowHandle);
            //            }
            //        }
            //    }

            //    this.Shutdown();
            //}
        }
    }
}
