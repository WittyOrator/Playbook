using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public class Title
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

        public Title()
        {

        }

        public Title(string strPath)
        {
            Path = strPath;

            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }
    }
}
