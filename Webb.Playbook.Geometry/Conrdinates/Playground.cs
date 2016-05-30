using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Webb.Playbook.Geometry.Conrdinates
{
    public class Playground : IDisposable
    {
        public Action CoordinateSystemUpdated;

        public void Dispose()
        {
            Drawing.Canvas.Children.Remove(playground);

            Drawing.Canvas.SizeChanged -= new System.Windows.SizeChangedEventHandler(Canvas_SizeChanged);
        }

        private Drawing drawing;
        public Drawing Drawing
        {
            get
            {
                return drawing;
            }
            set
            {
                drawing = value;
            }
        }

        public Canvas Canvas
        {
            get
            {
                if (drawing != null)
                {
                    return drawing.Canvas;
                }
                return null;
            }
        }

        private Webb.Playbook.Geometry.Playground playground;
        public Webb.Playbook.Geometry.Playground UCPlayground
        {
            get { return playground; }
            set { playground = value; }
        }

        public void SetPlaygroundType(Webb.Playbook.Data.PlaygroundTypes pt)
        {
            UCPlayground.SetPlaygroundType(pt);
        }

        public void SetPlaygroundColor(Webb.Playbook.Data.PlaygroundColors pc)
        {
            UCPlayground.SetPlaygroundColor(pc);
        }

        public Playground(Drawing drawing)
        {
            Drawing = drawing;

            UCPlayground = new Webb.Playbook.Geometry.Playground();
        }

        void UCPlayground_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayground();
        }

        void Canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdatePlayground();
        }

        public void UpdatePlayground()
        {
            UpdateCoordinateSystem();

            Recalculate();
        }

        private void UpdateCoordinateSystem()
        {
            CoordinateSystem.PhysicalPlaygroundWidth = Canvas.ActualWidth - 0;
            Drawing.CoordinateSystem.UnitLength = CoordinateSystem.PhysicalPlaygroundWidth / CoordinateSystem.LogicalPlaygroundWidth;
            Drawing.CoordinateSystem.Origin = new Point(Canvas.ActualWidth / 2 + Drawing.CoordinateSystem.ToLogical(CoordinateSystem.LogicalMidPos.X), Canvas.ActualHeight / 2/* + Drawing.CoordinateSystem.ToPhysical(CoordinateSystem.LogicalMidPos.Y)*/);    //Remove by Scott 01-18-2010

            if (CoordinateSystemUpdated != null)
            {
                CoordinateSystemUpdated();
            }
        }

        private void Recalculate()
        {
            // set y offset
            double xPos = CoordinateSystem.LogicalMidPos.X * Drawing.CoordinateSystem.UnitLength;
            double yPos = CoordinateSystem.LogicalMidPos.Y * Drawing.CoordinateSystem.UnitLength;

            // resize playground
            double xLen = CoordinateSystem.PhysicalPlaygroundWidth;
            double yLen = CoordinateSystem.LogicalPlaygroundHeight / CoordinateSystem.LogicalPlaygroundWidth * xLen;
            double xRatio = xLen / this.UCPlayground.ActualWidth;
            double yRatio = yLen / this.UCPlayground.ActualHeight;
            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform(xRatio, yRatio);
            tg.Children.Add(st);

            double xDif = Canvas.ActualWidth - xLen; // this.canvasPlayground.ActualWidth;
            double yDif = Canvas.ActualHeight - yLen; // this.canvasPlayground.ActualHeight;
            TranslateTransform tt = new TranslateTransform(xDif / 2 + xPos, yDif / 2 + yPos);
            tg.Children.Add(tt);

            this.UCPlayground.RenderTransform = tg;

            // set clip
            //Canvas.Clip = new RectangleGeometry(new Rect(0, 0, Canvas.ActualWidth, Canvas.ActualHeight));
            
            if (Drawing != null)
            {
                Drawing.Figures.UpdateVisual();
            }
        }

        // 09-16-2010 Scott
        public virtual void OnAddingToCanvas(Canvas newContainer)
        {
            Drawing.Canvas.SizeChanged += new System.Windows.SizeChangedEventHandler(Canvas_SizeChanged);

            UCPlayground.Loaded += new RoutedEventHandler(UCPlayground_Loaded);

            Drawing.Canvas.Children.Add(playground);
        }

        // 09-16-2010 Scott
        public virtual void OnRemovingFromCanvas(Canvas leavingContainer)
        {
            Drawing.Canvas.Children.Remove(playground);
        }
    }
}
