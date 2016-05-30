using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PlayFolderViewModel : TreeViewItemViewModel
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

        public PlayFolderViewModel(string strName, TreeViewItemViewModel tviv)
            : base(tviv, true, strName)
        {
            if (Directory.Exists(strName))
            {
                FolderPath = strName;
            }
            else if (tviv is PlayTypeViewModel)
            {
                FolderPath = (tviv as PlayTypeViewModel).PlayTypePath + "\\" + strName;
            }
            else if (tviv is PlayFolderViewModel)
            {
                FolderPath = (tviv as PlayFolderViewModel).FolderPath + "\\" + strName;
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
            if (child is PlayFolderViewModel)
            {
                if (Directory.Exists((child as PlayFolderViewModel).FolderPath))
                {
                    InteropHelper.DeleteToRecycleBin((child as PlayFolderViewModel).FolderPath, false);
                    //Directory.Delete((child as FolderViewModel).FolderPath, true);
                }
            }
            else
            {
                PlayViewModel pvm = child as PlayViewModel;

                if (pvm != null)
                {
                    InteropHelper.DeleteToRecycleBin(pvm.PlayPath, false);

                    if (File.Exists(pvm.PlayPath + ".FD"))
                    {
                        InteropHelper.DeleteToRecycleBin(pvm.PlayPath + ".FD", false);
                    }

                    if (File.Exists(pvm.PlayPath + ".BMP"))
                    {
                        InteropHelper.DeleteToRecycleBin(pvm.PlayPath + ".BMP", false);
                    }

                    if (File.Exists(pvm.PlayPath + ".PlayInfo"))
                    {
                        InteropHelper.DeleteToRecycleBin(pvm.PlayPath + ".PlayInfo", false);
                    }
                }
            }

            base.RemoveChild(child);
        }

        // 09-21-2010 Scott
        protected override void AddChildByOrder(TreeViewItemViewModel child)
        {
            bool bFolder = child is PlayFolderViewModel;
            bool bPlay = child is PlayViewModel;

            PlayFolderViewModel folderVM = child as PlayFolderViewModel;
            PlayViewModel playVM = child as PlayViewModel;

            int index = 0;

            foreach (TreeViewItemViewModel tviv in Children)
            {
                if (tviv is PlayViewModel && bPlay)
                {
                    if (string.Compare(playVM.PlayName, (tviv as PlayViewModel).PlayName) <= 0)
                    {
                        break;
                    }
                }

                if (tviv is PlayFolderViewModel && bFolder)
                {
                    if (string.Compare(folderVM.Name, (tviv as PlayFolderViewModel).Name) <= 0)
                    {
                        break;
                    }
                }

                if (tviv is PlayFolderViewModel && bPlay)
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
                foreach (string file in Directory.GetFiles(FolderPath, "*.Play"))
                {
                    Play play = new Play(file);

                    base.Children.Add(new PlayViewModel(play, this));
                }

                foreach (string directory in Directory.GetDirectories(FolderPath))
                {
                    PlayFolderViewModel fvm = new PlayFolderViewModel(directory, this);

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
                bHasChildren = Directory.GetFiles(ChildrenPath, "*.Play").Count() > 0 || Directory.GetDirectories(ChildrenPath).Count() > 0;
            }

            return bHasChildren;
        }
    }
}
