using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public enum PlayTypes
    {
        Run,
        Pass,
    }

    public class PlayTypeDirectory
    {
        private string userFolder;
        public string UserFolder
        {
            get
            {
                return userFolder;
            }
            set
            {
                userFolder = value;
            }
        }

        public PlayTypeDirectory(string strPlayType, string userFolder)
        {
            playType = (PlayTypes)Enum.Parse(typeof(PlayTypes), strPlayType);

            this.userFolder = userFolder;
        }

        private PlayTypes playType;
        public PlayTypes PlayType
        {
            get { return playType; }
            set { playType = value; }
        }

        public string Path
        {
            get { return GetPath(); }
        }

        private string GetPath()
        {
            string strPath = userFolder + @"\Playbook\" + playType.ToString();

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            return strPath;
        }

        public string Name
        {
            get { return playType.ToString(); }
        }
    }
}
