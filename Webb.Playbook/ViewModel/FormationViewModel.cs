using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class FormationViewModel : TreeViewItemViewModel
    {
        protected Formation formation;
        public Formation Formation
        {
            get { return formation; }
        }

        public string PlayFolderPath
        {
            get
            {
                string strPlayFolderPath = FormationPath.Remove(FormationPath.IndexOf(".Form")) + "@";

                if (!Directory.Exists(strPlayFolderPath))
                {
                    Directory.CreateDirectory(strPlayFolderPath);
                }

                return strPlayFolderPath;
            }
        }

        public static string GetPlayFolderPathByFormation(Formation formation)
        {
            int index = formation.Path.IndexOf(".Form");

            if (index > 0)
            {
                string strPlayFolderPath = formation.Path.Remove(index) + "@";

                if (Directory.Exists(strPlayFolderPath))
                {
                    return strPlayFolderPath;
                }
                else
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public FormationViewModel(Formation formation, TreeViewItemViewModel tviv)
            : base(tviv, true, GetPlayFolderPathByFormation(formation))
        {
            this.formation = formation;

            if(tviv != null)
            {
                TreeViewItemViewModel parentVM = Parent;

                while (true)
                {
                    if (parentVM == null) break;

                    if (!(parentVM is ViewModel.ScoutTypeViewModel))
                    {
                        parentVM = parentVM.Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                if (parentVM is ViewModel.ScoutTypeViewModel)
                {
                    ScoutTypeDirectory std = (parentVM as ViewModel.ScoutTypeViewModel).ScoutType;
                    if (std.ScoutType == ScoutTypes.Offensive)
                    {
                        this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
                    }
                    else if (std.ScoutType == ScoutTypes.Defensive)
                    {
                        this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Def Formation.ico";
                    }
                    else if (std.ScoutType == ScoutTypes.Kicks)
                    {// 10-26-2011 Scott
                        this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Kick Formation.ico";
                    }
                }
                else
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
                }

                ToolTip = formation.Name;
            }
            else
            {
                string strStandardFilePath = new FileInfo(formation.Path).FullName;

                if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Offensive")
                || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Offensive")
                || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Offensive"))
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
                }
                else if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Defensive")
                    || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Defensive")
                    || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Defensive"))
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Def Formation.ico";
                }
                else if (strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Offensive\Formation\Kicks")
                    || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Defensive\Formation\Kicks")
                    || strStandardFilePath.Contains(AppDomain.CurrentDomain.BaseDirectory + @"Kicks\Formation\Kicks"))
                {// 10-26-2011 Scott
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Kick Formation.ico";
                }
                else
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
                }
            }
        }

        public string FormationName
        {
            get { return formation.Name; }
            set { ; }
        }

        public string FormationPath
        {
            get { return formation.Path; }
        }

        public override void RemoveChild(TreeViewItemViewModel child)
        {// 09-19-2010 Scott
            base.RemoveChild(child);

            PlayViewModel pvm = child as PlayViewModel;
            if (pvm != null)
            {
                if (File.Exists(pvm.PlayPath))
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
        }

        public void CopyTo(string strFolder, bool overwrite)
        {
            string strNewFile = strFolder + @"\" + FormationName + @".Form";
            string strNewPlayFolder = strFolder + @"\" + FormationName + "@";

            if(File.Exists(strNewFile) && !overwrite)
            {
                return;
            }

            if (File.Exists(FormationPath))
            {
                File.Copy(FormationPath, strNewFile, true);
            }

            if (File.Exists(FormationPath + @".FD"))
            {
                File.Copy(FormationPath + @".FD", strNewFile + @".FD", true);
            }

            if (File.Exists(FormationPath + @".BMP"))
            {
                File.Copy(FormationPath + @".BMP", strNewFile + @".BMP", true);
            }

            if(Directory.Exists(strNewPlayFolder))
            {
                Directory.Delete(strNewPlayFolder, true);
            }

            Directory.CreateDirectory(strNewPlayFolder);

            CopyDirectory.CopyFolderFiles(PlayFolderPath, strNewPlayFolder, true,null);
        }

        protected override void AddChildByOrder(TreeViewItemViewModel child)
        {
            bool bPlay = child is PlayViewModel;

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

                index++;
            }

            this.Children.Insert(index, child);
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            string strPlayFolder = this.FormationPath.Remove(this.FormationPath.IndexOf(".Form")) + "@";

            if (Directory.Exists(strPlayFolder))
            {
                foreach (string strPlay in Directory.GetFiles(strPlayFolder,"*.Play"))
                {
                    Play play = new Play(strPlay);

                    base.Children.Add(new PlayViewModel(play, this));
                }
            }
        }

        // 09-19-2010 Scott
        protected override bool HasChildren()
        {
            bool bHasChildren = false;

            if (Directory.Exists(ChildrenPath))
            {
                bHasChildren = Directory.GetFiles(ChildrenPath, "*.Play").Count() > 0;
            }

            return bHasChildren;
        }
    }
}
