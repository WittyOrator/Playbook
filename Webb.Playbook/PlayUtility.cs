using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;

using Webb.Playbook.Data;
using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook
{
    public class PlayInfo
    {
        public PlayInfo()
        {
            HashLine = HashLine.Middle;
            Marks = new List<int>();
            for (int i = -50; i <= 50; i = i + 10)
            {
                Marks.Add(i);

            }
            Mark = 0;
        }

        private string offensiveFormation = string.Empty;
        public string OffensiveFormation
        {
            get { return offensiveFormation; }
            set { offensiveFormation = value; }
        }

        private string defensiveFormation = string.Empty;
        public string DefensiveFormation
        {
            get { return defensiveFormation; }
            set { defensiveFormation = value; }
        }

        public HashLine HashLine { set; get; }
        public List<int> marks;
        public List<int> Marks
        {
            get { return marks; }
            set { marks = new List<int>(); }
        }

        private int mark;
        public int Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        private string playName;
        public string PlayName
        {
            get { return playName; }
            set { playName = value; }
        }

        public void Save(string file)
        {
            XDocument doc = new XDocument();
            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");
            doc.Declaration = declare;

            doc.Add(new XElement("PlayInfo"));

            XElement hashElem = new XElement("HashLine");
            XAttribute hashxatt = new XAttribute("Value", HashLine);
            hashElem.Add(hashxatt);

            XElement markElem = new XElement("Mark");
            XAttribute markxatt = new XAttribute("Value", Mark);
            markElem.Add(markxatt);

            XElement offElem = new XElement("OffensiveFormation");
            XAttribute offAtrri = new XAttribute("Value", OffensiveFormation);
            offElem.Add(offAtrri);

            XElement defElem = new XElement("DefensiveFormation");
            XAttribute defAtrri = new XAttribute("Value", DefensiveFormation);
            defElem.Add(defAtrri);

            doc.Root.Add(hashElem);
            doc.Root.Add(markElem);
            doc.Root.Add(offElem);
            doc.Root.Add(defElem);

            doc.Save(file);
        }

        public void Load(string file)
        {
            // 09-17-2010 Scott
            int index = file.IndexOf(".PlayInfo");
            if (index > 0)
            {
                PlayName = file.Remove(index);
            }

            if (File.Exists(file))
            {
                try
                {
                    XDocument doc = XDocument.Load(file, LoadOptions.PreserveWhitespace);

                    if (doc.Root.Elements().Any(f => f.Name == "HashLine"))
                    {
                        this.HashLine = (HashLine)Enum.Parse(typeof(HashLine), doc.Root.Element("HashLine").Attribute("Value").Value);
                    }
                    if (doc.Root.Elements().Any(f => f.Name == "Mark"))
                    {
                        Mark = Convert.ToInt16(doc.Root.Element("Mark").Attribute("Value").Value);
                    }

                    OffensiveFormation = doc.Root.Element("OffensiveFormation").Attribute("Value").Value;
                    DefensiveFormation = doc.Root.Element("DefensiveFormation").Attribute("Value").Value;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.StackTrace,ex.Message);    // 08-30-2011 Scott
                }
            }
        }
    }

    public class PlayersCreator
    {
        public PlayersCreator()
        {

        }

        public void CreatePlayers(Webb.Playbook.Geometry.Drawing drawing, Data.Personnel personnel, bool bCreateBall, double dLogicScrimmage)
        {
            // 03-16-2011 Scott
            double radius = Webb.Playbook.Data.GameSetting.Instance.PlayerSize;
            double angle = Webb.Playbook.Data.GameSetting.Instance.PlayerAngle;
            PlayerAppearance playerAppearance = (PlayerAppearance)Enum.Parse(typeof(PlayerAppearance), Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance);
            double thickness = Webb.Playbook.Data.GameSetting.Instance.PlayerThickness;
            bool dash = Webb.Playbook.Data.GameSetting.Instance.PlayerDash;
            bool textVisibility = Webb.Playbook.Data.GameSetting.Instance.PlayerTextVisibility;

            IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> players = drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            if (players != null && players.Count() > 0)
            {
                radius = players.First().Radius;
            }

            double scrimmage = dLogicScrimmage;
            int i = 0;
            double x = 0;
            double y = 0;
            double size = Webb.Playbook.Data.GameSetting.Instance.PlayerSize * 1.1;
            //Color colorOffense = Colors.OrangeRed;
            //Color colorDefense = Colors.SteelBlue;

            if (personnel.ScoutType == ScoutTypes.Offensive)
            {
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("QB")))
                {
                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point(0, scrimmage - 2 * size),
                        Stance = "TwoPointCenter",
                        FillColor = ColorSetting.Instance.OffensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                y = -0.5 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("OL")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point((i + 1) / 2 * System.Math.Pow(-1, i) * size, scrimmage + y),
                        Stance = "ThreePointRight",
                        FillColor = ColorSetting.Instance.OffensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                x = -6 * size;
                y = -1 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("TE")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point(x,scrimmage + y - i * size),
                        Stance = "ThreePointRight",
                        FillColor = ColorSetting.Instance.OffensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                x = 8 * size;
                y = -1 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("WR")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point(x, scrimmage + y - i * size),
                        Stance = "TwoPointRight",
                        FillColor = ColorSetting.Instance.OffensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                x = 0 * size;
                y = -4 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("RB")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point(x, scrimmage + y - i * size),
                        Stance = "TwoPointCenter",
                        FillColor = ColorSetting.Instance.OffensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }
            }
            else if (personnel.ScoutType == ScoutTypes.Defensive)
            {
                y = 0.5 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("DL")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point((i + 1) / 2 * System.Math.Pow(-1, i) * size, scrimmage + y),
                        Stance = "ThreePointLeft",
                        FillColor = ColorSetting.Instance.DefensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                x = 8 * size;
                y = 1 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("CB")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point(x * System.Math.Pow(-1, i) * size, scrimmage + (i / 2) * size + y),
                        Stance = "TwoPointCenter",
                        FillColor = ColorSetting.Instance.DefensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                y = 3 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("LB")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point((i + 1) / 2 * System.Math.Pow(-1, i) * size, scrimmage + y),
                        Stance = "TwoPointCenter",
                        FillColor = ColorSetting.Instance.DefensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }

                y = 5.5 * size;
                foreach (Position pos in personnel.Positions.FindAll(p => p.Name.StartsWith("S")))
                {
                    i = pos.ID - 1;

                    PBPlayer player = new PBPlayer(playerAppearance, pos.Name)
                    {
                        Text = pos.Value,
                        Coordinates = new Point((i + 1) / 2 * System.Math.Pow(-1, i) * size, scrimmage + y),
                        Stance = "TwoPointCenter",
                        FillColor = ColorSetting.Instance.DefensivePlayerColor,
                        Radius = radius,
                        Angle = angle,
                        StrokeThickness = thickness,
                        TextVisible = textVisibility,
                        Dash = dash,
                    };
                    drawing.Add(player);
                }
            }

            if (bCreateBall)
            {
                Point ptBall = new Point(0, 0);
                bool bHasBall = false;

                foreach (PBPlayer player in drawing.Figures.OfType<PBPlayer>())
                {
                    if (personnel.ScoutType == ScoutTypes.Offensive)
                    {
                        if (player.Name == PersonnelEditor.OffHasBallPlayer)
                        {
                            ptBall = player.Coordinates;
                            bHasBall = true;
                        }
                    }
                    else if (personnel.ScoutType == ScoutTypes.Defensive)
                    {
                        if (player.Name == PersonnelEditor.DefHasBallPlayer)
                        {
                            ptBall = player.Coordinates;
                            bHasBall = true;
                        }
                    }
                }

                if (bHasBall)
                {
                    PBBall ball = new PBBall()
                    {
                        Center = new Point(ptBall.X, personnel.ScoutType == ScoutTypes.Defensive ? ptBall.Y - 0.5f : ptBall.Y + 0.5f),
                    };

                    if (drawing.Figures.OfType<PBBall>().Count() == 0)
                    {
                        ball.Visible = Webb.Playbook.Data.GameSetting.Instance.ShowBall;
                        drawing.Add(ball);
                    }
                }
            }
        }
    }
}
