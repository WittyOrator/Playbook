using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;

namespace Webb.Playbook.Data
{
    public class Question : ISerializableObj, INotifyPropertyChanged
    {
        public Question(string strcontent)
        {
            ID = currentID++;
            Content = strcontent;

        }
        public Question(bool ignoreID)
        {
            Content = string.Empty;
            if(!ignoreID)
            {
                ID = currentID++;
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
        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged("Content");
            }

        }
        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            ID = element.ReadInt("ID");
            Content = element.ReadString("Content");
        }
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeInt("ID", ID);
            writer.WriteAttributeString("Content", Content);

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
