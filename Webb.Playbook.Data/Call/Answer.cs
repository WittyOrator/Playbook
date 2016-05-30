using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;

namespace Webb.Playbook.Data
{
    public class Item : ISerializableObj, INotifyPropertyChanged
    {

        public Item()
        {
            this.value = "item";
          
        }
        private string value;
        public string Value
        {
            get { return this.value; }
            set 
            { 
                this.value = value;
                OnPropertyChanged("Value");
            }
        }
       

        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            value = element.ReadString("Value");
          
          
        }
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Value", Value);
           

        }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, arg);
            }
        }
        #endregion // INotifyPropertyChanged Members
    }
    public class Answer : ISerializableObj, INotifyPropertyChanged
    {
        public Answer(string strName)
        {
            ID = currentID++;
            Name = strName;
            Items = new ObservableCollection<Item>();
          

        }
        public Answer(bool ignoreID)
        {
            if (!ignoreID)
            {
                ID = currentID++;
            }
            Name = string.Empty;
            Items = new ObservableCollection<Item>();
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
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }

        }
        private ObservableCollection<Item> items;
        public ObservableCollection<Item> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<Item>();
                }
                return items;
            }
            set { items = value; }

        }
        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            ID = element.ReadInt("ID");
            Name = element.ReadString("Name");
            XElement elem = element.Element("Items");
            
            foreach (XElement e in elem.Elements())
            {
                Item item = new Item();

                item.ReadXml(e);
                items.Add(item);
            }
        }
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeInt("ID", ID);
            writer.WriteAttributeString("Name",Name);
            writer.WriteStartElement("Items");
            foreach (Item item in Items)
            {
                writer.WriteStartElement("Item");
                item.WriteXml(writer);
                writer.WriteEndElement();

            }
            writer.WriteEndElement();

        }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, arg);
            }
        }
        #endregion // INotifyPropertyChanged Members
    }
}
