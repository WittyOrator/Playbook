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
    public interface ICircle : ISquare
    {
    }

    public class PBCircle : CoordinatesShapeBase<Shape>, ICircle, IMovable
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

        private double radius;
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public PBCircle()
            : base()
        {
            Shape = CreateShape();
        }

        public PBCircle(Point center, double radius)
            : base()
        {
            Shape = CreateShape();

            Center = center;
            Radius = radius;
        }

        public override IFigure HitTest(Point point)
        {
            if (Center.Distance(point) - Radius < 0)
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

            Shape.Fill = new SolidColorBrush()
            {
                Color = FillColor,
            };

            Shape.CenterAt(center);
        }

        public override Shape CreateShape()
        {
            Shape sh = Factory.CreateCircleShape();

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
                    HandlePoint.MoveTo(new Point(Coordinates.X + Radius, Coordinates.Y));
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeDouble("X", Coordinates.X);
            writer.WriteAttributeDouble("Y", Coordinates.Y);
            writer.WriteAttributeDouble("Radius", Radius);

            writer.WriteElementColor("FillColor", FillColor);
        }

        public override void ReadXml(XElement element)
        {
            base.ReadXml(element);

            Center = new System.Windows.Point(element.ReadDouble("X"), element.ReadDouble("Y"));
            Radius = element.ReadDouble("Radius");

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
                        HandlePoint handlePoint = Factory.CreateHandlePoint(Drawing, new Point(Coordinates.X + Radius, Coordinates.Y));

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
