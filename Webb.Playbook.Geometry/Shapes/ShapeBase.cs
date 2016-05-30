using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

using System.Windows.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public abstract class ShapeBase<TShape> : FigureBase
        where TShape : UIElement
    {
        public ShapeBase()
        {
            Shape = CreateShape();
            ZIndex = DefaultZOrder();
        }

        protected virtual int DefaultZOrder()
        {
            return (int)ZOrder.Figures;
        }

        public virtual double Opacity   // 11-04-2010 Scott
        {
            get 
            {
                if (Shape != null)
                {
                    return Shape.Opacity;
                }
                return 0; 
            }
            set
            {
                if (Shape != null)
                {
                    Shape.Opacity = value;
                }
            }
        }

        private TShape shape;
        public TShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }

        public override int ZIndex
        {
            get
            {
                return base.ZIndex;
            }
            set
            {
                base.ZIndex = value;
                if (shape != null)
                {
                    Canvas.SetZIndex(shape, ZIndex);
                }
            }
        }

        // Create Shape
        public abstract TShape CreateShape();

        public void MoveTo(Point newPosition)
        {
            MoveToCore(newPosition);
            UpdateVisual();
        }

        // Move Shape
        public virtual void MoveToCore(Point newPosition)
        {

        }

        // Set Shape's Parameters
        public override void UpdateVisual()
        {
            
        }

        protected void UpdateVisible()  // 08-18-2009 scott
        {
            if (Visible)
            {
                Shape.Visibility = Visibility.Visible;
            }
            else
            {
                Shape.Visibility = Visibility.Hidden;
            }
        }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                UpdateShapeAppearance();
                OnSelected(base.Selected);
            }
        }

        protected virtual void OnSelected(bool bSelected)
        {

        }

        protected Brush oldBrush;
        protected Brush selectedBrush;
        public virtual Brush SelectedBrush
        {
            get
            {
                if (selectedBrush == null)
                {
                    selectedBrush = Brushes.Red;
                }
                return selectedBrush;
            }
        }
        // Highlight Shape
        protected virtual void UpdateShapeAppearance()
        {
            System.Windows.Shapes.Shape shape = Shape as System.Windows.Shapes.Shape;
            if (shape == null)
            {
                return;
            }
            if (base.Selected)
            {
                oldBrush = shape.Stroke;
                shape.Stroke = SelectedBrush;
            }
            else
            {
                if (oldBrush != null)
                {
                    shape.Stroke = oldBrush;
                }
            }
        }

        public override void OnAddingToCanvas(Canvas newContainer)
        {
            base.OnAddingToCanvas(newContainer);
            newContainer.Children.Add(Shape);
        }

        public override void OnRemovingFromCanvas(Canvas leavingContainer)
        {
            base.OnRemovingFromCanvas(leavingContainer);
            leavingContainer.Children.Remove(Shape);
        }
    }
}