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

using Webb.Playbook.Geometry;
using Draw = Webb.Playbook.Geometry.Drawing;

namespace Webb.Playbook
{
    /// <summary>
    /// AnimationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationWindow : Window
    {
        private Draw OriDrawing;
        private Canvas OriCanvas;
        private bool Play = false;
        private double Opacity = 1;

        private ToolManager toolManager2DToolBar = new ToolManager();

        public AnimationWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(AnimationWindow_Loaded);

            toolManager2DToolBar.Init2DFullScreenToolBarMenu();
            toolBarPlayControl.ItemsSource = toolManager2DToolBar.Tools;
        }

        void AnimationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            imgLogo.Effect = Webb.Playbook.Geometry.PBEffects.SelectedEffect;

            Scale(Webb.Playbook.Data.GameSetting.Instance.ScalingAnimation);
        }

        public AnimationWindow(Draw drawing)
            : this()
        {
            OriDrawing = drawing;

            OriDrawing.Stop();

            OriCanvas = drawing.Canvas;

            OriDrawing.Canvas = canvasDrawing;
        }

        public AnimationWindow(string path)
            : this()
        {
            OriDrawing = Draw.Load(path, canvasDrawing);
            OriDrawing.Behavior = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tool b = (sender as Button).Tag as Tool;

            if (b != null)
            {
                switch (b.Command)
                {
                    case Commands.PlayAni:
                        OriDrawing.Run();
                        break;
                    case Commands.StopAni:
                        OriDrawing.Stop();
                        break;
                    case Commands.Still:
                        OriDrawing.Still();
                        break;
                    case Commands.CloseAnimation:
                        CloseWindow();
                        break;
                    case Commands.SwitchRoutes:
                        OriDrawing.Stop();
                        switch (Opacity.ToString())
                        {
                            case "0":
                                Opacity = 0.2;
                                OriDrawing.TransparentRoute(0.2);
                                break;
                            case "0.2":
                                Opacity = 1;
                                OriDrawing.TransparentRoute(1);
                                break;
                            case "1":
                                Opacity = 0;
                                OriDrawing.TransparentRoute(0);
                                break;
                            default:
                                Opacity = 1;
                                OriDrawing.TransparentRoute(1);
                                break;
                        }
                        break;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CloseWindow();
                    break;
                case Key.Space:
                    OriDrawing.Still();
                    break;
                case Key.Enter:
                    OriDrawing.Run();
                    break;
            }

            if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                e.Handled = true;
            }
        }

        public void CloseWindow()
        {
            this.Close();

            OriDrawing.Stop();

            OriDrawing.Canvas = OriCanvas;
        }

        private void gridContainer_Loaded(object sender, RoutedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void gridContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OriDrawing != null)
            {
                OriDrawing.Stop();
            }

            Scale(e.NewValue);
        }

        private void Scale(double scale)
        {
            if (OriDrawing != null)
            {
                int nSpace = (int)((100 - scale) / 50 * 200);

                canvasDrawing.Margin = new Thickness(nSpace, 5, nSpace, 5);
            }
        }

        private void imgLogo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.webbelectronics.com//");
        }
    }
}
