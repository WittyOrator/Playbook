using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Game;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook
{
    public class ToolCollection<T> : CollectionWithEvents<T>
        where T : Tool
    {
        protected override void OnItemAdded(T item)
        {
            item.Parent = this as ToolCollection<Tool>;
        }

        protected override void OnItemRemoved(T item)
        {
            item.Parent = null;
        }
    }

    public class ToolManager : INotifyPropertyChanged
    {
        private static ReadOnlyCollection<Tool> mSingletons;
        public static ReadOnlyCollection<Tool> Singletons
        {
            get
            {
                if (mSingletons == null)
                {
                    mSingletons = InitializeTools();
                }
                return mSingletons;
            }
        }

        private static ReadOnlyCollection<Tool> InitializeTools()
        {
            List<Tool> Tools = new List<Tool>();

            Tools.Add(new Tool("Add Position", Commands.AddPosition));
            Tools.Add(new Tool("Substitute Position", Commands.SubstitutePosition));  // 07-19-2010 Scott
            Tools.Add(new Tool("Remove Position", Commands.RemovePosition));

            Tools.Add(new Tool("Team", Commands.Team));
            Tools.Add(new Tool("Personnel", Commands.Personnel));

            Tools.Add(new Tool("Select Position", Commands.Selector));
            Tools.Add(new Tool("Drag Position", Commands.Dragger));
            Tools.Add(new Tool("Pen", Commands.Pen));

            Tools.Add(new Tool("Assignments", Commands.Assignments));
            Tools.Add(new Tool("Stances", Commands.Stances));

            Tools.Add(new Tool("Zones", Commands.Zones));

            Tools.Add(new Tool("Routes", Commands.Routes));

            Tools.Add(new Tool("Arrow", Commands.EndTypeArrow));
            Tools.Add(new Tool("Block", Commands.EndTypeBlock));
            Tools.Add(new Tool("Block Person", Commands.EndTypeBlockPerson));

            //Set
            Tools.Add(new Tool("Set Defense", Commands.SetDefense));
            Tools.Add(new Tool("Set Offense", Commands.SetOffense));
            Tools.Add(new Tool("Set Kick", Commands.SetKick));
            Tools.Add(new Tool("Set Defenses", Commands.SetDefenses));
            Tools.Add(new Tool("Set Offenses", Commands.SetOffenses));
            Tools.Add(new Tool("Set Kicks", Commands.SetKicks));

            Tools.Add(new Tool("Actions", Commands.Actions));

            Tools.Add(new Tool("Set", Commands.Set));
            //Tools.Add(new Tool("Assign Handoff Left", Commands.AssignHandoffLeft));
            //Tools.Add(new Tool("Assign Handoff Right ", Commands.AssignHandoffRight));
            //Tools.Add(new Tool("Toss Left", Commands.TossLeft));
            //Tools.Add(new Tool("Toss Right", Commands.TossRight));
            Tools.Add(new Tool("Assign", Commands.AssignOption));
            //Tools.Add(new Tool("Fake Handoff Left", Commands.FakeHandoffLeft));
            //Tools.Add(new Tool("Fake Handoff Right", Commands.FakeHandoffRight));
            //Tools.Add(new Tool("Fake Toss Left", Commands.FakeTossLeft));
            //Tools.Add(new Tool("Fake Toss Right", Commands.FakeTossRight));
            Tools.Add(new Tool("Fake", Commands.FakeOption));
            Tools.Add(new Tool("Drop Step 1", Commands.DropStepOne));
            Tools.Add(new Tool("Drop Step 3", Commands.DropStepThree));
            Tools.Add(new Tool("Drop Step 5", Commands.DropStepFive));
            Tools.Add(new Tool("Drop Step 7", Commands.DropStepSeven));

            Tools.Add(new Tool("Run Block", Commands.RunBlock));
            Tools.Add(new Tool("Pass Block", Commands.PassBlock));
            Tools.Add(new Tool("Pass Block Area", Commands.PassBlockArea));

            Tools.Add(new Tool("Assign Man Press", Commands.AssignManPress));
            Tools.Add(new Tool("Assign Man Coverage", Commands.AssignManCoverage));
            Tools.Add(new Tool("Shade Left", Commands.ShadeLeft));
            Tools.Add(new Tool("Shade Right", Commands.ShadeRight));

            Tools.Add(new Tool("Pre Snap Motion", Commands.PreSnapMotion));
            Tools.Add(new Tool("Post Snap Motion", Commands.PostSnapMotion));
            Tools.Add(new Tool("Clear", Commands.Clear));
            Tools.Add(new Tool("Clear Lines", Commands.ClearLines));

            Tools.Add(new Tool("2 Point Left", Commands.TwoPointLeft));
            Tools.Add(new Tool("2 Point Center", Commands.TwoPointCenter));
            Tools.Add(new Tool("2 Point Right", Commands.TwoPointRight));
            Tools.Add(new Tool("3 Point Left", Commands.ThreePointLeft));
            Tools.Add(new Tool("3 Point Right", Commands.ThreePointRight));
            Tools.Add(new Tool("4 Point", Commands.FourPoint));
            Tools.Add(new Tool("Clear", Commands.ClearStance));

            Tools.Add(new Tool("Flat Left", Commands.FlatLeft));
            Tools.Add(new Tool("Flat Right", Commands.FlatRight));
            Tools.Add(new Tool("Curl Left", Commands.CurlLeft));
            Tools.Add(new Tool("Curl Right", Commands.CurlRight));
            Tools.Add(new Tool("Hook Left", Commands.HookLeft));
            Tools.Add(new Tool("Hook Right", Commands.HookRight));
            Tools.Add(new Tool("Hook Full Middle", Commands.HookFullMiddle));
            Tools.Add(new Tool("Curl/Flat Left", Commands.CurlFlatLeft));
            Tools.Add(new Tool("Curl/Flat Right", Commands.CurlFlatRight));
            Tools.Add(new Tool("Curl/Hook Left", Commands.CurlHookLeft));
            Tools.Add(new Tool("Curl/Hook Right", Commands.CurlHookRight));
            Tools.Add(new Tool("Thirds Deep Left", Commands.ThirdsDeepLeft));
            Tools.Add(new Tool("Thirds Deep Right", Commands.ThirdsDeepRight));
            Tools.Add(new Tool("Thirds Deep Middle", Commands.ThirdsDeepMiddle));
            Tools.Add(new Tool("Halves Deep Left", Commands.HalvesDeepLeft));
            Tools.Add(new Tool("Halves Deep Right", Commands.HalvesDeepRight));
            Tools.Add(new Tool("Quarters Deep Left", Commands.QuartersDeepLeft));
            Tools.Add(new Tool("Quarters Deep Right", Commands.QuartersDeepRight));
            Tools.Add(new Tool("Quarters Deep Mid Left", Commands.QuartersDeepMidLeft));
            Tools.Add(new Tool("Quarters Deep Mid Right", Commands.QuartersDeepMidRight));
            Tools.Add(new Tool("Swing Left", Commands.SwingLeft));
            Tools.Add(new Tool("Swing Right", Commands.SwingRight));
            Tools.Add(new Tool("Out Left", Commands.OutLeft));
            Tools.Add(new Tool("Out Right", Commands.OutRight));
            Tools.Add(new Tool("Clear", Commands.Clear));

            Tools.Add(new Tool("Open", Commands.Open));
            Tools.Add(new Tool("Save", Commands.Save));
            Tools.Add(new Tool("Print", Commands.Print));
            Tools.Add(new Tool("Gridlines", Commands.Gridlines));
            Tools.Add(new Tool("3D View", Commands.ThreeDView));

            Tools.Add(new Tool("Play", Commands.PlayAni));
            Tools.Add(new Tool("Pause", Commands.PauseAni));
            Tools.Add(new Tool("Stop", Commands.StopAni));
            Tools.Add(new Tool("Resume", Commands.ResumeAni));
            Tools.Add(new Tool("Still", Commands.Still));
            //HReverse
            Tools.Add(new Tool("HReverse", Commands.HReverse));
            Tools.Add(new Tool("OffHreverse", Commands.OffHreverse));
            Tools.Add(new Tool("DefHreverse", Commands.DeffHreverse));
            //LineType
            Tools.Add(new Tool("BeeLine", Commands.BeeLine));
            Tools.Add(new Tool("JaggedLine", Commands.JaggedLine));
            Tools.Add(new Tool("CurvyLine", Commands.CurvyLine));
            //VReverse
            Tools.Add(new Tool("VReverse", Commands.VReverse));

            //ConvertToSet
            Tools.Add(new Tool("Convert to Set", Commands.ConvertDefToSet));
            Tools.Add(new Tool("Convert to Set", Commands.ConvertOffToSet));
            Tools.Add(new Tool("Convert to Set", Commands.ConvertKickToSet));
            

            Tools.Add(new Tool("Create Zone", Commands.CreateZone));
            Tools.Add(new Tool("Save Zone", Commands.ConvertToZone));
            Tools.Add(new Tool("Convert to Route", Commands.ConvertToRoute));
            Tools.Add(new Tool("Convert to Action", Commands.ConvertToAction));
            Tools.Add(new Tool("Convert to Block", Commands.ConvertToBlock));
            Tools.Add(new Tool("Convert to Stunt/Blitz", Commands.ConvertToStuntBlitz));

            Tools.Add(new Tool("Notes and Calls", Commands.NotesAndCalls));
            Tools.Add(new Tool("Edit Play Note", Commands.EditPlayNote));
            Tools.Add(new Tool("Add Down Call", Commands.AddDownCall));
            Tools.Add(new Tool("Add Snap Call", Commands.AddSnapCall));

            Tools.Add(new Tool("Rename", Commands.Rename));
            Tools.Add(new Tool("Delete", Commands.Delete));
            Tools.Add(new Tool("Save As", Commands.SaveAs));
            Tools.Add(new Tool("New Formation", Commands.NewFormation));
            Tools.Add(new Tool("New Folder", Commands.NewFolder));

            // 08-02-2010 Scott
            Tools.Add(new Tool("Undo", Commands.Undo));
            Tools.Add(new Tool("Redo", Commands.Redo));

            Tools.Add(new Tool("New Play", Commands.NewPlay));
            Tools.Add(new Tool("New Adjustment", Commands.NewAdjustment));

            Tools.Add(new Tool("Animation", Commands.Animation));
            Tools.Add(new Tool("Close Animation", Commands.CloseAnimation));

            Tools.Add(new Tool("Presentation", Commands.Presentation));
            Tools.Add(new Tool("Close Presentation", Commands.ClosePresentation));

            Tools.Add(new Tool("Create Play", Commands.CreatePlay));    // 09-17-2010 Scott

            Tools.Add(new Tool("Coaching Points", Commands.CoachingPoints)); // 09-27-2010 Scott

            Tools.Add(new Tool("Text", Commands.Text));

            Tools.Add(new Tool("Switch Routes", Commands.SwitchRoutes));

            Tools.Add(new Tool("New", Commands.New));   // 11-17-2010 Scott

            Tools.Add(new Tool("Drag Position", Commands.ToolDragger));

            Tools.Add(new Tool("Solid", Commands.Solid));
            Tools.Add(new Tool("Dashed", Commands.Dashed));
            Tools.Add(new Tool("Dotted", Commands.Dotted));

            Tools.Add(new Tool("Blocks", Commands.RoutesBlock));
            Tools.Add(new Tool("Actions", Commands.RoutesAction));
            Tools.Add(new Tool("Stunt/Blitz", Commands.RoutesStuntBlitz));
            Tools.Add(new Tool("Blocks", Commands.RouteBlock));
            Tools.Add(new Tool("Actions", Commands.RouteAction));
            Tools.Add(new Tool("Stunt/Blitz", Commands.RouteStuntBlitz));

            Tools.Add(new Tool("None", Commands.EndTypeNone));

            Tools.Add(new Tool("Football", Commands.Ball));

            Tools.Add(new Tool("Snap", Commands.Snap));

            Tools.Add(new Tool("Save Base Formation", Commands.SaveBaseFormation));
            Tools.Add(new Tool("Load Base Formation", Commands.LoadBaseFormation));

            Tools.Add(new Tool("Report", Commands.Report));

            Tools.Add(new Tool("Save Label", Commands.ConvertToLabel));
            Tools.Add(new Tool("Create New Label", Commands.CreateNewLabel));
            Tools.Add(new Tool("Create Pre Snap Motion", Commands.CreatePreSnapMotion));
            Tools.Add(new Tool("Edit Coaching Points", Commands.CreateCoachingPoints));
            Tools.Add(new Tool("Save Coaching Points", Commands.SaveCoachingPoints));

            // 05-30-2011 Scott
            Tools.Add(new Tool("Insert Background", Commands.InsertBackground));
            Tools.Add(new Tool("Insert Image", Commands.InsertImage));
            Tools.Add(new Tool("Insert Text", Commands.IntertLabel));
            Tools.Add(new Tool("Clear Title", Commands.ClearTitle));

            // 06-13-2011 Scott
            Tools.Add(new Tool("Rev", Commands.VideoRev));
            Tools.Add(new Tool("RPlay", Commands.VideoRPlay));
            Tools.Add(new Tool("RSlow", Commands.VideoRSlow));
            Tools.Add(new Tool("Still", Commands.VideoStill));
            Tools.Add(new Tool("Slow", Commands.VideoSlow));
            Tools.Add(new Tool("Play", Commands.VideoPlay));
            Tools.Add(new Tool("FF", Commands.VideoFF));
            Tools.Add(new Tool("Stop", Commands.VideoStop));

            Tools.Add(new Tool("Help Topics", Commands.HelpTopics));

            Tools.Add(new Tool("Edit Pre Snap Motion", Commands.EditPreSnapMotion));
            Tools.Add(new Tool("Save Pre Snap Motion", Commands.ConvertToPreSnapMotion));

            return Tools.AsReadOnly();
        }

        public static Tool LookupByName(string name)
        {
            Tool tool = Singletons.Where(x => x.Text == name).FirstOrDefault();

            return tool;
        }
        public static Tool LookupByCommand(int Command)
        {
            Tool tool = Singletons.Where(x => x.Command == Command).FirstOrDefault();

            return tool;
        }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");

                // 09-19-2010 Scott
                if (name == string.Empty)
                {
                    Visible = Visibility.Collapsed;
                }
                else
                {
                    Visible = Visibility.Visible;
                }
            }
        }

        // 09-19-2010 Scott
        private Visibility visible = Visibility.Collapsed;
        public Visibility Visible
        {
            get { return visible; }
            set 
            { 
                visible = value;
                OnPropertyChanged("Visible");
            }
        }

        private ToolCollection<Tool> tools;
        public ToolCollection<Tool> Tools
        {
            get { return tools; }
            set { tools = value; }
        }

        public ToolManager()
        {
            tools = new ToolCollection<Tool>();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, arg);
            }
        }

        #endregion // INotifyPropertyChanged Members

        public void InitToolsMenu(WorkState workState)
        {
            Clear();
            Name = "Tools";

            switch (workState)
            {
                case WorkState.AfterNewFormation:
                case WorkState.AfterOpenFormation:
                case WorkState.AfterNewAdjustment:
                case WorkState.AfterOpenAdjustment:
                case WorkState.AfterNewPlay:
                case WorkState.AfterOpenPlay:
                case WorkState.AfterNewFormationPlay:
                case WorkState.AfterOpenFormationPlay:
                    Tools.Add(ToolManager.LookupByCommand(Commands.Selector));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Dragger));

                    Tools.Add(ToolManager.LookupByCommand(Commands.AddPosition));
                    Tools.Add(ToolManager.LookupByCommand(Commands.SubstitutePosition));    // 07-19-2010 Scott
                    Tools.Add(ToolManager.LookupByCommand(Commands.RemovePosition));
                    break;
                case WorkState.FormationBlank:
                    break;
                case WorkState.AdjustmentBlank:
                    break;
                case WorkState.AdjustmentTypeBlank:
                    break;
                case WorkState.AfterOpenTitle:
                // 05-30-2011 Scott
                case WorkState.AfterNewTitle:
                    Tools.Add(ToolManager.LookupByCommand(Commands.InsertBackground));
                    Tools.Add(ToolManager.LookupByCommand(Commands.InsertImage));
                    Tools.Add(ToolManager.LookupByCommand(Commands.IntertLabel));
                    Tools.Add(ToolManager.LookupByCommand(Commands.ClearTitle));
                    break;
            }
        }

        //add for set
        public void InitSetDefActionsMenu()
        {
            Clear();
            Tools.Add(ToolManager.LookupByCommand(Commands.SetDefenses));
        }
        //add for set
        public void InitSetOffActionsMenu()
        {
            Clear();
            tools.Add(ToolManager.LookupByCommand(Commands.SetOffenses));
        }
        //add for set
        public void InitSetKickActionsMenu()
        {
            Clear();
            tools.Add(ToolManager.LookupByCommand(Commands.SetKicks));
        }

        public void InitPlayerActionsMenu(PBPlayer player)
        {
            Clear();

            Tools.Add(ToolManager.LookupByCommand(Commands.SetDefenses));
            Tools.Add(ToolManager.LookupByCommand(Commands.SetOffenses));
            Tools.Add(ToolManager.LookupByCommand(Commands.SetKicks));

            if (player == null)
            {
                return;
            }
            Clear();
            Name = "Player Actions";

            Tools.Add(ToolManager.LookupByCommand(Commands.PreSnapMotion));

            //Tools.Add(ToolManager.LookupByCommand(Commands.Assignments));
            //Tools.Add(ToolManager.LookupByCommand(Commands.Stances));

            Tools.Add(ToolManager.LookupByCommand(Commands.Routes));
            Tools.Add(ToolManager.LookupByCommand(Commands.RoutesAction));
            Tools.Add(ToolManager.LookupByCommand(Commands.RoutesBlock));
            Tools.Add(ToolManager.LookupByCommand(Commands.RoutesStuntBlitz));

            Tools.Add(ToolManager.LookupByCommand(Commands.ClearLines));

            //Tools.Add(ToolManager.LookupByCommand(Commands.SetDefenses));
            //tools.Add(ToolManager.LookupByCommand(Commands.SetOffenses));
            //Tools.Add(ToolManager.LookupByCommand(Commands.NotesAndCalls));   // delete by scott 11-19-2009

            if (player.ScoutType == (int)Data.ScoutTypes.Offensive)
            {

            }

            if (player.ScoutType == (int)Data.ScoutTypes.Defensive)
            {
                Tools.Add(ToolManager.LookupByCommand(Commands.Zones));
            }

            Tools.Add(ToolManager.LookupByCommand(Commands.CoachingPoints));    // 09-27-2010 Scott

            //Tools.Add(ToolManager.LookupByCommand(Commands.Actions));
        }

        public void InitSubMenu(IFigure figure, int command, WorkState workState)
        {
            PBPlayer player = figure as PBPlayer;
            PBLabel label = figure as PBLabel;

            if (player != null)
            {
                switch (command)
                {
                    case Commands.Assignments:
                        //this.InitAssignmentsSubMenu(player, workState);
                        break;
                    case Commands.Stances:
                        this.InitStancesSubMenu(player);
                        break;
                    case Commands.Zones:
                        this.InitZonesSubMenu(player);
                        break;
                    case Commands.Routes:
                        this.InitRoutesSubMenu(player);
                        break;
                    case Commands.RoutesBlock:
                        this.InitBlocksSubMenu(player);
                        break;
                    case Commands.RoutesAction:
                        this.InitActionsSubMenu(player);
                        break;
                    case Commands.RoutesStuntBlitz:
                        this.InitStuntBlitzSubMenu(player);
                        break;
                    case Commands.CoachingPoints:
                        this.InitCoachingPoints(player);
                        break;
                    case Commands.PreSnapMotion:
                        this.InitPreSnapMotions(player);
                        break;
                    case Commands.NotesAndCalls:
                        this.InitNotesCallsMenu(player);
                        break;
                    case Commands.Actions:
                        this.InitActionsMenu(player, WorkState.FormationBlank, player.ScoutType);
                        break;
                    case Commands.SetDefenses:
                        this.InitSetDefSubMenu();
                        break;
                    case Commands.SetOffenses:
                        this.InitSetOffSubMenu();
                        break;
                    case Commands.SetKicks:
                        this.InitSetKickSubMenu();
                        break;
                    default:
                        this.Clear();
                        break;
                }
            }

            if (label != null)
            {
                this.InitLabelSubMenu(label);
            }
        }

        public void InitActionsMenu(PBPlayer player, WorkState workState, int nScoutType)
        {
            Clear();

            Name = "Actions";

            switch (workState)
            {
                case WorkState.AfterNewFormation:
                case WorkState.AfterOpenFormation:
                    ToolManager.LookupByCommand(Commands.CreatePlay).Text = "Create " + (nScoutType == 0 ? Webb.Playbook.Data.GameSetting.Instance.OffensiveSubField : Webb.Playbook.Data.GameSetting.Instance.DefensiveSubField);  // 11-16-2010 Scott
                    Tools.Add(ToolManager.LookupByCommand(Commands.CreatePlay));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Save));
                    Tools.Add(ToolManager.LookupByCommand(Commands.SaveAs));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Rename));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Delete));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Undo));          // 08-02-2010 Scott
                    Tools.Add(ToolManager.LookupByCommand(Commands.Redo));          // 08-02-2010 Scott
                    break;
                case WorkState.AfterNewPlay:
                case WorkState.AfterOpenPlay:
                case WorkState.AfterNewFormationPlay:
                case WorkState.AfterOpenFormationPlay:
                    Tools.Add(ToolManager.LookupByCommand(Commands.Save));
                    Tools.Add(ToolManager.LookupByCommand(Commands.SaveAs));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Rename));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Delete));
                    Tools.Add(ToolManager.LookupByCommand(Commands.Undo));          // 08-02-2010 Scott
                    Tools.Add(ToolManager.LookupByCommand(Commands.Redo));          // 08-02-2010 Scott
                    break;
                case WorkState.FormationBlank:
                    ToolManager.LookupByCommand(Commands.NewFormation).Text = "New " + (nScoutType == 0 ? Webb.Playbook.Data.GameSetting.Instance.OffensiveMainField : Webb.Playbook.Data.GameSetting.Instance.DefensiveMainField); // 11-16-2010 Scott
                    Tools.Add(ToolManager.LookupByCommand(Commands.NewFormation));
                    Tools.Add(ToolManager.LookupByCommand(Commands.NewFolder));
                    break;
                case WorkState.AdjustmentBlank:
                    break;
                case WorkState.AdjustmentTypeBlank:
                    Tools.Add(ToolManager.LookupByCommand(Commands.NewAdjustment));
                    break;
                case WorkState.PlaybookBlank:
                    Tools.Add(ToolManager.LookupByCommand(Commands.NewPlay));
                    break;
                case WorkState.AfterNewAdjustment:
                case WorkState.AfterOpenAdjustment:
                    Tools.Add(ToolManager.LookupByCommand(Commands.Delete));
                    break;
            }
        }

        public void InitNotesCallsMenu(PBPlayer player)
        {
            Clear();

            Name = "Calls and Notes";

            Tools.Add(ToolManager.LookupByCommand(Commands.EditPlayNote));
            Tools.Add(ToolManager.LookupByCommand(Commands.AddDownCall));
            Tools.Add(ToolManager.LookupByCommand(Commands.AddSnapCall));
        }

        public void InitRoutesSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Routes";

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToRoute));

            // load all routes  add by scott 11-10-2009
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Routes";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.Route));
            }
        }

        public void InitPreSnapMotions(PBPlayer player)
        {
            Clear();

            Name = "Pre Snap Motions";

            Tools.Add(ToolManager.LookupByCommand(Commands.EditPreSnapMotion));

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToPreSnapMotion));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\PreSnapMotions";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.RoutePreSnapMotion));
            }
        }

        public void InitCoachingPoints(PBPlayer player)
        {
            Clear();

            Name = "CoachingPoints";

            Tools.Add(ToolManager.LookupByCommand(Commands.CreateCoachingPoints));

            Tools.Add(ToolManager.LookupByCommand(Commands.SaveCoachingPoints));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\CoachingPoints";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.TextCoachingPoints));
            }
        }

        public void InitStuntBlitzSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Stunt/Blitz";

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToStuntBlitz));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Stunt Blitz";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.RouteStuntBlitz));
            }
        }

        public void InitLabelSubMenu(PBLabel label)
        {
            Clear();

            Name = "Labels";

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToLabel));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Labels";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.Label));
            }
        }

        public void InitLabelMenu()
        {
            Clear();

            Name = "Labels";

            Tools.Add(ToolManager.LookupByCommand(Commands.CreateNewLabel));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Labels";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.Label));
            }
        }

        public void InitActionsSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Actions";

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToAction));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Actions";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.RouteAction));
            }
        }

        public void InitBlocksSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Blocks";

            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToBlock));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Blocks";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.RouteBlock));
            }
        }

        public void InitSetDefSubMenu()
        {

            Clear();
            Name = "Set--Defense";
            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertDefToSet));
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Set";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

            }
            if (!System.IO.Directory.Exists(path + @"\Defense"))
            {
                System.IO.Directory.CreateDirectory(path + @"\Defense");
            }
            foreach (string file in System.IO.Directory.GetFiles(path + @"\Defense"))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.SetDefense));
            }
        }

        // 10-27-2011 Scott
        public void InitSetKickSubMenu()
        {

            Clear();
            Name = "Set--Special Teams";
            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertKickToSet));
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Set";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

            }
            if (!System.IO.Directory.Exists(path + @"\Kicks"))
            {
                System.IO.Directory.CreateDirectory(path + @"\Kicks");
            }
            foreach (string file in System.IO.Directory.GetFiles(path + @"\Kicks"))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.SetKick));
            }
        }

        public void InitSetOffSubMenu()
        {
            Clear();
            Name = "Set--Offense";
            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertOffToSet));
            string path = AppDomain.CurrentDomain.BaseDirectory + "Set";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            if (!System.IO.Directory.Exists(path + @"\Offense"))
            {
                System.IO.Directory.CreateDirectory(path + @"\Offense");
            }
            foreach (string file in System.IO.Directory.GetFiles(path + @"\Offense"))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                Tools.Add(new Tool(fi.Name, Commands.SetOffense));
            }
        }

        public void InitAssignmentsSubMenu(PBPlayer player, WorkState workState)
        {
            Clear();

            Name = "Assignments";

            if (player.Name == "QB")
            {
                //Tools.Add(ToolManager.LookupByCommand(Commands.Set));
                //Tools.Add(ToolManager.LookupByCommand(Commands.AssignHandoffLeft));
                //Tools.Add(ToolManager.LookupByCommand(Commands.AssignHandoffRight));
                //Tools.Add(ToolManager.LookupByCommand(Commands.TossLeft));
                //Tools.Add(ToolManager.LookupByCommand(Commands.TossRight));
                //Tools.Add(ToolManager.LookupByCommand(Commands.AssignOption));
                //Tools.Add(ToolManager.LookupByCommand(Commands.FakeHandoffLeft));
                //Tools.Add(ToolManager.LookupByCommand(Commands.FakeHandoffRight));
                //Tools.Add(ToolManager.LookupByCommand(Commands.FakeTossLeft));
                //Tools.Add(ToolManager.LookupByCommand(Commands.FakeTossRight));
                //Tools.Add(ToolManager.LookupByCommand(Commands.FakeOption));
                //Tools.Add(ToolManager.LookupByCommand(Commands.DropStepOne));
                //Tools.Add(ToolManager.LookupByCommand(Commands.DropStepThree));
                //Tools.Add(ToolManager.LookupByCommand(Commands.DropStepFive));
                //Tools.Add(ToolManager.LookupByCommand(Commands.DropStepSeven));
            }
            else
            {
                if (player.ScoutType == (int)Data.ScoutTypes.Offensive)
                {
                    if (workState == WorkState.AfterNewPlay || workState == WorkState.AfterOpenPlay)    // 09-26-2010 Scott
                    {
                        //Tools.Add(ToolManager.LookupByCommand(Commands.RunBlock));
                        //Tools.Add(ToolManager.LookupByCommand(Commands.PassBlock));
                    }
                    //Tools.Add(ToolManager.LookupByCommand(Commands.PassBlockArea));
                }
                else if (player.ScoutType == (int)Data.ScoutTypes.Defensive)
                {
                    if (workState == WorkState.AfterNewPlay || workState == WorkState.AfterOpenPlay)    // 09-26-2010 Scott
                    {
                        //Tools.Add(ToolManager.LookupByCommand(Commands.AssignManPress));
                        //Tools.Add(ToolManager.LookupByCommand(Commands.AssignManCoverage));
                    }
                    //Tools.Add(ToolManager.LookupByCommand(Commands.ShadeLeft));                       // 09-26-2010 Scott
                    //Tools.Add(ToolManager.LookupByCommand(Commands.ShadeRight));                      // 09-26-2010 Scott
                }

                Tools.Add(ToolManager.LookupByCommand(Commands.PreSnapMotion));
            }

            //Tools.Add(ToolManager.LookupByCommand(Commands.PostSnapMotion));
            Tools.Add(ToolManager.LookupByCommand(Commands.Clear));
        }

        public void InitStancesSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Stances";

            Tools.Add(ToolManager.LookupByCommand(Commands.TwoPointLeft));
            Tools.Add(ToolManager.LookupByCommand(Commands.TwoPointCenter));
            Tools.Add(ToolManager.LookupByCommand(Commands.TwoPointRight));
            Tools.Add(ToolManager.LookupByCommand(Commands.ThreePointLeft));
            Tools.Add(ToolManager.LookupByCommand(Commands.ThreePointRight));
            Tools.Add(ToolManager.LookupByCommand(Commands.FourPoint));
            Tools.Add(ToolManager.LookupByCommand(Commands.ClearStance));
        }

        public void InitZonesSubMenu(PBPlayer player)
        {
            Clear();

            Name = "Zones";

            Tools.Add(ToolManager.LookupByCommand(Commands.CreateZone));
            Tools.Add(ToolManager.LookupByCommand(Commands.ConvertToZone));

            // load all zones  add by scott 03-11-2011
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Zones";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);

                Tools.Add(new Tool(fi.Name, Commands.Zone));
            }

            //Tools.Add(ToolManager.LookupByCommand(Commands.FlatLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.FlatRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.HookLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.HookRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.HookFullMiddle));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlFlatLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlFlatRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlHookLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.CurlHookRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.ThirdsDeepLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.ThirdsDeepRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.ThirdsDeepMiddle));
            //Tools.Add(ToolManager.LookupByCommand(Commands.HalvesDeepLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.HalvesDeepRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.QuartersDeepLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.QuartersDeepRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.QuartersDeepMidLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.QuartersDeepMidRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.SwingLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.SwingRight));
            //Tools.Add(ToolManager.LookupByCommand(Commands.OutLeft));
            //Tools.Add(ToolManager.LookupByCommand(Commands.OutRight));
            Tools.Add(ToolManager.LookupByCommand(Commands.Clear));
        }

        public void InitToolBarMenu()
        {
            Clear();

            Name = "Tool Bar";

            Tools.Add(ToolManager.LookupByCommand(Commands.New));
            Tools.Add(ToolManager.LookupByCommand(Commands.NewFolder));
            Tools.Add(ToolManager.LookupByCommand(Commands.Save));
            Tools.Add(ToolManager.LookupByCommand(Commands.SaveAs));
            Tools.Add(ToolManager.LookupByCommand(Commands.Rename));
            Tools.Add(ToolManager.LookupByCommand(Commands.Delete));
            Tools.Add(ToolManager.LookupByCommand(Commands.Undo));
            Tools.Add(ToolManager.LookupByCommand(Commands.Redo));
            Tools.Add(ToolManager.LookupByCommand(Commands.Print));
            Tools.Add(ToolManager.LookupByCommand(Commands.Report));
            Tools.Add(ToolManager.LookupByCommand(Commands.Gridlines));
            Tools.Add(ToolManager.LookupByCommand(Commands.Snap));
            Tools.Add(ToolManager.LookupByCommand(Commands.Ball));

            ToolManager.LookupByCommand(Commands.Team).Enable = false;
            Tools.Add(ToolManager.LookupByCommand(Commands.Team));
            Tools.Add(ToolManager.LookupByCommand(Commands.Personnel));

            Tools.Add(ToolManager.LookupByCommand(Commands.HelpTopics));
        }

        public void InitBaseFormationMenu()
        {
            Clear();

            Name = "Base Formation";

            Tools.Add(ToolManager.LookupByCommand(Commands.SaveBaseFormation));
            Tools.Add(ToolManager.LookupByCommand(Commands.LoadBaseFormation));
        }

        public void Init2D3DToolBarMenu(bool visible)
        {
            Clear();

            Name = "2D 3D Tool Bar";

            if (visible)
            {
                // 09-19-2010 Delete by Scott
                Tools.Add(ToolManager.LookupByCommand(Commands.ThreeDView));    // 09-26-2010 Scott
                //Tools.Add(ToolManager.LookupByCommand(Commands.PlayAni));
                //Tools.Add(ToolManager.LookupByCommand(Commands.Still));
                //Tools.Add(ToolManager.LookupByCommand(Commands.StopAni));
                Tools.Add(ToolManager.LookupByCommand(Commands.Animation));
            }

            Tools.Add(ToolManager.LookupByCommand(Commands.Presentation));

            ToolManager.LookupByCommand(Commands.Presentation).Enable = Webb.Playbook.Data.ProductInfo.Type == Webb.Playbook.Data.ProductInfo.ProductType.Full;
        }

        public void InitVideoFullScreenToolBarMenu()
        {
            Clear();

            Name = "Video Tool Bar";

            Tools.Add(ToolManager.LookupByCommand(Commands.VideoRev));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoRPlay));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoRSlow));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoStill));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoSlow));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoPlay));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoFF));
            Tools.Add(ToolManager.LookupByCommand(Commands.VideoStop));
            Tools.Add(ToolManager.LookupByCommand(Commands.CloseAnimation));
        }

        public void Init2DFullScreenToolBarMenu()
        {
            Clear();

            Name = "2D Tool Bar";

            Tools.Add(ToolManager.LookupByCommand(Commands.PlayAni));
            Tools.Add(ToolManager.LookupByCommand(Commands.Still));
            Tools.Add(ToolManager.LookupByCommand(Commands.StopAni));
            Tools.Add(ToolManager.LookupByCommand(Commands.CloseAnimation));

            Tools.Add(ToolManager.LookupByCommand(Commands.SwitchRoutes));
        }
        
        public void InitHReverseToolBarMenu()
        {
            Clear();

            Name = "Reverse Tool Bar";
            
            Tools.Add(ToolManager.LookupByCommand(Commands.HReverse));
            Tools.Add(ToolManager.LookupByCommand(Commands.VReverse));
            Tools.Add(ToolManager.LookupByCommand(Commands.Text));
            //Tools.Add(ToolManager.LookupByCommand(Commands.OffHreverse));
            //Tools.Add(ToolManager.LookupByCommand(Commands.DeffHreverse));
        }
        public void InitPresentationColorToolBarMenu()
        {
            Clear();
            Name = "Color Tool Bar";

            Tools.Add(new Tool("Black", Commands.Color) { Checked = true });
            Tools.Add(new Tool("White", Commands.Color));
            Tools.Add(new Tool("Red", Commands.Color));
            Tools.Add(new Tool("Green", Commands.Color));
            Tools.Add(new Tool("Blue", Commands.Color));
            Tools.Add(new Tool("Cyan", Commands.Color));
            Tools.Add(new Tool("Magenta", Commands.Color));
            Tools.Add(new Tool("Yellow", Commands.Color));
        }
        public void InitColorToolBarMenu()
        {
            Clear();
            Name = "Color Tool Bar";

            Tools.Add(new Tool("Black",Commands.Color){Checked = true});
            Tools.Add(new Tool("White",Commands.Color));
            Tools.Add(new Tool("Red", Commands.Color));
            Tools.Add(new Tool("Green", Commands.Color));
            Tools.Add(new Tool("Blue", Commands.Color));
            Tools.Add(new Tool("Cyan", Commands.Color));
            Tools.Add(new Tool("Magenta", Commands.Color));
            Tools.Add(new Tool("Yellow", Commands.Color));
            Tools.Add(new Tool("Dark Red", Commands.Color));
            Tools.Add(new Tool("Dark Green", Commands.Color));
            Tools.Add(new Tool("Dark Blue", Commands.Color));
            Tools.Add(new Tool("Olive", Commands.Color));
            Tools.Add(new Tool("Teal", Commands.Color));
            Tools.Add(new Tool("Gray", Commands.Color));
        }
        public void InitDashTypeToolBarMenu()
        {
            Clear();
            Name = "Dash Type Tool Bar";
            ToolManager.LookupByCommand(Commands.Solid).Checked = true;
            Tools.Add(ToolManager.LookupByCommand(Commands.Solid));
            Tools.Add(ToolManager.LookupByCommand(Commands.Dashed));
            Tools.Add(ToolManager.LookupByCommand(Commands.Dotted));
        }
        public void InitLineTypeToolBarMenu()
        {
            Clear();
            Name = "Line Type Tool Bar";
            ToolManager.LookupByCommand(Commands.BeeLine).Checked = true;
            Tools.Add(ToolManager.LookupByCommand(Commands.BeeLine));
            Tools.Add(ToolManager.LookupByCommand(Commands.JaggedLine));
            Tools.Add(ToolManager.LookupByCommand(Commands.CurvyLine));
        }
        public void InitCapTypeToolBarMenu()
        {
            Clear();
            Name = "Cap Type Tool Bar";
            ToolManager.LookupByCommand(Commands.EndTypeArrow).Checked = true;
            Tools.Add(ToolManager.LookupByCommand(Commands.EndTypeNone));
            Tools.Add(ToolManager.LookupByCommand(Commands.EndTypeArrow));
            Tools.Add(ToolManager.LookupByCommand(Commands.EndTypeBlock));
            Tools.Add(ToolManager.LookupByCommand(Commands.EndTypeBlockPerson));
        }
        public void InitDrawingModeToolBarMenu()
        {
            Clear();
            Name = "Drawing Mode";
            ToolManager.LookupByCommand(Commands.ToolDragger).Checked = true;
            Tools.Add(ToolManager.LookupByCommand(Commands.ToolDragger));
            Tools.Add(ToolManager.LookupByCommand(Commands.Text));
            Tools.Add(ToolManager.LookupByCommand(Commands.Pen));
        }
        public void Clear()
        {
            Name = string.Empty;

            tools.Clear();
        }

        public static bool EventHandler(Geometry.Drawing drawing, IFigure figure, Tool tool)
        {
            PBPlayer player = figure as PBPlayer;

            int command = tool.Command;

            drawing.SetDefaultBehavior();

            switch (command)
            {
                case Commands.CreateCoachingPoints:
                    if (player != null)
                    {
                        CoachingPointsWindow cpw = new CoachingPointsWindow(player);

                        cpw.Owner = App.Current.MainWindow;

                        cpw.ShowDialog();
                    }
                    break;
                case Commands.InsertBackground:
                    {
                        Microsoft.Win32.OpenFileDialog ofDialog = new Microsoft.Win32.OpenFileDialog()
                        {
                            Title = "Insert Background",
                            Filter = Webb.Playbook.Data.Extensions.ImageFileFilter,
                        };

                        if (ofDialog.ShowDialog().Value)
                        {
                            drawing.BackgroundPath = ofDialog.FileName;

                            drawing.LoadTitleBackground();
                        }
                    }
                    break;
                case Commands.InsertImage:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Image");
                    break;
                case Commands.IntertLabel:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Text");
                    break;
                case Commands.ClearTitle:
                    drawing.BackgroundPath = string.Empty;
                    drawing.LoadTitleBackground();
                    drawing.Clear();
                    break;
                case Commands.FakeOption:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Fake");
                    break;
                case Commands.AssignOption:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Assign");
                    break;
                case Commands.Dragger:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Dragger");
                    break;
                case Commands.Selector:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Selector");
                    break;
                case Commands.RunBlock:
                    player.RemoveForActions();
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Run Block");
                    break;
                case Commands.PassBlock:
                    player.RemoveForActions();
                    (Webb.Playbook.Geometry.Behavior.LookupByName("Pass Block") as PassBlockTool).AddPassBlock(drawing, player);
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Pass Block");
                    break;
                case Commands.PassBlockArea:
                    player.RemoveForActions();
                    (Webb.Playbook.Geometry.Behavior.LookupByName("Pass Block Area") as PassBlockAreaTool).AddPassBlockArea(drawing, player);
                    //drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Pass Block Area");
                    break;
                case Commands.PostSnapMotion:
                    player.RemoveSubPoint();
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Post Snap Motion");
                    break;
                case Commands.EditPreSnapMotion:
                    player.RemoveSubPoint();
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Pre Snap Motion");
                    break;
                case Commands.Set:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Setter");
                    break;
                case Commands.AssignManCoverage:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Assign Man Coverage");
                    break;
                case Commands.AssignManPress:
                    drawing.Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Assign Man Press");
                    break;
            }
            switch (command)
            {
                case Commands.SaveCoachingPoints:
                    {
                        if (player != null)
                        {
                            NameWindow nw = new NameWindow()
                            {
                                Title = "Coaching Points Name",
                                Owner = App.Current.MainWindow,
                            };
                            if (nw.ShowDialog().GetValueOrDefault())
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + @"CoachingPoints\" + nw.FileName;
                                bool bOverwrite = false;
                                if (System.IO.File.Exists(file))
                                {
                                    if (MessageBox.Show("This Coaching Points already exists, do you want to overwrite the existing Coaching Points ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        bOverwrite = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                // save  coaching points
                                System.IO.FileStream fs = System.IO.File.Open(file, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                                foreach (string strLine in player.CoachingPoints.Split('\n'))
                                {
                                    sw.WriteLine(strLine);
                                }
                                sw.Flush();
                                sw.Close();
                                fs.Close();

                                // update UI
                                if (!bOverwrite)
                                {
                                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                    tool.Parent.Add(new Tool(fi.Name, Commands.TextCoachingPoints));
                                }
                            }
                        }
                    }
                    break;
                case Commands.TextCoachingPoints:
                    {
                        if (player != null)
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "CoachingPoints\\" + tool.Text;
                            if (System.IO.File.Exists(file))
                            {
                                // load Coaching Points for player
                                System.IO.FileStream fs = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                System.IO.StreamReader sr = new System.IO.StreamReader(fs);
                                player.CoachingPoints = string.Empty;
                                string strLine = string.Empty;
                                while ((strLine = sr.ReadLine()) != null)
                                {
                                    player.CoachingPoints += strLine + "\n";
                                }
                                sr.Close();
                                fs.Close();
                            }
                        }
                    }
                    break;
                case Commands.ConvertOffToSet:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Set Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Offense\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This set already exists, do you want to overwrite the existing set ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save  offense figures
                            drawing.SaveOff(file);

                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.SetOffense));
                            }
                        }
                    }
                    break;
                case Commands.ConvertDefToSet:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Set Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Defense\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This set already exists, do you want to overwrite the existing set ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save  defense figures
                            drawing.SaveDef(file);

                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.SetDefense));
                            }
                        }
                    }
                    break;
                case Commands.ConvertKickToSet:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Set Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + @"Set\Kicks\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This set already exists, do you want to overwrite the existing set ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save  kick figures
                            drawing.Save(file); //todo

                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.SetKick));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            if (player == null)
            {
                if (figure is PBLabel)
                {
                    PBLabel label = figure as PBLabel;

                    {
                        switch (command)
                        {
                            case Commands.ConvertToLabel:
                                {
                                    NameWindow nw = new NameWindow()
                                    {
                                        Title = "Label Name",
                                        Owner = App.Current.MainWindow,
                                    };
                                    if (nw.ShowDialog().GetValueOrDefault())
                                    {
                                        string file = AppDomain.CurrentDomain.BaseDirectory + "Labels\\" + nw.FileName;
                                        bool bOverwrite = false;
                                        if (System.IO.File.Exists(file))
                                        {
                                            if (MessageBox.Show("This label already exists, do you want to overwrite the existing label ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                            {
                                                bOverwrite = true;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        // save zone from player
                                        label.SaveLabel(file);
                                        // update UI
                                        if (!bOverwrite)
                                        {
                                            System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                            tool.Parent.Add(new Tool(fi.Name, Commands.Label));
                                        }
                                    }
                                }
                                break;
                            case Commands.Label:
                                {
                                    string file = AppDomain.CurrentDomain.BaseDirectory + "Labels\\" + tool.Text;
                                    if (System.IO.File.Exists(file))
                                    {
                                        label.LoadLabel(file);
                                    }
                                }
                                break;
                        }
                    }

                    return true;
                }

                return false;
            }

            switch (command)
            {
                case Commands.CreateZone:
                    {
                        player.ClearPath();
                        player.CreateZone();
                    }
                    break;
                case Commands.Zone:
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "Zones\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load zone for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToZone:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Zone Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "Zones\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This zone already exists, do you want to overwrite the existing zone ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save zone from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Insert(tool.Parent.Count - 1, new Tool(fi.Name, Commands.Zone));
                            }
                        }
                    }
                    break;
                case Commands.RoutePreSnapMotion:
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "PreSnapMotions\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load route for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToPreSnapMotion:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "PreSnapMotion Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "PreSnapMotions\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This PreSnapMotion already exists, do you want to overwrite the existing PreSnapMotion ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save route from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.RoutePreSnapMotion));
                            }
                        }
                    }
                    break;
                case Commands.Route:
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "Routes\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load route for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToRoute:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Route Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "Routes\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This route already exists, do you want to overwrite the existing route ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save route from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.Route));
                            }
                        }
                    }
                    break;
                case Commands.RouteBlock:
                    {
                     
                        string file = AppDomain.CurrentDomain.BaseDirectory + "Blocks\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load route for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToBlock:
                    {
                        IEnumerable<IFigure> figures = drawing.GetSelectedFigures();

                        NameWindow nw = new NameWindow()
                        {
                            Title = "Block Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "Blocks\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This block already exists, do you want to overwrite the existing block ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save route from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.RouteBlock));
                            }
                        }
                    }
                    break;
                case Commands.RouteStuntBlitz:
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "Stunt Blitz\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load route for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToStuntBlitz:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Stunt/Blitz Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "Stunt Blitz\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This Stunt/Blitz already exists, do you want to overwrite the existing Stunt/Blitz ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save route from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.RouteStuntBlitz));
                            }
                        }
                    }
                    break;
                case Commands.RouteAction:
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + tool.Text;
                        if (System.IO.File.Exists(file))
                        {
                            // load route for player
                            player.LoadRoute(file);
                        }
                    }
                    break;
                case Commands.ConvertToAction:
                    {
                        NameWindow nw = new NameWindow()
                        {
                            Title = "Action Name",
                            Owner = App.Current.MainWindow,
                        };
                        if (nw.ShowDialog().GetValueOrDefault())
                        {
                            string file = AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + nw.FileName;
                            bool bOverwrite = false;
                            if (System.IO.File.Exists(file))
                            {
                                if (MessageBox.Show("This action already exists, do you want to overwrite the existing action ?", "Webb Playbook", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    bOverwrite = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // save route from player
                            player.SaveRoute(file);
                            // update UI
                            if (!bOverwrite)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                tool.Parent.Add(new Tool(fi.Name, Commands.RouteAction));
                            }
                        }
                    }
                    break;
                case Commands.Clear:
                case Commands.ClearLines:
                    player.ClearAssignment();
                    break;
                // begin zone
                case Commands.FlatRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.FlatRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.FlatLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.FlatLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.ThirdsDeepRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.ThirdsDeepRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.ThirdsDeepLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.ThirdsDeepLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.ThirdsDeepMiddle:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.ThirdsDeepMiddle;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.HookFullMiddle:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.HookFullMiddle;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.HalvesDeepLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.HalvesDeepLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.HalvesDeepRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.HalvesDeepRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.HookLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.HookLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.HookRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.HookRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlFlatRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlFlatRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlFlatLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlFlatLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.QuartersDeepMidLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.QuartersDeepMidLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.QuartersDeepMidRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.QuartersDeepMidRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.QuartersDeepLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.QuartersDeepLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.QuartersDeepRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.QuartersDeepRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.OutLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.OutLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.OutRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.OutRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.SwingLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.SwingLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.SwingRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.SwingRight;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlHookLeft:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlHookLeft;
                        player.CoverageZone(zone);
                    }
                    break;
                case Commands.CurlHookRight:
                    {
                        player.ClearPath();
                        Zone zone = ZoneManager.CurlHookRight;
                        player.CoverageZone(zone);
                    }
                    break;
                // end zone
                // begin stance
                case Commands.TwoPointLeft:
                    player.Stance = "TwoPointLeft";
                    break;
                case Commands.TwoPointCenter:
                    player.Stance = "TwoPointCenter";
                    break;
                case Commands.TwoPointRight:
                    player.Stance = "TwoPointRight";
                    break;
                case Commands.ThreePointLeft:
                    player.Stance = "ThreePointLeft";
                    break;
                case Commands.ThreePointRight:
                    player.Stance = "ThreePointRight";
                    break;
                case Commands.FourPoint:
                    player.Stance = "FourPoint";
                    break;
                case Commands.ClearStance:
                    player.Stance = string.Empty;
                    break;
                // end stance
                // begin QB motion
                case Commands.DropStepOne:
                    player.QBMotion = "DropStep 1";
                    break;
                case Commands.DropStepThree:
                    player.QBMotion = "DropStep 3";
                    break;
                case Commands.DropStepFive:
                    player.QBMotion = "DropStep 5";
                    break;
                case Commands.DropStepSeven:
                    player.QBMotion = "DropStep 7";
                    break;
                // end QB motion
                // begin notes and calls
                case Commands.AddSnapCall:
                    {
                        NewCall newcall = new NewCall()
                        {
                            Title = "Snap Call",
                        };
                        //newcall.ShowDialog();
                        if (newcall.DialogResult == true)
                        {
                            // to do
                        }
                    }
                    break;

                case Commands.AddDownCall:
                    {
                        NewCall newcall = new NewCall()
                        {
                            Title = "Down Call",
                        };
                        //newcall.ShowDialog();
                        if (newcall.DialogResult == true)
                        {
                            // to do
                        }
                    }
                    break;

                case Commands.EditPlayNote:
                    Grid gridMain = App.Current.MainWindow.Content as Grid;

                    Grid grid = new Grid();
                    grid.Background = Brushes.AliceBlue;
                    grid.Opacity = 0.5;
                    Grid.SetColumnSpan(grid, 10);
                    Grid.SetRowSpan(grid, 10);
                    gridMain.Children.Add(grid);

                    RichTextBox rtb = new RichTextBox();
                    rtb.Width = 600;
                    rtb.HorizontalAlignment = HorizontalAlignment.Center;
                    rtb.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumnSpan(rtb, 10);
                    Grid.SetRowSpan(rtb, 10);
                    gridMain.Children.Add(rtb);
                    break;
                // end notes and calls
                default:
                    break;
            }
            return true;
        }
    }

    public class Tool : Webb.Playbook.Data.NotifyObj
    {
        public void Select()
        {
            if (Parent != null)
            {
                foreach (Tool t in Parent)
                {
                    if (t != this)
                    {
                        t.Checked = false;
                    }
                    else
                    {
                        t.Checked = true;
                    }
                }
            }
        }

        public Tool(string strText, int nCommand)
        {
            Text = strText;

            Command = nCommand;

            string imageFile = string.Empty;

            if (nCommand >= 50 && nCommand <= 74 || nCommand == Commands.CreateZone || nCommand == Commands.ConvertToZone || nCommand == Commands.Zone)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Zone";
            }
            else if (nCommand == Commands.ConvertToRoute || nCommand == Commands.Route || nCommand == Commands.ConvertToBlock || nCommand == Commands.RouteBlock || nCommand == Commands.ConvertToAction || nCommand == Commands.RouteAction || nCommand == Commands.RoutesBlock || nCommand == Commands.RoutesAction || nCommand == Commands.RouteStuntBlitz || nCommand == Commands.ConvertToStuntBlitz || nCommand == Commands.RoutesStuntBlitz)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Routes";
            }
            else if (nCommand == Commands.SetDefense || nCommand == Commands.SetOffense || nCommand == Commands.SetKick || nCommand == Commands.ConvertDefToSet || nCommand == Commands.ConvertOffToSet || nCommand == Commands.ConvertKickToSet)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Routes";
            }
            else if (nCommand == Commands.SetDefenses || nCommand == Commands.SetOffenses || nCommand == Commands.SetKicks)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Routes";
            }
            else if (nCommand == Commands.NotesAndCalls || nCommand == Commands.AddDownCall || nCommand == Commands.AddSnapCall)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Calls";
            }
            else if (nCommand == Commands.EditPlayNote)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Notes";
            }
            else if (nCommand == Commands.NewFolder)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Folder";
            }
            else if (nCommand == Commands.SubstitutePosition)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Offense";
            }
            else if (nCommand == Commands.Label || nCommand == Commands.ConvertToLabel || nCommand == Commands.CreateNewLabel)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Text";
            }
            else if (nCommand >= Commands.VideoRev && nCommand <= Commands.VideoStop)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Video\" + Text;
            }
            else if (nCommand == Commands.ClearLines)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Clear";
            }
            else if (nCommand == Commands.CreateCoachingPoints || nCommand == Commands.SaveCoachingPoints || nCommand == Commands.TextCoachingPoints)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Coaching Points";
            }
            else if (nCommand == Commands.ConvertToPreSnapMotion || nCommand == Commands.RoutePreSnapMotion || nCommand == Commands.EditPreSnapMotion)
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Pre Snap Motion";
            }
            else
            {
                imageFile = AppDomain.CurrentDomain.BaseDirectory + @"Resource\" + Text;
            }

            if (System.IO.File.Exists(imageFile + ".png"))
            {
                Image = imageFile + ".png";
            }
            else
            {
                Image = imageFile + ".ico";
            }

            if (nCommand == Commands.Route || nCommand == Commands.RouteBlock || nCommand == Commands.RouteAction || nCommand == Commands.SetOffense || nCommand == Commands.SetDefense || nCommand == Commands.SetKick || nCommand == Commands.Zone || nCommand == Commands.RouteStuntBlitz || nCommand == Commands.Label || nCommand == Commands.TextCoachingPoints || nCommand == Commands.RoutePreSnapMotion)
            {
                VisibilityDelete = System.Windows.Visibility.Visible;
            }
        }

        private bool isChecked = false;
        public bool Checked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;

                OnPropertyChanged("Checked");
            }
        }

        private Brush color;
        public Brush Color
        {
            get
            {
                if (Command == Commands.Color)
                {
                    try
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(Text.Replace(" ", string.Empty));
                        if (color != null)
                        {
                            return new SolidColorBrush(color);
                        }
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }

        private string image;
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set 
            { 
                enable = value;

                if (enable)
                {
                    VisibilityEnableIcon = Visibility.Hidden;
                }
                else
                {
                    VisibilityEnableIcon = Visibility.Visible;
                }
            }
        }

        private Visibility visibilityEnableIcon = Visibility.Hidden;
        public Visibility VisibilityEnableIcon
        {
            get { return visibilityEnableIcon; }
            set { visibilityEnableIcon = value; }
        }

        private int command;
        public int Command
        {
            get { return command; }
            set { command = value; }
        }

        private ToolCollection<Tool> parent;
        public ToolCollection<Tool> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private System.Windows.Visibility visibilityDelete = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility VisibilityDelete
        {
            get { return visibilityDelete; }
            set { visibilityDelete = value; }
        }

        private Object toolTip;
        public Object ToolTip
        {
            get
            {
                if (Command == Commands.Route || Command == Commands.RouteBlock || Command == Commands.RouteAction || Command == Commands.RouteStuntBlitz || Command == Commands.RoutePreSnapMotion)
                {
                    if (toolTip == null)
                    {
                        toolTip = new Grid();
                        Grid gridToolTip = toolTip as Grid;
                        gridToolTip.Width = 100;
                        gridToolTip.Height = 100;

                        string file = string.Empty;

                        switch (Command)
                        {
                            case Commands.Route:
                                file = AppDomain.CurrentDomain.BaseDirectory + "Routes\\" + Text;
                                break;
                            case Commands.RouteBlock:
                                file = AppDomain.CurrentDomain.BaseDirectory + "Blocks\\" + Text;
                                break;
                            case Commands.RouteAction:
                                file = AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + Text;
                                break;
                            case Commands.RouteStuntBlitz:
                                file = AppDomain.CurrentDomain.BaseDirectory + "Stunt Blitz\\" + Text;
                                break;
                            case Commands.RoutePreSnapMotion:
                                file = AppDomain.CurrentDomain.BaseDirectory + "PreSnapMotions\\" + Text;
                                break; 
                        }

                        if (System.IO.File.Exists(file))
                        {
                            Path path = new Path();
                            PathGeometry pg = new PathGeometry();
                            path.Data = pg;

                            PBRoute route = new PBRoute();
                            route.ReadXml(file);

                            EllipseGeometry ellipse = new EllipseGeometry(new Point(50, 50), 5, 5);
                            pg.AddGeometry(ellipse);

                            Point ptStart = new Point(50, 50);
                            Point ptEnd = new Point();

                            foreach (PBRoutePoint pbPoint in route.Path)
                            {
                                Point pt = pbPoint.Point;
                                ptEnd.X = 50 + pt.Scale(2.0).X;
                                ptEnd.Y = 50 - pt.Scale(2.0).Y;
                                LineGeometry line = new LineGeometry(ptStart, ptEnd);
                                ptStart = ptEnd;
                                pg.AddGeometry(line);
                            }

                            path.Stroke = Brushes.Red;
                            path.Fill = Brushes.LightBlue;
                            gridToolTip.Children.Add(path);
                        }
                    }
                }
                else
                {
                    if (Enable)
                    {
                        return Text;
                    }
                    else
                    {
                        return Text + "(Disabled)";
                    }
                }

                return toolTip;
            }
        }
    }

    public class Commands
    {
        public const int UnDefine = 0;

        public const int Assignments = 1;
        public const int Stances = 2;
        public const int Zones = 3;
        public const int Routes = 4;
        public const int Actions = 5;

        public const int Selector = 6;
        public const int Dragger = 7;
        public const int Pen = 8;

        public const int RunBlock = 11;
        public const int PassBlock = 12;
        public const int PassBlockArea = 13;

        public const int PreSnapMotion = 14;
        public const int PostSnapMotion = 15;

        public const int AssignManPress = 16;
        public const int AssignManCoverage = 17;
        public const int ShadeLeft = 18;
        public const int ShadeRight = 19;

        public const int Set = 20;
        public const int DropStepOne = 21;
        public const int DropStepThree = 22;
        public const int DropStepFive = 23;
        public const int DropStepSeven = 24;

        public const int TwoPointLeft = 31;
        public const int TwoPointCenter = 32;
        public const int TwoPointRight = 33;
        public const int ThreePointLeft = 34;
        public const int ThreePointRight = 35;
        public const int FourPoint = 36;

        public const int FlatLeft = 50;
        public const int CurlLeft = 51;
        public const int HookLeft = 52;
        public const int HookFullMiddle = 53;
        public const int HookRight = 54;
        public const int CurlRight = 55;
        public const int FlatRight = 56;
        public const int CurlFlatLeft = 57;
        public const int CurlHookLeft = 58;
        public const int CurlHookRight = 59;
        public const int CurlFlatRight = 60;
        public const int ThirdsDeepRight = 61;
        public const int ThirdsDeepMiddle = 62;
        public const int ThirdsDeepLeft = 63;
        public const int HalvesDeepRight = 64;
        public const int HalvesDeepLeft = 65;
        public const int QuartersDeepLeft = 66;
        public const int QuartersDeepMidLeft = 67;
        public const int QuartersDeepMidRight = 68;
        public const int QuartersDeepRight = 69;
        public const int SwingRight = 70;
        public const int SwingLeft = 71;
        public const int OutRight = 72;
        public const int OutLeft = 73;
        public const int Post = 74;

        public const int AssignHandoffLeft = 101;
        public const int AssignHandoffRight = 102;
        public const int TossLeft = 103;
        public const int TossRight = 104;
        public const int AssignOption = 105;
        public const int FakeHandoffLeft = 106;
        public const int FakeHandoffRight = 107;
        public const int FakeTossLeft = 108;
        public const int FakeTossRight = 109;
        public const int FakeOption = 110;

        public const int NotesAndCalls = 150;
        public const int EditPlayNote = 151;
        public const int AddDownCall = 152;
        public const int AddSnapCall = 153;

        public const int ConvertToStuntBlitz = 195;
        public const int CreateZone = 196;
        public const int ConvertToZone = 197;
        public const int ConvertToBlock = 198;
        public const int ConvertToAction = 199;
        public const int ConvertToRoute = 200;
        public const int Route = 201;
        public const int RouteBlock = 202;
        public const int RouteAction = 203;
        public const int Zone = 204;
        public const int RouteStuntBlitz = 205;
        public const int EditPreSnapMotion = 206;
        public const int ConvertToPreSnapMotion = 207;
        public const int RoutePreSnapMotion = 208;

        //set
        public const int SetDefense = 300;
        public const int SetOffense = 301;
        public const int ConvertDefToSet = 302;
        public const int ConvertOffToSet = 303;
        public const int SetDefenses = 304;
        public const int SetOffenses = 305;

        // 10-27-2011 Scott
        public const int ConvertKickToSet = 306;
        public const int SetKick = 307;
        public const int SetKicks = 308;
        // end

        public const int ClearLines = 998;
        public const int Clear = 999;
        public const int ClearStance = 1000;

        public const int AddPosition = 1200;
        public const int SubstitutePosition = 1201; // 07-19-2010 Scott
        public const int RemovePosition = 1202;

        public const int HelpTopics = 2000;
        public const int Open = 2001;
        public const int Save = 2002;
        public const int Gridlines = 2003;
        public const int ThreeDView = 2004;
        public const int Print = 2005;
        public const int PlayAni = 2006;
        public const int PauseAni = 2007;
        public const int StopAni = 2008;
        public const int ResumeAni = 2009;
        public const int Still = 2010;

        //HReverse
        public const int HReverse = 2011;
        public const int DeffHreverse = 2012;
        public const int OffHreverse = 2013;
        //VReverse
        public const int VReverse = 2015;

        //LineType
        public const int BeeLine = 2020;
        public const int JaggedLine = 2021;
        public const int CurvyLine = 2022;

        public const int Text = 2023;   // 10-27-2010 Scott

        public const int Rename = 2050;
        public const int Delete = 2051;
        public const int SaveAs = 2052;

        public const int NewFormation = 2100;
        public const int NewFolder = 2101;
        public const int NewPlay = 2102;
        public const int NewAdjustment = 2103;
        public const int CreatePlay = 2104;
        public const int New = 2105;    // 11-17-2010 Scott

        public const int Team = 2500;
        public const int Personnel = 2501;


        // 08-02-2010 Scott
        public const int Undo = 5000;
        public const int Redo = 5001;

        public const int Animation = 5100;
        public const int CloseAnimation = 5101;
        public const int Presentation = 5102;
        public const int ClosePresentation = 5103;

        public const int SwitchRoutes = 5200;

        public const int CoachingPoints = 6000; // 09-27-2010 Scott
        public const int TextCoachingPoints = 6001;
        public const int CreateCoachingPoints = 6002;
        public const int SaveCoachingPoints = 6003;

        public const int Color = 10000; // 11-23-2010 Scott

        public const int Solid = 10010;
        public const int Dashed = 10011;
        public const int Dotted = 10012;

        public const int EndTypeArrow = 10500;
        public const int EndTypeBlock = 10501;
        public const int EndTypeBlockPerson = 10502;

        public const int ToolDragger = 11000;

        public const int RoutesStuntBlitz = 11009;
        public const int RoutesAction = 11010;
        public const int RoutesBlock = 11011;

        public const int EndTypeNone = 11012;
        public const int Ball = 11013;
        public const int Snap = 11014;

        public const int SaveBaseFormation = 12000;
        public const int LoadBaseFormation = 12001;

        public const int Report = 13000;

        public const int ConvertToLabel = 15000;
        public const int Label = 15001;
        public const int CreateNewLabel = 15002;
        public const int CreatePreSnapMotion = 15004;

        // 05-30-2011 Scott
        public const int InsertBackground = 16001;
        public const int InsertImage = 16002;
        public const int IntertLabel = 16003;
        public const int ClearTitle = 16004;


        public const int VideoRev = 16100;
        public const int VideoRPlay = 16101;
        public const int VideoRSlow = 16102;
        public const int VideoStill = 16103;
        public const int VideoSlow = 16104;
        public const int VideoPlay = 16105;
        public const int VideoFF = 16106;
        public const int VideoStop = 16107;
        
    }
}