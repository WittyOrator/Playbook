using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PPTViewModel : TreeViewItemViewModel
    {
        protected PowerPoint ppt;
        public PowerPoint Ppt
        {
            get { return ppt; }
        }

        public PPTViewModel(PowerPoint ppt, TreeViewItemViewModel tvivm)
            : base(tvivm, false)
        {
            this.ppt = ppt;

            this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Ppt.png";

            ToolTip = ppt.Name;
        }

        public string PptName
        {
            get { return ppt.Name; }
        }

        public string PptPath
        {
            get { return ppt.Path; }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();
        }
    }
}
