using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;
using System.IO;
using System.Xml;


namespace Webb.Playbook.Data
{
    public class Team : ISerializableObj
    {
        public Team()
        {
            Name = "Team";

            HelmetColorSet = new HelmetColorSet();
            HomeJerseyColorSet = new JerseyColorSet();
            AwayJerseyColorSet = new JerseyColorSet();
            LineupSetting = new LineupSetting();
            Players = new ObservableCollection<Player>();


            ID = CurrentID++;
        }

        private static int currentID = 0;
        public static int CurrentID
        {
            get { return currentID; }
            set { currentID = value; }
        }

        private int id = 0;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private HelmetColorSet helmetColorSet;
        public HelmetColorSet HelmetColorSet
        {
            get { return helmetColorSet; }
            set { helmetColorSet = value; }
        }

        private JerseyColorSet homeJerseyColorSet;
        public JerseyColorSet HomeJerseyColorSet
        {
            get { return homeJerseyColorSet; }
            set { homeJerseyColorSet = value; }
        }

        private JerseyColorSet awayJerseyColorSet;
        public JerseyColorSet AwayJerseyColorSet
        {
            get { return awayJerseyColorSet; }
            set { awayJerseyColorSet = value; }
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
            set { players = value; }
        }
        private LineupSetting lineupSetting;
        public LineupSetting LineupSetting
        {
            get { return lineupSetting; }
            set { lineupSetting = value; }

        }
        private Lineup lineup;
        public Lineup Lineup
        {
            get { return lineup; }
            set { lineup = value; }

        }
        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            ID = element.ReadInt("ID");
            Name = element.ReadString("Name");
            Player.CurrentID = element.ReadInt("PlayerCurrentID");

            //color
            XElement elem = element.GetChildElement("HelmetColorSet");
            HelmetColorSet.ReadXml(elem);

            elem = element.GetChildElement("HomeJerseyColorSet");
            HomeJerseyColorSet.ReadXml(elem);

            elem = element.GetChildElement("AwayJerseyColorSet");
            AwayJerseyColorSet.ReadXml(elem);

            //players
            elem = element.Element("Players");

            Players.Clear();
            foreach (XElement e in elem.Elements())
            {
                Player player = new Player(true);

                player.ReadXml(e);


                Players.Add(player);
            }
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Team");

            writer.WriteAttributeInt("ID", ID);
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeInt("PlayerCurrentID", Player.CurrentID);
            //jersey color
            writer.WriteStartElement("ColorSet");
            writer.WriteAttributeString("Name", "HelmetColorSet");
            HelmetColorSet.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("ColorSet");
            writer.WriteAttributeString("Name", "HomeJerseyColorSet");
            HomeJerseyColorSet.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("ColorSet");
            writer.WriteAttributeString("Name", "AwayJerseyColorSet");
            AwayJerseyColorSet.WriteXml(writer);
            writer.WriteEndElement();
            //players
            writer.WriteStartElement("Players");
            foreach (Player player in Players)
            {
                writer.WriteStartElement("Player");
                player.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            //Lineup
            writer.WriteStartElement("PositionSetting");
            LineupSetting.WriteXml(writer);

            writer.WriteEndElement();


            writer.WriteEndElement();
        }

        public void SaveToXml(string FileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                })
                )
            {
                WriteXml(writer);

            }
            File.WriteAllText(FileName, Encoding.UTF8.GetString(stream.ToArray()));


        }
        public void LoadToXml(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                XElement elem = XElement.Load(FileName);
                ReadXml(elem);

            }

        }

    }
}
