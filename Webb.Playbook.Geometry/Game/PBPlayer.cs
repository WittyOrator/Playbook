using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Xml.Linq;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Data;

namespace Webb.Playbook.Geometry.Game
{
    public enum PlayerAppearance
    {
        Square,
        Circle,
        Triangle,
        Text,
    }

    public class DummyPlayer : PBPlayer
    {
        public override void MoveToCore(Point newLocation)
        {
            //base.MoveToCore(newLocation);
        }
    }

    public class PBPlayer : FreePoint
    {
        public Storyboard story;
        public Storyboard selectedStory;
        public static double Size = 30;

        public static int GetScoutTypeByName(string name)
        {
            if (name.StartsWith("QB")
                    || name.StartsWith("WR")
                    || name.StartsWith("RB")
                    || name.StartsWith("OL")
                    || name.StartsWith("TE"))
            {
                return 0;
            }
            return 1;
        }

        public int ScoutType
        {
            get
            {
                return GetScoutTypeByName(Name);
            }
        }

        public override System.Windows.Media.Brush SelectedBrush
        {
            get
            {
                if (selectedBrush == null)
                {
                    selectedBrush = System.Windows.Media.Brushes.Red;
                }
                return base.selectedBrush;
            }
        }

        private System.Collections.Specialized.StringCollection fakeArray;
        public System.Collections.Specialized.StringCollection FakeArray
        {
            get
            {
                if (fakeArray == null)
                {
                    fakeArray = new System.Collections.Specialized.StringCollection();
                }
                return fakeArray;
            }
        }

        private System.Collections.Specialized.StringCollection assignArray;
        public System.Collections.Specialized.StringCollection AssignArray
        {
            get
            {
                if (assignArray == null)
                {
                    assignArray = new System.Collections.Specialized.StringCollection();
                }
                return assignArray;
            }
        }

        private System.Collections.Specialized.StringCollection setArray;
        public System.Collections.Specialized.StringCollection SetArray
        {
            get
            {
                if (setArray == null)
                {
                    setArray = new System.Collections.Specialized.StringCollection();
                }
                return setArray;
            }
        }

        // 09-27-2010 Scott
        private string coachingPoints = string.Empty;
        public string CoachingPoints
        {
            get { return coachingPoints; }
            set { coachingPoints = value; }
        }

        private string manCoverage = string.Empty;
        public string ManCoverage
        {
            get { return manCoverage; }
            set { manCoverage = value; }
        }

        private string manPress = string.Empty;
        public string ManPress
        {
            get { return manPress; }
            set { manPress = value; }
        }

        private string qBMotion = string.Empty;
        public string QBMotion
        {
            get { return qBMotion; }
            set { qBMotion = value; }
        }

        private string stance = string.Empty;
        public string Stance
        {
            get { return stance; }
            set { stance = value; }
        }
        private string assignment = string.Empty;
        public string Assignment
        {
            get { return assignment; }
            set { assignment = value; }

        }
        private TextBlock textBlock;
        public TextBlock TextBlock
        {
            get
            {
                if (textBlock == null)
                {
                    textBlock = new TextBlock();
                    textBlock.Foreground = Brushes.Black;
                    Canvas.SetZIndex(textBlock, (int)ZOrder.Points);
                }
                return textBlock;
            }
            set { textBlock = value; }
        }

        public PBPlayer(string strName)
        {
            Name = strName;

            Appearance = PlayerAppearance.Circle;
            //Text = Name;

            StrokeColor = Colors.Black;
            FillColor = Colors.Gray;

            Shape = CreateShape();
            ZIndex = DefaultZOrder();
        }

        public PBPlayer()
        {
            Appearance = PlayerAppearance.Circle;
            //Text = Name;

            StrokeColor = Colors.Black;
            FillColor = Colors.Gray;

            Shape = CreateShape();
            ZIndex = DefaultZOrder();
        }

        public PBPlayer(PlayerAppearance appearance, string strName)
            : this()
        {
            Name = strName;

            Appearance = appearance;
            //Text = Name;

            StrokeColor = Colors.Black;
            FillColor = Colors.Gray;

            Shape = CreateShape();
            ZIndex = DefaultZOrder();
        }

        private Color strokeColor;
        public Color StrokeColor
        {
            get { return strokeColor; }
            set { strokeColor = value; }
        }

        private Color fillColor;
        public Color FillColor
        {
            get
            {
                if (EnableColor)
                {
                    return fillColor;
                }
                else
                {
                    return Colors.White;
                }

                // 10-20-2010 Remove by Scott
                //if (ScoutType == 0)
                //{
                //    return ColorSetting.Instance.OffensivePlayerColor;
                //}
                //else
                //{
                //    return ColorSetting.Instance.DefensivePlayerColor;
                //}
            }
            set
            {
                fillColor = value;
                //UpdateShapeAppearance();
            }
        }

        private Color textColor = Colors.Black;    // 11-16-2010 Scott
        public Color TextColor
        {
            get
            {
                if (EnableColor)
                {
                    return textColor;
                }
                else
                {
                    return Colors.Black;
                }
            }
            set
            {
                textColor = value;
            }
        }

        private bool textVisible = true;   // 11-16-2010 Scott
        public bool TextVisible
        {
            get
            {
                return textVisible;
            }
            set
            {
                textVisible = value;
            }
        }

        private double radius = 1.5;  // 11-16-2010 Scott
        public double Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }

        private bool dash = false;  // 11-16-2010 Scott
        public bool Dash
        {
            get
            {
                return dash;
            }
            set
            {
                dash = value;
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                TextBlock.Text = value;
            }
        }

        private PlayerAppearance appearance;
        public PlayerAppearance Appearance
        {
            get { return appearance; }
            set { appearance = value; }
        }

        private Shape shadow;
        public Shape Shadow
        {
            get
            {
                if (shadow == null)
                {
                    shadow = CreateShadow();
                }
                return shadow;
            }
            set
            {
                shadow = value;
            }
        }

        public Shape CreateShadow()
        {
            Shape shadow = new Ellipse();
            shadow.Visibility = Visibility.Visible;

            if (Appearance == PlayerAppearance.Circle)
            {

            }
            if (Appearance == PlayerAppearance.Square)
            {
                shadow = new Rectangle();
            }
            if (Appearance == PlayerAppearance.Triangle)
            {
                shadow = new Polygon();
                Point point1 = new Point(Size / 2, 0);
                Point point2 = new Point(0, Size);
                Point point3 = new Point(Size, Size);
                PointCollection myPointCollection = new PointCollection();
                myPointCollection.Add(point1);
                myPointCollection.Add(point2);
                myPointCollection.Add(point3);
                (shadow as Polygon).Points = myPointCollection;
            }
            if (Appearance == PlayerAppearance.Text)
            {
                shadow.Visibility = Visibility.Hidden;
            }
            shadow.CenterAt(new Point(20, 20));

            Canvas.SetZIndex(shadow, (int)ZOrder.Points);
            shadow.Fill = Brushes.Black;
            shadow.Opacity = 0.8;

            return shadow;
        }

