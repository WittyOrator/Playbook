using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PVFormationViewModel : TreeViewItemViewModel
    {
        protected ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return scoutType; }
            set { scoutType = value; }
        }

        protected string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public PVFormationViewModel(ScoutTypes scoutType, PlayViewModel ptm)
            : base(ptm, false)
        {
            ScoutType = scoutType;

            Path = ptm.PlayPath;

            if (scoutType == ScoutTypes.Offensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Off Formation.ico";
                this.Name = ptm.OffensiveFormation;
            }

            if (scoutType == ScoutTypes.Defensive)
            {
                this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Def Formation.ico";
                this.Name = ptm.DefensiveFormation;
            }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();
        }
    }
}
