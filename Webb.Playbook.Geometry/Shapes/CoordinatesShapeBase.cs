using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

namespace Webb.Playbook.Geometry.Shapes
{
    public abstract class CoordinatesShapeBase<TShape> : ShapeBase<TShape>
        where TShape : UIElement
    {
        public override void MoveToCore(Point newLocation)
        {
            Coordinates = newLocation;
        }

        public override void UpdateVisual()
        {
            Shape.MoveTo(ToPhysical(Coordinates));

            if (Visible)
            {
                Shape.Visibility = Visibility.Visible;
            }
            else
            {
                Shape.Visibility = Visibility.Hidden;
            }
        }

        // 12-16-2011 Scott
        public HandlePoint HandlePoint { get; set; }

        public Point Coordinates { get; set; }

        public override IFigure HitTest(Rect rect)
        {
            return null;
        }
    }
}
