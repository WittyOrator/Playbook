using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry
{
    public class PlayerCreator : Behavior
    {
        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var coordinates = Coordinates(e);
            CreatePlayerAtCurrentPosition(coordinates, true);
        }

        public override string Name
        {
            get { return "Player"; }
        }
    }
}
