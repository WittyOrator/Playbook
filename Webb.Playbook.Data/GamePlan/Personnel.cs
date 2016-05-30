using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    

    public enum PositionTypes
    {
        Quarterback,
        Receivers,
        Runningbacks,
        OffensiveLinemen,
        TightEnds,

        Cornerbacks,
        Linebackers,
        DefensiveLinemen,
        Safeties,
    }

    public class Position : NotifyObj
    {
        private List<string> players;
        public List<string> Players
        {
            get
            {
                if (players == null)
                {
                    players = new List<string>();
                    for (int i = 0; i < 5; i++)
                    {
                        players.Add(string.Empty);
                    }
                }

                return players;
            }
            set
            {
                players = value;

                OnPropertyChanged("Players");
            }
        }

        public Position()
        {
            this.name = string.Empty;
            this.value = string.Empty;
            this.hasBall = false;
        }

        public Position(string name)
        {
            this.name = name;
            this.value = string.Empty;
        }

        public int ID
        {
            get
            {
                if (Name != string.Empty)
                {
                    int i = 0;

                    for (; i < Name.Length; i++)
                    {
                        if (char.IsDigit(Name[i]))
                        {
                            break;
                        }
                    }

                    string str = Name.Substring(i);

                    return Int32.Parse(str);
                }

                return -1;
            }
        }

        private System.Windows.Visibility btnVisibility = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility BtnVisibility
        {
            get
            {
                return btnVisibility;
            }
            set
            {
                btnVisibility = value;
            }
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool hasBall;
        public bool HasBall
        {
            get { return hasBall; }
            set 
            { 
                hasBall = value;

                OnPropertyChanged("HasBall");
            }
        }

        public void Save(XElement element)
        {
            XElement elem = new XElement("Position");
            XAttribute atrri = new XAttribute("Name", Name);
            elem.Add(atrri);
            atrri = new XAttribute("Value", Value);
            elem.Add(atrri);
            atrri = new XAttribute("HasBall", HasBall);
            elem.Add(atrri);

            foreach (string strNum in Players)
            {
                XElement elemPlayer = new XElement("Player");
                atrri = new XAttribute("Number", strNum);
                elemPlayer.Add(atrri);

                elem.Add(elemPlayer);
            }

            element.Add(elem);
        }

        public void Load(XElement element)
        {
            Name = element.Attribute("Name").Value;
            Value = element.Attribute("Value").Value;
            if (element.Attribute("HasBall") != null)
            {
                HasBall = bool.Parse(element.Attribute("HasBall").Value);
            }

            List<XElement> elemPlayers = element.Elements("Player").ToList();
            for (int i = 0; i < elemPlayers.Count(); i++)
            {
                XElement elemPlayer = elemPlayers.ElementAt(i);

                Players[i] = elemPlayer.Attribute("Number").Value;
            }
        }
    }

    public class PersonnelSetting
    {
        private static int Max = 7;
        private Position quarterback;
        public Position Quarterback
        {
            get { return quarterback; }
            set { quarterback = value; }
        }
        private List<Position> receivers;
        public List<Position> Receivers
        {
            get { return receivers; }
            set { receivers = value; }
        }
        private List<Position> runningbacks;
        public List<Position> Runningbacks
        {
            get { return runningbacks; }
            set { runningbacks = value; }
        }
        private List<Position> offensiveLinemen;
        public List<Position> OffensiveLinemen
        {
            get { return offensiveLinemen; }
            set { offensiveLinemen = value; }
        }
        private List<Position> tightEnds;
        public List<Position> TightEnds
        {
            get { return tightEnds; }
            set { tightEnds = value; }
        }

        private List<Position> cornerbacks;
        public List<Position> Cornerbacks
        {
            get { return cornerbacks; }
            set { cornerbacks = value; }
        }
        private List<Position> linebackers;
        public List<Position> Linebackers
        {
            get { return linebackers; }
            set { linebackers = value; }
        }
        private List<Position> defensiveLinemen;
        public List<Position> DefensiveLinemen
        {
            get { return defensiveLinemen; }
            set { defensiveLinemen = value; }
        }
        private List<Position> safeties;
        public List<Position> Safeties
        {
            get { return safeties; }
            set { safeties = value; }
        }

        private List<Position> allPositions;
        public List<Position> AllPositions
        {
            get { return allPositions; }
        }

        private List<Position> offensivePositions;
        public List<Position> OffensivePositions
        {
            get { return offensivePositions; }
        }

        private List<Position> defensivePositions;
        public List<Position> DefensivePositions
        {
            get { return defensivePositions; }
        }

        public PersonnelSetting()
        {
            Quarterback = new Position("QB");
            Receivers = new List<Position>(Max);
            Runningbacks = new List<Position>(Max);
            OffensiveLinemen = new List<Position>(Max);
            TightEnds = new List<Position>(Max);
            Cornerbacks = new List<Position>(Max);
            Linebackers = new List<Position>(Max);
            DefensiveLinemen = new List<Position>(Max);
            Safeties = new List<Position>(Max);
            allPositions = new List<Position>();
            offensivePositions = new List<Position>();
            defensivePositions = new List<Position>();
        }

        public void SetBtnVisibility(System.Windows.Visibility visibility)
        {
            Quarterback.BtnVisibility = visibility;

            foreach (Position pos in Receivers)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in Runningbacks)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in OffensiveLinemen)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in TightEnds)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in Cornerbacks)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in Linebackers)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in DefensiveLinemen)
            {
                pos.BtnVisibility = visibility;
            }
            foreach (Position pos in Safeties)
            {
                pos.BtnVisibility = visibility;
            }
        }

        public void Init()
        {
            allPositions.Add(Quarterback);
            offensivePositions.Add(Quarterback);
            for (int i = 1; i <= Max; i++)
            {
                Receivers.Add(new Position("WR" + i.ToString()));
            }
            allPositions.AddRange(Receivers);
            offensivePositions.AddRange(Receivers);
            for (int i = 1; i <= Max; i++)
            {
                Runningbacks.Add(new Position("RB" + i.ToString()));
            }
            allPositions.AddRange(Runningbacks);
            offensivePositions.AddRange(Runningbacks);
            for (int i = 1; i <= Max; i++)
            {
                OffensiveLinemen.Add(new Position("OL" + i.ToString()));
            }
            OffensiveLinemen[0].HasBall = true;
            allPositions.AddRange(OffensiveLinemen);
            offensivePositions.AddRange(OffensiveLinemen);
            for (int i = 1; i <= Max; i++)
            {
                TightEnds.Add(new Position("TE" + i.ToString()));
            }
            allPositions.AddRange(TightEnds);
            offensivePositions.AddRange(TightEnds);


            for (int i = 1; i <= Max; i++)
            {
                Cornerbacks.Add(new Position("CB" + i.ToString()));
            }
            allPositions.AddRange(Cornerbacks);
            defensivePositions.AddRange(Cornerbacks);
            for (int i = 1; i <= Max; i++)
            {
                Linebackers.Add(new Position("LB" + i.ToString()));
            }
            allPositions.AddRange(Linebackers);
            defensivePositions.AddRange(Linebackers);
            for (int i = 1; i <= Max; i++)
            {
                DefensiveLinemen.Add(new Position("DL" + i.ToString()));
            }
            DefensiveLinemen[0].HasBall = true;
            allPositions.AddRange(DefensiveLinemen);
            defensivePositions.AddRange(DefensiveLinemen);
            for (int i = 1; i <= Max; i++)
            {
                Safeties.Add(new Position("S" + i.ToString()));
            }
            allPositions.AddRange(Safeties);
            defensivePositions.AddRange(Safeties);
        }

        public void ClearOffBall()
        {
            foreach (Position pos in OffensiveLinemen)
            {
                pos.HasBall = false;
            }
        }

        public void ClearDefBall()
        {
            foreach (Position pos in DefensiveLinemen)
            {
                pos.HasBall = false;
            }
        }

        public void Save(string strFile)
        {
            XDocument doc = new XDocument();

            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");

            doc.Declaration = declare;

            XElement rootElem = new XElement("PersonnelSetting");
            
            Quarterback.Save(rootElem);

            XElement elem = new XElement("Positions");
            XAttribute atrri = new XAttribute("Name", "Receivers");
            elem.Add(atrri);
            foreach (Position pos in Receivers)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "Runningbacks");
            elem.Add(atrri);
            foreach (Position pos in Runningbacks)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "OffensiveLinemen");
            elem.Add(atrri);
            foreach (Position pos in OffensiveLinemen)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "TightEnds");
            elem.Add(atrri);
            foreach (Position pos in TightEnds)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "Cornerbacks");
            elem.Add(atrri);
            foreach (Position pos in Cornerbacks)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "Linebackers");
            elem.Add(atrri);
            foreach (Position pos in Linebackers)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "DefensiveLinemen");
            elem.Add(atrri);
            foreach (Position pos in DefensiveLinemen)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            elem = new XElement("Positions");
            atrri = new XAttribute("Name", "Safeties");
            elem.Add(atrri);
            foreach (Position pos in Safeties)
            {
                pos.Save(elem);
            }
            rootElem.Add(elem);

            doc.Add(rootElem);

            doc.Save(strFile);
        }

        public void LoadPlayers(PersonnelSetting ps)
        {
            Quarterback.Players = ps.Quarterback.Players;
            //Quarterback.Players.Clear();
            //Quarterback.Players.AddRange(ps.Quarterback.Players);

            for (int i = 0; i < Receivers.Count(); i++ )
            {
                Receivers[i].Players = ps.Receivers[i].Players;
                //Receivers[i].Players.Clear();
                //Receivers[i].Players.AddRange(ps.Receivers[i].Players);
            }

            for (int i = 0; i < Runningbacks.Count(); i++)
            {
                Runningbacks[i].Players = ps.Runningbacks[i].Players;
                //Runningbacks[i].Players.Clear();
                //Runningbacks[i].Players.AddRange(ps.Runningbacks[i].Players);
            }

            for (int i = 0; i < OffensiveLinemen.Count(); i++)
            {
                OffensiveLinemen[i].Players = ps.OffensiveLinemen[i].Players;
                //OffensiveLinemen[i].Players.Clear();
                //OffensiveLinemen[i].Players.AddRange(ps.OffensiveLinemen[i].Players);
            }

            for (int i = 0; i < TightEnds.Count(); i++)
            {
                TightEnds[i].Players = ps.TightEnds[i].Players;
                //TightEnds[i].Players.Clear();
                //TightEnds[i].Players.AddRange(ps.TightEnds[i].Players);
            }

            for (int i = 0; i < Cornerbacks.Count(); i++)
            {
                Cornerbacks[i].Players = ps.Cornerbacks[i].Players;
                //Cornerbacks[i].Players.Clear();
                //Cornerbacks[i].Players.AddRange(ps.Cornerbacks[i].Players);
            }

            for (int i = 0; i < Linebackers.Count(); i++)
            {
                Linebackers[i].Players = ps.Linebackers[i].Players;
                //Linebackers[i].Players.Clear();
                //Linebackers[i].Players.AddRange(ps.Linebackers[i].Players);
            }

            for (int i = 0; i < DefensiveLinemen.Count(); i++)
            {
                DefensiveLinemen[i].Players = ps.DefensiveLinemen[i].Players;
                //DefensiveLinemen[i].Players.Clear();
                //DefensiveLinemen[i].Players.AddRange(ps.DefensiveLinemen[i].Players);
            }

            for (int i = 0; i < Safeties.Count(); i++)
            {
                Safeties[i].Players = ps.Safeties[i].Players;
                //Safeties[i].Players.Clear();
                //Safeties[i].Players.AddRange(ps.Safeties[i].Players);
            }
        }

        public void Load(string strFile)
        {
            this.Clear();

            if (System.IO.File.Exists(strFile))
            {
                XDocument doc = XDocument.Load(strFile);

                XElement elem = doc.Descendants("Position").First(f => f.Attribute("Name").Value == "QB");
                Quarterback.Load(elem);
                allPositions.Add(Quarterback);
                offensivePositions.Add(Quarterback);

                IEnumerable<XElement> elems = doc.Descendants("Positions");

                IEnumerable<XElement> posElems = elems.First(f => f.Attribute("Name").Value == "Receivers").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    Receivers.Add(pos);
                }
                allPositions.AddRange(Receivers);
                offensivePositions.AddRange(receivers);

                posElems = elems.First(f => f.Attribute("Name").Value == "Runningbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    Runningbacks.Add(pos);
                }
                allPositions.AddRange(Runningbacks);
                offensivePositions.AddRange(Runningbacks);

                posElems = elems.First(f => f.Attribute("Name").Value == "OffensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    OffensiveLinemen.Add(pos);
                }
                allPositions.AddRange(OffensiveLinemen);
                offensivePositions.AddRange(OffensiveLinemen);

                posElems = elems.First(f => f.Attribute("Name").Value == "TightEnds").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    TightEnds.Add(pos);
                }
                allPositions.AddRange(TightEnds);
                offensivePositions.AddRange(TightEnds);

                posElems = elems.First(f => f.Attribute("Name").Value == "Cornerbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    Cornerbacks.Add(pos);
                }
                allPositions.AddRange(Cornerbacks);
                defensivePositions.AddRange(Cornerbacks);

                posElems = elems.First(f => f.Attribute("Name").Value == "Linebackers").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    Linebackers.Add(pos);
                }
                allPositions.AddRange(Linebackers);
                defensivePositions.AddRange(Linebackers);

                posElems = elems.First(f => f.Attribute("Name").Value == "DefensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    DefensiveLinemen.Add(pos);
                }
                allPositions.AddRange(DefensiveLinemen);
                defensivePositions.AddRange(DefensiveLinemen);

                posElems = elems.First(f => f.Attribute("Name").Value == "Safeties").Elements();
                foreach (XElement e in posElems)
                {
                    Position pos = new Position();
                    pos.Load(e);
                    Safeties.Add(pos);
                }
                allPositions.AddRange(Safeties);
                defensivePositions.AddRange(Safeties);
            }
            else
            {
                this.Init();
            }
        }

        public void Clear()
        {
            Receivers.Clear();
            Runningbacks.Clear();
            OffensiveLinemen.Clear();
            TightEnds.Clear();
            Cornerbacks.Clear();
            Linebackers.Clear();
            DefensiveLinemen.Clear();
            Safeties.Clear();
            allPositions.Clear();
            offensivePositions.Clear();
            defensivePositions.Clear();
        }

        public IEnumerable<Position> GetOffensePositions()
        {
            List<Position> offensePositions = new List<Position>();

            offensePositions.Add(Quarterback);

            offensePositions.AddRange(Receivers.FindAll(f => f.Value != string.Empty));
            offensePositions.AddRange(Runningbacks.FindAll(f => f.Value != string.Empty));
            offensePositions.AddRange(OffensiveLinemen.FindAll(f => f.Value != string.Empty));
            offensePositions.AddRange(TightEnds.FindAll(f => f.Value != string.Empty));

            return offensePositions;
        }

        public IEnumerable<Position> GetDefensePositions()
        {
            List<Position> defensePositions = new List<Position>();;

            defensePositions.AddRange(Cornerbacks.FindAll(f => f.Value != string.Empty));
            defensePositions.AddRange(Linebackers.FindAll(f => f.Value != string.Empty));
            defensePositions.AddRange(DefensiveLinemen.FindAll(f => f.Value != string.Empty));
            defensePositions.AddRange(Safeties.FindAll(f => f.Value != string.Empty));

            return defensePositions;
        }
    }

    public class Personnel
    {
        public Personnel()
        {
            Name = "Personnel";
            ScoutType = ScoutTypes.Offensive;
            Positions = new List<Position>();
        }

        public Personnel(string strPath)
        {
            Path = strPath;
            Name = strPath.Substring(strPath.LastIndexOf('\\') + 1);
            Positions = new List<Position>();
            string strFile = Path + @"\" + Name + ".per";
            if(System.IO.File.Exists(strFile))
            {
                this.Load(strFile);
            }
            //continue
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return scoutType; }
            set { scoutType = value; }
        }

        private List<Position> positions;
        public List<Position> Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        public void Save(string strFile)
        {
            XDocument doc = new XDocument();

            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");

            doc.Declaration = declare;

            XElement rootElem = new XElement("Personnel");
            rootElem.Add(new XAttribute("Name",Name));
            rootElem.Add(new XAttribute("ScoutType",ScoutType.ToString()));

            foreach (Position pos in Positions)
            {
                pos.Save(rootElem);
            }

            doc.Add(rootElem);

            doc.Save(strFile);
        }

        public void Load(string strFile)
        {
            XDocument doc = XDocument.Load(strFile);

            Name = doc.Element("Personnel").Attribute("Name").Value;
            ScoutType = (ScoutTypes)Enum.Parse(typeof(ScoutTypes), doc.Element("Personnel").Attribute("ScoutType").Value);

            Positions = new List<Position>();
            IEnumerable<XElement> elems = doc.Descendants("Position");
            foreach (XElement elem in elems)
            {
                Position pos = new Position();
                pos.Load(elem);
                Positions.Add(pos);
            }
        }
    }
}
