using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public class PowerPoint
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

        public string ImagePath
        {
            get
            {
                string strPath = AppDomain.CurrentDomain.BaseDirectory + @"\PowerPoint\" + Name;

                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                return strPath;
            }
        }

        public PowerPoint()
        {

        }

        public PowerPoint(string strPath)
        {
            Path = strPath;

            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }
    }
}
