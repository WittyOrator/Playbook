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

using Webb.Playbook.Geometry;
using Webb.Playbook.Data;

namespace Webb.Playbook.Geometry.Shapes
{
    public enum CapType
    {
        None,
        Arrow,
        Block,
        BlockArea,
        BlockPerson,
    }

    public enum LineType
    {
        BeeLine,
        JaggedLine,
        CurvyLine,
    }

    public enum DashType
    {
        Solid,
        Dashed,
        Dotted,
        DashDot,
    }

    public enum PathType    // 12-01-2010 Scott
    {
        Straight,
        Curve,
    }

    public class PBLine : LineBase, ILine
    {
        FreePoint MovePoint = new FreePoint();
        protected Polygon cap;
        PointPair Pointline;
        Point CurvePoint = new Point(0, 0); // 12-01-2010 Scott
        private Path path;
        GeometryGroup geo;
        private Line line;

        private PathType pathType = PathType.Straight;
        public PathType PathType
        {
            get
            {
                return pathType;
            }
            set
            {
                pathType = value;
            }
        }

        private CapType capType;
        public CapType CapType
        {
            get { return capType; }
            set
            {
                capType = value;
                UpdateVisual();
            }
        }

        private DashType dashType;
        public DashType DashType
        {
            get { return dashType; }
            set { dashType = value; }
        }

        private LineType lineType;
        public LineType LineType
        {
            get { return lineType; }
            set
            {
                lineType = value;
                UpdateVisual();
            }
        }
        public double Length
        {
            get
            {
                return Coordinates.Length;
            }
        }

        public override double Opacity  // 11-04-2010 Scott
        {
            get
            {
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;

                if (cap != null)
                {
                    cap.Opacity = value;
                }

                if (path != null)
                {
                    path.Opacity = value;
                }
            }
        }

        public PBLine()
            : base()
        {
            path = new Path();
            Canvas.SetZIndex(path, (int)ZOrder.Figures);
            cap = new Polygon()
            {
                Fill = System.Windows.Media.Brushes.White
            };
            Canvas.SetZIndex(cap, (int)ZOrder.Figures);
        }

        public override double GetNearestParameterFromPoint(System.Windows.Point point)
        {
            var parameter = base.GetNearestParameterFromPoint(point);
            if (parameter < 0)
            {
                parameter = 0;
            }
            else if (parameter > 1)
            {
                parameter = 1;
            }
            return parameter;
        }

        public override IFigure HitTest(Point point)
        {
            if (Dependencies != null && Dependencies.All(p => p is ImmovablePoint))
            {
                return null;
            }

            PointPair boundingRect = Coordinates.GetBoundingRect().Inflate(CursorTolerance);

            switch (LineType)
            {
                case LineType.BeeLine:
                    return boundingRect.Contains(point) ? base.HitTest(point) : null;
                case LineType.CurvyLine:
                case LineType.JaggedLine:
                    boundingRect = Coordinates.GetBoundingRect().Inflate(CursorTolerance);

                    IFigure retFigure = null;
                    var basement = Math.GetProjectionPoint(point, Coordinates);
                    var distance = point.Distance(basement);
                    if (distance < 0.2 + CursorTolerance)
                    {
                        retFigure = this;
                    }

                    return boundingRect.Contains(point) ? retFigure : null;
            }
            return null;
        }

