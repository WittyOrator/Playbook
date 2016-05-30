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

namespace Webb.Playbook.Presentation
{
    /// <summary>
    /// DrawingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DrawingWindow : Window
    {
        private static Draw Drawing;
        public static Draw OriDrawing
        {
            get
            {
                return Drawing;
            }
        }

        public Canvas Canvas
        {
            get
            {
                return CanvasPalette;
            }
        }

        public TextBlock Title
        {
            get
            {
                return textblockTitle;
            }
        }

        public DrawingWindow()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(DrawingWindow_KeyDown);

            this.Loaded += new RoutedEventHandler(DrawingWindow_Loaded);
        }

        void DrawingWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void DrawingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    break;
            }
        }

        public void LoadDrawing(string strFile)
        {
            CloseDrawing();

            if (!System.IO.File.Exists(strFile))
            {
                Drawing = new Webb.Playbook.Geometry.Drawing(Canvas);
            }
            else
            {
                Drawing = Draw.Load(strFile, Canvas);
            }

            Drawing.Playground.UCPlayground.Visibility = Visibility.Hidden;
            Drawing.SetDefaultBehavior();
        }

        public void SaveDrawing(string strFile)
        {
            if (Drawing != null)
            {
                Drawing.Save(strFile);
            }
        }

        public void CloseDrawing()
        {
            if (Drawing != null)
            {
                Drawing.Behavior = null;

                Drawing.Dispose();

                Drawing = null;
            }

            this.Canvas.Children.Clear();
        }

        public void SetBehavior(string strBehavior)
        {
            if (Drawing != null)
            {
                Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName(strBehavior);
            }
        }

        public void MessageBox(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }
    }
}
