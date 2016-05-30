using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;

using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Game
{
    public class PBRoutePoint
    {
        private Point point = new Point();
        public Point Point
        {
            get { return point; }
            set { point = value; }
        }

        private LineType lineType = LineType.BeeLine;
        public LineType LineType
        {
            get { return lineType; }
            set { lineType = value; }
        }

        private CapType capType = CapType.Arrow;
        public CapType CapType
        {
            get { return capType; }
            set { capType = value; }
        }

        private DashType dashType = DashType.Solid;
        public DashType DashType
        {
            get { return dashType; }
            set { dashType = value; }
        }

        private bool prePoint = false;
        public bool PrePoint
        {
            get { return prePoint; }
            set { prePoint = value; }
        }

        private bool isZone = false;
        public bool IsZone
        {
            get
            {
                return isZone;
            }
            set
            {
                isZone = value;
            }
        }

        private double opacity = 0.5;
        public double Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        private double zoneWidth = 18;
        public double ZoneWidth
        {
            get { return zoneWidth; }
            set { zoneWidth = value; }
        }

        private double zoneHeight = 10;
        public double ZoneHeight
        {
            get { return zoneHeight; }
            set { zoneHeight = value; }
        }

        private System.Windows.Media.Color zoneColor = Webb.Playbook.Data.ColorSetting.Instance.ZoneColor;
        public System.Windows.Media.Color ZoneColor
        {
            get
            {
                return zoneColor;
            }
            set
            {
                zoneColor = value;
            }
        }

        private System.Windows.Media.Color strokeColor = System.Windows.Media.Colors.Black;
        public System.Windows.Media.Color StrokeColor
        {
            get
            {
                return strokeColor;
            }
            set
            {
                strokeColor = value;
            }
        }
    }

    public class PBRoute
    {
        private List<PBRoutePoint> path;
        public List<PBRoutePoint> Path
        {
            get
            {
                if (path == null)
                {
                    path = new List<PBRoutePoint>();
                }
                return path;
            }
        }

        public PBRoute()
        {

        }

        public virtual void ReadXml(string file)
        {
            if (System.IO.File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);

                Path.Clear();

                foreach (XElement elem in doc.Descendants("Point"))
                {
                    PBRoutePoint pbPoint = new PBRoutePoint();

                    pbPoint.Point = new Point(elem.ReadDouble("X"), elem.ReadDouble("Y"));

                    if (elem.Attribute("LineType") != null)
                    {
                        pbPoint.LineType = (LineType)Enum.Parse(typeof(LineType), elem.ReadString("LineType"));
                    }

                    if (elem.Attribute("DashType") != null)
                    {
                        pbPoint.DashType = (DashType)Enum.Parse(typeof(DashType), elem.ReadString("DashType"));
                    }

                    if (elem.Attribute("CapType") != null)
                    {
                        pbPoint.CapType = (CapType)Enum.Parse(typeof(CapType), elem.ReadString("CapType"));
                    }

                    if (elem.Attribute("PrePoint") != null)
                    {
                        pbPoint.PrePoint = bool.Parse(elem.ReadString("PrePoint"));
                    }

                    if (elem.Attribute("IsZone") != null)
                    {
                        pbPoint.IsZone = bool.Parse(elem.ReadString("IsZone"));
                    }

                    if (elem.Attribute("ZoneWidth") != null)
                    {
                        pbPoint.ZoneWidth = elem.ReadDouble("ZoneWidth");
                    }

                    if (elem.Attribute("ZoneHeight") != null)
                    {
                        pbPoint.ZoneHeight = elem.ReadDouble("ZoneHeight");
                    }

                    if (elem.Attribute("Opacity") != null)
                    {
                        pbPoint.Opacity = elem.ReadDouble("Opacity");
                    }

                    if (elem.Attribute("ZoneR") != null)
                    {
                        pbPoint.ZoneColor = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("ZoneR").Value), byte.Parse(elem.Attribute("ZoneG").Value), byte.Parse(elem.Attribute("ZoneB").Value));
                    }

                    if (elem.Attribute("R") != null)
                    {
                        pbPoint.StrokeColor = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("R").Value), byte.Parse(elem.Attribute("G").Value), byte.Parse(elem.Attribute("B").Value));
                    }

                    Path.Add(pbPoint);
                }
            }
        }

        public virtual void WriteXml(string file)
        {
            XDocument doc = new XDocument();

            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");

            doc.Declaration = declare;

            XElement rootElem = new XElement("Route");

            foreach (PBRoutePoint pbPoint in Path)
            {
                XElement xpt = new XElement("Point");
                xpt.Add(new XAttribute("X", pbPoint.Point.X));
                xpt.Add(new XAttribute("Y", pbPoint.Point.Y));

                xpt.Add(new XAttribute("LineType", pbPoint.LineType.ToString()));
                xpt.Add(new XAttribute("DashType", pbPoint.DashType.ToString()));
                xpt.Add(new XAttribute("CapType", pbPoint.CapType.ToString()));
                xpt.Add(new XAttribute("PrePoint", pbPoint.PrePoint.ToString()));
                xpt.Add(new XAttribute("IsZone", pbPoint.IsZone.ToString()));
                xpt.Add(new XAttribute("ZoneWidth", pbPoint.ZoneWidth.ToString()));
                xpt.Add(new XAttribute("ZoneHeight", pbPoint.ZoneHeight.ToString()));
                xpt.Add(new XAttribute("Opacity", pbPoint.Opacity.ToString()));

                xpt.Add(new XAttribute("ZoneR", pbPoint.ZoneColor.R.ToString()));
                xpt.Add(new XAttribute("ZoneG", pbPoint.ZoneColor.G.ToString()));
                xpt.Add(new XAttribute("ZoneB", pbPoint.ZoneColor.B.ToString()));
                xpt.Add(new XAttribute("R", pbPoint.StrokeColor.R.ToString()));
                xpt.Add(new XAttribute("G", pbPoint.StrokeColor.G.ToString()));
                xpt.Add(new XAttribute("B", pbPoint.StrokeColor.B.ToString()));

                rootElem.Add(xpt);
            }

            doc.Add(rootElem);

            doc.Save(file);
        }

        // 10-09-2011 Scott
        public virtual void ReadXml(XElement parentElem)
        {
            Path.Clear();

            foreach (XElement elem in parentElem.Descendants("Point"))
            {
                PBRoutePoint pbPoint = new PBRoutePoint();

                pbPoint.Point = new Point(elem.ReadDouble("X"), elem.ReadDouble("Y"));

                if (elem.Attribute("LineType") != null)
                {
                    pbPoint.LineType = (LineType)Enum.Parse(typeof(LineType), elem.ReadString("LineType"));
                }

                if (elem.Attribute("DashType") != null)
                {
                    pbPoint.DashType = (DashType)Enum.Parse(typeof(DashType), elem.ReadString("DashType"));
                }

                if (elem.Attribute("CapType") != null)
                {
                    pbPoint.CapType = (CapType)Enum.Parse(typeof(CapType), elem.ReadString("CapType"));
                }

                if (elem.Attribute("PrePoint") != null)
                {
                    pbPoint.PrePoint = bool.Parse(elem.ReadString("PrePoint"));
                }

                if (elem.Attribute("IsZone") != null)
                {
                    pbPoint.IsZone = bool.Parse(elem.ReadString("IsZone"));
                }

                if (elem.Attribute("ZoneWidth") != null)
                {
                    pbPoint.ZoneWidth = elem.ReadDouble("ZoneWidth");
                }

                if (elem.Attribute("ZoneHeight") != null)
                {
                    pbPoint.ZoneHeight = elem.ReadDouble("ZoneHeight");
                }

                if (elem.Attribute("Opacity") != null)
                {
                    pbPoint.Opacity = elem.ReadDouble("Opacity");
                }

                if (elem.Attribute("ZoneR") != null)
                {
                    pbPoint.ZoneColor = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("ZoneR").Value), byte.Parse(elem.Attribute("ZoneG").Value), byte.Parse(elem.Attribute("ZoneB").Value));
                }

                if (elem.Attribute("R") != null)
                {
                    pbPoint.StrokeColor = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("R").Value), byte.Parse(elem.Attribute("G").Value), byte.Parse(elem.Attribute("B").Value));
                }

                Path.Add(pbPoint);
            }
        }

        // 10-09-2011 Scott
        public virtual void WriteXml(XElement parentElem)
        {
            foreach (PBRoutePoint pbPoint in Path)
            {
                XElement xpt = new XElement("Point");
                xpt.Add(new XAttribute("X", pbPoint.Point.X));
                xpt.Add(new XAttribute("Y", pbPoint.Point.Y));

                xpt.Add(new XAttribute("LineType", pbPoint.LineType.ToString()));
                xpt.Add(new XAttribute("DashType", pbPoint.DashType.ToString()));
                xpt.Add(new XAttribute("CapType", pbPoint.CapType.ToString()));
                xpt.Add(new XAttribute("PrePoint", pbPoint.PrePoint.ToString()));
                xpt.Add(new XAttribute("IsZone", pbPoint.IsZone.ToString()));
                xpt.Add(new XAttribute("ZoneWidth", pbPoint.ZoneWidth.ToString()));
                xpt.Add(new XAttribute("ZoneHeight", pbPoint.ZoneHeight.ToString()));
                xpt.Add(new XAttribute("Opacity", pbPoint.Opacity.ToString()));

                xpt.Add(new XAttribute("ZoneR", pbPoint.ZoneColor.R.ToString()));
                xpt.Add(new XAttribute("ZoneG", pbPoint.ZoneColor.G.ToString()));
                xpt.Add(new XAttribute("ZoneB", pbPoint.ZoneColor.B.ToString()));
                xpt.Add(new XAttribute("R", pbPoint.StrokeColor.R.ToString()));
                xpt.Add(new XAttribute("G", pbPoint.StrokeColor.G.ToString()));
                xpt.Add(new XAttribute("B", pbPoint.StrokeColor.B.ToString()));

                parentElem.Add(xpt);
            }
        }
    }

    // 10-09-2011 Scott
    public class PBRouteList
    {
        private List<PBRoute> routes;
        public List<PBRoute> Routes
        {
            get
            {
                if (routes == null)
                {
                    routes = new List<PBRoute>();
                }
                return routes;
            }
        }

        public PBRouteList()
        {

        }

        public virtual void ReadXml(string file)
        {
            if (System.IO.File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);

                Routes.Clear();

                if (doc.Root.Name == "Route")
                {
                    PBRoute route = new PBRoute();
                    route.ReadXml(file);
                    Routes.Add(route);
                }

                if (doc.Root.Name == "Routes")
                {
                    foreach (XElement elem in doc.Descendants("Route"))
                    {
                        PBRoute route = new PBRoute();
                        route.ReadXml(elem);
                        Routes.Add(route);
                    }
                }
            }
        }

        public virtual void WriteXml(string file)
        {
            XDocument doc = new XDocument();

            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");

            doc.Declaration = declare;

            XElement rootElem = new XElement("Routes");

            foreach (PBRoute pbRoute in Routes)
            {
                XElement route = new XElement("Route");
                pbRoute.WriteXml(route);

                rootElem.Add(route);
            }

            doc.Add(rootElem);

            doc.Save(file);
        }
    }
}
