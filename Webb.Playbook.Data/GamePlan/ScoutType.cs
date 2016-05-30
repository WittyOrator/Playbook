using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public enum ScoutTypes
    {
        Offensive = 0,
        Defensive = 1,
        Kicks = 2,   // 10-26-2011 Scott
    }

    public enum UserTypes
    {
        Offensive = 0,
        Defensive = 1,
        Kicks = 2,
    }

    public class ScoutTypeDirectory
    {
        private string userFolder = string.Empty;
        public string UserFolder
        {
            get
            {
                return userFolder;
            }
        }

        private int mode;
        public int Mode
        {
            get { return mode; }
        }

        public ScoutTypeDirectory(string strScoutType,int mode, string userFolder)
        {
            scoutType = (ScoutTypes)Enum.Parse(typeof(ScoutTypes), strScoutType);

            this.mode = mode;   // 08-30-2010 Scott

            this.userFolder = userFolder;
        }

        private ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return scoutType; }
            set { scoutType = value; }
        }

        public string Path
        {
            get { return GetPath(); }
        }

        private string GetPath()
        {
            string strPath = userFolder + (mode == 0 ? @"\Formation\" : @"\Adjustment\") + scoutType.ToString();    // 08-30-2010 Scott

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            return strPath;
        }

        public string Name
        {
            get { return scoutType.ToString(); }
        }
    }
}
