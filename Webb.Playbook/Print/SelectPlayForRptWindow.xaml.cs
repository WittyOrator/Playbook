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
using System.Diagnostics;

namespace Webb.Playbook.Print
{
    /// <summary>
    /// SelectPlayForRptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlayForRptWindow : Window
    {
        private string reportHeader = string.Empty;

        private int setup = 0;
        private int Setup
        {
            set
            {
                setup = value;

                UpdateSetup();
            }

            get
            {
                return setup;
            }
        }

        private void UpdateSetup()
        {
            switch (Setup)
            {
                case 0:
                    tabGames.Visibility = Visibility.Visible;
                    tabSelectedGames.Visibility = Visibility.Visible;
                    tabReports.Visibility = Visibility.Hidden;
                    //tabSelectedReports.Visibility = Visibility.Hidden;
                    btnSelectReportTemplate.Visibility = Visibility.Visible;
                    btnPrintReport.Visibility = Visibility.Collapsed;
                    btnReportHeader.Visibility = Visibility.Collapsed;
                    btnSelectGame.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    tabGames.Visibility = Visibility.Hidden;
                    tabSelectedGames.Visibility = Visibility.Hidden;
                    tabReports.Visibility = Visibility.Visible;
                    //tabSelectedReports.Visibility = Visibility.Visible;
                    btnSelectReportTemplate.Visibility = Visibility.Collapsed;
                    btnPrintReport.Visibility = Visibility.Visible;
                    btnReportHeader.Visibility = Visibility.Visible;
                    btnSelectGame.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void InitReportTemplates()
        {
            this.lbReportTemplate.Items.Clear();

            string strFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\";

            if(System.IO.Directory.Exists(strFolder))
            {
                string[] arrFiles = System.IO.Directory.GetFiles(strFolder, "*.repx");

                Array.Sort(arrFiles, new Webb.Playbook.Data.FileNameComparer());    // 09-29-2011 Scott

                foreach (string strFile in arrFiles)
                {
                    StackPanel sp = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Tag = strFile,
                    };

                    string strImagePath = AppDomain.CurrentDomain.BaseDirectory + @"\Resource\Report.ico";
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(strImagePath, UriKind.RelativeOrAbsolute)),
                        Width  = 20,
                        Height = 20,
                    };

                    TextBlock lbi = new TextBlock()
                    {
                        Text = System.IO.Path.GetFileNameWithoutExtension(strFile),
                        FontSize = 15,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10,0,0,0),
                    };

                    sp.Children.Add(image);
                    sp.Children.Add(lbi);

                    this.lbReportTemplate.Items.Add(sp);
                }
            }
        }

        public SelectPlayForRptWindow(ViewModel.FormationRootViewModel formationRootViewModel, ViewModel.PlaybookRootViewModel playbookRootViewModel)
        {
            InitializeComponent();

            treeFormation.DataContext = formationRootViewModel;

            treePlaybook.DataContext = playbookRootViewModel;

            Setup = 0;

            InitReportTemplates();
        }

        private List<string> GetFiles()
        {
            List<string> arrFiles = new List<string>();

            foreach (object obj in this.lbSelectedFiles.Items)
            {
                if (obj is ViewModel.FormationViewModel)
                {
                    arrFiles.Add((obj as ViewModel.FormationViewModel).FormationPath);
                }
                else if (obj is ViewModel.PlayViewModel)
                {
                    arrFiles.Add((obj as ViewModel.PlayViewModel).PlayPath);
                }
            }

            return arrFiles;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void treeFormation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;

            ViewModel.TreeViewItemViewModel tivm = sp.DataContext as ViewModel.TreeViewItemViewModel;

            if (tivm is ViewModel.FormationViewModel)
            {
                if (!lbSelectedFiles.Items.Contains(tivm))
                {
                    lbSelectedFiles.Items.Add(tivm);
                }
            }
            else if (tivm is ViewModel.PlayViewModel)
            {
                if (!lbSelectedFiles.Items.Contains(tivm))
                {
                    lbSelectedFiles.Items.Add(tivm);
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSelectedFiles.SelectedItem != null)
            {
                int index = this.lbSelectedFiles.SelectedIndex;

                this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                if (index < this.lbSelectedFiles.Items.Count)
                {
                    this.lbSelectedFiles.SelectedIndex = index;
                }
                else
                {
                    this.lbSelectedFiles.SelectedIndex = index - 1;
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.lbSelectedFiles.Items.Clear();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSelectedFiles.SelectedItem != null)
            {
                int index = this.lbSelectedFiles.SelectedIndex;

                object o = this.lbSelectedFiles.SelectedItem;

                if (index > 0)
                {
                    this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                    this.lbSelectedFiles.Items.Insert(index - 1, o);

                    this.lbSelectedFiles.SelectedIndex = index - 1;
                }
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbSelectedFiles.SelectedItem != null)
            {
                int index = this.lbSelectedFiles.SelectedIndex;

                object o = this.lbSelectedFiles.SelectedItem;

                if (index < this.lbSelectedFiles.Items.Count - 1)
                {
                    this.lbSelectedFiles.Items.Remove(this.lbSelectedFiles.SelectedItem);

                    this.lbSelectedFiles.Items.Insert(index + 1, o);

                    this.lbSelectedFiles.SelectedIndex = index + 1;
                }
            }
        }

        private void btnPicSettings_Click(object sender, RoutedEventArgs e)
        {
            // 07-26-2011 Scott
            ImageSetupWindow isw = new ImageSetupWindow()
            {
                Owner = this,
            };

            if (isw.ShowDialog().Value)
            {
                ConvertImages();
            }
        }

        // 08-05-2011 Scott
        private void ConvertImages()
        {
            pBar.Visibility = Visibility.Visible;
            pText.Visibility = Visibility.Visible;
            pBar.Maximum = this.lbSelectedFiles.Items.Count;
            pBar.Value = 0;

            foreach (object o in this.lbSelectedFiles.Items)
            {
                if (o is ViewModel.PlayViewModel)
                {
                    SaveImages((o as ViewModel.PlayViewModel).PlayPath);
                }

                if (o is ViewModel.FormationViewModel)
                {
                    SaveImages((o as ViewModel.FormationViewModel).FormationPath);
                }

                pBar.Value++;
            }

            pBar.Visibility = Visibility.Hidden;
            pText.Visibility = Visibility.Hidden;
        }

        private void SaveImages(string strPath)
        {
            // make path
            string strName = System.IO.Path.GetFileNameWithoutExtension(strPath);
            string strScoutType = string.Empty;
            bool bSubFiled = strPath.Contains("@");
            string strFieldName = string.Empty;
            if (strPath.ToLower().Contains(@"formation\offensive"))
            {
                strScoutType = "Offense";
                strFieldName = bSubFiled ? Webb.Playbook.Data.GameSetting.Instance.OffensiveSubField : Webb.Playbook.Data.GameSetting.Instance.OffensiveMainField;
            }

            if (strPath.ToLower().Contains(@"formation\defensive"))
            {
                strScoutType = "Defense";
                strFieldName = bSubFiled ? Webb.Playbook.Data.GameSetting.Instance.DefensiveSubField : Webb.Playbook.Data.GameSetting.Instance.DefensiveMainField;
            }

            string strBmpDir = AppDomain.CurrentDomain.BaseDirectory + @"\Bitmaps\" + strScoutType + @"\" + strFieldName + @"\";
            if (strPath.ToLower().Contains(@"offensive\playbook") || strPath.ToLower().Contains(@"defensive\playbook"))
            {
                strBmpDir = AppDomain.CurrentDomain.BaseDirectory + @"\Bitmaps\Plays\";
            }
            if (!System.IO.Directory.Exists(strBmpDir))
            {
                System.IO.Directory.CreateDirectory(strBmpDir);
            }
            string strBmpPath = strBmpDir + strName + ".BMP";
            string strBmpPathCropped = strBmpDir + strName + "001.BMP";

            // create drawing
            Size size = Size.Empty;
            DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(strPath, true, new Size(1200, 3000), out size, Webb.Playbook.Data.GameSetting.Instance.ImageShowPlayground, true);

            if (size != Size.Empty)
            {
                dbw.Show();

                RenderTargetBitmap bmp = new RenderTargetBitmap((int)(size.Width), (int)(size.Height), 96, 96, PixelFormats.Default);
                DrawingVisual drawingvisual = new DrawingVisual();

                using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
                {
                    VisualBrush visualbrush = new VisualBrush(dbw);

                    drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, size.Width, size.Height));
                }

                bmp.Render(drawingvisual);

                SaveRTBAsBMP(bmp, strBmpPath);

                // 08-18-2011 Scott
                string strPlaybookBmpPath = strPath + ".BMP";
                SaveRTBAsBMP(bmp, strPlaybookBmpPath);

                dbw.Close();
            }
        }

        private void SaveRTBAsBMP(RenderTargetBitmap bmp, string filename)
        {
            JpegBitmapEncoder enc = new JpegBitmapEncoder();
            //BmpBitmapEncoder enc = new BmpBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        private void btnSelectReportTemplate_Click(object sender, RoutedEventArgs e)
        {
            Setup = 1;
        }

        private void btnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            Setup = 0;
        }

        private void btnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (lbReportTemplate.SelectedItem != null)
            {
                Data.InteractiveReport ir = new Webb.Playbook.Data.InteractiveReport(GetFiles());

                string strInw = ir.CreateInw((lbReportTemplate.SelectedItem as FrameworkElement).Tag.ToString(), reportHeader);

                if (System.IO.File.Exists(strInw))
                {
                    string strReportPath = ir.GetReportPath();

                    if (System.IO.File.Exists(strReportPath))
                    {
                        Process process = new Process();
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.Arguments = "\"" + strInw + "\"";

                        psi.FileName = strReportPath;
                        process.StartInfo = psi;
                        process.Start();
                    }
                }
            }
        }

        private void lbReportTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reportHeader = string.Empty;
        }

        //private void btnRemoveReportTemplate_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.lbSelectedReportTemplate.SelectedItem != null)
        //    {
        //        int index = this.lbSelectedReportTemplate.SelectedIndex;

        //        this.lbSelectedReportTemplate.Items.Remove(this.lbSelectedReportTemplate.SelectedItem);

        //        if (index < this.lbSelectedReportTemplate.Items.Count)
        //        {
        //            this.lbSelectedReportTemplate.SelectedIndex = index;
        //        }
        //        else
        //        {
        //            this.lbSelectedReportTemplate.SelectedIndex = index - 1;
        //        }
        //    }
        //}

        //private void btnClearReportTemplate_Click(object sender, RoutedEventArgs e)
        //{
        //    this.lbSelectedReportTemplate.Items.Clear();
        //}

        private void lbReportTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnImportReportTemplate_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Report Template Files (*.repx)|*.repx",
                AddExtension = true,
                Multiselect = true,
            };

            if (fileDialog.ShowDialog().Value)
            {
                foreach (string strFile in fileDialog.FileNames)
                {
                    string strFileName = System.IO.Path.GetFileName(strFile);
                    string strReportPath = AppDomain.CurrentDomain.BaseDirectory + @"Reports\";
                    string strNewPath = strReportPath + strFileName;
                    
                    if(!System.IO.Directory.Exists(strReportPath))
                    {
                        System.IO.Directory.CreateDirectory(strReportPath);
                    }

                    if (System.IO.File.Exists(strNewPath))
                    {
                        if (MessageBox.Show(string.Format("{0} already exists , do you want to overwrite it ?", strFileName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            System.IO.File.Copy(strFile, strReportPath + strFileName,true);
                        }
                    }
                    else
                    {
                        System.IO.File.Copy(strFile, strReportPath + strFileName, false);
                    }
                }

                InitReportTemplates();
            }
        }

        private void btnDeleteReportTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbReportTemplate.SelectedItem != null)
            {
                int oldIndex = this.lbReportTemplate.SelectedIndex;

                if (oldIndex == this.lbReportTemplate.Items.Count - 1)
                {
                    oldIndex--;
                }

                string strReport = (this.lbReportTemplate.SelectedItem as FrameworkElement).Tag.ToString();

                if (System.IO.File.Exists(strReport))
                {
                    if (MessageBox.Show(string.Format("Do you want to Delete Report: {0}", System.IO.Path.GetFileNameWithoutExtension(strReport)), "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        System.IO.File.Delete(strReport);

                        this.lbReportTemplate.Items.Remove(this.lbReportTemplate.SelectedItem);

                        this.lbReportTemplate.SelectedIndex = oldIndex;
                    }
                }
            }
        }

        private void btnReportHeader_Click(object sender, RoutedEventArgs e)
        {
            NameWindow nameWindow = new NameWindow()
            {
                Title = "Report Header",
                Owner = this,
            };

            if (nameWindow.ShowDialog().Value)
            {
                reportHeader = nameWindow.FileName;
            }
        }
    }
}
