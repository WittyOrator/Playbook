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
    public partial class EditPlayer
    {
        private Webb.Playbook.Data.Player objSendPlayer;
        public EditPlayer()
        {
            this.InitializeComponent();


        }
        public EditPlayer(Webb.Playbook.Data.Player objPlayer)
        {
            this.InitializeComponent();
            this.gridBinding.DataContext = objPlayer;
            this.gpboxPlayer.Header = objPlayer.Name;
            objSendPlayer = objPlayer;

        }
        private void Window_Load(object sender, RoutedEventArgs e)
        {


        }

        private void CommonClickHandler(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {

                case "btnName":
                    ChangeName();
                    break;
                case "btnNumber":
                    ChangeNumber();
                    break;

                case "btnOk":
                    Ok();
                    break;

                case "btnVitals":
                    Vitals();
                    break;
                case "btnGameStats":
                    break;
                case "btnCancel":
                    (this.Parent as Grid).Children.Remove(this);
                    
                    break;
            }
        }
        private void ChangeName()
        {
            Changetxt objChangetxt = new Changetxt("Rename Player", "Enter a new name for this Player");
            objChangetxt.Owner = (((this.Parent as Grid).Parent as Grid).Parent as Border).Parent as Window;
            bool? dialogResult = objChangetxt.ShowDialog();
         
            if (dialogResult == true)
            {
                this.txtName.Text = objChangetxt.txtOut.Text;
            }

        }
        private void ChangeNumber()
        {
            Changetxt objChangetxt = new Changetxt("Enter New Number", "Enter a new number for this Player","Number");
            objChangetxt.Owner = (((this.Parent as Grid).Parent as Grid).Parent as Border).Parent as Window;

            bool? DialogResult = objChangetxt.ShowDialog();
            if (DialogResult == true)
            {
                this.txtNumber.Text = objChangetxt.txtOut.Text;
            }
        }

        private void Vitals()
        {
            PlayerStatistics objPlayerStatistics = new PlayerStatistics(objSendPlayer);
            (this.Parent as Grid).Children.Add(objPlayerStatistics);

        }
        private void Ok()
        {
            BindingExpression bindingName = txtName.GetBindingExpression(TextBlock.TextProperty);
            bindingName.UpdateSource();

            BindingExpression bindingNumber = txtNumber.GetBindingExpression(TextBlock.TextProperty);
            bindingNumber.UpdateSource();
            BindingExpression bindinghandleft = rbtnHandLeft.GetBindingExpression(RadioButton.IsCheckedProperty);
            if (bindinghandleft != null)
            {
                bindinghandleft.UpdateSource();
            }
            BindingExpression bindinghandright = rbtnHandRight.GetBindingExpression(RadioButton.IsCheckedProperty);
            if (bindinghandright != null)
            {
                bindinghandright.UpdateSource();
            }

            BindingExpression bindingappblack = rbtnAppBlack.GetBindingExpression(RadioButton.IsCheckedProperty);
            if (rbtnAppBlack.IsChecked==true)
            {
                bindingappblack.UpdateSource();
            }
            BindingExpression bindingappyellow = rbtnAppYellow.GetBindingExpression(RadioButton.IsCheckedProperty);
            if (rbtnAppYellow.IsChecked == true)
            {
                bindingappyellow.UpdateSource();
            }
            BindingExpression bindingappwhite = rbtnAppWhite.GetBindingExpression(RadioButton.IsCheckedProperty);
            if (rbtnAppWhite.IsChecked == true)
            {
                bindingappwhite.UpdateSource();
            }
            (this.Parent as Grid).Children.Remove(this);
           


        }
    }

}