        public void Shade()
        {
            Shadow.Visibility = Visibility.Hidden;
            if (ScoutType == 0 && this.Name.StartsWith("OL"))
            {
                Shadow.Width = Shape.Width;
                Shadow.Height = Shape.Height;
                Shadow.CenterAt(ToPhysical(Coordinates));

                bool bShadeLeft = false;
                bool bShadeRight = false;
                Shadow.Clip = null;

                if (Drawing.Figures.OfType<PBPlayer>().Any(p => p.Name.StartsWith("DL") && (p.Coordinates.X >= Coordinates.X + 0.45 && p.Coordinates.X <= Coordinates.X + 0.55)))
                {// shade right
                    bShadeRight = true;
                }

                if (Drawing.Figures.OfType<PBPlayer>().Any(p => p.Name.StartsWith("DL") && (p.Coordinates.X <= Coordinates.X - 0.45 && p.Coordinates.X >= Coordinates.X - 0.55)))
                {// shade left
                    bShadeLeft = true;
                }

                if (bShadeLeft && bShadeRight)
                {
                    Shadow.Visibility = Visibility.Visible;
                }
                else if (bShadeLeft)
                {
                    Shadow.Clip = new RectangleGeometry(new Rect(new Point(0, 0), new Size(Shape.Width / 2, Shape.Height)));
                    Shadow.Visibility = Visibility.Visible;
                }
                else if (bShadeRight)
                {
                    Shadow.Clip = new RectangleGeometry(new Rect(new Point(Shape.Width / 2, 0), new Size(Shape.Width / 2, Shape.Height)));
                    Shadow.Visibility = Visibility.Visible;
                }
                else
                {
                    Shadow.Visibility = Visibility.Hidden;
                }
            }
        }

