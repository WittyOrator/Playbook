using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class AdjustmentRootViewModel
    {
        protected List<ScoutTypeViewModel> scoutTypes;

        public List<ScoutTypeViewModel> ScoutTypes
        {
            get { return scoutTypes; }
        }

        private static TreeViewItemViewModel selectedViewModel = null;
        public static TreeViewItemViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; }
        }

        public AdjustmentRootViewModel(string userFolder)
        {
            scoutTypes = new List<ScoutTypeViewModel>();

            foreach (string strScoutType in Enum.GetNames(typeof(ScoutTypes)))
            {
                ScoutTypeViewModel stvm = new ScoutTypeViewModel(new ScoutTypeDirectory(strScoutType, (int)Mode.Adjustment, userFolder));
                stvm.IsExpanded = true;
                scoutTypes.Add(stvm);
            }
        }
    }
}