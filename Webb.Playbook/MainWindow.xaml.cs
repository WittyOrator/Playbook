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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Linq;

using System.Windows.Markup;
using System.Management;

using Webb.Playbook.Geometry.Game;
using Webb.Playbook.Data;
using Draw = Webb.Playbook.Geometry.Drawing;
public enum WorkState
{
    FormationBlank,
    //  Minimize the Tools area
    //  Maximize the Action area
    //      Display New Formation
    //      Display New Folder
    AdjustmentBlank,
    AdjustmentTypeBlank,
    PlaybookBlank,
    AfterNewFormation,
    AfterOpenFormation,
    //  Maximize the Tools area
    //      Display Add Position
    //      Display Remove Position
    //          Remove Position is displayed but not active unless a psition is selected
    //      Display Substitute Position
    //          Only active if a position is selected
    //          Opens Personnel Editor
    //  Maximize the Symbol Set area
    //  Maximize the Action area
    //      Display Save,Save As,Rename,Delete,New Folder,New Formation
    //      Display Flip Formation(HReverse)
    //      Display Undo action
    //      Display Redo action
    AfterNewPlay,
    AfterOpenPlay,
    AfterNewAdjustment,
    AfterOpenAdjustment,
    AfterNewFormationPlay,
    AfterOpenFormationPlay,
    AfterOpenTitle,
    AfterNewTitle,
}

