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
using Webb.Playbook.ViewModel;

namespace Webb.Playbook.Presentation
{
    /// <summary>
    /// SelectPlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlayWindow : Window
    {
        public string PresentationPath
        {
            get
            {
                if (Presentation != null)
                {
                    return Data.ProductInfo.PresentationPath + Presentation.Name + ".Pres";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Webb.Playbook.Data.Presentation Presentation = null;

        public SelectPlayWindow(ViewModel.FormationRootViewModel formationRootViewModel, ViewModel.PlaybookRootViewModel playbookRootViewModel)
        {
            InitializeComponent();

            treeFormation.DataContext = formationRootViewModel;

            treePlaybook.DataContext = playbookRootViewModel;
        }

        public SelectPlayWindow(ViewModel.FormationRootViewModel formationRootViewModel, ViewModel.PlaybookRootViewModel playbookRootViewModel, Webb.Playbook.Data.Presentation presentation)
        {
            InitializeComponent();

            treeFormation.DataContext = formationRootViewModel;

            treePlaybook.DataContext = playbookRootViewModel;

            // set presentation
            Presentation = presentation;

            SetPresentation();
        }

        public void SetPresentation()
        {
            lbSelectedFiles.Items.Clear();

            foreach (PresentationPlay pPlay in Presentation.Plays)
            {
                string strStandardFilePath = new System.IO.FileInfo(pPlay.PlayPath).FullName;

                if (strStandardFilePath.EndsWith(".Ttl", true, null))
                {
                    Title title = new Title(strStandardFilePath);

                    TitleViewModel tvm = new TitleViewModel(title, null);

                    lbSelectedFiles.Items.Add(tvm);
                }
                else
                {

                    if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Offensive")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Offensive")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Offensive"))
                    {
                        Formation formation = new Formation(strStandardFilePath);

                        FormationViewModel fvm = new FormationViewModel(formation, null);

                        lbSelectedFiles.Items.Add(fvm);
                    }

                    if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Defensive")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Defensive")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Defensive"))
                    {
                        Formation formation = new Formation(strStandardFilePath);

                        FormationViewModel fvm = new FormationViewModel(formation, null);

                        lbSelectedFiles.Items.Add(fvm);
                    }

                    if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Kicks")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Kicks")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Kicks"))
                    {
                        Formation formation = new Formation(strStandardFilePath);

                        FormationViewModel fvm = new FormationViewModel(formation, null);

                        lbSelectedFiles.Items.Add(fvm);
                    }

                    if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Playbook")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Playbook")
                        || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Playbook"))
                    {
                        Play play = new Play(strStandardFilePath);

                        PlayViewModel pvm = new PlayViewModel(play, null);

                        lbSelectedFiles.Items.Add(pvm);
                    }
                }

                foreach (Object objVideo in pPlay.Videos)
                {
                    if (objVideo.ToString().EndsWith(".ppt", true, null))
                    {
                        PowerPoint ppt = new PowerPoint(objVideo.ToString());

                        PPTViewModel pptVM = new PPTViewModel(ppt, null);

                        lbSelectedFiles.Items.Add(pptVM);
                    }
                    else
                    {
                        VideoCoachingPointInfo vcpi = null;

                        if (!(objVideo is VideoCoachingPointInfo))
                        {
                            vcpi = new VideoCoachingPointInfo(objVideo.ToString());
                        }
                        else
                        {
                            vcpi = objVideo as VideoCoachingPointInfo;
                        }

                        VideoViewModel vvm = new VideoViewModel(vcpi, null);

                        this.lbSelectedFiles.Items.Add(vvm);
                    }
                }
            }
        }

        private Webb.Playbook.Data.Presentation GetPresentation()
        {
            Webb.Playbook.Data.Presentation pres = new Webb.Playbook.Data.Presentation();

            if (Presentation != null)
            {
                pres.Name = Presentation.Name;
            }

            PresentationPlay pPrevPay = null;

            foreach (object obj in this.lbSelectedFiles.Items)
            {
                if (obj is ViewModel.FormationViewModel)
                {
                    PresentationPlay pPlay = new PresentationPlay();

                    pPrevPay = pPlay;

                    pPlay.PlayPath = (obj as ViewModel.FormationViewModel).FormationPath;

                    pres.Plays.Add(pPlay);
                }
                else if (obj is ViewModel.PlayViewModel)
                {
                    PresentationPlay pPlay = new PresentationPlay();

                    pPrevPay = pPlay;

                    pPlay.PlayPath = (obj as ViewModel.PlayViewModel).PlayPath;

                    pres.Plays.Add(pPlay);
                }
                else if (obj is ViewModel.TitleViewModel)
                {
                    PresentationPlay pPlay = new PresentationPlay();

                    pPrevPay = pPlay;

                    pPlay.PlayPath = (obj as ViewModel.TitleViewModel).TitlePath;

                    pres.Plays.Add(pPlay);
                }
                else if (obj is ViewModel.PPTViewModel && pPrevPay != null)
                {
                    pPrevPay.Videos.Add((obj as ViewModel.PPTViewModel).PptPath);
                }
                else if (obj is ViewModel.VideoViewModel && pPrevPay != null)
                {
                    pPrevPay.Videos.Add((obj as ViewModel.VideoViewModel).VideoInfo);
                }
            }

            return pres;
        }

