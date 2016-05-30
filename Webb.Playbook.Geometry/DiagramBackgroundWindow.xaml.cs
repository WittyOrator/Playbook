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

namespace Webb.Playbook.Geometry
{
    /// <summary>
    /// DiagramBackgroundWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DiagramBackgroundWindow : Window
    {
        public DiagramBackgroundWindow(double width)
        {
            InitializeComponent();

            double scale = 8.4 / 10.0;

            Webb.Playbook.Geometry.Drawing drawing = new Webb.Playbook.Geometry.Drawing(canvas);

            this.Clip = new System.Windows.Media.RectangleGeometry(new Rect(0, 0, 720, 480));

            canvas.Margin = new Thickness((720 - (width * scale)) / 2, 0, (720 - (width * scale)) / 2, 0);
        }

        public DiagramBackgroundWindow(int width, Size size)
        {
            InitializeComponent();

            double scale = 8.4 / 10.0;

            this.Width = size.Width;
            this.Height = size.Height;

            Webb.Playbook.Geometry.Drawing drawing = new Webb.Playbook.Geometry.Drawing(canvas);

            this.Clip = new System.Windows.Media.RectangleGeometry(new Rect(0, 0, size.Width, size.Height));

            canvas.Margin = new Thickness((size.Width - (width * scale)) / 2, 0, (size.Width - (width * scale)) / 2, 0);
        }

        public DiagramBackgroundWindow(string filename, int width, Size size,bool background)
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = -1000;
            this.Top = -2000;

            double scale = 8.4 / 10.0;

            this.Width = size.Width;
            this.Height = size.Height;

            if (System.IO.File.Exists(filename))
            {
                Webb.Playbook.Geometry.Drawing drawing = Webb.Playbook.Geometry.Drawing.Load(filename, canvas);
                if (!background)
                {
                    drawing.Canvas.Children.Remove(drawing.Playground.UCPlayground);
                }
                if (drawing.Figures.OfType<Game.PBBall>().Count() > 0)
                {
                    drawing.Remove(drawing.Figures.OfType<Game.PBBall>().First());
                }
            }
            else
            {
                Webb.Playbook.Geometry.Drawing drawing = new Webb.Playbook.Geometry.Drawing(canvas);
            }

            this.Clip = new System.Windows.Media.RectangleGeometry(new Rect(0, 0, size.Width, size.Height));

            canvas.Margin = new Thickness((size.Width - (width * scale)) / 2, 0, (size.Width - (width * scale)) / 2, 0);
        }
    }
}
