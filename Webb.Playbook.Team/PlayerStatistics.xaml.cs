using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
namespace Webb.Playbook.Team
{
    public partial class PlayerStatistics
    {
        public PlayerStatistics()
        {
            this.InitializeComponent();

        }
        public PlayerStatistics(Webb.Playbook.Data.Player ObjPlayer)
        {
            this.InitializeComponent();

            this.gridBinding.DataContext = ObjPlayer;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void CommonClickHandler(Object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {
                case "btnNote":
                    ChangeNote();
                    break;
                case "btnOk":
                    Ok();
                    break;
                case "btnCancel":
                    (this.Parent as Grid).Children.Remove(this);
                    break;

            }
        }
        private void ChangeNote()
        {
            Changetxt objChangetxt = new Changetxt("Enter New Note", "Enter new note for this Player", true);
            objChangetxt.txtOut.Height = 50;
            objChangetxt.Owner = (((this.Parent as Grid).Parent as Grid).Parent as Border).Parent as Window;
            bool? dialogResult = objChangetxt.ShowDialog();

            if (dialogResult == true)
            {
                this.txtNote.Text = objChangetxt.txtOut.Text;
            }
        }
        private void Ok()
        {
            BindingExpression bindingHeight = cmbHeight.GetBindingExpression(ComboBox.TextProperty);
            if (bindingHeight != null)
            {
                bindingHeight.UpdateSource();
            }
            BindingExpression bindingWeight = txtWeight.GetBindingExpression(TextBox.TextProperty);
            if (bindingWeight != null)
            {
                bindingWeight.UpdateSource();
            }

            BindingExpression bindingNote = txtNote.GetBindingExpression(TextBlock.TextProperty);
            if (bindingNote != null)
            {
                bindingNote.UpdateSource();
            }

            (this.Parent as Grid).Children.Remove(this);

        }

    }
}