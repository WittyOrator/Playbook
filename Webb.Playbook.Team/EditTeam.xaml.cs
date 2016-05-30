using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;


using Webb.Playbook.Data;
namespace Webb.Playbook.Team
{
    public partial class EditTeam
    {
        private static string File = AppDomain.CurrentDomain.BaseDirectory + "Setting.per";
        private static Webb.Playbook.Data.Team objTeam;
        private static Webb.Playbook.Data.Team otherTeam;
        private static int Max = 7;
        public static Webb.Playbook.Data.Team ObjTeam
        {
            get
            {
                if (objTeam == null)
                {
                    objTeam = new Webb.Playbook.Data.Team();
                }
                return objTeam;
            }
            set
            {
                objTeam = value;
            }
        }
        public static Webb.Playbook.Data.Team OtherTeam
        {

            get
            {
                if (otherTeam == null)
                {
                    otherTeam = new Webb.Playbook.Data.Team();
                }
                return otherTeam;
            }
            set
            {
                otherTeam = value;
            }

        }
        private string strTeamPlayers;
        private string strfile;
        private string strotherfile;
        private LineupSetting lineupsetting;
        public LineupSetting LineupSetting
        {
            get
            {
                if (lineupsetting == null)
                {
                    lineupsetting = new Webb.Playbook.Data.LineupSetting();
                    LineupSetting.Init();
                }
                return lineupsetting;
            }
            set
            {
                lineupsetting = value;
            }
        }

        public EditTeam()
        {
            this.InitializeComponent();
        }
        public EditTeam(string strName)
        {
            this.InitializeComponent();
          
            this.grboxTeam.Header = strName;
            strTeamPlayers = strName + " Players";
            strfile = TeamEditor.FilePath + string.Format("{0}.team", strName);
             ObjTeam.LoadToXml(strfile);
            ObjTeam.LineupSetting.Load(File);
            if (System.IO.File.Exists(strfile))
            {
                LineupSetting.LoadLineup(strfile);
                setplayer(LineupSetting);
            }
            Addplayer();
            gridBinding.DataContext = ObjTeam;
        }
        public void GetTeam()
        {

            strfile = TeamEditor.FilePath + "Our Team.team";
            ObjTeam.LoadToXml(strfile);

            strotherfile = TeamEditor.FilePath + "Other Team.team";
            OtherTeam.LoadToXml(strotherfile);
        }

        private void setplayer(LineupSetting lineupsetting)
        {
            foreach (Lineup pos in lineupsetting.Quarterback)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.Receivers)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.Runningbacks)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.OffensiveLinemen)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.TightEnds)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.Cornerbacks)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
            foreach (Lineup pos in lineupsetting.Linebackers)
            {
                foreach (Player player in ObjTeam.Players)
                {
                    if (pos.Id == player.ID.ToString())
                    {
                        pos.Player = player;
                    }
                }

            }
        }
        private void Addplayer()
        {
            ObjTeam.LineupSetting.Quarterback[0].Player = LineupSetting.Quarterback[0].Player;
            for (int i = 0; i < Max; i++)
            {
                ObjTeam.LineupSetting.Receivers[i].Player = LineupSetting.Receivers[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.Runningbacks[i].Player = LineupSetting.Runningbacks[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.OffensiveLinemen[i].Player = LineupSetting.OffensiveLinemen[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.TightEnds[i].Player = LineupSetting.TightEnds[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.Cornerbacks[i].Player = LineupSetting.Cornerbacks[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.Linebackers[i].Player = LineupSetting.Linebackers[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.DefensiveLinemen[i].Player = LineupSetting.DefensiveLinemen[i].Player;
            }

            for (int i = 0; i < Max; i++)
            {

                ObjTeam.LineupSetting.Safeties[i].Player = LineupSetting.Safeties[i].Player;
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void CommonClickHandler(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {
                case "btnChName":
                    ChangeName();
                    break;
                case "btnChHelmet":
                    ChangeHelment();
                    break;
                case "btnChHome":
                    ChangeHomeJer();
                    break;
                case "btnChAway":
                    ChangeAwayJer();
                    break;
                case "btnShPlayer":
                    ShowPlayer();
                    break;
                case "btnShLine":
                    ShowLineup();
                    break;
                case "btnImPlayer":
                    break;
                case "btnImPlStats":
                    break;
                case "btnOk":
                    Ok();
                    break;
                case "btnCancel":
                    ObjTeam = null;
                    (this.Parent as Grid).Children.Remove(this);

                    break;
            }


        }
        private void ChangeName()
        {
            Changetxt objChangetxt = new Changetxt("Enter New Team Name", "Enter a new name for this team");
            objChangetxt.Owner = (((this.Parent as Grid).Parent as Grid).Parent as Border).Parent as Window;
            bool? dialogResult = objChangetxt.ShowDialog();
            if (dialogResult == true)
            {
                this.txtTeamName.Text = objChangetxt.txtOut.Text;

            }
        }
        private void ChangeHelment()
        {
            HelmetColor objhelemtcolor = new HelmetColor();
            (this.Parent as Grid).Children.Add(objhelemtcolor);

        }
        private void ShowPlayer()
        {
            TeamPlayers objTeamPlayers = new TeamPlayers(strTeamPlayers);
            (this.Parent as Grid).Children.Add(objTeamPlayers);
  
        }
        private void ChangeHomeJer()
        {
            JerseyColor objJerseyColor = new JerseyColor(ObjTeam.HomeJerseyColorSet);
            (this.Parent as Grid).Children.Add(objJerseyColor);

        }
        private void ChangeAwayJer()
        {
            JerseyColor objJerseyColor = new JerseyColor(ObjTeam.AwayJerseyColorSet);
            (this.Parent as Grid).Children.Add(objJerseyColor);

        }
        private void ShowLineup()
        {
            TeamLineup objTeamLineup = new TeamLineup();
            (this.Parent as Grid).Children.Add(objTeamLineup);
    
        }
        private void Ok()
        {
            objTeam.SaveToXml(strfile);
            ObjTeam = null;
            (this.Parent as Grid).Children.Remove(this);
         
        }



    }
}