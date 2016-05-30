using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public class Play
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string offensiveFormation;  // 12-21-2009 scott
        public string OffensiveFormation
        {
            get { return offensiveFormation; }
            set { offensiveFormation = value; }
        }

        private string defensiveFormation;  // 12-21-2009 scott
        public string DefensiveFormation
        {
            get { return defensiveFormation; }
            set { defensiveFormation = value; }
        }

        public Play()
        {

        }

        public Play(string strPath)
        {
            Path = strPath;
            
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }
    }
}
