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
using System.Text.RegularExpressions;

namespace Webb.Playbook.Call
{
    /// <summary>
    /// ChangeTxt.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeTxt : Window
    {
        private Regex regex;
        public ChangeTxt()
        {
            InitializeComponent();

        }
        public ChangeTxt(string strTitle, string strShow)
        {
            InitializeComponent();
            this.Title = strTitle;
            this.txbShow.Text = strShow;
            regex = new Regex(@"^[\w|\s]{0,18}\w{0,}$");
        }
        public ChangeTxt(string strTitle, string strShow, bool longer)
        {
            InitializeComponent();
            this.Title = strTitle;
            this.txbShow.Text = strShow;
            regex = new Regex(@"^\w[\w|\s]{0,32}");

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtOut.Focus();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Owner.IsEnabled = true;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            OnOK();
        }
        private void OnOK()
        {
            if (textPrompt.Visibility == Visibility.Hidden && this.txtOut.Text != string.Empty)
            {
                this.DialogResult = true;

                this.Close();
            }
        }
        protected string TxtOut()
        {
            return txtOut.Text;
        }

        private void txtOut_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (regex.Match(txtOut.Text).Success)
            {
                textPrompt.Visibility = Visibility.Hidden;
            }
            else
            {
                textPrompt.Visibility = Visibility.Visible;
            }
        }


    }
}
