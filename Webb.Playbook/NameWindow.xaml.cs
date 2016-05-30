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
    public partial class NameWindow : Window
    {
        private Regex regex;
        private NameViewModel nameViewModel;

        private bool selectAll = false;
        public bool SelectAll
        {
            get
            {
                return selectAll;
            }
            set
            {
                selectAll = value;
            }
        }

        public NameWindow()
        {
            InitializeComponent();

            nameViewModel = new NameViewModel();

            this.DataContext = nameViewModel;

            regex = new Regex(@"^\w[\w|\s|-]{0,50}\w{0,}$");  // 07-19-2010 Scott

            this.Loaded += new RoutedEventHandler(NameWindow_Loaded);
        }

        void NameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectAll)
            {
                this.textName.SelectAll();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OnOK();
        }

        private void OnOK()
        {
            if (textPrompt.Visibility == Visibility.Hidden && this.textName.Text != string.Empty)
            {
                this.DialogResult = true;

                this.Close();
            }
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

    public class NameViewModel : NotifyObj
    {
        private string title;
        public string Title
        {
            get { return title; }
            set 
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set 
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set 
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public NameViewModel()
        {
            Title = "New Name";
            Name = "Name:";
            Value = string.Empty;
        }
    }
}
