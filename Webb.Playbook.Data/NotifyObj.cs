using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Webb.Playbook.Data
{
    public class NotifyObj : INotifyPropertyChanged
    {
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

        //public static event PropertyChangedEventHandler SPropertyChanged;

        //protected virtual static void SOnPropertyChanged(string propertyName)
        //{
        //    if (SPropertyChanged != null)
        //    {
        //        PropertyChangedEventArgs arg = new PropertyChangedEventArgs(propertyName);
        //        SPropertyChanged(null, arg);
        //    }
        //}
        #endregion // INotifyPropertyChanged Members
    }
}
