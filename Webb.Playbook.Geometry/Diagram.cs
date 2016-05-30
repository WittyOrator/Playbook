using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Actions;
using Webb.Playbook.Geometry.Shapes.Creators;
using Webb.Playbook.Data;

namespace Webb.Playbook.Geometry
{
    public class Diagram
    {
        public static Size DiagramSize = new Size(720, 480);
        public static Size ActualDiagramSize = DiagramSize;
        public static Point ActualStartPoint = new Point(0, 0);
        public static Point DiagramCenter = new Point(DiagramSize.Width / 2, DiagramSize.Height / 2);

        public int nPenColor = ColorSetting.Instance.RouteColor.ToRgbInt();
        public int nFillColor = ColorSetting.Instance.ZoneColor.ToRgbInt();
        public int nTextColor = Colors.Black.ToRgbInt();

        private Drawing drawing;
        
        private WEBBGAMEDATALib.WebbDiagram2 diagram;

        public void ResetSize()
        {
            DiagramSize = new Size(720, 480);
        }

        public void Scale(double scale)
        {
            ActualDiagramSize = new Size(DiagramSize.Width * scale, DiagramSize.Height * scale);

            ActualStartPoint = new Point((DiagramSize.Width - ActualDiagramSize.Width) / 2, (DiagramSize.Height - ActualDiagramSize.Height) / 2);
        }

        public Diagram(Drawing drawing)
        {
            this.drawing = drawing;

            diagram = new WEBBGAMEDATALib.WebbDiagram2();
        }

        private void New()
        {
            diagram.New();
        }

        private void Save(string path)
        {
            diagram.SaveToFile(path);
        }

        private void Close()
        {
            diagram.Close();
        }

        public void TranslateToDiagram(string path, bool bPlayground)
        {
            New();

            if (bPlayground)
            {
                diagram.AddBackgroundBitmap(AppDomain.CurrentDomain.BaseDirectory + @"Resource\Playground.bmp");
            }

            var figures = from f in drawing.Figures
                          orderby f.ZIndex ascending
                          select f;

            foreach (IFigure figure in figures)
            {
                // zones
                if (figure is Game.Zone)
                {
                    AddZone(figure as Game.Zone);
                }

                // lines
                if (figure is PBLine && !(figure.Dependencies.First() is ImmovablePoint))
                {
                    AddLine(figure as PBLine);
                }

                // players
                if (figure is Game.PBPlayer)
                {
                    AddPlayer(figure as Game.PBPlayer);
                }
            }

            Save(path);

            Close();
        }

