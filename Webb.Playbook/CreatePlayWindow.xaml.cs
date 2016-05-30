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

using Webb.Playbook.Data;

namespace Webb.Playbook
{
    /// <summary>
    /// NameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatePlayWindow : Window
    {
        private Regex regexOK = new Regex(@"^[^?\*|""<>:/]{1,256}$");
        private Regex regex;
        private NameViewModel nameViewModel;

        public CreatePlayWindow(ValueCollect valueCollect)
        {
            InitializeComponent();

            nameViewModel = new NameViewModel();

            this.DataContext = nameViewModel;

            regex = new Regex(@"^\w[\w|\s|-]{0,50}\w{0,}$");  // 07-19-2010 Scott

            textName.DataContext = valueCollect;
        }

        private bool Validate(string value)
        {
            return regexOK.Match(value).Success;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(textName.Text))
            {
                tbValidation.Text = string.Format("Name can't contain the following any one of the characters:\r\n {0}", @"\ / : * ? "" < > | ");

                return;
            }

            OnOK();
        }

        private void OnOK()
        {
            if (textPrompt.Visibility == Visibility.Hidden && this.textName.Text != string.Empty)
            {
                this.DialogResult = true;

                this.Close();
            }

            FileName = textName.Text;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }

        public string FileName
        {
            set
            {
                nameViewModel.Value = value;
            }
            get
            {
                return nameViewModel.Value;
            }
        }

        new public string Title
        {
            set
            {
                nameViewModel.Title = value;
            }
            get
            {
                return nameViewModel.Title;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textName.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnOK();
            }
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (regex.Match(textName.Text).Success)
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
