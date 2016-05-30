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

namespace Webb.Playbook.Team
{
    public partial class HelmetColor
    {

        public HelmetColor()
        {
            this.InitializeComponent();

            gridBinding.DataContext = EditTeam.ObjTeam;

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
                case "recBaseColor":

                    SetRecColor(recBaseColor);
                    break;
                case "recStripeColor":
                    SetRecColor(recStripeColor);
                    break;
                case "recMaskColor":
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
            BindingExpression bindingExpressionbasecolor = BindingOperations.GetBindingExpression(recBaseColor, Rectangle.FillProperty);
            bindingExpressionbasecolor.UpdateSource();
            BindingExpression bindingstripecolor = BindingOperations.GetBindingExpression(recStripeColor, Rectangle.FillProperty);
            bindingstripecolor.UpdateSource();
            (this.Parent as Grid).Children.Remove(this);

        }
    }
}