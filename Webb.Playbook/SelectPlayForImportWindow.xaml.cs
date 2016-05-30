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

using System.IO;

namespace Webb.Playbook
{
    /// <summary>
    /// SelectPlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlayForImportWindow : Window
    {
        private string packageFile;
        public string PackageFile
        {
            get
            {
                return packageFile;
            }
        }

        private string tempUserFolder;
        public string TempUserFolder
        {
            get
            {
                return tempUserFolder;
            }
        }

        private Webb.Playbook.ViewModel.FormationRootViewModel formationTempRootViewModel;

        private Webb.Playbook.ViewModel.PlaybookRootViewModel playbookTempRootViewModel;

        private Webb.Playbook.ViewModel.FormationRootViewModel formationRootViewModel;

        private Webb.Playbook.ViewModel.PlaybookRootViewModel playbookRootViewModel;

        public SelectPlayForImportWindow(string strPackageFile, string strTempUserFolder)
        {
            this.packageFile = strPackageFile;
            this.tempUserFolder = strTempUserFolder;

            InitializeComponent();

            ZipClass.UnZip(strPackageFile, strTempUserFolder, true);

            formationTempRootViewModel = new Webb.Playbook.ViewModel.FormationRootViewModel(strTempUserFolder);
            treeFormationTemp.DataContext = formationTempRootViewModel;

            playbookTempRootViewModel = new Webb.Playbook.ViewModel.PlaybookRootViewModel(strTempUserFolder, Webb.Playbook.ViewModel.ViewMode.PlayNameView);
            treePlaybookTemp.DataContext = playbookTempRootViewModel;

            formationRootViewModel = new Webb.Playbook.ViewModel.FormationRootViewModel(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);
            treeFormation.DataContext = formationRootViewModel;

            playbookRootViewModel = new Webb.Playbook.ViewModel.PlaybookRootViewModel(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, Webb.Playbook.ViewModel.ViewMode.PlayNameView);
            treePlaybook.DataContext = playbookRootViewModel;
        }

        private void RefreshCurrentTree()
        {
            treeFormation.DataContext = null;
            treePlaybook.DataContext = null;

            formationRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);
            playbookRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);

            treeFormation.DataContext = formationRootViewModel;
            treePlaybook.DataContext = playbookRootViewModel;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void treeFormation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && (this.treeFormationTemp.IsKeyboardFocusWithin || this.treePlaybookTemp.IsKeyboardFocusWithin))
            {
                this.btnImport_Click(null, null);
            }
        }

        private void btnImportAll_Click(object sender, RoutedEventArgs e)
        {
            CopyDirectory.CopyFolderFiles(TempUserFolder, Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, chkOverwrite.IsChecked.Value,null);

            //ZipClass.UnZip(PackageFile, Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, chkOverwrite.IsChecked.Value);

            RefreshCurrentTree();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                ViewModel.TreeViewItemViewModel fTempVM = this.treeFormationTemp.SelectedItem as ViewModel.TreeViewItemViewModel;
                ViewModel.TreeViewItemViewModel fVM = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;

                string strFolder = string.Empty;

                if (fVM is ViewModel.PlayViewModel)
                {
                    fVM = fVM.Parent.Parent;
                }
                if (fVM is ViewModel.FormationViewModel)
                {
                    fVM = fVM.Parent;
                }
                if (fVM is ViewModel.ScoutTypeViewModel)
                {
                    strFolder = (fVM as ViewModel.ScoutTypeViewModel).ScoutTypePath;
                }
                if (fVM is ViewModel.FolderViewModel)
                {
                    strFolder = (fVM as ViewModel.FolderViewModel).FolderPath;
                }

                if (System.IO.Directory.Exists(strFolder))
                {
                    if (fTempVM is ViewModel.FormationViewModel)
                    {
                        ViewModel.FormationViewModel formationVM = fTempVM as ViewModel.FormationViewModel;

                        formationVM.CopyTo(strFolder,chkOverwrite.IsChecked.Value);
                    }
                    else if (fTempVM is ViewModel.FolderViewModel)
                    {
                        ViewModel.FolderViewModel folderVM = fTempVM as ViewModel.FolderViewModel;

                        CopyDirectory.CopyFolderFiles(folderVM.FolderPath, strFolder + @"\" + folderVM.FolderName, chkOverwrite.IsChecked.Value,null);
                    }
                    else if (fTempVM is ViewModel.TitleViewModel)
                    {// 10-27-2011 Scott
                        ViewModel.TitleViewModel titleVM = fTempVM as ViewModel.TitleViewModel;

                        titleVM.CopyTo(strFolder, chkOverwrite.IsChecked.Value);
                    }
                    else
                    {
                        MessageBox.Show("Please select a formation or folder to import");

                        return;
                    }

                    fVM.Refresh();
                    fVM.IsExpanded = true;
                }
                else
                {
                    MessageBox.Show("Please select a folder to import");
                }
            }

            if (tabControl.SelectedIndex == 1)
            {
                ViewModel.TreeViewItemViewModel pTempVM = this.treePlaybookTemp.SelectedItem as ViewModel.TreeViewItemViewModel;
                ViewModel.TreeViewItemViewModel pVM = this.treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;

                string strFolder = string.Empty;

                if (pVM is ViewModel.PlayViewModel)
                {
                    pVM = pVM.Parent;
                }
                if (pVM is ViewModel.PlayTypeViewModel)
                {
                    strFolder = (pVM as ViewModel.PlayTypeViewModel).PlayTypePath;
                }
                if (pVM is ViewModel.PlayFolderViewModel)
                {
                    strFolder = (pVM as ViewModel.PlayFolderViewModel).FolderPath;
                }

                if (System.IO.Directory.Exists(strFolder))
                {
                    if (pTempVM is ViewModel.PlayViewModel)
                    {
                        ViewModel.PlayViewModel playVM = pTempVM as ViewModel.PlayViewModel;

                        playVM.CopyTo(strFolder, chkOverwrite.IsChecked.Value);
                    }
                    else if (pTempVM is ViewModel.PlayFolderViewModel)
                    {
                        ViewModel.PlayFolderViewModel folderVM = pTempVM as ViewModel.PlayFolderViewModel;

                        CopyDirectory.CopyFolderFiles(folderVM.FolderPath, strFolder + @"\" + folderVM.FolderName, chkOverwrite.IsChecked.Value,null);
                    }
                    else
                    {
                        MessageBox.Show("Please select a play or folder to import");

                        return;
                    }

                    pVM.Refresh();
                    pVM.IsExpanded = true;
                }
                else
                {
                    MessageBox.Show("Please select a destination folder to import");
                }
            }
        }
    }
}
