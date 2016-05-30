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
    public partial class CallItem
    {
        Webb.Playbook.Data.Call call = new Webb.Playbook.Data.Call();
        public CallItem()
        {

            this.InitializeComponent();
            gridCall.DataContext = call;

        }

        private void btnDeleteCall_Click(object sender, RoutedEventArgs e)
        {


        }

        private void lstGenItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstGenItems.SelectedIndex != -1)
            {

                addCallItem();
            }
        }
        public void addCallItem()
        {
            TabItem tab = new TabItem();
            CallItem item = new CallItem();
            tab.Header = "New Call";
            tab.Content = item;
            if (this.Parent is TabItem)
            {
                TabItem tabitem = this.Parent as TabItem;
                if (tabitem.Parent is TabControl)
                {
                    TabControl tabcontrol = tabitem.Parent as TabControl;
                    tabcontrol.Items.Add(tab);
                    tabcontrol.SelectedItem = tab;
                }

            }


        }
    }
}