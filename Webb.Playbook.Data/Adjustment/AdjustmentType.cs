using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Webb.Playbook.Data
{
    public enum AdjustmentTypes
    {
        // offense
        Patterns = 0,
        Shifts = 2,
        BlockingScheme = 4,
        // defense
        Coverages = 1,
        Blitzes = 3,
        Stunts = 5,
        Dogs = 7,
    }

    public enum OffAdjustmentTypes
    {
        // offense
        Patterns = 0,
        Shifts = 2,
        BlockingScheme = 4,
    }

    public enum DefAdjustmentTypes
    {
        // defense
        Coverages = 1,
        Blitzes = 3,
        Stunts = 5,
        Dogs = 7,
    }

    public class AdjustmentTypeDirectory
    {
        private string userFolder;
        public string UserFolder
        {
            get
            {
                return userFolder;
            }
        }

        public AdjustmentTypeDirectory(string strAdjustmentType, ScoutTypes scoutType, string userFolder)
        {
            this.scoutType = scoutType;

            adjustmentType = (AdjustmentTypes)Enum.Parse(typeof(AdjustmentTypes), strAdjustmentType);

            this.userFolder = userFolder;
        }

        private ScoutTypes scoutType;
        public ScoutTypes ScoutType
        {
            get { return this.scoutType; }
        }

        private AdjustmentTypes adjustmentType;
        public AdjustmentTypes AdjustmentType
        {
            get { return adjustmentType; }
            set { adjustmentType = value; }
        }

        public string Path
        {
            get { return GetPath(); }
        }

        private string GetPath()
        {
            string strPath = userFolder + @"\Adjustment\" + scoutType.ToString() + @"\" + adjustmentType.ToString();

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            return strPath;
        }

        public string Name
        {
            get { return adjustmentType.ToString(); }
        }
    }
}