        private void AddZone(Game.Zone zone)
        {
            Point p1 = ToDiagramPoint(new Point(zone.Coordinates.X - zone.Width / 2,zone.Coordinates.Y - zone.Height/2));

            Point p2 = ToDiagramPoint(new Point(zone.Coordinates.X + zone.Width / 2, zone.Coordinates.Y + zone.Height / 2));

            diagram.AddRectangle((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, 0, 1, nPenColor, nFillColor, 2);
        }

        private void AddLine(PBLine line)
        {
            if(drawing != null)
            {
                Point pt1 = ToDiagramPoint(line.Coordinates.P1);
                Point pt2 = ToDiagramPoint(line.Coordinates.P2);
                int x1 = (int)pt1.X;
                int x2 = (int)pt2.X;
                int y1 = (int)pt1.Y;
                int y2 = (int)pt2.Y;

                // 08-16-2010 Scott
                int nColor = nPenColor;
                int nPenStyle = (int)AdvPenStyle.Solid;
                int lineStyle = (int)AdvLineEndType.stylePlain;

                nColor = line.StrokeColor.ToRgbInt();

                // endpoint
                if (line.Dependencies.Last() is Game.PBPlayer || line.Dependencies.Last().Dependents.Count() == 1)
                {
                    switch (line.CapType)
                    {
                        case CapType.Arrow:
                            lineStyle = (int)AdvLineEndType.styleArrow;
                            break;
                        case CapType.Block:
                        case CapType.BlockArea:
                            lineStyle = (int)AdvLineEndType.styleBlock;
                            break;
                    }

                    // reset endpoint
                    if (line.Dependencies.Last() is Game.PBPlayer)
                    {
                        int len = (int)ToDiagramLength(1);

                        // todo
                        
                    }
                }
                // end

                switch (line.DashType)
                {
                    case DashType.DashDot:
                        nPenStyle = (int)AdvPenStyle.DashDot;
                        break;
                    case DashType.Dashed:
                        nPenStyle = (int)AdvPenStyle.Dash;
                        break;
                    case DashType.Dotted:
                        nPenStyle = (int)AdvPenStyle.Dot;
                        break;
                    default:
                        nPenStyle = (int)AdvPenStyle.Solid;
                        break;
                }

                diagram.AddLine(x1, y1, x2, y2, nPenStyle, (int)line.StrokeThickness, nColor, 0, 0, lineStyle);
            }
        }
        
        private void AddPlayer(Game.PBPlayer player)
        {
            if (drawing != null)
            {
                // 08-16-2010 Scott
                int nColor = nFillColor;

                nColor = player.FillColor.ToRgbInt();
                // end

                int nPenStyle = (int)AdvPenStyle.Solid;

                Point pt = ToDiagramPoint(player.Coordinates);

                if (pt.X < 0)
                {
                    pt.X = DiagramSize.Width / 5;
                }
                if(pt.X > DiagramSize.Width)
                {
                    pt.X = DiagramSize.Width * 4 / 5;
                }

                int len = (int)ToDiagramLength(player.Radius);

                if (player.Dash)
                {
                    nPenStyle = (int)AdvPenStyle.Dash;
                }

                switch(player.Appearance)
                {
                    case Webb.Playbook.Geometry.Game.PlayerAppearance.Text:
                        break;
                    case Webb.Playbook.Geometry.Game.PlayerAppearance.Square:
                        diagram.AddSquare((int)(pt.X - len / 2), (int)(pt.Y - len / 2), (int)(pt.X + len / 2), (int)(pt.Y + len / 2), nPenStyle, (int)player.StrokeThickness, nPenColor, nColor, 2);
                        break;
                    default:
                        diagram.AddCircle((int)(pt.X - len / 2), (int)(pt.Y - len / 2), (int)(pt.X + len / 2), (int)(pt.Y + len / 2), nPenStyle, (int)player.StrokeThickness, nPenColor, nColor, 2);
                        break;
                }

                // 08-16-2010 Scott
                int nStart = (int)(pt.X - len / 2);
                // end

                if (player.TextVisible)
                {
                    int fontSize = (int)(len * 2/3) > 0 ? (int)(len * 2/3) : len;

                    int count = player.Text.Length > 1 ? player.Text.Length : 1;

                    if (player.Text.Length == 1)
                    {
                        nStart += 4;
                    }
                    else if (player.Text.Length == 2)
                    {
                        nStart += 2;
                    }

                    diagram.AddText(nStart + System.Math.Max((len - fontSize * count) / 2, 1), (int)(pt.Y - len / 2) + (len - fontSize) / 2, (int)(pt.X + len / 2), (int)(pt.Y + len / 2), 0, 1, player.TextColor.ToRgbInt(), 0, 2, player.Text, "", fontSize);
                }

                //diagram.AddCircle(0, 0, 50, 50, 0, 2, nPenColor, nFillColor, 2);
                //diagram.AddEllipse(50, 50, 200, 100, 0, 3, nPenColor, nFillColor, 2);
                //diagram.AddLine(0, 0, 650, 400, 0, 1, nPenColor, 0, 0, (int)AdvLineEndType.styleBlock);
                //diagram.AddRectangle(200, 200, 250, 250, 0, 1, nPenColor, nFillColor, 2);
                //diagram.AddSquare(200, 400, 400, 600, 0, 3, nPenColor, nFillColor, 2);
                //diagram.AddText(300, 200, 400, 300, 0, 2, nTextColor, nFillColor, 2, "Scott", "", 30);
                //diagram.AddBackgroundBitmap(@"D:\untitled2.bmp");
            }
        }

        private Point ToDiagramPoint(Point logicPoint)
        {
            Point pt = new Point(logicPoint.X * ToDiagramLength(1) + ActualDiagramSize.Width / 2 + ActualStartPoint.X, -logicPoint.Y * ToDiagramLength(1) + ActualDiagramSize.Height / 2 + ActualStartPoint.Y);

            return pt;
        }

        private double ToDiagramLength(double logicLength)
        {
            double len = logicLength * ActualDiagramSize.Width / CoordinateSystem.LogicalPlaygroundWidth;

            return len;
        }
    }
}
