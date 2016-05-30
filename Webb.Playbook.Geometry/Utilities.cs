using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

using M = System.Math;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry
{
    public enum AdvLineEndType  // 08-09-2010 Scott
    {
        stylePlain = 0x00,
        styleArrow = 0x01,
        styleArrowHollow = 0x11,
        styleArrowBarbed = 0x21,
        styleBlock = 0x02,
        styleBlockLeft = 0x12,
        styleBlockRight = 0x22,
        styleBlockSquare = 0x04
    }

    public enum AdvPenStyle
    {
        Solid = 0,
        Dash = 1,
        Dot = 2,
        DashDot = 3,
    }

    public struct PointPair
    {
        public PointPair(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Point P1;
        public Point P2;

        public bool Contains(Point point)
        {
            return point.X >= P1.X
                && point.X <= P2.X
                && point.Y >= P1.Y
                && point.Y <= P2.Y;
        }

        public bool ContainsInner(Point point)
        {
            return point.X > P1.X
                && point.X < P2.X
                && point.Y > P1.Y
                && point.Y < P2.Y;
        }

        public PointPair Reverse
        {
            get
            {
                return new PointPair(P2, P1);
            }
        }

        public PointPair GetBoundingRect()
        {
            PointPair result = this;
            if (result.P1.X > result.P2.X)
            {
                var t = result.P1.X;
                result.P1.X = result.P2.X;
                result.P2.X = t;
            }
            if (result.P1.Y > result.P2.Y)
            {
                var t = result.P1.Y;
                result.P1.Y = result.P2.Y;
                result.P2.Y = t;
            }
            return result;
        }

        public double Length
        {
            get
            {
                return P1.Distance(P2);
            }
        }

        public Point Midpoint
        {
            get
            {
                return new Point((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
            }
        }

        public override string ToString()
        {
            return string.Format("({0};{1})-({2};{3})", P1.X, P1.Y, P2.X, P2.Y);
        }

        public PointPair Inflate(double size)
        {
            return new PointPair(P1.Offset(-size), P2.Offset(size));
        }
    }

    public static class Tuple
    {
        public static Tuple<TP1, TP2> Create<TP1, TP2>(TP1 p1, TP2 p2)
            where TP1 : IEquatable<TP1>
            where TP2 : IEquatable<TP2>
        {
            return new Tuple<TP1, TP2>(p1, p2);
        }

        public static Tuple<TP1, TP2, TP3> Create<TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3)
        {
            return new Tuple<TP1, TP2, TP3>(p1, p2, p3);
        }
    }

    public struct Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    {
        public Tuple(T1 item1, T2 item2)
            : this()
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public bool Equals(Tuple<T1, T2> other)
        {
            return Item1.Equals(other.Item1) && Item2.Equals(other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tuple<T1, T2>))
            {
                return false;
            }
            return Equals((Tuple<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + Item1.ToString() + "," + Item2.ToString() + ")";
        }
    }

    public struct Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
    {
        public Tuple(T1 item1, T2 item2, T3 item3)
            : this()
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public bool Equals(Tuple<T1, T2, T3> other)
        {
            return Item1.Equals(other.Item1)
                && Item2.Equals(other.Item2)
                && Item3.Equals(other.Item3);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tuple<T1, T2, T3>))
            {
                return false;
            }
            return Equals((Tuple<T1, T2, T3>)obj);
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode()
                ^ Item2.GetHashCode()
                ^ Item3.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + Item1.ToString() + "," + Item2.ToString() + "," + Item3.ToString() + ")";
        }
    }

    public static class Extensions
    {
        // 08-30-2011 Scott
        public static double ToDouble(this string str, double defaultValue)
        {
            double ret = 0;

            try
            {
                ret = double.Parse(str);
            }
            catch
            {
                ret = defaultValue;
            }

            return ret;
        }

        public static System.Drawing.Color ToNet2Color(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public static int ToRgbInt(this System.Windows.Media.Color color) // 08-09-2010 Scott
        {
            int rgb = color.B << 16;
            rgb += color.G << 8;
            rgb += color.R;
            return rgb;
        }

        public static void WriteElementColor(this System.Xml.XmlWriter writer, string elementName, System.Windows.Media.Color color)
        {
            writer.WriteStartElement("Color");
            writer.WriteAttributeString("Name", elementName);
            writer.WriteAttributeString("R", color.R.ToString());
            writer.WriteAttributeString("G", color.G.ToString());
            writer.WriteAttributeString("B", color.B.ToString());
            writer.WriteEndElement();
        }

        public static void WriteElementColor(this System.Xml.Linq.XElement element, string elementName, System.Windows.Media.Color color)
        {
            XElement elemColor = new XElement("Color");

            elemColor.Add(new XAttribute("Name", elementName));
            elemColor.Add(new XAttribute("R", color.R.ToString()));
            elemColor.Add(new XAttribute("G", color.G.ToString()));
            elemColor.Add(new XAttribute("B", color.B.ToString()));

            element.Add(elemColor);
        }

        public static System.Windows.Media.Color ReadElementColor(this XElement element, string elementName)
        {
            System.Windows.Media.Color color = System.Windows.Media.Colors.Transparent;

            XElement elem = element.GetChildElement(elementName);

            color = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("R").Value), byte.Parse(elem.Attribute("G").Value), byte.Parse(elem.Attribute("B").Value));

            return color;
        }

        public static XElement GetChildElement(this XElement element, string elementName)
        {
            XElement elem = element.Elements().Single(f =>
                    f.Attribute("Name") != null && f.Attribute("Name").Value.Equals(elementName));

            return elem;
        }

        public static void WriteAttributeDouble(this System.Xml.XmlWriter writer, string attributeName, double value)
        {
            writer.WriteAttributeString(attributeName, value.ToStringInvariant());
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            items.ForEach(i => collection.Add(i));
        }

        public static PointPair GetSegment(this IList<Point> points, int startIndex)
        {
            return new PointPair(points[startIndex], points[(startIndex + 1) % points.Count]);
        }

        public static PointPair GetPreviousSegment(this IList<Point> points, int startIndex)
        {
            return new PointPair(points[startIndex > 0 ? startIndex - 1 : points.Count - 1], points[startIndex]);
        }

        public static int RotateNext(this int index, int count)
        {
            return (index + 1) % count;
        }

        public static int RotatePrevious(this int index, int count)
        {
            return index > 0 ? index - 1 : count - 1;
        }

        public static int RotatePrevious(this int index, int count, int steps)
        {
            int result = index - steps;
            if (result < 0)
            {
                result += count;
            }
            return result;
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static IEnumerable<int> Rotate(this int startIndex, int count)
        {
            while (true)
            {
                startIndex = startIndex.RotateNext(count);
                yield return startIndex;
            }
        }

        public static void SetZIndex<TShape>(this ShapeBase<TShape> shape, ZOrder typeOfLayer)
            where TShape : UIElement
        {
            shape.ZIndex = (int)typeOfLayer;
            Canvas.SetZIndex(shape.Shape, (int)typeOfLayer);
        }

        public static void CenterAt(this FrameworkElement element, Point center)
        {
            CenterAt(element, center.X, center.Y);
        }

        public static void CenterAt(this FrameworkElement element, double x, double y)
        {
            MoveTo(element, x - element.Width / 2, y - element.Height / 2);
        }

        public static void MoveTo(this UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        public static void MoveTo(this UIElement element, Point position)
        {
            MoveTo(element, position.X, position.Y);
        }

        public static void MoveOffset(this UIElement element, double xOffset, double yOffset)
        {
            if (element == null || double.IsNaN(xOffset) || double.IsNaN(yOffset))
            {
                return;
            }
            var coordinates = element.GetCoordinates();
            Canvas.SetLeft(element, coordinates.X + xOffset);
            Canvas.SetTop(element, coordinates.Y + yOffset);
        }

        public static void Move(
            this Line line,
            double x1,
            double y1,
            double x2,
            double y2,
            CoordinateSystem coordinateSystem)
        {
            var p1 = coordinateSystem.ToPhysical(new Point(x1, y1));
            var p2 = coordinateSystem.ToPhysical(new Point(x2, y2));
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
        }

        public static Point GetCoordinates(this UIElement element)
        {
            return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        }

        public static IEnumerable<T> AsEnumerable<T>(this T singleElement)
        {
            yield return singleElement;
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T oldItem, T newItem)
            where T : IEquatable<T>
        {
            foreach (var item in source)
            {
                if (item.Equals(oldItem))
                {
                    yield return newItem;
                }
                else
                {
                    yield return item;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<Point> ToPoints(this IEnumerable<IFigure> figures)
        {
            return figures
                .OfType<IPoint>()
                .Select(p => p.Coordinates);
        }

        public static PointCollection ToPointCollection(this IEnumerable<Point> points)
        {
            var result = new PointCollection();
            points.ForEach(result.Add);
            return result;
        }

        public static IEnumerable<Point> ToLogical(this PointCollection points, CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.ToLogical(points);
        }

        public static Point SetX(this Point p, double x)
        {
            return new Point(x, p.Y);
        }

        public static Point SetY(this Point p, double y)
        {
            return new Point(p.X, y);
        }

        public static Color ToColor(this string s)
        {
            if (s.IsEmpty())
            {
                return Colors.Black;
            }
            if (s.Length == 7)
            {
                return Color.FromArgb(
                    255,
                    Convert.ToByte(s.Substring(1, 2), 16),
                    Convert.ToByte(s.Substring(3, 2), 16),
                    Convert.ToByte(s.Substring(5, 2), 16));
            }
            if (s.Length == 6)
            {
                return Color.FromArgb(
                    255,
                    Convert.ToByte(s.Substring(0, 2), 16),
                    Convert.ToByte(s.Substring(2, 2), 16),
                    Convert.ToByte(s.Substring(4, 2), 16));
            }
            if (s.Length == 8)
            {
                return Color.FromArgb(
                    Convert.ToByte(s.Substring(0, 2), 16),
                    Convert.ToByte(s.Substring(2, 2), 16),
                    Convert.ToByte(s.Substring(4, 2), 16),
                    Convert.ToByte(s.Substring(6, 2), 16));
            }
            if (s.Length == 9)
            {
                return Color.FromArgb(
                    Convert.ToByte(s.Substring(1, 2), 16),
                    Convert.ToByte(s.Substring(3, 2), 16),
                    Convert.ToByte(s.Substring(5, 2), 16),
                    Convert.ToByte(s.Substring(7, 2), 16));
            }
            return Colors.Black;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsEmpty<T>(this IList<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool HasAttribute<T>(
            this MemberInfo attributeHost)
            where T : Attribute
        {
            return attributeHost.GetAttribute<T>() != null;
        }

        public static T GetAttribute<T>(
            this MemberInfo attributeHost)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(attributeHost, typeof(T));
        }

        public static bool HasInterface<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static Visibility ToVisibility(this bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Returns physical coordinates
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static PointPair GetBorderRectangle(this FrameworkElement canvas)
        {
            return new PointPair()
            {
                P1 = { X = 0, Y = 0 },
                P2 = { X = canvas.ActualWidth, Y = canvas.ActualHeight }
            };
        }

        public static void Set(this System.Windows.Shapes.Shape shape, Point coordinates)
        {

        }

        public static void Set(this System.Windows.Shapes.Line line, PointPair coordinates)
        {
            line.X1 = coordinates.P1.X == double.NaN ? 0 : coordinates.P1.X;
            line.Y1 = coordinates.P1.Y == double.NaN ? 0 : coordinates.P1.Y;
            line.X2 = coordinates.P2.X == double.NaN ? 0 : coordinates.P2.X;
            line.Y2 = coordinates.P2.Y == double.NaN ? 0 : coordinates.P2.Y;
        }

        public static string ToStringInvariant(this double number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        public static double ReadDouble(this XElement element, string attributeName)
        {
            double result = 0;
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                double.TryParse(
                    attribute.Value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result);
            }
            return result;
        }

        public static string ReadString(this XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                return attribute.Value;
            }
            return null;
        }
    }

    public static class PointExtensions
    {
        public static double Distance(this Point p1, Point p2)
        {
            return (
                  (p1.X - p2.X).Sqr()
                + (p1.Y - p2.Y).Sqr()).SquareRoot();
        }

        public static double AngleTo(this Point center, Point point)
        {
            return Math.OAngle(center.Plus(new Point(10, 0)), center, point);
        }

        public static Point Reflect(this Point point, Point center)
        {
            return new Point(2 * center.X - point.X, 2 * center.Y - point.Y);
        }

        public static double Length(this Point p)
        {
            return p.SumOfSquares().SquareRoot();
        }

        public static Point Scale(this Point p, double scaleFactor)
        {
            return new Point(p.X * scaleFactor, p.Y * scaleFactor);
        }

        public static Point SnapToIntegers(this Point p)
        {
            return new Point(M.Ceiling(p.X), M.Ceiling(p.Y));
        }

        public static Point TrimToMaxLength(this Point p, double maxLength)
        {
            var length = p.Length();
            if (maxLength > 0 && length > maxLength)
            {
                var ratio = maxLength / length;
                return new Point(p.X * ratio, p.Y * ratio);
            }
            return p;
        }

        public static Point Minus(this Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        public static Point Minus(this Point point, double offset)
        {
            return new Point(point.X - offset, point.Y - offset);
        }

        public static Point Plus(this Point point, double offset)
        {
            return new Point(point.X + offset, point.Y + offset);
        }

        public static Point Minus(this Point point, Point other)
        {
            return new Point(point.X - other.X, point.Y - other.Y);
        }

        public static Point OffsetX(this Point point, double xOffset)
        {
            return new Point(point.X + xOffset, point.Y);
        }

        public static Point OffsetY(this Point point, double yOffset)
        {
            return new Point(point.X, point.Y + yOffset);
        }

        public static Point Offset(this Point point, double offset)
        {
            return new Point(point.X + offset, point.Y + offset);
        }

        public static Point Offset(this Point point, double xOffset, double yOffset)
        {
            return new Point(point.X + xOffset, point.Y + yOffset);
        }

        public static Point Plus(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static double SumOfSquares(this Point point)
        {
            return point.X.Sqr() + point.Y.Sqr();
        }

        public static bool Exists(this Point p)
        {
            return !double.IsNaN(p.X) && !double.IsNaN(p.Y);
        }

        public static bool IsValidPositiveValue(this double value)
        {
            return value.IsValidValue()
                && value > 0;
        }

        public static bool IsValidNonNegativeValue(this double value)
        {
            return value.IsValidValue()
                && value >= 0;
        }

        public static bool IsValidValue(this double value)
        {
            return !double.IsNaN(value)
                && !double.IsInfinity(value);
        }

        public static bool EqualsWithPrecision(this double value, double center)
        {
            return Math.Abs(value - center) < Math.Precision;
        }

        public static bool IsWithinEpsilonTo(this double value, double center)
        {
            return Math.Abs(value - center) < Math.Epsilon;
        }

        public static bool EqualsWithPrecision(this Point point, Point other)
        {
            return point.X.EqualsWithPrecision(other.X) && point.Y.EqualsWithPrecision(other.Y);
        }

        public static bool IsWithinEpsilon(this double value)
        {
            return Math.Abs(value) < Math.Epsilon;
        }

        public static bool IsWithinTolerance(this double value)
        {
            return Math.Abs(value) < Math.CursorTolerance;
        }

        public static bool IsEqual(this Point point, Point other)
        {
            return point.Distance(other).IsWithinEpsilon();
        }

        public static double RoundToEpsilon(this double point)
        {
            return point.Round(6);
        }

        public static Point RoundToEpsilon(this Point point)
        {
            return new Point(point.X.RoundToEpsilon(), point.Y.RoundToEpsilon());
        }

        public static IEnumerable<Point> RoundToEpsilon(this IEnumerable<Point> list)
        {
            return list.Select(p => p.RoundToEpsilon());
        }
    }

    public static class Math
    {
        public static Point InfinitePoint
        {
            get
            {
                return new Point(double.NaN, double.NaN);
            }
        }

        public static PointPair InfinitePointPair
        {
            get
            {
                return new PointPair() { P1 = Math.InfinitePoint, P2 = Math.InfinitePoint };
            }
        }


        public static List<Point> GetIntersections(IList<Point> polygon, PointPair segment)
        {
            List<Point> result = new List<Point>();
            int n = polygon.Count;
            for (int i = 0; i < n; i++)
            {
                PointPair side = new PointPair(polygon[i], polygon[i.RotateNext(n)]);
                Point intersection = GetIntersectionOfSegments(side, segment);
                if (intersection.Exists()
                    && !intersection.IsEqual(polygon[i.RotateNext(n)])
                    && !intersection.IsEqual(segment.P1)
                    && !intersection.IsEqual(segment.P2))
                {
                    result.Add(intersection);
                }
            }
            return result;
        }

        public static double PolylineLength(this IList<Point> polyline)
        {
            double sum = 0;
            for (int i = 0; i < polyline.Count - 1; i++)
            {
                sum += polyline[i].Distance(polyline[i + 1]);
            }
            return sum;
        }

        public static int Sign(this double num)
        {
            return M.Sign(num);
        }

        public static double Abs(this double num)
        {
            return M.Abs(num);
        }

        public static double Round(this double num, int fractionalDigits)
        {
            return M.Round(num, fractionalDigits);
        }

        private static double mCursorTolerance = 5;
        public static double CursorTolerance
        {
            get
            {
                return mCursorTolerance;
            }
            set
            {
                mCursorTolerance = value;
            }
        }

        public static Point ScalePointBetweenTwo(Point p1, Point p2, double ratio)
        {
            return new Point(
                p1.X + (p2.X - p1.X) * ratio,
                p1.Y + (p2.Y - p1.Y) * ratio);
        }

        public static double OAngle(Point firstPoint, Point vertex, Point secondPoint)
        {
            var a1 = GetAngle(vertex, firstPoint);
            var a2 = GetAngle(vertex, secondPoint);
            if (a2 < a1)
            {
                a2 = a2 + 2 * PI;
            }
            var result = a2 - a1;
            if (result > 2 * PI)
            {
                result -= 2 * PI;
            }
            return result;
        }

        public static double GetAngle(Point center, Point endPoint)
        {
            var result = M.Atan2(endPoint.Y - center.Y, endPoint.X - center.X);
            if (result < 0)
            {
                result += 2 * PI;
            }
            return result;
        }

        public static double VectorProduct(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
        }

        public static double PI
        {
            get
            {
                return M.PI;
            }
        }

        /// <summary>
        /// Square of a number (multiplied by itself)
        /// </summary>
        public static double Sqr(this double num)
        {
            return num * num;
        }

        /// <summary>
        /// Square root
        /// </summary>
        public static double SquareRoot(this double num)
        {
            return System.Math.Sqrt(System.Math.Abs(num));
        }

        public static void Swap<T>(ref T p1, ref T p2)
        {
            T temp = p1;
            p1 = p2;
            p2 = temp;
        }

        public static PointPair GetLineFromSegment(PointPair segment, PointPair borders)
        {
            PointPair result = new PointPair();

            if (segment.P1 == segment.P2)
            {
                result = segment;
            }
            else if (segment.P1.X == segment.P2.X)
            {
                result.P1.X = segment.P1.X;
                result.P2.X = segment.P1.X;
                result.P1.Y = borders.P1.Y;
                result.P2.Y = borders.P2.Y;
                if (segment.P1.Y > segment.P2.Y)
                {
                    result.P1.Y = borders.P2.Y;
                    result.P2.Y = borders.P1.Y;
                }
            }
            else if (segment.P1.Y == segment.P2.Y)
            {
                result.P1.X = borders.P1.X;
                result.P1.Y = segment.P1.Y;
                result.P2.X = borders.P2.X;
                result.P2.Y = segment.P1.Y;
                if (segment.P1.X > segment.P2.X)
                {
                    result.P1.X = borders.P2.X;
                    result.P2.X = borders.P1.X;
                }
            }
            else
            {
                var deltaX = segment.P2.X - segment.P1.X;
                var deltaY = segment.P2.Y - segment.P1.Y;
                var deltaXYRatio = deltaX / deltaY;
                var deltaYXRatio = deltaY / deltaX;

                result.P1.Y = deltaY > 0 ? borders.P1.Y : borders.P2.Y;
                result.P1.X = segment.P1.X + (result.P1.Y - segment.P1.Y) * deltaXYRatio;
                if (result.P1.X < borders.P1.X)
                {
                    result.P1.X = borders.P1.X;
                    result.P1.Y = segment.P1.Y + (result.P1.X - segment.P1.X) * deltaYXRatio;
                }
                else if (result.P1.X > borders.P2.X)
                {
                    result.P1.X = borders.P2.X;
                    result.P1.Y = segment.P1.Y + (result.P1.X - segment.P1.X) * deltaYXRatio;
                }

                result.P2.X = deltaX > 0 ? borders.P2.X : borders.P1.X;
                result.P2.Y = segment.P2.Y + (result.P2.X - segment.P2.X) * deltaYXRatio;
                if (result.P2.Y < borders.P1.Y)
                {
                    result.P2.Y = borders.P1.Y;
                    result.P2.X = segment.P2.X + (result.P2.Y - segment.P2.Y) * deltaXYRatio;
                }
                else if (result.P2.Y > borders.P2.Y)
                {
                    result.P2.Y = borders.P2.Y;
                    result.P2.X = segment.P2.X + (result.P2.Y - segment.P2.Y) * deltaXYRatio;
                }
            }

            return result;
        }

        public class ProjectionInfo
        {
            public Point Point { get; set; }
            public double Ratio { get; set; }
            public double DistanceToLine { get; set; }
        }

        public static ProjectionInfo GetProjection(Point point, PointPair line)
        {
            Point projectionPoint = GetProjectionPoint(point, line);
            ProjectionInfo result = new ProjectionInfo()
            {
                Point = projectionPoint,
                Ratio = GetProjectionRatio(line, projectionPoint),
                DistanceToLine = projectionPoint.Distance(point)
            };
            return result;
        }

        public static double GetProjectionRatio(PointPair line, Point projection)
        {
            var result = 0.0;
            if (line.P1.X != line.P2.X)
            {
                result = (projection.X - line.P1.X) / (line.P2.X - line.P1.X);
            }
            else if (line.P1.Y != line.P2.Y)
            {
                result = (projection.Y - line.P1.Y) / (line.P2.Y - line.P1.Y);
            }
            return result;
        }

        public static Point GetProjectionPoint(Point p, PointPair line)
        {
            Point result = new Point();

            if (line.P1.Y == line.P2.Y)
            {
                result.X = p.X;
                result.Y = line.P1.Y;
            }
            else if (line.P1.X == line.P2.X)
            {
                result.X = line.P1.X;
                result.Y = p.Y;
            }
            else
            {
                var a = p.Minus(line.P1).SumOfSquares();
                var b = p.Minus(line.P2).SumOfSquares();
                var c = line.P1.Minus(line.P2).SumOfSquares();

                if (c != 0)
                {
                    var m = (a + c - b) / (2 * c);
                    result = ScalePointBetweenTwo(line.P1, line.P2, m);
                }
                else
                {
                    result = line.P1;
                }
            }

            return result;
        }

        public static Point GetIntersectionOfSegments(PointPair segment1, PointPair segment2)
        {
            Point result = GetIntersectionOfLines(segment1, segment2);
            if (!result.Exists())
            {
                return result;
            }
            if (IsPointInSegmentInnerBoundingRect(segment1, result)
                && IsPointInSegmentInnerBoundingRect(segment2, result))
            {
                return result;
            }
            return Math.InfinitePoint;
        }

        public static bool IsPointInSegmentBoundingRect(PointPair segment, Point point)
        {
            var boundingRect = segment.GetBoundingRect().Inflate(0.00001);
            return boundingRect.Contains(point);
        }

        public static bool IsPointInSegmentInnerBoundingRect(PointPair segment, Point point)
        {
            segment = segment.GetBoundingRect();
            if (segment.P1.X.IsWithinEpsilonTo(segment.P2.X))
            {
                return point.X.EqualsWithPrecision(segment.P1.X)
                    && point.Y > segment.P1.Y
                    && point.Y < segment.P2.Y;
            }
            else if (segment.P1.Y.IsWithinEpsilonTo(segment.P2.Y))
            {
                return point.Y.EqualsWithPrecision(segment.P1.Y)
                    && point.X > segment.P1.X
                    && point.X < segment.P2.X;
            }
            return segment.ContainsInner(point);
        }

        public static Point GetIntersectionOfLines(PointPair line1, PointPair line2)
        {
            var a1 = line1.P2.Y - line1.P1.Y;
            var b1 = line1.P1.X - line1.P2.X;
            var c1 = line1.P2.X * line1.P1.Y - line1.P1.X * line1.P2.Y;

            var a2 = line2.P2.Y - line2.P1.Y;
            var b2 = line2.P1.X - line2.P2.X;
            var c2 = line2.P2.X * line2.P1.Y - line2.P1.X * line2.P2.Y;

            return SolveLinearSystem(a1, b1, c1, a2, b2, c2);
        }

        public static Point SolveLinearSystem(
            double a1, double b1, double c1,
            double a2, double b2, double c2)
        {
            var d = a1 * b2 - a2 * b1;
            if (d == 0)
            {
                return Math.InfinitePoint;
            }

            var dx = b1 * c2 - b2 * c1;
            var dy = a2 * c1 - a1 * c2;
            return new Point(dx / d, dy / d);
        }

        public static ReadOnlyCollection<double> SolveSquareEquation(
            double a,
            double b,
            double c)
        {
            var result = new List<double>(2);
            var d = b * b - 4 * a * c;
            if (a == 0)
            {
                return new ReadOnlyCollection<double>(result);
            }

            if (d > 0)
            {
                d = d.SquareRoot();
                a *= 2;
                result.Add((-b - d) / a);
                result.Add((d - b) / a);
            }
            else if (d == 0)
            {
                result.Add(-b / (2 * a));
            }
            return new ReadOnlyCollection<double>(result);
        }

        public static Point Midpoint(this IEnumerable<Point> points)
        {
            Point[] array = points.ToArray();
            if (array == null || array.Length == 0)
            {
                return new Point();
            }
            Point sum = new Point();
            foreach (var point in array)
            {
                sum = sum.Plus(point);
            }
            return sum.Scale(1.0 / array.Length);
        }

        public static Point Midpoint(this IEnumerable<IPoint> points)
        {
            return points.Select(p => p.Coordinates).Midpoint();
        }

        public static Point Midpoint(params Point[] points)
        {
            return points.Midpoint();
        }

        //public static double Area(this IEnumerable<Point> vertices, CoordinateSystem reference)
        //{
        //    return reference.ToLogical(vertices).Area();
        //}

        public static double Area(this IEnumerable<Point> vertices)
        {
            var points = vertices.ToArray();
            if (points.Length < 2)
            {
                return 0;
            }
            else if (points.Length == 2)
            {
                // if only two points are given, assume area of a circle
                // with center in the first point and passing through the second one
                return points[0].Distance(points[1]).Sqr() * PI;
            }
            else
            {
                // general polygon area
                double sum = 0;
                for (int i = 0; i < points.Length - 1; i++)
                {
                    sum += (points[i + 1].X - points[i].X) * (points[i + 1].Y + points[i].Y) / 2;
                }
                var lastIndex = points.Length - 1;
                sum += (points[0].X - points[lastIndex].X) * (points[0].Y + points[lastIndex].Y) / 2;
                return sum.Abs();
            }
        }

        public static double Epsilon
        {
            get
            {
                return 0.00000000001;
            }
        }

        public static double Precision
        {
            get
            {
                return 0.00000000001;
            }
        }

        public static PointPair GetIntersectionOfCircleAndLine(
            Point center,
            double radius,
            PointPair line)
        {
            var result = Math.InfinitePointPair;
            var p = GetProjectionPoint(center, line);
            var h = center.Distance(p).Round(4);
            radius = radius.Round(4);

            if ((h - radius).IsWithinEpsilon())
            {
                result.P1 = p;
                result.P2 = p;
            }
            else if (h > 0 && h < radius)
            {
                var s = ((radius - h) * (radius + h)).SquareRoot();
                s = s / line.Length;
                result.P1.X = p.X - (line.P2.X - line.P1.X) * s;
                result.P1.Y = p.Y - (line.P2.Y - line.P1.Y) * s;
                result.P2.X = 2 * p.X - result.P1.X;
                result.P2.Y = 2 * p.Y - result.P1.Y;
            }
            else if (h == 0)
            {
                var a = line.P1.Distance(line.P2);
                if (a != 0)
                {
                    var s = radius / a;
                    result.P1.X = center.X + (line.P2.X - line.P1.X) * s;
                    result.P1.Y = center.Y + (line.P2.Y - line.P1.Y) * s;
                    result.P2.X = 2 * center.X - result.P1.X;
                    result.P2.Y = 2 * center.Y - result.P1.Y;
                }
            }

            return result;
        }

        public static PointPair GetIntersectionOfCircles(
            Point center1,
            double radius1,
            Point center2,
            double radius2)
        {
            var result = Math.InfinitePointPair;
            var x1 = center1.X;
            var y1 = center1.Y;
            var x2 = center2.X;
            var y2 = center2.Y;

            var r3 = center1.Distance(center2);
            if (r3 == 0)
            {
                return result;
            }

            if (y1 == y2)
            {
                if (radius1 + radius1 > r3 + Epsilon
                 && radius1 + r3 > radius2 + Epsilon
                 && radius2 + r3 > radius1 + Epsilon
                 && x1 != x2)
                {
                    var x3 = (radius1.Sqr() + r3.Sqr() - radius2.Sqr()) / (2 * r3);
                    var tsqr = (radius1.Sqr() - x3.Sqr()).SquareRoot();
                    result.P1.X = x1 + (x2 - x1) * x3 / r3;
                    result.P1.Y = y1 - tsqr;
                    result.P2.X = result.P1.X;
                    result.P2.Y = y1 + tsqr;
                    if (x2 < x1)
                    {
                        var t = result.P1.Y;
                        result.P1.Y = result.P2.Y;
                        result.P2.Y = t;
                    }
                }
                else if ((radius1 + radius2 - r3).IsWithinEpsilon())
                {
                    result.P1.X = x1 + M.Sign(x2 - x1) * radius1;
                    result.P1.Y = y1;
                    result.P2 = result.P1;
                }
                else if ((radius1 + r3 - radius2).Abs() <= Epsilon
                    || (r3 + radius2 - radius1).Abs() <= Epsilon)
                {
                    result.P1.X = x1 + M.Sign(x2 - x1) * M.Sign(radius1 - radius2) * radius1;
                    result.P1.Y = y1;
                    result.P2 = result.P1;
                }
                return result;
            }

            if ((radius1 + radius2 - r3).Abs() <= Epsilon)
            {
                r3 = radius1 / r3;
                result.P1.X = x1 + (x2 - x1) * r3;
                result.P1.Y = y1 + (y2 - y1) * r3;
                result.P2 = result.P1;
                return result;
            }

            if ((radius1 + r3 - radius2).Abs() <= Epsilon
                || (radius2 + r3 - radius1).Abs() <= Epsilon)
            {
                r3 = radius1 / r3 * M.Sign(radius1 - radius2);
                result.P1.X = x1 + (x2 - x1) * r3;
                result.P1.Y = y1 + (y2 - y1) * r3;
                result.P2 = result.P1;
                return result;
            }

            var k = -(x2 - x1) / (y2 - y1);
            var b = ((radius1 - radius2) * (radius1 + radius2)
                    + (x2 - x1) * (x2 + x1)
                    + (y2 - y1) * (y2 + y1))
                / (2 * (y2 - y1));
            var ea = k * k + 1;
            var eb = 2 * (k * b - x1 - k * y1);
            var ec = x1.Sqr() + b.Sqr() - 2 * b * y1 + y1.Sqr() - radius1.Sqr();
            var roots = SolveSquareEquation(ea, eb, ec);
            if (roots == null || roots.Count != 2)
            {
                return result;
            }
            result.P1.X = roots[0];
            result.P1.Y = roots[0] * k + b;
            result.P2.X = roots[1];
            result.P2.Y = roots[1] * k + b;

            if (y2 > y1)
            {
                var t = result.P1;
                result.P1 = result.P2;
                result.P2 = t;
            }

            return result;
        }

        public static double Distance(double x0, double y0, double x1, double y1)
        {
            return ((x1 - x0).Sqr() + (y1 - y0).Sqr()).SquareRoot();
        }

        public static PointPair GetTangentPoints(Point outside, Point center, double radius)
        {
            var distance = outside.Distance(center);
            if (distance == 0 || distance < radius)
            {
                return Math.InfinitePointPair;
            }
            var angle = System.Math.Acos(radius / distance);
            var originalAngle = System.Math.Atan2(outside.Y - center.Y, outside.X - center.X);
            var point1 = RotatePoint(center, radius, originalAngle + angle);
            var point2 = RotatePoint(center, radius, originalAngle - angle);
            return new PointPair(point1, point2);
        }

        public static Point RotatePoint(Point center, double radius, double angle)
        {
            return new Point(center.X + radius * M.Cos(angle),
                center.Y + radius * M.Sin(angle));
        }
    }

    public static class IFigureListExtensions
    {
        public static IEnumerable<IFigure> GetAllFiguresRecursive(this IFigure rootFigure)
        {
            yield return rootFigure;
            IFigureList list = rootFigure as IFigureList;
            if (list != null)
            {
                foreach (var item in list)
                {
                    foreach (var recursive in item.GetAllFiguresRecursive())
                    {
                        yield return recursive;
                    }
                }
            }
        }
    }

    public static class IFigureExtensions
    {
        public static void RegisterWithDependencies(this IFigure figure)
        {
            if (figure == null || figure.Dependencies == null)
            {
                return;
            }
            foreach (var dependency in figure.Dependencies)
            {
                dependency.Dependents.Add(figure);
            }
        }

        public static void UnregisterFromDependencies(this IFigure figure)
        {
            if (figure == null || figure.Dependencies == null)
            {
                return;
            }
            foreach (var dependency in figure.Dependencies)
            {
                dependency.Dependents.Remove(figure);
            }
        }

        public static void RecalculateAllDependents(this IFigure figure)
        {
            DependencyAlgorithms
                .FindDescendants(f => f.Dependents, new IFigure[] { figure })
                .Reverse()
                .ForEach(f => f.RecalculateAndUpdateVisual());
        }

        public static void SubstituteWith(this IFigure figure, IFigure replacement)
        {
            List<IFigure> dependents = new List<IFigure>(figure.Dependents);
            foreach (var dependent in dependents)
            {
                dependent.ReplaceDependency(figure, replacement);
            }
            replacement.Dependents.AddRange(figure.Dependents.ToArray());
            figure.Dependents.Clear();
        }

        public static void ReplaceDependency(this IFigure figure, int index, IFigure newDependency)
        {
            List<IFigure> temp = new List<IFigure>(figure.Dependencies);
            if (index < 0 || index >= temp.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            IFigure oldDependency = temp[index];
            oldDependency.Dependents.Remove(figure);
            temp[index] = newDependency;
            newDependency.Dependents.Add(figure);
            figure.Dependencies = temp;
        }

        public static void ReplaceDependency(this IFigure figure, IFigure oldDependency, IFigure newDependency)
        {
            figure.Dependencies = figure.Dependencies.Replace(oldDependency, newDependency).ToArray();
        }

        public static void Move(this IEnumerable<IMovable> figures, Point offset)
        {
            foreach (var figure in figures)
            {
                figure.MoveTo(figure.Coordinates.Plus(offset));
            }
        }

        public static void RecalculateAndUpdateVisual(this IFigure figure)
        {
            figure.Recalculate();
            figure.UpdateVisual();
        }

        public static void CheckConsistency(this IFigureList list)
        {
            foreach (var figure in list)
            {
                if (figure.Dependencies != null)
                {
                    foreach (var dependency in figure.Dependencies)
                    {
                        //if (!list.Contains(dependency))
                        //{
                        //    throw new Exception();
                        //}

                        if (!dependency.Dependents.Contains(figure))
                        {
                            throw new Exception();
                        }
                    }
                }
                if (figure.Dependents != null)
                {
                    foreach (var dependent in figure.Dependents)
                    {
                        //if (!list.Contains(dependent))
                        //{
                        //    throw new Exception();
                        //}

                        if (!dependent.Dependencies.Contains(figure))
                        {
                            throw new Exception();
                        }
                    }
                }
            }
        }
    }
}
