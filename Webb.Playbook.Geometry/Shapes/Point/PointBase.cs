using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;

namespace Webb.Playbook.Geometry.Shapes
{
    public abstract class PointBase : CoordinatesShapeBase<Shape>, IPoint
    {
        private double strokeThickness = 1;
        public double StrokeThickness
        {
            get { return strokeThickness; }
            set
            {
                strokeThickness = value;
            }
        }

        public PointBase()
            : base()
        {
            
        }

        protected override int DefaultZOrder()
        {
            return (int)ZOrder.Points;
        }

        public override Shape CreateShape()
        {
            return Factory.CreatePointShape();
        }

        public override void UpdateVisual()
        {
            Shape.CenterAt(ToPhysical(Coordinates));
        }

        public virtual double X
        {
            get
            {
                return Coordinates.X;
            }
            set
            {
                Coordinates = Coordinates.SetX(value);
            }
        }

        public virtual double Y
        {
            get
            {
                return Coordinates.Y;
            }
            set
            {
                Coordinates = Coordinates.SetY(value);
            }
        }

        protected double PointSize
        {
            get
            {
                return Shape.ActualWidth / 2;
            }
        }

        public override IFigure HitTest(Point point)
        {
            double sensitivity = CursorTolerance + ToLogical(PointSize);
            if (point.X >= Coordinates.X - sensitivity
                && point.X <= Coordinates.X + sensitivity
                && point.Y >= Coordinates.Y - sensitivity
                && point.Y <= Coordinates.Y + sensitivity)
            {
                return this;
            }
            return null;
        }

        public override IFigure HitTest(Rect rect)
        {
            return null;
        }

        public override void VReverse()
        {
            base.VReverse();
            Coordinates = new Point(-Coordinates.X, -Coordinates.Y);
        }

        public override void HReverse()
        {
            base.HReverse();
            Coordinates = new Point(-Coordinates.X, Coordinates.Y);
        }
        public override void BallHReverse(Point point)
        {
            base.BallHReverse(point);
            Coordinates = new Point(2 * point.X - Coordinates.X, Coordinates.Y);

        }
        public override void BallVReverse(Point point)
        {
            base.BallVReverse(point);
            Coordinates = new Point(2 * point.X - Coordinates.X, 2 * point.Y - Coordinates.Y);
        }

        protected override void UpdateShapeAppearance() // 11-18-2010 Scott
        {
            if (Selected)
            {
                Shape.Stroke = SelectedBrush;
            }
            else
            {
                Shape.Stroke = System.Windows.Media.Brushes.Transparent;
            }
        }
    }
}
