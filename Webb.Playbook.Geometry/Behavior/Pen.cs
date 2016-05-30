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
    public class Pen : Behavior
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
            get { return "Pen"; }
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
                line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.RouteColor;
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
    }
}