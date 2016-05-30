using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

using Webb.Playbook.Geometry.Actions;

namespace Webb.Playbook.Geometry.Shapes
{
    public interface IMovable
    {
        void MoveTo(Point position);
        Point Coordinates { get; }
    }

    public static class IMovableExtensions
    {
        public static void MoveTo(this IMovable movable, Point newPosition, Drawing drawing)
        {
            MoveAction action = new MoveAction(drawing, new[] { movable }, newPosition.Minus(movable.Coordinates), null);
            drawing.ActionManager.RecordAction(action);
        }

        public static void MoveTo(this IMovable movable, double x, double y)
        {
            movable.MoveTo(new Point(x, y));
        }
    }
}
