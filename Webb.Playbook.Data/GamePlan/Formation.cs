using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Webb.Playbook.Data
{
    public class Formation
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

        public Formation()
        {

        }

        public Formation(string strPath)
        {
            Path = strPath;
            FileInfo fi = new FileInfo(strPath);
            int index = fi.Name.LastIndexOf('.');
            if (index > 0)
            {
                Name = fi.Name.Substring(0, index);
            }
            else
            {
                Name = fi.Name;
            }
        }
    }
}
