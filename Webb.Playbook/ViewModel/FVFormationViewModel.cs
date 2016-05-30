using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class FVFormationViewModel : TreeViewItemViewModel
    {
        protected FVFormationModel fVFFormation;
        public FVFormationModel FVFFormation
        {
            get
            {
                return fVFFormation;
            }
            set
            {
                fVFFormation = value;
            }
        }

        private string formationName;
        public string FormationName
        {
            get { return formationName; }
            set { formationName = value; }
        }

        public FVFormationViewModel(FVFormationModel fVFFormation, ScoutTypeViewModel stvm)
            : base(stvm, false)
        {
            FVFFormation = fVFFormation;

            if (stvm.ScoutType.ScoutType == ScoutTypes.Offensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
            }

            if (stvm.ScoutType.ScoutType == ScoutTypes.Defensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Def Formation.ico";
            }

            this.formationName = FVFFormation.FormationName;
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            foreach (string strPlay in FVFFormation.Plays)
            {
                Play play = new Play(strPlay);

                PlayViewModel pvm = new PlayViewModel(play, this);

                base.Children.Add(pvm);
            }
        }
    }
}

