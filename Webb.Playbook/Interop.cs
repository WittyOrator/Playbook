using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Webb.Playbook
{
    public class InteropHelper
    {
        public static bool DeleteToRecycleBin(string path, bool sillent)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);

            ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();

            String[] source = new String[1];
            source[0] = fi.FullName;

            HwndSource hs = (HwndSource)HwndSource.FromVisual(App.Current.MainWindow);
            IntPtr handle = hs.Handle;

            fo.Silent = true;
            fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_DELETE;
            fo.OwnerWindow = handle;
            fo.SourceFiles = source;

            return fo.DoOperation();
        }

        [DllImport("shell32.dll")]
        public extern static IntPtr ShellExecute(IntPtr hwnd,
                                                 string lpOperation,
                                                 string lpFile,
                                                 string lpParameters,
                                                 string lpDirectory,
                                                 int nShowCmd
                                                );

        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, short wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(
            IntPtr hwnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int wFlags

        );

        public static int MakeLParam(int heigher, int lower)
        {
            int nRet = heigher << 16;

            nRet |= lower;

            return nRet;
        }

        //该函数设置由不同线程产生的窗口的显示状态;

        //如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零。

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        //该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。 

        //如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零。

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // IsIconic、IsZoomed ------ 分别判断窗口是否已最小化、最大化 

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        //获取当前系统中被激活的窗口

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, int wCmd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


        public static IntPtr GetProcessWindow(Process process)
        {
            IntPtr Tmp_Hwnd = FindWindow(null, null);

            int TmpPid = 0;

            while ((int)Tmp_Hwnd != 0)
            {
                GetWindowThreadProcessId(Tmp_Hwnd, ref TmpPid);

                StringBuilder str = new StringBuilder(512);

                if (TmpPid == process.Id)
                {
                    if (IsWindowVisible(Tmp_Hwnd))
                    {
                        GetWindowText(Tmp_Hwnd, str, str.Capacity);

                        string text = str.ToString();

                        if (text != string.Empty)
                        {
                            return Tmp_Hwnd;
                        }
                    }
                }

                Tmp_Hwnd = GetWindow(Tmp_Hwnd, 2);
            }
            return Tmp_Hwnd;
        }
    }

    /// <summary> 
    /// 文件夹复制 
    /// zgke@Sina.com 
    /// </summary> 
    public class CopyDirectory
    {
        /* 使用方法 
        private void button1_Click(object sender, EventArgs e) 
        { 
            Zgke.Copy.CopyDirectory _Info = new Zgke.Copy.CopyDirectory(@"F:\项目文件\产品化\联络中心V1.4.0\Source", @"E:\Temp"); 
            _Info.MyCopyRun += new Zgke.Copy.CopyDirectory.CopyRun(_Info_MyCopyRun); 
            _Info.MyCopyEnd += new Zgke.Copy.CopyDirectory.CopyEnd(_Info_MyCopyEnd); 
            _Info.StarCopy(); 
        } 

        void _Info_MyCopyEnd() 
        { 
            MessageBox.Show("复制完成"); 
        } 
        void _Info_MyCopyRun(int FileCount, int CopyCount, long FileSize, long CopySize, string FileName) 
        { 
            this.Invoke((MethodInvoker)delegate { 
                progressBar1.Maximum = FileCount; 
                progressBar1.Value = CopyCount; 
                label1.Text = CopySize.ToString() + "/" + FileSize.ToString(); 
                label2.Text = FileName; 
            });             
        } 
        */

        /// <summary> 
        /// 源目录 
        /// </summary> 
        private DirectoryInfo _Source;
        /// <summary> 
        /// 目标目录 
        /// </summary> 
        private DirectoryInfo _Target;

        /// <summary> 
        /// 文件复制完成 
        /// </summary> 
        /// <param name="FileCount">文件数量合计</param> 
        /// <param name="CopyCount">复制完成的数量</param> 
        /// <param name="FileSize">文件大小合计</param> 
        /// <param name="CopySize">复制完成的大小</param> 
        /// <param name="FileName">复制完成的文件名</param> 
        public delegate void CopyRun(int FileCount, int CopyCount, long FileSize, long CopySize, string FileName);
        public event CopyRun MyCopyRun;
        /// <summary> 
        /// 复制完成 
        /// </summary> 
        public delegate void CopyEnd();
        public event CopyEnd MyCopyEnd;

        private int _FileCount = 0;
        private int _CopyCount = 0;
        private long _FileSize = 0;
        private long _CopySize = 0;

        /// <summary> 
        /// 复制目录包含文件 
        /// </summary> 
        /// <param name="p_SourceDirectory">源目录</param> 
        /// <param name="p_TargetDirectory">目标目录</param> 
        public CopyDirectory(string p_SourceDirectory, string p_TargetDirectory)
        {
            _Source = new DirectoryInfo(p_SourceDirectory);
            _Target = new DirectoryInfo(p_TargetDirectory);
            FileSystemInfo[] Temp = _Source.GetFileSystemInfos();
        }

        /// <summary> 
        /// 开始复制 
        /// </summary> 
        public void StarCopy()
        {
            GetFile(_Source);
            System.Threading.Thread Th = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
            Th.Start();
        }

        private void Run()
        {
            Copy(_Source, _Target);
            if (MyCopyEnd != null) MyCopyEnd();
        }

        /// <summary> 
        /// 复制目录到指定目录 
        /// </summary> 
        /// <param name="source">源目录</param> 
        /// <param name="target">目标目录</param> 
        private void GetFile(DirectoryInfo MySiurceDirectory)
        {
            foreach (FileInfo _File in MySiurceDirectory.GetFiles())    //循环文件    
            {
                _FileCount++;
                _FileSize += _File.Length;
            }


            foreach (DirectoryInfo _SourceSubDir in MySiurceDirectory.GetDirectories())  //循环子目录 
            {
                GetFile(_SourceSubDir);
            }
        }


        /// <summary> 
        /// 复制目录到指定目录 
        /// </summary> 
        /// <param name="source">源目录</param> 
        /// <param name="target">目标目录</param> 
        private void Copy(DirectoryInfo p_Source, DirectoryInfo p_Target)
        {
            if (!Directory.Exists(p_Target.FullName)) Directory.CreateDirectory(p_Target.FullName);

            foreach (FileInfo _File in p_Source.GetFiles())       //循环文件  
            {
                _File.CopyTo(Path.Combine(p_Target.ToString(), _File.Name), true);
                _CopyCount++;
                _CopySize += _File.Length;
                if (MyCopyRun != null) MyCopyRun(_FileCount, _CopyCount, _FileSize, _CopySize, _File.Name);
            }

            foreach (DirectoryInfo _SourceSubDir in p_Source.GetDirectories())  //循环子目录 
            {
                DirectoryInfo _NextDir = p_Target.CreateSubdirectory(_SourceSubDir.Name);
                Copy(_SourceSubDir, _NextDir);
            }
        }

        public static void CopyFolderFiles(string srcdir, string desdir, bool bOverwrite, Action<string> copyAnction)
        {
            string desfolderdir = desdir;

            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyFolder(file, desfolderdir, bOverwrite, copyAnction);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;

                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }

                    if (bOverwrite)
                    {
                        File.Copy(file, srcfileName, bOverwrite);
                    }
                    else
                    {
                        if (!File.Exists(srcfileName))
                        {
                            File.Copy(file, srcfileName, bOverwrite);
                        }
                    }

                    if (copyAnction != null)
                    {
                        copyAnction(srcfileName);
                    }
                }
            }//foreach
        }

        public static void GetFileNum(string srcPath, ref int count)
        {
            try
            {
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                // 遍历所有的文件和目录
                foreach(string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就重新调用GetFileNum(string srcPath)
                    if(System.IO.Directory.Exists(file))
                        GetFileNum(file, ref count);
                    else
                        count++;
                }
                
            }
            catch (Exception e)
            {
                
            }
        }

        public static void CopyFolder(string srcdir, string desdir, bool bOverwrite, Action<string> copyAnction)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\")+1);

            string desfolderdir = desdir +"\\"+ folderName;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyFolder(file, desfolderdir, bOverwrite, copyAnction);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\")+1);

                    srcfileName = desfolderdir + "\\" + srcfileName;

                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }

                    if (bOverwrite)
                    {
                        File.Copy(file, srcfileName, bOverwrite);
                    }
                    else
                    {
                        if (!File.Exists(srcfileName))
                        {
                            File.Copy(file, srcfileName, bOverwrite);
                        }
                    }

                    if (copyAnction != null)
                    {
                        copyAnction(srcfileName);
                    }
                }
            }//foreach
        }//function end
    }
}
