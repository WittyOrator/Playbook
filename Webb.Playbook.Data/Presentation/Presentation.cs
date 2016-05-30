using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Webb.Playbook.Data
{
    public class Presentation
    {
        public Presentation()
        {
            Name = string.Empty;

            Plays.Clear();
        }

        public Presentation(string name, List<PresentationPlay> pPlays)
        {
            Name = name;

            Plays.Clear();

            foreach (PresentationPlay pPlay in pPlays)
            {
                Plays.Add(pPlay);
            }
        }

        private IList<PresentationPlay> plays;
        public IList<PresentationPlay> Plays
        {
            get
            {
                if (plays == null)
                {
                    plays = new List<PresentationPlay>();
                }
                return plays;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public void Load(string file)
        {
            if (File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);

                XElement elemPresentation = doc.Element("Presentation");

                Plays.Clear();

                if (elemPresentation != null)
                {
                    Name = elemPresentation.Attribute("Name").Value;

                    XElement elemFiles = elemPresentation.Element("Files");
                    if (elemFiles != null)
                    {
                        foreach (XElement elem in elemFiles.Elements("File"))
                        {
                            PresentationPlay play = new PresentationPlay();

                            play.ReadXML(elem);

                            Plays.Add(play);
                        }
                    }
                }
            }
        }

        public void Save(string file)
        {
            XDocument doc = new XDocument();
            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");
            doc.Declaration = declare;

            XElement elemPresentation = new XElement("Presentation");
            XAttribute attri = new XAttribute("Name", Name);
            elemPresentation.Add(attri);
            doc.Add(elemPresentation);

            XElement elemFiles = new XElement("Files");
            foreach (PresentationPlay play in Plays)
            {
                play.WriteXML(elemFiles);
            }
            elemPresentation.Add(elemFiles);

            doc.Save(file);
        }
    }

    public class PresentationPlay
    {
        public string PlayName
        {
            get 
            {
                if (System.IO.File.Exists(PlayPath))
                {
                    return System.IO.Path.GetFileNameWithoutExtension(PlayPath);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private string playPath;
        public string PlayPath
        {
            get { return playPath; }
            set { playPath = value; }
        }

        private List<object> videos;
        public List<object> Videos
        {
            get
            {
                if (videos == null)
                {
                    videos = new List<object>();
                }
                return videos;
            }
        }

        public PresentationPlay()
        {
            PlayPath = string.Empty;

            Videos.Clear();
        }

        public void WriteXML(XElement elem)
        {
            XElement elemFile = new XElement("File");
            XAttribute attri = new XAttribute("Path", PlayPath);
            elemFile.Add(attri);

            foreach (object objVideo in Videos)
            {
                XElement elemVideo = new XElement("Video");
                attri = new XAttribute("Path", objVideo.ToString());
                elemVideo.Add(attri);

                // 11-30-2011 Scott
                if (objVideo is VideoCoachingPointInfo)
                {
                    (objVideo as VideoCoachingPointInfo).WriteXML(elemVideo);
                }
                // end

                elemFile.Add(elemVideo);
            }
            
            elem.Add(elemFile);
        }

        public void ReadXML(XElement elem)
        {
            if (elem.Name == "File")
            {
                PlayPath = elem.Attribute("Path").Value;

                Videos.Clear();

                foreach (XElement elemVideo in elem.Elements("Video"))
                {
                    string strVideoPath = elemVideo.Attribute("Path").Value;

                    // 11-30-2011 Scott
                    XElement elemVideoCoachingPointInfo = elemVideo.Element("VideoCoachingPointInfo");
                    // end

                    if (elemVideoCoachingPointInfo != null)
                    {// 11-30-2011 Scott
                        VideoCoachingPointInfo vcpi = new VideoCoachingPointInfo(strVideoPath);
                        vcpi.ReadXML(elemVideoCoachingPointInfo);
                        Videos.Add(vcpi);
                    }
                    else
                    {
                        Videos.Add(strVideoPath);
                    }
                }
            }
        }
    }
}
