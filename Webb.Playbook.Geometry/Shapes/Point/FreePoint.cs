using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Webb.Playbook.Geometry.Shapes
{
    public class ImmovablePoint : PointBase
    {
        public override void UpdateVisual()
        {
            //change by scott 01-19-2010
            //base.UpdateVisual();
            System.Windows.Point ptCenter = new System.Windows.Point(Coordinates.X + CoordinateSystem.LogicalMidPos.X, Coordinates.Y);
            Shape.CenterAt(ToPhysical(ptCenter));
            //end
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            var x = element.ReadDouble("X");
            var y = element.ReadDouble("Y");
            Coordinates = new Point(x, y);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            var coordinates = Coordinates;
            writer.WriteAttributeDouble("X", coordinates.X);
            writer.WriteAttributeDouble("Y", coordinates.Y);
        }

        public override double X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
            }
        }

        public override double Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
            }
        }
        public override void BallHReverse(Point point)
        {
            
        }
        public override void BallVReverse(Point point)
        {

        }
        public override void HReverse()
        {
            
        }
        public override void VReverse()
        {

        }
    }

    public class PrePoint : FreePoint
    {
        
    }

    public class SubPoint : FreePoint
    {

    }

    // 12-16-2011 Scott
    public class HandlePoint : FreePoint
    {
        private IFigure figure;
        public IFigure Figure
        {
            get { return figure; }
            set { figure = value; }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            
        }

        public override void MoveToCore(Point newLocation)
        {
            base.MoveToCore(newLocation);

            if (Figure != null)
            {
                if (Figure is PBRect)
                {
                    PBRect rect = Figure as PBRect;

                    rect.Width = Math.Abs(rect.Coordinates.X - Coordinates.X) * 2;

                    rect.Height = Math.Abs(rect.Coordinates.Y - Coordinates.Y) * 2;

                    rect.UpdateVisual();
                }

                if (Figure is PBCircle)
                {
                    PBCircle circle = Figure as PBCircle;

                    circle.Radius = Math.Distance(circle.Coordinates.X, circle.Coordinates.Y, Coordinates.X, Coordinates.Y);

                    circle.UpdateVisual();
                }
            }
        }
    }

    public class FreePoint : PointBase, IMovable
    {
        public override System.Windows.Media.Brush SelectedBrush
        {
            get
            {
                if (selectedBrush == null)
                {
                    selectedBrush = System.Windows.Media.Brushes.Yellow;
                }
                return base.selectedBrush;
            }
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            var x = element.ReadDouble("X");
            var y = element.ReadDouble("Y");
            Coordinates = new Point(x, y);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            var coordinates = Coordinates;
            writer.WriteAttributeDouble("X", coordinates.X);
            writer.WriteAttributeDouble("Y", coordinates.Y);
        }

        public override double X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
            }
        }

        public override double Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
            }
        }
    }
}
