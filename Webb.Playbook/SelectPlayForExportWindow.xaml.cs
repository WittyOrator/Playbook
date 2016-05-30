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

using System.Threading;
using System.Windows.Threading;

using System.IO;

namespace Webb.Playbook
{
    /// <summary>
    /// SelectPlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlayForExportWindow : Window
    {
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

        public SelectPlayForExportWindow(string strTempUserFolder)
        {
            System.IO.Directory.CreateDirectory(strTempUserFolder + @"\Formation");
            System.IO.Directory.CreateDirectory(strTempUserFolder + @"\Playbook");

            this.tempUserFolder = strTempUserFolder;

            InitializeComponent();

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
            treeFormationTemp.DataContext = null;
            treePlaybookTemp.DataContext = null;

            formationTempRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);
            playbookTempRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);

            treeFormationTemp.DataContext = formationTempRootViewModel;
            treePlaybookTemp.DataContext = playbookTempRootViewModel;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            this.Close();
        }

        private void treeFormation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && (this.treeFormation.IsKeyboardFocusWithin || this.treePlaybook.IsKeyboardFocusWithin))
            {
                this.btnExport_Click(null, null);
            }
        }

        private void VisibleProgress(Visibility visible)
        {
            spProgress.Visibility = visible;
        }

        private void btnExportAll_Click(object sender, RoutedEventArgs e)
        {
            VisibleProgress(Visibility.Visible);

            int count = 0;
            CopyDirectory.GetFileNum(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, ref count);
            pbProgress.Minimum = 0;
            pbProgress.Maximum = count;
            pbProgress.Value = 0;

            ParameterizedThreadStart pts = new ParameterizedThreadStart(CopyThread);

            Thread th = new Thread(pts);

            th.Start(this);
            
            //ZipClass.UnZip(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, PackageFile, chkOverwrite.IsChecked.Value);
        }

        private void CopyThread(object obj)
        {
            SelectPlayForExportWindow win = obj as SelectPlayForExportWindow;

            if (win != null)
            {
                CopyDirectory.CopyFolderFiles(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, TempUserFolder, false, ExportAllProgress);

                Dispatcher.BeginInvoke(new Action<Visibility>(VisibleProgress), DispatcherPriority.Send, Visibility.Collapsed);

                Dispatcher.BeginInvoke(new Action(RefreshCurrentTree), DispatcherPriority.Send);
            }
        }

        private void ExportAllProgress(string strFile)
        {
            Dispatcher.BeginInvoke(new Action<string>(UpdateProgress), DispatcherPriority.Send, strFile);
        }

        private void UpdateProgress(string strFile)
        {
            FileInfo fi = new FileInfo(strFile);

            tbProgress.Text = string.Format("Export: {0}",fi.Name);

            pbProgress.Value++;
        }
        
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                ViewModel.TreeViewItemViewModel fTempVM = this.treeFormationTemp.SelectedItem as ViewModel.TreeViewItemViewModel;
                ViewModel.TreeViewItemViewModel fVM = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;

                string strFolder = string.Empty;

                if (fTempVM is ViewModel.PlayViewModel)
                {
                    fTempVM = fTempVM.Parent.Parent;
                }
                if (fTempVM is ViewModel.FormationViewModel)
                {
                    fTempVM = fTempVM.Parent;
                }
                if (fTempVM is ViewModel.ScoutTypeViewModel)
                {
                    strFolder = (fTempVM as ViewModel.ScoutTypeViewModel).ScoutTypePath;
                }
                if (fTempVM is ViewModel.FolderViewModel)
                {
                    strFolder = (fTempVM as ViewModel.FolderViewModel).FolderPath;
                }

                if (System.IO.Directory.Exists(strFolder))
                {
                    if (fVM is ViewModel.FormationViewModel)
                    {
                        ViewModel.FormationViewModel formationVM = fVM as ViewModel.FormationViewModel;

                        formationVM.CopyTo(strFolder,chkOverwrite.IsChecked.Value);
                    }
                    else if (fVM is ViewModel.FolderViewModel)
                    {
                        ViewModel.FolderViewModel folderVM = fVM as ViewModel.FolderViewModel;

                        CopyDirectory.CopyFolderFiles(folderVM.FolderPath, strFolder + @"\" + folderVM.FolderName, chkOverwrite.IsChecked.Value,null);
                    }
                    else if (fVM is ViewModel.TitleViewModel)
                    { // 10-27-2011 Scott
                        ViewModel.TitleViewModel titleViewModel = fVM as ViewModel.TitleViewModel;

                        titleViewModel.CopyTo(strFolder, chkOverwrite.IsChecked.Value);
                    }
                    else
                    {
                        MessageBox.Show("Please select a formation or folder to export");

                        return;
                    }

                    fTempVM.Refresh();
                    fTempVM.IsExpanded = true;
                }
                else
                {
                    MessageBox.Show("Please select a folder to export");
                }
            }

            if (tabControl.SelectedIndex == 1)
            {
                ViewModel.TreeViewItemViewModel pTempVM = this.treePlaybookTemp.SelectedItem as ViewModel.TreeViewItemViewModel;
                ViewModel.TreeViewItemViewModel pVM = this.treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;

                string strFolder = string.Empty;

                if (pTempVM is ViewModel.PlayViewModel)
                {
                    pTempVM = pTempVM.Parent;
                }
                if (pTempVM is ViewModel.PlayTypeViewModel)
                {
                    strFolder = (pTempVM as ViewModel.PlayTypeViewModel).PlayTypePath;
                }
                if (pTempVM is ViewModel.PlayFolderViewModel)
                {
                    strFolder = (pTempVM as ViewModel.PlayFolderViewModel).FolderPath;
                }

                if (System.IO.Directory.Exists(strFolder))
                {
                    if (pVM is ViewModel.PlayViewModel)
                    {
                        ViewModel.PlayViewModel playVM = pVM as ViewModel.PlayViewModel;

                        playVM.CopyTo(strFolder, chkOverwrite.IsChecked.Value);
                    }
                    else if (pVM is ViewModel.PlayFolderViewModel)
                    {
                        ViewModel.PlayFolderViewModel folderVM = pVM as ViewModel.PlayFolderViewModel;

                        CopyDirectory.CopyFolderFiles(folderVM.FolderPath, strFolder + @"\" + folderVM.FolderName, chkOverwrite.IsChecked.Value,null);
                    }
                    else
                    {
                        MessageBox.Show("Please select a play or folder to export");

                        return;
                    }

                    pTempVM.Refresh();
                    pTempVM.IsExpanded = true;
                }
                else
                {
                    MessageBox.Show("Please select a destination folder to export");
                }
            }
        }
    }
}