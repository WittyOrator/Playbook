using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Actions;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry
{
    public class Selector : Behavior
    {
        public override string Name
        {
            get { return "Selector"; }
        }

        public override string HintText
        {
            get
            {
                return "Use this tool to select object";
            }
        }

        //public Rectangle SelectRect = new Rectangle()
        //{
        //    Stroke = Brushes.Red,
        //    StrokeThickness = 0.5,
        //    Visibility = Visibility.Visible,
        //};

        private List<IMovable> moving = null;
        private IEnumerable<IFigure> toRecalculate = null;
        private Point offsetFromFigureLeftTopCorner;
        private Point oldCoordinates;

        public override void Started()
        {
            base.Started();
        }

        protected override void Reset()
        {
            base.Reset();
        }

        protected override void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.MouseLeftButtonDown(sender, e);

            if (e.Handled)
            {
                return;
            }

            offsetFromFigureLeftTopCorner = Coordinates(e);
            oldCoordinates = offsetFromFigureLeftTopCorner;

            moving = new List<IMovable>();
            IEnumerable<IFigure> roots = null;

            IFigure found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);
            IEnumerable<IFigure> selection = null;

            // 11-12-2010 Scott
            if (IsCtrlPressed())
            {
                if (found != null)
                {
                    found.Selected = !found.Selected;
                }
                selection = Drawing.GetSelectedFigures();
            }
            else
            {
                if (found == null || !found.Selected)
                {
                    Drawing.ClearSelectedFigures();
                    if (found != null && (found is Game.PBPlayer || found is LabelBase || found is LineBase))   // 11-19-2010 Scott
                    {
                        found.Selected = true;
                        selection = found.AsEnumerable();
                    }
                }
            }

            Drawing.RaiseSelectionChanged(new Drawing.SelectionChangedEventArgs(Drawing.GetSelectedFigures()));

            IMovable oneMovable = found as IMovable;
            if (oneMovable != null)
            {
                offsetFromFigureLeftTopCorner = offsetFromFigureLeftTopCorner.Minus(oneMovable.Coordinates);
                moving.Add(oneMovable);
                roots = found.AsEnumerable();
            }
            else if (found != null)
            {
                if (!(found is Game.Zone) && !(found is LineBase))  // 11-19-2010 Scott
                {
                    roots = DependencyAlgorithms.FindRoots(f => f.Dependencies, found);
                    if (roots.All(root => root is IMovable))
                    {
                        moving.AddRange(roots.Cast<IMovable>());
                    }
                }
            }

            if (roots != null)
            {
                toRecalculate = DependencyAlgorithms.FindDescendants(f => f.Dependents, roots).Reverse();
            }
            else
            {
                toRecalculate = null;
            }

            if (moving.IsEmpty())
            {
                toRecalculate = null;
            }

            //if (found == null)
            //{
            //    if (!Drawing.Canvas.Children.Contains(SelectRect))
            //    {
            //        Drawing.Canvas.Children.Add(SelectRect);
            //    }
            //    System.Windows.Controls.Canvas.SetLeft(SelectRect, e.GetPosition(Drawing.Canvas).X);
            //    System.Windows.Controls.Canvas.SetTop(SelectRect, e.GetPosition(Drawing.Canvas).Y);
            //    SelectRect.Visibility = Visibility.Visible;
            //}
            Drawing.Figures.UpdateVisual();
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            //var currentCoordinates = Coordinates(e);
            //if (!moving.IsEmpty())
            //{
            //    var offset = currentCoordinates.Minus(oldCoordinates);
            //    MoveAction action = new MoveAction(Drawing, moving, offset, toRecalculate);
            //    Drawing.ActionManager.RecordAction(action);
            //}
            //oldCoordinates = Coordinates(e);

            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    SelectRect.Visibility = Visibility.Visible;

            //    var currentCoordinates = Coordinates(e);
            //    var offset = currentCoordinates.Minus( oldCoordinates);
            //    double width = 0;
            //    double height = 0;
                
            //    if (offset.X > 0)
            //    {
            //        width = SelectRect.ActualWidth + ToPhysical(offset.X);
            //    }
            //    else
            //    {
            //        width = SelectRect.ActualWidth + ToPhysical(offset.X);
            //    }

            //    if (offset.Y > 0)
            //    {
            //        height = SelectRect.ActualHeight - ToPhysical(offset.Y);
            //    }
            //    else
            //    {
            //        height = SelectRect.ActualHeight - ToPhysical(offset.Y);
            //    }
                
            //    if (width < 0)
            //    {
            //        System.Windows.Controls.Canvas.SetLeft(SelectRect, ToPhysical(currentCoordinates).X);
            //        SelectRect.Width = -width;
            //    }
            //    else
            //    {
            //        SelectRect.Width = width;
            //    }

            //    if (height < 0)
            //    {
            //        System.Windows.Controls.Canvas.SetTop(SelectRect, ToPhysical(currentCoordinates).Y);
            //        SelectRect.Height = -height;
            //    }
            //    else
            //    {
            //        SelectRect.Height = height;
            //    }

            //    oldCoordinates = Coordinates(e);
            //}
        }

        protected override void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.MouseLeftButtonUp(sender, e);
            //if (SelectRect.Width != 0 && SelectRect.Height != 0)
            //{
            //    Rect rect = new Rect();
            //    Point fPoint = new Point(System.Windows.Controls.Canvas.GetLeft(SelectRect), System.Windows.Controls.Canvas.GetTop(SelectRect));
            //    Point lPoint = Drawing.CoordinateSystem.ToLogical(fPoint);
            //    rect.X = lPoint.X;
            //    rect.Y = lPoint.Y;
            //    rect.Width = ToLogical(SelectRect.ActualWidth);
            //    rect.Height = ToLogical(SelectRect.ActualHeight);
            //    System.Collections.ObjectModel.ReadOnlyCollection<IFigure> selection = Drawing.Figures.HitTestMany(rect);
            //    Drawing.ClearSelectedFigures();

            //    foreach (IFigure f in selection)
            //    {
            //        f.Selected = true;
            //    }

            //    Drawing.RaiseSelectionChanged(new Drawing.SelectionChangedEventArgs(Drawing.GetSelectedFigures()));
            //}

            //Drawing.Canvas.Children.Remove(SelectRect);
            //SelectRect.Width = 0;
            //SelectRect.Height = 0;
            //SelectRect.Visibility = Visibility.Visible;
            moving = null;
        }

        private static bool IsCtrlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }
    }
}
