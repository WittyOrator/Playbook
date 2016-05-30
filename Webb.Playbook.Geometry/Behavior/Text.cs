using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Game;
using Webb.Playbook.Data;

namespace Webb.Playbook.Geometry
{
    public class Text : Behavior
    {
        private static TextBox edit;
        public static TextBox Edit
        {
            get
            {
                if(edit == null)
                {
                    edit = new TextBox()
                    {
                        FontSize = 14,
                        AcceptsReturn = true,
                        FontFamily = new FontFamily("Arial"),
                        Foreground = Brushes.Black,
                    };

                    Panel.SetZIndex(edit, 10000);
                }

                return edit;
            }
        }

        private PBLabel label;
        public PBLabel Label
        {
            get { return label; }
            set { label = value; }
        }

        public void StartEdit(Point ptMouse)
        {
            if (Label != null)
            {
                Label.Visible = false;

                Label.UpdateVisual();
            }

            Edit.KeyDown += new System.Windows.Input.KeyEventHandler(tb_KeyDown);
            if (Label != null)
            {
                Edit.Text = Label.Text;
                Edit.FontFamily = Label.Shape.FontFamily;
                Edit.FontSize = Label.Shape.FontSize;
                Edit.Foreground = Label.Shape.Foreground;
            }
            Edit.SelectAll();
            Canvas.SetLeft(Edit, Label == null ? ptMouse.X - 5 : ptMouse.X);
            Canvas.SetTop(Edit, Label == null ? ptMouse.Y - 10 : ptMouse.Y);
            Drawing.Canvas.Children.Add(Edit);
            Edit.Focus();
        }

        public void EndEdit()
        {
            if (Edit.Text.Trim().IsEmpty())
            {
                return;
            }

            double left = Canvas.GetLeft(Edit);
            double top = Canvas.GetTop(Edit);

            if (Label == null)
            {
                PBLabel label = new PBLabel()
                {
                    Coordinates = ToLogical(new Point(left, top)),
                    Text = Edit.Text,
                };
                Drawing.Add(label, true);
            }
            else
            {
                Label.Text = Edit.Text;
            }

            CancelEdit();
        }

        public void CancelEdit()
        {
            Drawing.Canvas.Children.Remove(Edit);

            Drawing.SetDefaultBehavior();

            Edit.KeyDown -= new System.Windows.Input.KeyEventHandler(tb_KeyDown);

            if (Label != null)
            {
                Label.Visible = true;

                Label.UpdateVisual();

                Label = null;
            }

            if (Edit != null)
            {
                Edit.FontSize = 14;
                Edit.FontFamily = new FontFamily("Arial");
                Edit.Foreground = Brushes.Black;
            }
        }

        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!Drawing.Canvas.Children.Contains(Edit))
            {
                Point ptMouse = e.GetPosition(Drawing.Canvas);

                StartEdit(ptMouse);
            }
            else
            {
                EndEdit();
            }
        }

        void tb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            switch (e.Key)
            {
                //case System.Windows.Input.Key.Enter:
                //    {
                //        EndEdit();
                //    }
                //    break;
                case System.Windows.Input.Key.Escape:
                    {
                        CancelEdit();
                    }
                    break;
            }
        }

        public override void Started()
        {
            base.Started();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.IBeam;
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
            get { return "Text"; }
        }

        public override IntPtr WndProc(UIElement ui, UIElement uiRelatedTo, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            switch (msg)
            {
                case (int)MessageCommands.WM_LBUTTONDOWN:
                    if (!Drawing.Canvas.Children.Contains(Edit))
                    {
                        Point ptMouse = MessageHelper.GetPosition(lParam);

                        ptMouse = ui.TranslatePoint(ptMouse, uiRelatedTo);

                        StartEdit(ptMouse);
                    }
                    else
                    {
                        EndEdit();
                    }
                    break;
            }

            return base.WndProc(ui, uiRelatedTo, hwnd, msg, wParam, lParam, ref b);
        }
    }
}

