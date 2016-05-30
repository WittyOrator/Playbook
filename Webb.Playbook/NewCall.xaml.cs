using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Webb.Playbook
{
    public partial class NewCall
    {
        public NewCall()
        {
            this.InitializeComponent();
            TabItem tab = new TabItem();
            CallItem item = new CallItem();
            tab.Header = "New Call";
            tab.Content = item;
            tabCall.Items.Add(tab);
            this.tabCall.SelectedItem = tab;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}