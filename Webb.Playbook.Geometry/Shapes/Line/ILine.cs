using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public interface ILine : IFigure, ILinearFigure
    {
        PointPair Coordinates { get; }
    }
}
