using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

using Webb.Playbook.Data;

namespace Webb.Playbook.Team
{
    /// <summary>
    /// TeamLineup.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class TeamLineup : UserControl
    {
        private Lineup lineup;
        public Lineup Lineup
        {
            get
            {
                if (lineup == null)
                {
                    lineup = new Lineup();
                }
                return lineup;
            }
            set { lineup = value; }

        }

        public TeamLineup()
        {
            InitializeComponent();

            GetAllPlayers();
            addplayers(EditTeam.ObjTeam.LineupSetting);
            DelSelPlayer(EditTeam.ObjTeam.LineupSetting);

            this.DataContext = EditTeam.ObjTeam.LineupSetting;

        }
        private void DelSelPlayer(LineupSetting lineupsetting)
        {
            foreach (Lineup pos in lineupsetting.Quarterback)
            {
                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.Receivers)
            {
                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.Runningbacks)
            {

                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.OffensiveLinemen)
            {

                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.TightEnds)
            {
                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.Cornerbacks)
            {

                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
            foreach (Lineup pos in lineupsetting.Linebackers)
            {

                if (pos.Player != null)
                {
                    Lineup.ExistPlayers.Remove(pos.Player);
                }

            }
        }

        private void addplayers(LineupSetting lineupsetting)
        {
            foreach (Lineup pos in lineupsetting.Quarterback)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);

                }
            }
            foreach (Lineup pos in lineupsetting.Receivers)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }
            foreach (Lineup pos in lineupsetting.Runningbacks)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }
            foreach (Lineup pos in lineupsetting.OffensiveLinemen)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }
            foreach (Lineup pos in lineupsetting.TightEnds)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }
            foreach (Lineup pos in lineupsetting.Cornerbacks)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }
            foreach (Lineup pos in lineupsetting.Linebackers)
            {
                pos.Players.Clear();
                foreach (Player player in pos.ExistPlayers)
                {

                    pos.Players.Add(player);
                }
            }

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            IEnumerable visuals = this.GetVisuals(grid);
            foreach (DependencyObject visual in visuals)
            {
                if (visual.GetType() == typeof(ItemsControl))
                {
                    update(visual as ItemsControl);
                }
            }

        
            (this.Parent as Grid).Children.Remove(this);

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        
            (this.Parent as Grid).Children.Remove(this);


        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                if (!Lineup.ExistPlayers.Contains(e.RemovedItems[0] as Player))
                {
                    Lineup.ExistPlayers.Add(e.RemovedItems[0] as Player);
                }
            }

            if (e.AddedItems.Count > 0)
            {
                Lineup.ExistPlayers.Remove(e.AddedItems[0] as Player);
            }
        }
        private void GetAllPlayers()
        {
            Lineup.ExistPlayers.Clear();

            foreach (Webb.Playbook.Data.Player objPlayer in EditTeam.ObjTeam.Players)
            {

                Lineup.ExistPlayers.Add(objPlayer);
            }
        }
        private void cmbPlayers_DropDownOpened(object sender, EventArgs e)
        {

            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                System.Collections.ObjectModel.ObservableCollection<Player> players = cb.ItemsSource as System.Collections.ObjectModel.ObservableCollection<Player>;

                if (players != null)
                {
                    players.Clear();

                    foreach (Player player in Lineup.ExistPlayers)
                    {
                        players.Add(player);
                    }
                }
            }
        }
        private void update(ItemsControl itemsControl)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                Lineup lineup = itemsControl.Items[i] as Lineup;

                DependencyObject dp = itemsControl.ItemContainerGenerator.ContainerFromItem(lineup);

                DataTemplate dt = (DataTemplate)(this.Resources["LineupTemplate"]);

                ComboBox cb = dt.FindName("cmbPlayers", dp as ContentPresenter) as ComboBox;

                BindingExpression be = cb.GetBindingExpression(ComboBox.SelectedValueProperty);

                be.UpdateSource();
            }

        }

        public System.Collections.IEnumerable GetVisuals(DependencyObject root)
        {

            int count = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < count; i++)
            {

                var child = VisualTreeHelper.GetChild(root, i);

                yield return child;

                foreach (var descendants in GetVisuals(child))
                {

                    yield return descendants;

                }

            }

        }

    }
}
