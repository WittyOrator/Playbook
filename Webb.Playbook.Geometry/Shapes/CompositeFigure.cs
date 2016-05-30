using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public class CompositeFigure : FigureList
    {
        public CompositeFigure()
        {
            Name = this.GetType().Name + FigureBase.ID;
        }

        protected override void OnItemAdded(IFigure item)
        {
            item.RegisterWithDependencies();
        }

        protected override void OnItemRemoved(IFigure item)
        {
            item.UnregisterFromDependencies();
        }
    }
}
