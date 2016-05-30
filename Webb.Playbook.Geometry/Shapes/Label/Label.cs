using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;
using System.Xml;

namespace Webb.Playbook.Geometry.Shapes
{
    public class PBLabel : LabelBase, IMovable
    {
        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);

            XAttribute attri = element.Attribute("Text");
            if (attri != null)
            {
                Text = attri.Value;
            }

            attri = element.Attribute("Angle");
            if (attri != null)
            {
                Angle = attri.Value.ToDouble(0);
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Text", Text);

            writer.WriteAttributeDouble("Angle", Angle);

            base.WriteXml(writer);
        }

        public void SaveLabel(string file)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }
                ))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Label");

                WriteXml(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            File.WriteAllText(file, Encoding.UTF8.GetString(stream.ToArray()));
        }

        public void LoadLabel(string file)
        {
            Point ptOld = this.Coordinates;

            if (System.IO.File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);

                XElement element = doc.Element("Label");

                if (element != null)
                {
                    ReadXml(element);

                    this.Coordinates = ptOld;

                    UpdateVisual();
                }
            }
        }

        public override IFigure HitTest(System.Windows.Point point)
        {
            if (Drawing != null && Drawing.Canvas != null)
            {
                Point pt = Canvas.TranslatePoint(ToPhysical(point), Shape);

                TextPointer dummy = Shape.ContentStart; // God !

                TextPointer tp = Shape.GetPositionFromPoint(pt, false);

                if (tp != null)
                {
                    return this;
                }
            }

            return null;
            //return base.HitTest(point);
        }

        public override void UpdateVisual()
        {
            base.UpdateVisual();

            this.Shape.RenderTransform = new RotateTransform(Angle, this.Shape.ActualWidth / 2, this.Shape.ActualHeight/2);

            this.Border.RenderTransform = new RotateTransform(Angle, this.Border.ActualWidth / 2, this.Border.ActualHeight / 2);
        }
    }
}
