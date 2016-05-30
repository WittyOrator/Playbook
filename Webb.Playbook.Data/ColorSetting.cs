using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media;

namespace Webb.Playbook.Data
{
    public class ColorSetting
    {
        private static ColorSetting instance = new ColorSetting();

        public static ColorSetting Instance
        {
            get
            {
                return instance;
            }
        }

        public Color RouteColor = Colors.Black;
        public Color ZoneLineColor = Colors.LightBlue;
        public Color PreSnapMotionColor = Colors.Red;
        public Color BlockColor = Colors.Blue;
        public Color ZoneColor = Colors.LightBlue;
        public Color OffensivePlayerColor = Colors.OrangeRed;
        public Color DefensivePlayerColor = Colors.SteelBlue;

        public void Save(XElement e)
        {
            XElement elemColorSetting = new XElement("ColorSetting");
            e.Add(elemColorSetting);

            elemColorSetting.WriteElementColor("RouteColor", RouteColor);
            elemColorSetting.WriteElementColor("ZoneLineColor", ZoneLineColor);
            elemColorSetting.WriteElementColor("PreSnapMotionColor", PreSnapMotionColor);
            elemColorSetting.WriteElementColor("BlockColor", BlockColor);
            elemColorSetting.WriteElementColor("ZoneColor", ZoneColor);
            elemColorSetting.WriteElementColor("OffensivePlayerColor", OffensivePlayerColor);
            elemColorSetting.WriteElementColor("DefensivePlayerColor", DefensivePlayerColor);
        }

        public void Load(XElement e)
        {
            XElement elemColorSetting = e.Element("ColorSetting");
            if (elemColorSetting != null)
            {
                RouteColor = elemColorSetting.ReadElementColor("RouteColor");
                ZoneLineColor = elemColorSetting.ReadElementColor("ZoneLineColor");
                PreSnapMotionColor = elemColorSetting.ReadElementColor("PreSnapMotionColor");
                BlockColor = elemColorSetting.ReadElementColor("BlockColor");
                ZoneColor = elemColorSetting.ReadElementColor("ZoneColor");
                if (elemColorSetting.HasElements)
                {
                    bool bRet = elemColorSetting.Elements().Any(el => el.Attribute("Name").Value == "OffensivePlayerColor");

                    if (bRet)
                    {
                        OffensivePlayerColor = elemColorSetting.ReadElementColor("OffensivePlayerColor");
                    }
                }
                if (elemColorSetting.HasElements)
                {
                    bool bRet = elemColorSetting.Elements().Any(el => el.Attribute("Name").Value == "DefensivePlayerColor");

                    if (bRet)
                    {
                        DefensivePlayerColor = elemColorSetting.ReadElementColor("DefensivePlayerColor");
                    }
                }
            }
        }
    }
}