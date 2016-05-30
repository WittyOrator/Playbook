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
    public class RunBlockTool : Behavior
    {
        public override string Name
        {
            get { return "Run Block"; }
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

            if (Drawing != null && Drawing.Canvas != null)
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

            IFigure found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner, typeof(Game.PBPlayer));

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
                            IFigure endfigure = sourcePlayer.GetEndFigure();

                            if (endfigure == null)
                            {
                                endfigure = source;
                            }

                            if (!(endfigure is Game.PBPlayer) && !(endfigure is Game.Zone) && (endfigure is FreePoint))
                            {
                                if (endfigure.Dependents.Count > 0)
                                {
                                    PBLine endline = endfigure.Dependents.ElementAt(0) as PBLine;

                                    if (endline != null /*&& endline.CapType == CapType.Arrow*/)
                                    {
                                        PBLine line = new PBLine();

                                        List<IFigure> dependencies = new List<IFigure>();
                                        dependencies.Add(endfigure);
                                        dependencies.Add(found);
                                        line.Dependencies = dependencies;

                                        sourcePlayer.ChangeLineType(CapType.Arrow);

                                        Drawing.Add(line);

                                        if (!Drawing.DrawingMode)
                                        {
                                            line.CapType = CapType.Block;

                                            line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                                        }

                                        Drawing.Figures.UpdateVisual();

                                        Drawing.SetDefaultBehavior();

                                        bSuccess = true;
                                    }
                                }
                            }
                            else if (endfigure == source)
                            {
                                PBLine line = new PBLine();

                                List<IFigure> dependencies = new List<IFigure>();
                                dependencies.Add(endfigure);
                                dependencies.Add(found);
                                line.Dependencies = dependencies;

                                sourcePlayer.ChangeLineType(CapType.Arrow);

                                Drawing.Add(line);

                                if (!Drawing.DrawingMode)
                                {
                                    line.CapType = CapType.Block;

                                    line.StrokeColor = Webb.Playbook.Data.ColorSetting.Instance.BlockColor;
                                }

                                Drawing.Figures.UpdateVisual();

                                Drawing.SetDefaultBehavior();

                                bSuccess = true;
                            }
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
    }
}
