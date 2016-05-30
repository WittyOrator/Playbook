using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Shapes
{
    public class RootFigureList : CompositeFigure
    {
        //public override void Recalculate()
        //{
        //    var allFigures = this.GetAllFiguresRecursive();
        //    var figuresSorted = DependencyAlgorithms.TopologicalSort(allFigures, f => f.Dependencies);
        //    foreach (var figure in figuresSorted)
        //    {
        //        if (figure != this)
        //        {
        //            figure.UpdateExistence();
        //            figure.RecalculateAndUpdateVisual();
        //        }
        //    }
        //}

        protected override void OnItemAdded(IFigure item)
        {
            base.OnItemAdded(item);
            item.OnAddingToCanvas(Drawing.Canvas);
            item.UpdateVisual();
        }

        protected override void OnItemRemoved(IFigure item)
        {
            item.OnRemovingFromCanvas(Drawing.Canvas);
            base.OnItemRemoved(item);
        }
    }
}
