using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Management;

namespace Webb.Playbook
{
    /// <summary>
    /// ActivateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ActivateWindow : Window
    {
        private bool bActive = false;

        private bool checkRealOnly = false;
        public bool CheckRealOnly
        {
            get
            {
                return checkRealOnly;
            }
            set
            {
                checkRealOnly = value;
            }
        }

        public ActivateWindow()
        {
            InitializeComponent();

            tbSeed.Text = GetSerialNumber();

            //tbSerial.Text = WebbActivate.Activate.GenerateKey(tbSeed.Text, false, 13);

            this.Loaded += new RoutedEventHandler(ActivateWindow_Loaded);
        }

        void ActivateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool? bReg = Webb.Playbook.Activate.Register.CheckRegister();

            if (bReg != null)
            {
                btnContinueTrial.Visibility = Visibility.Visible;

                if (bReg == false)
                {
                    tbTrialed.Visibility = Visibility.Visible;

                    tbReal.Visibility = Visibility.Hidden;

                    tbTrial.Visibility = Visibility.Hidden;
                }

                if (CheckRealOnly)
                {
                    btnContinueTrial.Visibility = Visibility.Collapsed;

                    tbTrialed.Visibility = Visibility.Hidden;

                    tbReal.Visibility = Visibility.Visible;

                    tbTrial.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                btnContinueTrial.Visibility = Visibility.Collapsed;

                tbTrial.Visibility = Visibility.Visible;

                tbReal.Visibility = Visibility.Hidden;

                tbTrialed.Visibility = Visibility.Hidden;
            }
        }

        public static string GetSerialNumber()
        {
            const int MAX_FILENAME_LEN = 256;
            int retVal = 0;
            int a = 0;
            int b = 0;
            string str1 = null;
            string str2 = null;

            GetVolumeInformation(
                @"C:\",
                str1,
                MAX_FILENAME_LEN,
                ref retVal,
                a,
                b,
                str2,
                MAX_FILENAME_LEN);

            return retVal.ToString("x");
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetVolumeInformation(
        string lpRootPathName,                      //欲获取信息的那个卷的根路径 
        string lpVolumeNameBuffer,                  //用于装载卷名（卷标）的一个字串 
        int nVolumeNameSize,                        //lpVolumeNameBuffer字串的长度   
        ref int lpVolumeSerialNumber,               //用于装载磁盘卷序列号的变量   
        int lpMaximumComponentLength,               //指定一个变量，用于装载文件名每一部分的长度。例如，在“c:\component1\component2.ext”的情况下，它就代表component1或component2名称的长度 .

        int lpFileSystemFlags,                      //用于装载一个或多个二进制位标志的变量。对这些标志位的解释如下：
        //FS_CASE_IS_PRESERVED                      //文件名的大小写记录于文件系统
        //FS_CASE_SENSITIVE                         //文件名要区分大小写
        //FS_UNICODE_STORED_ON_DISK                 //文件名保存为Unicode格式
        //FS_PERSISTANT_ACLS                        //文件系统支持文件的访问控制列表（ACL）安全机制
        //FS_FILE_COMPRESSION                       //文件系统支持逐文件的进行文件压缩
        //FS_VOL_IS_COMPRESSED                      //整个磁盘卷都是压缩的

        string lpFileSystemNameBuffer,              //指定一个缓冲区,用于装载文件系统的名称（如FAT，NTFS以及其他）
        int nFileSystemNameSize                     //lpFileSystemNameBuffer字串的长度
        );

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int nDays = 0;
            bool bReal = false;

            Webb.Playbook.Activate.Register reg = new Webb.Playbook.Activate.Register();

            bActive = WebbActivate.Activate.CheckSerial(tbSeed.Text, tbSerial.Text, out bReal, out nDays);

            if (bActive)
            {
                if (bReal)
                {//real
                    reg.SetRegister(true, Webb.Playbook.Activate.Register.RealDay, true);

                    MessageBox.Show("Thank you! Your Webb Playbook application has been activated!");
                }
                else
                {//trial
                    if (CheckRealOnly)
                    {
                        bActive = false;

                        MessageBox.Show("Webb Playbook\n\nThe trial version of this product has expired.\nContact Webb Electronics today to purchase!\n\nPhone: 972-242-5400\nwww.webbelectronics.com", "Webb Electronics Activate", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (Webb.Playbook.Activate.Register.CheckRegister() == false)
                        {
                            MessageBox.Show("ERROR: Please check that the seed and the key are correct.");
                        }
                        else
                        {
                            reg.SetRegister(true, nDays, false);

                            MessageBox.Show(string.Format("Thank you! Your Webb Playbook {0} day trial has been activated!", nDays.ToString()));
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("ERROR: Please check that the seed and the key are correct.");
            }

            string strDate = DateTime.Now.AddDays(-1).ToShortDateString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = bActive;

            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void btnContinueTrial_Click(object sender, RoutedEventArgs e)
        {
            if (CheckRealOnly)
            {
                MessageBox.Show("Webb Playbook\n\nThe trial version of this product has expired.\nContact Webb Electronics today to purchase!\n\nPhone: 972-242-5400\nwww.webbelectronics.com", "Webb Electronics Activate", MessageBoxButton.OK);
            }
            else
            {
                this.DialogResult = true;

                this.Close();
            }
        }
    }
}
