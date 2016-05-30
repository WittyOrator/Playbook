using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Win32;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook.Geometry
{
    public class Image : Behavior
    {
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Drawing != null)
            {
                Point ptPhy = Coordinates(e);

                Microsoft.Win32.OpenFileDialog ofDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Title = "Insert Background",
                    Filter = Webb.Playbook.Data.Extensions.ImageFileFilter,
                };

                if (ofDialog.ShowDialog().Value)
                {
                    ImageBrush imgBrush = new ImageBrush()
                    {
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(ofDialog.FileName, UriKind.RelativeOrAbsolute)),
                    };

                    System.Drawing.Image image = System.Drawing.Image.FromFile(ofDialog.FileName);

                    PBImage pbImg = new PBImage()
                    {
                        Coordinates = ptPhy,
                        File = ofDialog.FileName,

                        Width = ToLogical(image.Width),
                        Height = ToLogical(image.Height),
                    };

                    Drawing.Add(pbImg);
                    pbImg.MoveTo(pbImg.Coordinates);
                }
            
                Drawing.SetDefaultBehavior();
            }
        }

        public override void Started()
        {
            base.Started();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Cross;
            }
        }

        public override void Stopping()
        {
            base.Stopping();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = null;
            }
        }

        public override string Name
        {
            get { return "Image"; }
        }
    }
}

