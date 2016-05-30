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
    /// FieldsSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FieldsSettingWindow : Window
    {
        public FieldsSettingWindow()
        {
            InitializeComponent();

            tbPath.Text = GameSetting.Instance.UserFolder;

            Webb.Playbook.Data.Terminology.Instance.SetTermFilePath(GameSetting.Instance.UserFolder);

            this.DataContext = null;
            this.DataContext = Webb.Playbook.Data.Terminology.Instance;

            cbOM.SelectedValue = GameSetting.Instance.OffensiveMainField;
            cbOS.SelectedValue = GameSetting.Instance.OffensiveSubField;
            cbDM.SelectedValue = GameSetting.Instance.DefensiveMainField;
            cbDS.SelectedValue = GameSetting.Instance.DefensiveSubField;
            cbKM.SelectedValue = GameSetting.Instance.KickMainField;
            cbKS.SelectedValue = GameSetting.Instance.KickSubField;

            cbScoutType.SelectedValue = GameSetting.Instance.UserType;
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (System.IO.Directory.Exists(GameSetting.Instance.UserFolder))
            {
                fbd.SelectedPath = GameSetting.Instance.UserFolder;
            }

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbPath.Text = fbd.SelectedPath;

                GameSetting.Instance.UserFolder = fbd.SelectedPath;

                Webb.Playbook.Data.Terminology.Instance.SetTermFilePath(GameSetting.Instance.UserFolder);

                this.DataContext = null;
                this.DataContext = Webb.Playbook.Data.Terminology.Instance;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            GameSetting.Instance.OffensiveMainField = cbOM.Text != string.Empty ? cbOM.Text : "Formation";
            GameSetting.Instance.OffensiveSubField = cbOS.Text != string.Empty ? cbOS.Text : "Play Name";
            GameSetting.Instance.DefensiveMainField = cbDM.Text != string.Empty ? cbDM.Text : "Front";
            GameSetting.Instance.DefensiveSubField = cbDS.Text != string.Empty ? cbDS.Text : "Defense";
            GameSetting.Instance.KickMainField = cbKM.Text != string.Empty ? cbKM.Text : "Kick Type";
            GameSetting.Instance.KickSubField = cbKS.Text != string.Empty ? cbKS.Text : "Play Name";

            GameSetting.Instance.UserType = (Data.UserTypes)cbScoutType.SelectedValue;

            this.DialogResult = true;

            CloseWindow();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            CloseWindow();
        }

        private void CloseWindow()
        {
            this.Close();
        }
    }
}
