using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

using System.IO;

namespace Webb.Playbook.Data
{
    public static class Extensions
    {
        public static string[] VideoFiles = { "avi", "mpg2", "mpg", "mpeg", "m2t", "m2p", "wmv", "mlv", "mp2", "ts", "tp", "vob", "mp4", "m4a", "3gp", "3ga", "asf", "wm", "mov", "mxf", "m2v", "mts", "mod" };
        public static string[] ImageFiles = { "jpg","bmp","png","gif" };
        
        public static string VideoFileFilter
        {
            get
            {
                string strVideoFileFilter = string.Empty;

                strVideoFileFilter += string.Format("Video Files (All Types)|");

                foreach (string videoFile in VideoFiles)
                {
                    strVideoFileFilter += string.Format("*.{0};", videoFile);
                }

                return strVideoFileFilter;
            }
        }

        public static string PackageFilter  // 09-26-2011 Scott
        {
            get
            {
                string strPackageFilter = "Package File (.pac)|*.pac";

                return strPackageFilter;
            }
        }

        public static string ImageFileFilter
        {
            get
            {
                string strImageFileFilter = string.Empty;

                strImageFileFilter += string.Format("Image Files (All Types)|");

                foreach (string imageFile in ImageFiles)
                {
                    strImageFileFilter += string.Format("*.{0};", imageFile);
                }

                return strImageFileFilter;
            }
        }

        // 08-30-2011 Scott
        public static double ToDouble(this string str, double defaultValue)
        {
            double ret = 0;

            try
            {
                ret = double.Parse(str);
            }
            catch
            {
                ret = defaultValue;
            }

            return ret;
        }

        public static void WriteElementColor(this System.Xml.Linq.XElement element, string elementName, System.Windows.Media.Color color)
        {
            XElement elemColor = new XElement("Color");

            elemColor.Add(new XAttribute("Name", elementName));
            elemColor.Add(new XAttribute("R", color.R.ToString()));
            elemColor.Add(new XAttribute("G", color.G.ToString()));
            elemColor.Add(new XAttribute("B", color.B.ToString()));

            element.Add(elemColor);
        }

        public static void WriteElementColor(this System.Xml.XmlWriter writer, string elementName, System.Windows.Media.Color color)
        {
            writer.WriteStartElement("Color");
            writer.WriteAttributeString("Name", elementName);
            writer.WriteAttributeString("R", color.R.ToString());
            writer.WriteAttributeString("G", color.G.ToString());
            writer.WriteAttributeString("B", color.B.ToString());
            writer.WriteEndElement();
        }

        public static System.Windows.Media.Color ReadElementColor(this XElement element, string elementName)
        {
            System.Windows.Media.Color color = System.Windows.Media.Colors.Transparent;

            XElement elem = element.GetChildElement(elementName);

            color = System.Windows.Media.Color.FromRgb(byte.Parse(elem.Attribute("R").Value), byte.Parse(elem.Attribute("G").Value), byte.Parse(elem.Attribute("B").Value));

            return color;
        }

        public static XElement GetChildElement(this XElement element, string elementName)
        {
            XElement elem = element.Elements().Single(f => 
                    f.Attribute("Name") != null && f.Attribute("Name").Value.Equals(elementName));

            return elem;
        }

        public static void WriteAttributeDouble(this System.Xml.XmlWriter writer, string attributeName, double value)
        {
            writer.WriteAttributeString(attributeName, value.ToStringInvariant());
        }

        public static void WriteAttributeInt(this System.Xml.XmlWriter writer, string attributeName, int value)
        {
            writer.WriteAttributeString(attributeName, value.ToStringInvariant());
        }

        public static double ReadDouble(this XElement element, string attributeName)
        {
            double result = 0;
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                double.TryParse(
                    attribute.Value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result);
            }
            return result;
        }

        public static int ReadInt(this XElement element, string attributeName)
        {
            int result = 0;
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                int.TryParse( 
                    attribute.Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out result);
            }
            return result;
        }

        public static string ReadString(this XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                return attribute.Value;
            }
            return null;
        }

        public static string ToStringInvariant(this int number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this double number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum ShowWindowCommands : int
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_MAX = 10
    }

    public enum MessageCommands : int
    {
        WM_MOVE = 3,
        WM_SIZE = 5,
        WM_SETFOCUS = 7,
        WM_CLOSE = 16,
        WM_USER = 1024,
        WM_OPENGAME = 1025,
        WM_CLOSEGAME = 1026,
        WM_REMOTE = 1027,

        WM_MOUSEMOVE = 512,
        WM_LBUTTONDOWN = 513,
        WM_LBUTTONUP = 514,
        WM_LBUTTONDBLCLK = 515,
        WM_RBUTTONDOWN = 516,
        WM_RBUTTONUP = 517,
        WM_RBUTTONDBLCLK = 518,

        WM_KEYDOWN = 256,
    }

    public class MessageHelper
    {
        public static System.Windows.Point GetPosition(IntPtr lParam)
        {
            int nPosX = (lParam.ToInt32() & 65535);
            int nPosY = (lParam.ToInt32() >> 16);

            return new System.Windows.Point(nPosX,nPosY);
        }
    }

    public class IOHelper
    {
        public static void ChangeCaseForFileName(string oldPath, string newPath)
        {
            FileInfo fi = new FileInfo(oldPath);

            if (fi.Exists)
            {
                fi.MoveTo(newPath);
            }
        }

        public static bool CopyFile(string oldPath, string newPath, bool overwrite)
        {
            if (string.Compare(oldPath, newPath, true) == 0)
            {
                ChangeCaseForFileName(oldPath, newPath);

                return false;
            }
            else
            {
                File.Copy(oldPath, newPath, overwrite);

                return true;
            }
        }

        public static bool MoveFolder(string oldPath, string newPath)
        {
            if (string.Compare(oldPath, newPath, true) == 0)
            {
                ChangeCaseForFileName(oldPath, newPath);

                return false;
            }
            else
            {
                Directory.Move(oldPath, newPath);

                return true;
            }
        }

        public static bool FileEqual(string pathA, string pathB)
        {
            if (string.Compare(pathA, pathB, true) == 0)
            {
                return true;
            }

            return false;
        }
    }

    public class VideoHelper
    {
        public static double FpsPerSecond = 29.97;

        public static int GetSecond(int nFrame)
        {
            return (int)(nFrame / FpsPerSecond);
        }
        
        public static long GetTick(int nFrame)
        {
            return (long)((nFrame / FpsPerSecond) * 10000000);
        }

        public static TimeSpan GetTimeSpan(int nFrame)
        {
            TimeSpan retTimeSpan = TimeSpan.Zero;

            long nTick = GetTick(nFrame);

            retTimeSpan = new TimeSpan(nTick);

            return retTimeSpan;
        }

        public static string GetTimeString(int nFrame)
        {
            TimeSpan ts = GetTimeSpan(nFrame);

            DateTime dt = new DateTime(1984, 1, 20, 0, 0, 0, 0);

            dt = dt.Add(ts);

            return dt.ToString("HH:mm:ss");
        }
    }
}
