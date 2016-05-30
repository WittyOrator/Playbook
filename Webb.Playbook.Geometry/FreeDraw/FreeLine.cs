using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Xml.Linq;

namespace Webb.Playbook.Geometry.FreeDraw
{
    public class FreeLine
    {
        public FreeLine()
        {
            Line = new Line();
        }

        private double thickness = 1;
        public double Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        private Color stroke = Colors.Black;
        public Color Stroke
        {
            get
            {
                return stroke;
            }
            set
            {
                stroke = value;
            }
        }

        private Point p1 = new Point();
        public Point P1
        {
            get { return p1; }
            set { p1 = value; }
        }

        private Point p2 = new Point();
        public Point P2
        {
            get { return p2; }
            set { p2 = value; }
        }

        private Line line;
        public Line Line
        {
            get { return line; }
            set { line = value; }
        }

        public void UpdateVisual(Drawing draw)
        {
            if (draw != null)
            {
                line.X1 = draw.CoordinateSystem.ToPhysical(p1).X;
                line.Y1 = draw.CoordinateSystem.ToPhysical(p1).Y;
                line.X2 = draw.CoordinateSystem.ToPhysical(p2).X;
                line.Y2 = draw.CoordinateSystem.ToPhysical(p2).Y;

                line.Stroke = new SolidColorBrush(Stroke);
                line.StrokeThickness = thickness;
            }
        }

        public void SaveXml(XElement elem)
        {
            elem.Add(new XAttribute("X1", P1.X));
            elem.Add(new XAttribute("Y1", P1.Y));
            elem.Add(new XAttribute("X2", P2.X));
            elem.Add(new XAttribute("Y2", P2.Y));
            elem.Add(new XAttribute("Thickness", Thickness));
            elem.WriteElementColor("Color", Stroke);
        }

        public void LoadXml(XElement elem)
        {
            if (elem.Name == "FreeDrawLine")
            {
                P1 = new Point(elem.Attribute("X1").Value.ToDouble(0), elem.Attribute("Y1").Value.ToDouble(0));
                P2 = new Point(elem.Attribute("X2").Value.ToDouble(0), elem.Attribute("Y2").Value.ToDouble(0));
                Thickness = elem.Attribute("Thickness").Value.ToDouble(1);
                Stroke = elem.ReadElementColor("Color");
            }
        }
    }
}

