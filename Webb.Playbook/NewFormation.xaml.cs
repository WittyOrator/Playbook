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

namespace Webb.Playbook
{
    /// <summary>
    /// NewFormation.xaml 的交互逻辑
    /// </summary>
    public partial class NewFormation : Window
    {
        private Regex regex = new Regex(@"^[^?\*|""<>:/]{1,256}$");
        public event Action<NewFormation> FormationSelected;

        public NewFormation(FormationCollect formationcollect, ValueCollect valuecollect)
        {
            InitializeComponent();

            cmbset.DataContext = formationcollect;

            txtName.DataContext = valuecollect;

            this.Loaded += new RoutedEventHandler(NewFormation_Loaded);
        }

        void NewFormation_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtName.Focus();
        }

        private bool Validate(string value)
        {
            return regex.Match(value).Success;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(txtName.Text))
            {
                tbValidation.Text = string.Format("Name can't contain the following any one of the characters:\r\n {0}", @"\ / : * ? "" < > | ");

                return;
            }
            
            OnOK();
        }
        private void OnOK()
        {
            if (this.txtName.Text != string.Empty)
            {
                this.DialogResult = true;

                this.Close();
            }
        }

        private void cmbset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormationSelected(this);
        }

        private void cmbset_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }

    public class FormationCollect
    {
       private List<string> formations;
       public List<string> Formations
        {
            get
            {
                if (formations == null)
                {
                    formations = new List<string>();
                }
                return formations;
            }
        }
    }

    public class ValueCollect
    {
        private List<string> values;
        public List<string> Values
        {
            get
            {
                if (values == null)
                {
                    values = new List<string>();
                }
                return values;
            }
        }
    }
}
