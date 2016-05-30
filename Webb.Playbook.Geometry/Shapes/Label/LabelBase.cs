using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Windows;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public abstract class LabelBase : CoordinatesShapeBase<TextBlock>
    {
        private System.Windows.Controls.Border border;
        public System.Windows.Controls.Border Border
        {
            get
            {
                if (border == null)
                {
                    border = new Border()
                    {
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new System.Windows.Thickness(0),
                    };
                }
                return border;
            }
        }

        protected override int DefaultZOrder()
        {
            return (int)ZOrder.Labels;
        }

        public override TextBlock CreateShape()
        {
            return Factory.CreateLabelShape();
        }

        private double fontSize = 32;
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }

        public string fontFamily = "Arial";
        public string FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
            }
        }

        private Color textColor = Colors.Black;
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
            }
        }

        private Color fillColor = Colors.White;
        public Color FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        private double borderWidth = 0;
        public double BorderWidth
        {
            get
            {
                return borderWidth;
            }
            set
            {
                borderWidth = value;
            }
        }

        private Color borderColor = Colors.Black;
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        public string Text
        {
            get
            {
                return Shape.Text;
            }
            set
            {
                Shape.Text = value;
            }
        }

        public override IFigure HitTest(System.Windows.Point point)
        {
            double left = Canvas.GetLeft(Shape);
            double top = Canvas.GetTop(Shape);
            point = ToPhysical(point);

            if (left <= point.X
                && left + Shape.ActualWidth >= point.X
                && top <= point.Y
                && top + Shape.ActualHeight >= point.Y)
            {
                return this;
            }
            return null;
        }

        protected override void UpdateShapeAppearance()
        {
            Shape.Foreground = new SolidColorBrush(TextColor);

            if (base.Selected)
            {
                Shape.Effect = PBEffects.SelectedEffect;

                Border.Effect = PBEffects.SelectedEffect;
            }
            else
            {
                Shape.Effect = null;

                Border.Effect = null;
            }
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);

            XAttribute attriX = element.Attribute("X");
            XAttribute attriY = element.Attribute("Y");
            if (attriX != null && attriY != null)
            {
                Coordinates = new System.Windows.Point(attriX.Value.ToDouble(0), attriY.Value.ToDouble(0));
            }
            
            // 11-12-2010 Scott
            if (element.Attribute("FontFamily") != null)
            {
                FontFamily = element.ReadString("FontFamily");
            }
            if (element.Attribute("FontSize") != null)
            {
                FontSize = element.ReadDouble("FontSize");
            }
            if (element.Attribute("BorderWidth") != null)
            {
                BorderWidth = element.ReadDouble("BorderWidth");
            }
            if (element.Elements("Color") != null && element.Elements("Color").Any(e => e.Attribute("Name").Value == "TextColor"))
            {
                TextColor = element.ReadElementColor("TextColor");
            }
            if (element.Elements("Color") != null && element.Elements("Color").Any(e => e.Attribute("Name").Value == "FillColor"))
            {
                FillColor = element.ReadElementColor("FillColor");
            }
            if (element.Elements("Color") != null && element.Elements("Color").Any(e => e.Attribute("Name").Value == "BorderColor"))
            {
                BorderColor = element.ReadElementColor("BorderColor");
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeDouble("X", Coordinates.X);
            writer.WriteAttributeDouble("Y", Coordinates.Y);

            // 11-12-2010 Scott
            writer.WriteAttributeString("FontFamily", FontFamily);
            writer.WriteAttributeDouble("FontSize", FontSize);
            writer.WriteAttributeDouble("BorderWidth", BorderWidth);
            writer.WriteElementColor("TextColor", TextColor);
            if (!(FillColor == Colors.Transparent))
            {
                writer.WriteElementColor("FillColor", FillColor);
            }
            writer.WriteElementColor("BorderColor", BorderColor);
        }

        public override void UpdateVisual()
        {
            base.UpdateVisual();

            Shape.FontSize = ToFontSize(FontSize);

            Shape.FontFamily = new FontFamily(FontFamily);

            Shape.Background = new SolidColorBrush(FillColor);

            Border.Width = Shape.ActualWidth + BorderWidth * 2;
            Border.Height = Shape.ActualHeight + BorderWidth * 2;
            Canvas.SetTop(Border, Canvas.GetTop(Shape) - BorderWidth);
            Canvas.SetLeft(Border, Canvas.GetLeft(Shape) - BorderWidth);
            Border.BorderThickness = new System.Windows.Thickness(BorderWidth);
            Border.BorderBrush = new SolidColorBrush(BorderColor);

            UpdateShapeAppearance();
        }

        public double ToFontSize(double size)
        {
            if (Canvas != null && Canvas.ActualWidth > 0)
            {
                return (Canvas.ActualWidth / 1000) * size; // 11-12-2010 Scott
            }

            return size;
        }

        public double FromFontSize(double size)
        {
            if (Canvas != null && Canvas.ActualWidth > 0)
            {
                return (1000 / Canvas.ActualWidth) * size; // 11-12-2010 Scott
            }

            return size;
        }

        public override void OnAddingToCanvas(Canvas newContainer)
        {
            base.OnAddingToCanvas(newContainer);

            if (!newContainer.Children.Contains(border))
            {
                newContainer.Children.Add(Border);
            }
        }

        public override void OnRemovingFromCanvas(Canvas leavingContainer)
        {
            base.OnRemovingFromCanvas(leavingContainer);

            if (leavingContainer.Children.Contains(border))
            {
                leavingContainer.Children.Remove(Border);
            }
        }

        public override void BallHReverse(System.Windows.Point point)
        {
            base.BallHReverse(point);

            Coordinates = new Point(2 * point.X - Coordinates.X, Coordinates.Y);
        }

        public override void BallVReverse(System.Windows.Point point)
        {
            base.BallVReverse(point);

            Coordinates = new Point(2 * point.X - Coordinates.X, 2 * point.Y - Coordinates.Y);
        }

        public override void HReverse()
        {
            base.HReverse();

            Coordinates = new Point(-Coordinates.X, Coordinates.Y);
        }

        public override void VReverse()
        {
            base.VReverse();

            Coordinates = new Point(-Coordinates.X, -Coordinates.Y);
        }
    }
}
