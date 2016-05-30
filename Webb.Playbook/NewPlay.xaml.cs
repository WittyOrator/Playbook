using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

using Draw = Webb.Playbook.Geometry.Drawing;

namespace Webb.Playbook
{
    /// <summary>
    /// NewPlay.xaml 的交互逻辑
    /// </summary>
    public partial class NewPlay : Window
    {
        Regex regex = new Regex(@"^\w[\w|\s|-]{0,50}\w{0,}$");  // 07-19-2010 Scott

        private ViewModel.FormationRootViewModel formationRootViewModel;

        private NewPlayViewModel playViewModel = new NewPlayViewModel();

        public NewPlayViewModel PlayViewModel
        {
            get { return playViewModel; }
        }

        private Draw drawing;
        public Draw Drawing
        {
            get { return drawing; }
            protected set { drawing = value; }
        }
        public NewPlay(ViewModel.FormationRootViewModel formationRootViewModel)
        {
            InitializeComponent();

            treeFormation.MouseDoubleClick += new MouseButtonEventHandler(treeFormation_MouseDoubleClick);

            this.formationRootViewModel = formationRootViewModel;

            grid.DataContext = playViewModel;

            treeFormation.DataContext = this.formationRootViewModel;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (textPrompt.Visibility == Visibility.Visible || this.tbPlayName.Text == string.Empty)
            {
                return;
            }

            if (playViewModel.ObjFormation == null)
            {
                //MessageBox.Show("Please select offensive team");
                //return;
            }
            if (playViewModel.OppFormation == null)
            {
                //MessageBox.Show("Please select defensive team");
                //return;
            }
            if (playViewModel.ObjFormation == null && playViewModel.OppFormation == null)
            {
                MessageBox.Show("Please select at least one team");
                return;
            }

            // 09-13-2010 Scott
            //playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
            //playViewModel.PlayName = playViewModel.PlayName.Trim();
            this.DialogResult = true;

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnObject_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FormationViewModel formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;
            if (formationViewModel != null)
            {
                ViewModel.ScoutTypeViewModel scoutTypeViewModel = formationViewModel.Parent.Parent as ViewModel.ScoutTypeViewModel;
                if (scoutTypeViewModel != null)
                {
                    if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Offensive)
                    {
                        playViewModel.ObjFormation = formationViewModel;
                    }
                }
            }
        }

        private void btnOpponent_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FormationViewModel formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;
            if (formationViewModel != null)
            {
                ViewModel.ScoutTypeViewModel scoutTypeViewModel = formationViewModel.Parent.Parent as ViewModel.ScoutTypeViewModel;
                if (scoutTypeViewModel != null)
                {
                    if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Defensive)
                    {
                        playViewModel.OppFormation = formationViewModel;
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Drawing != null)
            {
                Drawing.Dispose();
            }
        }

        private void treeFormation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;

            ViewModel.ScoutTypeViewModel scoutTypeViewModel;
            ViewModel.ScoutTypeViewModel oppScoutTypeViewModel;
            ViewModel.TreeViewItemViewModel tivm = sp.DataContext as ViewModel.TreeViewItemViewModel;

