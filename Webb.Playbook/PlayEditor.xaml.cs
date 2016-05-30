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
    /// PlayEditor.xaml 的交互逻辑
    /// </summary>
    public partial class PlayEditor : Window
    {
        public PlayEditor(PlayInfo playinfo)
        {
            InitializeComponent();

            gridPlay.DataContext = playinfo;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();

        }
    }
   
   
}
