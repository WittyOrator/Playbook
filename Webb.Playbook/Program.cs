using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Windows.Interop;

namespace Webb.Playbook
{
    static class Program
    {
        private static Mutex singleProcessMutex;
        /// <summary>
        /// 应用程序的主入口点。

        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bMutexWasCreated;

            singleProcessMutex = new Mutex(true, "PlaybookLock", out bMutexWasCreated);

            if (bMutexWasCreated)
            {
                singleProcessMutex.ReleaseMutex();
            }
            else
            {
                Process[] processes = Process.GetProcessesByName("Playbook");
                if (processes.Count() > 1)
                {
                    foreach (Process process in processes)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            InteropHelper.ShowWindowAsync(process.MainWindowHandle, 1);

                            InteropHelper.SetForegroundWindow(process.MainWindowHandle);
                        }
                    }
                }

                return;
            }

            try
            {
                App app = new App();

                app.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Source:\n" + ex.Source + "\n\n" + "Message:\n" + ex.Message + "\n\n" + "StackTrace:\n" + ex.StackTrace, "Webb Trace");
            }
        }
    }
}
