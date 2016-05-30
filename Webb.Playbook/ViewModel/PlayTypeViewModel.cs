using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PlayTypeViewModel : TreeViewItemViewModel
    {
        protected PlayTypeDirectory playType;
        public PlayTypeDirectory PlayType
        {
            get { return playType; }
        }

        public string PlayTypeName
        {
            get
            {
                return "Play";
                //return playType.Name; 
            }
        }

        public string PlayTypePath
        {
            get 
            {
                string strPlayPath = playType.UserFolder + @"\Playbook";

                if (!System.IO.Directory.Exists(strPlayPath))
                {
                    System.IO.Directory.CreateDirectory(strPlayPath);
                }

                return strPlayPath;
                //return playType.Path;
            }
            set
            {
                playType.UserFolder = value;
            }
        }

        public void Refresh(string userFolder)
        {
            PlayTypePath = userFolder;
            Refresh();
        }

        public PlayTypeViewModel(PlayTypeDirectory playType)
            : base(null, true)
        {
            this.playType = playType;

            if (playType.PlayType == PlayTypes.Run)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Run.ico";

                this.IsSelected = true; // 07-19-2010 Scott
            }
            else if (playType.PlayType == PlayTypes.Pass)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Pass.ico";
            }
        }

        public override void AddChild(TreeViewItemViewModel child)
        {
            base.AddChild(child);
        }

        // 09-21-2010
        protected override void AddChildByOrder(TreeViewItemViewModel child)
        {
            bool bPlay = child is PlayViewModel;
            bool bFolder = child is PlayFolderViewModel;

            PlayFolderViewModel folderVM = child as PlayFolderViewModel;
            PlayViewModel playVM = child as PlayViewModel;

            int index = 0;

            foreach (TreeViewItemViewModel tviv in Children)
            {
                if (bPlay && tviv is PlayViewModel)
                {
                    if (string.Compare(playVM.PlayName, (tviv as PlayViewModel).PlayName) < 0)
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

            foreach (string strFile in Directory.GetFiles(PlayTypePath, "*.Play"))
            {
                Play play = new Play(strFile);
                PlayViewModel pvm = new PlayViewModel(play, this);
                base.Children.Add(pvm);
                pvm.IsExpanded = true;
            }

            foreach (string strDir in Directory.GetDirectories(PlayTypePath))
            {
                base.Children.Add(new PlayFolderViewModel(strDir, this));
            }
        }

        public override void RemoveChild(TreeViewItemViewModel child)
        {
            PlayViewModel pvm = child as PlayViewModel;

            if (pvm != null)
            {
                System.IO.File.Delete(pvm.PlayPath);
            }

            PlayFolderViewModel folderVM = child as PlayFolderViewModel;

            if (folderVM != null && Directory.Exists(folderVM.FolderPath))
            {
                InteropHelper.DeleteToRecycleBin(folderVM.FolderPath, false);
                //Directory.Delete(folderVM.FolderPath, true);
            }

            base.RemoveChild(child);
        }
    }
}
