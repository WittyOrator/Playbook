using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Webb.Playbook.Data
{
    public enum DominantHand
    {
        Right,
        Left,
    }
    public enum Appearance
    {
        Black,
        Yellow,
        White,
        
    }
    public class Player : ISerializableObj
    {
        public Player()
        {
            Name = "Player";
            Number = 0;
            Height = 0;
            Weight = 0;
            Note = string.Empty;
            MydominantHand = DominantHand.Right;
            Myappearance=Appearance.White;
            ID = CurrentID++;
            Name = string.Format("Player#{0}",ID);

        }
        public DominantHand MydominantHand { get; set; }
        public Appearance Myappearance { get; set; }
        public Player(bool ignoreID)
        {
            Name = "Player";
            Number = 0;
            Height = 0;
            Weight = 0;
            Note = string.Empty;
            MydominantHand = DominantHand.Right;
            Myappearance = Appearance.White;
            if (!ignoreID)
            {
                ID = CurrentID++;
            }
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

        private int number;
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        private double weight;
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private string note;
        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
                ID = element.ReadInt("ID");
           
            if (element.ReadString("Name") != null)
            {

                Name = element.ReadString("Name");
            }
        
                Number = element.ReadInt("Number");
                Height = element.ReadDouble("Height");
                Weight = element.ReadDouble("Weight");
           
            if (element.ReadString("DominantHand") != null)
            {
                MydominantHand = (DominantHand)Enum.Parse(typeof(DominantHand), element.ReadString("DominantHand"));
            }
            if (element.ReadString("Appearance") != null)
            {
                Myappearance = (Appearance)Enum.Parse(typeof(Appearance), element.ReadString("Appearance"));
            }
            if (element.ReadString("Note") != null)
            {
                Note = element.ReadString("Note");
            }
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeInt("ID", ID);
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeInt("Number", Number);
            writer.WriteAttributeDouble("Height", Height);
            writer.WriteAttributeDouble("Weight", Weight);
            writer.WriteAttributeString("DominantHand", MydominantHand.ToString());
            writer.WriteAttributeString("Appearance", Myappearance.ToString());

            writer.WriteAttributeString("Note", Note);
        }
    }
}
