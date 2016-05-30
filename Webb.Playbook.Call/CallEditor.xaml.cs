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
namespace Webb.Playbook.Call
{
    public partial class CallEditor
    {
        private string strfile;
        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Calls\";
        private static Webb.Playbook.Data.Call objcall;
        public static Webb.Playbook.Data.Call Objcall
        {
            get
            {
                if (objcall == null)
                {
                    objcall = new Webb.Playbook.Data.Call();
                }
                return objcall;
            }
            set
            {
                objcall = value;
            }
        }
        public CallEditor()
        {
            this.InitializeComponent();
            LoadThemes();
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            strfile = FilePath + "Team.Call";
            if (System.IO.File.Exists(strfile))
            {
                Objcall.LoadToXml(strfile);
            }


            grdBinding.DataContext = Objcall;

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


        private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            ChangeTxt objchangetxt = new ChangeTxt("Add Question", "Enter new a Question.",true);
            objchangetxt.Owner = this;
            this.IsEnabled = false;
            objchangetxt.ShowDialog();
            if (objchangetxt.DialogResult == true)
            {
                objcall.Questions.Add(new Question(objchangetxt.txtOut.Text));
            }
        }

        private void btnDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            objcall.Questions.Remove(lstQustion.SelectedItem as Question);
        }

        private void btnEditQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (lstQustion.SelectedIndex == -1)
            {
                MessageBox.Show("please select one question ");
                return;
            }
            ChangeTxt objchangetxt = new ChangeTxt("Edit Question", "Change the Question.",true);
            objchangetxt.Owner = this;
            this.IsEnabled = false;

            objchangetxt.txtOut.Text = (lstQustion.SelectedItem as Question).Content;
            objchangetxt.ShowDialog();
            if (objchangetxt.DialogResult == true)
            {
                (lstQustion.SelectedItem as Question).Content = objchangetxt.txtOut.Text;
            }

        }

        private void btnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            ChangeTxt objchangetxt = new ChangeTxt("Add Call Group", "Enter new Call Group name.");
            objchangetxt.Owner = this;
            this.IsEnabled = false;
            objchangetxt.ShowDialog();

            if (objchangetxt.DialogResult == true)
            {
                Answer answer = new Answer(objchangetxt.txtOut.Text);
                objcall.Answers.Add(answer);
                cmbGroup.SelectedItem = answer;
            }

        }

        private void btnRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            objcall.Answers.Remove(cmbGroup.SelectedItem as Answer);
        }

        private void btnRenameGroup_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroup.SelectedIndex == -1)
            {
                MessageBox.Show("please select one Group");
                return;
            }
            ChangeTxt objchangetxt = new ChangeTxt("Rename Call Group", "Enter another Call Group name.");
            objchangetxt.Owner = this;
            this.IsEnabled = false;
            objchangetxt.txtOut.Text = (cmbGroup.SelectedItem as Answer).Name;
            objchangetxt.ShowDialog();
            if (objchangetxt.DialogResult == true)
            {
                (cmbGroup.SelectedItem as Answer).Name = objchangetxt.txtOut.Text;
            }


        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroup.SelectedIndex == -1)
            {
                MessageBox.Show("please select one Group");
                return;
            }
            ChangeTxt objchangetxt = new ChangeTxt("Add Call Group Item", "Enter new Call Group item name.");
            objchangetxt.Owner = this;
            this.IsEnabled = false;
            objchangetxt.ShowDialog();

            if (objchangetxt.DialogResult == true)
            {
                Item item = new Item();
                item.Value = objchangetxt.txtOut.Text;
                (cmbGroup.SelectedItem as Answer).Items.Add(item);
            }
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            (cmbGroup.SelectedItem as Answer).Items.Remove(lstItems.SelectedItem as Item);
        }

        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if(lstItems.SelectedIndex ==-1)
            {
                MessageBox.Show("please select one item");
                return;
            }
            ChangeTxt objchangetxt = new ChangeTxt("Edit Call Group Item", "Change Call Group item ");
            objchangetxt.Owner = this;
            this.IsEnabled = false;
            objchangetxt.txtOut.Text = (lstItems.SelectedItem as Item).Value;
            objchangetxt.ShowDialog();
            
            if (objchangetxt.DialogResult == true)
            {
                (lstItems.SelectedItem as Item).Value = objchangetxt.txtOut.Text;
            }

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Objcall.SaveToXml(strfile);
            Objcall = null;
            this.Close();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Objcall = null;
            this.Close();
        }
    }
}