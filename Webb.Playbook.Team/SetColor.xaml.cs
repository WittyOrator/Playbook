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

namespace Webb.Playbook.Team
{
    /// <summary>
    /// SetColor.xaml 的交互逻辑
    /// </summary>
    public partial class SetColor : Window
    {
        public SetColor()
        {
            InitializeComponent();
        }
        Color _value = Colors.Black;
 public void OnRedSliderChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._value = Color.FromArgb(_value.A, Convert.ToByte(e.NewValue), _value.G, _value.B);
            this.OnValueChanged();
        }
        public void OnGreenSliderChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._value = Color.FromArgb(_value.A, _value.R, Convert.ToByte(e.NewValue), _value.B);
           this.OnValueChanged();
        }
        public void OnBlueSliderChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._value = Color.FromArgb(_value.A, _value.R, _value.G, Convert.ToByte(e.NewValue));
           this.OnValueChanged();
       }

        private void OnValueChanged()
        {
            this.Preview.Background = new SolidColorBrush(this._value);
            this.redValue.Text = this._value.R.ToString();
            this.greenValue.Text = this._value.G.ToString();
            this.blueValue.Text = this._value.B.ToString();
       }
    }
}
