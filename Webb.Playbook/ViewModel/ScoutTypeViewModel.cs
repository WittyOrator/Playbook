using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class FVFormationModel
    {
        private string formationName;
        public string FormationName
        {
            get { return formationName; }
            set { formationName = value; }
        }

        private ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return scoutType; }
            set { scoutType = value; }
        }

        private List<string> plays;
        public List<string> Plays
        {
            get
            {
                if (plays == null)
                {
                    plays = new List<string>();
                }
                return plays;
            }
        }

        public FVFormationModel(string formationName, ScoutTypes scoutType)
        {
            FormationName = formationName;

            ScoutType = scoutType;
        }

        public void ClearPlays()
        {
            Plays.Clear();
        }

        public void AddPlay(string strPlay)
        {
            Plays.Add(strPlay);
        }

        public void RemovePlay(string strPlay)
        {
            if (Plays.Contains(strPlay))
            {
                Plays.Remove(strPlay);
            }
        }
    }

    public class FVFormationModelCollection
    {
        private ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return scoutType; }
            set { scoutType = value; }
        }

        private List<FVFormationModel> formations;
        public List<FVFormationModel> Formations
        {
            get
            {
                if (formations == null)
                {
                    formations = new List<FVFormationModel>();
                }

                return formations;
            }
        }

        public FVFormationModelCollection(ScoutTypes scoutType)
        {
            ScoutType = scoutType;
        }

        public void AddPlayInfo(PlayInfo playInfo)
        {
            string strFormationName = string.Empty;

            if(ScoutType == ScoutTypes.Offensive)
            {
                strFormationName = playInfo.OffensiveFormation;
            }

            if(ScoutType == ScoutTypes.Defensive)
            {
                strFormationName = playInfo.DefensiveFormation;
            }

            FVFormationModel fvfM = Formations.Find(f => f.FormationName == strFormationName);

            if (fvfM == null)
            {
                fvfM = new FVFormationModel(strFormationName, ScoutType);

                Formations.Add(fvfM);
            }

            fvfM.AddPlay(playInfo.PlayName);
        }

        public void Load(string userFolder)
        {
            string strRunPlayPath = userFolder + @"\Playbook\Run";

            string strPassPlayPath = userFolder + @"\Playbook\Pass";

            foreach (string strRunPlay in Directory.GetFiles(strRunPlayPath, "*.Play"))
            {
                PlayInfo playInfo = new PlayInfo();

                playInfo.Load(strRunPlay + ".PlayInfo");

                AddPlayInfo(playInfo);
            }

            foreach (string strPassPlay in Directory.GetFiles(strPassPlayPath, "*.Play"))
            {
                PlayInfo playInfo = new PlayInfo();
                
                playInfo.Load(strPassPlay + ".PlayInfo");

                AddPlayInfo(playInfo);
            }
        }

        public void Clear()
        {
            Formations.Clear();
        }
    }

    public class ScoutTypeViewModel : TreeViewItemViewModel
    {
        protected FVFormationModelCollection fVFormations;
        public FVFormationModelCollection FVFormations
        {
            get { return fVFormations; }
            set { fVFormations = value; }
        }

        protected ScoutTypeDirectory scoutType;
        public ScoutTypeDirectory ScoutType
        {
            get { return scoutType; }
        }

        public string ScoutTypeName
        {
            get 
            {
                if (scoutType.ScoutType == ScoutTypes.Kicks)
                {// 10-26-2011 Scott
                    return "Special Teams";
                }
                return scoutType.Name; 
            }
        }

        public string ScoutTypePath
        {
            get { return scoutType.Path; }
        }

        public ScoutTypeViewModel(ScoutTypeDirectory scoutType)
            : base(null, true)
        {
            this.scoutType = scoutType;

            // sorting
            //System.ComponentModel.ICollectionView view = System.Windows.Data.CollectionViewSource.GetDefaultView(Children);
            //view.SortDescriptions.Add(new System.ComponentModel.SortDescription("PersonnelName", System.ComponentModel.ListSortDirection.Ascending));

            if (scoutType.ScoutType == ScoutTypes.Offensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Offense.ico";

                this.IsSelected = true; // 07-19-2010 Scott
            }
            else if (scoutType.ScoutType == ScoutTypes.Defensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Defense.ico";
            }

            // 10-26-2011 Scott
            if (scoutType.ScoutType == ScoutTypes.Kicks)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Kicks.ico";
            }
        }

        public override void AddChild(TreeViewItemViewModel child)
        {
            base.AddChild(child);
        }

        // 09-21-2010 Scott
        protected override void AddChildByOrder(TreeViewItemViewModel child)
        {
            bool bFolder = child is FolderViewModel;
            bool bFormation = child is FormationViewModel;
            bool bTitle = child is TitleViewModel;

            FolderViewModel folderVM = child as FolderViewModel;
            FormationViewModel formationVM = child as FormationViewModel;
            TitleViewModel titleVM = child as TitleViewModel;

            int index = 0;

            if (ScoutType.Mode == (int)Mode.Formation)
            {
                foreach (TreeViewItemViewModel tviv in Children)
                {
                    if (tviv is TitleViewModel && bTitle)
                    {
                        if (string.Compare(titleVM.TitleName, (tviv as TitleViewModel).TitleName) <= 0)
                        {
                            break;
                        }
                    }

                    if (tviv is FormationViewModel && bFormation)
                    {
                        if ( string.Compare(formationVM.FormationName,(tviv as FormationViewModel).FormationName) <= 0)
                        {
                            break;
                        }
                    }

                    if (tviv is FolderViewModel && bFolder)
                    {
                        if (string.Compare(folderVM.Name, (tviv as FolderViewModel).Name) <= 0)
                        {
                            break;
                        }
                    }

                    if (tviv is FolderViewModel && bFormation)
                    {
                        break;
                    }

                    if ((tviv is FormationViewModel || tviv is FolderViewModel) && bTitle)
                    {
                        break;
                    }

                    index++;
                }

                this.Children.Insert(index, child);
            }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            //foreach (string strDir in Directory.GetDirectories(ScoutTypePath))
            //{
            //    Personnel personnel = new Personnel(strDir);
            //    base.Children.Add(new PersonnelViewModel(personnel, this));
            //}

            if (ScoutType.Mode == (int)Mode.Formation)
            {
                foreach (string strFile in Directory.GetFiles(ScoutTypePath, "*.Ttl"))
                {
                    Title title = new Title(strFile);

                    TitleViewModel tvm = new TitleViewModel(title, this);

                    base.Children.Add(tvm);
                }

                foreach (string strFile in Directory.GetFiles(ScoutTypePath, "*.Form"))
                {
                    Formation formation = new Formation(strFile);

                    FormationViewModel fvm = new FormationViewModel(formation, this);

                    base.Children.Add(fvm);
                }

                foreach (string strDir in Directory.GetDirectories(ScoutTypePath))
                {
                    if (strDir.EndsWith("@"))
                    {
                        continue;
                    }

                    base.Children.Add(new FolderViewModel(strDir, this));
                }
            }

            // 08-30-2010 Scott
            if (ScoutType.Mode == (int)Mode.Adjustment)
            {
                if (ScoutType.ScoutType == ScoutTypes.Offensive)
                {
                    foreach (string strAdjustmentType in Enum.GetNames(typeof(OffAdjustmentTypes)))
                    {
                        AdjustmentTypeViewModel atv = new AdjustmentTypeViewModel(new AdjustmentTypeDirectory(strAdjustmentType, ScoutType.ScoutType, ScoutType.UserFolder), this);

                        base.Children.Add(atv);
                    }
                }

                if (ScoutType.ScoutType == ScoutTypes.Defensive)
                {
                    foreach (string strAdjustmentType in Enum.GetNames(typeof(DefAdjustmentTypes)))
                    {
                        AdjustmentTypeViewModel atv = new AdjustmentTypeViewModel(new AdjustmentTypeDirectory(strAdjustmentType, ScoutType.ScoutType, ScoutType.UserFolder), this);

                        base.Children.Add(atv);
                    }
                }
            }

            if (ScoutType.Mode == (int)Mode.Playbook)
            {
                FVFormations = new FVFormationModelCollection(ScoutType.ScoutType);
                
                FVFormations.Load(ScoutType.UserFolder);

                foreach (FVFormationModel fvf in FVFormations.Formations)
                {
                    FVFormationViewModel fvfVM = new FVFormationViewModel(fvf, this);

                    base.Children.Add(fvfVM);

                    fvfVM.IsExpanded = true;
                }
            }
        }

        public override void RemoveChild(TreeViewItemViewModel child)
        {
            if (ScoutType.Mode == (int)Mode.Formation)
            {
                FormationViewModel formationVM = child as FormationViewModel;

                if (formationVM != null)
                {
                    InteropHelper.DeleteToRecycleBin(formationVM.FormationPath, false);
                    //File.Delete(formationVM.FormationPath);

                    if (File.Exists(formationVM.FormationPath + ".FD"))
                    {
                        InteropHelper.DeleteToRecycleBin(formationVM.FormationPath + ".FD", false);
                    }

                    if (File.Exists(formationVM.FormationPath + ".BMP"))
                    {
                        InteropHelper.DeleteToRecycleBin(formationVM.FormationPath + ".BMP", false);
                    }

                    // 09-19-2010 Scott
                    if (Directory.Exists(formationVM.PlayFolderPath))
                    {
                        InteropHelper.DeleteToRecycleBin(formationVM.PlayFolderPath, false);
                    }
                }

                FolderViewModel folderVM = child as FolderViewModel;

                if (folderVM != null && Directory.Exists(folderVM.FolderPath))
                {
                    InteropHelper.DeleteToRecycleBin(folderVM.FolderPath, false);
                    //Directory.Delete(folderVM.FolderPath, true);
                }

                TitleViewModel tvm = child as TitleViewModel;

                if (tvm != null && File.Exists(tvm.TitlePath))
                {
                    InteropHelper.DeleteToRecycleBin(tvm.TitlePath, false);

                    if (File.Exists(tvm.TitlePath + ".FD"))
                    {
                        InteropHelper.DeleteToRecycleBin(tvm.TitlePath + ".FD", false);
                    }
                }

                //PersonnelViewModel pvm = child as PersonnelViewModel;

                //if (pvm != null)
                //{
                //    Directory.Delete(pvm.PersonnelPath,true);
                //}
            }

            // 08-30-2010 Scott
            if (ScoutType.Mode == (int)Mode.Adjustment)
            {
                
            }

            base.RemoveChild(child);
        }
    }
}
