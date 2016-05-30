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

namespace Webb.Playbook
{
    /// <summary>
    /// SnapSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SnapSettingWindow : Window
    {
        public SnapSettingWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public void CloseWindow()
        {
            this.Close();
        }
    }
}
