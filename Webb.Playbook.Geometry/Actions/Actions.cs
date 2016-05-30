using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Actions
{
    public class AddFigureAction : GeometryAction
    {
        public AddFigureAction(Drawing drawing, IFigure figure)
            : base(drawing)
        {
            Figure = figure;
        }

        public IFigure Figure { get; set; }

        protected override void ExecuteCore()
        {
            // 09-28-2010 Scott
            // handle pre snap motion line
            if (Figure is PrePoint)
            {
                IFigure pointStart = Figure as PrePoint;

                if (pointStart.Dependents.Count() == 2)
                {
                    IFigure lineAdd = pointStart.Dependents.ElementAt(1);

                    if (lineAdd.Dependencies != null && lineAdd.Dependencies.Count() == 2)
                    {
                        IFigure pointOld = lineAdd.Dependencies.ElementAt(0);

                        if (pointOld.Dependents.Count() == 3)
                        {// redo error
                            IFigure lineEnd = pointOld.Dependents.ElementAt(1);

                            foreach (IFigure f in pointOld.Dependents)
                            {
                                if (f.Dependencies.ElementAt(0) is PrePoint && !(f.Dependencies.ElementAt(1) is PrePoint) && f.Dependencies.ElementAt(1) is FreePoint)
                                {
                                    lineEnd = f;
                                }
                            }

                            pointOld.Dependents.Remove(lineEnd);

                            List<IFigure> dependencies = new List<IFigure>();
                            dependencies.Add(pointStart);
                            dependencies.Add(lineEnd.Dependencies.ElementAt(1));
                            lineEnd.Dependencies = dependencies;

                            pointStart.Dependents.RemoveAt(0);
                            pointStart.Dependents.Insert(0, lineEnd);
                            //continue
                        }
                    }
                }
            }

            Drawing.Figures.Add(Figure);
        }

        protected override void UnExecuteCore()
        {
            // 09-28-2010 Scott
            // handle pre snap motion line
            if (Figure is PBLine)
            {
                PBLine line = Figure as PBLine;

                if (line.Dependencies != null && line.Dependencies.Count() == 2)
                {
                    IFigure figureEnd = line.Dependencies.ElementAt(1);

                    IFigure figureStart = line.Dependencies.ElementAt(0);

                    if (figureEnd is PrePoint)
                    {
                        PrePoint pp = figureEnd as PrePoint;

                        if (pp.Dependents.Count() == 2)
                        {
                            IFigure lineEnd = pp.Dependents.ElementAt(1);

                            List<IFigure> dependencies = new List<IFigure>();
                            dependencies.Add(figureStart);
                            dependencies.Add(lineEnd.Dependencies.ElementAt(1));
                            lineEnd.Dependencies = dependencies;

                            if (figureStart.Dependents.Count() > 1)
                            {
                                figureStart.Dependents.RemoveAt(1);
                            }
                            figureStart.Dependents.Add(lineEnd);
                        }
                    }
                }
            }

            Drawing.Figures.Remove(Figure);
        }
    }

    public class RemoveFigureAction : GeometryAction
    {
        public RemoveFigureAction(Drawing drawing, IFigure figure)
            : base(drawing)
        {
            Figure = figure;
        }

        public IFigure Figure { get; set; }

        protected override void ExecuteCore()
        {
            Drawing.Figures.Remove(Figure);
        }

        protected override void UnExecuteCore()
        {
            Drawing.Figures.Add(Figure);
        }
    }

    public class RemoveFiguresAction : GeometryAction
    {
        public RemoveFiguresAction(Drawing drawing, IEnumerable<IFigure> figures)
            : base(drawing)
        {
            Figures = figures.ToArray();
        }

        public IEnumerable<IFigure> Figures { get; set; }

        protected override void ExecuteCore()
        {
            Drawing.Figures.Remove(Figures);
        }

        protected override void UnExecuteCore()
        {
            Drawing.Figures.AddRange(Figures);
        }
    }

    public class MoveAction : GeometryAction
    {
        public MoveAction(Drawing drawing, IEnumerable<IMovable> points, Point offset, IEnumerable<IFigure> toRecalculte)
            : base(drawing)
        {
            Points = points;
            Offset = offset;
            ToRecalculate = toRecalculte;
        }

        public IEnumerable<IFigure> ToRecalculate { get; set; }
        public IEnumerable<IMovable> Points { get; set; }
        public Point Offset { get; set; }

        protected override void ExecuteCore()
        {
            Points.Move(Offset);
            Recalculate();
        }

        void Recalculate()
        {
            if (ToRecalculate != null)
            {
                ToRecalculate.ForEach(f => f.RecalculateAndUpdateVisual());
            }
        }

        protected override void UnExecuteCore()
        {
            Points.Move(Offset.Minus());
            Recalculate();
        }

        public override bool TryToMerge(IAction FollowingAction)
        {
            MoveAction next = FollowingAction as MoveAction;
            if (next != null
                && next.Points == this.Points)
            {
                Points.Move(next.Offset);
                Offset = Offset.Plus(next.Offset);
                Recalculate();
                return true;
            }
            return false;
        }
    }
}