            if (tivm != null)
            {
                while (true)
                {
                    if (tivm is ViewModel.ScoutTypeViewModel)
                    {
                        break;
                    }

                    tivm = tivm.Parent;
                }

                scoutTypeViewModel = tivm as ViewModel.ScoutTypeViewModel;

                // 09-19-2010 Scott
                ViewModel.PlayViewModel fPlayViewModel = sp.DataContext as ViewModel.PlayViewModel;

                ViewModel.FormationViewModel formationViewModel = null;

                if (fPlayViewModel != null)
                {
                    if (scoutTypeViewModel != null)
                    {
                        Data.Formation formation = new Webb.Playbook.Data.Formation(fPlayViewModel.PlayPath);

                        formationViewModel = new Webb.Playbook.ViewModel.FormationViewModel(formation, null);

                        if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Offensive || scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Kicks/*01-19-2012 Scott*/)
                        {
                            playViewModel.ObjFormation = formationViewModel;
                        }

                        if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Defensive)
                        {
                            playViewModel.OppFormation = formationViewModel;
                        }

                        playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
                        playViewModel.PlayName = playViewModel.PlayName.Trim();
                        ViewModel.FormationViewModel offensiveFormation = PlayViewModel.ObjFormation;
                        ViewModel.FormationViewModel defensiveFormation = PlayViewModel.OppFormation;

                        if (Drawing != null)
                        {
                            Drawing.Dispose();
                        }

                        Drawing = Draw.Load(offensiveFormation == null ? string.Empty : offensiveFormation.FormationPath, defensiveFormation == null ? string.Empty : defensiveFormation.FormationPath, (this.Owner as MainWindow).canvasDrawing);
                    }
                }

                formationViewModel = sp.DataContext as ViewModel.FormationViewModel;
                if (formationViewModel != null)
                {
                    if (scoutTypeViewModel != null)
                    {
                        if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Offensive || scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Kicks/*01-19-2012 Scott*/)
                        {
                            playViewModel.ObjFormation = formationViewModel;
                        }

                        if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Defensive)
                        {
                            playViewModel.OppFormation = formationViewModel;
                        }

                        playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
                        playViewModel.PlayName = playViewModel.PlayName.Trim();
                        ViewModel.FormationViewModel offensiveFormation = PlayViewModel.ObjFormation;
                        ViewModel.FormationViewModel defensiveFormation = PlayViewModel.OppFormation;

                        if (Drawing != null)
                        {
                            Drawing.Dispose();
                        }

                        Drawing = Draw.Load(offensiveFormation == null ? string.Empty : offensiveFormation.FormationPath, defensiveFormation == null ? string.Empty : defensiveFormation.FormationPath, (this.Owner as MainWindow).canvasDrawing);
                    }

                }

                // 09-13-2010 Scott
                playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
                playViewModel.PlayName = playViewModel.PlayName.Trim();
            }
        }

        private void treeFormation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TreeView formationTree = sender as TreeView;

            //if (formationTree == this.treeFormation)
            //{
            //    ViewModel.FormationViewModel formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;
            //    if (formationViewModel != null)
            //    {
            //        ViewModel.ScoutTypeViewModel scoutTypeViewModel;
            //        ViewModel.ScoutTypeViewModel otherscoutTypeViewModel;
            //        ViewModel.TreeViewItemViewModel tivm = formationViewModel;

            //        while (true)
            //        {
            //            tivm = tivm.Parent;

            //            if (tivm is ViewModel.ScoutTypeViewModel)
            //            {
            //                break;
            //            }
            //        }

            //        scoutTypeViewModel = tivm as ViewModel.ScoutTypeViewModel;
            //        if (scoutTypeViewModel != null)
            //        {
            //            if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Offensive)
            //            {
            //                otherscoutTypeViewModel = treeFormation.Items.OfType<ViewModel.ScoutTypeViewModel>().ElementAt(1);
            //                playViewModel.ObjFormation = formationViewModel;
            //                otherscoutTypeViewModel.IsSelected = true;

            //            }
            //            if (scoutTypeViewModel.ScoutType.ScoutType == Data.ScoutTypes.Defensive)
            //            {
            //                otherscoutTypeViewModel = treeFormation.Items.OfType<ViewModel.ScoutTypeViewModel>().ElementAt(0);
            //                playViewModel.OppFormation = formationViewModel;
            //                otherscoutTypeViewModel.IsSelected = true;
            //            }
            //            playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
            //            playViewModel.PlayName = playViewModel.PlayName.Trim(); 
            //            ViewModel.FormationViewModel offensiveFormation = PlayViewModel.ObjFormation;
            //            ViewModel.FormationViewModel defensiveFormation = PlayViewModel.OppFormation;

            //            if (Drawing != null)
            //            {
            //                Drawing.Dispose();
            //            }

            //            Drawing = Draw.Load(offensiveFormation == null ? string.Empty : offensiveFormation.FormationPath, defensiveFormation == null ? string.Empty : defensiveFormation.FormationPath, (this.Owner as MainWindow).canvasDrawing);
                      
            //        }

            //    }

            //    // 09-13-2010 Scott
            //    playViewModel.PlayName = txtObj.Text.ToString() + " vs " + txtOpp.Text.ToString();
            //    playViewModel.PlayName = playViewModel.PlayName.Trim();
            //}
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (regex.Match(tbPlayName.Text).Success)
            {
                textPrompt.Visibility = Visibility.Hidden;
            }
            else
            {
                textPrompt.Visibility = Visibility.Visible;
            }
        }

        private void btnRevOff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRevDef_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class NewPlayViewModel : Webb.Playbook.Data.NotifyObj
    {
        private string playName = @"New Play";
        public string PlayName
        {
            get { return playName; }
            set 
            { 
                playName = value;

                OnPropertyChanged("PlayName");  // 09-13-2010 Scott
            }
        }

        private ViewModel.FormationViewModel objFormation;
        public ViewModel.FormationViewModel ObjFormation
        {
            get { return objFormation; }
            set
            {
                if (objFormation != value)
                {
                    objFormation = value;

                    OnPropertyChanged("ObjFormation");
                }
            }
        }

        private ViewModel.FormationViewModel oppFormation;
        public ViewModel.FormationViewModel OppFormation
        {
            get { return oppFormation; }
            set
            {
                if (oppFormation != value)
                {
                    oppFormation = value;

                    OnPropertyChanged("OppFormation");
                }
            }
        }
    }
}
