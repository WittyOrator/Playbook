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

namespace Webb.Playbook.Presentation
{
    /// <summary>
    /// VideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VideoWindow : Window
    {
        public VideoWindow()
        {
            InitializeComponent();
        }

        public void PlayVideo(string strFile)
        {
            if (System.IO.File.Exists(strFile))
            {
                Title = strFile;

                mediaElement.Source = new Uri(strFile);
            }
            else
            {
                MessageBox.Show("Can't find " + strFile);
            }
        }
    }
}
