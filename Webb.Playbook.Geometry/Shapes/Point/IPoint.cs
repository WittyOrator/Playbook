using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Shapes
{
    public interface IPoint : IFigure
    {
        Point Coordinates { get; }
    }
}