namespace Webb.Playbook
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr mD3DHandle = IntPtr.Zero;
        private bool mInitializing = false;
        private PBPlayer selectedPlayer = null;
        private string currentCopyPlayPath = string.Empty;
        private string stringreportPath = AppDomain.CurrentDomain.BaseDirectory + @"\Report\";
        private string currentPath = string.Empty;
        public string CurrentPath
        {
            get { return currentPath; }
            set
            {
                currentPath = value;

                if (File.Exists(currentPath))
                {
                    FileInfo fi = new FileInfo(currentPath);
                    this.Title = "Webb Playbook - " + fi.FullName;
                    this.tabMain.Header = fi.Name;
                }

                if (currentPath == string.Empty)
                {
                    this.Title = "Webb Playbook";
                    this.tabMain.Header = "     ";
                }
                VisibleBaseFormation(currentPath.EndsWith(".Play"), currentPath.EndsWith(".Form"));
                VisibleHreverseControls(currentPath.EndsWith(".Play"), currentPath.EndsWith(".Form"));
                Visible2D3DControls(File.Exists(CurrentPath));
                VisibleDrawingControls(currentPath.EndsWith(".Play"), currentPath.EndsWith(".Form"));

                //03-10-2010 Scott
                Webb.Playbook.Geometry.Dragger dragger = Webb.Playbook.Geometry.Behavior.LookupByName("Dragger") as Webb.Playbook.Geometry.Dragger;
                if (dragger != null)
                {
                    // 12-01-2010 Scott
                    //dragger.Mode = currentPath.EndsWith(".Play") ? 1 : 0;
                    dragger.Mode = 1;
                }
            }
        }

        // 07-21-2010 Scott
        private event Action<WorkState, WorkState> WorkStateChanged;
        private WorkState workState = WorkState.FormationBlank;
        public WorkState WorkState
        {
            get { return workState; }
            set
            {
                if (workState != value)
                {
                    WorkState oldWorkState = workState;

                    workState = value;

                    if (WorkStateChanged != null)
                    {
                        WorkStateChanged(oldWorkState, workState);
                    }
                }
            }
        }
        private string currentTheme;
        public string CurrentTheme
        {
            get { return currentTheme; }
            set { currentTheme = value; }
        }

        private Draw drawing;
        public Draw Drawing
        {
            get { return drawing; }
            protected set { drawing = value; }
        }

        private static Mode mainMode;
        public static Mode MainMode
        {
            get { return mainMode; }
            set { mainMode = value; }
        }

        private ViewModel.FormationRootViewModel formationRootViewModel;
        private ViewModel.PlaybookRootViewModel playbookRootViewModel;
        //private ViewModel.AdjustmentRootViewModel assignmentRootViewModel = new Webb.Playbook.ViewModel.AdjustmentRootViewModel();
        //private ViewModel.PlaybookRootViewModel playbookRootViewModel_FV = new Webb.Playbook.ViewModel.PlaybookRootViewModel(Webb.Playbook.ViewModel.ViewMode.FormationNameView); // 09-16-2010 Scott
        private ToolManager toolManagerToolsMenu = new ToolManager();
        private ToolManager toolManagerPlayerActionsMenu = new ToolManager();
        private ToolManager toolManagerSubMenu = new ToolManager();
        private ToolManager toolManagerActions = new ToolManager();
        private ToolManager toolManagerToolBar = new ToolManager();
        private ToolManager toolManager2D3DToolBar = new ToolManager();
        private ToolManager toolManagerBaseFormation = new ToolManager();
        private ToolManager toolManagerHreverseToolBar = new ToolManager();
        private ToolManager toolManagerLineTypeToolBar = new ToolManager();
        private ToolManager toolManagerDashTypeToolBar = new ToolManager();
        private ToolManager toolManagerCapTypeToolBar = new ToolManager();
        private ToolManager toolManagerColorsToolBar = new ToolManager();
        private ToolManager toolManagerDrawingMode = new ToolManager();
        private ViewModel.MenuManager formationMenuManager = new Webb.Playbook.ViewModel.MenuManager();
        private ViewModel.MenuManager playbookMenuManager = new Webb.Playbook.ViewModel.MenuManager();
        //private ViewModel.MenuManager adjustmentMenuManager = new Webb.Playbook.ViewModel.MenuManager();

        public MainWindow()
        {
            WorkStateChanged += new Action<WorkState, WorkState>(MainWindow_WorkStateChanged);  //07-21-2010 Scott
            Settings.Instance.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Settings");

            formationRootViewModel = new Webb.Playbook.ViewModel.FormationRootViewModel(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);
            playbookRootViewModel = new Webb.Playbook.ViewModel.PlaybookRootViewModel(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder, Webb.Playbook.ViewModel.ViewMode.PlayNameView);

            InitializeComponent();

            this.Closing += new CancelEventHandler(MainWindow_Closing);

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            RegisterComs();

            LoadThemes();

            // activate
            int nDays = 0;
            bool bFirstRun = false;
            Webb.Playbook.Activate.Register register = new Webb.Playbook.Activate.Register();
            if (!register.CheckRegister(out nDays, out bFirstRun))
            {
                ActivateWindow aw = new ActivateWindow();

                if (nDays == 0 && !bFirstRun)
                {
                    aw.CheckRealOnly = true;
                }

                if (nDays > 0)
                {
                    MessageBox.Show(string.Format("Webb Playbook\n\n{0} day remaining in your evaluation!", nDays), "Activation", MessageBoxButton.OK);
                }

                if (aw.CheckRealOnly)
                {
                    MessageBox.Show("Webb Playbook\n\nThe trial version of this product has expired.\nContact Webb Electronics today to purchase!\n\nPhone: 972-242-5400\nwww.webbelectronics.com", "Webb Electronics Activate", MessageBoxButton.OK);
                }

                if (!aw.ShowDialog().Value)
                {
                    App.Current.Shutdown();
                    return;
                }
            }

            Webb.Playbook.Activate.Register.GetProductTypeReg();

            SplashWindow splashWindow = new SplashWindow();
            splashWindow.ShowDialog();

            toolManagerToolBar.InitToolBarMenu();
            toolbarMain.ItemsSource = toolManagerToolBar.Tools;

            toolManager2D3DToolBar.Init2D3DToolBarMenu(false);
            toolbar2D3D.ItemsSource = toolManager2D3DToolBar.Tools;

            toolManagerBaseFormation.InitBaseFormationMenu();
            toolbarBaseFormation.ItemsSource = toolManagerBaseFormation.Tools;

            toolManagerHreverseToolBar.InitHReverseToolBarMenu();
            toolbarHReverse.ItemsSource = toolManagerHreverseToolBar.Tools;

            toolManagerLineTypeToolBar.InitLineTypeToolBarMenu();
            toolbarLineType.ItemsSource = toolManagerLineTypeToolBar.Tools;

            toolManagerDashTypeToolBar.InitDashTypeToolBarMenu();
            toolbarDashType.ItemsSource = toolManagerDashTypeToolBar.Tools;

            toolManagerCapTypeToolBar.InitCapTypeToolBarMenu();
            toolbarCapType.ItemsSource = toolManagerCapTypeToolBar.Tools;

            toolManagerColorsToolBar.InitColorToolBarMenu();
            toolbarColors.ItemsSource = toolManagerColorsToolBar.Tools;

            toolManagerDrawingMode.InitDrawingModeToolBarMenu();
            toolbarDrawingMode.ItemsSource = toolManagerDrawingMode.Tools;

            toolManagerToolsMenu.InitToolsMenu(WorkState.FormationBlank);
            toolManagerActions.InitActionsMenu(null, WorkState.FormationBlank, (int)ScoutTypes.Offensive);
            PersonnelEditor.Setting.Load(PersonnelEditor.File);
            Team.EditTeam et = new Webb.Playbook.Team.EditTeam();
            et.GetTeam();

            Webb.Playbook.Data.Terminology.Instance.SetTermFilePath(GameSetting.Instance.UserFolder);    //test

            if (GameSetting.Instance.CurrentTheme == "Gray.xaml") // bad code
            {
                LoadTheme(AppDomain.CurrentDomain.BaseDirectory + @"\Themes\Blue.xaml");
            }
        }

        private bool RegisterComs()
        {
            bool bSuccess = true;
            string strDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Process p = new Process();
            p.StartInfo.FileName = "Regsvr32.exe";

            p.StartInfo.Arguments = "/s " + strDirectory + @"\Diagram\WebbGameData.dll";
            if (!p.Start())
            {
                bSuccess = false;
                return bSuccess;
            }

            p.StartInfo.Arguments = "/s " + strDirectory + @"\game\CoRemote.dll";
            if (!p.Start())
            {
                bSuccess = false;
                return bSuccess;
            }

            p.StartInfo.Arguments = "/s " + strDirectory + @"\WWPlayer.ocx";
            if (!p.Start())
            {
                bSuccess = false;
                return bSuccess;
            }

            return bSuccess;
        }

        // 07-21-2010 Scott
        void MainWindow_WorkStateChanged(WorkState oldState, WorkState newState)
        {
            toolManagerToolsMenu.InitToolsMenu(newState);
            toolManagerActions.InitActionsMenu(null, newState, (int)GetScoutType(treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel));

            if (newState == WorkState.PlaybookBlank | newState == WorkState.FormationBlank | newState == WorkState.AdjustmentBlank | newState == WorkState.AdjustmentTypeBlank)
            {// todo
                toolManagerPlayerActionsMenu.Clear();
                toolManagerSubMenu.Clear();
            }

            if (newState == WorkState.AfterNewFormation)
            {// todo

            }

            if (newState == WorkState.AfterNewFormation || newState == WorkState.AfterNewFormationPlay || newState == WorkState.AfterNewPlay)
            {
                SaveImages();
                LoadFreeDraw();
                Drawing.ShowCenterLine();
            }

            // 05-30-2011 Scott
            if (newState == WorkState.AfterNewFormation || newState == WorkState.AfterNewPlay || newState == WorkState.AfterOpenFormation || newState == WorkState.AfterOpenPlay)
            {
                borderFormation.Visibility = Visibility.Visible;
            }
            else
            {
                borderFormation.Visibility = Visibility.Collapsed;
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mInitializing)
            {
                e.Cancel = true;
                return;
            }

            if (mD3DHandle != IntPtr.Zero)
            {
                e.Cancel = true;
                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_CLOSE, 0, 0);
                mD3DHandle = IntPtr.Zero;
                mInitializing = false;
                EnableWindow = true;
            }
            else
            {
                SaveCurrentFile(); // 09-20-2010 Scott
            }

            Settings.Instance.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Settings");
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
                    item.Header = fi.Name.Remove(fi.Name.IndexOf("."));
                    item.Tag = strFile;

                    item.Click += new RoutedEventHandler(item_Click);

                    this.menuItemThemes.Items.Add(item);

                    if (fi.Name == GameSetting.Instance.CurrentTheme)
                    {//default theme
                        LoadTheme(strFile);
                        SetCheck(item);
                    }
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

                GameSetting.Instance.CurrentTheme = mi.Header + ".xaml";
            }
        }

        protected void SetCheck(MenuItem mi)
        {
            MenuItem miParent = mi.Parent as MenuItem;

            if (miParent != null)
            {
                foreach (object o in miParent.Items)
                {
                    MenuItem menuItem = o as MenuItem;
                    if (menuItem != null)
                    {
                        menuItem.IsChecked = false;
                    }
                }
                mi.IsChecked = true;
            }
        }

        public void DetachData()
        {
            if (this.treeFormation.DataContext != null)
            {
                this.treeFormation.DataContext = null;
            }
            if (this.treePlaybook.DataContext != null)
            {
                this.treePlaybook.DataContext = null;
            }
            if (this.treePlaybook_FV.DataContext != null)
            {
                this.treePlaybook_FV = null;
            }
            if (this.treeAdjustment.DataContext != null)   // 08-27-2010 Scott
            {
                this.treeAdjustment.DataContext = null;
            }
            if (this.expanderTool.DataContext != null)
            {
                this.expanderTool.DataContext = null;
            }
            if (this.expanderTop.DataContext != null)
            {
                this.expanderTop.DataContext = null;
            }
            if (this.expanderBottom.DataContext != null)
            {
                this.expanderBottom.DataContext = null;
            }
            if (this.expanderAction.DataContext != null)
            {
                this.expanderAction.DataContext = null;
            }
            if (this.treeFormation.ContextMenu.DataContext != null)
            {
                this.treeFormation.ContextMenu.DataContext = null;
            }
            if (this.treePlaybook.ContextMenu.DataContext != null)
            {
                this.treePlaybook.ContextMenu.DataContext = null;
            }
            if (this.treeAdjustment.ContextMenu.DataContext != null)
            {
                this.treeAdjustment.ContextMenu.DataContext = null;
            }
        }

        public void AttachData()
        {
            this.treeFormation.ContextMenu.DataContext = formationMenuManager;

            this.treePlaybook.ContextMenu.DataContext = playbookMenuManager;

            //this.treeAdjustment.ContextMenu.DataContext = adjustmentMenuManager;

            this.treeFormation.DataContext = formationRootViewModel;

            this.treePlaybook.DataContext = playbookRootViewModel;

            //this.treePlaybook_FV.DataContext = playbookRootViewModel_FV;

            //this.treeAdjustment.DataContext = assignmentRootViewModel; // 08-27-2010 Scott

            this.expanderTool.DataContext = toolManagerToolsMenu;

            this.expanderTop.DataContext = toolManagerPlayerActionsMenu;

            this.expanderBottom.DataContext = toolManagerSubMenu;

            this.expanderAction.DataContext = toolManagerActions;
        }

        public void LoadTheme(string strFile)
        {
            if (File.Exists(strFile))
            {
                // 11-08-2010 Scott
                ViewModel.TreeViewItemViewModel formVM = null;
                ViewModel.TreeViewItemViewModel playVM = null;

                if (treeFormation.SelectedItem != null)
                {
                    formVM = treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;
                }

                if (treeFormation.SelectedItem != null)
                {
                    playVM = treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;
                }

                DetachData();

                ResourceDictionary resDic = new ResourceDictionary();
                resDic.Source = new Uri(strFile, UriKind.RelativeOrAbsolute);
                App.Current.Resources = resDic;

                AttachData();

                if (formVM != null)
                {
                    formVM.IsSelected = true;
                }

                if (playVM != null)
                {
                    playVM.IsSelected = true;
                }
            }
        }

        // Interop, 3D View
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GameSetting.Instance.CurrentTheme == "ExpressionDark.xaml")
            {
                LoadTheme(AppDomain.CurrentDomain.BaseDirectory + @"\Themes\ExpressionDark.xaml");
            }

            HwndSource hs = (HwndSource)HwndSource.FromVisual(this);
            hs.AddHook(new HwndSourceHook(WndProc));

            // 03-18-2011 Scott
            CreatePlayerMenuItems(miPlayerSettings);

            imgLogo.Effect = Webb.Playbook.Geometry.PBEffects.SelectedEffect;

            // 07-25-2011
            string strHelpVideosPath = AppDomain.CurrentDomain.BaseDirectory + @"\Help Videos\";
            if (Directory.Exists(strHelpVideosPath))
            {
                foreach (string strVideoFile in Directory.GetFiles(strHelpVideosPath, "*.wmv"))
                {
                    MenuItem miVideo = new MenuItem()
                    {
                        Tag = strVideoFile,
                        Header = System.IO.Path.GetFileNameWithoutExtension(strVideoFile),
                    };

                    miVideo.Click += new RoutedEventHandler(miVideo_Click);

                    miHelpVideos.Items.Add(miVideo);
                }
            }
        }

        public void CreatePlayerMenuItems(ItemsControl menu)
        {
            // player size
            Slider sliderPlayerSize = new Slider()
            {
                Value = Webb.Playbook.Data.GameSetting.Instance.PlayerSize,
                Minimum = 0.25,
                Maximum = 2.0,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.25,
                ToolTip = "Size",
                IsTabStop = false,
            };
            sliderPlayerSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderPlayerSize_ValueChanged);
            menu.Items.Add(sliderPlayerSize);

            // letters visibility
            MenuItem miTextVisible = new MenuItem()
            {
                Header = "Hide letters",
            };
            if (selectedPlayer != null)
            {
                miTextVisible.Header = Webb.Playbook.Data.GameSetting.Instance.PlayerTextVisibility ? "Hide letters" : "Show letters";
            }
            miTextVisible.Click += new RoutedEventHandler(miTextVisible_Click);
            menu.Items.Add(miTextVisible);

            // dash
            MenuItem miDash = new MenuItem()
            {
                Header = "Dash",
            };
            if (selectedPlayer != null)
            {
                miDash.Header = Webb.Playbook.Data.GameSetting.Instance.PlayerDash ? "Solid" : "Dash";
            }
            miDash.Click += new RoutedEventHandler(miDash_Click);
            menu.Items.Add(miDash);

            // shape
            MenuItem miSymbols = new MenuItem()
            {
                Header = "Symbols",
            };
            menu.Items.Add(miSymbols);

            MenuItem miCircle = new MenuItem()
            {
                Header = "Circle",
            };
            miCircle.Click += new RoutedEventHandler(miCircle_Click);
            miCircle.IsCheckable = true;
            miCircle.IsChecked = Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance.ToString() == miCircle.Header;
            miSymbols.Items.Add(miCircle);

            MenuItem miSquare = new MenuItem()
            {
                Header = "Square",
            };
            miSquare.Click += new RoutedEventHandler(miSquare_Click);
            miSquare.IsCheckable = true;
            miSquare.IsChecked = Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance.ToString() == miSquare.Header;
            miSymbols.Items.Add(miSquare);

            MenuItem miTriangle = new MenuItem()
            {
                Header = "Triangle",
            };
            miTriangle.Click += new RoutedEventHandler(miTriangle_Click);
            miTriangle.IsCheckable = true;
            miTriangle.IsChecked = Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance.ToString() == miTriangle.Header;
            miSymbols.Items.Add(miTriangle);

            MenuItem miText = new MenuItem()
            {
                Header = "Letters",
            };
            miText.Click += new RoutedEventHandler(miText_Click);
            miText.IsCheckable = true;
            miText.IsChecked = Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance.ToString() == miText.Header;
            miSymbols.Items.Add(miText);

            // angle
            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            sp.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
            TextBlock tblk = new TextBlock()
            {
                Text = "Angle: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            tblk.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
            tblk.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
            TextBox tbox = new TextBox()
            {
                Width = 50,
                Text = Webb.Playbook.Data.GameSetting.Instance.PlayerAngle.ToString(),
            };
            tbox.KeyDown += new KeyEventHandler(tbox_KeyDown);
            tbox.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
            tbox.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
            sp.Children.Add(tblk);
            sp.Children.Add(tbox);
            menu.Items.Add(sp);

            // thickness
            MenuItem miThickness = new MenuItem()
            {
                Header = "Thickness",
            };
            Slider sliderLineThickness = new Slider()
            {
                Value = Webb.Playbook.Data.GameSetting.Instance.PlayerThickness,
                Minimum = 1,
                Maximum = 5,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.5,
                ToolTip = "Thickness",
                IsTabStop = false,
                Orientation = Orientation.Vertical,
                Width = 25,
                Height = 100,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                AutoToolTipPrecision = 1,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
                IsDirectionReversed = false,
                IsMoveToPointEnabled = false,
            };
            sliderLineThickness.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderLineThickness_ValueChanged);
            miThickness.Items.Add(sliderLineThickness);
            menu.Items.Add(miThickness);
        }

        void sliderLineThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Webb.Playbook.Data.GameSetting.Instance.PlayerThickness = e.NewValue;
        }

        void tbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox tb = sender as TextBox;

                if (tb.Text.ToCharArray().All(c => char.IsDigit(c)))
                {
                    double angle = tb.Text.ToDouble(0);

                    Webb.Playbook.Data.GameSetting.Instance.PlayerAngle = angle;
                }
            }
        }

        void miText_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SetCheck(mi);
            Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance = PlayerAppearance.Text.ToString();
        }

        void miTriangle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SetCheck(mi);
            Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance = PlayerAppearance.Triangle.ToString();
        }

        void miSquare_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SetCheck(mi);
            Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance = PlayerAppearance.Square.ToString();
        }

        void miCircle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SetCheck(mi);
            Webb.Playbook.Data.GameSetting.Instance.PlayerAppearance = PlayerAppearance.Circle.ToString();
        }

        void miDash_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            switch (mi.Header.ToString())
            {
                case "Dash":
                    mi.Header = "Solid";
                    break;
                case "Solid":
                    mi.Header = "Dash";
                    break;
            }

            Webb.Playbook.Data.GameSetting.Instance.PlayerDash = !Webb.Playbook.Data.GameSetting.Instance.PlayerDash;
        }

        void miTextVisible_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            switch (mi.Header.ToString())
            {
                case "Show letters":
                    mi.Header = "Hide letters";
                    break;
                case "Hide letters":
                    mi.Header = "Show letters";
                    break;
            }

            Webb.Playbook.Data.GameSetting.Instance.PlayerTextVisibility = !Webb.Playbook.Data.GameSetting.Instance.PlayerTextVisibility;
        }

        void sliderPlayerSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Webb.Playbook.Data.GameSetting.Instance.PlayerSize = e.NewValue;
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            if (msg == (int)MessageCommands.WM_OPENGAME)    // game will send a handle in wparam in this meg
            {
                //System.Windows.Media.Animation.Storyboard story = FindResource("Storyboard1") as System.Windows.Media.Animation.Storyboard;
                //story.Stop();

                mD3DHandle = wParam;

                //int nlSizeParam = InteropHelper.MakeLParam((int)this.gridPlayGround.ActualHeight, (int)this.gridPlayGround.ActualWidth);
                //Point location = this.gridPlayGround.TranslatePoint(new Point(0, 0), this);
                int nlSizeParam = InteropHelper.MakeLParam((int)this.gridMain.ActualHeight, (int)this.gridMain.ActualWidth);
                Point location = this.TranslatePoint(new Point(0, 0), this);

                int nlPosParam = InteropHelper.MakeLParam((int)location.Y, (int)location.X);

                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_MOVE, 0, nlPosParam);
                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_SIZE, 0, nlSizeParam);
                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_SETFOCUS, 0, 0);

                mInitializing = false;
            }
            if (msg == (int)MessageCommands.WM_CLOSEGAME)    // close game
            {
                if (mD3DHandle != IntPtr.Zero)
                {
                    InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_CLOSE, 0, 0);
                    mD3DHandle = IntPtr.Zero;
                    this.Focus();
                }
                EnableWindow = true;
            }
            return IntPtr.Zero;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int lSizeParam = InteropHelper.MakeLParam((int)(e.NewSize.Height), (int)(e.NewSize.Width));

            Point location = this.TranslatePoint(new Point(0, 0), this);
            int lPosParam = InteropHelper.MakeLParam((int)location.Y, (int)location.X);

            if (mD3DHandle != IntPtr.Zero)
            {
                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_MOVE, 0, lPosParam);
                InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_SIZE, 0, lSizeParam);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //int lSizeParam = InteropHelper.MakeLParam((int)(e.NewSize.Height), (int)(e.NewSize.Width));

            //Point location = this.gridPlayGround.TranslatePoint(new Point(0, 0), this);
            //int lPosParam = InteropHelper.MakeLParam((int)location.Y, (int)location.X);

            //if (mD3DHandle != IntPtr.Zero)
            //{
            //    InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_MOVE, 0, lPosParam);
            //    InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_SIZE, 0, lSizeParam);
            //}
        }

        // Functions
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            Drawing.ActionManager.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            Drawing.ActionManager.Redo();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Delete();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);

            switch (e.Key)
            {
                case System.Windows.Input.Key.Z:
                    if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl) || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
                        Drawing.ActionManager.Undo();
                    break;
                case System.Windows.Input.Key.Y:
                    if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl) || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
                        Drawing.ActionManager.Redo();
                    break;
                case System.Windows.Input.Key.Delete:
                    Drawing.Delete();
                    break;
                case System.Windows.Input.Key.Escape:
                    //Drawing.SetDefaultBehavior();
                    break;
            }
        }

        private void btnOpenGame_Click(object sender, RoutedEventArgs e)
        {
            string strFile = AppDomain.CurrentDomain.BaseDirectory + @"\Game\WEBB3D.exe";

            if (File.Exists(strFile))
            {
                //System.Windows.Media.Animation.Storyboard story = FindResource("Storyboard1") as System.Windows.Media.Animation.Storyboard;
                //story.Begin();

                IntPtr ptrGame = InteropHelper.FindWindow(string.Empty, "WEBB3D");

                if (mD3DHandle == IntPtr.Zero && !mInitializing /*&& MainMode == Mode.Playbook && CurrentPath.EndsWith(".Play")*/)
                {
                    EnableWindow = false;

                    mInitializing = true;

                    HwndSource hs = (HwndSource)HwndSource.FromVisual(this);
                    IntPtr handle = hs.Handle;
                    string str = handle.ToString();

                    // prepare files for 3D View
                    Settings.Instance.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Settings");
                    PlayInfo pi = new PlayInfo();
                    if (File.Exists(CurrentPath + ".PlayInfo"))
                    {
                        pi.Load(CurrentPath + ".PlayInfo");
                    }

                    string strFormation = string.Format("{0} VS {1}", pi.OffensiveFormation, pi.DefensiveFormation);
                    Drawing.SaveToXml(strFormation, AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\Play", GameSetting.Instance.OurTeamOffensive);
                    System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\setting.per", AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\Setting", true);
                    System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\Settings", AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\Settings", true);
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Teams\Our Team.team"))
                    {
                        System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\Teams\Our Team.team", AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\OurTeam", true);
                    }
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Teams\Other Team.team"))
                    {
                        System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\Teams\Other Team.team", AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\OtherTeam", true);
                    }
                    if (File.Exists(CurrentPath + ".PlayInfo"))
                    {
                        System.IO.File.Copy(CurrentPath + ".PlayInfo", AppDomain.CurrentDomain.BaseDirectory + @"\Game\MyGame\PlayInfo", true);
                    }

                    // start
                    Process process = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = strFile;
                    psi.Arguments = "-window " + str;
                    process.StartInfo = psi;
                    bool bRet = process.Start();
                }
            }
            else
            {
                MessageBox.Show("Can't find WEBB3D.exe");
            }
        }

        private void btnCloseGame_Click(object sender, RoutedEventArgs e)
        {
            //if (mD3DHandle != IntPtr.Zero)
            //{
            InteropHelper.PostMessage(mD3DHandle, (int)MessageCommands.WM_CLOSE, 0, 0);
            mD3DHandle = IntPtr.Zero;
            mInitializing = false;
            //}
        }

        private void File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //saveFileDialog.Filter = "Formation files (*.Form)|*.Form|All files (*.*)|*.*";
            //saveFileDialog.AddExtension = true;

            //if (saveFileDialog.ShowDialog().Value)
            //{
            //    Drawing.Save(saveFileDialog.FileName);
            //}
            SaveAs();
        }

        private void File_New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void File_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }

        private void File_Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void File_Rename_Click(object sender, RoutedEventArgs e)
        {
            Rename();
        }

        private void File_Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void File_Close_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentFile();

            CreateDrawing();

            this.borderFormation.DataContext = null;
            CurrentPath = string.Empty;
        }

        private void Import_Games_Click(object sender, RoutedEventArgs e)   // 09-27-2011 Scott
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = Webb.Playbook.Data.Extensions.PackageFilter,
                AddExtension = true,
            };

            if (openFileDialog.ShowDialog().Value)
            {
                //if (MessageBox.Show("Do you want to load this drawings? that will overwrite your current drawings.", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                CloseDrawing();

                string strTempUserFolder = AppDomain.CurrentDomain.BaseDirectory + "TempUser";

                if (System.IO.Directory.Exists(strTempUserFolder))
                {
                    System.IO.Directory.Delete(strTempUserFolder, true);
                }

                SelectPlayForImportWindow importWindow = new SelectPlayForImportWindow(openFileDialog.FileName, strTempUserFolder)
                {
                    Owner = this,
                };

                importWindow.ShowDialog();

                ReloadTree();
                //}
            }
        }

        private void Export_Games_Click(object sender, RoutedEventArgs e)   // 09-27-2011 Scott
        {
            string strTempUserFolder = AppDomain.CurrentDomain.BaseDirectory + "TempUser";

            if (System.IO.Directory.Exists(strTempUserFolder))
            {
                System.IO.Directory.Delete(strTempUserFolder, true);
            }

            SelectPlayForExportWindow exportWindow = new SelectPlayForExportWindow(strTempUserFolder)
            {
                Owner = this,
            };

            if (exportWindow.ShowDialog().Value)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    Filter = Webb.Playbook.Data.Extensions.PackageFilter,
                    AddExtension = true,
                };

                if (saveFileDialog.ShowDialog().Value)
                {
                    ZipClass.CreateZip(strTempUserFolder, saveFileDialog.FileName);
                }
            }
        }

        private void Save()
        {
            //todo title
            //if (CurrentPath.EndsWith(".TTL"))
            //{
            //foreach(PBImage pbImg in Drawing.Figures.OfType<PBImage>())
            //{
            //   //pbImg.LocalizeImg(CurrentPath);
            //}
            //}

            if (File.Exists(CurrentPath))
            {
                Drawing.Save(CurrentPath);

                if (!CurrentPath.EndsWith(".TTL"))
                {
                    SaveImages();
                }

                SaveFreeDraw();
            }
        }

        private void SaveImages()
        {
            if (Drawing != null)
            {
                // make path
                string strName = System.IO.Path.GetFileNameWithoutExtension(CurrentPath);
                string strScoutType = string.Empty;
                bool bSubFiled = CurrentPath.Contains("@");
                string strFieldName = string.Empty;
                if (Drawing.ScoutType == ScoutTypes.Offensive)
                {
                    strScoutType = "Offense";
                    strFieldName = bSubFiled ? Webb.Playbook.Data.GameSetting.Instance.OffensiveSubField : Webb.Playbook.Data.GameSetting.Instance.OffensiveMainField;
                }

                if (Drawing.ScoutType == ScoutTypes.Defensive)
                {
                    strScoutType = "Defense";
                    strFieldName = bSubFiled ? Webb.Playbook.Data.GameSetting.Instance.DefensiveSubField : Webb.Playbook.Data.GameSetting.Instance.DefensiveMainField;
                }

                // 10-26-2011 Scott
                if (Drawing.ScoutType == ScoutTypes.Kicks)
                {
                    strScoutType = "Kick";
                    strFieldName = bSubFiled ? Webb.Playbook.Data.GameSetting.Instance.KickSubField : Webb.Playbook.Data.GameSetting.Instance.KickMainField;
                }

                string strBmpDir = AppDomain.CurrentDomain.BaseDirectory + @"\Bitmaps\" + strScoutType + @"\" + strFieldName + @"\";
                if (MainMode == Mode.Playbook)
                {
                    strBmpDir = AppDomain.CurrentDomain.BaseDirectory + @"\Bitmaps\Plays\";
                }

                if (!Directory.Exists(strBmpDir))
                {
                    Directory.CreateDirectory(strBmpDir);
                }

                string strBmpPath = strBmpDir + strName + ".BMP";
                string strBmpPathCropped = strBmpDir + strName + "001.BMP";

                // create drawing
                Size size = Size.Empty;
                DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(CurrentPath, true, new Size(1200, 3000), out size, Webb.Playbook.Data.GameSetting.Instance.ImageShowPlayground, true);

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
                    string strPlaybookBmpPath = CurrentPath + ".BMP";
                    SaveRTBAsBMP(bmp, strPlaybookBmpPath);

                    dbw.Close();

                    // save diagram
                    if (System.IO.Directory.Exists(Webb.Playbook.Data.GameSetting.Instance.UserFolder))
                    {
                        string strDiaDir = Webb.Playbook.Data.GameSetting.Instance.UserFolder + @"\Diagrams\" + strScoutType + @"\" + strFieldName + @"\";
                        if (MainMode == Mode.Playbook)
                        {
                            strDiaDir = Webb.Playbook.Data.GameSetting.Instance.UserFolder + @"\Diagrams\Plays\";
                        }
                        if (!Directory.Exists(strDiaDir))
                        {
                            Directory.CreateDirectory(strDiaDir);
                        }
                        string strDiaPath = strDiaDir + strName + ".DIA";
                        string strDiaPathCropped = strDiaDir + strName + "001.BMP";

                        Drawing.SaveToDiagram(strDiaPath, false);
                    }
                }
            }
            ResetBehavior();
        }

        private void SaveBasFormationImages(ScoutTypes scoutType)
        {
            string strScoutType = string.Empty;

            // 10-26-2011 Scott
            switch (scoutType)
            {
                case ScoutTypes.Offensive:
                    strScoutType = "Offense";
                    break;
                case ScoutTypes.Defensive:
                    strScoutType = "Defense";
                    break;
                case ScoutTypes.Kicks:
                    strScoutType = "Kick";
                    break;
            }

            string strBmpFolder = string.Format(@"{0}\Bitmaps\{1}\", AppDomain.CurrentDomain.BaseDirectory, strScoutType);

            if (!Directory.Exists(strBmpFolder))
            {
                Directory.CreateDirectory(strBmpFolder);
            }

            string strBmpPath = strBmpFolder + "Base Formation.BMP";

            if (Drawing != null)
            {
                // create drawing
                Size size = Size.Empty;
                DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(CurrentPath, true, new Size(1200, 3000), out size, Webb.Playbook.Data.GameSetting.Instance.ImageShowPlayground, true);

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

                    dbw.Close();

                    // save diagram
                    if (System.IO.Directory.Exists(Webb.Playbook.Data.GameSetting.Instance.UserFolder))
                    {
                        string strDiaDir = Webb.Playbook.Data.GameSetting.Instance.UserFolder + @"\Diagrams\" + strScoutType + @"\";
                        if (MainMode == Mode.Playbook)
                        {
                            strDiaDir = Webb.Playbook.Data.GameSetting.Instance.UserFolder + @"\Diagrams\Plays\";
                        }
                        if (!Directory.Exists(strDiaDir))
                        {
                            Directory.CreateDirectory(strDiaDir);
                        }
                        string strDiaPath = strDiaDir + "Base Formation.DIA";

                        Drawing.SaveToDiagram(strDiaPath, false);
                    }
                }
            }
            ResetBehavior();
        }

        private void SaveBitmap(string strBmpPath, Rect rect)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(rect.Width), (int)(rect.Height), 96, 96, PixelFormats.Default);
            DrawingVisual drawingvisual = new DrawingVisual();

            if (GameSetting.Instance.GridLine)
                Drawing.GridLines.Remove();

            Drawing.Canvas.Children.Remove(Drawing.Playground.UCPlayground);

            Print.LocalPrinter.ShowWindow();

            using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
            {
                VisualBrush visualbrush = new VisualBrush(this.gridDrawingContainer);

                drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, rect.Width, rect.Height));
            }

            bmp.Render(drawingvisual);

            SaveRTBAsPNG(bmp, strBmpPath);

            Drawing.Playground.UCPlayground.SetValue(Panel.ZIndexProperty, -1);
            Drawing.Canvas.Children.Add(Drawing.Playground.UCPlayground);
            if (GameSetting.Instance.GridLine)
                Drawing.GridLines.Add();
        }

        //shot screen
        private void btnSaveToImage_Click(object sender, RoutedEventArgs e)
        {// 09-28-2010 Scott
            Size size = Size.Empty;
            DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(CurrentPath, true, new Size(800, 1000), out size, false, false);

            if (size != Size.Empty)
            {
                Microsoft.Win32.SaveFileDialog dialogOpenFile = new Microsoft.Win32.SaveFileDialog();
                dialogOpenFile.Filter = "BMP(*.bmp)|*.BMP|PNG (*.png)|*.PNG|JPG(*.jpg)|*.JPG";
                dialogOpenFile.FileName = this.textFormation.Text;
                if (GameSetting.Instance.GridLine)
                    Drawing.GridLines.Remove();

                bool? b = dialogOpenFile.ShowDialog();
                if (b == true)
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

                    SaveRTBAsBMP(bmp, dialogOpenFile.FileName);

                    dbw.Close();
                }
                ResetBehavior();
            }

            //Rect rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(15), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(25));
            //if (CurrentPath.EndsWith(".Form"))
            //{
            //    if (Drawing.ScoutType == ScoutTypes.Offensive)
            //    {
            //        rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(0.5), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(10.5));
            //    }

            //    if (Drawing.ScoutType == ScoutTypes.Defensive)
            //    {
            //        rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(15), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(15.5));
            //    }
            //}
            //this.gridDrawingContainer.Clip = new RectangleGeometry(rect);

            //RenderTargetBitmap bmp = new RenderTargetBitmap((int)(rect.Width), (int)(rect.Height), 96, 96, PixelFormats.Default);
            //DrawingVisual drawingvisual = new DrawingVisual();

            //Microsoft.Win32.SaveFileDialog dialogOpenFile = new Microsoft.Win32.SaveFileDialog();
            //dialogOpenFile.Filter = "BMP(*.bmp)|*.BMP|PNG (*.png)|*.PNG|JPG(*.jpg)|*.JPG";
            //dialogOpenFile.FileName = this.textFormation.Text;
            //if (GameSetting.Instance.GridLine)
            //    Drawing.GridLines.Remove();

            //Drawing.Canvas.Children.Remove(Drawing.Playground.UCPlayground);

            //bool? b = dialogOpenFile.ShowDialog();
            //if (b == true)
            //{
            //    using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
            //    {
            //        VisualBrush visualbrush = new VisualBrush(this.gridDrawingContainer);

            //        drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, rect.Width, rect.Height));
            //    }

            //    bmp.Render(drawingvisual);

            //    SaveRTBAsPNG(bmp, dialogOpenFile.FileName);
            //}

            //Drawing.Playground.UCPlayground.SetValue(Panel.ZIndexProperty, -1);
            //Drawing.Canvas.Children.Add(Drawing.Playground.UCPlayground);
            //if (GameSetting.Instance.GridLine)
            //    Drawing.GridLines.Add();

            //this.gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, this.gridDrawingContainer.ActualWidth, this.gridDrawingContainer.ActualHeight));
        }

        private void btnSaveToImageField_Click(object sender, RoutedEventArgs e)
        {// 09-28-2010 Scott
            Size size = Size.Empty;
            DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(CurrentPath, true, new Size(800, 1000), out size, true, false);

            if (size != Size.Empty)
            {
                Microsoft.Win32.SaveFileDialog dialogOpenFile = new Microsoft.Win32.SaveFileDialog();
                dialogOpenFile.Filter = "BMP(*.bmp)|*.BMP|PNG (*.png)|*.PNG|JPG(*.jpg)|*.JPG";
                dialogOpenFile.FileName = this.textFormation.Text;
                if (GameSetting.Instance.GridLine)
                    Drawing.GridLines.Remove();

                bool? b = dialogOpenFile.ShowDialog();
                if (b == true)
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

                    SaveRTBAsBMP(bmp, dialogOpenFile.FileName);

                    dbw.Close();
                }

                ResetBehavior();
            }

            //Rect rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(15), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(25));
            //if (CurrentPath.EndsWith(".Form"))
            //{
            //    if (Drawing.ScoutType == ScoutTypes.Offensive)
            //    {
            //        rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(0.5), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(10.5));
            //    }

            //    if (Drawing.ScoutType == ScoutTypes.Defensive)
            //    {
            //        rect = new Rect(0, this.gridDrawingContainer.ActualHeight / 2 - Drawing.CoordinateSystem.ToPhysical(15), this.gridDrawingContainer.ActualWidth, Drawing.CoordinateSystem.ToPhysical(15.5));
            //    }
            //}
            //this.gridDrawingContainer.Clip = new RectangleGeometry(rect);

            //RenderTargetBitmap bmp = new RenderTargetBitmap((int)(rect.Width), (int)(rect.Height), 96, 96, PixelFormats.Pbgra32);
            //DrawingVisual drawingvisual = new DrawingVisual();

            //Microsoft.Win32.SaveFileDialog dialogOpenFile = new Microsoft.Win32.SaveFileDialog();
            //dialogOpenFile.Filter = "BMP(*.bmp)|*.BMP|PNG (*.png)|*.PNG|JPG(*.jpg)|*.JPG";
            //dialogOpenFile.FileName = this.textFormation.Text;
            //bool? b = dialogOpenFile.ShowDialog();
            //if (b == true)
            //{
            //    using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
            //    {
            //        VisualBrush visualbrush = new VisualBrush(this.gridDrawingContainer);

            //        drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, rect.Width, rect.Height));
            //    }

            //    bmp.Render(drawingvisual);

            //    SaveRTBAsPNG(bmp, dialogOpenFile.FileName);
            //}

            //this.gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, this.gridDrawingContainer.ActualWidth, this.gridDrawingContainer.ActualHeight));
        }

        private void CloseDrawing()
        {
            if (Drawing != null)
            {
                Drawing.Dispose();
            }

            Drawing = new Webb.Playbook.Geometry.Drawing(this.canvasDrawing);
            CurrentPath = string.Empty;
            borderFormation.DataContext = null;
        }

        private void Load(string strFileName)
        {
            // 11-04-2011 Scott
            if (CurrentPath == strFileName)
            {
                return;
            }

            CurrentPath = strFileName;

            if (Drawing != null)
            {
                Drawing.Dispose();
            }
            Drawing = Draw.Load(strFileName, this.canvasDrawing);
            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);

            if (GameSetting.Instance.GridLine)
            {
                Drawing.ShowGridLines();
            }
            PlayInfo playinfo = new PlayInfo();

            if (File.Exists(CurrentPath + ".PlayInfo"))
            {
                playinfo.Load(CurrentPath + ".PlayInfo");
                Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = playinfo.Mark;
                Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = PlaygroundStructure.GetHashLine(playinfo.HashLine);
            }

            ClearSelectedTools();

            // 07-29-2010 Scott
            if (MainMode == Mode.Formation && this.treeFormation.SelectedItem is ViewModel.FormationViewModel)
            {
                WorkState = WorkState.AfterOpenFormation;

                ScoutTypes scoutType = GetScoutType(treeFormation.SelectedItem as ViewModel.FormationViewModel);
                // 08-02-2010 Scott
                if (scoutType == ScoutTypes.Offensive)
                {
                    toolManagerSubMenu.InitSetOffSubMenu();
                    //toolManagerPlayerActionsMenu.InitSetOffSubMenu();
                }
                else if (scoutType == ScoutTypes.Defensive)
                {
                    toolManagerSubMenu.InitSetDefSubMenu();
                    //toolManagerPlayerActionsMenu.InitSetDefSubMenu();
                }
                else if (scoutType == ScoutTypes.Kicks)
                {// 10-26-2011 Scott
                    toolManagerSubMenu.InitSetKickSubMenu();
                }
                else
                {
                    toolManagerSubMenu.Clear();
                }
            }
            else
            {
                toolManagerSubMenu.Clear();
            }

            if (strFileName.EndsWith(".Play"))
            {
                if (MainMode == Mode.Adjustment)
                {
                    WorkState = WorkState.AfterOpenAdjustment;
                }
                if (MainMode == Mode.Playbook)
                {
                    WorkState = WorkState.AfterOpenPlay;
                }
                if (MainMode == Mode.Formation)
                {
                    WorkState = WorkState.AfterOpenFormationPlay;
                }
            }

            if (strFileName.EndsWith(".Ttl", true, null))
            {
                WorkState = WorkState.AfterOpenTitle;
            }

            LoadFreeDraw();
            Drawing.ShowCenterLine();
            Drawing.SetStartYardByBall();  // 01-20-2012 Scott
        }

        //private void LoadByScoutType(string strFileName, ScoutTypes scoutType)
        //{
        //    if (CurrentPath == strFileName)
        //    {
        //        return;
        //    }

        //    CurrentPath = strFileName;

        //    if (Drawing != null)
        //    {
        //        Drawing.Dispose();
        //    }
        //    Drawing = Draw.LoadByScoutType(strFileName, (int)scoutType, this.canvasDrawing);
        //    Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
        //    Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
        //    Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);

        //    if (GameSetting.Instance.GridLine)
        //    {
        //        Drawing.ShowGridLines();
        //    }
        //    PlayInfo playinfo = new PlayInfo();

        //    if (File.Exists(CurrentPath + ".PlayInfo"))
        //    {
        //        playinfo.Load(CurrentPath + ".PlayInfo");
        //        Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = playinfo.Mark;
        //        Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = PlaygroundStructure.GetHashLine(playinfo.HashLine);
        //    }

        //    ClearSelectedTools();

        //    // 07-29-2010 Scott
        //    if (strFileName.EndsWith(".Form"))
        //    {
        //        WorkState = WorkState.AfterOpenFormation;

        //        // 08-02-2010 Scott
        //        if (strFileName.Contains("Offensive"))
        //        {
        //            toolManagerSubMenu.InitSetOffSubMenu();
        //            //toolManagerPlayerActionsMenu.InitSetOffSubMenu();
        //        }
        //        else
        //        {
        //            toolManagerSubMenu.InitSetDefSubMenu();
        //            //toolManagerPlayerActionsMenu.InitSetDefSubMenu();
        //        }
        //    }
        //    if (strFileName.EndsWith(".Play"))
        //    {
        //        if (MainMode == Mode.Adjustment)
        //        {
        //            WorkState = WorkState.AfterOpenAdjustment;
        //        }
        //        if (MainMode == Mode.Playbook)
        //        {
        //            WorkState = WorkState.AfterOpenPlay;
        //        }
        //    }
        //}

        private void VisibleBaseFormation(bool playVisible, bool formationVisible)
        {
            if (MainMode == Mode.Formation)
            {
                if (formationVisible)
                {
                    toolbarBaseFormation.Visibility = Visibility.Visible;
                }
                else if (playVisible)
                {
                    toolbarBaseFormation.Visibility = Visibility.Visible;
                }
                else
                {
                    toolbarBaseFormation.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                toolbarBaseFormation.Visibility = Visibility.Collapsed;
            }
        }

        private void Visible2D3DControls(bool bVisible)
        {
            this.toolManager2D3DToolBar.Init2D3DToolBarMenu(bVisible);
            //this.toolbar2D3D.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void VisibleHreverseControls(bool bVisible, bool aVisible)
        {
            this.toolbarHReverse.Visibility = bVisible || aVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void VisibleDrawingControls(bool bVisible, bool aVisible)
        {
            this.toolbarTrayDrawing.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableControls(bool bEnable)
        {
            this.canvasDrawing.IsEnabled =
            this.treeFormation.IsEnabled =
            this.treePlaybook.IsEnabled =
            this.treeAdjustment.IsEnabled =    // 08-27-2010 Scott
            this.expanderTool.IsEnabled =
            this.expanderTop.IsEnabled =
            this.expanderBottom.IsEnabled =
            this.expanderAction.IsEnabled = bEnable;
        }

        private void btnHReverse_Click(object sender, RoutedEventArgs e)
        {
            Drawing.HReverse();
        }

        private void btnVReverse_Click(object sender, RoutedEventArgs e)
        {
            Drawing.VReverse();
        }

        private void canvasDrawing_Loaded(object sender, RoutedEventArgs e)
        {
            CreateDrawing();

            canvasDrawing.Margin = new Thickness(5);
            canvasDrawing.SizeChanged += new SizeChangedEventHandler(canvasDrawing_SizeChanged);

            Scale(GameSetting.Instance.Scaling);
        }

        void canvasDrawing_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasOutLine.Margin = canvasDrawing.Margin;
        }

        private void treeFormation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos = new Point(0, 0);
            //TreeView formationTree = sender as TreeView;

            //if (formationTree == this.treeFormation)
            //{
            //    ViewModel.FormationViewModel fvm = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

            //    if (fvm != null)
            //    {
            //        Load(fvm.FormationPath);

            //        this.borderFormation.DataContext = fvm.Formation;
            //        InitSetMenu(fvm);
            //    }
            //}
        }

        public bool SaveCurrentFile()
        {
            if (File.Exists(CurrentPath))
            {
                if (Drawing.OnEdit)
                {
                    Webb.Playbook.Geometry.Text textBehavior = Drawing.Behavior as Webb.Playbook.Geometry.Text;

                    if (textBehavior != null)
                    {
                        textBehavior.EndEdit();
                    }
                }

                FileInfo fi = new FileInfo(CurrentPath);
                if (fi != null && Drawing != null && Drawing.Modified)
                {
                    MessageBoxResult mbr = MessageBox.Show(string.Format("Do you want to save [{0}]", fi.Name), "Webb playbook", MessageBoxButton.YesNo);

                    if (mbr == MessageBoxResult.Yes)
                    {
                        Save();
                    }
                }
            }

            return true;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos = new Point(0, 0);

            StackPanel sp = sender as StackPanel;

            ViewModel.TreeViewItemViewModel tviv = sp.DataContext as ViewModel.TreeViewItemViewModel;

            if (tviv != null)
            {
                tviv.IsSelected = true;
            }

            // 05-27-2011 Scott
            if (sp.DataContext is ViewModel.TitleViewModel)
            {
                if (!SaveCurrentFile())
                    return;

                ViewModel.TitleViewModel tvm = sp.DataContext as ViewModel.TitleViewModel;

                if (tvm != null)
                {
                    Load(tvm.TitlePath);

                    Drawing.LoadTitleBackground();
                }
            }

            if (sp.DataContext is ViewModel.FormationViewModel && MainMode == Mode.Formation)
            {
                if (!SaveCurrentFile())
                    return;

                ViewModel.FormationViewModel fvm = sp.DataContext as ViewModel.FormationViewModel;

                if (fvm != null)
                {
                    Load(fvm.FormationPath);

                    this.borderFormation.DataContext = fvm.Formation;
                }
            }

            // 09-15-2010 Scott
            if (sp.DataContext is ViewModel.PVFormationViewModel && MainMode == Mode.Playbook)
            {
                if (!SaveCurrentFile())
                    return;

                ViewModel.PVFormationViewModel pvfViewModel = sp.DataContext as ViewModel.PVFormationViewModel;

                if (pvfViewModel != null)
                {
                    Load(pvfViewModel.Path);

                    UpdateTransparent((int)pvfViewModel.ScoutType);
                }
            }

            if (sp.DataContext is ViewModel.PlayViewModel)
            {
                if (!SaveCurrentFile())
                    return;

                ViewModel.PlayViewModel pvm = sp.DataContext as ViewModel.PlayViewModel;

                if (pvm != null)
                {
                    Load(pvm.PlayPath);

                    this.borderFormation.DataContext = pvm.Play;

                    UpdateTransparent(-1);
                    // 09-03-2010 Scott
                    ViewModel.AdjustmentTypeViewModel atvm = pvm.Parent as ViewModel.AdjustmentTypeViewModel;
                    if (atvm != null)
                    {
                        UpdateTransparent((int)atvm.AdjustmentTypeDirectory.ScoutType);
                    }
                }
            }

            if (sp.DataContext is ViewModel.FolderViewModel | sp.DataContext is ViewModel.ScoutTypeViewModel && MainMode == Mode.Formation)
            {
                ClearWorkSpace();

                WorkState = WorkState.FormationBlank;
            }

            if (sp.DataContext is ViewModel.ScoutTypeViewModel && MainMode == Mode.Adjustment)
            {
                ClearWorkSpace();

                WorkState = WorkState.AdjustmentBlank;
            }

            if (sp.DataContext is ViewModel.AdjustmentTypeViewModel && MainMode == Mode.Adjustment)
            {
                ClearWorkSpace();

                WorkState = WorkState.AdjustmentTypeBlank;
            }

            if ((sp.DataContext is ViewModel.PlayTypeViewModel || sp.DataContext is ViewModel.PlayFolderViewModel) && MainMode == Mode.Playbook)
            {
                ClearWorkSpace();

                WorkState = WorkState.PlaybookBlank;
            }

            UpdatePlayersText();
        }

        private void ClearWorkSpace()   // 07-19-2010 Scott
        {
            if (!SaveCurrentFile())
            {
                return;
            }

            CloseDrawing();
        }

        private void treePlaybook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                if (String.Compare(processes[i].ProcessName, "Call Editor", true) == 0)
                {
                    return;
                }
            }

            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = AppDomain.CurrentDomain.BaseDirectory + "Call Editor.exe";
            process.StartInfo = psi;
            process.Start();
        }

        private void btnTeam_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                if (String.Compare(processes[i].ProcessName, "Team Editor", true) == 0)
                {
                    return;
                }
            }

            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = AppDomain.CurrentDomain.BaseDirectory + "Team Editor.exe";
            process.StartInfo = psi;
            process.Start();
        }

        public void UpdatePlayersText()
        {
            // 01-17-2011 Scott remove
            //foreach (PBPlayer player in Drawing.Figures.OfType<PBPlayer>())
            //{
            //    foreach (Position pos in PersonnelEditor.Setting.AllPositions)
            //    {
            //        if (player.Name == pos.Name)
            //        {
            //            player.Text = pos.Value;
            //        }
            //    }
            //}
        }

        private void btnPersonnel_Click(object sender, RoutedEventArgs e)
        {
            PersonnelEditor pe = new PersonnelEditor();
            pe.Owner = this;
            if (pe.ShowDialog().Value)
            {
                //Update player's name.
                UpdatePlayersText();
            }
        }

        private void btnGridLine_Click(object sender, RoutedEventArgs e)
        {
            if (!GameSetting.Instance.GridLine)
            {
                Drawing.ShowGridLines();
            }
            else
            {
                Drawing.HideGridLines();
            }
            GameSetting.Instance.GridLine = !GameSetting.Instance.GridLine;
        }

        private void btnBall_Click(object sender, RoutedEventArgs e)
        {
            if (Drawing.Figures.OfType<PBBall>().Count() > 0)
            {
                GameSetting.Instance.ShowBall = !GameSetting.Instance.ShowBall;

                Drawing.ShowBall();
            }
        }

        private void contextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            if (treeView == this.treeFormation)
            {
                if (this.treeFormation.SelectedItem != null)
                {
                    ScoutTypes scoutType = GetScoutType(this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                    this.treeFormation.ContextMenu.Visibility = Visibility.Visible;

                    ViewModel.TreeViewItemViewModel viewModel = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;

                    if (viewModel is ViewModel.FormationViewModel)
                    {
                        formationMenuManager.LoadFormationMenu((int)scoutType);
                    }
                    else if (viewModel is ViewModel.PersonnelViewModel)
                    {
                        formationMenuManager.LoadPersonnelMenu();
                    }
                    else if (viewModel is ViewModel.ScoutTypeViewModel)
                    {
                        formationMenuManager.LoadScoutTypeMenu((int)scoutType);
                    }
                    else if (viewModel is ViewModel.FolderViewModel)
                    {
                        formationMenuManager.LoadFolderMenu((int)scoutType);
                    }
                    else if (viewModel is ViewModel.PlayViewModel)
                    {// 09-20-2010 Scott
                        formationMenuManager.LoadFormationPlayMenu();
                    }
                    else if (viewModel is ViewModel.TitleViewModel)
                    {// 11-02-2011 Scott
                        formationMenuManager.LoadTitleMenu();
                    }
                    else
                    {
                        this.treeFormation.ContextMenu.Visibility = Visibility.Hidden;
                        formationMenuManager.Clear();
                    }
                }
            }
            else if (treeView == this.treePlaybook)
            {
                this.treePlaybook.ContextMenu.Visibility = Visibility.Visible;

                ViewModel.TreeViewItemViewModel viewModel = this.treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;

                if (viewModel is ViewModel.PlayTypeViewModel)
                {
                    playbookMenuManager.LoadPlayTypeMenu();
                }
                else if (viewModel is ViewModel.PlayViewModel)
                {
                    playbookMenuManager.LoadPlayMenu();
                }
                else if (viewModel is ViewModel.PlayFolderViewModel)
                {
                    playbookMenuManager.LoadPlayFolderMenu();
                }
                else
                {
                    this.treePlaybook.ContextMenu.Visibility = Visibility.Hidden;
                    playbookMenuManager.Clear();
                }
            }
            //else if (treeView == this.treeAdjustment)
            //{
            //    this.treeAdjustment.ContextMenu.Visibility = Visibility.Visible;

            //    ViewModel.TreeViewItemViewModel viewModel = this.treeAdjustment.SelectedItem as ViewModel.TreeViewItemViewModel;

            //    if (viewModel is ViewModel.AdjustmentTypeViewModel)
            //    {
            //        adjustmentMenuManager.LoadAdjustmentTypeMenu();
            //    }
            //    else if (viewModel is ViewModel.PlayViewModel/*ViewModel.AdjustmentViewModel*/)
            //    {
            //        adjustmentMenuManager.LoadAdjustmentMenu();
            //    }
            //    else
            //    {
            //        this.treeAdjustment.ContextMenu.Visibility = Visibility.Hidden;
            //        adjustmentMenuManager.Clear();
            //    }
            //}
        }

        private void Open()
        {
            ViewModel.FormationViewModel formationViewModel;
            ViewModel.PlayViewModel playViewModel;
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos = new Point(0, 0);
            if ((int)MainMode == 0)
            {
                formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

                if (formationViewModel != null)
                {
                    this.Load(formationViewModel.FormationPath);
                    this.borderFormation.DataContext = formationViewModel.Formation;
                }
            }
            else if ((int)MainMode == 2)
            {
                playViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayViewModel;

                if (playViewModel != null)
                {
                    this.Load(playViewModel.PlayPath);
                    this.borderFormation.DataContext = playViewModel.Play;
                }
            }
        }

        // 09-17-2010 Scott
        public void CreatePlay()
        {
            if (!SaveCurrentFile())
                return;

            ViewModel.FormationViewModel formationViewModel = null;

            formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

            if (this.treeFormation.SelectedItem != null)
            {

                ScoutTypes scoutType = GetScoutType(this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                if (formationViewModel != null)
                {
                    string strFormation = formationViewModel.FormationPath;

                    string strDirectory = strFormation.Remove(strFormation.IndexOf(".Form")) + "@";

                    if (!Directory.Exists(strDirectory))
                    {
                        Directory.CreateDirectory(strDirectory);
                    }

                    ValueCollect vc = new ValueCollect();
                    vc.Values.AddRange(Data.Terminology.Instance.LoadValuesForFieldByType(GameSetting.Instance.GetField(scoutType, true), (int)GameSetting.Instance.UserType));

                    CreatePlayWindow nameWindow = new CreatePlayWindow(vc)
                    {
                        Title = "Create " + GameSetting.Instance.GetField(scoutType, true),
                        Owner = this,
                    };

                    if (nameWindow.ShowDialog().GetValueOrDefault())
                    {
                        string strPlayPath = strDirectory + @"\" + nameWindow.FileName + ".Play";

                        bool bOverwrite = false;    // 11-16-2010 Scott

                        if (File.Exists(strPlayPath))
                        {
                            if (MessageBox.Show("The " + GameSetting.Instance.GetField(scoutType, true) + " was exist,do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, true) + " ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {// OK
                                bOverwrite = true;
                            }
                            else
                            {// Cancel
                                return;
                            }
                        }

                        if (bOverwrite)
                        {
                            ViewModel.PlayViewModel oldPvm = formationViewModel.GetChild<ViewModel.PlayViewModel>(nameWindow.FileName);

                            if (oldPvm != null)
                            {
                                formationViewModel.RemoveChild(oldPvm);
                            }
                        }

                        File.Copy(strFormation, strPlayPath, bOverwrite);

                        Play play = new Play(strPlayPath);

                        ViewModel.PlayViewModel pvm = new Webb.Playbook.ViewModel.PlayViewModel(play, formationViewModel);

                        formationViewModel.AddChild(pvm);

                        Load(pvm.PlayPath);

                        borderFormation.DataContext = pvm.Play;

                        WorkState = WorkState.AfterNewFormationPlay;
                    }
                }
            }

            Drawing.SetStartYardByBall();   // 01-20-2012 Scott
        }

        public void Rename()
        {
            ViewModel.PersonnelViewModel personnelViewModel = null;
            ViewModel.FormationViewModel formationViewModel = null;
            ViewModel.PlayViewModel playViewModel = null;
            ViewModel.PlayTypeViewModel playTypeViewModel = null;
            ViewModel.FolderViewModel folderViewModel = null;

            if (this.treeFormation.SelectedItem != null)
            {
                ScoutTypes scoutType = GetScoutType(this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                if ((int)MainMode == 2)
                {
                    ViewModel.PlayFolderViewModel playFolderViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayFolderViewModel;
                    if (playFolderViewModel != null)
                    {
                        ViewModel.TreeViewItemViewModel parentViewModel = playFolderViewModel.Parent;

                        NameWindow nameWindow = new NameWindow()
                        {
                            Title = "Rename Folder",
                            Owner = this,
                            SelectAll = true,
                            FileName = playFolderViewModel.Name,
                        };
                        if (nameWindow.ShowDialog().Value)
                        {
                            int index = playFolderViewModel.FolderPath.LastIndexOf(playFolderViewModel.Name);
                            string str = playFolderViewModel.FolderPath.Substring(0, index);
                            string strNewDir = str + nameWindow.FileName;
                            //string strNewDir = folderViewModel.FolderPath.Replace(folderViewModel.Name, nameWindow.FileName);
                            if (Directory.Exists(strNewDir))
                            {
                                MessageBox.Show("This folder already exists !");
                                return;
                            }
                            Directory.Move(playFolderViewModel.FolderPath, strNewDir);

                            playFolderViewModel.Parent.RemoveChild(playFolderViewModel);
                            ViewModel.PlayFolderViewModel pViewModel = new Webb.Playbook.ViewModel.PlayFolderViewModel(strNewDir, parentViewModel);
                            parentViewModel.AddChild(pViewModel);
                        }
                    }

                    playViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayViewModel;
                    if (playViewModel != null)
                    {
                        ViewModel.TreeViewItemViewModel parentVM = playViewModel.Parent;
                        NameWindow nameWindow = new NameWindow()
                        {
                            Title = "Rename Play",
                            Owner = this,
                            SelectAll = true,
                        };
                        nameWindow.FileName = playViewModel.Play.Name;
                        if (nameWindow.ShowDialog() == true)
                        {
                            bool bOverwrite = false;    // 11-16-2010 Scott
                            string strNewFile = playViewModel.Play.Path.Replace(playViewModel.Play.Name + ".", nameWindow.FileName + ".");
                            if (File.Exists(strNewFile))
                            {
                                if (playViewModel.Play.Path == strNewFile)
                                {
                                    return;
                                }

                                if (MessageBox.Show("The play was exist,do you want to overwrite the existing play ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {// overwrite
                                    bOverwrite = true;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (IOHelper.FileEqual(playViewModel.Play.Path, strNewFile))
                            {// 12-13-2011 Scott
                                ViewModel.PlayViewModel oldPvm = parentVM.GetChild<ViewModel.PlayViewModel>(nameWindow.FileName);

                                if (oldPvm != null)
                                {
                                    IOHelper.ChangeCaseForFileName(oldPvm.Play.Path, strNewFile);
                                    if (File.Exists(oldPvm.Play.Path + ".PlayInfo"))
                                    {//12-28-2009 scott
                                        IOHelper.ChangeCaseForFileName(oldPvm.Play.Path + ".PlayInfo", strNewFile + ".PlayInfo");
                                    }
                                    oldPvm.Parent.Refresh();
                                }
                            }
                            else
                            {
                                if (bOverwrite)
                                {
                                    ViewModel.PlayViewModel oldPvm = parentVM.GetChild<ViewModel.PlayViewModel>(nameWindow.FileName);

                                    if (oldPvm != null)
                                    {
                                        parentVM.RemoveChild(oldPvm);
                                    }
                                }

                                File.Copy(playViewModel.Play.Path, strNewFile, bOverwrite);
                                if (File.Exists(playViewModel.Play.Path + ".PlayInfo"))
                                {//12-28-2009 scott
                                    File.Copy(playViewModel.Play.Path + ".PlayInfo", strNewFile + ".PlayInfo", bOverwrite);
                                    File.Delete(playViewModel.Play.Path + ".PlayInfo");
                                }
                                if (File.Equals(CurrentPath, playViewModel.Play.Path))
                                {
                                    CurrentPath = strNewFile;
                                }
                                parentVM.RemoveChild(playViewModel);
                                Play play = new Play(strNewFile);
                                ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, parentVM);
                                parentVM.AddChild(pViewModel);
                                this.borderFormation.DataContext = pViewModel;  // 09-21-2010 Scott
                            }
                        }
                    }
                }
                if ((int)MainMode == 0)
                {
                    folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;
                    if (folderViewModel != null)
                    {
                        ViewModel.TreeViewItemViewModel parentViewModel = folderViewModel.Parent;

                        NameWindow nameWindow = new NameWindow()
                        {
                            Title = "Rename Folder",
                            Owner = this,
                            SelectAll = true,
                            FileName = folderViewModel.Name,
                        };
                        if (nameWindow.ShowDialog().Value)
                        {
                            int index = folderViewModel.FolderPath.LastIndexOf(folderViewModel.Name);
                            string str = folderViewModel.FolderPath.Substring(0, index);
                            string strNewDir = str + nameWindow.FileName;
                            //string strNewDir = folderViewModel.FolderPath.Replace(folderViewModel.Name, nameWindow.FileName);
                            if (Directory.Exists(strNewDir))
                            {
                                MessageBox.Show("This folder already exists !");
                                return;
                            }
                            Directory.Move(folderViewModel.FolderPath, strNewDir);

                            folderViewModel.Parent.RemoveChild(folderViewModel);
                            ViewModel.FolderViewModel pViewModel = new Webb.Playbook.ViewModel.FolderViewModel(strNewDir, parentViewModel);
                            parentViewModel.AddChild(pViewModel);
                        }
                    }

                    formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;
                    if (formationViewModel != null)
                    {
                        ViewModel.TreeViewItemViewModel parentViewModel = formationViewModel.Parent;
                        NameWindow nameWindow = new NameWindow()
                        {
                            Title = "Rename " + GameSetting.Instance.GetField(scoutType, false),
                            Owner = this,
                            SelectAll = true,
                        };
                        nameWindow.FileName = formationViewModel.Formation.Name;
                        if (nameWindow.ShowDialog() == true)
                        {
                            bool bOverwrite = false;
                            string strNewFile = formationViewModel.Formation.Path.Replace(formationViewModel.Formation.Name + ".", nameWindow.FileName + ".");
                            if (File.Exists(strNewFile))
                            {
                                if (formationViewModel.Formation.Path == strNewFile)
                                {
                                    return;
                                }

                                if (MessageBox.Show("The " + GameSetting.Instance.GetField(scoutType, false) + " already exists, do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, false) + " ?", "Webb Playbbok", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (IOHelper.FileEqual(formationViewModel.Formation.Path, strNewFile))
                            {// 12-13-2011 Scott
                                ViewModel.FormationViewModel oldFvm = parentViewModel.GetChild<ViewModel.FormationViewModel>(nameWindow.FileName);

                                if (oldFvm != null)
                                {
                                    IOHelper.ChangeCaseForFileName(oldFvm.Formation.Path, strNewFile);
                                    IOHelper.MoveFolder(formationViewModel.PlayFolderPath, formationViewModel.PlayFolderPath.Replace(formationViewModel.FormationName + "@", nameWindow.FileName + "@"));
                                    oldFvm.Parent.Refresh();
                                }
                            }
                            else
                            {
                                if (bOverwrite)
                                {
                                    ViewModel.FormationViewModel oldFvm = parentViewModel.GetChild<ViewModel.FormationViewModel>(nameWindow.FileName);

                                    if (oldFvm != null)
                                    {
                                        parentViewModel.RemoveChild(oldFvm);
                                    }
                                }

                                File.Copy(formationViewModel.Formation.Path, strNewFile, bOverwrite);

                                Directory.Move(formationViewModel.PlayFolderPath, formationViewModel.PlayFolderPath.Replace(formationViewModel.FormationName + "@", nameWindow.FileName + "@"));   // 09-19-2010 Scott

                                if (File.Equals(CurrentPath, formationViewModel.Formation.Path))
                                {
                                    CurrentPath = strNewFile;
                                }
                                formationViewModel.Parent.RemoveChild(formationViewModel);
                                Formation formation = new Formation(strNewFile);
                                ViewModel.FormationViewModel pViewModel = new Webb.Playbook.ViewModel.FormationViewModel(formation, parentViewModel);
                                parentViewModel.AddChild(pViewModel);
                                this.borderFormation.DataContext = pViewModel;  // 09-21-2010 Scott
                            }
                        }
                    }

                    // 09-19-2010
                    playViewModel = this.treeFormation.SelectedItem as ViewModel.PlayViewModel;
                    if (playViewModel != null)
                    {
                        formationViewModel = playViewModel.Parent as ViewModel.FormationViewModel;
                        NameWindow nameWindow = new NameWindow()
                        {
                            Title = "Rename " + GameSetting.Instance.GetField(scoutType, true),
                            Owner = this,
                            SelectAll = true,
                        };
                        nameWindow.FileName = playViewModel.Play.Name;
                        if (nameWindow.ShowDialog() == true)
                        {
                            bool bOverwrite = false;
                            string strNewFile = playViewModel.Play.Path.Replace(playViewModel.Play.Name + ".", nameWindow.FileName + ".");
                            if (File.Exists(strNewFile))
                            {
                                if (playViewModel.Play.Path == strNewFile)
                                {
                                    return;
                                }

                                if (MessageBox.Show("The " + GameSetting.Instance.GetField(scoutType, true) + " already exists, do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, true) + " ?", "Webb Playbbok", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (IOHelper.FileEqual(playViewModel.Play.Path, strNewFile))
                            {// 12-13-2011 Scott
                                ViewModel.PlayViewModel oldPvm = formationViewModel.GetChild<ViewModel.PlayViewModel>(nameWindow.FileName);

                                if (oldPvm != null)
                                {
                                    IOHelper.ChangeCaseForFileName(oldPvm.Play.Path, strNewFile);
                                    if (File.Exists(oldPvm.Play.Path + ".PlayInfo"))
                                    {//12-28-2009 scott
                                        IOHelper.ChangeCaseForFileName(oldPvm.Play.Path + ".PlayInfo", strNewFile + ".PlayInfo");
                                    }
                                    oldPvm.Parent.Refresh();
                                }
                            }
                            else
                            {
                                if (bOverwrite)
                                {
                                    ViewModel.PlayViewModel oldPvm = formationViewModel.GetChild<ViewModel.PlayViewModel>(nameWindow.FileName);

                                    if (oldPvm != null)
                                    {
                                        formationViewModel.RemoveChild(oldPvm);
                                    }
                                }

                                File.Copy(playViewModel.Play.Path, strNewFile, bOverwrite);
                                if (File.Exists(playViewModel.Play.Path + ".PlayInfo"))
                                {//12-28-2009 scott
                                    File.Copy(playViewModel.Play.Path + ".PlayInfo", strNewFile + ".PlayInfo");
                                    File.Delete(playViewModel.Play.Path + ".PlayInfo");
                                }
                                if (File.Equals(CurrentPath, playViewModel.Play.Path))
                                {
                                    CurrentPath = strNewFile;
                                }
                                playViewModel.Parent.RemoveChild(playViewModel);
                                Play play = new Play(strNewFile);
                                ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, formationViewModel);
                                formationViewModel.AddChild(pViewModel);
                                this.borderFormation.DataContext = pViewModel;  // 09-21-2010 Scott
                            }
                        }
                    }
                }
            }
        }

        public ScoutTypes GetScoutType(ViewModel.TreeViewItemViewModel tvm)
        {
            if (tvm == null)
            {
                return ScoutTypes.Offensive;
            }

            ViewModel.TreeViewItemViewModel tvivm = tvm;

            while (true)
            {
                if (tvivm is ViewModel.ScoutTypeViewModel)
                {
                    break;
                }

                tvivm = tvivm.Parent;
            }

            if (tvivm is ViewModel.ScoutTypeViewModel)
            {
                return (tvivm as ViewModel.ScoutTypeViewModel).ScoutType.ScoutType;
            }
            else
            {
                return ScoutTypes.Offensive;
            }
        }

        public void Delete()
        {
            if ((int)MainMode == 0)
            {
                if (this.treeFormation.SelectedItem is ViewModel.FolderViewModel)
                {
                    Delete(true);

                    return;
                }
            }

            Delete(false);
        }

        public void New()
        {
            if ((int)MainMode == 0)
            {
                if (this.treeFormation.SelectedItem is ViewModel.FolderViewModel || this.treeFormation.SelectedItem is ViewModel.ScoutTypeViewModel)
                {
                    NewFormation();

                    return;
                }

                if (this.treeFormation.SelectedItem is ViewModel.FormationViewModel)
                {
                    CreatePlay();

                    return;
                }

                if (this.treeFormation.SelectedItem is ViewModel.TreeViewItemViewModel)
                {
                    ViewModel.TreeViewItemViewModel tvivm = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;
                    ScoutTypes scoutType = GetScoutType(tvivm);

                    if (this.treeFormation.SelectedItem is ViewModel.PlayViewModel)
                    {
                        MessageBox.Show(string.Format("You can not create a new {0} on a {1}",
                            GameSetting.Instance.GetField(scoutType, true),
                            GameSetting.Instance.GetField(scoutType, true)), "Webb Playbook");

                        return;
                    }
                }
            }

            if ((int)MainMode == 2)
            {
                if (this.treePlaybook.SelectedItem is ViewModel.PlayTypeViewModel || this.treePlaybook.SelectedItem is ViewModel.PlayFolderViewModel)
                {
                    NewPlay();

                    return;
                }

                if (this.treePlaybook.SelectedItem is ViewModel.TreeViewItemViewModel)
                {
                    if (this.treePlaybook.SelectedItem is ViewModel.PlayViewModel)
                    {
                        MessageBox.Show("You can not create a new play on a play", "Webb Playbook");

                        return;
                    }
                }
            }
        }

        public void Delete(bool bFolder)
        {
            if (Drawing != null)
            {
                IEnumerable<Webb.Playbook.Geometry.Shapes.IFigure> figures = Drawing.GetSelectedFigures();

                if (figures != null && figures.Count() > 0)
                {
                    Drawing.Delete();

                    return;
                }
            }

            ViewModel.PersonnelViewModel personnelViewModel = null;
            ViewModel.FormationViewModel formationViewModel = null;
            ViewModel.PlayViewModel playViewModel = null;
            ViewModel.FolderViewModel folderViewModel = null;
            ViewModel.TitleViewModel titleViewModel = null;

            if ((int)MainMode == 0)
            {
                personnelViewModel = this.treeFormation.SelectedItem as ViewModel.PersonnelViewModel;

                if (personnelViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", personnelViewModel.PersonnelName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        personnelViewModel.Parent.RemoveChild(personnelViewModel);
                    }
                }

                formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

                if (formationViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", formationViewModel.FormationName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        formationViewModel.Parent.RemoveChild(formationViewModel);

                        if (File.Equals(formationViewModel.Formation.Path, CurrentPath))
                        {
                            CloseDrawing();

                            WorkState = WorkState.FormationBlank;
                        }
                    }
                }

                titleViewModel = this.treeFormation.SelectedItem as ViewModel.TitleViewModel;

                if (titleViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", titleViewModel.TitleName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        titleViewModel.Parent.RemoveChild(titleViewModel);

                        if (File.Equals(titleViewModel.TitlePath, CurrentPath))
                        {
                            CloseDrawing();

                            WorkState = WorkState.FormationBlank;
                        }
                    }
                }

                if (bFolder)
                {
                    folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;

                    if (folderViewModel != null)
                    {
                        if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", folderViewModel.Name), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            folderViewModel.Parent.RemoveChild(folderViewModel);
                            //need improve
                            if (CurrentPath.Contains(folderViewModel.FolderPath))
                            {
                                CloseDrawing();
                            }
                        }
                    }
                }

                // 09-19-2010 Scott
                playViewModel = this.treeFormation.SelectedItem as ViewModel.PlayViewModel;
                if (playViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", playViewModel.PlayName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        playViewModel.Parent.RemoveChild(playViewModel);

                        if (File.Equals(playViewModel.Play.Path, CurrentPath))
                        {
                            CloseDrawing();

                            WorkState = WorkState.FormationBlank;
                        }
                    }
                }
            }
            else if ((int)MainMode == 1)
            {
                // 09-13-2010 Scott
                playViewModel = this.treeAdjustment.SelectedItem as ViewModel.PlayViewModel;

                if (playViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", playViewModel.PlayName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        playViewModel.Parent.RemoveChild(playViewModel);
                        if (File.Equals(playViewModel.Play.Path, CurrentPath))
                        {
                            CloseDrawing();

                            WorkState = WorkState.AdjustmentTypeBlank;
                        }
                    }
                }
            }
            else if ((int)MainMode == 2)
            {
                playViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayViewModel;

                if (playViewModel != null)
                {
                    if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", playViewModel.PlayName), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        playViewModel.Parent.RemoveChild(playViewModel);

                        if (File.Equals(playViewModel.Play.Path, CurrentPath))
                        {
                            CloseDrawing();

                            WorkState = WorkState.PlaybookBlank;
                        }
                    }
                }

                if (bFolder)
                {
                    ViewModel.PlayFolderViewModel playFolderViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayFolderViewModel;

                    if (playFolderViewModel != null)
                    {
                        if (MessageBox.Show(this, string.Format("Do you want to delete [{0}]?", playFolderViewModel.Name), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            playFolderViewModel.Parent.RemoveChild(playFolderViewModel);
                            //need improve
                            if (CurrentPath.Contains(playFolderViewModel.FolderPath))
                            {
                                CloseDrawing();
                            }
                        }
                    }
                }
            }
        }

        public void SaveAs()
        {
            if ((int)MainMode == 0)
            {
                ViewModel.FolderViewModel folderViewModel = null;
                ViewModel.ScoutTypeViewModel scoutTypeViewModel = null;
                ViewModel.FormationViewModel formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

                if (this.treeFormation.SelectedItem != null)
                {

                    ScoutTypes scoutType = GetScoutType(this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                    if (formationViewModel != null)
                    {
                        string strPath = string.Empty;

                        ViewModel.TreeViewItemViewModel parentViewModel = formationViewModel.Parent as ViewModel.TreeViewItemViewModel;
                        folderViewModel = parentViewModel as ViewModel.FolderViewModel;
                        if (folderViewModel != null)
                        {
                            strPath = folderViewModel.FolderPath;
                        }
                        scoutTypeViewModel = parentViewModel as ViewModel.ScoutTypeViewModel;
                        if (scoutTypeViewModel != null)
                        {
                            strPath = scoutTypeViewModel.ScoutTypePath;
                        }

                        if (strPath != string.Empty)
                        {
                            string strDestPath = string.Empty;
                            NameWindow nw = new NameWindow()
                            {
                                Title = GameSetting.Instance.GetField(scoutType, false) + " Save As",
                                Owner = this,
                                SelectAll = true,
                            };
                            nw.FileName = formationViewModel.Formation.Name;
                            if (nw.ShowDialog() == true)
                            {
                                strDestPath = strPath + @"\" + nw.FileName + @".Form";
                                bool bOverwrite = false;
                                if (File.Exists(strDestPath))
                                {
                                    if (formationViewModel.Formation.Path == strDestPath)
                                    {
                                        Save();
                                        return;
                                    }

                                    if (MessageBox.Show("This " + GameSetting.Instance.GetField(scoutType, false) + " already exists, do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, false) + " ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        bOverwrite = true;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }

                                if (bOverwrite)
                                {
                                    ViewModel.FormationViewModel oldFvm = formationViewModel.Parent.GetChild<ViewModel.FormationViewModel>(nw.FileName);

                                    if (oldFvm != null)
                                    {
                                        formationViewModel.Parent.RemoveChild(oldFvm);
                                    }
                                }

                                drawing.Save(strDestPath);

                                Formation formation = new Formation(strDestPath);
                                ViewModel.FormationViewModel fViewModel = new Webb.Playbook.ViewModel.FormationViewModel(formation, parentViewModel);
                                this.Load(strDestPath);
                                this.borderFormation.DataContext = formation;
                                formationViewModel.Parent.AddChild(fViewModel);
                                CopyDirectory.CopyFolderFiles(formationViewModel.PlayFolderPath, formationViewModel.PlayFolderPath.Replace(formationViewModel.FormationName + "@", nw.FileName + "@"), true, null);   // 11-16-2010 Scott
                                fViewModel.Refresh();

                                treeFormation_SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, fViewModel));

                                SaveImages();
                                SaveFreeDraw();
                            }
                        }
                    }

                    // 09-19-2010 Scott
                    formationViewModel = null;
                    ViewModel.PlayViewModel playViewModel = this.treeFormation.SelectedItem as ViewModel.PlayViewModel;

                    if (playViewModel != null)
                    {
                        formationViewModel = playViewModel.Parent as ViewModel.FormationViewModel;

                        if (formationViewModel != null)
                        {
                            string strDestPath = string.Empty;
                            NameWindow nw = new NameWindow()
                            {
                                Title = GameSetting.Instance.GetField(scoutType, true) + " Save As",
                                Owner = this,
                                SelectAll = true,
                            };
                            nw.FileName = playViewModel.Play.Name;
                            if (nw.ShowDialog() == true)
                            {
                                strDestPath = formationViewModel.PlayFolderPath + @"\" + nw.FileName + @".Play";
                                bool bOverwrite = false;
                                if (File.Exists(strDestPath))
                                {
                                    if (playViewModel.Play.Path == strDestPath)
                                    {
                                        Save();
                                        return;
                                    }

                                    if (MessageBox.Show("This " + GameSetting.Instance.GetField(scoutType, true) + " already exists, do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, true) + " ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        bOverwrite = true;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }

                                if (bOverwrite)
                                {
                                    ViewModel.PlayViewModel oldPvm = playViewModel.Parent.GetChild<ViewModel.PlayViewModel>(nw.FileName);

                                    if (oldPvm != null)
                                    {
                                        playViewModel.Parent.RemoveChild(oldPvm);
                                    }
                                }

                                drawing.Save(strDestPath);
                                if (File.Exists(playViewModel.Play.Path + ".PlayInfo"))
                                {// 01-05-2010 Scott
                                    File.Copy(playViewModel.Play.Path + ".PlayInfo", strDestPath + ".PlayInfo");
                                }
                                Play play = new Play(strDestPath);
                                ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, formationViewModel);
                                playViewModel.Parent.AddChild(pViewModel);
                                this.Load(strDestPath);
                                this.borderFormation.DataContext = play;
                            }
                        }
                    }
                }
            }
            else if ((int)MainMode == 2)
            {
                ViewModel.PlayTypeViewModel playTypeViewModel = null;
                ViewModel.PlayViewModel playViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayViewModel;

                if (playViewModel != null)
                {
                    string strDestPath = string.Empty;
                    NameWindow nw = new NameWindow()
                    {
                        Title = "Play Save As",
                        Owner = this,
                        SelectAll = true,
                    };
                    nw.FileName = playViewModel.Play.Name;
                    if (nw.ShowDialog() == true)
                    {
                        if (playViewModel.Parent is ViewModel.PlayFolderViewModel)
                        {
                            ViewModel.PlayFolderViewModel pfvm = playViewModel.Parent as ViewModel.PlayFolderViewModel;
                            strDestPath = pfvm.FolderPath + @"\" + nw.FileName + @".Play";
                        }
                        else if (playViewModel.Parent is ViewModel.PlayTypeViewModel)
                        {
                            ViewModel.PlayTypeViewModel ptvm = playViewModel.Parent as ViewModel.PlayTypeViewModel;
                            strDestPath = ptvm.PlayTypePath + @"\" + nw.FileName + @".Play";
                        }
                        else
                        {
                            strDestPath = string.Empty;
                        }

                        if (strDestPath != string.Empty)
                        {
                            bool bOverwrite = false;
                            if (File.Exists(strDestPath))
                            {
                                if (playViewModel.Play.Path == strDestPath)
                                {
                                    Save();
                                    return;
                                }

                                if (MessageBox.Show("This play already exists, do you want to overwrite the existing play ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (bOverwrite)
                            {
                                ViewModel.PlayViewModel oldPvm = playViewModel.Parent.GetChild<ViewModel.PlayViewModel>(nw.FileName);

                                if (oldPvm != null)
                                {
                                    playViewModel.Parent.RemoveChild(oldPvm);
                                }
                            }

                            drawing.Save(strDestPath);
                            if (File.Exists(playViewModel.Play.Path + ".PlayInfo"))
                            {// 01-05-2010 Scott
                                File.Copy(playViewModel.Play.Path + ".PlayInfo", strDestPath + ".PlayInfo", bOverwrite);
                            }
                            Play play = new Play(strDestPath);
                            ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, playViewModel.Parent);
                            playViewModel.Parent.AddChild(pViewModel);
                            this.Load(strDestPath);
                            this.borderFormation.DataContext = play;
                        }
                    }
                }
            }
        }

        public void NewFolder()
        {
            if ((int)MainMode == 0)
            {
                ViewModel.TreeViewItemViewModel treeViewItemViewModel;

                if (!SaveCurrentFile())
                    return;

                string path = string.Empty;

                treeViewItemViewModel = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;

                if (treeViewItemViewModel is ViewModel.ScoutTypeViewModel)
                {
                    path = (this.treeFormation.SelectedItem as ViewModel.ScoutTypeViewModel).ScoutTypePath;
                }
                else if (treeViewItemViewModel is ViewModel.FolderViewModel)
                {
                    path = (this.treeFormation.SelectedItem as ViewModel.FolderViewModel).FolderPath;
                }

                if (path != null && path != string.Empty)
                {
                    NameWindow nameWnd = new NameWindow()
                    {
                        Title = "New Folder",
                        Owner = this,
                    };

                    if (nameWnd.ShowDialog().Value)
                    {
                        string strPath = path + "\\" + nameWnd.FileName;
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                            ViewModel.FolderViewModel fvm = new Webb.Playbook.ViewModel.FolderViewModel(strPath, treeViewItemViewModel);
                            treeViewItemViewModel.AddChild(fvm);
                        }
                        else
                        {
                            MessageBox.Show("This folder already exists !");
                        }
                    }
                }
                else
                {
                    if (this.treeFormation.SelectedItem is ViewModel.TreeViewItemViewModel)
                    {
                        ViewModel.TreeViewItemViewModel tvivm = this.treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;
                        ScoutTypes scoutType = GetScoutType(tvivm);

                        if (this.treeFormation.SelectedItem is ViewModel.FormationViewModel)
                        {
                            MessageBox.Show(string.Format("You can not create a new folder on a {0}",
                                GameSetting.Instance.GetField(scoutType, false)), "Webb Playbook");

                            return;
                        }

                        if (this.treeFormation.SelectedItem is ViewModel.PlayViewModel)
                        {
                            MessageBox.Show(string.Format("You can not create a new folder on a {0}",
                                GameSetting.Instance.GetField(scoutType, true)), "Webb Playbook");

                            return;
                        }
                    }
                }
            }
            else if ((int)MainMode == 2)
            {
                ViewModel.TreeViewItemViewModel treeViewItemViewModel;

                if (!SaveCurrentFile())
                    return;

                string path = string.Empty;

                treeViewItemViewModel = this.treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;

                if (treeViewItemViewModel is ViewModel.PlayTypeViewModel)
                {
                    path = (this.treePlaybook.SelectedItem as ViewModel.PlayTypeViewModel).PlayTypePath;
                }
                else if (treeViewItemViewModel is ViewModel.PlayFolderViewModel)
                {
                    path = (this.treePlaybook.SelectedItem as ViewModel.PlayFolderViewModel).FolderPath;
                }

                if (path != null && path != string.Empty)
                {
                    NameWindow nameWnd = new NameWindow()
                    {
                        Title = "New Folder",
                        Owner = this,
                    };

                    if (nameWnd.ShowDialog().Value)
                    {
                        string strPath = path + "\\" + nameWnd.FileName;
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                            ViewModel.PlayFolderViewModel fvm = new Webb.Playbook.ViewModel.PlayFolderViewModel(strPath, treeViewItemViewModel);
                            treeViewItemViewModel.AddChild(fvm);
                        }
                        else
                        {
                            MessageBox.Show("This folder already exists !");
                        }
                    }
                }
                else
                {
                    if (this.treePlaybook.SelectedItem is ViewModel.TreeViewItemViewModel)
                    {
                        if (this.treePlaybook.SelectedItem is ViewModel.PlayViewModel)
                        {
                            MessageBox.Show("You can not create a new folder on a play", "Webb Playbook");

                            return;
                        }
                    }
                }
            }
        }

        public void UpdateTransparent(int scoutType)
        {
            foreach (Webb.Playbook.Geometry.Game.PBPlayer pbPlayer in Drawing.Figures.OfType<Webb.Playbook.Geometry.Game.PBPlayer>())
            {
                pbPlayer.Shape.Opacity = 1;

                if (scoutType == 0 && pbPlayer.ScoutType == 1 || scoutType == 1 && pbPlayer.ScoutType == 0)
                {
                    pbPlayer.Shape.Opacity = 0.3;
                }
            }

            Drawing.Figures.UpdateVisual();
        }

        void contextItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.OriginalSource as MenuItem;
            if (mi != null)
            {
                switch (mi.Header.ToString())
                {
                    case "New Adjustment":  // 09-03-2010 Scott
                        NewAdjustment();
                        break;

                    case "New Folder":
                        NewFolder();    //07-29-2010 Scott
                        break;

                    case "Copy":
                        Copy();
                        break;

                    case "Paste":
                        Paste();
                        break;

                    case "Edit":
                        Edit();
                        break;

                    case "Delete":
                        Delete(false);
                        break;

                    case "Delete Folder":
                        Delete(true);
                        break;

                    case "Open":
                        Open();
                        break;

                    case "Rename":
                    case "Rename Folder":
                        this.Rename();
                        break;

                    // 05-27-2011 Scott
                    case "New Title":
                        NewTitle();
                        break;
                    //case "New Formation":
                    //    NewFormation();
                    //    break;

                    case "New Play":
                        NewPlay();
                        break;

                    //case "Create Play":
                    //    CreatePlay();
                    //    break;
                    default:
                        break;
                }

                if (mi.Header.ToString() == "New " + GameSetting.Instance.OffensiveMainField
                    || mi.Header.ToString() == "New " + GameSetting.Instance.DefensiveMainField
                    || mi.Header.ToString() == "New " + GameSetting.Instance.KickMainField)
                {
                    NewFormation();
                }

                if (mi.Header.ToString() == "Create " + GameSetting.Instance.OffensiveSubField
                    || mi.Header.ToString() == "Create " + GameSetting.Instance.DefensiveSubField
                    || mi.Header.ToString() == "Create " + GameSetting.Instance.KickSubField)
                {
                    CreatePlay();
                }
            }
        }

        public void Edit()
        {
            PlayInfo playinfo = new PlayInfo();

            if (File.Exists(CurrentPath + ".PlayInfo"))
            {
                playinfo.Load(CurrentPath + ".PlayInfo");
                Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = playinfo.Mark;
                Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = PlaygroundStructure.GetHashLine(playinfo.HashLine);
            }

            PlayEditor playeditor = new PlayEditor(playinfo)
            {
                Title = "Edit Play",
                Owner = this,
            };

            if (playeditor.ShowDialog().Value)
            {
                Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = playinfo.Mark;

                if (playinfo.HashLine != HashLine.None)
                {
                    Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = PlaygroundStructure.GetHashLine(playinfo.HashLine);
                }
                playinfo.Save(CurrentPath + ".PlayInfo");
                Drawing.UpdatePlayground();
                Drawing.Figures.UpdateVisual();
            }
        }

        public void Copy()
        {// 11-04-2011 Scott
            ViewModel.FormationViewModel formationViewModel;
            ViewModel.PlayViewModel playViewModel;

            if ((int)MainMode == 2)
            {
                playViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayViewModel;

                if (playViewModel != null)
                {
                    currentCopyPlayPath = playViewModel.PlayPath;
                }
            }
            if ((int)MainMode == 0)
            {
                formationViewModel = this.treeFormation.SelectedItem as ViewModel.FormationViewModel;

                if (formationViewModel != null)
                {
                    currentCopyPlayPath = formationViewModel.FormationPath;
                }
            }
        }

        public void Paste()
        {// 11-04-2011 Scott
            if (File.Exists(currentCopyPlayPath))
            {
                ViewModel.PlayTypeViewModel playTypeViewModel;
                ViewModel.PlayFolderViewModel playFolderViewModel;
                ViewModel.ScoutTypeViewModel scoutTypeViewModel;
                ViewModel.FolderViewModel folderViewModel;

                ViewModel.TreeViewItemViewModel parentVM = null;
                string folderPath = string.Empty;

                if ((int)MainMode == 0 && currentCopyPlayPath.EndsWith(".Form",true,null))
                {
                    scoutTypeViewModel = this.treeFormation.SelectedItem as ViewModel.ScoutTypeViewModel;
                    folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;

                    if (scoutTypeViewModel != null)
                    {
                        folderPath = scoutTypeViewModel.ScoutTypePath;
                        parentVM = scoutTypeViewModel;
                    }

                    if (folderViewModel != null)
                    {
                        folderPath = folderViewModel.FolderPath;
                        parentVM = folderViewModel;
                    }

                    if (parentVM != null)
                    {
                        string strDestPath = folderPath + currentCopyPlayPath.Substring(currentCopyPlayPath.LastIndexOf('\\'));
                        int num = 1;
                        string strTempPath = strDestPath;
                        while (File.Exists(strDestPath))
                        {
                            int index = strTempPath.LastIndexOf('.');
                            strDestPath = strTempPath.Insert(index, string.Format(" {0:00}", num));
                            num++;
                        }
                        File.Copy(currentCopyPlayPath, strDestPath, true);
                        if (File.Exists(currentCopyPlayPath + ".BMP"))
                        {
                            File.Copy(currentCopyPlayPath + ".BMP", strDestPath + ".BMP", true);
                        }
                        if (File.Exists(currentCopyPlayPath + ".FD"))
                        {
                            File.Copy(currentCopyPlayPath + ".FD", strDestPath + ".FD", true);
                        }
                        // 11-04-2011 Scott
                        string strPlayFolderPath = currentCopyPlayPath.Remove(currentCopyPlayPath.IndexOf(".Form")) + "@";
                        string strDestPlayFolderPath = strDestPath.Remove(strDestPath.IndexOf(".Form")) + "@";
                        if (Directory.Exists(strDestPlayFolderPath))
                        {
                            Directory.Delete(strDestPlayFolderPath, true);
                        }
                        Directory.CreateDirectory(strDestPlayFolderPath);
                        if (Directory.Exists(strPlayFolderPath))
                        {
                            CopyDirectory.CopyFolderFiles(strPlayFolderPath, strDestPlayFolderPath, false, null);
                        }
                        // end
                        Formation formation = new Formation(strDestPath);
                        ViewModel.FormationViewModel fViewModel = new Webb.Playbook.ViewModel.FormationViewModel(formation, parentVM);
                        parentVM.AddChild(fViewModel);
                    }
                }

                if ((int)MainMode == 2 && currentCopyPlayPath.EndsWith(".Play", true, null))
                {
                    playTypeViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayTypeViewModel;
                    playFolderViewModel = this.treePlaybook.SelectedItem as ViewModel.PlayFolderViewModel;

                    if (playTypeViewModel != null)
                    {
                        folderPath = playTypeViewModel.PlayTypePath;
                        parentVM = playTypeViewModel;
                    }

                    if (playFolderViewModel != null)
                    {
                        folderPath = playFolderViewModel.FolderPath;
                        parentVM = playFolderViewModel;
                    }

                    if (parentVM != null)
                    {
                        string strDestPath = folderPath + currentCopyPlayPath.Substring(currentCopyPlayPath.LastIndexOf('\\'));
                        int num = 1;
                        string strTempPath = strDestPath;
                        while (File.Exists(strDestPath))
                        {
                            int index = strTempPath.LastIndexOf('.');
                            strDestPath = strTempPath.Insert(index, string.Format(" {0:00}", num));
                            num++;
                        }
                        File.Copy(currentCopyPlayPath, strDestPath, true);
                        if (File.Exists(currentCopyPlayPath + ".PlayInfo"))
                        {
                            File.Copy(currentCopyPlayPath + ".PlayInfo", strDestPath + ".PlayInfo", true);
                        }
                        if (File.Exists(currentCopyPlayPath + ".BMP"))
                        {
                            File.Copy(currentCopyPlayPath + ".BMP", strDestPath + ".BMP", true);
                        }
                        if (File.Exists(currentCopyPlayPath + ".FD"))
                        {
                            File.Copy(currentCopyPlayPath + ".FD", strDestPath + ".FD", true);
                        }
                        Play play = new Play(strDestPath);
                        ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, parentVM);
                        parentVM.AddChild(pViewModel);
                    }
                }
            }
        }

        public void NewPlay()
        {
            ViewModel.TreeViewItemViewModel parentViewModel = this.treePlaybook.SelectedItem as ViewModel.TreeViewItemViewModel;
            string strParentPath = string.Empty;

            if (!SaveCurrentFile())
                return;
            CloseDrawing();
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = 0;
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = 0;

            if (parentViewModel is ViewModel.PlayTypeViewModel)
            {
                strParentPath = (parentViewModel as ViewModel.PlayTypeViewModel).PlayTypePath;
            }
            if (parentViewModel is ViewModel.PlayFolderViewModel)
            {
                strParentPath = (parentViewModel as ViewModel.PlayFolderViewModel).FolderPath;
            }

            if (parentViewModel is ViewModel.PlayTypeViewModel || parentViewModel is ViewModel.PlayFolderViewModel)
            {
                NewPlay newPlay = new NewPlay(formationRootViewModel)
                {
                    Owner = this
                };

                if (newPlay.ShowDialog().Value)
                {
                    string strFile = strParentPath + @"\" + newPlay.PlayViewModel.PlayName + @".Play";
                    bool bOverwrite = false;
                    if (File.Exists(strFile))
                    {
                        if (MessageBox.Show("This play already exists, do you want to overwrite the existing play ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            bOverwrite = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (bOverwrite)
                    {
                        ViewModel.PlayViewModel oldPvm = parentViewModel.GetChild<ViewModel.PlayViewModel>(newPlay.PlayViewModel.PlayName);

                        if (oldPvm != null)
                        {
                            parentViewModel.RemoveChild(oldPvm);
                        }
                    }

                    ViewModel.FormationViewModel offensiveFormation = newPlay.PlayViewModel.ObjFormation;
                    ViewModel.FormationViewModel defensiveFormation = newPlay.PlayViewModel.OppFormation;
                    if (Drawing != null)
                    {
                        Drawing.Dispose();
                    }
                    Drawing = Draw.Load(offensiveFormation == null ? string.Empty : offensiveFormation.FormationPath, defensiveFormation == null ? string.Empty : defensiveFormation.FormationPath, this.canvasDrawing);
                    Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                    Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                    Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
                    Drawing.Save(strFile);

                    // 12-28-2009 Scott
                    PlayInfo pi = new PlayInfo()
                    {
                        OffensiveFormation = newPlay.PlayViewModel.ObjFormation == null ? string.Empty : newPlay.PlayViewModel.ObjFormation.FormationName,

                        DefensiveFormation = newPlay.PlayViewModel.OppFormation == null ? string.Empty : newPlay.PlayViewModel.OppFormation.FormationName,
                    };
                    pi.Save(strFile + ".PlayInfo");

                    Play play = new Play(strFile);
                    ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, parentViewModel);
                    parentViewModel.AddChild(pViewModel);
                    CurrentPath = pViewModel.PlayPath;
                    borderFormation.DataContext = pViewModel.Play;
                    parentViewModel.IsExpanded = true;

                    if (GameSetting.Instance.GridLine)
                    {
                        Drawing.ShowGridLines();
                    }

                    ClearSelectedTools();

                    WorkState = WorkState.AfterNewPlay;
                }
                else
                {
                    Drawing.ActionManager.Undo();
                }
            }

            Drawing.SetStartYardByBall();   // 01-20-2012 Scott
        }

        public void NewAdjustment()
        {
            ViewModel.AdjustmentTypeViewModel adjustmentTypeViewModel = null;

            if (!SaveCurrentFile())
                return;
            CloseDrawing();
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = 0;
            Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = 0;
            adjustmentTypeViewModel = this.treeAdjustment.SelectedItem as ViewModel.AdjustmentTypeViewModel;

            if (adjustmentTypeViewModel != null)
            {
                NewPlay newPlay = new NewPlay(formationRootViewModel)
                {
                    Owner = this,
                    Title = "New Adjustment",
                };

                if (newPlay.ShowDialog().Value)
                {
                    string strFile = adjustmentTypeViewModel.FolderPath + @"\" + newPlay.PlayViewModel.PlayName + @".Play";
                    if (File.Exists(strFile))
                    {
                        MessageBox.Show("This adjustment already exists !");
                        return;
                    }

                    ViewModel.FormationViewModel offensiveFormation = newPlay.PlayViewModel.ObjFormation;
                    ViewModel.FormationViewModel defensiveFormation = newPlay.PlayViewModel.OppFormation;
                    if (Drawing != null)
                    {
                        Drawing.Dispose();
                    }
                    Drawing = Draw.Load(offensiveFormation == null ? string.Empty : offensiveFormation.FormationPath, defensiveFormation == null ? string.Empty : defensiveFormation.FormationPath, this.canvasDrawing);
                    Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                    Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                    Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
                    Drawing.Save(strFile);

                    // 12-28-2009 Scott
                    PlayInfo pi = new PlayInfo()
                    {
                        OffensiveFormation = newPlay.PlayViewModel.ObjFormation == null ? string.Empty : newPlay.PlayViewModel.ObjFormation.FormationName,

                        DefensiveFormation = newPlay.PlayViewModel.OppFormation == null ? string.Empty : newPlay.PlayViewModel.OppFormation.FormationName,
                    };
                    pi.Save(strFile + ".PlayInfo");

                    Play play = new Play(strFile);
                    ViewModel.PlayViewModel pViewModel = new Webb.Playbook.ViewModel.PlayViewModel(play, adjustmentTypeViewModel);
                    adjustmentTypeViewModel.AddChild(pViewModel);
                    CurrentPath = pViewModel.PlayPath;
                    borderFormation.DataContext = pViewModel.Play;
                    adjustmentTypeViewModel.IsExpanded = true;

                    if (GameSetting.Instance.GridLine)
                    {
                        Drawing.ShowGridLines();
                    }

                    ClearSelectedTools();

                    UpdateTransparent((int)adjustmentTypeViewModel.AdjustmentTypeDirectory.ScoutType);  // 09-03-2010 Scott

                    WorkState = WorkState.AfterNewAdjustment;
                }
                else
                {
                    Drawing.ActionManager.Undo();
                }
            }
        }

        // 05-27-2011 Scott
        public void NewTitle()
        {
            ViewModel.TreeViewItemViewModel treeViewItemViewModel;
            ViewModel.ScoutTypeViewModel scoutTypeViewModel;
            ViewModel.FolderViewModel folderViewModel;

            if (!SaveCurrentFile())
                return;
            scoutTypeViewModel = this.treeFormation.SelectedItem as ViewModel.ScoutTypeViewModel;
            folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;

            treeViewItemViewModel = null;

            string sPath = string.Empty;

            if (scoutTypeViewModel != null)
            {
                sPath = scoutTypeViewModel.ScoutTypePath;
                treeViewItemViewModel = scoutTypeViewModel;
            }
            else if (folderViewModel != null)
            {
                sPath = folderViewModel.FolderPath;
                treeViewItemViewModel = folderViewModel;
            }

            if (treeViewItemViewModel != null)
            {
                NameWindow nw = new NameWindow()
                {
                    Title = "New Title",
                    Owner = this,
                    FileName = "New Title",
                };

                if (nw.ShowDialog().Value)
                {
                    sPath = sPath + @"\" + nw.FileName + ".TTL";

                    if (File.Exists(sPath))
                    {
                        if (MessageBox.Show("The title already exist, do you want to overwrite it ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            ViewModel.TreeViewItemViewModel oldVM = treeViewItemViewModel.GetChild<ViewModel.TitleViewModel>(nw.textName.Text);

                            if (oldVM != null)
                            {
                                treeViewItemViewModel.RemoveChild(oldVM);
                            }
                        }
                    }

                    Webb.Playbook.Data.Title title = new Title(sPath);

                    ViewModel.TitleViewModel tvm = new Webb.Playbook.ViewModel.TitleViewModel(title, treeViewItemViewModel);

                    CreateDrawing();
                    Drawing.Title = true;
                    Drawing.Save(sPath);

                    Drawing.LoadTitleBackground();

                    CurrentPath = tvm.TitlePath;    // 11-10-2011 Scott

                    treeViewItemViewModel.AddChild(tvm);

                    WorkState = WorkState.AfterNewTitle;
                }
            }
        }

        public void NewFormation()
        {
            ViewModel.TreeViewItemViewModel treeViewItemViewModel;
            ViewModel.ScoutTypeViewModel scoutTypeViewModel;
            ViewModel.FolderViewModel folderViewModel;

            if (!SaveCurrentFile())
                return;
            scoutTypeViewModel = this.treeFormation.SelectedItem as ViewModel.ScoutTypeViewModel;
            folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;

            FormationCollect formationcollect = new FormationCollect();
            ValueCollect valuecollect = new ValueCollect();

            treeViewItemViewModel = null;

            string sPath = string.Empty;

            if (scoutTypeViewModel != null)
            {
                sPath = scoutTypeViewModel.ScoutTypePath;
                treeViewItemViewModel = scoutTypeViewModel;
            }
            else if (folderViewModel != null)
            {
                sPath = folderViewModel.FolderPath;
                treeViewItemViewModel = folderViewModel;
            }
            ScoutTypes scoutType = ScoutTypes.Offensive;
            if (scoutTypeViewModel != null)
            {
                scoutType = scoutTypeViewModel.ScoutType.ScoutType;
            }
            if (folderViewModel != null)
            {
                ViewModel.TreeViewItemViewModel sctVM = folderViewModel;
                while (!(sctVM is ViewModel.ScoutTypeViewModel))
                {
                    sctVM = sctVM.Parent;
                }
                scoutType = (sctVM as ViewModel.ScoutTypeViewModel).ScoutType.ScoutType;
            }
            if (scoutType == Data.ScoutTypes.Offensive)
            {
                string pathset = AppDomain.CurrentDomain.BaseDirectory + "Set";
                if (!System.IO.Directory.Exists(pathset))
                {
                    System.IO.Directory.CreateDirectory(pathset);

                }
                if (!System.IO.Directory.Exists(pathset + @"\Offense"))
                {
                    System.IO.Directory.CreateDirectory(pathset + @"\Offense");
                }
                foreach (string file in System.IO.Directory.GetFiles(pathset + @"\Offense"))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    formationcollect.Formations.Add(fi.Name);
                }

                valuecollect.Values.AddRange(Webb.Playbook.Data.Terminology.Instance.LoadValuesForFieldByType(GameSetting.Instance.OffensiveMainField, (int)GameSetting.Instance.UserType));
            }
            else if (scoutType == Data.ScoutTypes.Defensive)
            {
                string pathset = AppDomain.CurrentDomain.BaseDirectory + "Set";
                if (!System.IO.Directory.Exists(pathset))
                {
                    System.IO.Directory.CreateDirectory(pathset);
                }
                if (!System.IO.Directory.Exists(pathset + @"\Dffense"))
                {
                    System.IO.Directory.CreateDirectory(pathset + @"\Defense");
                }
                foreach (string file in System.IO.Directory.GetFiles(pathset + @"\Defense"))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    formationcollect.Formations.Add(fi.Name);
                }

                valuecollect.Values.AddRange(Webb.Playbook.Data.Terminology.Instance.LoadValuesForFieldByType(GameSetting.Instance.DefensiveMainField, (int)GameSetting.Instance.UserType));
            }
            else
            {// 10-26-2011 Scott
                string pathset = AppDomain.CurrentDomain.BaseDirectory + "Set";
                if (!System.IO.Directory.Exists(pathset))
                {
                    System.IO.Directory.CreateDirectory(pathset);
                }
                if (!System.IO.Directory.Exists(pathset + @"\Kicks"))
                {
                    System.IO.Directory.CreateDirectory(pathset + @"\Kicks");
                }
                foreach (string file in System.IO.Directory.GetFiles(pathset + @"\Kicks"))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    formationcollect.Formations.Add(fi.Name);
                }

                valuecollect.Values.AddRange(Webb.Playbook.Data.Terminology.Instance.LoadValuesForFieldByType(GameSetting.Instance.KickMainField, (int)GameSetting.Instance.UserType));
            }

            if (sPath != string.Empty)
            {
                NewFormation nameWnd = new NewFormation(formationcollect, valuecollect)
                {
                    Title = "New " + GameSetting.Instance.GetField(scoutType, false),
                    Owner = this,
                };
                nameWnd.FormationSelected += new Action<NewFormation>(pe_FomationSelected);
                if (nameWnd.ShowDialog().Value)
                {
                    string strFile = sPath + @"\" + nameWnd.txtName.Text + @".Form";
                    bool bOverwrite = false;
                    if (File.Exists(strFile))
                    {
                        if (MessageBox.Show("This " + GameSetting.Instance.GetField(scoutType, false) + " already exists, do you want to overwrite the existing " + GameSetting.Instance.GetField(scoutType, false) + " ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            bOverwrite = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (bOverwrite)
                    {
                        ViewModel.FormationViewModel oldFvm = treeViewItemViewModel.GetChild<ViewModel.FormationViewModel>(nameWnd.txtName.Text);

                        if (oldFvm != null)
                        {
                            treeViewItemViewModel.RemoveChild(oldFvm);
                        }
                    }

                    CreateDrawing();

                    //add default formation to the playground

                    if (nameWnd.cmbset.Text != string.Empty)
                    {
                        if (scoutType == Data.ScoutTypes.Offensive)
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Offense\" + nameWnd.cmbset.Text;
                            if (System.IO.File.Exists(file))
                            {
                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }

                                drawing = Draw.Load(file, this.canvasDrawing);
                            }
                        }
                        else if (scoutType == Data.ScoutTypes.Defensive)
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Defense\" + nameWnd.cmbset.Text;
                            if (System.IO.File.Exists(file))
                            {
                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }
                                drawing = Draw.Load(file, this.canvasDrawing);
                            }
                        }
                        else
                        {// 10-26-2011 Scott
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Kicks\" + nameWnd.cmbset.Text;
                            if (System.IO.File.Exists(file))
                            {
                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }
                                Drawing = Draw.Load(file, this.canvasDrawing);
                            }
                        }
                    }
                    else
                    {
                        if (scoutType == ScoutTypes.Kicks)
                        { // 10-27-2011 Scott
                            string strKickBaseFormation = AppDomain.CurrentDomain.BaseDirectory + "KicksFormation.Form";
                            if (File.Exists(strKickBaseFormation))
                            {
                                Drawing = Draw.Load(strKickBaseFormation, this.canvasDrawing);
                            }
                        }
                        else
                        {
                            PlayersCreator pc = new PlayersCreator();
                            Personnel personnel = new Personnel();
                            personnel.ScoutType = scoutType;
                            personnel.Positions.Clear();
                            // 10-26-2011 Scott
                            IEnumerable<Position> positions = null;
                            switch (scoutType)
                            {
                                case ScoutTypes.Offensive:
                                    positions = PersonnelEditor.Setting.GetOffensePositions();
                                    break;
                                case ScoutTypes.Defensive:
                                    positions = PersonnelEditor.Setting.GetDefensePositions();
                                    break;
                                case ScoutTypes.Kicks:
                                    positions = PersonnelEditor.Setting.GetOffensePositions();
                                    break;
                            }
                            // end
                            personnel.Positions.AddRange(positions);
                            double scrimmage = -5;
                            switch (scoutType)
                            {
                                case ScoutTypes.Offensive:
                                    scrimmage = -5;
                                    break;
                                case ScoutTypes.Defensive:
                                    scrimmage = 5;
                                    break;
                                case ScoutTypes.Kicks:
                                    scrimmage = -5;
                                    break;
                            }
                            pc.CreatePlayers(Drawing, personnel, true, scrimmage);
                        }
                    }

                    Drawing.Save(strFile);

                    Formation formation = new Formation(strFile);
                    ViewModel.FormationViewModel fViewModel = new ViewModel.FormationViewModel(formation, treeViewItemViewModel);
                    treeViewItemViewModel.AddChild(fViewModel);
                    treeFormation_SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, fViewModel));
                    CurrentPath = fViewModel.FormationPath;
                    borderFormation.DataContext = fViewModel.Formation;
                    treeViewItemViewModel.IsExpanded = true;
                    treeFormation.InvalidateVisual();
                    fViewModel.IsSelected = true;

                    if (GameSetting.Instance.GridLine)
                    {
                        Drawing.ShowGridLines();
                    }

                    ClearSelectedTools();

                    WorkState = WorkState.AfterNewFormation;    //07-29-2010 Scott
                }
                else
                {
                    this.ClearWorkSpace();
                }
            }

            Drawing.SetStartYardByBall();   // 01-20-2012 Scott
        }

        void pe_FomationSelected(NewFormation newformation)
        {
            ViewModel.TreeViewItemViewModel treeViewItemViewModel;
            ViewModel.ScoutTypeViewModel scoutTypeViewModel;
            ViewModel.FolderViewModel folderViewModel;
            scoutTypeViewModel = this.treeFormation.SelectedItem as ViewModel.ScoutTypeViewModel;
            folderViewModel = this.treeFormation.SelectedItem as ViewModel.FolderViewModel;

            treeViewItemViewModel = null;

            string sPath = string.Empty;

            if (scoutTypeViewModel != null)
            {
                sPath = scoutTypeViewModel.ScoutTypePath;
                treeViewItemViewModel = scoutTypeViewModel;
            }
            else if (folderViewModel != null)
            {
                sPath = folderViewModel.FolderPath;
                treeViewItemViewModel = folderViewModel;
            }
            ScoutTypes scoutType = ScoutTypes.Offensive;
            if (scoutTypeViewModel != null)
            {
                scoutType = scoutTypeViewModel.ScoutType.ScoutType;
            }
            if (folderViewModel != null)
            {
                ViewModel.TreeViewItemViewModel sctVM = folderViewModel;
                while (!(sctVM is ViewModel.ScoutTypeViewModel))
                {
                    sctVM = sctVM.Parent;
                }
                scoutType = (sctVM as ViewModel.ScoutTypeViewModel).ScoutType.ScoutType;
            }
            if (newformation.cmbset.SelectedItem.ToString() != string.Empty)
            {
                if (scoutType == Data.ScoutTypes.Offensive)
                {
                    string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Offense\" + newformation.cmbset.SelectedItem.ToString();
                    if (System.IO.File.Exists(file))
                    {
                        if (Drawing != null)
                        {
                            Drawing.Dispose();
                        }
                        drawing = Draw.Load(file, this.canvasDrawing);

                    }
                }
                else if (scoutType == Data.ScoutTypes.Defensive)
                {
                    string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Defense\" + newformation.cmbset.SelectedItem.ToString();
                    if (System.IO.File.Exists(file))
                    {
                        if (Drawing != null)
                        {
                            Drawing.Dispose();
                        }
                        drawing = Draw.Load(file, this.canvasDrawing);
                    }
                }
                else
                { // 10-26-2011 Scott
                    string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Kicks\" + newformation.cmbset.SelectedItem.ToString();
                    if (System.IO.File.Exists(file))
                    {
                        if (Drawing != null)
                        {
                            Drawing.Dispose();
                        }
                        drawing = Draw.Load(file, this.canvasDrawing);
                    }
                }
            }
        }

        void Drawing_BehaviorChanged(Webb.Playbook.Geometry.Behavior oldObj, Webb.Playbook.Geometry.Behavior obj)
        {
            if (oldObj == Webb.Playbook.Geometry.Behavior.LookupByName("Text") || oldObj == Webb.Playbook.Geometry.Behavior.LookupByName("Pen"))
            {
                ToolManager.LookupByCommand(Commands.Dragger).Select();
                ToolManager.LookupByCommand(Commands.ToolDragger).Select();
            }

            if (obj is Webb.Playbook.Geometry.Selector || obj is Webb.Playbook.Geometry.Dragger || obj is Webb.Playbook.Geometry.PlaygroundDragger)
            {
                dockPanelTools.IsEnabled = true;
                tabControl.IsEnabled = true;
            }
            else
            {
                dockPanelTools.IsEnabled = false;
                tabControl.IsEnabled = false;
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainMode = (Mode)tabControl.SelectedIndex;
            switch (MainMode)
            {
                case Mode.Formation:
                    if (this.treeFormation.SelectedItem is ViewModel.PersonnelViewModel)
                    {
                        stackPanelDisplay.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackPanelDisplay.Visibility = Visibility.Collapsed;
                    }

                    //this.expanderTop.Visibility = Visibility.Visible;
                    //this.expanderBottom.Visibility = Visibility.Visible;
                    this.gridPlayerInfo.Visibility = Visibility.Collapsed;
                    break;
                case Mode.Playbook:
                    stackPanelDisplay.Visibility = Visibility.Collapsed;
                    //this.expanderTop.Visibility = Visibility.Visible;
                    //this.expanderBottom.Visibility = Visibility.Visible;
                    this.gridPlayerInfo.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void treeFormation_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.stackPanelDisplay.Visibility = Visibility.Hidden;

            if (e.NewValue is ViewModel.PersonnelViewModel)
            {
                ViewModel.PersonnelViewModel personnelViewModel = e.NewValue as ViewModel.PersonnelViewModel;

                this.stackPanelDisplay.DataContext = personnelViewModel.Personnel;

                this.stackPanelDisplay.Visibility = Visibility.Visible;
            }
        }

        private void treePlaybook_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.stackPanelDisplay.Visibility = Visibility.Hidden;
        }

        private void CreateDrawing()
        {
            if (Drawing != null)
            {
                Drawing.Dispose();
            }

            Drawing = new Draw(this.canvasDrawing);

            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
        }

        void Drawing_SelectionChanged(object sender, Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs e)
        {
            if (CurrentPath.EndsWith(".Play"))
            {
                if (e.SelectedFigures != null && e.SelectedFigures.Count() > 0)
                {
                    PBPlayer player = e.SelectedFigures.ElementAt(0) as PBPlayer;

                    selectedPlayer = player;

                    if (player != null && !player.PlaceHolder)
                    {
                        toolManagerPlayerActionsMenu.InitPlayerActionsMenu(player);
                        this.listBoxTop.SelectedItem = ToolManager.LookupByCommand(SelectActionCommand);
                        toolManagerSubMenu.InitSubMenu(player, SelectActionCommand, WorkState);
                        switch (SelectSubActionCommand)
                        {
                            case Commands.PostSnapMotion:
                            case Commands.EditPreSnapMotion:
                            case Commands.RunBlock:
                            case Commands.PassBlock:
                            case Commands.PassBlockArea:
                                this.listBoxBottom.SelectedItem = ToolManager.LookupByCommand(SelectSubActionCommand);
                                break;
                        }
                    }
                    else
                    {
                        toolManagerPlayerActionsMenu.Clear();
                        toolManagerSubMenu.Clear();
                    }
                }
                else
                {
                    selectedPlayer = null;

                    toolManagerPlayerActionsMenu.Clear();
                    toolManagerSubMenu.Clear();
                }

                UpdateSelectedPlayerInfo();
            }

            if (e.SelectedFigures != null && e.SelectedFigures.Count() > 0)
            {
                Webb.Playbook.Geometry.Shapes.PBLabel label = e.SelectedFigures.ElementAt(0) as Webb.Playbook.Geometry.Shapes.PBLabel;
                if (label != null)
                {// 04-27-2011 Scott
                    toolManagerSubMenu.InitSubMenu(label, SelectActionCommand, WorkState);
                    this.listBoxBottom.SelectedIndex = 0;
                }

                if (selectedPlayer == null && label == null)
                {
                    toolManagerPlayerActionsMenu.Clear();
                    toolManagerSubMenu.Clear();
                }
            }
            else
            {
                toolManagerPlayerActionsMenu.Clear();
                toolManagerSubMenu.Clear();
            }
        }

        public void UpdateSelectedPlayerInfo()
        {
            if (selectedPlayer != null)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("{0}: (Stance:{1}) ", selectedPlayer.Text, selectedPlayer.Stance);

                if (selectedPlayer != null)
                {
                    if (selectedPlayer.Name == "QB")
                    {
                        sb.AppendFormat("(Motion:{0}) ", selectedPlayer.QBMotion);

                        sb.AppendFormat("(Set:");
                        foreach (string setPlayer in selectedPlayer.SetArray)
                        {
                            Position pos = PersonnelEditor.Setting.GetOffensePositions().First(p => p.Name == setPlayer);

                            if (pos != null)
                            {
                                sb.Append(pos.Value);
                            }
                        }
                        sb.AppendFormat(") ");

                        sb.AppendFormat("(Assign:");
                        foreach (string assignPlayer in selectedPlayer.AssignArray)
                        {
                            Position pos = PersonnelEditor.Setting.GetOffensePositions().First(p => p.Name == assignPlayer);

                            if (pos != null)
                            {
                                sb.Append(pos.Value);
                            }
                        }
                        sb.AppendFormat(") ");

                        sb.AppendFormat("(Fake:");
                        foreach (string fakePlayer in selectedPlayer.FakeArray)
                        {
                            Position pos = PersonnelEditor.Setting.GetOffensePositions().FirstOrDefault(p => p.Name == fakePlayer);

                            if (pos != null)
                            {
                                sb.Append(pos.Value);
                            }
                        }
                        sb.AppendFormat(") ");
                    }
                    else if (selectedPlayer.ScoutType == (int)ScoutTypes.Defensive)
                    {
                        if (selectedPlayer.ManCoverage != string.Empty)
                        {
                            Position pos = PersonnelEditor.Setting.GetOffensePositions().FirstOrDefault(p => p.Name == selectedPlayer.ManCoverage);
                            if (pos != null)
                            {
                                sb.AppendFormat("(Man Coverage:{0}) ", pos.Value);
                            }
                        }
                        if (selectedPlayer.ManPress != string.Empty)
                        {
                            Position pos = PersonnelEditor.Setting.GetOffensePositions().FirstOrDefault(p => p.Name == selectedPlayer.ManPress);
                            if (pos != null)
                            {
                                sb.AppendFormat("(Man Press:{0}) ", pos.Value);
                            }
                        }
                    }
                }

                tbPlayerInfo.Text = sb.ToString();
            }
            else
            {
                tbPlayerInfo.Text = string.Empty;
            }
        }

        private void ClearSelectedTools()
        {
            this.listBoxTool.UnselectAll();
            toolManagerPlayerActionsMenu.Clear();
            toolManagerSubMenu.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public int SelectActionCommand = Commands.Assignments;
        public int SelectSubActionCommand = -1;

        void listBoxTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tool selectedTool = this.listBoxTop.SelectedItem as Tool;

            if (selectedTool != null)
            {
                toolManagerSubMenu.InitSubMenu(selectedPlayer, selectedTool.Command, WorkState);
                SelectActionCommand = selectedTool.Command;
            }
        }

        private void listBoxTop_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void listBoxTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Tool selectedTool = this.listBoxTop.SelectedItem as Tool;

            if (selectedTool != null)
            {
                int command = selectedTool.Command;

                switch (command)
                {
                    case Commands.ClearLines:
                        if (selectedTool != null)
                        {
                            ToolManager.EventHandler(Drawing, Drawing.GetSelectedFigure(), selectedTool);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void listBoxBottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tool selectedTool = this.listBoxBottom.SelectedItem as Tool;
            if (selectedTool != null)
            {
                int command = selectedTool.Command;
                SelectSubActionCommand = command;
                switch (command)
                {
                    case Commands.SetOffense:
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Offense\" + selectedTool.Text;
                            if (System.IO.File.Exists(file))
                            {
                                //Geometry.Lists.FigureList figrures = drawing.GetDefPBPlayer();
                                //Point balldef = drawing.GetBallPoint();

                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }
                                drawing = Draw.Load(file, this.canvasDrawing);
                                //  Point ballOff = drawing.GetBallPoint();
                                // drawing.MoveBall();
                                // drawing.Add(figrures);
                                // drawing.ResetDeffFigures(ballOff, balldef);
                            }
                            if (GameSetting.Instance.GridLine)
                            {
                                Drawing.ShowGridLines();
                            }
                            drawing.Figures.UpdateVisual();
                            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
                            Drawing.Save(CurrentPath);
                        }
                        break;

                    case Commands.SetDefense:
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Defense\" + selectedTool.Text;

                            if (System.IO.File.Exists(file))
                            {
                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }

                                drawing = Draw.Load(file, this.canvasDrawing);

                                if (GameSetting.Instance.GridLine)
                                {
                                    Drawing.ShowGridLines();
                                }
                            }
                            if (GameSetting.Instance.GridLine)
                            {
                                Drawing.ShowGridLines();
                            }
                            drawing.Figures.UpdateVisual();
                            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
                            Drawing.Save(CurrentPath);
                        }
                        break;
                    case Commands.SetKick:
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Kicks\" + selectedTool.Text;

                            if (System.IO.File.Exists(file))
                            {
                                if (Drawing != null)
                                {
                                    Drawing.Dispose();
                                }

                                drawing = Draw.Load(file, this.canvasDrawing);

                                if (GameSetting.Instance.GridLine)
                                {
                                    Drawing.ShowGridLines();
                                }
                            }
                            if (GameSetting.Instance.GridLine)
                            {
                                Drawing.ShowGridLines();
                            }
                            drawing.Figures.UpdateVisual();
                            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);
                            Drawing.Save(CurrentPath);
                        }
                        break;
                    case Commands.ConvertDefToSet:
                        return;
                    case Commands.ConvertOffToSet:
                        return;
                    case Commands.ConvertKickToSet:
                        return;
                }
            }
        }

        private void listBoxBottom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)
                {
                    ToolManager.EventHandler(Drawing, Drawing.GetSelectedFigure(), selectedTool);
                }

                UpdateSelectedPlayerInfo();
            }
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)
                {
                    switch (selectedTool.Command)
                    {
                        case Commands.Zone:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Zones\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.Route:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Routes\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.RouteAction:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.RouteBlock:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Blocks\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.RouteStuntBlitz:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Stunt Blitz\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.TextCoachingPoints:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "CoachingPoints\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.RoutePreSnapMotion:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "PreSnapMotions\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.Label:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Labels\\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.SetDefense:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Defense\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.SetOffense:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Offense\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                        case Commands.SetKick:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Kicks\" + selectedTool.Text;

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                    }

                }
            }

        }

        private void listBoxTool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxTool_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.listBoxTop.SelectedIndex >= 0)
            {
                this.listBoxTop.UnselectAll();
            }
            if (this.listBoxBottom.SelectedIndex >= 0)
            {
                this.listBoxBottom.UnselectAll();
            }
            this.toolManagerSubMenu.Clear();
        }

        private void listBoxTool_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)
                {
                    switch (selectedTool.Command)
                    {
                        case Commands.AddPosition:
                            {
                                PersonnelEditor pe = new PersonnelEditor(false);
                                if (MainMode == Mode.Formation) // 11-04-2010 Scott
                                {
                                    ViewModel.TreeViewItemViewModel selectedVM = treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel;

                                    if (selectedVM != null)
                                    {
                                        ScoutTypes scoutType = GetScoutType(selectedVM);

                                        if (scoutType == ScoutTypes.Offensive)
                                        {
                                            pe.OffensiveOnly = true;
                                        }
                                        if (scoutType == ScoutTypes.Defensive)
                                        {
                                            pe.DefensiveOnly = true;
                                        }
                                        if (scoutType == ScoutTypes.Kicks)
                                        {// 10-26-2011 Scott
                                            pe.OffensiveOnly = true;
                                        }
                                    }
                                }
                                pe.PositionSelected += new Action<PersonnelEditor, Position>(pe_PositionSelected);
                                pe.Owner = this;
                                if (!pe.ShowDialog().Value)
                                {// cancel add position
                                    for (int i = 0; i < pe.AddCount; i++)
                                    {
                                        Drawing.ActionManager.Undo();
                                    }
                                }
                                Drawing.Figures.UpdateVisual();
                            }
                            break;
                        case Commands.RemovePosition:
                            if (Drawing.GetSelectedPlayer() == null)
                            {
                                MessageBox.Show("Please select a player.");

                                int i = 2;

                                i = 2;
                            }
                            else
                            {
                                Drawing.Delete();
                            }
                            break;
                        case Commands.SubstitutePosition:
                            {
                                PBPlayer player = Drawing.GetSelectedPlayer();

                                if (player != null)
                                {
                                    PersonnelEditor pe = new PersonnelEditor(false);
                                    if (player.ScoutType == 0)
                                    {
                                        pe.OffensiveOnly = true;
                                    }
                                    if (player.ScoutType == 1)
                                    {
                                        pe.DefensiveOnly = true;
                                    }
                                    if (player.ScoutType == 2)
                                    {// 10-26-2011 Scott
                                        pe.OffensiveOnly = true;
                                    }
                                    pe.PositionSelected += new Action<PersonnelEditor, Position>(pe_PositionSubstitute);
                                    pe.Owner = this;
                                    if (!pe.ShowDialog().Value)
                                    {// cancel add position
                                        for (int i = 0; i < pe.AddCount; i++)
                                        {
                                            Drawing.ActionManager.Undo();
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please select a player.");
                                }
                            }
                            break;
                        default:
                            ToolManager.EventHandler(Drawing, selectedPlayer, selectedTool);
                            break;
                    }
                }
            }
        }

        private void listBoxTop_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.listBoxTool.SelectedIndex >= 0)
            {
                this.listBoxTool.UnselectAll();
            }
        }

        private void canvasDrawing_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedPlayerInfo();
        }

        private void ResetBehavior()
        {
            if (Drawing.Behavior == Webb.Playbook.Geometry.Behavior.LookupByName("Dragger"))
            {
                Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Selector");
                Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Dragger");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tool tool = (sender as Button).Tag as Tool;

            tool.Select();

            if (tool != null)
            {
                if (!tool.Enable)
                {
                    MessageBox.Show(string.Format("You can't use {0} in this version", tool.Text));

                    return;
                }

                switch (tool.Command)
                {
                    case Commands.LoadBaseFormation:
                        if (MainMode == Mode.Formation)
                        {
                            Drawing.RemovePlaceHolder();

                            ScoutTypes scoutType = GetScoutType(treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                            if (scoutType == ScoutTypes.Kicks)
                            {
                                return;
                            }

                            Drawing.Save(CurrentPath);

                            if (Drawing != null)
                            {
                                Drawing.Dispose();
                            }

                            Drawing = Draw.Load(scoutType == ScoutTypes.Offensive ? CurrentPath : AppDomain.CurrentDomain.BaseDirectory + @"\" + ScoutTypes.Offensive.ToString() + "Formation.Form",
                                                scoutType == ScoutTypes.Defensive ? CurrentPath : AppDomain.CurrentDomain.BaseDirectory + @"\" + ScoutTypes.Defensive.ToString() + "Formation.Form",
                                                canvasDrawing, (int)scoutType);

                            Drawing.SelectionChanged += new EventHandler<Webb.Playbook.Geometry.Drawing.SelectionChangedEventArgs>(Drawing_SelectionChanged);
                            Drawing.BehaviorChanged += new Action<Webb.Playbook.Geometry.Behavior, Webb.Playbook.Geometry.Behavior>(Drawing_BehaviorChanged);
                            Drawing.Playground.CoordinateSystemUpdated += new Action(CoordinateSystemUpdated);

                            if (GameSetting.Instance.GridLine)
                            {
                                Drawing.ShowGridLines();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please choose a formation or a play.");
                        }
                        break;
                    case Commands.SaveBaseFormation:
                        if (MainMode == Mode.Formation && CurrentPath.EndsWith(".Form"))
                        {
                            ScoutTypes scoutType = GetScoutType(treeFormation.SelectedItem as ViewModel.TreeViewItemViewModel);

                            if (scoutType == ScoutTypes.Kicks)
                            {
                                return;
                            }

                            Drawing.Save(AppDomain.CurrentDomain.BaseDirectory + @"\" + scoutType.ToString() + "Formation.Form");

                            SaveBasFormationImages(scoutType);

                            MessageBox.Show(string.Format("This formation was saved as {0} base formation", scoutType.ToString()));
                        }
                        else
                        {
                            MessageBox.Show("Please choose a formation.");
                        }
                        break;
                    case Commands.New:
                        New();
                        break;
                    case Commands.NewFolder:
                        NewFolder();
                        break;
                    case Commands.SaveAs:
                        SaveAs();
                        break;
                    case Commands.Redo:
                        Drawing.ActionManager.Redo();
                        break;
                    case Commands.Undo:
                        Drawing.ActionManager.Undo();
                        break;
                    case Commands.Delete:
                        Delete();
                        break;
                    case Commands.Rename:
                        Rename();
                        break;
                    case Commands.Personnel:
                        this.btnPersonnel_Click(null, null);
                        break;
                    case Commands.Team:
                        this.btnTeam_Click(null, null);
                        break;
                    case Commands.Open:
                        this.Open();
                        break;
                    case Commands.Save:
                        this.Save();
                        break;
                    case Commands.Gridlines:
                        this.btnGridLine_Click(null, null);
                        break;
                    case Commands.Snap:
                        SnapSettingWindow ssw = new SnapSettingWindow()
                        {
                            Owner = this,
                            DataContext = GameSetting.Instance,
                        };
                        if (ssw.ShowDialog().Value)
                        {

                        }
                        break;
                    case Commands.HelpTopics:
                        // 08-16-2011 Scott
                        ContextMenu cmHelp = new ContextMenu();
                        cmHelp.Items.Clear();
                        int count = miHelp.Items.Count;
                        for (int i = 0; i < count; i++)
                        {
                            MenuItem mi = new MenuItem();
                            object o = miHelp.Items[0];
                            miHelp.Items.Remove(o);
                            cmHelp.Items.Add(o);
                        }
                        cmHelp.Closed += new RoutedEventHandler(cmHelp_Closed);
                        cmHelp.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        cmHelp.IsOpen = true;
                        //this.HelpTopics_Click(null, null);
                        break;
                    case Commands.Ball:
                        this.btnBall_Click(null, null);
                        break;
                    case Commands.ThreeDView:
                        this.btnOpenGame_Click(null, null);
                        break;
                    case Commands.Animation:
                        string tempPath = AppDomain.CurrentDomain.BaseDirectory + @"\TempGame";
                        Drawing.TempSave(tempPath);
                        AnimationWindow aw = new AnimationWindow(tempPath);
                        aw.Owner = this;
                        aw.ShowDialog();
                        ResetBehavior();
                        break;
                    case Commands.Presentation:
                        Presentation.CreatePresentationWindow cpw = new Webb.Playbook.Presentation.CreatePresentationWindow(formationRootViewModel, playbookRootViewModel);
                        cpw.Owner = this;
                        cpw.ShowDialog();
                        ResetBehavior();
                        break;
                    case Commands.Text:
                        ToolListWindow tlw = new ToolListWindow(Drawing)
                        {
                            Owner = this,
                        };
                        if (tlw.ShowDialog().Value)
                        {
                            Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Text");
                        }
                        else
                        {

                        }
                        break;
                    case Commands.ToolDragger:
                        Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Dragger");
                        break;
                    case Commands.Print:
                        PrintDirectly();
                        break;
                    case Commands.Report:
                        miReport_Click(null, null);
                        break;
                    case Commands.HReverse:
                        btnHReverse_Click(null, null);
                        break;
                    case Commands.OffHreverse:
                        btnOffHReverse(null, null);
                        break;
                    case Commands.DeffHreverse:
                        btnDefHReverse(null, null);
                        break;
                    case Commands.VReverse:
                        btnVReverse_Click(null, null);
                        break;
                    case Commands.Solid:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.DashType = Webb.Playbook.Geometry.Shapes.DashType.Solid;
                            SelectPen();
                            break;
                        }
                    case Commands.Dashed:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.DashType = Webb.Playbook.Geometry.Shapes.DashType.Dashed;
                            SelectPen();
                            break;
                        }
                    case Commands.Dotted:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.DashType = Webb.Playbook.Geometry.Shapes.DashType.Dotted;
                            SelectPen();
                            break;
                        }
                    case Commands.BeeLine:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.LineType = Webb.Playbook.Geometry.Shapes.LineType.BeeLine;
                            SelectPen();
                            break;
                        }
                    case Commands.JaggedLine:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.LineType = Webb.Playbook.Geometry.Shapes.LineType.JaggedLine;
                            SelectPen();
                            break;
                        }
                    case Commands.CurvyLine:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.LineType = Webb.Playbook.Geometry.Shapes.LineType.CurvyLine;
                            SelectPen();
                            break;
                        }
                    case Commands.Color:
                        if (tool.Color is SolidColorBrush)
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.Color = (tool.Color as SolidColorBrush).Color;
                        }
                        break;
                    case Commands.EndTypeNone:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.CapType = Webb.Playbook.Geometry.Shapes.CapType.None;
                            SelectPen();
                            break;
                        }
                    case Commands.EndTypeArrow:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.CapType = Webb.Playbook.Geometry.Shapes.CapType.Arrow;
                            SelectPen();
                            break;
                        }
                    case Commands.EndTypeBlock:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.CapType = Webb.Playbook.Geometry.Shapes.CapType.BlockArea;
                            SelectPen();
                            break;
                        }
                    case Commands.EndTypeBlockPerson:
                        {
                            Webb.Playbook.Geometry.Shapes.Factory.CapType = Webb.Playbook.Geometry.Shapes.CapType.BlockPerson;
                            SelectPen();
                            break;
                        }
                    case Commands.Pen:
                        {
                            if (Drawing.Behavior is Webb.Playbook.Geometry.Pen)
                            {
                                sliderLineSize.Value = Webb.Playbook.Geometry.Shapes.Factory.StrokeThickness;
                                cmLineSize.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                                cmLineSize.PlacementTarget = sender as Button;
                                if (!cmLineSize.Items.Contains(sliderLineSize))
                                {
                                    cmLineSize.Items.Add(sliderLineSize);
                                }
                                (sender as Button).ContextMenu = cmLineSize;
                                cmLineSize.IsOpen = true;

                                sliderLineSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderSize_ValueChanged);
                            }
                            else
                            {
                                Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Pen");
                            }
                            break;
                        }
                }
            }
        }

        // 08-16-2011 Scott
        void cmHelp_Closed(object sender, RoutedEventArgs e)
        {
            ContextMenu cmHelp = sender as ContextMenu;
            miHelp.Items.Clear();
            int count = cmHelp.Items.Count;
            for (int i = 0; i < count; i++)
            {
                MenuItem mi = new MenuItem();
                object o = cmHelp.Items[0];
                cmHelp.Items.Remove(o);
                miHelp.Items.Add(o);
            }
        }

        public void SelectPen() //08-05-2011
        {
            ToolManager.LookupByCommand(Commands.Pen).Select();
            Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Pen");
        }

        ContextMenu cmLineSize = new ContextMenu();
        Slider sliderLineSize = new Slider()
        {
            Value = 3,
            Minimum = 1,
            Maximum = 5,
            IsSnapToTickEnabled = true,
            TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
            TickFrequency = 0.5,
            ToolTip = "Thickness",
            IsTabStop = false,
            Orientation = Orientation.Vertical,
            Width = 25,
            Height = 100,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
            AutoToolTipPrecision = 1,
            AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
            IsDirectionReversed = false,
            IsMoveToPointEnabled = false,
        };

        void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Webb.Playbook.Geometry.Shapes.Factory.StrokeThickness = e.NewValue;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scale(e.NewValue);
        }

        private void Scale(double scale)
        {
            if (Drawing != null)
            {
                int nSpace = (int)(20 + (100 - scale) / 50 * 100);
                double width = gridDrawingContainer.ActualWidth - 2 * nSpace;
                Point pt = gridDrawingContainer.TranslatePoint(new Point(gridDrawingContainer.ActualWidth / 2, gridDrawingContainer.ActualHeight / 2), canvasDrawing);

                double offsetX = width / canvasDrawing.ActualWidth * (canvasDrawing.ActualWidth / 2 - pt.X);
                double offsetY = width / canvasDrawing.ActualWidth * (gridDrawingContainer.ActualHeight / 2 - pt.Y);

                canvasDrawing.Margin = new Thickness(nSpace + offsetX, offsetY, nSpace - offsetX, -offsetY); // 01-14-2011 Scott

                //Point pt = gridDrawingContainer.TranslatePoint(new Point(gridDrawingContainer.ActualWidth / 2, gridDrawingContainer.ActualHeight / 2), canvasDrawing);
                //canvasDrawing.RenderTransform = new ScaleTransform(scale / 60, scale / 60, pt.X, pt.Y);
            }
        }

        private bool enableWindow;
        public bool EnableWindow
        {
            get { return enableWindow; }
            set
            {
                enableWindow = value;
                if (enableWindow)
                {
                    gridTopmost.Visibility = Visibility.Hidden;
                }
                else
                {
                    gridTopmost.Visibility = Visibility.Visible;
                }
            }
        }

        private void listBoxAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxAction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)  // 10-27-2010 Scott
                {
                    switch (selectedTool.Command)
                    {
                        case Commands.Save:
                            this.Save();
                            break;
                        case Commands.SaveAs:
                            this.SaveAs();
                            break;
                        case Commands.Delete:
                            this.Delete();
                            break;
                        case Commands.Rename:
                            this.Rename();
                            break;
                        case Commands.NewFolder:    //07-29-2010 Scott
                            NewFolder();
                            break;
                        case Commands.NewFormation: //07-29-2010 Scott
                            NewFormation();
                            break;
                        case Commands.NewAdjustment:
                            NewAdjustment();    //09-13-2010 Scott
                            break;
                        case Commands.NewPlay:
                            NewPlay();
                            break;
                        case Commands.Undo: // 08-02-2010 Scott
                            drawing.ActionManager.Undo();
                            break;
                        case Commands.Redo: // 08-02-2010 Scott
                            drawing.ActionManager.Redo();
                            break;
                        case Commands.CreatePlay:
                            CreatePlay();
                            break;
                        default:
                            break;
                    }

                    UpdateSelectedPlayerInfo();
                }
            }
        }

        // 09-20-2010 Scott
        void pe_PositionSubstitute(PersonnelEditor pe, Position pos)
        {
            // substitute position
            PBPlayer selectedPlayer = Drawing.GetSelectedPlayer();

            if (selectedPlayer != null)
            {
                if (Drawing.CheckPlayer(pos.Name))
                {
                    Drawing.Delete();

                    pe.AddCount++;

                    PBPlayer player = new PBPlayer(pos.Name)
                    {
                        Coordinates = selectedPlayer.Coordinates,
                        Radius = selectedPlayer.Radius,
                        FillColor = selectedPlayer.FillColor,
                        TextColor = selectedPlayer.TextColor,
                        Text = pos.Value,
                    };

                    //player.FillColor = (player.ScoutType == (int)ScoutTypes.Offensive) ? ColorSetting.Instance.OffensivePlayerColor : ColorSetting.Instance.DefensivePlayerColor;   // 10-27-2010 Scott

                    Drawing.Add(player);

                    pe.AddCount++;

                    player.Selected = true;
                }
            }
        }

        void pe_PositionSelected(PersonnelEditor pe, Position pos)
        {
            // add position
            if (Drawing.CheckPlayer(pos.Name))
            {
                Personnel personnel = new Personnel();
                personnel.Positions.Clear();
                personnel.Positions.Add(pos);
                personnel.ScoutType = (ScoutTypes)(PBPlayer.GetScoutTypeByName(pos.Name));
                PlayersCreator pc = new PlayersCreator();
                pc.CreatePlayers(drawing, personnel, true, Drawing.GetScrimmageLine());
                pe.AddCount++;
            }
        }

        private void gridDrawingContainer_Loaded(object sender, RoutedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void gridDrawingContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void btnOffHReverse(object sender, RoutedEventArgs e)
        {
            drawing.HReverse(drawing.GetOffFigures());
        }

        private void btnDefHReverse(object sender, RoutedEventArgs e)
        {
            drawing.HReverse(drawing.GetDefFigures());
        }

        private void btnOffVReverse(object sender, RoutedEventArgs e)
        {
            drawing.VReverse(drawing.GetOffFigures());
        }

        private void btnDefVReverse(object sender, RoutedEventArgs e)
        {
            drawing.VReverse(drawing.GetDefFigures());
        }

        Point ptFormation = new Point(0, 0);

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (IsCtrlPressed())
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.Move;
            }

            string[] array = e.Data.GetFormats();

            if (array.Count() == 1)
            {
                object obj = e.Data.GetData(array[0]);

                // 11-04-2011 Scott
                ViewModel.TreeViewItemViewModel objViewModel = obj as ViewModel.TreeViewItemViewModel;

                ViewModel.TreeViewItemViewModel parentViewModel = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                if (objViewModel != null && parentViewModel != null && objViewModel.Parent == parentViewModel)
                {
                    return;
                }
                // end

                if (obj is ViewModel.FolderViewModel)    // 11-15-2010 Scott
                {
                    ViewModel.FolderViewModel selFolderVM = obj as ViewModel.FolderViewModel;

                    ViewModel.TreeViewItemViewModel parentVM = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                    if (selFolderVM != parentVM)
                    {
                        ViewModel.FolderViewModel folderVM = parentVM as ViewModel.FolderViewModel;

                        ViewModel.ScoutTypeViewModel stVM = parentVM as ViewModel.ScoutTypeViewModel;

                        string strOldPath = string.Empty;

                        if (folderVM != null)
                        {
                            strOldPath = folderVM.FolderPath;
                        }
                        else if (stVM != null)
                        {
                            strOldPath = stVM.ScoutTypePath;
                        }

                        if (strOldPath != string.Empty && Directory.Exists(strOldPath))
                        {
                            string strNewPath = strOldPath + @"\" + selFolderVM.FolderName;

                            if (!Directory.Exists(strNewPath))
                            {
                                Directory.Move(selFolderVM.FolderPath, strNewPath);

                                parentVM.Refresh();

                                selFolderVM.Parent.RemoveChild(selFolderVM);
                            }
                            else
                            {
                                MessageBox.Show("The folder was exist !");
                            }
                        }
                    }
                }

                if (obj is ViewModel.FormationViewModel)
                {
                    ViewModel.FormationViewModel formationVM = obj as ViewModel.FormationViewModel;

                    ViewModel.TreeViewItemViewModel parentVM = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                    ViewModel.FolderViewModel folderVM = parentVM as ViewModel.FolderViewModel;

                    ViewModel.ScoutTypeViewModel stVM = parentVM as ViewModel.ScoutTypeViewModel;

                    string strOldPath = string.Empty;

                    if (folderVM != null)
                    {
                        strOldPath = folderVM.FolderPath;
                    }
                    else if (stVM != null)
                    {
                        strOldPath = stVM.ScoutTypePath;
                    }

                    if (strOldPath != string.Empty && Directory.Exists(strOldPath))
                    {// 11-03-2011 Scott
                        string strNewPath = strOldPath + @"\" + formationVM.FormationName + ".Form";

                        if (File.Exists(strNewPath))
                        {
                            if (MessageBox.Show("The formation was exist ! Do you want to overwrite it ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                return;
                            }

                            ViewModel.FormationViewModel delFvm = parentVM.GetChild<ViewModel.FormationViewModel>(formationVM.FormationName);

                            if (delFvm != null)
                            {
                                delFvm.Parent.RemoveChild(delFvm);
                            }
                        }

                        File.Copy(formationVM.FormationPath, strNewPath, true);

                        string strNewFolder = strOldPath + @"\" + formationVM.FormationName + "@";

                        if (Directory.Exists(strNewFolder))
                        {
                            Directory.Delete(strNewFolder, true);
                        }

                        if (Directory.Exists(formationVM.PlayFolderPath))
                        {
                            Directory.Move(formationVM.PlayFolderPath, strNewFolder);    // 09-19-2010 Scott
                        }

                        Formation formation = new Formation(strNewPath);

                        ViewModel.FormationViewModel fvm = new Webb.Playbook.ViewModel.FormationViewModel(formation, parentVM);

                        parentVM.AddChild(fvm);

                        //if (e.Effects == DragDropEffects.Move)
                        //{
                        formationVM.Parent.RemoveChild(formationVM);
                        //}
                    }
                }

                // 06-16-2011 Scott
                if (obj is ViewModel.TitleViewModel)
                {
                    ViewModel.TitleViewModel titleVM = obj as ViewModel.TitleViewModel;

                    ViewModel.TreeViewItemViewModel parentVM = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                    ViewModel.FolderViewModel folderVM = parentVM as ViewModel.FolderViewModel;

                    ViewModel.ScoutTypeViewModel stVM = parentVM as ViewModel.ScoutTypeViewModel;

                    string strOldPath = string.Empty;

                    if (folderVM != null)
                    {
                        strOldPath = folderVM.FolderPath;
                    }
                    else if (stVM != null)
                    {
                        strOldPath = stVM.ScoutTypePath;
                    }

                    if (strOldPath != string.Empty && Directory.Exists(strOldPath))
                    {
                        string strNewPath = strOldPath + @"\" + titleVM.TitleName + ".Ttl";

                        if (!File.Exists(strNewPath))
                        {
                            File.Copy(titleVM.TitlePath, strNewPath);

                            Title title = new Title(strNewPath);

                            ViewModel.TitleViewModel tvm = new Webb.Playbook.ViewModel.TitleViewModel(title, parentVM);

                            parentVM.AddChild(tvm);

                            //if (e.Effects == DragDropEffects.Move)
                            //{
                            titleVM.Parent.RemoveChild(titleVM);
                            //}
                        }
                        else
                        {
                            MessageBox.Show("The title was exist !");
                        }
                    }
                }

                if (obj is ViewModel.PlayFolderViewModel)
                {
                    ViewModel.PlayFolderViewModel selFolderVM = obj as ViewModel.PlayFolderViewModel;

                    ViewModel.TreeViewItemViewModel parentVM = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                    if (selFolderVM != parentVM)
                    {
                        ViewModel.PlayFolderViewModel folderVM = parentVM as ViewModel.PlayFolderViewModel;

                        ViewModel.PlayTypeViewModel stVM = parentVM as ViewModel.PlayTypeViewModel;

                        string strOldPath = string.Empty;

                        if (folderVM != null)
                        {
                            strOldPath = folderVM.FolderPath;
                        }
                        else if (stVM != null)
                        {
                            strOldPath = stVM.PlayTypePath;
                        }

                        if (strOldPath != string.Empty && Directory.Exists(strOldPath))
                        {
                            string strNewPath = strOldPath + @"\" + selFolderVM.FolderName;

                            if (!Directory.Exists(strNewPath))
                            {
                                Directory.Move(selFolderVM.FolderPath, strNewPath);

                                parentVM.Refresh();

                                selFolderVM.Parent.RemoveChild(selFolderVM);
                            }
                            else
                            {
                                MessageBox.Show("The folder was exist !");
                            }
                        }
                    }
                }

                if (obj is ViewModel.PlayViewModel)
                {
                    ViewModel.PlayViewModel playVM = obj as ViewModel.PlayViewModel;

                    ViewModel.TreeViewItemViewModel parentVM = (sender as StackPanel).DataContext as ViewModel.TreeViewItemViewModel;

                    ViewModel.PlayFolderViewModel folderVM = parentVM as ViewModel.PlayFolderViewModel;

                    ViewModel.PlayTypeViewModel stVM = parentVM as ViewModel.PlayTypeViewModel;

                    string strOldPath = string.Empty;

                    if (folderVM != null)
                    {
                        strOldPath = folderVM.FolderPath;
                    }
                    else if (stVM != null)
                    {
                        strOldPath = stVM.PlayTypePath;
                    }

                    if (strOldPath != string.Empty && Directory.Exists(strOldPath))
                    {// 11-03-2011 Scott
                        string strNewPath = strOldPath + @"\" + playVM.PlayName + ".Play";

                        if (File.Exists(strNewPath))
                        {
                            if (MessageBox.Show("The play was exist ! Do you want to overwrite it ?", "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                return;
                            }

                            ViewModel.PlayViewModel delPvm = parentVM.GetChild<ViewModel.PlayViewModel>(playVM.PlayName);

                            if (delPvm != null)
                            {
                                delPvm.Parent.RemoveChild(delPvm);
                            }
                        }

                        File.Copy(playVM.PlayPath, strNewPath, true);

                        File.Copy(playVM.PlayPath + ".PlayInfo", strNewPath + ".PlayInfo", true);

                        Play play = new Play(strNewPath);

                        ViewModel.PlayViewModel fvm = new Webb.Playbook.ViewModel.PlayViewModel(play, parentVM);

                        parentVM.AddChild(fvm);

                        if (e.Effects == DragDropEffects.Move)
                        {
                            playVM.Parent.RemoveChild(playVM);
                        }
                    }
                }
            }
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ptFormation = Mouse.GetPosition(treeFormation);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (MainMode == Mode.Formation)
                {
                    object selectedItem = this.treeFormation.SelectedItem;

                    StackPanel container = sender as StackPanel;

                    if (container is StackPanel && selectedItem is ViewModel.FormationViewModel || selectedItem is ViewModel.FolderViewModel || selectedItem is ViewModel.TitleViewModel)   // 11-15-2010 Scott
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(container, selectedItem, DragDropEffects.All);
                    }
                }

                if (MainMode == Mode.Playbook)
                {
                    object selectedItem = this.treePlaybook.SelectedItem;

                    StackPanel container = sender as StackPanel;

                    if (container is StackPanel && selectedItem is ViewModel.PlayViewModel || selectedItem is ViewModel.PlayFolderViewModel)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(container, selectedItem, DragDropEffects.All);
                    }
                }
            }
        }

        private static bool IsCtrlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        //save to PNG
        private void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            PngBitmapEncoder enc = new PngBitmapEncoder();
            //BmpBitmapEncoder enc = new BmpBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        private void SaveRTBAsBMP(RenderTargetBitmap bmp, string filename)
        {
            JpegBitmapEncoder enc = new JpegBitmapEncoder();
            //BmpBitmapEncoder enc = new BmpBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        private void btnSaveToDiagramField_Click(object sender, RoutedEventArgs e)
        {
            if (Drawing != null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                saveFileDialog.Filter = "Advantage Diagram files (*.Dia)|*.Dia";
                saveFileDialog.DefaultExt = ".Dia";
                saveFileDialog.FileName = this.textFormation.Text;

                if (saveFileDialog.ShowDialog().GetValueOrDefault())
                {
                    DiagramBackgroundWindow dbw = new DiagramBackgroundWindow(canvasDrawing.ActualWidth);
                    dbw.Owner = this;
                    dbw.Show();

                    RenderTargetBitmap bmp = new RenderTargetBitmap((int)(dbw.ActualWidth), (int)(dbw.ActualHeight), 96, 96, PixelFormats.Default);
                    DrawingVisual drawingvisual = new DrawingVisual();

                    using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
                    {
                        VisualBrush visualbrush = new VisualBrush(dbw);

                        drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, dbw.ActualWidth, dbw.ActualHeight));
                    }

                    bmp.Render(drawingvisual);

                    SaveRTBAsBMP(bmp, AppDomain.CurrentDomain.BaseDirectory + @"Resource\Playground.bmp");

                    Drawing.SaveToDiagram(saveFileDialog.FileName, true);

                    dbw.Close();
                }
            }
        }

        private void btnSaveToDiagram_Click(object sender, RoutedEventArgs e)
        {
            if (Drawing != null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                saveFileDialog.Filter = "Advantage Diagram files (*.Dia)|*.Dia";
                saveFileDialog.DefaultExt = ".Dia";
                saveFileDialog.FileName = this.textFormation.Text;

                if (saveFileDialog.ShowDialog().GetValueOrDefault())
                {
                    Drawing.SaveToDiagram(saveFileDialog.FileName, false);
                }
            }
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow()
            {
                Owner = this,
            };

            if (cw.ShowDialog().Value)
            {
                PlaygroundStructure.SetHashLine(GameSetting.Instance.PlaygroundType);

                Drawing.SetPlaygroundType(GameSetting.Instance.PlaygroundType);
            }
        }

        private void btnToggleView_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = sender as Button;

            //if (btn.Content.ToString() == "Formation Name View")
            //{
            //    btn.Content = "Play Name View";
            //    treePlaybook_FV.SetValue(Panel.ZIndexProperty, 1);
            //    treePlaybook.SetValue(Panel.ZIndexProperty, 0);
            //    playbookRootViewModel_FV.Refresh();
            //}
            //else if (btn.Content.ToString() == "Play Name View")
            //{
            //    btn.Content = "Formation Name View";
            //    treePlaybook_FV.SetValue(Panel.ZIndexProperty, 0);
            //    treePlaybook.SetValue(Panel.ZIndexProperty, 1);
            //    playbookRootViewModel.Refresh();
            //}
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();

            about.Owner = this;

            about.ShowDialog();
        }

        private void MenuItem_Color_Click(object sender, RoutedEventArgs e)
        {
            ColorSettingWindow csw = new ColorSettingWindow(Drawing);

            csw.Owner = this;

            csw.ShowDialog();
        }

        private void StackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {// 08-11-2011 Scott
            StackPanel_MouseLeftButtonDown(sender, e);

            StackPanel sp = sender as StackPanel;

            if (sp != null)
            {
                ViewModel.TreeViewItemViewModel vm = sp.DataContext as ViewModel.TreeViewItemViewModel;

                if (vm != null)
                {
                    vm.IsSelected = true;
                }
            }
        }

        private void MenuItem_Field_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveCurrentFile())
            {
                return;
            }

            CloseDrawing();

            FieldsSettingWindow fsw = new FieldsSettingWindow();

            fsw.Owner = this;

            if (fsw.ShowDialog().Value)
            {
                ReloadTree();
            }
        }

        // 10-09-2011 Scott
        private void ReloadTree()
        {
            DetachData();

            formationRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);
            playbookRootViewModel.Refresh(Webb.Playbook.Data.GameSetting.Instance.PlaybookUserFolder);

            AttachData();
        }

        private void btnDragPlayground_Click(object sender, RoutedEventArgs e)
        {
            if (Drawing != null)
            {
                Drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Playground Dragger");
                btnDragPlayground.Focus();
                listBoxTool.SelectedIndex = -1;
            }
        }

        MenuItem playerItem = new MenuItem()
        {
            Header = "Player",
        };
        MenuItem lineItem = new MenuItem()
        {
            Header = "Line",
        };
        MenuItem labelItem = new MenuItem()
        {
            Header = "Label",
        };
        MenuItem ballItem = new MenuItem()
        {
            Header = "Ball",
        };
        MenuItem zoneItem = new MenuItem()
        {
            Header = "Zone",
        };
        private void menuEdit_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == menuEdit)
            {
                if (!menuEdit.Items.Contains(playerItem))
                {
                    menuEdit.Items.Add(playerItem);
                }
                Drawing.Behavior.CreatePlayerMenuItems(playerItem);

                if (!menuEdit.Items.Contains(lineItem))
                {
                    menuEdit.Items.Add(lineItem);
                }
                Drawing.Behavior.CreateLineMenuItems(lineItem);

                if (!menuEdit.Items.Contains(labelItem))
                {
                    menuEdit.Items.Add(labelItem);
                }
                Drawing.Behavior.CreateLabelMenuItems(labelItem);

                if (!menuEdit.Items.Contains(ballItem))
                {
                    menuEdit.Items.Add(ballItem);
                }
                Drawing.Behavior.CreateBallMenuItems(ballItem);

                if (!menuEdit.Items.Contains(zoneItem))
                {
                    menuEdit.Items.Add(zoneItem);
                }
                Drawing.Behavior.CreateZoneMenuItems(zoneItem);
            }
        }

        private void PrintDirectly()
        {
            if (System.IO.File.Exists(CurrentPath))
            {
                Print.CustomDocumentViewer dv = new Print.CustomDocumentViewer();

                List<string> files = new List<string>();
                files.Add(CurrentPath);

                FixedDocument fixedDoc = new FixedDocument();

                Print.PrintPreviewWindow.FillDocument(fixedDoc, files, 1);

                dv.Document = fixedDoc;

                Print.LocalPrinter.ShowWindow();

                dv.Print();

                ResetBehavior();
            }
        }

        private void miPrint_Click(object sender, RoutedEventArgs e)
        {
            this.InvalidateVisual();

            PrintDirectly();
        }

        private void miPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            Print.PrintPreviewWindow ppw = new Webb.Playbook.Print.PrintPreviewWindow();
            ppw.Owner = this;

            ppw.PrintFiles.Add(CurrentPath);

            if (ppw.ShowDialog().Value)
            {

            }

            ResetBehavior();
        }

        private void imgLogo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.webbelectronics.com//");
        }

        private void miPrintMany_Click(object sender, RoutedEventArgs e)
        {
            Print.SelectPlayWindow spw = new Webb.Playbook.Print.SelectPlayWindow(formationRootViewModel, playbookRootViewModel);

            spw.Owner = this;

            spw.ShowDialog();

            ResetBehavior();
        }

        private void miReport_Click(object sender, RoutedEventArgs e)
        {
            InteractiveReport ir = new InteractiveReport();

            if (ir.GetReportPath() == string.Empty)
            {
                MessageBox.Show("Please install Webb Interactive Report");

                return;
            }

            SaveCurrentFile();

            Print.SelectPlayForRptWindow spfr = new Webb.Playbook.Print.SelectPlayForRptWindow(formationRootViewModel, playbookRootViewModel);

            spfr.Owner = this;

            spfr.ShowDialog();
        }

        private ManagementObjectSearcher query;
        private ManagementObjectCollection queryCollection;
        string _classname = "SELECT * FROM Win32_Printer";
        private void miPageSetup_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.PageSetupDialog psd = new System.Windows.Forms.PageSetupDialog();

            psd.AllowMargins = false;
            psd.AllowOrientation = true;
            psd.AllowPaper = true;
            psd.AllowPrinter = true;

            psd.PrinterSettings = Print.LocalPrinter.DefaultPrinterSettings;
            psd.PageSettings = Print.LocalPrinter.DefaultPageSettings;
            psd.PageSettings.Landscape = Webb.Playbook.Data.GameSetting.Instance.Landscape;

            if (psd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                query = new ManagementObjectSearcher(_classname);

                queryCollection = query.Get();

                foreach (ManagementObject mo in queryCollection)
                {
                    if (string.Compare(mo["Name"].ToString(), psd.PrinterSettings.PrinterName, true) == 0)
                    {
                        mo.InvokeMethod("SetDefaultPrinter", null);

                        break;
                    }
                }

                Webb.Playbook.Data.GameSetting.Instance.Landscape = psd.PageSettings.Landscape;
            }
        }

        private void miGameSetup_Click(object sender, RoutedEventArgs e)
        {
            Print.GameSetupWindow gsw = new Webb.Playbook.Print.GameSetupWindow()
            {
                Owner = this,
            };

            if (gsw.ShowDialog().Value)
            {

            }

            ResetBehavior();
        }

        private void tabMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(CurrentPath))
            {
                string dir = System.IO.Path.GetDirectoryName(CurrentPath);

                Process.Start(dir);
            }
        }

        private void Automatic_Picture_Click(object sender, RoutedEventArgs e)
        {
            ImageSetupWindow isw = new ImageSetupWindow()
            {
                Owner = this,
            };

            if (isw.ShowDialog().Value)
            {

            }

            ResetBehavior();
        }

        // 05-19-2011 Scott
        private Point previousPoint = new Point();
        private List<Webb.Playbook.Geometry.FreeDraw.FreeLine> FreeLines = new List<Webb.Playbook.Geometry.FreeDraw.FreeLine>();
        private void canvasOutLine_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (previousPoint != new Point())
                {
                    Point pt = Drawing.CoordinateSystem.ToLogical(e.GetPosition(canvasOutLine));

                    Webb.Playbook.Geometry.FreeDraw.FreeLine line = new Webb.Playbook.Geometry.FreeDraw.FreeLine()
                    {
                        P1 = previousPoint,
                        P2 = pt,
                        Stroke = Webb.Playbook.Geometry.Shapes.Factory.Color,
                        Thickness = Webb.Playbook.Geometry.Shapes.Factory.StrokeThickness,
                    };

                    FreeLines.Add(line);

                    canvasOutLine.Children.Add(line.Line);

                    line.UpdateVisual(Drawing);

                    previousPoint = pt;
                }
            }
        }

        private void canvasOutLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = Drawing.CoordinateSystem.ToLogical(e.GetPosition(canvasOutLine));

            previousPoint = pt;
        }

        private void canvasOutLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = Drawing.CoordinateSystem.ToLogical(e.GetPosition(canvasOutLine));

            ClearFreeDraw();
        }

        bool bFreeDraw = false;
        private void btnFreeDraw_Click(object sender, RoutedEventArgs e)
        {
            bFreeDraw = !bFreeDraw;

            if (bFreeDraw)
            {
                imgFreeDraw.Source = new BitmapImage(new Uri(@"\Resource\SolidPen.png", UriKind.Relative));
                Panel.SetZIndex(canvasOutLine, 10000);
                UpdateFreeDraw();
            }
            else
            {
                imgFreeDraw.Source = new BitmapImage(new Uri(@"\Resource\Pen.png", UriKind.Relative));
                Panel.SetZIndex(canvasOutLine, -1);
            }
        }

        private void UpdateFreeDraw()
        {
            foreach (Webb.Playbook.Geometry.FreeDraw.FreeLine line in FreeLines)
            {
                line.UpdateVisual(Drawing);
            }
        }

        private void ClearFreeDraw()
        {
            FreeLines.Clear();

            for (int i = canvasOutLine.Children.Count - 1; i >= 0; i--)
            {
                if (canvasOutLine.Children[i] is Line)
                {
                    canvasOutLine.Children.RemoveAt(i);
                }
            }
        }

        private void CoordinateSystemUpdated()
        {
            if (bFreeDraw)
            {
                UpdateFreeDraw();
            }
        }

        private void SaveFreeDraw()
        {
            string file = CurrentPath + ".FD";

            XDocument doc = new XDocument();
            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");
            doc.Declaration = declare;

            XElement elemFreeDraw = new XElement("FreeDraw");
            doc.Add(elemFreeDraw);

            foreach (Webb.Playbook.Geometry.FreeDraw.FreeLine line in FreeLines)
            {
                XElement elemFreeDrawLine = new XElement("FreeDrawLine");
                line.SaveXml(elemFreeDrawLine);
                elemFreeDraw.Add(elemFreeDrawLine);
            }

            doc.Save(file);
        }

        private void LoadFreeDraw()
        {
            ClearFreeDraw();

            string file = CurrentPath + ".FD";

            if (File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);

                XElement elemFreeDraw = doc.Element("FreeDraw");

                if (elemFreeDraw != null)
                {
                    foreach (XElement elemLine in elemFreeDraw.Elements("FreeDrawLine"))
                    {
                        Webb.Playbook.Geometry.FreeDraw.FreeLine line = new Webb.Playbook.Geometry.FreeDraw.FreeLine();

                        line.LoadXml(elemLine);

                        FreeLines.Add(line);

                        canvasOutLine.Children.Add(line.Line);

                        line.UpdateVisual(Drawing);
                    }
                }
            }
        }

        private void HelpTopics_Click(object sender, RoutedEventArgs e)
        {
            string strHelpFile = AppDomain.CurrentDomain.BaseDirectory + @"\PLAYBOOK.chm";

            if (File.Exists(strHelpFile))
            {
                Process.Start(strHelpFile);
            }
            else
            {
                MessageBox.Show("Can't find help file");
            }
        }

        void miVideo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                string strVideo = (sender as MenuItem).Tag.ToString();

                if (File.Exists(strVideo))
                {
                    Process.Start(strVideo);
                }
            }
        }

        private void HelpVideos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void treeFormation_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

            }
        }

        private void treePlaybook_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

            }
        }

        private void treeFormation_DragOver(object sender, DragEventArgs e)
        {
            Point pt = e.GetPosition(treeFormation);

            Grid grid = VisualTreeHelper.GetChild(treeFormation, 0) as Grid;
            Border border = grid.Children[0] as Border;

            if (border is Border)
            {
                ScrollViewer scroll = border.Child as ScrollViewer;

                if (pt.Y > 0 && pt.Y < 20)
                {
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - 30);
                }

                if (pt.Y < treeFormation.ActualHeight && pt.Y > treeFormation.ActualHeight - 50)
                {
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset + 30);
                }
            }
        }
    }
}