using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class AdjustmentViewModel : TreeViewItemViewModel
    {
        private string folderPath = string.Empty;
        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; }
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

        public string FileName
        {
            get
            {
                return this.Name;
            }
            set
            {
                return;
            }
        }

        public AdjustmentViewModel(string strName, TreeViewItemViewModel tviv)
            : base(tviv, true)
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

                    // 09-19-2010 Scott
                    if (Directory.Exists(fvm.PlayFolderPath))
                    {
                        InteropHelper.DeleteToRecycleBin(fvm.PlayFolderPath, false);
                    }
                }
            }

            base.RemoveChild(child);
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.GetFiles(FolderPath, "*.Form"))
                {
                    Formation formation = new Formation(file);

                    base.Children.Add(new FormationViewModel(formation, this));
                }

                foreach (string directory in Directory.GetDirectories(FolderPath))
                {
                    FolderViewModel fvm = new FolderViewModel(directory, this);

                    base.Children.Add(fvm);
                }
            }
        }
    }
}
