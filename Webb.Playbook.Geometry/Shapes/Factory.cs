using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry.Shapes
{
    public class Factory
    {
        const double size = 8;

        public static Polygon CreateArrowShape()
        {
            return new System.Windows.Shapes.Polygon()
            {
                Fill = CreateDefaultFillBrush()
            };
        }

        public static LineType LineType = LineType.BeeLine;
        public static CapType CapType = CapType.Arrow;
        public static Color Color = Colors.Black;
        public static DashType DashType = DashType.Solid;
        public static double StrokeThickness = 3;
        
        public static Shape CreatePointShape()
        {
            Rectangle rect = new Rectangle()
            {
                Width = size,
                Height = size,
                //Fill = Brushes.Yellow,
                //Stroke = Brushes.Black,
                //StrokeThickness = 0.5
                Fill = Brushes.Transparent,
                Stroke = Brushes.Transparent,
            };
            Ellipse ellipse = new Ellipse()
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5
            };

            return rect;
        }

        public static Shape CreateCircleShape()
        {
            double size = 8;
            Ellipse ellipse = new Ellipse()
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.LightBlue
            };

            if (Behavior.DrawVideo)
            {
                ellipse.StrokeThickness = 2;
            }

            return ellipse;
        }

        public static PBLine CreateLine(Drawing drawing, FigureList dependencies)
        {
            PBLine retLine = new PBLine() 
            { 
                Drawing = drawing, 
                Dependencies = dependencies, 
            };

            if (Behavior.DrawVideo)
            {
                retLine.StrokeColor = Color;
            }

            return retLine;
        }

        public static PBLine CreateArrowLine(Drawing drawing, FigureList dependencies)
        {
            PBLine retLine = new PBLine() { Drawing = drawing, Dependencies = dependencies, CapType = CapType.Arrow, LineType = LineType, DashType = DashType };

            if (drawing.DrawingMode)
            {
                retLine.CapType = CapType;

                retLine.StrokeColor = Color;

                retLine.StrokeThickness = StrokeThickness;
            }

            if (Behavior.DrawVideo)
            {
                retLine.StrokeColor = Color;
            }

            return retLine;
        }

        public static PBLine CreatePassBlockAreaLine(Drawing drawing, FigureList dependencies)
        {
            PBLine retLine = new PBLine()
            {
                Drawing = drawing,
                Dependencies = dependencies,
                CapType = CapType.BlockArea,
            };

            if (Behavior.DrawVideo)
            {
                retLine.StrokeColor = Color;
            }

            return retLine;
        }

        public static PBLine CreateLine(Drawing drawing, FigureList dependencies, Webb.Playbook.Geometry.Game.PBRoutePoint pbPoint)
        {
            PBLine retLine = new PBLine() 
            { 
                Drawing = drawing, Dependencies = dependencies, 
                CapType = pbPoint.CapType, 
                LineType = pbPoint.LineType,
                DashType = pbPoint.DashType, 
                StrokeColor = pbPoint.StrokeColor,
            };

            return retLine;
        }

        public static PBSquare CreateSquareShape(Drawing drawing, System.Windows.Point coordinates)
        {
            PBSquare square = new PBSquare() { Drawing = drawing };
            square.MoveTo(coordinates);
            square.Radius = 1;
            return square;
        }

        public static PBRect CreateRectShape(Drawing drawing, System.Windows.Point coordinates)
        {
            PBRect rect = new PBRect() { Drawing = drawing };
            rect.MoveTo(coordinates);
            rect.Height = 1;
            rect.Width = 1;

            if (Behavior.DrawVideo)
            {
                rect.Shape.Stroke = new SolidColorBrush(Color);
                rect.Shape.Fill = Brushes.Transparent;
                rect.Shape.StrokeThickness = 2;
            }

            return rect;
        }

        public static PBCircle CreateCircleShape(Drawing drawing, System.Windows.Point coordinates)
        {
            PBCircle circle = new PBCircle() { Drawing = drawing };
            circle.MoveTo(coordinates);
            circle.Radius = 1;

            if (Behavior.DrawVideo)
            {
                circle.Shape.Stroke = new SolidColorBrush(Color);
                circle.Shape.Fill = Brushes.Transparent;
            }

            return circle;
        }

        public static Webb.Playbook.Geometry.Game.PBPlayer CreatePlayerShape(Drawing drawing, System.Windows.Point coordinates)
        {
            Webb.Playbook.Geometry.Game.PBPlayer player = new Webb.Playbook.Geometry.Game.PBPlayer() { Drawing = drawing };
            player.MoveTo(coordinates);
            return player;
        }

        public static Shape CreateSquareShape()
        {
            double size = 8;
            Rectangle rect = new Rectangle()
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };

            return rect;
        }

        public static Line CreateLineShape()
        {
            Line result = new Line()
            {
                Stroke = Brushes.AliceBlue,
                StrokeThickness = 3
            };

            return result;
        }

        public static FreePoint CreateFreePoint(Drawing drawing, Point coordinates)
        {
            FreePoint result = new FreePoint() { Drawing = drawing };
            result.MoveTo(coordinates);
            return result;
        }

        // 12-16-2011 Scott
        public static HandlePoint CreateHandlePoint(Drawing drawing, Point coordinates)
        {
            HandlePoint result = new HandlePoint() { Drawing = drawing };
            result.MoveTo(coordinates);
            return result;
        }

        public static PrePoint CreatePrePoint(Drawing drawing, Point coordinates)
        {
            PrePoint result = new PrePoint() { Drawing = drawing };
            result.MoveTo(coordinates);
            return result;
        }

        public static SubPoint CreateSubPoint(Drawing drawing, Point coordinates)
        {
            SubPoint result = new SubPoint() { Drawing = drawing };
            result.MoveTo(coordinates);
            return result;
        }

        public static Brush CreateDefaultFillBrush()
        {
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 200));
        }

        public static Brush CreateGradientBrush(Color c1, Color c2, double angle)
        {
            LinearGradientBrush result = new LinearGradientBrush(
                new GradientStopCollection()
                {
                    new GradientStop() { Offset = 0.1, Color = c1 },
                    new GradientStop() { Offset = 1.0, Color = c2 }
                }, angle);
            return result;
        }

        public static TextBlock CreateLabelShape()
        {
            TextBlock tb = new TextBlock();
            tb.SetValue(TextBlock.FontSizeProperty, 16.5);
            return tb;
        }
    }
}