        public override System.Windows.Shapes.Shape CreateShape()
        {
            System.Windows.Shapes.Shape shape = new Ellipse()
            {
                Width = Size,
                Height = Size,
                Stroke = new SolidColorBrush(StrokeColor),
                StrokeThickness = 1,
                Fill = new LinearGradientBrush(Colors.White, FillColor, 45),
                Visibility = Visibility.Visible,
            };

            if (Appearance == PlayerAppearance.Circle)
            {

            }
            if (Appearance == PlayerAppearance.Square)
            {
                shape = new Rectangle()
                {
                    Width = Size,
                    Height = Size,
                    Stroke = new SolidColorBrush(StrokeColor),
                    StrokeThickness = 1,
                    Fill = new LinearGradientBrush(Colors.White, FillColor, 45),
                };
            }
            if (Appearance == PlayerAppearance.Triangle)
            {
                Point point1 = new Point(Size / 2, 0);
                Point point2 = new Point(0, Size);
                Point point3 = new Point(Size, Size);
                PointCollection myPointCollection = new PointCollection();
                myPointCollection.Add(point1);
                myPointCollection.Add(point2);
                myPointCollection.Add(point3);

                shape = new Polygon()
                {
                    Width = Size,
                    Height = Size,
                    Stroke = new SolidColorBrush(StrokeColor),
                    StrokeThickness = 1,
                    Fill = new LinearGradientBrush(Colors.White, FillColor, 45),
                    Points = myPointCollection,
                };
            }
            if (Appearance == PlayerAppearance.Text)
            {
                shape.Visibility = Visibility.Hidden;
            }

            return shape;
        }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            double x = Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X;
            double y = Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y;
            var coordinates = Coordinates;
            coordinates.X = coordinates.X - x;
            coordinates.Y = coordinates.Y + y;
            writer.WriteAttributeDouble("X", -coordinates.X);
            writer.WriteAttributeDouble("Y", -coordinates.Y);
            writer.WriteAttributeString("Text", Text);
            writer.WriteAttributeString("Selected", Selected.ToString());
            writer.WriteAttributeString("Stance", Stance);
            writer.WriteAttributeString("CoachingPoints", CoachingPoints);  // 09-27-2010 Scott
            //writer.WriteAttributeString("Assignment", Assignment);
            if (this.Name == "QB")
            {
                writer.WriteAttributeString("QBMotion", QBMotion);

                writer.WriteStartElement("Set");
                foreach (string set in SetArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", set);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Assign");
                foreach (string assign in AssignArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", assign);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Fake");
                foreach (string fake in FakeArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", fake);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (this.ScoutType == 1)
            {// add by scott 11-12-2009
                if (ManCoverage != string.Empty)
                {
                    writer.WriteStartElement("ManCoverage");
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", ManCoverage);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                if (ManPress != string.Empty)
                {
                    writer.WriteStartElement("ManPress");
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", ManPress);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }

            IEnumerable<IFigure> pathPointFigure = GetPathPointFigure(this, null, 0);

            foreach (IFigure figure in pathPointFigure)
            {
                if (figure is Game.PBPlayer)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", (figure as Game.PBPlayer).Name);
                    writer.WriteEndElement();
                }
                else if (figure is PointBase)
                {
                    if (figure is PrePoint)
                    {
                        writer.WriteStartElement("PrePoint");
                    }
                    else if (figure.Dependents != null && figure.Dependents.Count() == 1)
                    {
                        PBLine line = figure.Dependents.ElementAt(0) as PBLine;

                        if (line != null)
                        {
                            switch (line.CapType)
                            {
                                case CapType.Block:
                                    writer.WriteStartElement("PassBlockPoint");
                                    writer.WriteAttributeString("Player", Assignment);  //01-11-2010 scott
                                    break;
                                case CapType.BlockArea:
                                    writer.WriteStartElement("PassBlockAreaPoint");
                                    break;
                                default:

                                    writer.WriteStartElement("Point");
                                    break;
                            }
                        }
                        else
                        {
                            writer.WriteStartElement("Point");
                        }

                    }
                    else
                    {
                        writer.WriteStartElement("Point");
                    }

                    writer.WriteAttributeDouble("X", -(figure as PointBase).Coordinates.X + x);
                    writer.WriteAttributeDouble("Y", -(figure as PointBase).Coordinates.Y - y);
                    PBLine line1 = figure.Dependents.ElementAt(0) as PBLine;
                    LineType linetype = line1.LineType;
                    writer.WriteAttributeString("LineTpye", linetype.ToString());

                    writer.WriteEndElement();
                }
                else if (figure is Zone)
                {
                    writer.WriteStartElement("Zone");
                    writer.WriteAttributeDouble("X", -(figure as Zone).Coordinates.X);
                    writer.WriteAttributeDouble("Y", -(figure as Zone).Coordinates.Y - y);
                    writer.WriteAttributeDouble("Width", (figure as Zone).Width);
                    writer.WriteAttributeDouble("Height", (figure as Zone).Height);
                    writer.WriteEndElement();
                }
            }

            //pathPoint.ForEach(p =>
            //{
            //    writer.WriteStartElement("Point");
            //    writer.WriteAttributeDouble("X", -ToLogical(p).X);
            //    writer.WriteAttributeDouble("Y", -ToLogical(p).Y);
            //    writer.WriteEndElement();
            //});
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("Text", text);
            writer.WriteAttributeString("Stance", Stance);
            writer.WriteAttributeString("R", FillColor.R.ToString());
            writer.WriteAttributeString("G", FillColor.G.ToString());
            writer.WriteAttributeString("B", FillColor.B.ToString());

            // 11-17-2010 Scott
            writer.WriteAttributeString("Dash", Dash.ToString());
            writer.WriteAttributeString("TextVisible", TextVisible.ToString());
            writer.WriteAttributeDouble("Radius", Radius);
            writer.WriteAttributeString("Appearance", Appearance.ToString());   // 10-20-2010 Scott
            writer.WriteAttributeString("TextR", TextColor.R.ToString());
            writer.WriteAttributeString("TextG", TextColor.G.ToString());
            writer.WriteAttributeString("TextB", TextColor.B.ToString());
            writer.WriteAttributeDouble("Angle", Angle);
            writer.WriteAttributeDouble("StrokeThickness", StrokeThickness);

            writer.WriteAttributeString("Assignment", Assignment);
            writer.WriteAttributeString("CoachingPoints", CoachingPoints); // 09-27-2010 Scott
            if (this.Name == "QB")
            {
                writer.WriteAttributeString("QBMotion", QBMotion);
                writer.WriteStartElement("Set");
                foreach (string set in SetArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", set);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Assign");
                foreach (string assign in AssignArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", assign);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Fake");
                foreach (string fake in FakeArray)
                {
                    writer.WriteStartElement("PBPlayer");
                    writer.WriteAttributeString("Name", fake);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (this.ScoutType == 1)
            {// add by scott 11-12-2009
                writer.WriteStartElement("ManCoverage");
                writer.WriteStartElement("PBPlayer");
                writer.WriteAttributeString("Name", ManCoverage);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("ManPress");
                writer.WriteStartElement("PBPlayer");
                writer.WriteAttributeString("Name", ManPress);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);
            Text = element.ReadString("Text");
            Stance = element.ReadString("Stance");
            Assignment = element.ReadString("Assignment");
            StrokeThickness = element.ReadDouble("StrokeThickness");
            if (StrokeThickness < 1)
            {
                StrokeThickness = 1;
            }
            string strAppearance = element.ReadString("Appearance");
            if (strAppearance != null)
            {
                Appearance = strAppearance != null ? (PlayerAppearance)Enum.Parse(typeof(PlayerAppearance), strAppearance) : PlayerAppearance.Circle;   // 10-20-2010 Scott
                UpdateAppearance();
            }
            CoachingPoints = element.ReadString("CoachingPoints");  // 09-27-2010 Scott
            if (this.Name == "QB")
            {
                QBMotion = element.ReadString("QBMotion");
                SetArray.Clear();
                IEnumerable<System.Xml.Linq.XElement> elems = element.Descendants("Set");
                if (elems != null && elems.Count() > 0)
                {
                    System.Xml.Linq.XElement elem = element.Descendants("Set").First();
                    if (elem != null)
                    {
                        foreach (System.Xml.Linq.XElement e in elem.Elements())
                        {
                            SetArray.Add(e.Attribute("Name").Value);
                        }
                    }
                }
                AssignArray.Clear();
                elems = element.Descendants("Assign");
                if (elems != null && elems.Count() > 0)
                {
                    System.Xml.Linq.XElement elem = element.Descendants("Assign").First();
                    if (elem != null)
                    {
                        foreach (System.Xml.Linq.XElement e in elem.Elements())
                        {
                            AssignArray.Add(e.Attribute("Name").Value);
                        }
                    }
                }
                FakeArray.Clear();
                elems = element.Descendants("Fake");
                if (elems != null && elems.Count() > 0)
                {
                    System.Xml.Linq.XElement elem = element.Descendants("Fake").First();
                    if (elem != null)
                    {
                        foreach (System.Xml.Linq.XElement e in elem.Elements())
                        {
                            FakeArray.Add(e.Attribute("Name").Value);
                        }
                    }
                }
            }
            if (this.ScoutType == 1)
            {// add by scott 11-12-2009
                IEnumerable<System.Xml.Linq.XElement> elems = element.Descendants("ManCoverage");
                if (elems != null && elems.Count() > 0)
                {
                    System.Xml.Linq.XElement elem = element.Descendants("ManCoverage").First();
                    if (elem != null)
                    {
                        foreach (System.Xml.Linq.XElement e in elem.Elements())
                        {
                            ManCoverage = e.Attribute("Name").Value;
                        }
                    }
                }
                elems = element.Descendants("ManPress");
                if (elems != null && elems.Count() > 0)
                {
                    System.Xml.Linq.XElement elem = element.Descendants("ManPress").First();
                    if (elem != null)
                    {
                        foreach (System.Xml.Linq.XElement e in elem.Elements())
                        {
                            ManPress = e.Attribute("Name").Value;
                        }
                    }
                }
            }
            FillColor = System.Windows.Media.Color.FromRgb(byte.Parse(element.Attribute("R").Value), byte.Parse(element.Attribute("G").Value), byte.Parse(element.Attribute("B").Value));

            if (element.Attribute("Dash") != null)
            {
                string strDash = element.ReadString("Dash");
                Dash = bool.Parse(strDash);
            }
            if (element.Attribute("TextVisible") != null)
            {
                string strTextVisible = element.ReadString("TextVisible");
                TextVisible = bool.Parse(strTextVisible);
            }
            if (element.Attribute("Radius") != null)
            {
                Radius = element.ReadDouble("Radius");
            }
            if (element.Attribute("Angle") != null)
            {
                Angle = element.ReadDouble("Angle");
            }
            if (element.Attribute("TextR") != null)
            {
                TextColor = System.Windows.Media.Color.FromRgb(byte.Parse(element.Attribute("TextR").Value), byte.Parse(element.Attribute("TextG").Value), byte.Parse(element.Attribute("TextB").Value));
            }
        }

        public void ClearPath()
        {
            List<IFigure> pathFigures = GetAllPathFigure().ToList();

            int count = pathFigures.Count() - 1;

            for (int i = count; i >= 0; i--)
            {
                IFigure figure = pathFigures.ElementAt(i);

                if (figure.Dependencies != null)
                {
                    foreach (IFigure depFigure in figure.Dependencies)
                    {
                        if (!(depFigure is Game.PBPlayer) && !pathFigures.Contains(depFigure))
                        {
                            pathFigures.Add(depFigure);
                        }
                    }
                }
            }

            Drawing.Remove(pathFigures);
        }

        public void ClearEndPath()
        {
            List<IFigure> pathFigures = GetPathFigure(this, null,0).ToList();
            int count = pathFigures.Count() - 1;
            List<IFigure> removepathFigures = new List<IFigure>();
            removepathFigures.Add(pathFigures.ElementAt(count));

            Drawing.Remove(removepathFigures);
        }

        //03-29-2010 Scott
        public IEnumerable<IFigure> GetDeleteFigure(bool containPlayer)
        {
            List<IFigure> figures = new List<IFigure>();
            if (containPlayer)
            {
                figures.Add(this);
            }
            figures.AddRange(this.GetAllPathFigure());
            figures.AddRange(this.GetAllPatheExceptEndPlayer());

            foreach (IFigure dependent in this.Dependents)
            {
                if (dependent.Dependencies.ElementAt(1) == this)
                {
                    figures.Add(dependent);
                }
            }

            return figures;
        }

        // 09-26-2011 Scott
        public IEnumerable<IFigure> GetAllPathFigure()
        {
            List<IFigure> figures = new List<IFigure>();

            for (int i = 0; i < this.Dependents.Count; i++)
            {
                foreach (IFigure figure in GetPathFigure(this, null, i))
                {
                    if (!figures.Contains(figure))
                    {
                        figures.Add(figure);
                    }
                }
            }

            return figures;
        }

        public IEnumerable<IFigure> GetPathFigure(IFigure figure, IFigure existDependent, int indexLine)
        {
            int i = 0;

            foreach (IFigure dependent in figure.Dependents)
            {
                // 09-26-2011 Scott
                if (figure == this && i != indexLine)
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                }
                // end

                if ((dependent.Dependencies.ElementAt(0) == figure) &&
                    dependent is LineBase &&
                    (existDependent == null || (existDependent != null && dependent != existDependent)))
                {
                    yield return dependent;

                    if (dependent.Dependencies.ElementAt(1) is PBPlayer)
                    {//08-27-2009 scott
                        break;
                    }

                    foreach (IFigure dependency in dependent.Dependencies)
                    {
                        if (dependency != figure)
                        {
                            foreach (IFigure f in GetPathFigure(dependency, dependent, 0))
                            {
                                yield return f;
                            }
                        }
                    }
                }
            }
        }

        public Path GetPath(ref double distance, ref double preDistance, ref int preCount)
        {
            Path path = new Path();

            PathGeometry pg = new PathGeometry();

            this.GetPath(ref pg, ref distance, ref preDistance, ref preCount, this, null, 0);

            if (pg.Bounds == Rect.Empty) return null;

            path.Data = pg;

            return path;
        }

        public void GetPath(ref PathGeometry pg, ref double distance, ref double preDistance, ref int preCount, IFigure figure, IFigure existDependent, int indexLine)
        {
            int i = 0;

            foreach (IFigure dependent in figure.Dependents)
            {
                // 09-26-2011 Scott
                if (figure == this && i != indexLine)
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                }
                // end

                if ((dependent.Dependencies.ElementAt(0) == figure) &&
                    dependent is LineBase &&
                    (existDependent == null || (existDependent != null && dependent != existDependent)))
                {
                    Line line = (dependent as LineBase).Shape;

                    pg.AddGeometry(
                        new LineGeometry()
                        {
                            StartPoint = new Point(line.X1, line.Y1),
                            EndPoint = new Point(line.X2, line.Y2)
                        });

                    distance += Math.Distance(line.X1, line.Y1, line.X2, line.Y2);

                    if (dependent.Dependencies.ElementAt(1) is PrePoint)
                    {
                        preDistance += Math.Distance(line.X1, line.Y1, line.X2, line.Y2);
                        preCount++;
                    }

                    if (dependent.Dependencies.ElementAt(1) is PBPlayer)
                    {//09-08-2009 scott
                        break;
                    }

                    foreach (IFigure dependency in dependent.Dependencies)
                    {
                        if (dependency != figure)
                        {
                            this.GetPath(ref pg, ref distance, ref preDistance, ref preCount, dependency, dependent, 0);
                        }
                    }
                }
            }
        }

        public IFigure GetEndFigure()
        {
            IEnumerable<IFigure> pathPointFigures = GetPathPointFigure(this, null, 0);

            if (pathPointFigures.Count() > 0)
            {
                return pathPointFigures.Last();
            }
            else
            {
                return null;
            }
        }

        public PrePoint GetEndPrePoint()
        {
            IEnumerable<IFigure> pathPointFigures = GetPathPointFigure(this, null, 0);

            if (pathPointFigures.Count() > 0)
            {
                if (pathPointFigures.Any(f => f is PrePoint))
                {
                    IFigure figure = pathPointFigures.Last(f => f is PrePoint);

                    return figure as PrePoint;
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

        public IFigure GetStartFreeFigure()
        {
            IEnumerable<IFigure> pathPointFigures = GetPathPointFigure(this, null, 0);

            if (pathPointFigures.Count() > 0)
            {
                if (pathPointFigures.Any(f => !(f is PrePoint)))
                {
                    IFigure figure = pathPointFigures.First(f => !(f is PrePoint));

                    return figure;
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

        // 09-26-2011 Scott
        public IEnumerable<IFigure> GetAllPathPointFigure()
        {
            List<IFigure> figures = new List<IFigure>();

            for (int i = 0; i < this.Dependents.Count; i++)
            {
                foreach (IFigure figure in GetPathPointFigure(this, null, i))
                {
                    if (!figures.Contains(figure))
                    {
                        figures.Add(figure);
                    }
                }
            }

            return figures;
        }

        // 10-09-02011 Scott
        public int GetPathCount()
        {
            return this.Dependents.Count;
        }

        public IEnumerable<IFigure> GetPathPointFigure(IFigure figure, IFigure existDependent, int indexLine)
        {
            int i = 0;

            foreach (IFigure dependent in figure.Dependents)
            {
                // 09-26-2011 Scott
                if (figure == this && i != indexLine)
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                }
                // end

                if ((dependent.Dependencies.ElementAt(0) == figure) &&
                    dependent is LineBase &&
                    (existDependent == null || (existDependent != null && dependent != existDependent)))
                {
                    yield return (dependent as LineBase).Dependencies.ElementAt(1);

                    if (dependent.Dependencies.ElementAt(1) is PBPlayer)
                    {//08-27-2009 scott
                        break;
                    }

                    foreach (IFigure dependency in dependent.Dependencies)
                    {
                        if (dependency != figure)
                        {
                            foreach (IFigure p in GetPathPointFigure(dependency, dependent, 0))
                            {
                                yield return p;
                            }
                        }
                    }
                }
            }
        }

        // 09-26-2011 Scott
        public IEnumerable<IFigure> GetAllPatheExceptEndPlayer()
        {
            List<IFigure> figures = new List<IFigure>();

            for (int i = 0; i < this.Dependents.Count; i++)
            {
                foreach (IFigure figure in GetPatheExceptEndPlayer(this, null, i))
                {
                    if (!figures.Contains(figure))
                    {
                        figures.Add(figure);
                    }
                }
            }

            return figures;
        }

        public IEnumerable<IFigure> GetPatheExceptEndPlayer(IFigure figure, IFigure existDependent, int indexLine)
        {
            int i = 0;

            foreach (IFigure dependent in figure.Dependents)
            {
                // 09-26-2011 Scott
                if (figure == this && i != indexLine)
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                }
                // end

                if ((dependent.Dependencies.ElementAt(0) == figure) &&
                    dependent is LineBase &&
                    (existDependent == null || (existDependent != null && dependent != existDependent)))
                {
                    if (dependent.Dependencies.ElementAt(1) is PBPlayer)
                    {
                        break;
                    }
                    yield return (dependent as LineBase).Dependencies.ElementAt(1);

                    foreach (IFigure dependency in dependent.Dependencies)
                    {
                        if (dependency != figure)
                        {
                            foreach (IFigure p in GetPatheExceptEndPlayer(dependency, dependent, 0))
                            {
                                yield return p;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<Point> GetPathPoint(IFigure figure, IFigure existDependent, int indexLine)
        {
            int i = 0;

            foreach (IFigure dependent in figure.Dependents)
            {
                // 09-26-2011 Scott
                if (figure == this && i != indexLine)
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                }
                // end

                if ((dependent.Dependencies.ElementAt(0) == figure) &&
                    dependent is LineBase &&
                    (existDependent == null || (existDependent != null && dependent != existDependent)))
                {
                    Line line = (dependent as LineBase).Shape;

                    yield return new Point(line.X2, line.Y2);

                    if (dependent.Dependencies.ElementAt(1) is PBPlayer)
                    {//08-27-2009 scott
                        break;
                    }

                    foreach (IFigure dependency in dependent.Dependencies)
                    {
                        if (dependency != figure)
                        {
                            foreach (Point p in GetPathPoint(dependency, dependent, 0))
                            {
                                yield return p;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnSelected(bool bSelected)
        {
            //InitSelectedMovie();

            //if (bSelected)
            //{
            //    selectedStory.Begin(Drawing.Canvas, true);
            //}
            //else
            //{
            //    selectedStory.Stop();
            //}

            if (bSelected)
            {
                Canvas.SetZIndex(Shape, (int)ZOrder.Selected);
                Canvas.SetZIndex(TextBlock, (int)ZOrder.Selected);
                Canvas.SetZIndex(Shadow, (int)ZOrder.Selected);
            }
            else
            {
                Canvas.SetZIndex(Shape, (int)ZOrder.Points);
                Canvas.SetZIndex(TextBlock, (int)ZOrder.Points);
                Canvas.SetZIndex(Shadow, (int)ZOrder.Points);
            }
        }

        public void InitSelectedMovie()
        {
            this.Shape.RenderTransformOrigin = new Point(0.5, 0.5);
            this.TextBlock.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform scale = new ScaleTransform(1, 1);
            this.Shape.RenderTransform = scale;
            this.TextBlock.RenderTransform = scale;
            NameScope.SetNameScope(Drawing.Canvas, new NameScope());
            Drawing.Canvas.RegisterName("scale", scale);

            double sec = 1;
            Duration duration = new Duration(TimeSpan.FromSeconds(sec));

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.From = 1;
            animationY.From = 1;
            animationX.To = 1.1;
            animationY.To = 1.1;
            animationX.Duration = duration;
            animationY.Duration = duration;

            selectedStory = new Storyboard();
            selectedStory.Duration = new Duration(TimeSpan.FromSeconds(sec));
            selectedStory.Children.Add(animationX);
            selectedStory.Children.Add(animationY);
            selectedStory.AutoReverse = true;
            selectedStory.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(animationX, "scale");
            Storyboard.SetTargetName(animationY, "scale");
            Storyboard.SetTargetProperty(animationX, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(ScaleTransform.ScaleYProperty));
        }

        public double Run()
        {
            double speed = 5;
            double distance = 0;
            double preDistance = 0;
            int preCount = 0;
            Path path = GetPath(ref distance, ref preDistance, ref preCount);

            if (path == null)
            {
                path = new Path();

                PathGeometry pg = new PathGeometry();

                path.Data = pg;

                (path.Data as PathGeometry).AddGeometry(
                        new LineGeometry()
                        {
                            StartPoint = ToPhysical(Coordinates),
                            EndPoint = ToPhysical(new Point(Coordinates.X, Coordinates.Y + 0.1)),
                        });
            }

            if ((path.Data as PathGeometry).Figures.Count == preCount)
            {
                Point pt = GetEndPrePoint().Coordinates;

                (path.Data as PathGeometry).AddGeometry(
                        new LineGeometry()
                        {
                            StartPoint = ToPhysical(pt),
                            EndPoint = ToPhysical(new Point(pt.X, pt.Y + 0.1)),
                        });
            }

            for (int i = 0; i < preCount; i++)
            {
                (path.Data as PathGeometry).Figures.RemoveAt(0);
            }

            double beginTime = 0;
            double duringTime = ToLogical(distance - preDistance) / speed;

            Canvas.SetLeft(this.TextBlock, -this.TextBlock.ActualWidth / 2);
            Canvas.SetTop(this.TextBlock, -this.TextBlock.ActualHeight / 2);
            Canvas.SetLeft(this.Shape, -this.Shape.ActualWidth / 2);
            Canvas.SetTop(this.Shape, -this.Shape.ActualHeight / 2);
            Canvas.SetLeft(this.Shadow, -this.Shadow.ActualWidth / 2);
            Canvas.SetTop(this.Shadow, -this.Shadow.ActualHeight / 2);
            this.Shape.RenderTransformOrigin = new Point(0.5, 0.5);
            this.TextBlock.RenderTransformOrigin = new Point(0.5, 0.5);
            this.Shadow.RenderTransformOrigin = new Point(0.5, 0.5);

            MatrixTransform matrix = new MatrixTransform();
            this.Shape.RenderTransform = matrix;
            this.TextBlock.RenderTransform = matrix;
            this.Shadow.RenderTransform = matrix;

            NameScope.SetNameScope(Drawing.Canvas, new NameScope());
            Drawing.Canvas.RegisterName("matrix", matrix);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            matrixAnimation.PathGeometry = path.Data.GetFlattenedPathGeometry();
            matrixAnimation.BeginTime = TimeSpan.FromSeconds(beginTime);
            matrixAnimation.Duration = new Duration(TimeSpan.FromSeconds(duringTime));
            matrixAnimation.RepeatBehavior = new RepeatBehavior(1);
            matrixAnimation.AutoReverse = false;
            matrixAnimation.IsOffsetCumulative = !matrixAnimation.AutoReverse;
            matrixAnimation.DoesRotateWithTangent = false;

            story = new Storyboard();
            story.Children.Add(matrixAnimation);
            Storyboard.SetTargetName(matrixAnimation, "matrix");
            Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));
            story.Begin(Drawing.Canvas, true);

            return distance - preDistance;
        }

        public double RunPreRoute(Action<object, EventArgs> story_Completed)
        {
            double speed = 5;
            double distance = 0;
            int preCount = 0;
            double preDistance = 0;
            Path path = GetPath(ref distance, ref preDistance, ref preCount);

            if (path != null)
            {
                int count = (path.Data as PathGeometry).Figures.Count;

                for (int i = preCount; i < count; i++)
                {
                    (path.Data as PathGeometry).Figures.RemoveAt(preCount);
                }
            }

            if (preDistance == 0 || path == null)
            {
                path = new Path();

                PathGeometry pg = new PathGeometry();

                path.Data = pg;

                pg.AddGeometry(
                            new LineGeometry()
                            {
                                StartPoint = ToPhysical(Coordinates),
                                EndPoint = ToPhysical(new Point(Coordinates.X, Coordinates.Y + 0.1)),
                            });
            }

            double beginTime = 0;
            double duringTime = ToLogical(preDistance) / speed;

            Canvas.SetLeft(this.TextBlock, -this.TextBlock.ActualWidth / 2);
            Canvas.SetTop(this.TextBlock, -this.TextBlock.ActualHeight / 2);
            Canvas.SetLeft(this.Shape, -this.Shape.ActualWidth / 2);
            Canvas.SetTop(this.Shape, -this.Shape.ActualHeight / 2);
            Canvas.SetLeft(this.Shadow, -this.Shadow.ActualWidth / 2);
            Canvas.SetTop(this.Shadow, -this.Shadow.ActualHeight / 2);
            this.Shape.RenderTransformOrigin = new Point(0.5, 0.5);
            this.TextBlock.RenderTransformOrigin = new Point(0.5, 0.5);
            this.Shadow.RenderTransformOrigin = new Point(0.5, 0.5);

            MatrixTransform matrix = new MatrixTransform();
            this.Shape.RenderTransform = matrix;
            this.TextBlock.RenderTransform = matrix;
            this.Shadow.RenderTransform = matrix;

            NameScope.SetNameScope(Drawing.Canvas, new NameScope());
            Drawing.Canvas.RegisterName("matrix", matrix);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            matrixAnimation.PathGeometry = path.Data.GetFlattenedPathGeometry();
            matrixAnimation.BeginTime = TimeSpan.FromSeconds(beginTime);
            matrixAnimation.Duration = new Duration(TimeSpan.FromSeconds(duringTime));
            matrixAnimation.RepeatBehavior = new RepeatBehavior(1);
            matrixAnimation.AutoReverse = false;
            matrixAnimation.IsOffsetCumulative = !matrixAnimation.AutoReverse;
            matrixAnimation.DoesRotateWithTangent = false;

            story = new Storyboard();
            story.Completed += new EventHandler(story_Completed);
            story.Children.Add(matrixAnimation);
            Storyboard.SetTargetName(matrixAnimation, "matrix");
            Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));
            story.Begin(Drawing.Canvas, true);

            return preDistance;
        }

        public void Stop()
        {
            if (story != null)
            {
                story.Stop(Drawing.Canvas);
                this.UpdateVisual();
            }
        }

        public void Pause()
        {
            if (story != null)
            {
                story.Pause(Drawing.Canvas);
            }
        }

        public void Resume()
        {
            if (story != null)
            {
                story.Resume(Drawing.Canvas);
            }
        }

        public override void OnAddingToCanvas(Canvas newContainer)
        {
            base.OnAddingToCanvas(newContainer);
            newContainer.Children.Add(TextBlock);
            newContainer.Children.Add(Shadow);
        }

        public override void OnRemovingFromCanvas(Canvas leavingContainer)
        {
            base.OnRemovingFromCanvas(leavingContainer);
            leavingContainer.Children.Remove(TextBlock);
            leavingContainer.Children.Remove(Shadow);
        }

        public override void MoveToCore(Point newLocation)
        {
            foreach (SubPoint sp in this.GetPathPointFigure(this, null, 0).OfType<SubPoint>())
            {// 01-07-2010 Scott
                Point newSubLocation = new Point(sp.Coordinates.X + newLocation.X - this.Coordinates.X,
                    sp.Coordinates.Y + newLocation.Y - this.Coordinates.Y);
                sp.MoveTo(newSubLocation);
            }

            base.MoveToCore(newLocation);
        }

        public override void UpdateVisual()
        {
            double nLen = ToPhysical(Radius);

            // 10-20-2010 Scott
            if (Appearance == PlayerAppearance.Triangle)
            {
                Polygon polygon = Shape as Polygon;
                if (polygon != null)
                {
                    Point point1 = new Point(nLen / 2, 1);
                    Point point2 = new Point(1, nLen - 1);
                    Point point3 = new Point(nLen - 1, nLen - 1);
                    PointCollection myPointCollection = new PointCollection();
                    myPointCollection.Add(point1);
                    myPointCollection.Add(point2);
                    myPointCollection.Add(point3);
                    polygon.Points = myPointCollection;
                }
            }

            if (Canvas != null && Canvas.ActualWidth > 0)
            {
                if (TextBlock.Text.Length < 3)
                {
                    TextBlock.FontSize = (Canvas.ActualWidth / 1000) * 12 * Radius; // 11-03-2010 Scott
                }
                else
                {
                    TextBlock.FontSize = (Canvas.ActualWidth / 1000) * 10 * Radius; // 11-03-2010 Scott
                }
            }

            Shape.Width = nLen;
            Shape.Height = nLen;
            base.UpdateVisual();
            double x = ToPhysical(Coordinates).X;
            double y = ToPhysical(Coordinates).Y;
            TextBlock.TextAlignment = TextAlignment.Center;
            Canvas.SetLeft(TextBlock, Canvas.GetLeft(Shape));
            Canvas.SetTop(TextBlock, Canvas.GetTop(Shape) + (nLen - TextBlock.FontSize) / 2);
            TextBlock.Width = nLen;

            //TextBlock.Clip = Shape.RenderedGeometry;

            //if (ScoutType == 1)
            //{
            //    TextBlock.RenderTransform = new RotateTransform(180, nLen / 2, 7);
            //}

            if (Shape != null)
            {
                Shape.Fill = new LinearGradientBrush(Colors.White, FillColor, 45);

                if (Dash)
                {
                    DoubleCollection dc = new DoubleCollection();
                    dc.Add(5);
                    dc.Add(5);
                    Shape.StrokeDashArray = dc;
                }
                else
                {
                    Shape.StrokeDashArray = null;
                }
            }

            TextBlock.Foreground = new SolidColorBrush(TextColor);

            if (Selected)
            {
                Shape.Effect = PBEffects.SelectedEffect;    // 11-18-2010 Scott
                Canvas.SetZIndex(Shape, (int)ZOrder.Selected);
                Canvas.SetZIndex(TextBlock, (int)ZOrder.Selected);
                Canvas.SetZIndex(Shadow, (int)ZOrder.Selected);
            }
            else
            {
                Shape.Effect = null;    // 11-18-2010 Scott
                Canvas.SetZIndex(Shape, (int)ZOrder.Points);
                Canvas.SetZIndex(TextBlock, (int)ZOrder.Points);
                Canvas.SetZIndex(Shadow, (int)ZOrder.Points);
            }

            if (this.TextVisible)
            {
                TextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlock.Visibility = Visibility.Hidden;
            }

            Shape.StrokeThickness = StrokeThickness;

            Shade();

            SetAngle();
        }

        private void SetAngle()
        {
            if (Shape != null)
            {
                this.Shape.RenderTransform = new RotateTransform(Angle, this.Shape.ActualWidth / 2, this.Shape.ActualHeight / 2);
            }
            if (TextBlock != null)
            {
                this.TextBlock.RenderTransform = new RotateTransform(Angle, this.TextBlock.ActualWidth / 2, this.TextBlock.ActualHeight / 2);
            }
            if (Shadow != null)
            {
                this.Shadow.RenderTransform = new RotateTransform(Angle, this.Shadow.ActualWidth / 2, this.Shadow.ActualHeight / 2);
            }
        }

        protected override void UpdateShapeAppearance()
        {
            if (Shape != null)
            {
                Shape.Fill = new LinearGradientBrush(Colors.White, FillColor, 45);
            }

            if (Selected)
            {
                TextBlock.Foreground = SelectedBrush;
                if (Shape != null)
                {
                    Shape.Stroke = SelectedBrush;
                }
                //this.GetPathPointFigure(this, null).Where(f => f == this || !(f is PBPlayer)).ForEach(f => f.Selected = true); // 01-08-2010 Scott
            }
            else
            {
                TextBlock.Foreground = new SolidColorBrush(TextColor);
                if (Shape != null)
                {
                    Shape.Stroke = Brushes.Black;
                }
                //this.GetPathPointFigure(this, null).Where(f => f == this || !(f is PBPlayer)).ForEach(f => f.Selected = false); // 01-08-2010 Scott
            }
            //base.UpdateShapeAppearance();
        }

        //public PathGeometry GetTextPath(string word, string fontFamily, int fontSize)
        //{
        //    Typeface typeface = new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal); 
        //    return GetTextPath(word, typeface, fontSize);
        //}

        //public PathGeometry GetTextPath(string word, Typeface typeface, int fontSize)
        //{
        //    FormattedText text = new FormattedText(word, 
        //        new System.Globalization.CultureInfo("zh-cn"), 
        //        FlowDirection.LeftToRight, typeface, fontSize,
        //        Brushes.Black);

        //    Geometry geo = text.BuildGeometry(new Point(0, 0)); 
        //    PathGeometry path = geo.GetFlattenedPathGeometry();

        //    return path; 
        //}

        public void CoverageZone(Zone zone)
        {
            if (this.ScoutType == 1)
            {
                PBLine line = new PBLine();
                line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.ZoneLineColor;
                List<IFigure> dependencies = new List<IFigure>();
                dependencies.Add(this);
                dependencies.Add(zone);
                line.Dependencies = dependencies;
                Lists.FigureList fl = new Lists.FigureList();
                fl.Add(zone);
                fl.Add(line);
                Drawing.Add(fl as IEnumerable<IFigure>);
            }
        }

        public void CreateZone()
        {
            if (this.ScoutType == 1)
            {
                Zone zone = new Zone()
                {
                    Center = Coordinates,
                    Height = 10,
                    Width = 18,
                    FillColor = Webb.Playbook.Data.ColorSetting.Instance.ZoneColor,
                };

                PBLine line = new PBLine();
                line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.ZoneLineColor;
                List<IFigure> dependencies = new List<IFigure>();
                dependencies.Add(this);
                dependencies.Add(zone);
                line.Dependencies = dependencies;
                Lists.FigureList fl = new Lists.FigureList();
                fl.Add(zone);
                fl.Add(line);
                Drawing.Add(fl as IEnumerable<IFigure>);
            }
        }

        public void ChangeLineType(CapType capType)
        {// 01-07-2010 scott
            IEnumerable<IFigure> figures = this.GetPathFigure(this, null, 0);
            figures.OfType<PBLine>().ForEach(line => line.CapType = CapType.Arrow);

            if (figures.Count() > 0 && figures.Last() is PBLine)
            {
                PBLine endLine = figures.Last() as PBLine;

                endLine.CapType = capType;

                if (!Drawing.DrawingMode)
                {
                    if (endLine.Dependencies != null && !(endLine.Dependencies.ElementAt(1) is PrePoint))
                    {
                        switch (capType)
                        {
                            case CapType.None:
                            case CapType.Arrow:
                                //endLine.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.RouteColor;
                                break;
                            case CapType.Block:
                            case CapType.BlockArea:
                                endLine.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                                break;
                        }
                    }
                }
            }
        }

        public void ChangeAppearance(PlayerAppearance playerAppearnce)  // 10-20-2010 Scott
        {
            if (Drawing != null && Drawing.Canvas != null)
            {
                this.Appearance = playerAppearnce;

                Drawing.Canvas.Children.Remove(Shadow);
                Drawing.Canvas.Children.Remove(TextBlock);
                Drawing.Canvas.Children.Remove(Shape);

                UpdateAppearance();

                Drawing.Canvas.Children.Add(Shape);
                Drawing.Canvas.Children.Add(TextBlock);
                Drawing.Canvas.Children.Add(Shadow);

                Canvas.SetZIndex(Shape, (int)ZOrder.Selected);
                Canvas.SetZIndex(TextBlock, (int)ZOrder.Selected);
                Canvas.SetZIndex(Shadow, (int)ZOrder.Selected);

                this.Selected = true;
                this.UpdateVisual();
            }
        }

        public void UpdateAppearance()
        {
            Shape = CreateShape();
            Shadow = CreateShadow();
        }

        public void RemoveSubPoint()
        {
            IFigure figure = this.GetEndFigure();
            if (figure is SubPoint)
            {
                List<IFigure> figures = new List<IFigure>();
                figures.Add(figure);
                figures.Add(this.GetPathFigure(this, null, 0).Last());
                Drawing.Remove(figures);
            }
        }

        public void RemoveBlockPlayer()
        {
            IFigure figure = this.GetEndFigure();
            if (figure is PBPlayer)
            {
                Drawing.Remove(this.GetPathFigure(this, null, 0).Last());
            }
        }

        public void RemoveForActions()
        {
            RemoveSubPoint();
            RemoveBlockPlayer();
        }

        public void ClearAssignment()
        {
            Assignment = string.Empty;
            ManPress = string.Empty;
            ManCoverage = string.Empty;
            SetArray.Clear();
            FakeArray.Clear();
            AssignArray.Clear();
            QBMotion = string.Empty;
            ClearPath();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override IFigure HitTest(Rect rect)
        {
            if (Coordinates.X >= rect.Left && Coordinates.X <= rect.Right
                && Coordinates.Y >= rect.Top && Coordinates.Y <= rect.Bottom)
            {
                return this;
            }
            return null;
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

        public void LoadRoute(string file)
        {
            this.ClearPath();

            PBRouteList routeList = new PBRouteList();
            routeList.ReadXml(file);

            foreach (PBRoute route in routeList.Routes)
            {
                IPoint startpoint = this;
                Lists.FigureList flAdd = new Webb.Playbook.Geometry.Lists.FigureList();

                foreach (PBRoutePoint pbPoint in route.Path)
                {
                    Point ptReal = this.Coordinates.Plus(pbPoint.Point);

                    IPoint endpoint = null;

                    if (pbPoint.PrePoint)
                    {
                        endpoint = Factory.CreatePrePoint(Drawing, ptReal);
                    }
                    else if (pbPoint.IsZone)
                    {
                        endpoint = new Zone()
                        {
                            Drawing = Drawing,
                            Width = pbPoint.ZoneWidth,
                            Height = pbPoint.ZoneHeight,
                            Coordinates = ptReal,
                            FillColor = pbPoint.ZoneColor,
                            Transparency = pbPoint.Opacity,
                        };
                        (endpoint as Zone).MoveTo(ptReal);
                    }
                    else
                    {
                        endpoint = Factory.CreateFreePoint(Drawing, ptReal);
                    }

                    Lists.FigureList fl = new Webb.Playbook.Geometry.Lists.FigureList();
                    fl.Add(startpoint);
                    fl.Add(endpoint);
                    PBLine line = Factory.CreateLine(Drawing, fl, pbPoint);

                    flAdd.Add(endpoint);
                    flAdd.Add(line);

                    startpoint = endpoint;
                }

                Drawing.Add(flAdd as IEnumerable<IFigure>);
            }
            Drawing.Figures.UpdateVisual();
        }

        public void SaveRoute(string file)
        {
            PBRouteList routeList = new PBRouteList();
            int count = GetPathCount();
            
            for (int i = 0; i < count; i++)
            {
                PBRoute route = new PBRoute();
                IEnumerable<IFigure> figures = GetPathPointFigure(this, null, i);
                foreach (IFigure f in figures)
                {
                    if (f is IPoint)
                    {
                        PBRoutePoint pbPoint = new PBRoutePoint();
                        pbPoint.Point = (f as IPoint).Coordinates.Minus(this.Coordinates);

                        if (f.Dependents.Count > 0)
                        {
                            PBLine line = f.Dependents.ElementAt(0) as PBLine;
                            if (line != null)
                            {
                                pbPoint.LineType = line.LineType;
                                pbPoint.CapType = line.CapType;
                                pbPoint.DashType = line.DashType;
                                pbPoint.StrokeColor = line.StrokeColor;
                            }
                        }

                        if (f is PrePoint)
                        {
                            pbPoint.PrePoint = true;
                        }
                        else if (f is Zone)
                        {
                            Zone zone = f as Zone;
                            pbPoint.IsZone = true;
                            pbPoint.ZoneWidth = zone.Width;
                            pbPoint.ZoneHeight = zone.Height;
                            pbPoint.ZoneColor = zone.FillColor;
                            pbPoint.Opacity = zone.Transparency;
                        }

                        route.Path.Add(pbPoint);
                    }
                }
                routeList.Routes.Add(route);
            }

            routeList.WriteXml(file);
        }

        // add for Set
        public void SetWriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            var coordinates = Coordinates;
            writer.WriteAttributeDouble("X", coordinates.X);
            writer.WriteAttributeDouble("Y", coordinates.Y);

            writer.WriteAttributeString("R", FillColor.R.ToString());
            writer.WriteAttributeString("G", FillColor.G.ToString());
            writer.WriteAttributeString("B", FillColor.B.ToString());

            writer.WriteAttributeString("Dash", Dash.ToString());
            writer.WriteAttributeString("TextVisible", TextVisible.ToString());
            writer.WriteAttributeDouble("Radius", Radius);
            writer.WriteAttributeString("Appearance", Appearance.ToString());

            writer.WriteAttributeString("TextR", TextColor.R.ToString());
            writer.WriteAttributeString("TextG", TextColor.G.ToString());
            writer.WriteAttributeString("TextB", TextColor.B.ToString());

            writer.WriteAttributeString("Text", Text);
        }
    }
}
