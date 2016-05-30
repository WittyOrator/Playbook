using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public abstract class GeometryAction : AbstractAction
    {
        public GeometryAction(Drawing drawing)
        {
            Drawing = drawing;
        }

        public Drawing Drawing { get; set; }
    }
}
