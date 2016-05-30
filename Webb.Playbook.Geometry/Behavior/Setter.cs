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
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook.Geometry
{
    public class Setter : Behavior
    {
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool bSuccess = false;
            var coordinates = Coordinates(e);
            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
            if (selectedFigures != null && selectedFigures.Count() > 0)
            {
                PBPlayer qbPlayer = selectedFigures.ElementAt(0) as PBPlayer;

                if (qbPlayer != null && qbPlayer.Name == "QB")
                {
                    IFigure found = Drawing.Figures.HitTest(coordinates);

                    if (found != null && found is PBPlayer)
                    {
                        PBPlayer setPlayer = found as PBPlayer;

                        if (setPlayer.ScoutType == 0 && setPlayer != qbPlayer)
                        {
                            qbPlayer.AssignArray.Clear();
                            qbPlayer.SetArray.Clear();
                            qbPlayer.SetArray.Add(setPlayer.Name);

                            Drawing.SetDefaultBehavior();

                            bSuccess = true;
                        }
                    }
                }
            }

            if (!bSuccess)
            {
                MessageBox.Show("You must select a teammate !");
            }
        }

        public override string Name
        {
            get { return "Setter"; }
        }

        public override void Started()
        {
            base.Started();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Hand;
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
    }

    public class AssignManPress : Behavior
    {
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool bSuccess = false;

            var coordinates = Coordinates(e);
            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
            if (selectedFigures != null && selectedFigures.Count() > 0)
            {
                PBPlayer player = selectedFigures.ElementAt(0) as PBPlayer;

                if (player != null && player.ScoutType == 1)
                {
                    IFigure found = Drawing.Figures.HitTest(coordinates);

                    if (found != null && found is PBPlayer)
                    {
                        PBPlayer setPlayer = found as PBPlayer;

                        if (setPlayer.ScoutType == 0)
                        {
                            player.ManPress = setPlayer.Name;
                            player.ManCoverage = string.Empty;

                            Drawing.SetDefaultBehavior();

                            bSuccess = true;
                        }
                    }
                }
            }

            if (!bSuccess)
            {
                MessageBox.Show("You must select a player of opposing team!");
            }
        }

        public override string Name
        {
            get { return "Assign Man Press"; }
        }

        public override void Started()
        {
            base.Started();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Hand;
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
    }

    public class AssignManCoverage : Behavior
    {
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool bSuccess = false;

            var coordinates = Coordinates(e);
            IEnumerable<IFigure> selectedFigures = Drawing.GetSelectedFigures();
            if (selectedFigures != null && selectedFigures.Count() > 0)
            {
                PBPlayer player = selectedFigures.ElementAt(0) as PBPlayer;

                if (player != null && player.ScoutType == 1)
                {
                    IFigure found = Drawing.Figures.HitTest(coordinates);

                    if (found != null && found is PBPlayer)
                    {
                        PBPlayer setPlayer = found as PBPlayer;

                        if (setPlayer.ScoutType == 0)
                        {
                            player.ManCoverage = setPlayer.Name;
                            player.ManPress = string.Empty;

                            Drawing.SetDefaultBehavior();

                            bSuccess = true;
                        }
                    }
                }
            }

            if (!bSuccess)
            {
                MessageBox.Show("You must select a player of opposing team!");
            }
        }

        public override string Name
        {
            get { return "Assign Man Coverage"; }
        }

        public override void Started()
        {
            base.Started();

            if (Drawing != null && Drawing.Canvas != null)
            {
                Drawing.Canvas.Cursor = System.Windows.Input.Cursors.Hand;
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
    }
}
