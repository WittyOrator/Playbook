using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Geometry.Shapes.Creators;
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook.Geometry
{
    public class PreSnapMotion : Behavior
    {
        public override string Name
        {
            get { return "Pre Snap Motion"; }
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

        // 09-26-2011 Scott
        /// <summary>
        /// Assumes coordinates are logical already
        /// </summary>
        /// <param name="coordinates">Logical coordinates of the click point</param>
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                Drawing.SetDefaultBehavior();
                return;
            }

            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
            PBPlayer player = null;
            IFigure endFigure = null;

            if (selectedFigures != null && selectedFigures.Count() > 0)
            {
                player = selectedFigures.ElementAt(0) as PBPlayer;
            }

            //Type expectedType = ExpectedDependency;
            System.Windows.Point coordinates = Coordinates(e);
            IFigure underMouse = null;
            underMouse = Drawing.Figures.HitTest(coordinates);
            if (underMouse is Game.PBPlayer)
            {
                if (underMouse == player)
                {
                    Drawing.SetDefaultBehavior();
                    return;
                }

                player = underMouse as Game.PBPlayer;

                Drawing.ClearSelectedFigures();
                player.Selected = true;
                Drawing.Figures.UpdateVisual();
                return;
            }
            else if (underMouse is IPoint && (underMouse as IPoint).Dependents.Count == 1)
            {
                Drawing.ClearSelectedFigures();
                underMouse.Selected = true;
                Drawing.Figures.UpdateVisual();
                return;
            }

            if (player == null)
            {
                if (selectedFigures != null && selectedFigures.Count() > 0)
                {
                    IFigure figure = selectedFigures.ElementAt(0);
                    if (figure is IPoint && (figure as IPoint).Dependents.Count == 1)
                    {
                        endFigure = figure;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {// 09-26-2011 Scott
                endFigure = null;
                //endFigure = player.GetEndFigure();
                //player.ChangeLineType(CapType.Arrow);

                //if (endFigure is Game.Zone)
                //{
                //    return;
                //}
            }

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
                line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.PreSnapMotionColor;
            }
            FigureList figureList = new FigureList();
            figureList.Add(line);
            figureList.Add(pp);

            Drawing.Add(figureList as IEnumerable<IFigure>);

            Drawing.ClearSelectedFigures();
            pp.Selected = true;

            Drawing.Figures.CheckConsistency();
            Drawing.Figures.UpdateVisual();
        }
        //protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    // 11-18-2010 Scott
        //    if (e.ClickCount == 2)
        //    {
        //        Drawing.SetDefaultBehavior();
        //        return;
        //    }

        //    IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
        //    PBPlayer player = null;

        //    if (selectedFigures != null && selectedFigures.Count() > 0)
        //    {
        //        player = selectedFigures.ElementAt(0) as PBPlayer;

        //        if (player == null)
        //        {
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    System.Windows.Point coordinates = Coordinates(e);

        //    IFigure underMouse = null;
        //    underMouse = Drawing.Figures.HitTest(coordinates);

        //    if (underMouse == player)
        //    {//quit 01-11-2010 scott
        //        Drawing.SetDefaultBehavior();
                
        //        return;
        //    }

        //    PrePoint endPrePoint = player.GetEndPrePoint();
        //    IFigure startFigure = player.GetStartFreeFigure();
        //    FreePoint pp = CreatePointAtCurrentPosition(coordinates, false);
        //    FigureList fl = new FigureList();
        //    if(endPrePoint != null)
        //    {
        //        fl.Add(endPrePoint);
        //    }
        //    else
        //    {
        //        fl.Add(player);
        //    }
        //    fl.Add(pp);
        //    PBLine line = Factory.CreateArrowLine(Drawing, fl);
        //    if (!Drawing.DrawingMode)
        //    {
        //        line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.PreSnapMotionColor;
        //    }
        //    FigureList figureList = new FigureList();
        //    figureList.Add(line);
        //    figureList.Add(pp);
        //    Drawing.Add(figureList as IEnumerable<IFigure>);

        //    if (startFigure != null)
        //    {
        //        startFigure.Dependents.ElementAt(0).ReplaceDependency(0, pp);
        //    }

        //    Drawing.Figures.CheckConsistency();
        //    Drawing.Figures.UpdateVisual();
        //}
        //end

        protected override void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            return;
        }

        protected override FreePoint CreatePointAtCurrentPosition(System.Windows.Point coordinates, bool recordAction)
        {
            var result = Factory.CreatePrePoint(Drawing, coordinates);
            AddFigure(result, recordAction);
            return result;
        }
    }
}
