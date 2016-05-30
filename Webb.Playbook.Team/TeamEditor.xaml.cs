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
    public partial class TeamEditor
    {
        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Teams\";

        public TeamEditor()
        {
            this.InitializeComponent();
            LoadThemes();

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            // 在此点之下插入创建对象所需的代码。
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gridControl.Children.Clear();
        }

        // Themes
        public void LoadThemes()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Themes"))
            {
                foreach (string strFile in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\Themes", "*.xaml"))
                {
                    FileInfo fi = new FileInfo(strFile);

                    MenuItem item = new MenuItem();
                    item.Header = fi.Name;
                    item.Tag = strFile;

                    item.Click += new RoutedEventHandler(item_Click);

                    if (fi.Name == @"Blue.xaml")
                    {//default theme
                        LoadTheme(strFile);
                        SetCheck(item);
                    }

                    this.menuItemThemes.Items.Add(item);
                }
            }
        }
        void item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                string strThemeName = mi.Tag.ToString();

                LoadTheme(strThemeName);
                SetCheck(mi);
            }
        }

        protected void SetCheck(MenuItem mi)
        {
            foreach (object o in this.menuItemThemes.Items)
            {
                MenuItem menuItem = o as MenuItem;
                if (menuItem != null)
                {
                    menuItem.IsChecked = false;
                }
            }
            mi.IsChecked = true;
        }

        public void LoadTheme(string strFile)
        {
            if (File.Exists(strFile))
            {
                ResourceDictionary resDic = new ResourceDictionary();
                resDic.Source = new Uri(strFile, UriKind.RelativeOrAbsolute);
                App.Current.Resources = resDic;
            }
        }

        private void CommonClickHandler(Object sender, RoutedEventArgs e)
        {
            FrameworkElement feSourse = e.Source as FrameworkElement;
            switch (feSourse.Name)
            {
                case "btnEditTeam":
                    EditTeam();
                    break;
                case "btnOurTeam":
                    SetOurTeam();
                    break;
                case "btnOtherTeam":
                    SetOtherTeam();
                    break;
                case "btnExit":
                    this.Window.Close();
                    break;
            }
        }
        private void lstselectionChanged(Object sender, RoutedEventArgs e)
        {
            this.btnOurTeam.IsEnabled = this.btnOtherTeam.IsEnabled = true;
            string lstSelect = (lstTeam.SelectedItem as ListBoxItem).Content.ToString();
            switch (lstSelect)
            {

                case "OtherTeam(OtherTeam)":
                    this.btnOtherTeam.IsEnabled = false;

                    break;
                case "OurTeam(OurTeam)":
                    this.btnOurTeam.IsEnabled = false;
                    break;

            }

        }
        private void EditTeam()
        {
            if (lstTeam.SelectedIndex == -1)
            {

                MessageBox.Show("please select one team!");
                return;
            }
            string lstSelect = (lstTeam.SelectedItem as ListBoxItem).Content.ToString();
            switch (lstSelect)
            {

                case "OtherTeam(OtherTeam)":
                    EditTeam objOtherTeam = new EditTeam("Other Team");
                    objOtherTeam.gridhome.Visibility = Visibility.Hidden;

                    gridControl.Children.Add(objOtherTeam);


                    break;
                case "OurTeam(OurTeam)":
                    EditTeam objOurTeam = new EditTeam("Our Team");
                    objOurTeam.gridAway.Visibility = Visibility.Hidden;
                    gridControl.Children.Add(objOurTeam);
                    break;
            }

        }
        private void SetOurTeam()
        {
            EditTeam objOurTeam = new EditTeam("Our Team");
            objOurTeam.gridAway.Visibility = Visibility.Hidden;
            gridControl.Children.Add(objOurTeam);


        }
        private void SetOtherTeam()
        {
            EditTeam objOtherTeam = new EditTeam("Other Team");
            objOtherTeam.gridhome.Visibility = Visibility.Hidden;
            gridControl.Children.Add(objOtherTeam);

        }


    }
}