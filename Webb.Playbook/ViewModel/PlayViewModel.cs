using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PlayViewModel : TreeViewItemViewModel
    {
        protected Play play;
        public Play Play
        {
            get { return play; }
        }

        protected PlayInfo playInfo;
        public PlayInfo PlayInfo
        {
            get { return playInfo; }
        }

        // 09-15-2010 Scott
        protected static bool showFormation = true;
        public static bool ShowFormation
        {
            get { return showFormation; }
            set { showFormation = value; }
        }

        public PlayViewModel(Play play, TreeViewItemViewModel tvivm)
            : base(tvivm, false)
        {
            this.play = play;

            // 09-15-2010 Scott
            PlayInfo playInfo = new PlayInfo();
            playInfo.Load(play.Path + ".PlayInfo");
            this.playInfo = playInfo;

            FormationViewModel fvm = Parent as FormationViewModel;
            if (fvm != null)
            {
                if (fvm.Image.Contains("Off"))
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Play.ico";
                }

                if (fvm.Image.Contains("Def"))
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Play.ico";
                }

                // 10-26-2011 Scott
                if (fvm.Image.Contains("Kick"))
                {
                    this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Play.ico";
                }
            }
            else
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Play.ico";
            }

            ToolTip = play.Name;
        }

        public void CopyTo(string strFolder, bool overwrite)
        {
            string strNewFile = strFolder + @"\" + PlayName + @".Play";

            if (File.Exists(strNewFile) && !overwrite)
            {
                return;
            }

            if(File.Exists(PlayPath))
            {
                File.Copy(PlayPath, strNewFile, true);
            }
            if (File.Exists(PlayPath + @".FD"))
            {
                File.Copy(PlayPath + @".FD", strNewFile + @".FD", true);
            }
            if (File.Exists(PlayPath + @".BMP"))
            {
                File.Copy(PlayPath + @".BMP", strNewFile + @".BMP", true);
            }
            if (File.Exists(PlayPath + @".PlayInfo"))
            {
                File.Copy(PlayPath + @".PlayInfo", strNewFile + @".PlayInfo", true);
            }
        }

        public string PlayName
        {
            get { return play.Name; }
        }

        public string PlayPath
        {
            get { return play.Path; }
        }

        public string OffensiveFormation
        {
            get
            {
                if (PlayInfo != null)
                {
                    return PlayInfo.OffensiveFormation;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string DefensiveFormation
        {
            get
            {
                if (PlayInfo != null)
                {
                    return PlayInfo.DefensiveFormation;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string OffensiveImage
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
            }
        }

        public string DefensiveImage
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"Resource\Def Formation.ico";
            }
        }

        public string KickImage
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"Resource\Kick Formation.ico";
            }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            // 09-15-2010 Scott
            //if (ShowFormation && this.Parent is PlayTypeViewModel)
            //{
            //    PVFormationViewModel offViewModel = new PVFormationViewModel(ScoutTypes.Offensive, this);
            //    PVFormationViewModel defViewModel = new PVFormationViewModel(ScoutTypes.Defensive, this);
            //    this.Children.Add(offViewModel);
            //    this.Children.Add(defViewModel);
            //}
        }
    }
}
