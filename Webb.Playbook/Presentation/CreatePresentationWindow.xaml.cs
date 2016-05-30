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

namespace Webb.Playbook.Presentation
{
    /// <summary>
    /// CreatePresentationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatePresentationWindow : Window
    {
        private ViewModel.FormationRootViewModel fRootViewModel;
        private ViewModel.PlaybookRootViewModel pRootViewModel;

        public CreatePresentationWindow(ViewModel.FormationRootViewModel formationRootViewModel, ViewModel.PlaybookRootViewModel playbookRootViewModel)
        {
            InitializeComponent();

            fRootViewModel = formationRootViewModel;
            pRootViewModel = playbookRootViewModel;

            LoadPresentations();

            ClearDump();
        }

        private void ClearDump()
        {
            if (System.IO.Directory.Exists(Webb.Playbook.Data.ProductInfo.PresentationPath))
            {
                foreach (string strDir in System.IO.Directory.GetDirectories(Webb.Playbook.Data.ProductInfo.PresentationPath))
                {
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(strDir);

                    if (dir.GetFiles().Count() == 0)
                    {
                        System.IO.Directory.Delete(strDir, true);
                    }
                }
            }
        }

        private void LoadPresentations()
        {
            lbPresentations.Items.Clear();

            string dir = AppDomain.CurrentDomain.BaseDirectory + @"Presentations";

            if (System.IO.Directory.Exists(dir))
            {
                foreach (string strPresentation in System.IO.Directory.GetFiles(dir, "*.Pres"))
                {
                    StackPanel sp = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Tag = strPresentation,
                    };

                    string strImagePath = AppDomain.CurrentDomain.BaseDirectory + @"\Resource\Presentation.png";
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(strImagePath, UriKind.RelativeOrAbsolute)),
                        Width = 20,
                        Height = 20,
                    };

                    TextBlock lbi = new TextBlock()
                    {
                        Text = System.IO.Path.GetFileNameWithoutExtension(strPresentation),
                        FontSize = 15,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0),
                    };

                    sp.Children.Add(image);
                    sp.Children.Add(lbi);

                    this.lbPresentations.Items.Add(sp);
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(dir);
            }
        }

        private void btnCreatePresentation_Click(object sender, RoutedEventArgs e)
        {
            SelectPlayWindow spw = new SelectPlayWindow(fRootViewModel,pRootViewModel)
            {
                Owner = this,
            };

            if (spw.ShowDialog().Value && spw.Presentation != null)
            {
                Webb.Playbook.Data.Presentation presentation = spw.Presentation;

                presentation.Save(spw.PresentationPath);

                LoadPresentations();
            }
        }

        private void btnOpenPresentation_Click(object sender, RoutedEventArgs e)
        {
            if (lbPresentations.SelectedItem != null)
            {
                Webb.Playbook.Data.Presentation presentation = new Webb.Playbook.Data.Presentation();

                presentation.Load((this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString());

                Presentation.PresentationWindow pw = new PresentationWindow(presentation);

                pw.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a presentation");
            }
        }

        private void btnEditPresentation_Click(object sender, RoutedEventArgs e)
        {
            if (lbPresentations.SelectedItem != null)
            {
                Webb.Playbook.Data.Presentation presentation = new Webb.Playbook.Data.Presentation();

                presentation.Load((this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString());

                SelectPlayWindow spw = new SelectPlayWindow(fRootViewModel, pRootViewModel, presentation)
                {
                    Owner = this,
                };

                if (spw.ShowDialog().Value && spw.Presentation != null)
                {
                    presentation = spw.Presentation;

                    presentation.Save(spw.PresentationPath);

                    LoadPresentations();
                }
            }
            else
            {
                MessageBox.Show("Please select a presentation");
            }
        }

        private void btnDeletePresentation_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbPresentations.SelectedItem != null)
            {
                if (MessageBox.Show("Do you want to delete this presentation ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (System.IO.File.Exists((this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString()))
                    {
                        System.IO.File.Delete((this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString());
                    }

                    this.lbPresentations.Items.Remove(this.lbPresentations.SelectedItem);
                }
            }
            else
            {
                MessageBox.Show("Please select a presentation");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbPresentations_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.lbPresentations.SelectedItem != null)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem miSaveAs = new MenuItem()
                {
                    Header = "Save As",
                };
                miSaveAs.Click += new RoutedEventHandler(miSaveAs_Click);
                menu.Items.Add(miSaveAs);

                MenuItem miRename = new MenuItem()
                {
                    Header = "Rename",
                };
                miRename.Click += new RoutedEventHandler(miRename_Click);
                menu.Items.Add(miRename);

                MenuItem miDelete = new MenuItem()
                {
                    Header = "Delete",
                };
                miDelete.Click += new RoutedEventHandler(miDelete_Click);
                menu.Items.Add(miDelete);

                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                menu.IsOpen = true;
            }
        }

        void miDelete_Click(object sender, RoutedEventArgs e)
        {
            this.btnDeletePresentation_Click(null, null);
        }

        void miRename_Click(object sender, RoutedEventArgs e)
        {
            if(this.lbPresentations.SelectedItem != null)
            {
                string strFile = (this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString();

                string strName = System.IO.Path.GetFileNameWithoutExtension(strFile);

                NameWindow nw = new NameWindow()
                {
                    FileName = strName,
                    Title = "Presentation Save As",
                    Owner = this,
                };

                if (nw.ShowDialog().Value)
                {
                    string strNewFile = Data.ProductInfo.PresentationPath + nw.FileName + @".Pres";

                    if (System.IO.File.Exists(strNewFile))
                    {
                        if (MessageBox.Show("This presentation already exists, do you want to overwrite it ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    //
                    System.IO.File.Copy(strFile, strNewFile, true);
                    System.IO.File.Delete(strFile);
                    LoadPresentations();
                }
            }
        }

        void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            string strFile = (this.lbPresentations.SelectedItem as FrameworkElement).Tag.ToString();

            string strName = System.IO.Path.GetFileNameWithoutExtension(strFile);

            NameWindow nw = new NameWindow()
            {
                FileName = strName,
                Title = "Presentation Save As",
                Owner = this,
            };

            if (nw.ShowDialog().Value)
            {
                string strNewFile = Data.ProductInfo.PresentationPath + nw.FileName + @".Pres";

                if (System.IO.File.Exists(strNewFile))
                {
                    if (MessageBox.Show("This presentation already exists, do you want to overwrite it ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                //
                System.IO.File.Copy(strFile, strNewFile, true);
                LoadPresentations();
            }
        }
    }
}
