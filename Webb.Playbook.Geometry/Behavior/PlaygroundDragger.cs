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
    public class PlaygroundDragger : Behavior
    {
        public override string Name
        {
            get { return "Playground Dragger"; }
        }

        public override string HintText
        {
            get
            {
                return "Use this tool to drag playground";
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

            if(found is SubPoint)
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
            if (oneMovable != null&&!(oneMovable is Game.Zone))
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
                if (!(found is Game.Zone))
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
                if (figuresSelected.All(f => f is Game.PBPlayer))
                {
                    moving.Clear();
                    moving.AddRange(Drawing.GetSelectedFigures().Cast<IMovable>());
                }
            }

            if (moving.IsEmpty())
            {// coordinate system

                Drawing.Canvas.Cursor = new Cursor(AppDomain.CurrentDomain.BaseDirectory + @"\Resource\CursorHand.cur");    //01-04-2010 scott

                //moving.Add(Drawing.CoordinateSystem);

                //var allFigures = Drawing.Figures.GetAllFiguresRecursive();    // remove by scott 06-24-2009
                //roots = DependencyAlgorithms.FindRoots(f => f.Dependencies, allFigures);
                //moving.AddRange(roots.OfType<IMovable>());
                //roots = null;
                toRecalculate = null; // Figures;
            }
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            Point ptCoordinates = Coordinates(e);

            var currentCoordinates = ptCoordinates;

            if (!moving.IsEmpty())
            {
                //var offset = currentCoordinates.Minus(oldCoordinates);

                ////03-09-2010 Scott
                //foreach (IMovable mov in moving)
                //{
                //    if (mov.Coordinates.X > CoordinateSystem.LogicalPlaygroundWidth / 2 - 0.5 && offset.X > 0 || mov.Coordinates.X < -CoordinateSystem.LogicalPlaygroundWidth / 2 + 0.5 && offset.X < 0)
                //    {
                //        offset.X = 0;
                //    }

                //    if (mov.Coordinates.Y > CoordinateSystem.LogicalPlaygroundHeight / 2 - 0.5 && offset.Y > 0 || mov.Coordinates.Y < -CoordinateSystem.LogicalPlaygroundHeight / 2 + 0.5 && offset.Y < 0)
                //    {
                //        offset.Y = 0;
                //    }
                //    if(mov is Game.Zone)
                //    {
                //        if (mov.Coordinates.X + (mov as Game.Zone).Width / 2 > CoordinateSystem.LogicalPlaygroundWidth / 2 - 0.5 && offset.X > 0 || mov.Coordinates.X-(mov as Game.Zone).Width / 2 < -CoordinateSystem.LogicalPlaygroundWidth / 2 + 0.5 && offset.X < 0)
                //        {
                //            offset.X = 0;
                //        }
                //        if (mov.Coordinates.Y + (mov as Game.Zone).Height / 2 > CoordinateSystem.LogicalPlaygroundHeight / 2 - 0.5 && offset.Y > 0 || mov.Coordinates.Y- (mov as Game.Zone).Height / 2 < -CoordinateSystem.LogicalPlaygroundHeight / 2 + 0.5 && offset.Y< 0)
                //        {
                //            offset.Y= 0;
                //        }
                //    }
                //}

                //MoveAction action = new MoveAction(Drawing, moving, offset, toRecalculate);
                //Drawing.ActionManager.RecordAction(action);
            }
            else
            {// move coordinate system 01-04-2010 scott
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    var offset = currentCoordinates.Minus(oldCoordinates);

                    if (this.Drawing != null && this.Drawing.Canvas != null)
                    {
                        Drawing.Canvas.Margin = new Thickness(Drawing.Canvas.Margin.Left + ToPhysical(offset.X)/2, Drawing.Canvas.Margin.Top - ToPhysical(offset.Y)/2,
                            Drawing.Canvas.Margin.Right - ToPhysical(offset.X)/2, Drawing.Canvas.Margin.Bottom + ToPhysical(offset.Y)/2);
                        //Drawing.CoordinateSystem.Origin = new Point(Drawing.CoordinateSystem.Origin.X + offset.X, Drawing.CoordinateSystem.Origin.Y - offset.Y);
                    }
                }
            }

            oldCoordinates = ptCoordinates;
            /*if (moving != null && moving.Count == 1 && moving[0] is IPoint)
            {
                oldCoordinates = moving[0].Coordinates;
            }*/

            //if (!moving.IsEmpty())
            //{
            //    if (moving.First() is Game.PBBall)  // 09-28-2010 Scott
            //    {
            //        Drawing.Figures.UpdateVisual();
            //    }
            //    else
            //    {
            //        moving.OfType<Game.PBPlayer>().ForEach(p => p.GetPathFigure(p, null).ForEach(f => f.UpdateVisual()));   // 10-20-2010 Scott
            //    }
            //}
        }

        protected override void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Add by scott 11-06-2009
            if (Drawing.HasGridLines)
            {
                Point ptCoordinates = Coordinates(e);

                if ( moving != null && moving.Count > 0 && ( moving.Count == 1 && moving[0] is Game.PBPlayer || moving[0] is Game.PBBall || moving.Count > 1 ) )
                {
                    IMovable mv = moving[0];

                    double x = mv.Coordinates.X % 0.5;
                    double y = mv.Coordinates.Y % 0.5;
                    if (Math.Abs(x) > 0.25)
                    {
                        if (x >= 0)
                        {
                            ptCoordinates.X = ((int)(mv.Coordinates.X / 0.5) + 1) * 0.5;
                        }
                        else
                        {
                            ptCoordinates.X = ((int)(mv.Coordinates.X / 0.5) - 1) * 0.5;
                        }
                    }
                    else
                    {
                        ptCoordinates.X = (int)(mv.Coordinates.X / 0.5) * 0.5;
                    }
                    if (Math.Abs(y) > 0.25)
                    {
                        if (y >= 0)
                        {
                            ptCoordinates.Y = ((int)(mv.Coordinates.Y / 0.5) + 1) * 0.5;
                        }
                        else
                        {
                            ptCoordinates.Y = ((int)(mv.Coordinates.Y / 0.5) - 1) * 0.5;
                        }
                    }
                    else
                    {
                        ptCoordinates.Y = (int)(mv.Coordinates.Y / 0.5) * 0.5;
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
    }
}
