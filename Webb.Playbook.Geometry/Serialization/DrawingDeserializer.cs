using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Reflection;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry
{
    public class DrawingDeserializer
    {
        public static Drawing OpenDrawing(Canvas canvas, XElement element)
        {
            Drawing drawing = new Drawing(canvas);
            ReadDrawing(drawing, element);
            return drawing;
        }

        public static Drawing OpenDrawing(Canvas canvas, string savedDrawing)
        {
            XElement element = XElement.Parse(savedDrawing);
            return OpenDrawing(canvas, element);
        }

        public static System.Data.DataTable ReportReadData(string[] games)
        {
            DataTable table = new DataTable();

            table.Columns.Add("FileName", typeof(string));
            table.Columns.Add("FilePath", typeof(string));
            table.Columns.Add("PlayerName", typeof(string));
            table.Columns.Add("SymbolName", typeof(string));
            table.Columns.Add("PlayerCoachingPoints", typeof(string));
            table.Columns.Add("PlayerScoutType", typeof(Data.ScoutTypes));
            table.Columns.Add("ImagePath", typeof(string));

            try
            {
                Canvas canvas = new Canvas();

                Drawing drawing = null;

                foreach (string game in games)
                {
                    string gamePath = new System.IO.FileInfo(game).FullName;

                    if (System.IO.File.Exists(gamePath))
                    {
                        drawing = Drawing.Load(gamePath, canvas);

                        IEnumerable<Game.PBPlayer> players = drawing.Figures.OfType<Game.PBPlayer>();

                        foreach (Game.PBPlayer player in players)
                        {
                            DataRow row = table.NewRow();

                            row[0] = System.IO.Path.GetFileNameWithoutExtension(gamePath);
                            row[1] = gamePath;
                            row[2] = player.Name;
                            row[3] = player.Text;
                            row[4] = player.CoachingPoints;
                            row[5] = player.ScoutType;
                            // 08-18-2011 Scott
                            if (!System.IO.Directory.Exists(gamePath + ".BMP"))
                            {
                                row[6] = gamePath + ".BMP";
                            }
                            else
                            {
                                row[6] = string.Empty;
                            }

                            //string basePath = string.Empty;
                            //string productKeyPath = @"SOFTWARE\Webb Electronics\Playbook";
                            //Microsoft.Win32.RegistryKey productKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(productKeyPath);

                            //if (productKey != null)
                            //{
                            //    object objDir = productKey.GetValue("InstallDir");

                            //    if (objDir != null)
                            //    {
                            //        basePath = objDir.ToString();
                            //    }
                            //}

                            //if (!System.IO.Directory.Exists(basePath))
                            //{
                            //    break;
                            //}

                            //string path = basePath + @"\Bitmaps\";
                            //if (gamePath.Contains(basePath + @"Offensive\Formation\Offensive")
                            //    || gamePath.Contains(basePath + @"Defensive\Formation\Offensive"))
                            //{
                            //    path += "Offense";
                            //}
                            //else if (gamePath.Contains(basePath + @"Offensive\Formation\Defensive")
                            //    || gamePath.Contains(basePath + @"Defensive\Formation\Defensive"))
                            //{
                            //    path += "Defense";
                            //}
                            //else if (gamePath.Contains(basePath + @"Offensive\Playbook")
                            //    || gamePath.Contains(basePath + @"Defensive\Playbook"))
                            //{
                            //    path += "Plays";
                            //}
                            //string[] arrBitmaps = System.IO.Directory.GetFiles(path, row[0].ToString() + ".bmp", System.IO.SearchOption.AllDirectories);
                            //if (arrBitmaps != null && arrBitmaps.Count() > 0)
                            //{
                            //    row[6] = arrBitmaps[0];
                            //}
                            //else
                            //{
                            //    row[6] = string.Empty;
                            //}

                            table.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Source:\n" + ex.Source + "\n\n" + "Message:\n" + ex.Message + "\n\n" + "StackTrace:\n" + ex.StackTrace, "Webb Trace");
            }
            return table;
        }

        private static void Scale(Canvas canvas, double scale)
        {
            int nSpace = (int)((100 - scale) / 50 * 200);

            canvas.Margin = new System.Windows.Thickness(nSpace, 5, nSpace, 5);
        }

        public static System.Drawing.Image ReportReadImage(string game, double width, double height,bool background)
        {
            try
            {
                string strTempFile = AppDomain.CurrentDomain.BaseDirectory + "Image";

                if (System.IO.File.Exists(game))
                {
                    DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(game, (int)width * 3, new System.Windows.Size(width, height), background);
                    dbw.Show();

                    RenderTargetBitmap bmp = new RenderTargetBitmap((int)(width), (int)(height), 96, 96, PixelFormats.Default);
                    DrawingVisual drawingvisual = new DrawingVisual();

                    using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
                    {
                        VisualBrush visualbrush = new VisualBrush(dbw);

                        drawingcontext.DrawRectangle(visualbrush, null, new System.Windows.Rect(0, 0, width + 10, height + 10));
                    }

                    bmp.Render(drawingvisual);

                    SaveRTBAsPNG(bmp, strTempFile);

                    dbw.Close();
                }

                System.Drawing.Image img = System.Drawing.Image.FromFile(strTempFile);

                return img;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

                return null;
            }
        }

        public static string ReportReadDiagram(string game, double width, double height, bool background)
        {
            try
            {
                string strTempFile = AppDomain.CurrentDomain.BaseDirectory + "Diagram.Dia";

                if (System.IO.File.Exists(game))
                {
                    Drawing draw = Drawing.Load(game, new Canvas());

                    Diagram dia = new Diagram(draw);

                    Diagram.DiagramSize = new System.Windows.Size(width, height);

                    dia.TranslateToDiagram(strTempFile, background);
                }

                return strTempFile;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

                return null;
            }
        }

        static void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //save to PNG
        private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            PngBitmapEncoder enc = new PngBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);

                stm.Close();
            }
        }

        //public static void ReadPlayer(Drawing drawing, Game.PBPlayer player, string strFile)    // 11-04-2010 Scott
        //{
        //    string text = System.IO.File.ReadAllText(strFile);

        //    XElement element = XElement.Parse(text);

        //    DrawingDeserializer.ReadPlayer(drawing, player, element);
        //}

        //public static void ReadPlayer(Drawing drawing, Game.PBPlayer player, XElement element)  // 11-04-2010 Scott
        //{
        //    var figuresNode = element.Element("Figures");

        //    var figures = ReadFigures(figuresNode, drawing);

        //    drawing.Add(figures, false);

        //    drawing.Recalculate();
        //}

        public static void ReadDrawing(Drawing drawing, XElement element)
        {
            if(element.Attribute("Title") != null)
            {
                drawing.Title = bool.Parse(element.ReadString("Title"));
            }
            if(element.Attribute("BackgroundPath") != null)
            {
                drawing.BackgroundPath = element.ReadString("BackgroundPath");
            }
            if (element.Attribute("YardLine") != null)  // 01-20-2012 Scott
            {
                drawing.YardLine = element.ReadDouble("YardLine");
            }
            var figuresNode = element.Element("Figures");
            var figures = ReadFigures(figuresNode, drawing);

            drawing.Add(figures, false);

            drawing.Recalculate();
            //drawing.CoordinateSystem.MoveTo(drawing.Figures.OfType<IPoint>().Midpoint().Minus());
        }

        public static void ReadDrawingReverse(Drawing drawing, XElement element)
        {
            var figuresNode = element.Element("Figures");
            var figures = ReadFigures(figuresNode, drawing);

            drawing.Add(figures, false);

            if (drawing.Figures.OfType<Game.PBPlayer>().All(p => p.Coordinates.Y < 0))  // 01-04-2010 Scott
            {
                drawing.Figures.OfType<PointBase>().ForEach(p => { p.X = -p.Coordinates.X; p.Y = -p.Coordinates.Y; });
            }
            drawing.Recalculate();
        }

        private static IEnumerable<IFigure> ReadFigures(XElement figuresNode, Drawing drawing)
        {
            Dictionary<string, IFigure> figures = new Dictionary<string, IFigure>();
            foreach (var figureNode in figuresNode.Elements())
            {
                //Webb.Playbook.Data.Team team = new Webb.Playbook.Data.Team();
                //team.ReadXml(figureNode);
                var figure = ReadFigure(figureNode, figures, drawing);
                if (figure != null)
                {
                    yield return figure;
                }
            }
        }

        private static Dictionary<string, Type> mFigureTypes;
        public static Dictionary<string, Type> FigureTypes
        {
            get
            {
                if (mFigureTypes == null)
                {
                    mFigureTypes = new Dictionary<string, Type>();
                    foreach (var type in Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => typeof(IFigure).IsAssignableFrom(t)))
                    {
                        mFigureTypes.Add(type.Name, type);
                    }
                }
                return mFigureTypes;
            }
        }

        public static Type FindType(string typeName)
        {
            return FigureTypes[typeName];
        }

        private static IFigure ReadFigure(
            XElement figureNode,
            Dictionary<string, IFigure> dictionary,
            Drawing drawing)
        {
            Type type = FindType(figureNode.Name.LocalName);
            if (type == null)
            {
                return null;
            }

            IFigure instance = null;
            IFigureList dependencies = ReadDependencies(figureNode, dictionary);

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            if (defaultCtor != null)
            {
                instance = Activator.CreateInstance(type) as IFigure;
                instance.Dependencies = dependencies;
                instance.Drawing = drawing;
            }
            else
            {
                var ctorWithDependencies = type.GetConstructor(new Type[] { typeof(IFigureList) });
                if (ctorWithDependencies != null)
                {
                    instance = Activator.CreateInstance(type, dependencies) as IFigure;
                }
                else
                {
                    var ctorWithDrawingAndDependencies = type.GetConstructor(new Type[] { typeof(Drawing), typeof(IFigureList) });
                    if (ctorWithDrawingAndDependencies != null)
                    {
                        instance = Activator.CreateInstance(type, drawing, dependencies) as IFigure;
                    }
                }
            }

            instance.Name = figureNode.ReadString("Name");

            // get the max id , 07-23-2009 scott
            if (!(instance is Game.PBPlayer) && !(instance is Game.PBBall))
            {
                int index = instance.Name.Length - 1;
                for (int len = instance.Name.Length - 1; len >= 0; len--)
                {
                    if (Char.IsLetter(instance.Name[len]))
                    {
                        index = len;
                        break;
                    }
                }

                string strNum = instance.Name.Substring(index + 1);

                FigureBase.ID = System.Math.Max(FigureBase.ID, int.Parse(strNum));
            }
            // end

            instance.ReadXml(figureNode);
            if (!(instance is Game.PBBall))
            {
                dictionary.Add(instance.Name, instance);
            }
            return instance;
        }

        private static IFigureList ReadDependencies(XElement figureNode, Dictionary<string, IFigure> dictionary)
        {
            FigureList result = new FigureList();
            foreach (var node in figureNode.Elements("Dependency"))
            {
                var name = node.ReadString("Name");
                IFigure figure = null;
                if (dictionary.TryGetValue(name, out figure))
                {
                    result.Add(figure);
                }
            }
            return result;
        }

        public static void ReadDrawing(Drawing drawing, string savedDrawing)
        {
            XElement element = XElement.Parse(savedDrawing);
            ReadDrawing(drawing, element);
        }

        public static void ReadDrawingReverse(Drawing drawing, string savedDrawing)
        {
            XElement element = XElement.Parse(savedDrawing);
            ReadDrawingReverse(drawing, element);
        }

        // 08-19-2011 Scott
        public static void ReadDrawingBottomTeam(Drawing drawing, string savedDrawing)
        {
            XElement element = XElement.Parse(savedDrawing);
            ReadDrawingBottom(drawing, element);
        }

        // 08-19-2011 Scott
        public static void ReadDrawingUpTeam(Drawing drawing, string savedDrawing)
        {
            XElement element = XElement.Parse(savedDrawing);
            ReadDrawingUp(drawing, element);
        }

        // 08-19-2011 Scott
        public static void ReadDrawingBottom(Drawing drawing, XElement element)
        {
            var figuresNode = element.Element("Figures");
            var figures = ReadFigures(figuresNode, drawing);

            drawing.Add(figures, false);

            // 08-19-2011 Scott
            System.Windows.Point ptBall = new System.Windows.Point(0, 0);

            foreach (IFigure figure in figures)
            {
                if (figure is Game.PBBall)
                {
                    ptBall = (figure as Game.PBBall).Coordinates;

                    break;
                }
            }

            System.Windows.Point ptPlayer = figures.OfType<Game.PBPlayer>().First().Coordinates;

            if (figures.OfType<Game.PBPlayer>().All(p => p.ScoutType == 0))  // 01-04-2010 Scott
            {
                if (ptBall.Y < ptPlayer.Y)
                {
                    figures.OfType<PointBase>().ForEach(p => { p.X = -p.Coordinates.X; p.Y = -p.Coordinates.Y; });
                }
            }
            // end

            drawing.Recalculate();
        }

        // 08-19-2011 Scott
        public static void ReadDrawingUp(Drawing drawing, XElement element)
        {
            var figuresNode = element.Element("Figures");
            var figures = ReadFigures(figuresNode, drawing);

            drawing.Add(figures, false);

            // 08-19-2011 Scott
            System.Windows.Point ptBall = new System.Windows.Point(0, 0);
            
            foreach (IFigure figure in figures)
            {
                if (figure is Game.PBBall)
                {
                    ptBall = (figure as Game.PBBall).Coordinates;

                    break;
                }
            }

            System.Windows.Point ptPlayer = figures.OfType<Game.PBPlayer>().First().Coordinates;

            if (figures.OfType<Game.PBPlayer>().All(p => p.ScoutType == 1))  // 01-04-2010 Scott
            {
                if (ptBall.Y > ptPlayer.Y)
                {
                    figures.OfType<PointBase>().ForEach(p => { p.X = -p.Coordinates.X; p.Y = -p.Coordinates.Y; });
                }
            }
            // end

            drawing.Recalculate();
        }
    }
}
