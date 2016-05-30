using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Webb.Playbook.Data;

namespace Webb.Playbook.Team
{
    public partial class JerseyColor
    {


        public JerseyColor(Object objColor)
        {
            this.InitializeComponent();
            gridBinding.DataContext = objColor;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CommonClickHandler(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {

                case "btnOk":
                    Ok();
                    break;
                case "btnCancel":
                    (this.Parent as Grid).Children.Remove(this);
                    break;
            }


        }
        private void CommonMouseHanler(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {
                case "recTorso":

                    SetRecColor(recTorso);

                    break;
                case "recPads":
                    SetRecColor(recPads);

                    break;
                case "recText":
                    SetRecColor(recText);

                    break;
                case "recPrimary":
                    SetRecColor(recPrimary);

                    break;
                case "recLegStripe":
                    SetRecColor(recLegStripe);

                    break;

            }
        }
        private void SetRecColor(Rectangle fsRec)
        {
            Microsoft.Samples.CustomControls.ColorPickerDialog cPicker
                = new Microsoft.Samples.CustomControls.ColorPickerDialog();
            cPicker.Owner = App.Current.MainWindow;
            cPicker.StartingColor = (fsRec.Fill as SolidColorBrush).Color;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                (fsRec.Fill as SolidColorBrush).Color = cPicker.SelectedColor;
            }
        }
        private void Ok()
        {
            BindingExpression bindingExpressionTorsoColor = BindingOperations.GetBindingExpression(recTorso, Rectangle.FillProperty);
            bindingExpressionTorsoColor.UpdateSource();
            BindingExpression bindingPadsColor = BindingOperations.GetBindingExpression(recPads, Rectangle.FillProperty);
            bindingPadsColor.UpdateSource();
            BindingExpression bindingTextColor = BindingOperations.GetBindingExpression(recText, Rectangle.FillProperty);
            bindingTextColor.UpdateSource();
            BindingExpression bindingPrimaryColor = BindingOperations.GetBindingExpression(recPrimary, Rectangle.FillProperty);
            bindingPrimaryColor.UpdateSource();
            BindingExpression bindingLegStripeColor = BindingOperations.GetBindingExpression(recLegStripe, Rectangle.FillProperty);
            bindingLegStripeColor.UpdateSource();

            (this.Parent as Grid).Children.Remove(this);

        }

    }
}