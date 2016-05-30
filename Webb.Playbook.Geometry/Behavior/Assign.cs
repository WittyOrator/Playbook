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
    public class Assign : Behavior
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
                            qbPlayer.SetArray.Clear();
                            qbPlayer.AssignArray.Clear();
                            qbPlayer.AssignArray.Add(setPlayer.Name);

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
            get { return "Assign"; }
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
