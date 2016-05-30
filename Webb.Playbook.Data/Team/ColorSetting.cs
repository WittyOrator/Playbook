using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
namespace Webb.Playbook.Data
{
    public class HelmetColorSet : ISerializableObj, INotifyPropertyChanged
    {
        public HelmetColorSet()
        {
            BaseColor = Colors.Blue;
            StripeColor = Colors.White;
        }

        private Color baseColor;
        public Color BaseColor
        {
            get { return baseColor; }
            set
            {
                baseColor = value;
                OnPropertyChanged("BaseColor");
            }
        }

        private Color stripeColor;
        public Color StripeColor
        {
            get { return stripeColor; }
            set
            {
                stripeColor = value;
                OnPropertyChanged("StripeColor");
            }
        }

        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            if (element.HasElements)
            {
                BaseColor = element.ReadElementColor("BaseColor");
                StripeColor = element.ReadElementColor("StripeColor");
            }
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementColor("BaseColor", BaseColor);
            writer.WriteElementColor("StripeColor", StripeColor);
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

    public class JerseyColorSet : ISerializableObj, INotifyPropertyChanged
    {
        public JerseyColorSet()
        {
            TorsoColor = Colors.Yellow;
            PadsColor = Colors.Violet;
            TextColor = Colors.White;

            LegPrimaryColor = Colors.LightBlue;
            LegStripeColor = Colors.Green;
        }

        //Jersey
        private Color torsoColor;
        public Color TorsoColor
        {
            get { return torsoColor; }
            set
            {
                torsoColor = value;
                OnPropertyChanged("TorsoColor");
            }
        }

        private Color padsColor;
        public Color PadsColor
        {
            get { return padsColor; }
            set
            {
                padsColor = value;
                OnPropertyChanged("TorsoColor");
            }
        }

        private Color textColor;
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        //Pants
        private Color legPrimaryColor;
        public Color LegPrimaryColor
        {
            get { return legPrimaryColor; }
            set
            {
                legPrimaryColor = value;
                OnPropertyChanged("LegPrimaryColor");
            }
        }

        private Color legStripeColor;
        public Color LegStripeColor
        {
            get { return legStripeColor; }
            set
            {
                legStripeColor = value;
                OnPropertyChanged("LegStripeColor");
            }
        }

        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            if (element.HasElements)
            {
                TorsoColor = element.ReadElementColor("TorsoColor");
                PadsColor = element.ReadElementColor("PadsColor");
                TextColor = element.ReadElementColor("TextColor");
                LegPrimaryColor = element.ReadElementColor("LegPrimaryColor");
                LegStripeColor = element.ReadElementColor("LegStripeColor");
            }
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementColor("TorsoColor", TorsoColor);
            writer.WriteElementColor("PadsColor", PadsColor);
            writer.WriteElementColor("TextColor", TextColor);
            writer.WriteElementColor("LegPrimaryColor", LegPrimaryColor);
            writer.WriteElementColor("LegStripeColor", LegStripeColor);

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

