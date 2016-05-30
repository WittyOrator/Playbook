using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Webb.Playbook.Call
{
	public partial class CallItem
	{
        Webb.Playbook.Data.Call call = new Webb.Playbook.Data.Call();
		public CallItem()
		{
           
            this.InitializeComponent();
            //gridCall.DataContext = call;
				
		}

        private void btnDeleteCall_Click(object sender, RoutedEventArgs e)
        {
         

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            gridCall.DataContext = call;
        }
	}
}