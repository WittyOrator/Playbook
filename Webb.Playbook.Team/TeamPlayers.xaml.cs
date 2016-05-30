using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Webb.Playbook.Team
{
	public partial class TeamPlayers
	{
       
     
        public TeamPlayers()
		{
			this.InitializeComponent();
       
		}
        public TeamPlayers(string strTeamPkayers) 
        {

            this.InitializeComponent();
            gpboxTeamPlayers.Header = strTeamPkayers;
            lstPlayer.DataContext = EditTeam.ObjTeam;


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
     
        private void CommonClickHandler(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {
                case "btnEdPlayer":
                    EdPlayer();
                    break;
                case "btnCrPlayer":
                    EditTeam.ObjTeam.Players.Add(new Webb.Playbook.Data.Player());
                    break;
                case "btnCoPlayer":
                    break;
                case "btnDePlayer":
                    EditTeam.ObjTeam.Players.Remove(lstPlayer.SelectedItem as Webb.Playbook.Data.Player);
                    break;
                case "btnViwTeSta":
                    break;
                case "btnClose":
                    (this.Parent as Grid).Children.Remove(this);
                    break;
            }
        }
        private void EdPlayer() 
        {
            if (lstPlayer.SelectedIndex == -1) 
            {
                MessageBox.Show("Please select one PLayer");
                return;
            }
          
            EditPlayer objEditPlayer = new EditPlayer(lstPlayer.SelectedItem as Webb.Playbook.Data.Player);
           
              (this.Parent as Grid).Children.Add(objEditPlayer);



        }

      
	}
}