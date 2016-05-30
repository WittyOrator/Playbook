using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class FormationRootViewModel
    {
        protected List<ScoutTypeViewModel> scoutTypes;

        public List<ScoutTypeViewModel> ScoutTypes
        {
            get
            {
                if (scoutTypes == null)
                {
                    scoutTypes = new List<ScoutTypeViewModel>();
                }
                return scoutTypes; 
            }
        }

        private static TreeViewItemViewModel selectedViewModel = null;
        public static TreeViewItemViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; }
        } 

        public FormationRootViewModel(string userFolder)
        {
            Refresh(userFolder);
        }

        public void Refresh(string userFolder)
        {
            ScoutTypes.Clear();

            foreach (string strScoutType in Enum.GetNames(typeof(ScoutTypes)))
            {
                ScoutTypeViewModel stv = new ScoutTypeViewModel(new ScoutTypeDirectory(strScoutType, (int)Mode.Formation, userFolder));
                stv.IsExpanded = true;
                ScoutTypes.Add(stv);
            }
        }
    }
}
