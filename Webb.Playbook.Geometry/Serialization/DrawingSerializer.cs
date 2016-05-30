using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry
{
    public class DrawingSerializer
    {
#if !SILVERLIGHT
        public static void Save(Drawing drawing, string fileName)
        {
            string serialized = SaveDrawing(drawing);
            File.WriteAllText(fileName, serialized);
        }

        //public static void SavePlayer(Game.PBPlayer player, string fileName)    // 11-04-2010 Scott
        //{
        //    string serialized = SavePlayer(player);
        //    File.WriteAllText(fileName, serialized);
        //}
#endif
        public static void SaveToXml(Drawing drawing, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Play");

                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> offensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 0);
                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> defensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 1);

                writer.WriteStartElement("Offensive");
                offensivePlayers.ForEach(p => WritePlayer(p, writer));
                writer.WriteEndElement();

                writer.WriteStartElement("Defensive");
                defensivePlayers.ForEach(p => WritePlayer(p, writer));
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
        }
        // add for Set
        public static void SaveOffToXml(Drawing drawing, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }
                ))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Drawing");

                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> offensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 0);
                Webb.Playbook.Geometry.Game.PBBall pbball = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBBall>().FirstOrDefault();
                writer.WriteStartElement("Figures");
                offensivePlayers.ForEach(p => SetWritePlayer(p, writer));
                if (pbball != null)
                {
                    writer.WriteStartElement(GetTagNameForFigure(pbball));
                    pbball.WriteXml(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

            }
            File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
        }
        public static void SaveDefToXml(Drawing drawing, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }
                ))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Drawing");
                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> defensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 1);
                Webb.Playbook.Geometry.Game.PBBall pbball = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBBall>().FirstOrDefault();
                writer.WriteStartElement("Figures");
                defensivePlayers.ForEach(p => SetWritePlayer(p, writer));
                if (pbball != null)
                {
                    writer.WriteStartElement(GetTagNameForFigure(pbball));
                    pbball.WriteXml(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();


            }
            File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
        }

        // 10-27-2011 Scott
        public static void SaveKickToXml(Drawing drawing, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }
                ))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Drawing");

                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> kickPlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 2);
                Webb.Playbook.Geometry.Game.PBBall pbball = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBBall>().FirstOrDefault();
                writer.WriteStartElement("Figures");
                kickPlayers.ForEach(p => SetWritePlayer(p, writer));
                if (pbball != null)
                {
                    writer.WriteStartElement(GetTagNameForFigure(pbball));
                    pbball.WriteXml(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

            }
            File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
        }


        public static void SaveToXml(Drawing drawing, string fileName, string gameName, bool ourTeamOffensive)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Play");

                writer.WriteAttributeString("PlayName", gameName);

                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> offensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 0);
                IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> defensivePlayers = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>().Where(p => p.ScoutType == 1);

                // 12-25-2009 Scott
                if (offensivePlayers.Any(p => p.Selected) || defensivePlayers.Any(p => p.Selected))
                {

                }
                else
                {
                    if (ourTeamOffensive)
                    {
                        if (offensivePlayers.Any(p => p.Name.StartsWith("QB")))
                        {
                            Webb.Playbook.Geometry.Game.PBPlayer player = offensivePlayers.First(p => p.Name.StartsWith("QB"));

                            if (player != null)
                            {
                                player.Selected = true;
                            }
                        }
                        else
                        {
                            if (offensivePlayers != null && offensivePlayers.Count() > 0)   //04-01-2010 scott
                            {
                                offensivePlayers.First().Selected = true;
                            }
                        }
                    }
                    else
                    {
                        if (defensivePlayers.Any(p => p.Name.StartsWith("LB")))
                        {
                            Webb.Playbook.Geometry.Game.PBPlayer player = defensivePlayers.First(p => p.Name.StartsWith("LB"));

                            if (player != null)
                            {
                                player.Selected = true;
                            }
                        }
                        else
                        {
                            if (defensivePlayers != null && defensivePlayers.Count() > 0)   //04-01-2010 scott
                            {
                                defensivePlayers.First().Selected = true;
                            }
                        }
                    }
                }

                writer.WriteStartElement("Offensive");
                offensivePlayers.ForEach(p => WritePlayer(p, writer));
                writer.WriteEndElement();

                writer.WriteStartElement("Defensive");
                defensivePlayers.ForEach(p => WritePlayer(p, writer));
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
        }

        private static void WritePlayer(Webb.Playbook.Geometry.Game.PBPlayer player, XmlWriter writer)
        {
            writer.WriteStartElement(GetTagNameForFigure(player));
            player.WriteToXml(writer);
            WriteDependencies(player, writer);
            writer.WriteEndElement();
        }
        // add for Set
        private static void SetWritePlayer(Webb.Playbook.Geometry.Game.PBPlayer player, XmlWriter writer)
        {
            writer.WriteStartElement(GetTagNameForFigure(player));
            player.SetWriteToXml(writer);
            writer.WriteEndElement();
        }

        //public static void SavePlayer(Game.PBPlayer player, XmlWriter writer)   // 11-04-2010 Scott
        //{
        //    writer.WriteStartDocument();
        //    writer.WriteStartElement("Player");
        //    WriteFigureList(player.GetPathFigure(player,null), writer);
        //    writer.WriteEndElement();
        //    writer.WriteEndDocument();
        //}

        //public static string SavePlayer(Game.PBPlayer player)   // 11-04-2010 Scott
        //{
        //    var s = new StringBuilder();

        //    using (var w = XmlWriter.Create(s, new XmlWriterSettings()
        //    {
        //        Indent = true,
        //    }))
        //    {
        //        SavePlayer(player, w);
        //    }

        //    return s.ToString();
        //}

        public static void SaveDrawing(Drawing drawing, XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Drawing");
            writer.WriteAttributeString("Title", drawing.Title.ToString());
            writer.WriteAttributeString("BackgroundPath", drawing.BackgroundPath);
            writer.WriteAttributeDouble("YardLine", drawing.YardLine);  // 01-20-2012 Scott
            WriteFigureList(drawing.GetSerializableFigures(), writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public static string SaveDrawing(Drawing drawing)
        {
            var s = new StringBuilder();
            using (var w = XmlWriter.Create(s, new XmlWriterSettings()
            {
                Indent = true
            }))
            {
                SaveDrawing(drawing, w);
            }
            return s.ToString();
        }

        public static void WriteFigureList(IEnumerable<IFigure> list, XmlWriter writer)
        {
            writer.WriteStartElement("Figures");
            foreach (var figure in list)
            {
                WriteFigure(figure, writer);
            }
            writer.WriteEndElement();
        }

        private static void WriteFigure(IFigure figure, XmlWriter writer)
        {
            writer.WriteStartElement(GetTagNameForFigure(figure));
            writer.WriteAttributeString("Name", figure.Name);
            figure.WriteXml(writer);
            WriteDependencies(figure, writer);
            writer.WriteEndElement();
        }

        private static void WriteDependencies(IFigure figure, XmlWriter writer)
        {
            if (figure.Dependencies.IsEmpty())
            {
                return;
            }
            foreach (var item in figure.Dependencies)
            {
                writer.WriteStartElement("Dependency");
                writer.WriteAttributeString("Name", item.Name);
                writer.WriteEndElement();
            }
        }

        private static string GetTagNameForFigure(IFigure figure)
        {
            return figure.GetType().Name;
        }
    }
}