        public override IFigure HitTest(Rect rect)
        {
            if (Dependencies != null && !Dependencies.Any(f => f is ImmovablePoint))
            {
                PointPair boundingRect = Coordinates.GetBoundingRect().Inflate(CursorTolerance);

                if (rect.Contains(boundingRect.Midpoint) || rect.Contains(boundingRect.P1) || rect.Contains(boundingRect.P2))
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public override Line CreateShape()
        {
            line = new Line()
            {
                StrokeThickness = this.StrokeThickness,
            };

            return line;
        }

        public override Tuple<double, double> GetParameterDomain()
        {
            return Tuple.Create(0.0, 1.0);
        }

        private void SetColor(Color color)
        {
            if (EnableColor)
            {
                color = StrokeColor;
            }
            else
            {
                color = Colors.Black;
            }

            if (Shape != null)
            {
                Shape.Stroke = new SolidColorBrush(color);
            }
            if (cap != null)
            {
                cap.Fill = new SolidColorBrush(color);
            }
            if (path != null)
            {
                path.Stroke = new SolidColorBrush(color);
            }
        }

        public void UpdateColor()
        {
            if (this.Dependencies != null)
            {
                switch (CapType)
                {
                    case CapType.Arrow:
                        SetColor(ColorSetting.Instance.RouteColor);
                        break;
                    case CapType.Block:
                    case CapType.BlockArea:
                    case CapType.BlockPerson:
                        SetColor(ColorSetting.Instance.BlockColor);
                        break;
                    case CapType.None:
                        SetColor(StrokeColor);
                        break;
                }

                if (this.Dependencies.Any(f => f is Game.Zone))
                {
                    SetColor(ColorSetting.Instance.ZoneLineColor);
                }
                else if (this.Dependencies.Count() > 1 && this.Dependencies.ElementAt(1) is PrePoint)
                {
                    SetColor(ColorSetting.Instance.PreSnapMotionColor);
                }

                // 11-18-2010 Scott
                if (Selected)
                {
                    if (Shape != null)
                    {
                        Shape.Effect = PBEffects.SelectedEffect;
                    }
                    if (cap != null)
                    {
                        cap.Effect = PBEffects.SelectedEffect;
                    }
                    if (path != null)
                    {
                        path.Effect = PBEffects.SelectedEffect;
                    }
                }
                else
                {
                    if (Shape != null)
                    {
                        Shape.Effect = null;
                    }
                    if (cap != null)
                    {
                        cap.Effect = null;
                    }
                    if (path != null)
                    {
                        path.Effect = null;
                    }
                }

                if (Dependencies != null && Dependencies.Count() > 1 && (Dependencies.ElementAt(1).Dependents.Count() == 1 || Dependencies.ElementAt(1) is Game.PBPlayer))
                {
                    cap.Visibility = Visibility.Visible;
                }
                else
                {
                    cap.Visibility = Visibility.Hidden;
                }
            }
        }

        public override void UpdateVisual()
        {
            base.UpdateVisual();
            if (PathType == PathType.Straight)
            {
                switch (LineType)
                {
                    case LineType.JaggedLine:
                        {
                            double length = 15;
                            double angel = Math.PI / 4;
                            Pointline = ToPhysical(OnScreenCoordinates);
                            double PILine = -GetRadian(Pointline.P1, Pointline.P2);
                            double LineLongth = GetLength(Pointline.P1, Pointline.P2);
                            geo = new GeometryGroup();
                            binTree(Pointline.P1, Pointline.P2, length, angel, PILine, LineLongth);
                            path.StrokeThickness = this.StrokeThickness;
                            line.StrokeThickness = 0;
                            path.Data = geo;
                            break;
                        }
                    case LineType.CurvyLine:
                        {
                            Pointline = ToPhysical(OnScreenCoordinates);
                            PathFigure pf = DrawWave(Pointline.P1, Pointline.P2, 20d, 11, 11);
                            PathGeometry pg = new PathGeometry();
                            pg.Figures.Add(pf);
                            path.Data = pg;
                            path.StrokeThickness = this.StrokeThickness;
                            line.StrokeThickness = 0;
                            break;
                        }
                    default:
                        {
                            if (this.Dependencies != null && !this.Dependencies.Any(f => f is ImmovablePoint))
                            {
                                line.StrokeThickness = this.StrokeThickness;
                            }
                            path.StrokeThickness = 0;
                            break;
                        }
                }
            }
            else if (PathType == PathType.Curve)
            {
                Pointline = ToPhysical(OnScreenCoordinates);
                PathFigure pf = DrawCurveLine(Pointline.P1, Pointline.P2, 20d, 11, 11);
                PathGeometry pg = new PathGeometry();
                pg.Figures.Add(pf);
                path.Data = pg;
                if (this.Dependencies != null && !this.Dependencies.Any(f => f is ImmovablePoint))
                {
                    path.StrokeThickness = StrokeThickness;
                }
                line.StrokeThickness = 0;
            }

            if (this.Dependencies != null && this.Dependencies.Count() > 1)
            {
                if (this.Dependencies.ElementAt(1).Dependents.Count == 2 || this.Dependencies.ElementAt(1).Dependents.Count == 1 || this.Dependencies.ElementAt(1) is Game.Zone || this.Dependencies.ElementAt(1) is Game.PBPlayer)
                {
                    cap.Fill = Brushes.White;
                    PointPair line = OnScreenCoordinates;
                    Point p1 = line.P1;
                    Point p2 = line.P2;

                    double angle = System.Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
                    bool blockPlayer = false;

                    if (this.Dependencies.ElementAt(1) is Game.PBPlayer)
                    {
                        blockPlayer = true;

                        double logicalSpace = CoordinateSystem.LogicalPlayerSize + 0.2;

                        if (p2.X < p1.X)
                        {
                            p2.X += logicalSpace / 2 * System.Math.Cos(angle);
                            p2.Y += logicalSpace / 2 * System.Math.Sin(angle);
                        }
                        else
                        {
                            p2.X -= logicalSpace / 2 * System.Math.Cos(angle);
                            p2.Y -= logicalSpace / 2 * System.Math.Sin(angle);
                        }
                    }

                    switch (CapType)
                    {
                        case CapType.Arrow:
                            {
                                double d = p1.Distance(p2);
                                double arrowLength = ToLogical(16);
                                double arrowWidth = 1.0 / 3;
                                Point triangleBase = new Point(
                                    p2.X + (p1.X - p2.X) * arrowLength / d,
                                    p2.Y + (p1.Y - p2.Y) * arrowLength / d);

                                cap.Points = new System.Windows.Media.PointCollection()
                            {
                                ToPhysical(p2),
                                ToPhysical(new Point(
                                    triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth)),
                                ToPhysical(new Point(
                                    triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth)),
                            };
                                break;
                            }
                        case CapType.Block:
                            {
                                Shape.Set(ToPhysical(new PointPair(p1, p2)));    //04-01-2010 scott

                                double d = p1.Distance(p2);
                                double arrowLength = ToLogical(2);
                                double arrowWidth = ToPhysical(0.1);

                                Point triangleBase = new Point(
                                    p2.X + (p1.X - p2.X) * arrowLength / d,
                                    p2.Y + (p1.Y - p2.Y) * arrowLength / d);

                                double angle2 = Math.PI / 4;
                                double len2 = ToPhysical(0.005);

                                double angle3 = angle - angle2;
                                double angle4 = angle2 + angle;
                                int i = 1;
                                if (p2.X < p1.X)
                                {
                                    i = -1;
                                }

                                cap.Points = new System.Windows.Media.PointCollection();
                                cap.Points.Add(ToPhysical(new Point(
                                    p2.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    p2.Y + (triangleBase.X - p2.X) * arrowWidth)));

                                if (blockPlayer)
                                {
                                    cap.Points.Add(ToPhysical(new Point(
                                    (p2.X + (p2.Y - triangleBase.Y) * arrowWidth) + i * System.Math.Cos(angle3) * len2,
                                    (p2.Y + (triangleBase.X - p2.X) * arrowWidth) + i * System.Math.Sin(angle3) * len2)));

                                    cap.Points.Add(ToPhysical(new Point(
                                    (triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth) + i * System.Math.Cos(angle3) * len2,
                                    (triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth) + i * System.Math.Sin(angle3) * len2)));
                                }

                                cap.Points.Add(ToPhysical(new Point(
                                    triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth)));
                                cap.Points.Add(ToPhysical(new Point(
                                    triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth)));

                                if (blockPlayer)
                                {
                                    cap.Points.Add(ToPhysical(new Point(
                                    (triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth) + i * System.Math.Cos(angle4) * len2,
                                    (triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth) + i * System.Math.Sin(angle4) * len2)));

                                    cap.Points.Add(ToPhysical(new Point(
                                    (p2.X + (triangleBase.Y - p2.Y) * arrowWidth) + i * System.Math.Cos(angle4) * len2,
                                    (p2.Y + (p2.X - triangleBase.X) * arrowWidth) + i * System.Math.Sin(angle4) * len2)));
                                }

                                cap.Points.Add(ToPhysical(new Point(
                                    p2.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    p2.Y + (p2.X - triangleBase.X) * arrowWidth)));
                                break;
                            }
                        case CapType.BlockArea:
                            {
                                double d = p1.Distance(p2);
                                double arrowLength = ToLogical(3);
                                double arrowWidth = 3;
                                Point triangleBase = new Point(
                                    p2.X + (p1.X - p2.X) * arrowLength / d,
                                    p2.Y + (p1.Y - p2.Y) * arrowLength / d);


                                cap.Points = new System.Windows.Media.PointCollection()
                            {
                                ToPhysical(new Point(
                                    p2.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    p2.Y + (triangleBase.X - p2.X) * arrowWidth)),
                                ToPhysical(new Point(
                                    triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth)),
                                ToPhysical(new Point(
                                    triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth)),
                                ToPhysical(new Point(
                                    p2.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    p2.Y + (p2.X - triangleBase.X) * arrowWidth)),
                            };
                                break;
                            }
                        case CapType.BlockPerson:
                            {
                                Shape.Set(ToPhysical(new PointPair(p1, p2)));    //04-01-2010 scott

                                double d = p1.Distance(p2);
                                double arrowLength = ToLogical(3);
                                double arrowWidth = ToPhysical(0.1);

                                Point triangleBase = new Point(
                                    p2.X + (p1.X - p2.X) * arrowLength / d,
                                    p2.Y + (p1.Y - p2.Y) * arrowLength / d);

                                double angle2 = Math.PI / 4;
                                double len2 = ToPhysical(0.005);

                                double angle3 = angle - angle2;
                                double angle4 = angle2 + angle;
                                int i = 1;
                                if (p2.X < p1.X)
                                {
                                    i = -1;
                                }

                                cap.Points = new System.Windows.Media.PointCollection();
                                cap.Points.Add(ToPhysical(new Point(
                                    p2.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    p2.Y + (triangleBase.X - p2.X) * arrowWidth)));


                                cap.Points.Add(ToPhysical(new Point(
                                (p2.X + (p2.Y - triangleBase.Y) * arrowWidth) + i * System.Math.Cos(angle3) * len2,
                                (p2.Y + (triangleBase.X - p2.X) * arrowWidth) + i * System.Math.Sin(angle3) * len2)));

                                cap.Points.Add(ToPhysical(new Point(
                                (triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth) + i * System.Math.Cos(angle3) * len2,
                                (triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth) + i * System.Math.Sin(angle3) * len2)));


                                cap.Points.Add(ToPhysical(new Point(
                                    triangleBase.X + (p2.Y - triangleBase.Y) * arrowWidth,
                                    triangleBase.Y + (triangleBase.X - p2.X) * arrowWidth)));
                                cap.Points.Add(ToPhysical(new Point(
                                    triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth)));

                                cap.Points.Add(ToPhysical(new Point(
                                (triangleBase.X + (triangleBase.Y - p2.Y) * arrowWidth) + i * System.Math.Cos(angle4) * len2,
                                (triangleBase.Y + (p2.X - triangleBase.X) * arrowWidth) + i * System.Math.Sin(angle4) * len2)));

                                cap.Points.Add(ToPhysical(new Point(
                                (p2.X + (triangleBase.Y - p2.Y) * arrowWidth) + i * System.Math.Cos(angle4) * len2,
                                (p2.Y + (p2.X - triangleBase.X) * arrowWidth) + i * System.Math.Sin(angle4) * len2)));

                                cap.Points.Add(ToPhysical(new Point(
                                    p2.X + (triangleBase.Y - p2.Y) * arrowWidth,
                                    p2.Y + (p2.X - triangleBase.X) * arrowWidth)));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                }
                else
                {
                    cap.Fill = Brushes.Transparent;
                }

                DoubleCollection dc = new DoubleCollection();
                switch (DashType)
                {
                    case DashType.Solid:
                        break;
                    case DashType.Dashed:
                        dc.Add(3);
                        dc.Add(1);
                        break;
                    case DashType.Dotted:
                        dc.Add(1);
                        dc.Add(1);
                        break;
                }
                Shape.StrokeDashArray = dc;
                path.StrokeDashArray = dc;

                if (Dependencies != null && Dependencies.Any(f => f is ImmovablePoint))
                {
                    Shape.StrokeThickness = 0.5;
                    path.StrokeThickness = 0;
                }

                UpdateColor();
            }
        }
        private double GetRadian(Point sp, Point ep)
        {
            double x = ep.X - sp.X;
            double y = ep.Y - sp.Y;
            if (x == 0 && y > 0)
                return Math.PI / 2;
            else if (x == 0 && y < 0)
                return Math.PI * 1.5;
            else if (x > 0 && y == 0)
                return 0;
            else if (x == 0 && y == 0)
                return 0;
            else if (x > 0 && y > 0)
                return System.Math.Atan(y / x);
            else if (x < 0 && y < 0)
                return System.Math.Atan(y / x) + Math.PI;
            else if (x < 0 && y > 0)
                return System.Math.Atan(y / x) + Math.PI;
            else if (x > 0 && y < 0)
                return System.Math.Atan(y / x) + 2 * Math.PI;
            else
                return System.Math.Atan(y / x);
        }

        public static double GetLength(Point sp, Point ep)
        {
            return System.Math.Sqrt((sp.X - ep.X) * (sp.X - ep.X) + (sp.Y - ep.Y) * (sp.Y - ep.Y));
        }

        private void binTree(Point LStart, Point Lend, double length, double angle, double Langle, double linelongth)
        {
            Point pEnd;

            double sin = System.Math.Sin(Langle + angle);
            double cos = System.Math.Cos(Langle + angle);

            pEnd = new Point(LStart.X + length * cos, LStart.Y - length * sin);

            LineGeometry l = new LineGeometry();
            l.StartPoint = LStart;
            l.EndPoint = pEnd;
            geo.Children.Add(l);
            if (linelongth - geo.Children.Count * length * System.Math.Cos(angle) <= length * System.Math.Cos(angle))
            {
                LineGeometry lg = new LineGeometry();
                lg.StartPoint = pEnd;
                lg.EndPoint = Pointline.P2;

                geo.Children.Add(lg);
                return;
            }
            binTree(pEnd, Lend, length, -angle, Langle, linelongth);
        }

        private PathFigure DrawWave(Point startPoint, Point endpoint, double waveWith, double xRadius, double yRadius)
        {
            double PILine = GetRadian(startPoint, endpoint);

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPoint;
            int sum = Convert.ToInt32(GetLength(startPoint, endpoint) / waveWith);
            double leavelength = GetLength(startPoint, endpoint) - sum * waveWith;

            for (int i = 1; i <= sum; i++)
            {
                ArcSegment arcSegment = new ArcSegment();
                arcSegment.Point = new Point(startPoint.X + waveWith * i * System.Math.Cos(PILine), startPoint.Y + waveWith * i * System.Math.Sin(PILine));
                arcSegment.Size = new Size(xRadius, yRadius);
                pathFigure.Segments.Add(arcSegment);
            }
            ArcSegment arcsegment = new ArcSegment();
            arcsegment.Point = endpoint;
            arcsegment.Size = new Size(xRadius, yRadius);
            pathFigure.Segments.Add(arcsegment);

            return pathFigure;
        }

        private PathFigure DrawCurveLine(Point startPoint, Point endpoint, double waveWith, double xRadius, double yRadius)
        {
            double PILine = GetRadian(startPoint, endpoint);

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPoint;

            Point pt = GetCurvePoint();

            BezierSegment bezierSegment = new BezierSegment();
            bezierSegment.Point1 = startPoint;
            bezierSegment.Point2 = pt;
            bezierSegment.Point3 = endpoint;
            pathFigure.Segments.Add(bezierSegment);

            return pathFigure;
        }

        private Point GetCurvePoint()
        {
            Pointline = ToPhysical(Coordinates);

            double angle = System.Math.Atan((Pointline.P2.Y - Pointline.P1.Y) / (Pointline.P2.X - Pointline.P1.X));

            Point pt = new Point(Pointline.Midpoint.X + 10 * System.Math.Sin(angle), Pointline.Midpoint.Y + 10 * System.Math.Cos(angle));

            return pt;
        }

        public override void OnAddingToCanvas(System.Windows.Controls.Canvas newContainer)
        {
            base.OnAddingToCanvas(newContainer);

            newContainer.Children.Add(cap);
            newContainer.Children.Add(path);
        }

        public override void OnRemovingFromCanvas(System.Windows.Controls.Canvas leavingContainer)
        {
            base.OnRemovingFromCanvas(leavingContainer);

            leavingContainer.Children.Remove(cap);
            leavingContainer.Children.Remove(path);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("LineType", LineType.ToString());
            writer.WriteAttributeString("DashType", DashType.ToString());
            writer.WriteAttributeString("Type", CapType.ToString());
            writer.WriteAttributeDouble("StrokeThickness", StrokeThickness);
            writer.WriteAttributeString("R", StrokeColor.R.ToString());
            writer.WriteAttributeString("G", StrokeColor.G.ToString());
            writer.WriteAttributeString("B", StrokeColor.B.ToString());
        }

        public override void ReadXml(XElement element)
        {
            base.ReadXml(element);
            if (element.Attribute("LineType") != null)
            {
                LineType = (LineType)Enum.Parse(typeof(LineType), element.ReadString("LineType"));
            }
            if (element.Attribute("DashType") != null)
            {
                DashType = (DashType)Enum.Parse(typeof(DashType), element.ReadString("DashType"));
            }
            if (element.Attribute("Type") != null)
            {
                CapType = (CapType)Enum.Parse(typeof(CapType), element.ReadString("Type"));
            }
            if (element.Attribute("R") != null)
            {
                StrokeColor = System.Windows.Media.Color.FromRgb(byte.Parse(element.Attribute("R").Value), byte.Parse(element.Attribute("G").Value), byte.Parse(element.Attribute("B").Value));
            }
            if (element.Attribute("StrokeThickness") != null)
            {
                StrokeThickness = element.ReadDouble("StrokeThickness");
            }
        }

        // 11-18-2010 Scott
        protected override void UpdateShapeAppearance()
        {
            //base.UpdateShapeAppearance();
        }
    }
}
