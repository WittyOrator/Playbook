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
    public class PassBlockTool : Behavior
    {
        public override string Name
        {
            get { return "Pass Block"; }
        }

        public override string HintText
        {
            get
            {
                return "Use this tool to draw run block line.";
            }
        }

        private Point offsetFromFigureLeftTopCorner;
        private Point oldCoordinates;

        public override void Started()
        {
            base.Started();
        
            if(Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = Cursors.Hand;
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

        protected override void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool bSuccess = false;
            offsetFromFigureLeftTopCorner = Coordinates(e);
            oldCoordinates = offsetFromFigureLeftTopCorner;

            IFigure found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);

            if (found != null && found is Game.PBPlayer)
            {
                Game.PBPlayer targetPlayer = found as Game.PBPlayer;

                IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

                if (figures != null && figures.Count() > 0)
                {
                    IFigure source = figures.ElementAt(0);

                    if (source is Game.PBPlayer)
                    {
                        Game.PBPlayer sourcePlayer = source as Game.PBPlayer;

                        if (sourcePlayer.ScoutType != targetPlayer.ScoutType)
                        {
                            sourcePlayer.Assignment = targetPlayer.Name;

                            Drawing.SetDefaultBehavior();

                            bSuccess = true;
                        }
                    }
                }
            }

            if (!bSuccess)
            {
                MessageBox.Show("You must select a player of opposing team !");
            }
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {

        }

        protected override void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private static bool IsCtrlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        public void AddPassBlock(Geometry.Drawing drawing, Geometry.Game.PBPlayer player)
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
                player.ChangeLineType(CapType.Block);
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
                    line.CapType = CapType.Block;
                    line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                }
                //player.Assignment = "Pass Block";
            }
        }
    }
}

