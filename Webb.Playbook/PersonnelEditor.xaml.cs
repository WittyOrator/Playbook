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
using System.Windows.Shapes;

using Webb.Playbook.Data;

namespace Webb.Playbook
{
    /// <summary>
    /// PersonnelEditor.xaml 的交互逻辑
    /// </summary>
    public partial class PersonnelEditor : Window
    {
        public int AddCount = 0;

        public event Action<PersonnelEditor, Position> PositionSelected;

        public static string File = AppDomain.CurrentDomain.BaseDirectory + "setting.per";
        private static PersonnelSetting setting;
        public static PersonnelSetting Setting
        {
            get
            {
                if (setting == null)
                {
                    setting = new PersonnelSetting();
                }
                return setting;
            }
        }

        public static string TeamPath = AppDomain.CurrentDomain.BaseDirectory + @"Teams\";
        public static string TeamFile = AppDomain.CurrentDomain.BaseDirectory + @"Teams\Team";
        private static List<PersonnelSetting> teams;
        public static List<PersonnelSetting> Teams
        {
            get
            {
                if (teams == null)
                {
                    teams = new List<PersonnelSetting>();

                    for (int i = 0; i < 5; i++)
                    {
                        teams.Add(new PersonnelSetting());
                    }
                }

                return teams;
            }
        }

        // 09-13-2010 Scott
        public bool OffensiveOnly
        {
            set
            {
                DefensiveColumn.Width = new GridLength(value ? 0 : 50, GridUnitType.Star);
                btnCancel.SetValue(Grid.ColumnProperty, 0);
                this.Width = 310;
            }
        }

        // 09-13-2010 Scott
        public bool DefensiveOnly
        {
            set
            {
                OffensiveColumn.Width = new GridLength(value ? 0 : 50, GridUnitType.Star);
                btnOk.SetValue(Grid.ColumnProperty, 2);
                this.Width = 310;
            }
        }

        private bool editMode;
        public bool EditMode
        {
            get { return editMode; }
            set 
            { 
                editMode = value;

                if (editMode)
                {
                    Setting.SetBtnVisibility(Visibility.Hidden);
                }
                else
                {
                    Setting.SetBtnVisibility(Visibility.Visible);
                }
            }
        }

        public static string OffHasBallPlayer
        {
            get
            {
                string strOffHasBallPlayer = string.Empty;

                foreach (Position pos in Setting.OffensiveLinemen)
                {
                    if (pos.HasBall)
                    {
                        strOffHasBallPlayer = pos.Name; 
                    }
                }

                return strOffHasBallPlayer;
            }
        }

        public static string DefHasBallPlayer
        {
            get
            {
                string strDefHasBallPlayer = string.Empty;

                foreach (Position pos in Setting.DefensiveLinemen)
                {
                    if (pos.HasBall)
                    {
                        strDefHasBallPlayer = pos.Name;
                    }
                }

                return strDefHasBallPlayer;
            }
        }

        public PersonnelEditor()
        {
            InitializeComponent();

            Setting.Load(File);

            for (int i = 0; i < Teams.Count; i++)
            {
                string strFile = TeamFile + (i + 1).ToString();

                Teams[i].Load(strFile);
            }

            comboTeams.SelectedIndex = 0;

            EditMode = true;

            this.DataContext = Setting;
        }

        public PersonnelEditor(bool bEditMode)
        {
            InitializeComponent();

            Setting.Load(File);

            for (int i = 0; i < Teams.Count; i++)
            {
                string strFile = TeamFile + (i + 1).ToString();

                Teams[i].Load(strFile);
            }

            comboTeams.SelectedIndex = 0;

            EditMode = bEditMode;

            if (!EditMode)
            {
                this.Width = 560;
                spTeams.Visibility = Visibility.Hidden;
                this.ResizeMode = ResizeMode.NoResize;
            }

            this.DataContext = Setting;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Setting.Save(File);

            if (!System.IO.Directory.Exists(TeamPath))
            {
                System.IO.Directory.CreateDirectory(TeamPath);
            }
            for (int i = 0; i < Teams.Count; i++)
            {
                string strFile = TeamFile + (i + 1).ToString();

                Teams[i].Save(strFile);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            if ( image != null )
            {
                Position pos = image.DataContext as Position;

                if ( pos != null )
                {
                    if (pos.Name.StartsWith("OL"))
                    {
                        Setting.ClearOffBall();
                    }
                    if (pos.Name.StartsWith("DL"))
                    {
                        Setting.ClearDefBall();
                    }
                    pos.HasBall = true;
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PositionSelected != null)
            {
                FrameworkElement fe = sender as FrameworkElement;
                if (fe != null)
                {
                    Position pos = fe.DataContext as Position;

                    if (pos != null)
                    {
                        if (pos.Value == string.Empty)
                        {
                            return;
                        }

                        PositionSelected(this, pos);
                    }
                    else
                    {
                        PositionSelected(this, Setting.Quarterback);
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            textBox.Text = textBox.Text.Trim();

            TextChange[] change = new TextChange[e.Changes.Count];

            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;

            if (change[0].AddedLength > 0)
            {
                double num = 0;

                if (!Double.TryParse(textBox.Text, out num))
                {
                    int delLen = Math.Min(textBox.Text.Length - offset, change[0].AddedLength);

                    textBox.Text = textBox.Text.Remove(offset, delLen);

                    textBox.Select(offset, 0);
                }

                if (textBox.Text.Length > 2)
                {
                    textBox.Text = textBox.Text.Remove(2);
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return;
            }

            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) || ((sender as TextBox).Text.Length >= 2 && (sender as TextBox).SelectedText == string.Empty))
            {
                e.Handled = true;
            }
        }

        private void comboTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                string strPreItem = (e.RemovedItems[0] as TextBlock).Text;

                int index = Int32.Parse(strPreItem.Substring(strPreItem.Length - 1)) - 1;

                Teams[index].LoadPlayers(Setting);
            }

            Setting.LoadPlayers(Teams[comboTeams.SelectedIndex]);
        }
    }
}
