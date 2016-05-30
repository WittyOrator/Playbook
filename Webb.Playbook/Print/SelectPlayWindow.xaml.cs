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

namespace Webb.Playbook.Print
{
    /// <summary>
    /// SelectPlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlayWindow : Window
    {
        public SelectPlayWindow(ViewModel.FormationRootViewModel formationRootViewModel, ViewModel.PlaybookRootViewModel playbookRootViewModel)
        {
            InitializeComponent();

            treeFormation.DataContext = formationRootViewModel;

            treePlaybook.DataContext = playbookRootViewModel;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print.CustomDocumentViewer dv = new Print.CustomDocumentViewer();

            List<string> files = GetFiles();

            int nPageNum = 1;

            if (files.Count() > 0)
            {
                gridProgress.Visibility = Visibility.Visible;
                pBar.Maximum = files.Count();
                pText.Text = string.Format("{0}/{1}", (nPageNum - 1).ToString(), files.Count().ToString());
            }

            foreach (string file in files)
            {
                FixedDocument fixedDoc = new FixedDocument();

                List<string> arrFiles = new List<string>();

                arrFiles.Add(file);

                PrintPreviewWindow.FillDocument(fixedDoc, arrFiles, nPageNum);

                nPageNum++;

                dv.Document = fixedDoc;

                LocalPrinter.ShowWindow();

                dv.Print();

                pBar.Value = nPageNum - 1;
                pText.Text = string.Format("{0}/{1}", (nPageNum - 1).ToString(), files.Count().ToString());
            }

            gridProgress.Visibility = Visibility.Hidden;
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            Print.PrintPreviewWindow ppw = new Webb.Playbook.Print.PrintPreviewWindow();
            ppw.Owner = this;

            List<string> files = GetFiles();

            foreach (string file in files)
            {
                ppw.PrintFiles.Add(file);
            }

            if (files.Count > 0)
            {
                if (ppw.ShowDialog().Value)
                {

                }
            }
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
                        if (!lbSelectedFiles.Items.Contains(tvChild))
                        {
                            lbSelectedFiles.Items.Add(tvChild);
                        }
                    }
                    else if (tvChild is ViewModel.PlayViewModel)
                    {
                        if (!lbSelectedFiles.Items.Contains(tvChild))
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
    }
}
