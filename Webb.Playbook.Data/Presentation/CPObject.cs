using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    public class CPObject
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private string image;
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public CPObject()
        {

        }

        public CPObject(string text, string image)
        {
            this.text = text;
            this.image = image;
        }
    }

    public class CPTextBox : CPObject
    {
        public CPTextBox(string text, string image)
            : base(text, image)
        {

        }

        public CPTextBox(string text)
        {
            Text = text;

            Image = AppDomain.CurrentDomain.BaseDirectory + @"\Resource\" + Text + ".png";
        }
    }

    public class CPShape : CPObject
    {
        public CPShape(string text, string image)
            : base(text, image)
        {

        }

        public CPShape(string text)
        {
            Text = text;

            Image = AppDomain.CurrentDomain.BaseDirectory + @"\Resource\" + Text + ".png";
        }
    }

    public class CoachingPointObjectInfo
    {
        private static CoachingPointObjectInfo instance;
        public static CoachingPointObjectInfo Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new CoachingPointObjectInfo(); 
                }

                return instance;
            }
        }

        public CoachingPointObjectInfo()
        {
            Init();
        }

        public void Init()
        {
            Shapes.Add(new CPShape("Line"));
            Shapes.Add(new CPShape("ArrowLine"));
            Shapes.Add(new CPShape("BlockLine"));
            Shapes.Add(new CPShape("Square"));
            Shapes.Add(new CPShape("Circle"));

            TextBoxes.Add(new CPTextBox("Bubble"));
        }

        private List<CPTextBox> textBoxes;
        public List<CPTextBox> TextBoxes
        {
            get 
            {
                if (textBoxes == null)
                {
                    textBoxes = new List<CPTextBox>();
                }
 
                return textBoxes; 
            }
            set { textBoxes = value; }
        }

        private List<CPShape> shapes;
        public List<CPShape> Shapes
        {
            get 
            {
                if (shapes == null)
                {
                    shapes = new List<CPShape>();
                }
 
                return shapes; 
            }
            set { shapes = value; }
        }
    }
}
