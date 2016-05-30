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
    public class Dragger : Behavior
    {
        public override string Name
        {
            get { return "Dragger"; }
        }

        double SnapUnit
        {
            get
            {
                return 0.5 / 18 * Webb.Playbook.Data.GameSetting.Instance.SnapValue;
            }
        }

        public override void Started()
        {
            base.Started();
        }

        public override string HintText
        {
            get
            {
                return "Use this tool to drag object";
            }
        }

        private int mode;
        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private List<IMovable> moving = null;
        private IEnumerable<IFigure> toRecalculate = null;
        private Point offsetFromFigureLeftTopCorner;
        private Point oldCoordinates;

        // 07-18-2011
        public System.Windows.Input.Cursor CheckCursorForResize(Point pt, Rectangle rect)
        {
            System.Windows.Input.Cursor cursor = null;
            double space = 3;

            if ((pt.X >= 0 && pt.X <= space) || (pt.X >= rect.ActualWidth - space && pt.X <= rect.ActualWidth) && pt.Y >= 0 && pt.Y <= rect.ActualHeight)
            {
                cursor = System.Windows.Input.Cursors.SizeWE;
            }

            if ((pt.Y >= 0 && pt.Y <= space) || (pt.Y >= rect.ActualHeight - space && pt.Y <= rect.ActualHeight) && pt.X >= 0 && pt.Y <= rect.ActualHeight)
            {
                cursor = System.Windows.Input.Cursors.SizeNS;
            }

            if ((pt.X >= 0 && pt.X <= space) && (pt.Y >= 0 && pt.Y <= space) || (pt.X >= rect.ActualWidth - space && pt.X <= rect.ActualWidth) && (pt.Y >= rect.ActualHeight - space && pt.Y <= rect.ActualHeight))
            {
                cursor = System.Windows.Input.Cursors.SizeNWSE;
            }

            if ((pt.X >= 0 && pt.X <= space) && (pt.Y >= rect.ActualHeight - space && pt.Y <= rect.ActualHeight) || (pt.Y >= 0 && pt.Y <= space) && (pt.X >= rect.ActualWidth - space && pt.X <= rect.ActualWidth))
            {
                cursor = System.Windows.Input.Cursors.SizeNESW;
            }

            if(cursor != null)
            {
                Drawing.Canvas.Cursor = cursor;
            }
            else
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Arrow;
            }

            return cursor;
        }

        protected override void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.MouseLeftButtonDown(sender, e);

            if (e.Handled)
            {
                return;
            }

            Point pt = e.GetPosition(Parent);

            LButtonDown(pt);
        }

        private void LButtonDown(Point pt)
        {
            offsetFromFigureLeftTopCorner =  this.ToLogical(pt);

            oldCoordinates = offsetFromFigureLeftTopCorner;

            moving = new List<IMovable>();
            IEnumerable<IFigure> roots = null;

            IFigure found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);
            IEnumerable<IFigure> selection = null;

            if (found is SubPoint)
            {// can't move sub point 01-11-10 scott
                return;
            }
            // remove by scott 08-25-2009
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
                    if (found != null)
                    {
                        found.Selected = true;
                        selection = found.AsEnumerable();
                    }
                }
            }

            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();

            Drawing.RaiseSelectionChanged(new Drawing.SelectionChangedEventArgs(selectedFigures));

            IMovable oneMovable = found as IMovable;
            if (oneMovable != null /*&&!(oneMovable is Game.Zone)* 03-10-2011 Scott*/)
            {
                /*if (oneMovable is IPoint)
                {
                    // when we drag a point, we want it to snap to the cursor
                    // so that the point center is directly under the tip of the mouse
                    offsetFromFigureLeftTopCorner = new Point();
                    oldCoordinates = oneMovable.Coordinates;
                }
                else
                {*/
                // however when we drag other stuff (such as text labels)
                // we want the mouse to always touch the part of the draggable
                // where it first touched during MouseDown
                // we don't want the draggable to "snap" to the cursor like points do
                offsetFromFigureLeftTopCorner = offsetFromFigureLeftTopCorner.Minus(oneMovable.Coordinates);
                /*}*/
                moving.Add(oneMovable);
                roots = found.AsEnumerable();
            }
            else if (found != null)
            {
                if (!(found is Game.Zone) && !(found is LineBase))  // 11-18-2010 Scott
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

            //03-09-2010 Scott
            if (moving.Count > 0 && moving[0] is Game.PBBall && mode == 1)
            {
                Drawing.Figures.OfType<IMovable>().ForEach(f =>
                {
                    if (!(f is Game.PBBall) && !(f is SubPoint))
                    {
                        moving.Add(f);
                    }
                }
                );

            }
            else if (found != null)
            {
                IEnumerable<IFigure> figuresSelected = Drawing.GetSelectedFigures();
                if (figuresSelected.All(f => f is Game.PBPlayer || f is LabelBase || f is FreePoint || f is PrePoint || f is PBLine)) // 11-12-2010 Scott add labelbase support
                {
                    moving.Clear();
                    IEnumerable<IFigure> fs = Drawing.GetSelectedFigures();
                    // 08-04-11 Scott
                    foreach (IFigure f in fs)
                    {
                        if (f is PBLine)
                        {
                            foreach (IFigure pf in (f as PBLine).Dependencies)
                            {
                                if (pf is IMovable && !moving.Contains(pf as IMovable))
                                {
                                    moving.Add(pf as IMovable);
                                }
                            }
                        }

                        if (f is IMovable)
                        {
                            moving.Add(f as IMovable);
                        }
                    }
                    // end
                }
            }

            if (moving.IsEmpty())
            {// coordinate system

                //Drawing.Canvas.Cursor = new Cursor(AppDomain.CurrentDomain.BaseDirectory + @"\Resource\CursorHand.cur");    //01-04-2010 scott

                //moving.Add(Drawing.CoordinateSystem);

                //var allFigures = Drawing.Figures.GetAllFiguresRecursive();    // remove by scott 06-24-2009
                //roots = DependencyAlgorithms.FindRoots(f => f.Dependencies, allFigures);
                //moving.AddRange(roots.OfType<IMovable>());
                //roots = null;
                toRecalculate = null; // Figures;
            }
        }

        private PBRect rectResize = null;
        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            base.MouseMove(sender, e);

            Point pt = e.GetPosition(Parent);

            MouseMove(pt);
        }

        private void MouseMove(Point pt)
        {
            Point ptCoordinates = ToLogical(pt);

            var currentCoordinates = ptCoordinates;

            // 07-18-2011
            IFigure found = Drawing.Figures.HitTest(ptCoordinates);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Drawing.Canvas.Cursor != System.Windows.Input.Cursors.Arrow)
                {
                    if (rectResize != null)
                    {
                        if (Drawing.Canvas.Cursor == System.Windows.Input.Cursors.SizeWE)
                        {
                            rectResize.Width = System.Math.Abs(ptCoordinates.X - rectResize.Center.X) * 2;
                        }

                        if (Drawing.Canvas.Cursor == System.Windows.Input.Cursors.SizeNS)
                        {
                            rectResize.Height = System.Math.Abs(ptCoordinates.Y - rectResize.Center.Y) * 2;
                        }

                        if (Drawing.Canvas.Cursor == System.Windows.Input.Cursors.SizeNESW || Drawing.Canvas.Cursor == System.Windows.Input.Cursors.SizeNWSE)
                        {
                            rectResize.Width = System.Math.Abs(ptCoordinates.X - rectResize.Center.X) * 2;
                            rectResize.Height = System.Math.Abs(ptCoordinates.Y - rectResize.Center.Y) * 2;
                        }

                        rectResize.UpdateVisual();
                    }

                    return;
                }
            }
            else
            {
                if (found is PBRect && !Behavior.DrawVideo)
                {
                    PBRect pbRect = found as PBRect;

                    Rectangle rect = pbRect.Shape as Rectangle;

                    Point ptRect = Parent.TranslatePoint(pt, rect);

                    if (CheckCursorForResize(ptRect, rect) != null)
                    {
                        rectResize = pbRect;

                        return;
                    }
                }
                else
                {
                    rectResize = null;
                    Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Arrow;
                }
            }
            // end

            if (!moving.IsEmpty())
            {
                var offset = currentCoordinates.Minus(oldCoordinates);

                //03-09-2010 Scott
                foreach (IMovable mov in moving)
                {
                    if (mov.Coordinates.X > CoordinateSystem.LogicalPlaygroundWidth / 2 - SnapUnit && offset.X > 0 || mov.Coordinates.X < -CoordinateSystem.LogicalPlaygroundWidth / 2 + SnapUnit && offset.X < 0)
                    {
                        offset.X = 0;
                    }

                    if (mov.Coordinates.Y > CoordinateSystem.LogicalPlaygroundHeight / 2 - SnapUnit && offset.Y > 0 || mov.Coordinates.Y < -CoordinateSystem.LogicalPlaygroundHeight / 2 + SnapUnit && offset.Y < 0)
                    {
                        offset.Y = 0;
                    }
                    if (mov is Game.Zone)
                    {
                        if (mov.Coordinates.X + (mov as Game.Zone).Width / 2 > CoordinateSystem.LogicalPlaygroundWidth / 2 - SnapUnit && offset.X > 0 || mov.Coordinates.X - (mov as Game.Zone).Width / 2 < -CoordinateSystem.LogicalPlaygroundWidth / 2 + SnapUnit && offset.X < 0)
                        {
                            offset.X = 0;
                        }
                        if (mov.Coordinates.Y + (mov as Game.Zone).Height / 2 > CoordinateSystem.LogicalPlaygroundHeight / 2 - SnapUnit && offset.Y > 0 || mov.Coordinates.Y - (mov as Game.Zone).Height / 2 < -CoordinateSystem.LogicalPlaygroundHeight / 2 + SnapUnit && offset.Y < 0)
                        {
                            offset.Y = 0;
                        }
                    }
                }

                MoveAction action = new MoveAction(Drawing, moving, offset, toRecalculate);
                Drawing.ActionManager.RecordAction(action);
            }
            else
            {// move coordinate system 01-04-2010 scott
                //if (Mouse.LeftButton == MouseButtonState.Pressed)
                //{
                //    var offset = currentCoordinates.Minus(oldCoordinates);

                //    if (this.Drawing != null && this.Drawing.Canvas != null)
                //    {
                //        Drawing.Canvas.Margin = new Thickness(Drawing.Canvas.Margin.Left + ToPhysical(offset.X)/2, Drawing.Canvas.Margin.Top - ToPhysical(offset.Y)/2,
                //            Drawing.Canvas.Margin.Right - ToPhysical(offset.X)/2, Drawing.Canvas.Margin.Bottom + ToPhysical(offset.Y)/2);
                //        //Drawing.CoordinateSystem.Origin = new Point(Drawing.CoordinateSystem.Origin.X + offset.X, Drawing.CoordinateSystem.Origin.Y - offset.Y);
                //    }
                //}
            }

            oldCoordinates = ptCoordinates;
            /*if (moving != null && moving.Count == 1 && moving[0] is IPoint)
            {
                oldCoordinates = moving[0].Coordinates;
            }*/

            if (!moving.IsEmpty())
            {
                if (moving.First() is Game.PBBall)  // 09-28-2010 Scott
                {
                    Drawing.Figures.UpdateVisual();
                }
                else
                {
                    moving.OfType<Game.PBPlayer>().ForEach(p => p.GetAllPathFigure().ForEach(f => f.UpdateVisual()));   // 10-20-2010 Scott
                }
            }
        }

        protected override void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.MouseLeftButtonUp(sender, e);

            Point pt = e.GetPosition(Parent);

            LButtonUp(pt);
        }

        private void LButtonUp(Point pt)
        {
            // Add by scott 11-06-2009
            if (Drawing.HasGridLines && Webb.Playbook.Data.GameSetting.Instance.SnapToGrid)
            {
                Point ptCoordinates = ToLogical(pt);

                if (moving != null && moving.All(f => f is LabelBase))
                {

                }
                else if (moving != null && moving.Count > 0 && (moving.Count == 1 && moving[0] is Game.PBPlayer || moving[0] is Game.PBBall || moving.Count > 1))
                {
                    IMovable mv = moving[0];

                    double x = mv.Coordinates.X % SnapUnit;
                    double y = mv.Coordinates.Y % SnapUnit;
                    if (Math.Abs(x) > SnapUnit / 2)
                    {
                        if (x >= 0)
                        {
                            ptCoordinates.X = ((int)(mv.Coordinates.X / SnapUnit) + 1) * SnapUnit;
                        }
                        else
                        {
                            ptCoordinates.X = ((int)(mv.Coordinates.X / SnapUnit) - 1) * SnapUnit;
                        }
                    }
                    else
                    {
                        ptCoordinates.X = (int)(mv.Coordinates.X / SnapUnit) * SnapUnit;
                    }
                    if (Math.Abs(y) > SnapUnit / 2)
                    {
                        if (y >= 0)
                        {
                            ptCoordinates.Y = ((int)(mv.Coordinates.Y / SnapUnit) + 1) * SnapUnit;
                        }
                        else
                        {
                            ptCoordinates.Y = ((int)(mv.Coordinates.Y / SnapUnit) - 1) * SnapUnit;
                        }
                    }
                    else
                    {
                        ptCoordinates.Y = (int)(mv.Coordinates.Y / SnapUnit) * SnapUnit;
                    }
                    //ptCoordinates.X = Math.Round(player.Coordinates.X, 0);
                    //ptCoordinates.Y = Math.Round(player.Coordinates.Y, 0);

                    var offset = ptCoordinates.Minus(mv.Coordinates);

                    MoveAction action = new MoveAction(Drawing, moving, offset, toRecalculate);
                    Drawing.ActionManager.RecordAction(action);
                }
            }

            if (moving != null && moving.IsEmpty())
            {//01-04-2010 scott
                Drawing.Canvas.Cursor = null;
            }

            moving = null;

            Drawing.Figures.UpdateVisual();
        }

        private static bool IsCtrlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        public override IntPtr WndProc(UIElement ui, UIElement uiRelatedTo, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            switch (msg)
            {
                case (int)Data.MessageCommands.WM_LBUTTONDOWN:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        LButtonDown(pt);
                    }
                    break;
                case (int)Data.MessageCommands.WM_LBUTTONUP:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        LButtonUp(pt);
                    }
                    break;
                case (int)Data.MessageCommands.WM_RBUTTONDOWN:
                    break;
                case (int)Data.MessageCommands.WM_RBUTTONUP:
                    break;
                case (int)Data.MessageCommands.WM_MOUSEMOVE:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        MouseMove(pt);
                    }
                    break;
                default:
                    break;
            }

            return base.WndProc(ui, uiRelatedTo, hwnd, msg, wParam, lParam, ref b);
        }
    }
}
