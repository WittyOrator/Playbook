using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class TitleViewModel : TreeViewItemViewModel
    {
        protected Title title;
        public Title Title
        {
            get { return title; }
        }

        public TitleViewModel(Title title, TreeViewItemViewModel tvivm)
            : base(tvivm, false)
        {
            this.title = title;

            this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Title.png";

            ToolTip = title.Name;
        }

        // 10-27-2011 Scott
        public void CopyTo(string strFolder, bool bOverwrite)
        {
            string strNewFile = strFolder + @"\" + TitleName + @".TTL";

            if(File.Exists(strNewFile) && !bOverwrite)
            {
                return;
            }

            if(File.Exists(TitlePath))
            {
                File.Copy(TitlePath,strNewFile,true);
            }

            if(File.Exists(TitlePath + ".FD"))
            {
                File.Copy(TitlePath + ".FD",strNewFile + ".FD",true);
            }
        }

        public string TitleName
        {
            get { return title.Name; }
        }

        public string TitlePath
        {
            get { return title.Path; }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

        }
    }
}