        private bool Contains(ViewModel.TreeViewItemViewModel tvivSelect)
        {
            string strSelectPath = string.Empty;

            PlayViewModel pvmSelect = tvivSelect as PlayViewModel;
            FormationViewModel fvmSelect = tvivSelect as FormationViewModel;
            TitleViewModel tvmSelect = tvivSelect as TitleViewModel;
            PPTViewModel pptVMSelect = tvivSelect as PPTViewModel;

            if (pvmSelect != null)
            {
                strSelectPath = pvmSelect.PlayPath;
            }

            if (fvmSelect != null)
            {
                strSelectPath = fvmSelect.FormationPath;
            }

            if (tvmSelect != null)
            {
                strSelectPath = tvmSelect.TitlePath;
            }

            if (pptVMSelect != null)
            {
                strSelectPath = pptVMSelect.PptPath;
            }

            if (strSelectPath != string.Empty)
            {
                foreach (object o in lbSelectedFiles.Items)
                {
                    if (o is PlayViewModel)
                    {
                        PlayViewModel pvm = o as PlayViewModel;

                        if (strSelectPath == pvm.PlayPath)
                        {
                            return true;
                        }
                    }

                    if (o is FormationViewModel)
                    {
                        FormationViewModel fvm = o as FormationViewModel;

                        if (strSelectPath == fvm.FormationPath)
                        {
                            return true;
                        }
                    }

                    if (o is TitleViewModel)
                    {
                        TitleViewModel tvm = o as TitleViewModel;

                        if (strSelectPath == tvm.TitlePath)
                        {
                            return true;
                        }
                    }

                    if (o is PPTViewModel)
                    {
                        PPTViewModel pptVM = o as PPTViewModel;

                        if (strSelectPath == pptVM.PptPath)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void treeFormation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;

            ViewModel.TreeViewItemViewModel tivm = sp.DataContext as ViewModel.TreeViewItemViewModel;

            if (tivm is ViewModel.FormationViewModel)
            {
                if (!Contains(tivm))
                {
                    lbSelectedFiles.Items.Add(tivm);
                }
            }
            else if (tivm is ViewModel.PlayViewModel)
            {
                if (!Contains(tivm))
                {
                    lbSelectedFiles.Items.Add(tivm);
                }
            }
            else if (tivm is ViewModel.TitleViewModel)
            {
                if (!Contains(tivm))
                {
                    lbSelectedFiles.Items.Add(tivm);
                }
            }

            if (e.ClickCount > 1)
            {
                AddChildren(tivm);
            }
        }

        private void AddChildren(ViewModel.TreeViewItemViewModel tvivm)
        {
            if (!(tvivm is ViewModel.FormationViewModel) && !(tvivm is ViewModel.PlayViewModel) && tvivm.Children != null)
            {
                foreach (ViewModel.TreeViewItemViewModel tvChild in tvivm.Children)
                {
                    if (tvChild is ViewModel.FormationViewModel)
                    {
                        if (!Contains(tvChild))
                        {
                            lbSelectedFiles.Items.Add(tvChild);
                        }
                    }
                    else if (tvChild is ViewModel.PlayViewModel)
                    {
                        if (!Contains(tvChild))
                        {
                            lbSelectedFiles.Items.Add(tvChild);
                        }
                    }
                    else if (tvChild is ViewModel.TitleViewModel)
                    {
                        if (!Contains(tvChild))
                        {
                            lbSelectedFiles.Items.Add(tvChild);
                        }
                    }
                    else if (tvChild is ViewModel.PPTViewModel)
                    {
                        if (!Contains(tvChild))
                        {
                            lbSelectedFiles.Items.Add(tvChild);
                        }
                    }
                    else
                    {
                        AddChildren(tvChild);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            bool? ret = IsVideo(this.lbSelectedFiles.SelectedItem);

            if (ret != null)
            {
                if (ret.Value)
                {
                    int index = this.lbSelectedFiles.SelectedIndex;

                    this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                    if (index > 0)
                    {
                        this.lbSelectedFiles.SelectedIndex = index - 1;
                    }
                    else
                    {
                        this.lbSelectedFiles.SelectedIndex = index;
                    }
                }
                else
                {
                    List<object> list = new List<object>();
                    list.Add(this.lbSelectedFiles.SelectedItem);

                    for (int i = this.lbSelectedFiles.SelectedIndex + 1; i < this.lbSelectedFiles.Items.Count; i++)
                    {
                        bool? retNext = IsVideo(this.lbSelectedFiles.Items[i]);

                        if (retNext == ret)
                        {
                            break;
                        }
                        else
                        {
                            list.Add(this.lbSelectedFiles.Items[i]);
                        }
                    }

                    for (int i = this.lbSelectedFiles.SelectedIndex + 1; i < this.lbSelectedFiles.Items.Count; i++)
                    {
                        bool? retNext = IsVideo(this.lbSelectedFiles.Items[i]);

                        if (retNext == ret)
                        {
                            break;
                        }
                        else
                        {
                            list.Add(this.lbSelectedFiles.Items[i]);
                        }
                    }

                    bool bFind = false;
                    int indexPrevious = this.lbSelectedFiles.SelectedIndex - 1;
                    for (int i = indexPrevious; i >= 0; i--)
                    {
                        bool? retPrevious = IsVideo(this.lbSelectedFiles.Items[i]);

                        if (retPrevious == ret)
                        {
                            indexPrevious = i;

                            bFind = true;

                            break;
                        }
                    }

                    foreach (object o in list)
                    {
                        if (this.lbSelectedFiles.Items.Contains(o))
                        {
                            this.lbSelectedFiles.Items.Remove(o);
                        }
                    }

                    if (bFind)
                    {
                        this.lbSelectedFiles.SelectedIndex = indexPrevious;
                    }
                    else
                    {
                        this.lbSelectedFiles.SelectedIndex = 0;
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.lbSelectedFiles.Items.Clear();
        }

        private void btnSavePresentation_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSelectedFiles.Items.Count == 0)
            {
                MessageBox.Show("Please select games");

                return;
            }

            Presentation = GetPresentation();

            NameWindow nw = new NameWindow()
            {
                Title = "Save Presentation",
                Owner = this,
                SelectAll = true,
                FileName = Presentation.Name,
            };

            if (nw.ShowDialog() == true)
            {
                Presentation.Name = nw.FileName;

                if (System.IO.File.Exists(PresentationPath))
                {
                    if (MessageBox.Show("This presentation already exists, do you want to overwrite the existing presentation ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                this.DialogResult = true;

                CloseWindow();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Presentation = null;

            this.DialogResult = false;

            CloseWindow();
        }


        private void CloseWindow()
        {
            this.Close();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            bool? retCurrent = IsVideo(this.lbSelectedFiles.SelectedItem);

            if (retCurrent != null)
            {
                int indexCurrent = this.lbSelectedFiles.SelectedIndex;

                int indexPrevious = indexCurrent - 1;

                List<object> list = new List<object>();

                list.Add(this.lbSelectedFiles.SelectedItem);

                bool bFind = false;

                if (indexCurrent > 0)
                {
                    for (int i = indexPrevious; i >= 0; i--)
                    {
                        object previousO = this.lbSelectedFiles.Items[i];

                        bool? retPrevious = IsVideo(previousO);

                        if (retPrevious == retCurrent)
                        {
                            indexPrevious = i;

                            bFind = true;

                            break;
                        }
                    }

                    for (int i = indexCurrent + 1; i < this.lbSelectedFiles.Items.Count; i++)
                    {
                        object nextO = this.lbSelectedFiles.Items[i];

                        bool? retNext = IsVideo(nextO);

                        if (retNext == retCurrent)
                        {
                            break;
                        }
                        else
                        {
                            list.Add(this.lbSelectedFiles.Items[i]);
                        }
                    }

                    if (bFind)
                    {
                        if (retCurrent.Value)
                        {
                            if (indexCurrent - 1 >= 0)
                            {
                                if (IsVideo(this.lbSelectedFiles.Items[indexCurrent - 1]).Value)
                                {
                                    this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                                    this.lbSelectedFiles.Items.Insert(indexCurrent - 1, list[0]);

                                    this.lbSelectedFiles.SelectedIndex = indexCurrent - 1;
                                }
                            }
                        }
                        else
                        {
                            list.Reverse();

                            foreach (object o in list)
                            {
                                this.lbSelectedFiles.Items.Remove(o);

                                this.lbSelectedFiles.Items.Insert(indexPrevious, o);

                                this.lbSelectedFiles.SelectedIndex = indexPrevious;
                            }
                        }
                    }
                }
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            bool? retCurrent = IsVideo(this.lbSelectedFiles.SelectedItem);

            if (retCurrent != null)
            {
                int indexCurrent = this.lbSelectedFiles.SelectedIndex;

                int indexNext = indexCurrent + 1;

                List<object> list = new List<object>();

                list.Add(this.lbSelectedFiles.SelectedItem);

                bool bFind = false;

                if (indexCurrent < this.lbSelectedFiles.Items.Count - 1)
                {
                    for (int i = indexNext; i < this.lbSelectedFiles.Items.Count; i++)
                    {
                        object nextO = this.lbSelectedFiles.Items[i];

                        bool? retNext = IsVideo(nextO);

                        if (retNext == retCurrent)
                        {
                            indexNext = i;

                            bFind = true;

                            break;
                        }
                        else
                        {
                            list.Add(this.lbSelectedFiles.Items[i]);
                        }
                    }

                    if (bFind)
                    {
                        if (retCurrent.Value)
                        {
                            if (indexCurrent + 1 < this.lbSelectedFiles.Items.Count)
                            {
                                if (IsVideo(this.lbSelectedFiles.Items[indexCurrent + 1]).Value)
                                {
                                    this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                                    this.lbSelectedFiles.Items.Insert(indexCurrent + 1, list[0]);

                                    this.lbSelectedFiles.SelectedIndex = indexCurrent + 1;
                                }
                            }
                        }
                        else
                        {
                            int indexLastVideo = indexNext + 1;
                            for (; indexLastVideo < this.lbSelectedFiles.Items.Count; indexLastVideo++)
                            {
                                object o = this.lbSelectedFiles.Items[indexLastVideo];

                                bool? retLastVideo = IsVideo(o);

                                if (retLastVideo == null || !retLastVideo.Value)
                                {
                                    break;
                                }
                            }
                            indexLastVideo--;

                            foreach (object o in list)
                            {
                                this.lbSelectedFiles.Items.Remove(o);

                                this.lbSelectedFiles.Items.Insert(indexLastVideo, o);

                                this.lbSelectedFiles.SelectedIndex = indexLastVideo - list.Count + 1;
                            }
                        }
                    }
                }
            }
        }

        private void btnPreviewVideo_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSelectedFiles.SelectedItem is ViewModel.VideoViewModel)
            {
                VideoWindow vw = new VideoWindow()
                {
                    Owner = this,
                };

                vw.Show();

                vw.PlayVideo((this.lbSelectedFiles.SelectedItem as ViewModel.VideoViewModel).VideoPath);
            }
        }

        private void btnAddVideo_Click(object sender, RoutedEventArgs e)
        {
            bool? ret = IsVideo(this.lbSelectedFiles.SelectedItem);

            if (ret != null && !ret.Value && !(this.lbSelectedFiles.SelectedItem is ViewModel.PPTViewModel))
            {
                int index = this.lbSelectedFiles.SelectedIndex;

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = Webb.Playbook.Data.Extensions.VideoFileFilter,
                    AddExtension = true,
                    Multiselect = true,
                };

                if (openFileDialog.ShowDialog().Value)
                {
                    foreach (string strVideoFile in openFileDialog.FileNames)
                    {
                        // 11-30-2011 Scott
                        foreach (Object obj in this.lbSelectedFiles.Items)
                        {
                            if (obj is VideoViewModel && strVideoFile == (obj as VideoViewModel).VideoPath)
                            {
                                return;
                            }
                        }
                        // end

                        VideoCoachingPointInfo vcpi = new VideoCoachingPointInfo(strVideoFile);

                        VideoViewModel vvm = new VideoViewModel(vcpi, null);

                        this.lbSelectedFiles.Items.Insert(index + 1, vvm);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a play or formation to add videos");
            }
        }

        private bool? IsVideo(Object item)
        {
            if (item == null)
            {
                return null;
            }

            if (item is ViewModel.PlayViewModel || item is ViewModel.FormationViewModel || item is ViewModel.TitleViewModel)
            {
                return false;
            }

            return true;
        }

        private void btnAddPpt_Click(object sender, RoutedEventArgs e)
        {
            bool? ret = IsVideo(this.lbSelectedFiles.SelectedItem);

            if (ret != null && !ret.Value && !(this.lbSelectedFiles.SelectedItem is ViewModel.PPTViewModel))
            {
                int index = this.lbSelectedFiles.SelectedIndex;

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "PowerPoint Document (*.PPT)|*.ppt",
                    AddExtension = true,
                    Multiselect = true,
                };

                if (openFileDialog.ShowDialog().Value)
                {
                    foreach (string strPptFile in openFileDialog.FileNames)
                    {
                        // 11-30-2011 Scott
                        foreach (Object obj in this.lbSelectedFiles.Items)
                        {
                            if (obj is PPTViewModel && strPptFile == (obj as PPTViewModel).PptPath)
                            {
                                return;
                            }
                        }
                        // end

                        PowerPoint ppt = new PowerPoint(strPptFile);

                        PPTViewModel pptVM = new PPTViewModel(ppt, null);

                        this.lbSelectedFiles.Items.Insert(index + 1, pptVM);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a play or formation to add ppts");
            }
        }
    }
}
