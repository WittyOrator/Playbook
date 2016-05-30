using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    public class Terminology
    {
        private static Terminology instance;
        public static Terminology Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Terminology();
                }

                return instance;
            }
        }

        private VictoryData victoryData;
        public VictoryData VictoryData
        {
            get
            {
                if (victoryData == null)
                {
                    victoryData = new VictoryData();
                }

                return victoryData;
            }
        }

        private WEBBGAMEDATALib.WebbGame2 game;
        public WEBBGAMEDATALib.WebbGame2 Game
        {
            get
            {
                if (game == null)
                {
                    game = new WEBBGAMEDATALib.WebbGame2();
                }

                return game;
            }
        }

        private List<String> fields;
        public List<String> Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = new List<string>();
                }

                return fields;
            }
        }

        public Terminology()
        {
            
        }

        public void SetTermFilePath(string strFilePath)
        {
            if (GameSetting.Instance.ProductType == ProductType.None)
            {
            }
            else if (GameSetting.Instance.ProductType == ProductType.Victory)
            {
                VictoryData.UserFolder = strFilePath;
            }
            else
            {
                game = null;

                Game.SetTermFilePath(strFilePath);
            }

            LoadFields();
        }

        public void AddField(string strFiled)
        {
            if (!Fields.Contains(strFiled))
            {
                Fields.Add(strFiled);
            }
        }

        public void LoadFields()
        {
            Fields.Clear();

            if (GameSetting.Instance.ProductType == ProductType.None)
            {
            }
            else if (GameSetting.Instance.ProductType == ProductType.Victory)
            {
                Fields.AddRange(VictoryData.GetFields());
            }
            else
            {
                int nField = 0;

                Game.GetNumOfDefinedFileds(out nField);

                for (int i = 0; i < nField; i++)
                {
                    string strField = string.Empty;

                    Game.GetDefinedFieldName(i, out strField);

                    Fields.Add(strField);
                }
            }

            // 04-27-2011 Scott
            AddField("Formation");
            AddField("Play Name");
            AddField("Front");
            AddField("Defense");
            AddField("Kick Type");

            Fields.Sort();
        }

        public List<String> LoadValuesForFieldByType(string strField, int nUserType)
        {
            List<String> arrValue = new List<string>();

            if (GameSetting.Instance.ProductType == ProductType.None)
            {
            }
            else if (GameSetting.Instance.ProductType == ProductType.Victory)
            {
                arrValue = VictoryData.GetValues(strField);
            }
            else
            {
                if (Fields.Contains(strField))
                {
                    int nValue = 0;

                    string strValue = string.Empty;

                    Game.GetNumValuesOfOneField2(strField, nUserType, out nValue);

                    for (int i = 0; i < nValue; i++)
                    {
                        Game.GetValueOfOneField(strField, i, out strValue);
                        arrValue.Add(strValue);
                    }
                }
            }

            arrValue.Sort();

            return arrValue;
        }
    }
}
