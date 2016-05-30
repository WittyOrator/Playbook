using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public interface ISquare
    {
        Point Center { get; set; }
        double Radius { get; set; }
    }

    public class PBSquare : CoordinatesShapeBase<Shape> , ISquare, IMovable
    {
        private Point center;
        public Point Center
        {
            get { return center; }
            set 
            { 
                center = value;
                Coordinates = value;
            }
        }

        private double radius;
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public PBSquare()
        {
            Shape = CreateShape();
        }

        public PBSquare(Point center, double radius)
        {
            Shape = CreateShape();

            this.Center = center;
            this.Radius = radius;
        }

        public override IFigure HitTest(Point point)
        {
            if ( Center.Distance(point) - Radius < 0)
            {
                return this;
            }
            return null;
        }

        public override void UpdateVisual()
        {
            var center = ToPhysical(Center);
            var diameter = ToPhysical(Radius * 2);
            Shape.Width = diameter;
            Shape.Height = diameter;
            Shape.CenterAt(center);
        }

        public override Shape CreateShape()
        {
            return Factory.CreateSquareShape();
        }

        public override void MoveToCore(Point newPosition)
        {
            Center = newPosition;
            base.MoveToCore(newPosition);
        }
    }

    public interface IRect
    {
        Point Center { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }


    public class PBRect : CoordinatesShapeBase<Shape>, IRect, IMovable
    {
        private Color fillColor = Colors.Transparent;
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        private Point center;
        public Point Center
        {
            get { return center; }
            set
            {
                center = value;
                Coordinates = value;
            }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public Rectangle Rectangle
        {
            get
            {
                return Shape as Rectangle;
            }
        }

        public PBRect()
        {
            Shape = CreateShape();
        }

        public PBRect(Point center, double width, double height)
        {
            Shape = CreateShape();

            this.Center = center;
            this.Width = width;
            this.Height = height;
        }

        public override IFigure HitTest(Point point)
        {
            if (Rectangle != null)
            {
                if (point.X > Center.X - Width / 2 && point.X < Center.X + Width / 2
                    && point.Y > Center.Y - Height / 2 && point.Y < Center.Y + Height / 2)
                {
                    return this;
                }
            }

            return null;
        }

        public override void UpdateVisual()
        {
            var center = ToPhysical(Center);
            Shape.Width = ToPhysical(Width);
            Shape.Height = ToPhysical(Height);

            Shape.Fill = new SolidColorBrush()
            {
                Color = FillColor,
            };

            Shape.CenterAt(center);
        }

        public override Shape CreateShape()
        {
            Shape sh = Factory.CreateSquareShape();

            return sh;
        }

        public override void MoveToCore(Point newPosition)
        {
            Center = newPosition;
            base.MoveToCore(newPosition);

            // 12-16-0-2011 Scott
            if (Behavior.DrawVideo)
            {
                if (HandlePoint != null)
                {
                    HandlePoint.MoveTo(new Point(Coordinates.X + Width / 2, Coordinates.Y - Height / 2));
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeDouble("X", Coordinates.X);
            writer.WriteAttributeDouble("Y", Coordinates.Y);
            writer.WriteAttributeDouble("Width", Width);
            writer.WriteAttributeDouble("Height", Height);

            writer.WriteElementColor("FillColor", FillColor);
        }

        public override void ReadXml(XElement element)
        {
            base.ReadXml(element);

            Center = new System.Windows.Point(element.ReadDouble("X"), element.ReadDouble("Y"));
            Width = element.ReadDouble("Width");
            Height = element.ReadDouble("Height");

            if (element.Element("FillColor") != null)
            {
                FillColor = element.ReadElementColor("FillColor");
            }
        }

        protected override void OnSelected(bool bSelected)
        {
            base.OnSelected(bSelected);

            // 12-16-2011 Scott
            if (Behavior.DrawVideo)
            {
                if (bSelected)
                {
                    if (HandlePoint == null)
                    {
                        HandlePoint handlePoint = Factory.CreateHandlePoint(Drawing, new Point(Coordinates.X + Width / 2, Coordinates.Y - Height / 2));

                        HandlePoint = handlePoint;

                        HandlePoint.Figure = this;

                        Drawing.Add(HandlePoint);
                    }

                    HandlePoint.Selected = true;
                }
                else
                {
                    if (HandlePoint != null)
                    {
                        HandlePoint.Selected = false;
                    }
                }
            }
        }
    }
}
