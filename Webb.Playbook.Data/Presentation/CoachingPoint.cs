using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using System.Windows.Shapes;

namespace Webb.Playbook.Data
{
    public class VideoCoachingPoint
    {
        private Shape shape;
        public Shape Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        private string videoPath = string.Empty;
        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; }
        }

        private int frame = -1;
        public int Frame
        {
            get { return frame; }
            set { frame = value; }
        }

        private string path = string.Empty;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string drawingFile = "drawing";
        public string DrawingFile
        {
            get { return Path + @"\" + drawingFile; }
        }

        private string audioFile = "audio.wav";
        public string AudioFile
        {
            get { return Path + @"\" + audioFile; }
        }

        public override string ToString()
        {
            return string.Format("Coaching Point : {0}", VideoHelper.GetTimeString(Frame));
        }

        public override bool Equals(object obj)
        {
            bool bEqual = false;

            VideoCoachingPoint otherVCP = obj as VideoCoachingPoint;
            
            if (otherVCP != null)
            {
                bEqual = this.Frame == otherVCP.Frame;
            }

            return bEqual;
        }

        public void CreatePath()
        {
            for(int i = 1; ; i++)
            {
                string strPath = ProductInfo.PresentationPath + @"\" + i.ToString();

                if (!System.IO.Directory.Exists(strPath))
                {
                    System.IO.Directory.CreateDirectory(strPath);

                    Path = strPath;

                    break;
                }
            }
        }

        public VideoCoachingPoint(string strVideoPath, int nFrame, bool bCreatePath)
        {
            videoPath = strVideoPath;

            frame = nFrame;

            if (bCreatePath)
            {
                CreatePath();
            }
        }

        public VideoCoachingPoint(string strVideoPath, bool bCreatePath)
        {
            videoPath = strVideoPath;

            if (bCreatePath)
            {
                CreatePath();
            }
        }

        public void WriteXML(XElement elem)
        {
            XElement elemVideoCoachingPoint = new XElement("VideoCoachingPoint");

            elemVideoCoachingPoint.Add(new XAttribute("Frame", Frame));

            elemVideoCoachingPoint.Add(new XAttribute("Path", Path));

            elem.Add(elemVideoCoachingPoint);
        }

        public void ReadXML(XElement elem)
        {
            if (elem.Name == "VideoCoachingPoint")
            {
                Frame = Int32.Parse(elem.Attribute("Frame").Value);

                Path = elem.Attribute("Path").Value;
            }
        }

        public VideoCoachingPoint Copy()
        {
            VideoCoachingPoint vcp = new VideoCoachingPoint(VideoPath, Frame, false);

            vcp.Path = Path;

            vcp.Shape = Shape;

            return vcp;
        }

        public void Apply(VideoCoachingPoint vcp)
        {
            this.Frame = vcp.Frame;
            this.Path = vcp.Path;
            this.VideoPath = vcp.VideoPath;
        }
    }

    public class VideoCoachingPointInfo
    {
        public static double ShapeSize = 12;

        private string videoPath = string.Empty;
        public string VideoPath
        {
            get { return videoPath; }
        }

        public override string ToString()
        {
            return VideoPath;
        }

        public VideoCoachingPointInfo(string strVideoPath)
        {
            videoPath = strVideoPath;
        }

        private List<VideoCoachingPoint> videoCoachingPoints;
        public List<VideoCoachingPoint> VideoCoachingPoints
        {
            get
            {
                if (videoCoachingPoints == null)
                {
                    videoCoachingPoints = new List<VideoCoachingPoint>();
                }

                return videoCoachingPoints;
            }
        }

        public void WriteXML(XElement elem)
        {
            XElement elemVideoCoachingPointInfo = new XElement("VideoCoachingPointInfo");

            foreach (VideoCoachingPoint vcp in VideoCoachingPoints)
            {
                vcp.WriteXML(elemVideoCoachingPointInfo);
            }

            elem.Add(elemVideoCoachingPointInfo);
        }

        public void ReadXML(XElement elem)
        {
            VideoCoachingPoints.Clear();

            if (elem.Name == "VideoCoachingPointInfo")
            {
                foreach (XElement elemVideoCoachingPoint in elem.Elements("VideoCoachingPoint"))
                {
                    VideoCoachingPoint vcp = new VideoCoachingPoint(VideoPath, false);

                    vcp.ReadXML(elemVideoCoachingPoint);

                    this.VideoCoachingPoints.Add(vcp);
                }
            }
        }

        public bool Contains(VideoCoachingPoint vcp)
        {
            foreach (VideoCoachingPoint v in this.VideoCoachingPoints)
            {
                if (v.Equals(vcp))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(int nFrame)
        {
            foreach (VideoCoachingPoint v in this.VideoCoachingPoints)
            {
                if (v.Frame == nFrame)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveCoachingPoint(VideoCoachingPoint vcp)
        {
            this.VideoCoachingPoints.Remove(vcp);

            if (System.IO.Directory.Exists(vcp.Path))
            {
                System.IO.Directory.Delete(vcp.Path, true);
            }
        }

        public VideoCoachingPoint CheckFrame(int nFrame)
        {
            VideoCoachingPoint bRet = null;

            foreach(VideoCoachingPoint vcp in this.VideoCoachingPoints)
            {
                if (nFrame <= vcp.Frame + 20 && nFrame >= vcp.Frame - 20)
                {
                    bRet = vcp;

                    break;
                }
            }

            return bRet;
        }

        public VideoCoachingPoint CheckFrame(System.Windows.Point pt)
        {
            VideoCoachingPoint bRet = null;

            foreach (VideoCoachingPoint vcp in this.VideoCoachingPoints)
            {
                if (pt.X > vcp.Shape.Margin.Left && pt.X < vcp.Shape.Margin.Left + vcp.Shape.ActualWidth)
                {
                    bRet = vcp;

                    break;
                }
            }

            return bRet;
        }
    }
}
