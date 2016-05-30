using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Webb.Playbook.Data
{
    public enum HashLine
    {
        Left = 7,
        Middle = 0,
        Right = -7,
        None
    }

    public class PlaygroundStructure
    {
        public static double Left = 7;
        public static double Middle = 0;
        public static double Right = -7;

        public static double GetHashLine(HashLine hl)
        {
            SetHashLine(GameSetting.Instance.PlaygroundType);
            switch (hl)
            {
                case HashLine.Left:
                    return Left;
                    break;
                case HashLine.Right:
                    return Right;
                    break;
                case HashLine.Middle:
                    return Middle;
                    break;
            }
            return Middle;
        }

        public static void SetHashLine(PlaygroundTypes pt)
        {
            switch (pt)
            {
                case PlaygroundTypes.HighSchool:
                    Left = 8.9;
                    Right = -8.9;
                    break;
                case PlaygroundTypes.NCAA:
                    Left = 6.67;
                    Right = -6.67;
                    break;
                case PlaygroundTypes.NFL:
                    Left = 3.1;
                    Right = -3.1;
                    break;
                case PlaygroundTypes.CFL:
                    Left = 8.5;
                    Right = -8.5;
                    break;
            }
        }
    }

    public enum PlaygroundColors
    {
        BlackAndWhite,
        Color,
    }

    public enum PlaygroundTypes
    {
        HighSchool, //  53.4/3 = 17.8   8.9
        NCAA,       //  40/3 = 13.3     6.67
        NFL,        //  18.6/3 = 6.2    3.1
        CFL,        //  17              8.5
    }

    public class Playground : NotifyObj
    {
        private bool use = false;
        public bool Use
        {
            get { return use; }
            set
            {
                use = value;
            }
        }

        private PlaygroundTypes playgroundType = PlaygroundTypes.NCAA;
        public PlaygroundTypes PlaygroundType
        {
            get { return playgroundType; }
            set { playgroundType = value; }
        }

        private string playgroundName = string.Empty;
        public string PlaygroundName
        {
            get { return playgroundName; }
            set { playgroundName = value; }
        }

        public Playground(PlaygroundTypes pt)
        {
            playgroundType = pt;

            playgroundName = pt.ToString();
        }

        public override string ToString()
        {
            return playgroundName;
        }

        public static string GetPlaygroundName(ProductType pt)
        {
            string retPlaygroundName = string.Empty;

            return retPlaygroundName;
        }
    }

    public class Settings
    {
        private static Settings instance = new Settings();

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        public void Load(string file)
        {
            if (File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);
                ColorSetting.Instance.Load(doc.Root);
                GameSetting.Instance.Load(doc.Root);
            }
        }

        public void Save(string file)
        {
            XDocument doc = new XDocument();
            XDeclaration declare = new XDeclaration("1.0", "utf-8", "true");
            doc.Declaration = declare;

            doc.Add(new XElement("Settings"));
            ColorSetting.Instance.Save(doc.Root);
            GameSetting.Instance.Save(doc.Root);

            doc.Save(file);
        }
    }

    public enum BallSize
    {
        Small = 0, 
        Medium = 1,
        Large = 2,
    }

    public class GameSetting : Webb.Playbook.Data.NotifyObj
    {
        public const string DefaultTheme = @"Blue.xaml";

        private static GameSetting instance = new GameSetting();
        public static GameSetting Instance
        {
            get
            {
                return instance;
            }
        }

        // 10-26-2011 Scott
        public string GetField(ScoutTypes scoutType, bool bSubField)
        {
            switch (scoutType)
            {
                case ScoutTypes.Offensive:
                    return bSubField ? OffensiveSubField : OffensiveMainField;
                case ScoutTypes.Defensive:
                    return bSubField ? DefensiveSubField : DefensiveMainField;
                case ScoutTypes.Kicks:
                    return bSubField ? KickSubField : KickMainField;
                default:
                    return bSubField ? OffensiveSubField : OffensiveMainField;
            }
        }

        // 10-09-011 Scott
        public string PlaybookUserFolder
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + GameSetting.Instance.UserType.ToString();
            }
        }

        // 03-18-2011 Scott
        private double playerSize = 1.5;
        public double PlayerSize
        {
            get { return playerSize; }
            set { playerSize = value; }
        }

        private bool playerTextVisibility = true;
        public bool PlayerTextVisibility
        {
            get { return playerTextVisibility; }
            set { playerTextVisibility = value; }
        }

        private bool playerDash = false;
        public bool PlayerDash
        {
            get { return playerDash; }
            set { playerDash = value; }
        }

        private string playerAppearance = "Circle";
        public string PlayerAppearance
        {
            get { return playerAppearance; }
            set { playerAppearance = value; }
        }

        private double playerAngle = 0;
        public double PlayerAngle
        {
            get { return playerAngle; }
            set { playerAngle = value; }
        }

        private double playerThickness = 1.5;
        public double PlayerThickness
        {
            get { return playerThickness; }
            set { playerThickness = value; }
        }
        // end

        private bool landscape = true;
        public bool Landscape
        {
            get
            {
                return landscape;
            }
            set
            {
                landscape = value;
            }
        }

        private BallSize ballSize = BallSize.Large;
        public BallSize BallSize
        {
            get { return ballSize; }
            set { ballSize = value; }
        }

        private bool enableColor = false;
        public bool EnableColor
        {
            get
            {
                return enableColor;
            }
            set
            {
                enableColor = value;
            }
        }

        private bool enableSymbolColor = false;
        public bool EnableSymbolColor
        {
            get
            {
                return enableSymbolColor;
            }
            set
            {
                enableSymbolColor = value;
            }
        }

        private bool enableTitle = true;
        public bool EnableTitle
        {
            get
            {
                return enableTitle;
            }
            set
            {
                enableTitle = value;
            }
        }

        private bool showPlayground = true;
        public bool ShowPlayground
        {
            get
            {
                return showPlayground;
            }
            set
            {
                showPlayground = value;
            }
        }

        private bool imageShowPlayground = true;
        public bool ImageShowPlayground
        {
            get
            {
                return imageShowPlayground;
            }
            set
            {
                imageShowPlayground = value;
            }
        }

        private bool imageEnableSymbolColor = true;
        public bool ImageEnableSymbolColor
        {
            get
            {
                return imageEnableSymbolColor;
            }
            set
            {
                imageEnableSymbolColor = value;
            }
        }

        private bool imageEnableColor = true;
        public bool ImageEnableColor
        {
            get
            {
                return imageEnableColor;
            }
            set
            {
                imageEnableColor = value;
            }
        }

        private bool showBall = true;
        public bool ShowBall
        {
            get
            {
                return showBall;
            }
            set
            {
                showBall = value;
            }
        }

        private int snapValue = 18;
        public int SnapValue
        {
            get
            {
                return snapValue;
            }
            set
            {
                snapValue = value;
            }
        }

        private bool snapToGrid = true;
        public bool SnapToGrid
        {
            get
            {
                return snapToGrid;
            }
            set
            {
                snapToGrid = value;
            }
        }

        private string currentTheme = DefaultTheme;
        public string CurrentTheme
        {
            get
            {
                if (currentTheme == string.Empty)
                {
                    currentTheme = DefaultTheme;
                }

                return currentTheme;
            }
            set
            {
                currentTheme = value;
            }
        }

        private bool gridLine = true;
        public bool GridLine
        {
            get { return gridLine; }
            set { gridLine = value; }
        }

        private double scaling = 100;
        public double Scaling
        {
            get { return scaling; }
            set
            {
                scaling = value;
                OnPropertyChanged("Scaling");
            }
        }

        private double scalingAnimation = 100;
        public double ScalingAnimation
        {
            get { return scalingAnimation; }
            set
            {
                scalingAnimation = value;
                OnPropertyChanged("ScalingAnimation");
            }
        }

        private bool ourTeamOffensive = true;
        public bool OurTeamOffensive
        {
            get { return ourTeamOffensive; }
            set
            {
                ourTeamOffensive = value;
            }
        }

        public Data.ScoutTypes OurTeamScoutType
        {
            get
            {
                return OurTeamOffensive ? Data.ScoutTypes.Offensive : Data.ScoutTypes.Defensive;
            }
            set
            {
                if (value == Data.ScoutTypes.Offensive)
                {
                    OurTeamOffensive = true;
                }
                if (value == Data.ScoutTypes.Defensive)
                {
                    OurTeamOffensive = false;
                }
                OnPropertyChanged("OurTeamScoutType");
            }
        }

        // 10-08-2010 Scott
        private string offensiveMainField = "Formation";
        public string OffensiveMainField
        {
            get { return offensiveMainField; }
            set { offensiveMainField = value; }
        }

        private string offensiveSubField = "Play Name";
        public string OffensiveSubField
        {
            get { return offensiveSubField; }
            set { offensiveSubField = value; }
        }

        private string defensiveMainField = "Front";
        public string DefensiveMainField
        {
            get { return defensiveMainField; }
            set { defensiveMainField = value; }
        }

        private string defensiveSubField = "Defense";
        public string DefensiveSubField
        {
            get { return defensiveSubField; }
            set { defensiveSubField = value; }
        }

        private string kickMainField = "Kick Type";
        public string KickMainField
        {
            get { return kickMainField; }
            set { kickMainField = value; }
        }

        private string kickSubField = "Play Name";
        public string KickSubField
        {
            get { return kickSubField; }
            set { kickSubField = value; }
        }

        private string userFolder = string.Empty;
        public string UserFolder
        {
            get { return userFolder; }
            set { userFolder = value; }
        }

        private ProductType productType = ProductType.None;
        public ProductType ProductType
        {
            get { return productType; }
            set { productType = value; }
        }

        private PlaygroundTypes playgroundType = PlaygroundTypes.NCAA;
        public PlaygroundTypes PlaygroundType
        {
            get { return playgroundType; }
            set { playgroundType = value; }
        }

        private UserTypes userType = UserTypes.Offensive;
        public UserTypes UserType
        {
            get { return userType; }
            set { userType = value; }
        }

        public void Save(XElement e)
        {
            XElement elemGameSetting = new XElement("GameSetting");
            e.Add(elemGameSetting);

            XElement elem = new XElement("OurTeamOffensive");
            XAttribute attri = new XAttribute("Value", OurTeamOffensive.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("Scaling");
            attri = new XAttribute("Value", Scaling.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            // 11-09-2010 Scott
            elem = new XElement("ScalingAnimation");
            attri = new XAttribute("Value", ScalingAnimation.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            // 10-08-2010 Scott
            elem = new XElement("OffensiveMainField");
            attri = new XAttribute("Value", OffensiveMainField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("OffensiveSubField");
            attri = new XAttribute("Value", OffensiveSubField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("DefensiveMainField");
            attri = new XAttribute("Value", DefensiveMainField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("DefensiveSubField");
            attri = new XAttribute("Value", DefensiveSubField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            // 10-26-2011 Scott
            elem = new XElement("KickMainField");
            attri = new XAttribute("Value", KickMainField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("KickSubField");
            attri = new XAttribute("Value", KickSubField.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);
            // end

            elem = new XElement("UserFolder");
            attri = new XAttribute("Value", UserFolder);
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("UserType");
            attri = new XAttribute("Value", UserType.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ProductType");
            attri = new XAttribute("Value", ProductType.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlaygroundType");
            attri = new XAttribute("Value", PlaygroundType.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("CurrentTheme");
            attri = new XAttribute("Value", CurrentTheme);
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("GridLine");
            attri = new XAttribute("Value", GridLine.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("SnapValue");
            attri = new XAttribute("Value", SnapValue.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("SnapToGrid");
            attri = new XAttribute("Value", SnapToGrid.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ShowBall");
            attri = new XAttribute("Value", ShowBall.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("BallSize");
            attri = new XAttribute("Value", BallSize.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("Landscape");
            attri = new XAttribute("Value", Landscape.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ShowPlayground");
            attri = new XAttribute("Value", ShowPlayground.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("EnableColor");
            attri = new XAttribute("Value", EnableColor.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("EnableSymbolColor");
            attri = new XAttribute("Value", EnableSymbolColor.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("EnableTitle");
            attri = new XAttribute("Value", EnableTitle.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerAngle");
            attri = new XAttribute("Value", PlayerAngle.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerAppearance");
            attri = new XAttribute("Value", PlayerAppearance.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerDash");
            attri = new XAttribute("Value", PlayerDash.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerSzie");
            attri = new XAttribute("Value", PlayerSize.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerTextVisibility");
            attri = new XAttribute("Value", PlayerTextVisibility.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("PlayerThickness");
            attri = new XAttribute("Value", PlayerThickness.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ImageShowPlayground");
            attri = new XAttribute("Value", ImageShowPlayground.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ImageEnableColor");
            attri = new XAttribute("Value", ImageEnableColor.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);

            elem = new XElement("ImageEnableSymbolColor");
            attri = new XAttribute("Value", ImageEnableSymbolColor.ToString());
            elem.Add(attri);
            elemGameSetting.Add(elem);
        }

        public void Load(XElement e)
        {
            XElement elemGameSetting = e.Element("GameSetting");
            if (elemGameSetting != null)
            {
                XElement elem = elemGameSetting.Element("OurTeamOffensive");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        OurTeamOffensive = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("Scaling");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        Scaling = attri.Value.ToDouble(150);
                    }
                }

                // 11-09-2010 Scott
                elem = elemGameSetting.Element("ScalingAnimation");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ScalingAnimation = attri.Value.ToDouble(100);
                    }
                }

                // 10-08-2010 Scott
                elem = elemGameSetting.Element("OffensiveMainField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        OffensiveMainField = attri.Value;
                    }
                }
                OffensiveMainField = OffensiveMainField == string.Empty ? "Formation" : OffensiveMainField;

                elem = elemGameSetting.Element("OffensiveSubField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        OffensiveSubField = attri.Value;
                    }
                }
                OffensiveSubField = OffensiveSubField == string.Empty ? "Play Name" : OffensiveSubField;

                elem = elemGameSetting.Element("DefensiveMainField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        DefensiveMainField = attri.Value;
                    }
                }
                DefensiveMainField = DefensiveMainField == string.Empty ? "Front" : DefensiveMainField;

                elem = elemGameSetting.Element("DefensiveSubField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        DefensiveSubField = attri.Value;
                    }
                }
                DefensiveSubField = DefensiveSubField == string.Empty ? "Defense" : DefensiveSubField;

                // 10-26-2011 Scott
                elem = elemGameSetting.Element("KickMainField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        KickMainField = attri.Value;
                    }
                }
                KickMainField = KickMainField == string.Empty ? "Kick Type" : KickMainField;

                elem = elemGameSetting.Element("KickSubField");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        KickSubField = attri.Value;
                    }
                }
                KickSubField = KickSubField == string.Empty ? "Play Name" : KickSubField;
                // end

                elem = elemGameSetting.Element("UserFolder");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        UserFolder = attri.Value;
                    }
                }

                elem = elemGameSetting.Element("UserType");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        UserType = (UserTypes)Enum.Parse(typeof(UserTypes), attri.Value);
                    }
                }

                elem = elemGameSetting.Element("ProductType");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ProductType = (ProductType)Enum.Parse(typeof(ProductType), attri.Value);
                    }
                }

                elem = elemGameSetting.Element("PlaygroundType");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlaygroundType = (PlaygroundTypes)Enum.Parse(typeof(PlaygroundTypes), attri.Value);
                    }
                }

                elem = elemGameSetting.Element("CurrentTheme");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        CurrentTheme = attri.Value;

                        switch (CurrentTheme)
                        {
                            case "BureauBlack.xaml":
                                CurrentTheme = "Black.xaml";
                                break;
                            case "BureauBlue.xaml":
                                CurrentTheme = "Blue.xaml";
                                break;
                            case "ExpressionDark.xaml":
                                CurrentTheme = "Grey.xaml";
                                break;
                            case "ShinyBlue.xaml":
                                CurrentTheme = "Shiny Blue.xaml";
                                break;
                            case "ShinyRed.xaml":
                                CurrentTheme = "Shiny Red.xaml";
                                break;
                            case "WhistlerBlue.xaml":
                                CurrentTheme = "Whistler Blue.xaml";
                                break;
                        }
                    }
                }

                elem = elemGameSetting.Element("GridLine");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        GridLine = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("SnapValue");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        SnapValue = int.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("SnapToGrid");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        SnapToGrid = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("ShowBall");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ShowBall = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("BallSize");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        BallSize = (BallSize)Enum.Parse(typeof(BallSize), attri.Value);
                    }
                }

                elem = elemGameSetting.Element("Landscape");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        Landscape = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("ShowPlayground");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ShowPlayground = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("EnableColor");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        EnableColor = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("EnableSymbolColor");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        EnableSymbolColor = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("EnableTitle");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        EnableTitle = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("PlayerAngle");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerAngle = attri.Value.ToDouble(0);
                    }
                }

                elem = elemGameSetting.Element("PlayerAppearance");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerAppearance = attri.Value;
                    }
                }

                elem = elemGameSetting.Element("PlayerDash");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerDash = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("PlayerSize");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerSize = attri.Value.ToDouble(1.5);
                    }
                }

                elem = elemGameSetting.Element("PlayerTextVisibility");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerTextVisibility = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("PlayerThickness");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        PlayerThickness = attri.Value.ToDouble(1.5);
                    }
                }

                elem = elemGameSetting.Element("ImageShowPlayground");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ImageShowPlayground = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("ImageEnableColor");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ImageEnableColor = bool.Parse(attri.Value);
                    }
                }

                elem = elemGameSetting.Element("ImageEnableSymbolColor");
                if (elem != null)
                {
                    XAttribute attri = elem.Attribute("Value");
                    if (attri != null)
                    {
                        ImageEnableSymbolColor = bool.Parse(attri.Value);
                    }
                }
            }
        }
    }
}