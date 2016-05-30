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
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Shapes.Creators;
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook.Geometry
{
    public class PostSnapMotion : Behavior
    {
        //protected override Webb.Playbook.Geometry.Lists.DependencyList InitExpectedDependencies()
        //{
        //    return DependencyList.PointPoint;
        //}

        //protected override IFigure CreateFigure()
        //{
        //    return Factory.CreateArrowLine(Drawing, FoundDependencies);
        //}

        public override string Name
        {
            get { return "Post Snap Motion"; }
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

        /// <summary>
        /// Assumes coordinates are logical already
        /// </summary>
        /// <param name="coordinates">Logical coordinates of the click point</param>
        protected override void  MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 11-18-2010 Scott
            if (e.ClickCount == 2)
            {
                Drawing.SetDefaultBehavior();
                return;
            }

            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
            PBPlayer player = null;

            if (selectedFigures != null && selectedFigures.Count() > 0)
            {
                player = selectedFigures.ElementAt(0) as PBPlayer;

                if (player == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
            //Type expectedType = ExpectedDependency;
            System.Windows.Point coordinates = Coordinates(e);
            IFigure underMouse = null;
            underMouse = Drawing.Figures.HitTest(coordinates);
            if (underMouse == player)
            {
                Drawing.SetDefaultBehavior();
                return;
            }

            IFigure endFigure = player.GetEndFigure();
            if (endFigure is Game.Zone)
            {
                return;

            }

            player.ChangeLineType(CapType.Arrow);

            FreePoint pp = CreatePointAtCurrentPosition(coordinates, false);
            FigureList fl = new FigureList();
            if (endFigure != null)
            {
                if (endFigure is Game.PBPlayer)
                {
                    player.ClearEndPath();
                    endFigure = player.GetEndFigure();
                    if (endFigure != null)
                    {
                        fl.Add(endFigure);
                    }
                    else
                    {
                        fl.Add(player);
                    }

                }
                else
                {
                    fl.Add(endFigure);
                }
            }
            else
            {
                fl.Add(player);
            }
            fl.Add(pp);
            PBLine line = Factory.CreateArrowLine(Drawing, fl);
            if (!Drawing.DrawingMode)
            {
                line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.RouteColor;
            }
            FigureList figureList = new FigureList();
            figureList.Add(line);
            figureList.Add(pp);

            Drawing.Add(figureList as IEnumerable<IFigure>);

            Drawing.Figures.CheckConsistency();
            Drawing.Figures.UpdateVisual();
            //if (TempPoint != null)
            //{
            //    underMouse = Drawing.Figures.HitTest(
            //        coordinates,
            //        typeof(IPoint));
            //}
            //else if (expectedType != null)
            //{
            //    underMouse = Drawing.Figures.HitTest(coordinates, expectedType);
            //}
            //else
            //{
            //    underMouse = Drawing.Figures.HitTest(coordinates);
            //}

            //if (underMouse != null
            //    && underMouse != TempPoint
            //    && ((FoundDependencies.Contains(underMouse) && !CanReuseDependency())
            //        || underMouse == TempResult))
            //{
            //    return;
            //}

            //IFigure endPointFigure = player.GetEndFigure();
            //if (TempPoint == null && underMouse != player && underMouse != endPointFigure)
            //{
            //    return;
            //}

            //if (ExpectingAPoint())
            //{
            //    if (underMouse == null)
            //    {
            //       underMouse = CreatePointAtCurrentPosition(coordinates, true);
            //       FigureList fl = new FigureList();
            //       fl.Add(player);
            //       if (underMouse != null)
            //       {
            //           fl.Add(underMouse);
            //       }
            //       PBLine line = Factory.CreateArrowLine(Drawing, fl);
            //       FigureList figureList = new FigureList();
            //       figureList.Add(line);
            //       figureList.Add(underMouse);
            //       Drawing.Add(figureList as IEnumerable<IFigure>);
            //       return;
            //    }
            //    else
            //    {
            //        // one branch only
            //        if (underMouse is Webb.Playbook.Geometry.Game.PBPlayer && underMouse.Dependents.Count > 0)
            //        {
            //            foreach (IFigure dep in underMouse.Dependents)
            //            {
            //                if (dep.Dependencies != null && dep.Dependencies.Count() > 1 && dep.Dependencies.ElementAt(0) == underMouse)
            //                {
            //                    return;
            //                }
            //            }

            //            if (underMouse != player)
            //            {
            //                return;
            //            }
            //        }
            //        // at most two branch
            //        else if (underMouse is IPoint && underMouse.Dependents.Count > 1)
            //        {
            //            return;
            //        }
            //        else if (underMouse is Zone)
            //        {
            //            return;
            //        }
            //    }
            //}

            //RemoveIntermediateFigureIfNecessary();

            //if (TempPoint != null)
            //{
            //    //if (underMouse == TempPoint || underMouse == TempResult || underMouse == null)
            //    //{
            //    underMouse = CreatePointAtCurrentPosition(coordinates, true);
            //    //}
            //    TempPoint.SubstituteWith(underMouse);
            //    FoundDependencies.Remove(TempPoint);
            //    Drawing.Figures.Remove(TempPoint);
            //    TempPoint = null;
            //}

            //if (ExpectedDependency != null)
            //{
            //    AddFoundDependency(underMouse);
            //}

            //if (ExpectedDependency != null)
            //{
            // if (ExpectingAPoint())
            //    {
            //        player.ChangeLineType(CapType.Arrow);   // 01-07-2010 Scott

            //        TempPoint = CreateTempPoint(coordinates);
            //        AddFoundDependency(TempPoint);
            //        if (ExpectedDependency == null)
            //        {
            //            CreateAndAddFigure();

            //            Drawing.Figures.UpdateVisual();
            //        }
            //    }
            //    AddIntermediateFigureIfNecessary();
            //    AdvertiseNextDependency();
            //}
            //else
            //{
            //    Finish();

            //    //07-22-2009 scott
            //    if (IsMouseButtonDown)
            //    {// click mode
            //        IFigure endFigure = Drawing.Figures.HitTest(coordinates, typeof(Webb.Playbook.Geometry.Game.PBPlayer));
            //        if (endFigure is Webb.Playbook.Geometry.Game.PBPlayer)
            //        {
            //            Drawing.ActionManager.Undo();
            //        }
            //        else
            //        {
            //            MouseLeftButtonDown(sender, e as System.Windows.Input.MouseButtonEventArgs);
            //        }
            //    }
            //    else
            //    {// drag mode 

            //    }
            //}

            //Drawing.Figures.CheckConsistency();
            //Drawing.Figures.UpdateVisual();
        }
    }
}