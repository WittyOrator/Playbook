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
using Webb.Playbook.Geometry.Game;
using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook
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

        public DiagramBackgroundWindow(string filename, bool cropped, Size size, out Size retSize, bool background, bool autoImage)
        {
            InitializeComponent();

            this.MinWidth = size.Width;
            this.MinHeight = size.Height;

            this.ShowInTaskbar = false;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = -3000;
            this.Top = -3000;

            this.ResizeMode = ResizeMode.NoResize;

            Webb.Playbook.Geometry.Drawing drawing = null;

            if (System.IO.File.Exists(filename))
            {
                drawing = Webb.Playbook.Geometry.Drawing.Load(filename, canvas);
                if (!background)
                {
                    drawing.Canvas.Children.Remove(drawing.Playground.UCPlayground);
                }

                if (autoImage)
                {
                    drawing.SetFiguresColor(Webb.Playbook.Data.GameSetting.Instance.ImageEnableSymbolColor);
                    drawing.SetPlaygroundColor(Webb.Playbook.Data.GameSetting.Instance.ImageEnableColor ? Webb.Playbook.Data.PlaygroundColors.Color : Webb.Playbook.Data.PlaygroundColors.BlackAndWhite);
                }

                IEnumerable<FreePoint> fPoints = drawing.Figures.OfType<FreePoint>().Where(f => f.Visible);

                if (fPoints != null && fPoints.Count() > 0)
                {
                    Point startPoint = fPoints.First().Coordinates;

                    if (drawing.Figures.Any(f => f is PBBall))
                    {
                        PBBall ball = drawing.Figures.OfType<PBBall>().First();

                        if (ball.Visible)
                        {
                            startPoint = ball.Coordinates;

                            ball.Visible = false;
                        }
                    }
                    
                    Rect rectLogic = Rect.Empty;
                    foreach (FreePoint fPoint in fPoints)
                    {
                        if (rectLogic == Rect.Empty)
                        {
                            rectLogic = new Rect(startPoint, fPoint.Coordinates);
                        }
                        else
                        {
                            rectLogic.Union(fPoint.Coordinates);
                        }
                    }

                    double unitLength = size.Width / CoordinateSystem.LogicalPlaygroundWidth;
                    Point origin = new Point(size.Width / 2, size.Height / 2);

                    Rect rect = new Rect(new Point(origin.X + rectLogic.TopLeft.X * unitLength, origin.Y - rectLogic.TopLeft.Y * unitLength),
                        new Point(origin.X + rectLogic.BottomRight.X * unitLength, origin.Y - rectLogic.BottomRight.Y * unitLength));

                    rect.Inflate(unitLength, unitLength);

                    this.Clip = new System.Windows.Media.RectangleGeometry(rect);

                    retSize = rect.Size;
                }
                else
                {
                    retSize = Size.Empty;
                }
            }
            else
            {
                retSize = Size.Empty;
            }
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

        public DiagramBackgroundWindow(string filename, int width, Size size)
        {
            InitializeComponent();

            double scale = 8.4 / 10.0;

            this.Width = size.Width;
            this.Height = size.Height;

            if (System.IO.File.Exists(filename))
            {
                Webb.Playbook.Geometry.Drawing drawing = Webb.Playbook.Geometry.Drawing.Load(filename, canvas);
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
