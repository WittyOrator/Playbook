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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Webb.Playbook.Geometry
{
    /// <summary>
    /// Playground.xaml 的交互逻辑
    /// </summary>
    public partial class Playground : UserControl
    {
        public Playground()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Playground_Loaded);

            SetPlaygroundType(Webb.Playbook.Data.GameSetting.Instance.PlaygroundType);
        }

        void Playground_Loaded(object sender, RoutedEventArgs e)
        {
            this.borderBackground.Height = this.borderBackground.ActualWidth *
                System.Windows.SystemParameters.PrimaryScreenHeight / System.Windows.SystemParameters.PrimaryScreenWidth;
        }

        public void ShowCenterLine()
        {
            this.lineCenter.Visibility = Visibility.Visible;
        }

        public void SetTitleBackground(Brush brush)
        {
            Panel.SetZIndex(gridBackground, 10000);

            canvasBackground.Background = brush;

            canvasPlayground.Visibility = Visibility.Hidden;

            lineCenter.Visibility = Visibility.Hidden;
        }

        public void SetPlaygroundType(Webb.Playbook.Data.PlaygroundTypes pt)
        {
            int nWidth = 642;

            switch (pt)
            {
                case Webb.Playbook.Data.PlaygroundTypes.NCAA:
                    Canvas.SetLeft(GroupRight, nWidth * 100 / 160);
                    Canvas.SetLeft(GroupLeft, nWidth * 60 / 160);
                    TxtLeft.Visibility = Visibility.Visible;
                    TxtRight.Visibility = Visibility.Visible;
                    TxtLeftCFL.Visibility = Visibility.Hidden;
                    TxtRightCFL.Visibility = Visibility.Hidden;
                    break;
                case Webb.Playbook.Data.PlaygroundTypes.HighSchool:
                    Canvas.SetLeft(GroupRight, nWidth * 106.7 / 160);
                    Canvas.SetLeft(GroupLeft, nWidth * 53.4 / 160);
                    TxtLeft.Visibility = Visibility.Visible;
                    TxtRight.Visibility = Visibility.Visible;
                    TxtLeftCFL.Visibility = Visibility.Hidden;
                    TxtRightCFL.Visibility = Visibility.Hidden;
                    break;
                case Webb.Playbook.Data.PlaygroundTypes.NFL:
                    Canvas.SetLeft(GroupRight, nWidth * 89.5 / 160);
                    Canvas.SetLeft(GroupLeft, nWidth * 70.9 / 160);
                    TxtLeft.Visibility = Visibility.Visible;
                    TxtRight.Visibility = Visibility.Visible;
                    TxtLeftCFL.Visibility = Visibility.Hidden;
                    TxtRightCFL.Visibility = Visibility.Hidden;
                    break;
                case Webb.Playbook.Data.PlaygroundTypes.CFL:
                    Canvas.SetLeft(GroupRight, nWidth * 105.75 / 160);
                    Canvas.SetLeft(GroupLeft, nWidth * 54.25 / 160);
                    TxtLeft.Visibility = Visibility.Hidden;
                    TxtRight.Visibility = Visibility.Hidden;
                    TxtLeftCFL.Visibility = Visibility.Visible;
                    TxtRightCFL.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void SetPlaygroundColor(Webb.Playbook.Data.PlaygroundColors pc)
        {
            SolidColorBrush backBrush = grid.FindResource("PlaygroundBackColor") as SolidColorBrush;
            SolidColorBrush foreBrush = grid.FindResource("PlaygroundForeColor") as SolidColorBrush;

            switch (pc)
            {
                case Webb.Playbook.Data.PlaygroundColors.BlackAndWhite:
                    backBrush.Color = Colors.White;
                    foreBrush.Color = Colors.Black;
                    break;
                case Webb.Playbook.Data.PlaygroundColors.Color:
                    backBrush.Color = Color.FromRgb(115,172,103);
                    foreBrush.Color = Colors.White;
                    break;
            }

            this.InvalidateVisual();
        }
    }
}