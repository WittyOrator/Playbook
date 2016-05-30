using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Input;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Shapes.Creators;
using Webb.Playbook.Geometry.Game;
using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Actions;

namespace Webb.Playbook.Geometry
{
    public class PassBlockAreaTool : Behavior
    {
        public override string Name
        {
            get { return "Pass Block Area"; }
        }

        private Point offsetFromFigureLeftTopCorner;
        private Point oldCoordinates;

        protected override void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            offsetFromFigureLeftTopCorner = Coordinates(e);
            oldCoordinates = offsetFromFigureLeftTopCorner;

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() > 0)
            {
                IFigure source = figures.ElementAt(0);

                if (source is Game.PBPlayer)
                {
                    Game.PBPlayer sourcePlayer = source as Game.PBPlayer;

                    IFigure endFigure = sourcePlayer.GetEndFigure();

                    if (endFigure == null)
                    {
                        endFigure = source;
                    }

                    if (!(endFigure is Game.PBPlayer) && !(endFigure is Game.Zone) && (endFigure is FreePoint))
                    {
                        if (endFigure.Dependents.Count > 0)
                        {
                            PBLine endline = endFigure.Dependents.ElementAt(0) as PBLine;

                            if (endline != null && endline.CapType == CapType.Arrow)
                            {
                                IFigure underMouse = CreatePointAtCurrentPosition(offsetFromFigureLeftTopCorner, false);

                                PBLine line = new PBLine();

                                List<IFigure> dependencies = new List<IFigure>();
                                dependencies.Add(endFigure);
                                dependencies.Add(underMouse);
                                line.Dependencies = dependencies;

                                Drawing.Add(line);

                                if (!Drawing.DrawingMode)
                                {
                                    line.CapType = CapType.BlockArea;
                                    line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                                }

                                Drawing.Figures.UpdateVisual();
                            }
                        }
                    }
                    else if (endFigure == source)
                    {
                        IFigure underMouse = CreatePointAtCurrentPosition(offsetFromFigureLeftTopCorner, false);

                        PBLine line = new PBLine();

                        List<IFigure> dependencies = new List<IFigure>();
                        dependencies.Add(endFigure);
                        dependencies.Add(underMouse);
                        line.Dependencies = dependencies;

                        Drawing.Add(line);

                        if (!Drawing.DrawingMode)
                        {
                            line.CapType = CapType.BlockArea;
                            line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                        }

                        Drawing.Figures.UpdateVisual();
                    }
                }
            }
        }

        public void AddPassBlockArea(Geometry.Drawing drawing, Geometry.Game.PBPlayer player)
        {
            IEnumerable<IFigure> figures = player.GetPathPointFigure(player, null, 0);

            if (figures.Count() > 0)
            {// have path
                IFigure figure = player.GetEndFigure();

                if (figure is Game.PBPlayer)
                {
                    IEnumerable<IFigure> allFigures = player.GetPathFigure(player, null, 0);
                    IFigure lineFigure = allFigures.Last();
                    drawing.Remove(lineFigure);
                }
                player.ChangeLineType(CapType.BlockArea);
            }
            else
            {// no path
                player.ClearPath();

                IFigure source = player as IFigure;
                var result = Factory.CreateSubPoint(drawing, new Point(player.Coordinates.X, player.Coordinates.Y - 0.8));
                FigureList dependencies = new FigureList();
                dependencies.Add(source);
                dependencies.Add(result);
                PBLine line = Factory.CreateLine(drawing, dependencies);
                List<IFigure> figuresPassBlock = new List<IFigure>();
                figuresPassBlock.Add(line);
                figuresPassBlock.Add(result);
                drawing.Add(figuresPassBlock);
                if (!drawing.DrawingMode)
                {
                    line.CapType = CapType.BlockArea;
                    line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                }
                //player.Assignment = "Pass Block Area";
            }

            drawing.Figures.UpdateVisual();
        }
    }
}
