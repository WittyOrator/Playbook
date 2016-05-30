using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public enum ViewMode
    {
        PlayNameView,
        FormationNameView,
    }

    public class PlaybookRootViewModel
    {
        protected List<PlayTypeViewModel> playTypes = new List<PlayTypeViewModel>();
        public List<PlayTypeViewModel> PlayTypes
        {
            get { return playTypes; }
        }

        protected List<ScoutTypeViewModel> scoutTypes = new List<ScoutTypeViewModel>();
        public List<ScoutTypeViewModel> ScoutTypes
        {
            get { return scoutTypes; }
        }

        protected ViewMode viewMode = ViewMode.PlayNameView;
        public ViewMode ViewMode
        {
            get { return viewMode; }
            set { viewMode = value; }
        }

        private static TreeViewItemViewModel selectedViewModel = null;
        public static TreeViewItemViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; }
        }

        public PlaybookRootViewModel(string userFolder, ViewMode viewMode)
        {
            ViewMode = viewMode;

            if (ViewMode == ViewMode.PlayNameView)
            {
                playTypes.Clear();

                PlayTypeViewModel ptvm = new PlayTypeViewModel(new PlayTypeDirectory(Enum.GetName(typeof(Webb.Playbook.Data.PlayTypes),Webb.Playbook.Data.PlayTypes.Run),userFolder));
                ptvm.IsExpanded = true;
                playTypes.Add(ptvm);
                //foreach (string strPlayType in Enum.GetNames(typeof(PlayTypes)))
                //{
                //    PlayTypeViewModel ptvm = new PlayTypeViewModel(new PlayTypeDirectory(strPlayType));
                //    ptvm.IsExpanded = true;
                //    playTypes.Add(ptvm);
                //}
            }

            if (ViewMode == ViewMode.FormationNameView)
            {
                scoutTypes.Clear();

                foreach (string strScoutType in Enum.GetNames(typeof(ScoutTypes)))
                {
                    ScoutTypeViewModel stvm = new ScoutTypeViewModel(new ScoutTypeDirectory(strScoutType, (int)Mode.Playbook, userFolder));
                    stvm.IsExpanded = true;
                    scoutTypes.Add(stvm);
                }
            }
        }

        public void Refresh(string userFolder)
        {
            if (ViewMode == ViewMode.PlayNameView)
            {
                foreach (PlayTypeViewModel ptvm in PlayTypes)
                {
                    ptvm.Refresh(userFolder);
                }
            }

            if (ViewMode == ViewMode.FormationNameView)
            {
                foreach (ScoutTypeViewModel stv in ScoutTypes)
                {
                    stv.Refresh();
                }
            }
        }
    }
}
