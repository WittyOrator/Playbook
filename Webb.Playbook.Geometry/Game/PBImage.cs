using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Game
{
    public class PBBall : PBImage, IPoint, IMovable
    {
        public PBBall()
            : base()
        {
            Init();
        }

        public void Init()
        {
            Width = 1.0f;
            Height = 1.0f;
            file = AppDomain.CurrentDomain.BaseDirectory + @"/Resource/Football.png";
        }

        public override void UpdateVisual()
        {
            Init();

            System.Windows.Point ptCenter = new System.Windows.Point(Center.X, Center.Y);
            var center = ToPhysical(ptCenter);
            switch (Webb.Playbook.Data.GameSetting.Instance.BallSize)
            {
                case Webb.Playbook.Data.BallSize.Large:
                    Shape.Width = ToPhysical(Width);
                    Shape.Height = ToPhysical(Height);
                    break;
                case Webb.Playbook.Data.BallSize.Medium:
                    Shape.Width = ToPhysical(Width * 3 / 4);
                    Shape.Height = ToPhysical(Height * 3 / 4);
                    break;
                case Webb.Playbook.Data.BallSize.Small:
                    Shape.Width = ToPhysical(Width / 2);
                    Shape.Height = ToPhysical(Height / 2);
                    break;
            }
            Shape.CenterAt(center);
            //end

            if (System.IO.File.Exists(File))
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource =
                    new BitmapImage(
                        new Uri(File, UriKind.RelativeOrAbsolute)
                    );

                Shape.Fill = imageBrush;
            }

            Shape.StrokeThickness = 0;
            Shape.Visibility = Visible ? Visibility.Visible : Visibility.Hidden;

            ZIndex = (int)ZOrder.Image;

            if (Selected)
            {
                Shape.Effect = PBEffects.SelectedEffect;
            }
            else
            {
                Shape.Effect = null;
            }
        }

        public override void MoveToCore(Point newLocation)
        {
            base.MoveToCore(newLocation);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);
        }
    }

    public class PBImage : PBRect, IPoint, IMovable
    {
        protected string file = string.Empty;
        public string File
        {
            get { return file; }
            set { file = value; }
        }

        protected override int DefaultZOrder()
        {
            return (int)ZOrder.Image;
        }

        public override void UpdateVisual()
        {
            //change by scott 01-19-2010
            //base.UpdateVisual();
            System.Windows.Point ptCenter = new System.Windows.Point(Center.X, Center.Y);
            var center = ToPhysical(ptCenter);
            Shape.Width = ToPhysical(Width);
            Shape.Height = ToPhysical(Height);
            Shape.CenterAt(center);
            //end

            if (System.IO.File.Exists(File))
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource =
                    new BitmapImage(
                        new Uri(File, UriKind.RelativeOrAbsolute)
                    );

                Shape.Fill = imageBrush;
            }

            if (Selected)
            {
                Shape.Effect = PBEffects.SelectedEffect;
            }
            else
            {
                Shape.Effect = null;
            }

            Shape.StrokeThickness = 0;
        }

        public override void MoveToCore(Point newLocation)
        {
            base.MoveToCore(newLocation);

            Drawing.Figures.UpdateVisual();
        }

        public override void VReverse()
        {

        }

        public override void HReverse()
        {

        }

        public override void BallHReverse(Point point)
        {

        }

        public override void BallVReverse(Point point)
        {

        }

        protected override void UpdateShapeAppearance()
        {

        }

        protected override void OnSelected(bool bSelected)
        {
            base.OnSelected(bSelected);

            if (bSelected)
            {
                ZIndex = (int)ZOrder.Selected;
            }
            else
            {
                ZIndex = (int)ZOrder.Image;
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            //base.WriteXml(writer);

            writer.WriteAttributeDouble("X", Coordinates.X);
            writer.WriteAttributeDouble("Y", Coordinates.Y);
            writer.WriteAttributeDouble("Width", Width);
            writer.WriteAttributeDouble("Height", Height);

            writer.WriteAttributeString("File", File);
        }

        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            //base.ReadXml(element);

            Center = new System.Windows.Point(element.ReadDouble("X"), element.ReadDouble("Y"));
            Width = element.ReadDouble("Width");
            Height = element.ReadDouble("Height");

            File = element.ReadString("File");
        }
    }
}
