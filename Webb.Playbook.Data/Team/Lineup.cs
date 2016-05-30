using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Webb.Playbook.Data
{
    public enum LineupTypes
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

    public class Lineup : NotifyObj
    {
        private Player player;
        public Player Player
        {
            get
            {
                return player;
            }
            set
            { 
                player = value;
             
            }
        }
        public Lineup()
        {
            this.name = string.Empty;
            this.value = string.Empty;
            this.Id = string.Empty;

        }

        public Lineup(string name)
        {
            this.name = name;
            this.value = string.Empty;
            this.Id = string.Empty;
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

        private string namevalue;
        public string NameValue
        {
            get { return namevalue; }
            set { namevalue = value; }
        }
        private string id;
        public string Id 
        {
            get { return id;  }
            set { id = value; }

        }
        private static ObservableCollection<Player> existPlayers;
        public  ObservableCollection<Player> ExistPlayers
        {
            get
            {
                if (existPlayers == null)
                {
                    existPlayers = new ObservableCollection<Player>();
                }
                return existPlayers;
            }
            set
            {
                existPlayers = value;
              
            }
        }

        private ObservableCollection<Player> players;
        public ObservableCollection<Player> Players
        {
            get
            {
                if (players == null)
                {
                    players = new ObservableCollection<Player>();
                }
                return players;
            }
            set
            {
                players = value;
           

            }
        }


        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("Name", NameValue);
            string strId = player == null ? string.Empty : player.ID.ToString();

            writer.WriteAttributeString("Value", strId);
            writer.WriteEndElement();

        }
        public void Load(XElement element)
        {
            XAttribute xa = element.Attribute("Name");
            if (xa != null)
            {
                Name = element.Attribute("Name").Value;
            }
            xa = element.Attribute("Value");
            if (xa != null)
            {
                Value = element.Attribute("Value").Value;
            }
            NameValue = Value != string.Empty ? Value : Name;

        }
        public void LoadLineup(XElement element)
        {
            XAttribute xa = element.Attribute("Name");
            if (xa != null)
            {
                NameValue = element.Attribute("Name").Value;
            }
            xa = element.Attribute("Value");
            if (xa != null)
            {
                Id = element.Attribute("Value").Value;

            }
        }
    }
    public class LineupSetting
    {
        private static int Max = 7;
        private List<Lineup> quarterback;
        public List<Lineup> Quarterback
        {
            get { return quarterback; }
            set { quarterback = value; }
        }
        private List<Lineup> receivers;
        public List<Lineup> Receivers
        {
            get { return receivers; }
            set { receivers = value; }
        }
        private List<Lineup> runningbacks;
        public List<Lineup> Runningbacks
        {
            get { return runningbacks; }
            set { runningbacks = value; }
        }
        private List<Lineup> offensiveLinemen;
        public List<Lineup> OffensiveLinemen
        {
            get { return offensiveLinemen; }
            set { offensiveLinemen = value; }
        }
        private List<Lineup> tightEnds;
        public List<Lineup> TightEnds
        {
            get { return tightEnds; }
            set { tightEnds = value; }
        }

        private List<Lineup> cornerbacks;
        public List<Lineup> Cornerbacks
        {
            get { return cornerbacks; }
            set { cornerbacks = value; }
        }
        private List<Lineup> linebackers;
        public List<Lineup> Linebackers
        {
            get { return linebackers; }
            set { linebackers = value; }
        }
        private List<Lineup> defensiveLinemen;
        public List<Lineup> DefensiveLinemen
        {
            get { return defensiveLinemen; }
            set { defensiveLinemen = value; }
        }
        private List<Lineup> safeties;
        public List<Lineup> Safeties
        {
            get { return safeties; }
            set { safeties = value; }
        }

        public LineupSetting()
        {
            Quarterback = new List<Lineup>(1);
            Receivers = new List<Lineup>(Max);
            Runningbacks = new List<Lineup>(Max);
            OffensiveLinemen = new List<Lineup>(Max);
            TightEnds = new List<Lineup>(Max);
            Cornerbacks = new List<Lineup>(Max);
            Linebackers = new List<Lineup>(Max);
            DefensiveLinemen = new List<Lineup>(Max);
            Safeties = new List<Lineup>(Max);
        }



        public void Init()
        {

            Quarterback.Add(new Lineup("QB"));
            for (int i = 1; i <= Max; i++)
            {
                Receivers.Add(new Lineup("WR" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                Runningbacks.Add(new Lineup("RB" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                OffensiveLinemen.Add(new Lineup("OL" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                TightEnds.Add(new Lineup("TE" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                Cornerbacks.Add(new Lineup("CB" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                Linebackers.Add(new Lineup("LB" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                DefensiveLinemen.Add(new Lineup("DL" + i.ToString()));
            }

            for (int i = 1; i <= Max; i++)
            {
                Safeties.Add(new Lineup("S" + i.ToString()));
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("Name", "Quarterback");
            foreach (Lineup pos in Quarterback)
            {
                pos.WriteXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "Receivers");
            foreach (Lineup pos in Receivers)
            {
                pos.WriteXml(writer);

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "Runningbacks");
            foreach (Lineup pos in Runningbacks)
            {

                pos.WriteXml(writer);

            }
            writer.WriteEndElement();
            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "OffensiveLinemen");
            foreach (Lineup pos in OffensiveLinemen)
            {

                pos.WriteXml(writer);

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "TightEnds");
            foreach (Lineup pos in TightEnds)
            {
                pos.WriteXml(writer);

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "Cornerbacks");
            foreach (Lineup pos in Cornerbacks)
            {
                pos.WriteXml(writer);

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "Linebackers");
            foreach (Lineup pos in Linebackers)
            {

                pos.WriteXml(writer);

            }
            writer.WriteEndElement();


            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "DefensiveLinemen");
            foreach (Lineup pos in DefensiveLinemen)
            {

                pos.WriteXml(writer);

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Positions");
            writer.WriteAttributeString("Name", "Safeties");
            foreach (Lineup pos in Safeties)
            {

                pos.WriteXml(writer);
                ;
            }
            writer.WriteEndElement();


        }
        public void Load(string strFile)
        {
            this.Clear();
            if (System.IO.File.Exists(strFile))
            {
                XDocument doc = XDocument.Load(strFile);

                XElement elem = doc.Descendants("Position").First(f => f.Attribute("Name").Value == "QB");
                Lineup lineup = new Lineup();
                lineup.Load(elem);
                Quarterback.Add(lineup);


                IEnumerable<XElement> elems = doc.Descendants("Positions");

                IEnumerable<XElement> posElems = elems.First(f => f.Attribute("Name").Value == "Receivers").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    Receivers.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Runningbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    Runningbacks.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "OffensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    OffensiveLinemen.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "TightEnds").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    TightEnds.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Cornerbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    Cornerbacks.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Linebackers").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    Linebackers.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "DefensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    DefensiveLinemen.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Safeties").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    Safeties.Add(pos);
                }
            }
            else
            {
                this.Init();
            }


        }
        public void Clear()
        {
            Quarterback.Clear();
            Receivers.Clear();
            Runningbacks.Clear();
            OffensiveLinemen.Clear();
            TightEnds.Clear();
            Cornerbacks.Clear();
            Linebackers.Clear();
            DefensiveLinemen.Clear();
            Safeties.Clear();
        }
        public void LoadLineup(string strFile)
        {
            this.Clear();
            if (System.IO.File.Exists(strFile))
            {
                XDocument doc = XDocument.Load(strFile);
                IEnumerable<XElement> elem = doc.Descendants("Position");
                IEnumerable<XElement> posElems = elem.First(f => f.Attribute("Name").Value == "Quarterback").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.Load(e);
                    pos.LoadLineup(e);
                    Quarterback.Add(pos);
                }

                IEnumerable<XElement> elems = doc.Descendants("Positions");

                 posElems = elems.First(f => f.Attribute("Name").Value == "Receivers").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    Receivers.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Runningbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    Runningbacks.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "OffensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    OffensiveLinemen.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "TightEnds").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    TightEnds.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Cornerbacks").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    Cornerbacks.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Linebackers").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    Linebackers.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "DefensiveLinemen").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    DefensiveLinemen.Add(pos);
                }

                posElems = elems.First(f => f.Attribute("Name").Value == "Safeties").Elements();
                foreach (XElement e in posElems)
                {
                    Lineup pos = new Lineup();
                    pos.LoadLineup(e);
                    Safeties.Add(pos);
                }
            }
            else
            {
                this.Init();
            }


        }


    }
}
