using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry
{
    public class GridLines
    {
        List<IFigure> gridFigures = new List<IFigure>();

        Drawing drawing;

        bool hasGridLines;
        public bool HasGridLines
        {
            get { return hasGridLines; }
        }

        public GridLines(Drawing drawing)
        {
            this.drawing = drawing;

            CreateFigures();

            Add();
            Hide();
        }

        public void CreateFigures()
        {
            // grid
            for (double y = -30; y <= 30; y += 1)
            {
                ImmovablePoint point1 = new ImmovablePoint()
                {
                    X = -27,
                    Y = y
                };

                ImmovablePoint point2 = new ImmovablePoint()
                {
                    X = 27,
                    Y = y
                };

                List<IFigure> dependencies = new List<IFigure>();
                dependencies.Add(point1);
                dependencies.Add(point2);
                PBLine line = new PBLine()
                {
                    Dependencies = dependencies,
                    StrokeColor = y % 2 == 0 ? System.Windows.Media.Colors.AliceBlue : System.Windows.Media.Colors.LightBlue,
                    StrokeThickness = 0.5,
                };

                gridFigures.Add(point1);
                gridFigures.Add(point2);
                gridFigures.Add(line);
            }

            for (double x = -27; x <= 27; x += 1)
            {
                ImmovablePoint point1 = new ImmovablePoint()
                {
                    X = x,
                    Y = -30
                };

                ImmovablePoint point2 = new ImmovablePoint()
                {
                    X = x,
                    Y = 30
                };

                List<IFigure> dependencies = new List<IFigure>();
                dependencies.Add(point1);
                dependencies.Add(point2);
                PBLine line = new PBLine()
                {
                    Dependencies = dependencies,
                    StrokeColor = x % 2 == 0 ? System.Windows.Media.Colors.LightBlue : System.Windows.Media.Colors.AliceBlue,
                    StrokeThickness = 0.5,
                };

                gridFigures.Add(point1);
                gridFigures.Add(point2);
                gridFigures.Add(line);
            }
        }

        public void Remove()
        {
            gridFigures.ForEach(f => drawing.Figures.Remove(f));
        }

        public void Add()
        {
            gridFigures.ForEach(f => f.ZIndex = 0); // add by scott 11-06-2009
            gridFigures.ForEach(f => drawing.Figures.Add(f));
        }

        public void Show()
        {
            gridFigures.ForEach(f => f.Visible = true);
            drawing.Figures.UpdateVisual();
            hasGridLines = true;
        }

        public void Hide()
        {
            gridFigures.ForEach(f => f.Visible = false);
            drawing.Figures.UpdateVisual();
            hasGridLines = false;
        }

        // 09-16-2010 Scott
        public virtual void OnAddingToCanvas(System.Windows.Controls.Canvas newContainer)
        {
            
        }

        // 09-16-2010 Scott
        public virtual void OnRemovingFromCanvas(System.Windows.Controls.Canvas leavingContainer)
        {
            
        }
    }
}
