using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Webb.Playbook.Geometry.Shapes
{
    public abstract class LineBase : ShapeBase<Line>, ILinearFigure
    {
        //08-12-2009 scott
        private double strokeThickness = 3;
        public double StrokeThickness
        {
            get { return strokeThickness; }
            set 
            { 
                strokeThickness = value;
            }
        }

        //08-12-2009 scott
        private Color strokeColor = Colors.Black;
        public Color StrokeColor
        {
            get { return strokeColor; }
            set { strokeColor = value; }
        }

        public override Line CreateShape()
        {
            Line line = Factory.CreateLineShape();

            return line;
        }

        public virtual PointPair OnScreenCoordinates
        {
            get
            {
                return Coordinates;
            }
        }

        public override void UpdateVisual()
        {
            this.UpdateVisible();
            Shape.Set(ToPhysical(OnScreenCoordinates));
        }

        public virtual PointPair Coordinates
        {
            get { return new PointPair(Point(0), Point(1)); }
        }

        public override IFigure HitTest(System.Windows.Point point)
        {
            var basement = Math.GetProjectionPoint(point, Coordinates);
            var distance = point.Distance(basement);
            if (distance < ToLogical(this.Shape.StrokeThickness) + CursorTolerance)
            {
                return this;
            }
            return null;
        }

        public virtual double GetNearestParameterFromPoint(System.Windows.Point point)
        {
            return Math.GetProjectionRatio(Coordinates, point);
        }

        public Point GetPointFromParameter(double parameter)
        {
            PointPair line = Coordinates;
            return new System.Windows.Point(
                line.P1.X + (line.P2.X - line.P1.X) * parameter,
                line.P1.Y + (line.P2.Y - line.P1.Y) * parameter);
        }

        public virtual Tuple<double, double> GetParameterDomain()
        {
            var coordinates = OnScreenCoordinates;
            var p1 = GetNearestParameterFromPoint(coordinates.P1);
            var p2 = GetNearestParameterFromPoint(coordinates.P2);
            return new Tuple<double, double>(p1 * 2, p2 * 2);
        }

        public IEnumerable<IFigure> GetPathFigures()
        {
            int indexLine = 0;

            Game.PBPlayer player = this.GetPlayer(ref indexLine);

            if (player != null)
            {
                return player.GetPathFigure(player, null, indexLine);
            }

            return null;
        }

        public Game.PBPlayer GetPlayer(ref int indexLine)
        {
            if (this.Dependencies != null && this.Dependencies.Count() > 0)
            {
                IFigure figure = this.Dependencies.ElementAt(0);

                if (figure is Game.PBPlayer)
                {
                    // 09-26-2011 Scott
                    for (int i = 0; i < figure.Dependents.Count(); i++)
                    {
                        if (this == figure.Dependents.ElementAt(i))
                        {
                            indexLine = i;

                            break;
                        }
                    }
                    // end

                    return figure as Game.PBPlayer;
                }
                else
                {
                    if (figure.Dependents.Count() > 0)
                    {
                        foreach (IFigure lineFigure in figure.Dependents)
                        {
                            if (lineFigure != this && lineFigure is LineBase)
                            {
                                return (lineFigure as LineBase).GetPlayer(ref indexLine);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public override void VReverse()
        {
            base.VReverse();
        }

        public override void HReverse()
        {
            base.HReverse();
        }
        public override void BallHReverse(Point point)
        {
            base.BallHReverse(point);
        }

        public override void BallVReverse(Point point)
        {
            base.BallVReverse(point);
        }
        public override IFigure HitTest(Rect rect)
        {
            return null;
        }
    }
}
