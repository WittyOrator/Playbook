using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class FolderViewModel : TreeViewItemViewModel
    {
        private string folderPath = string.Empty;
        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; }
        }

        public string FolderName    // 11-15-2010 Scott
        {
            get
            {
                int index = FolderPath.LastIndexOf(@"\");

                if (index >= 0)
                {
                    return FolderPath.Remove(0, index + 1);
                }
                else
                {
                    return FolderPath;
                }
            }
        }

        public string Name
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(FolderPath);

                if (di != null)
                {
                    return di.Name;
                }

                return string.Empty;
            }
        }

        public bool HaveChildren // 09-14-2010 Scott
        {
            get
            {
                return !(base.Children.Count() == 0 || base.Children.Count() == 1 && base.Children.First() is ViewModel.TreeViewItemViewModel);
            }
        }

        public FolderViewModel(string strName, TreeViewItemViewModel tviv)
            : base(tviv, true, strName)
        {
            if (Directory.Exists(strName))
            {
                FolderPath = strName;
            }
            else if (tviv is ScoutTypeViewModel)
            {
                FolderPath = (tviv as ScoutTypeViewModel).ScoutTypePath + "\\" + strName;
            }
            else if (tviv is FolderViewModel)
            {
                FolderPath = (tviv as FolderViewModel).FolderPath + "\\" + strName;
            }

            this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\CloseFolder.ico";
        }

        public override void OnExpanded(bool bExpanded)
        {
            base.OnExpanded(bExpanded);

            if (bExpanded)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Folder.ico";
            }
            else
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\CloseFolder.ico";
            }
        }

        public override void RemoveChild(TreeViewItemViewModel child)
        {
            if (child is FolderViewModel)
            {
                if (Directory.Exists((child as FolderViewModel).FolderPath))
                {
                    InteropHelper.DeleteToRecycleBin((child as FolderViewModel).FolderPath, false);
                    //Directory.Delete((child as FolderViewModel).FolderPath, true);
                }
            }
            else
            {
                FormationViewModel fvm = child as FormationViewModel;

                if (fvm != null)
                {
                    InteropHelper.DeleteToRecycleBin(fvm.FormationPath, false);
                    //System.IO.File.Delete(fvm.FormationPath);

                    if (File.Exists(fvm.FormationPath + ".FD"))
                    {
                        InteropHelper.DeleteToRecycleBin(fvm.FormationPath + ".FD", false);
                    }

                    if (File.Exists(fvm.FormationPath + ".BMP"))
                    {
                        InteropHelper.DeleteToRecycleBin(fvm.FormationPath + ".BMP", false);
                    }

                    // 09-19-2010 Scott
                    if (Directory.Exists(fvm.PlayFolderPath))
                    {
                        InteropHelper.DeleteToRecycleBin(fvm.PlayFolderPath, false);
                    }
                }

                TitleViewModel tvm = child as TitleViewModel;

                if (tvm != null)
                {
                    if (File.Exists(tvm.TitlePath))
                    {
                        InteropHelper.DeleteToRecycleBin(tvm.TitlePath, false);

                        if (File.Exists(tvm.TitlePath + ".FD"))
                        {
                            InteropHelper.DeleteToRecycleBin(tvm.TitlePath + ".FD", false);
                        }
                    }
                }
            }

            base.RemoveChild(child);
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
                    if (string.Compare(formationVM.FormationName, (tviv as FormationViewModel).FormationName) <= 0)
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

        protected override void LoadChildren()
        {
            base.LoadChildren();
            
            if (Directory.Exists(FolderPath))
            {
                foreach (string strFile in Directory.GetFiles(FolderPath, "*.Ttl"))
                {
                    Title title = new Title(strFile);

                    TitleViewModel tvm = new TitleViewModel(title, this);

                    base.Children.Add(tvm);
                }

                foreach (string file in Directory.GetFiles(FolderPath, "*.Form"))
                {
                    Formation formation = new Formation(file);

                    base.Children.Add(new FormationViewModel(formation, this));
                }

                foreach (string directory in Directory.GetDirectories(FolderPath))
                {
                    if (directory.EndsWith("@"))
                    {
                        continue;
                    }

                    FolderViewModel fvm = new FolderViewModel(directory, this);

                    base.Children.Add(fvm);
                }
            }
        }

        // 09-19-2010 Scott
        protected override bool HasChildren()
        {
            bool bHasChildren = false;

            if (Directory.Exists(ChildrenPath))
            {
                bHasChildren = Directory.GetFiles(ChildrenPath, "*.Form").Count() > 0 || Directory.GetFiles(ChildrenPath, "*.Ttl").Count() > 0 || Directory.GetDirectories(ChildrenPath).Count() > 0;
            }

            return bHasChildren;
        }
    }
}
