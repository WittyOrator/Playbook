using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Data;

namespace Webb.Playbook.Geometry.Game
{
    public class ZoneManager
    {
        public static Zone ThirdsDeepMiddle
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(0, 20),
                    Height = 10,
                    Width = 18,
                };
            }
        }

        public static Zone ThirdsDeepRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-18, 20),
                    Height = 10,
                    Width = 18,
                };
            }
        }

        public static Zone ThirdsDeepLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(18, 20),
                    Height = 10,
                    Width = 18,
                };
            }
        }

        public static Zone FlatLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(21, 5),
                    Height = 10,
                    Width = 12,
                };
            }
        }

        public static Zone FlatRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-21, 5),
                    Height = 10,
                    Width = 12,
                };
            }
        }

        public static Zone CurlLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(11, 5),
                    Height = 10,
                    Width = 8,
                };
            }
        }

        public static Zone CurlRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-11, 5),
                    Height = 10,
                    Width = 8,
                };
            }
        }

        public static Zone HookFullMiddle
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(0, 5),
                    Height = 10,
                    Width = 14,
                };
            }
        }

        public static Zone HalvesDeepRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-13.5, 20),
                    Height = 10,
                    Width = 27,
                };
            }
        }

        public static Zone HalvesDeepLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(13.5, 20),
                    Height = 10,
                    Width = 27,
                };
            }
        }

        public static Zone HookLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(3.5, 5),
                    Height = 10,
                    Width = 7,
                };
            }
        }

        public static Zone HookRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-3.5, 5),
                    Height = 10,
                    Width = 7,
                };
            }
        }

        public static Zone CurlFlatLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(17, 5),
                    Height = 10,
                    Width = 20,
                };
            }
        }

        public static Zone CurlFlatRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-17, 5),
                    Height = 10,
                    Width = 20,
                };
            }
        }

        public static Zone CurlHookLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(7.5, 5),
                    Height = 10,
                    Width = 15,
                };
            }
        }

        public static Zone CurlHookRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-7.5, 5),
                    Height = 10,
                    Width = 15,
                };
            }
        }

        public static Zone QuartersDeepMidLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(7, 20),
                    Height = 10,
                    Width = 14,
                };
            }
        }

        public static Zone QuartersDeepMidRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-7, 20),
                    Height = 10,
                    Width = 14,
                };
            }
        }

        public static Zone QuartersDeepLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(20.5, 20),
                    Height = 10,
                    Width = 13,
                };
            }
        }

        public static Zone QuartersDeepRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-20.5, 20),
                    Height = 10,
                    Width = 13,
                };
            }
        }

        public static Zone OutRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-22, 15),
                    Height = 10,
                    Width = 10,
                };
            }
        }

        public static Zone OutLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(22, 15),
                    Height = 10,
                    Width = 10,
                };
            }
        }

        public static Zone SwingLeft
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(13.5, 2.5),
                    Height = 5,
                    Width = 27,
                };
            }
        }

        public static Zone SwingRight
        {
            get
            {
                return new Zone()
                {
                    Center = new System.Windows.Point(-13.5, 2.5),
                    Height = 5,
                    Width = 27,
                };
            }
        }
    }

    public class Zone : PBRect, IPoint,IMovable
    {
        private Color fillColor = Webb.Playbook.Data.ColorSetting.Instance.ZoneColor;
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        private double transparency = 0.5;
        public double Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }

        protected override int DefaultZOrder()
        {
            return (int)ZOrder.Zone;
        }

        public override IFigure HitTest(System.Windows.Point point)
        {//03-11-2010 Scott
            if (Rectangle != null)
            {
                if (point.X > Center.X - Width / 2 + CoordinateSystem.LogicalMidPos.X && point.X < Center.X + Width / 2 + CoordinateSystem.LogicalMidPos.X
                    && point.Y > Center.Y - Height / 2 && point.Y < Center.Y + Height / 2)
                {
                    return this;
                }
            }

            return null;

            //return base.HitTest(point);
        }

        public override void UpdateVisual()
        {
            //change by scott 01-19-2010
            //base.UpdateVisual();
            System.Windows.Point ptCenter = new System.Windows.Point(Center.X + CoordinateSystem.LogicalMidPos.X, Center.Y);
            var center = ToPhysical(ptCenter);
            Shape.Width = ToPhysical(Width);
            Shape.Height = ToPhysical(Height);
            Shape.CenterAt(center);
            //end

            Shape.Fill = new SolidColorBrush()
            {
                Color = FillColor,
                Opacity = Transparency
            };

            Shape.StrokeThickness = 0.5;
        }

        public override void VReverse() // 12-25-2009 Scott
        {
            base.VReverse();
            Coordinates = new System.Windows.Point(-Coordinates.X, -Coordinates.Y);
            MoveTo(Coordinates);
        }

        public override void HReverse() // 12-25-2009 Scott
        {
            base.HReverse();
            Coordinates = new System.Windows.Point(-Coordinates.X, Coordinates.Y);
            MoveTo(Coordinates);
        }
        public override void BallHReverse(System.Windows.Point point)
        {
            base.BallHReverse(point);
            Coordinates = new System.Windows.Point(2 * point.X - Coordinates.X, Coordinates.Y);
            MoveTo(Coordinates);

        }
        public override void BallVReverse(System.Windows.Point point)
        {
            base.BallVReverse(point);
            Coordinates = new System.Windows.Point(2 * point.X - Coordinates.X, 2 * point.Y -
                Coordinates.Y);
            MoveTo(Coordinates);
        }
        protected override void UpdateShapeAppearance()
        {
            if (Selected)
            {
                Shape.Effect = PBEffects.SelectedEffect;
            }
            else
            {
                Shape.Effect = null; 
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            //base.WriteXml(writer);

            writer.WriteAttributeDouble("X", Coordinates.X);
            writer.WriteAttributeDouble("Y", Coordinates.Y);
            writer.WriteAttributeDouble("Width", Width);
            writer.WriteAttributeDouble("Height", Height);
            writer.WriteAttributeDouble("Transparency", Transparency);
            writer.WriteElementColor("FillColor", FillColor);
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            //base.ReadXml(element);

            Center = new System.Windows.Point(element.ReadDouble("X"), element.ReadDouble("Y"));
            Width = element.ReadDouble("Width");
            Height = element.ReadDouble("Height");
            if (element.Attribute("Transparency") != null)
            {
                Transparency = element.ReadDouble("Transparency");
            }
            if (element.Element("FillColor") != null)
            {
                FillColor = element.ReadElementColor("FillColor");
            }
        }
    }
}
