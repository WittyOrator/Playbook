using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class VideoViewModel : TreeViewItemViewModel
    {
        private VideoCoachingPointInfo videoInfo;
        public VideoCoachingPointInfo VideoInfo
        {
            get { return videoInfo; }
            set { videoInfo = value; }
        }

        public VideoViewModel(VideoCoachingPointInfo vcpi, TreeViewItemViewModel tvivm)
            : base(tvivm, false)
        {
            videoInfo = vcpi;

            videoPath = vcpi.VideoPath;

            this.Image = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Video.png";
        }

        public string VideoName
        {
            get 
            {
                return System.IO.Path.GetFileName(VideoPath);
            }
        }

        private string videoPath = string.Empty;
        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();
        }
    }
}
