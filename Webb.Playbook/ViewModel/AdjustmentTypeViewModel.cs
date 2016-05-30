using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class AdjustmentTypeViewModel : TreeViewItemViewModel
    {
        private AdjustmentTypeDirectory adjustmentTypeDirectory;
        public AdjustmentTypeDirectory AdjustmentTypeDirectory
        {
            get { return adjustmentTypeDirectory; }
            set { adjustmentTypeDirectory = value; }
        }

        private string folderPath = string.Empty;
        public string FolderPath
        {
            get { return folderPath; }
        }

        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }
        }

        public AdjustmentTypeViewModel(AdjustmentTypeDirectory adjustmentTypeDirectory, TreeViewItemViewModel tviv)
            : base(tviv, true)
        {
            this.adjustmentTypeDirectory = adjustmentTypeDirectory;

            name = AdjustmentTypeDirectory.AdjustmentType.ToString();
            folderPath = AdjustmentTypeDirectory.Path;

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

            foreach (string strFile in Directory.GetFiles(FolderPath, "*.Play"))
            {
                Play play = new Play(strFile);
                base.Children.Add(new PlayViewModel(play, this));
            }
        }
    }
}
