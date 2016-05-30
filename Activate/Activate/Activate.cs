using System;
using System.Collections.Generic;
using System.Text;

using Webb.Playbook.Data;

namespace WebbActivate
{
    public class Activate
    {
        public static byte realKey = 0x25;
        public static byte trialKey = 0x26;

        static Activate()
        {
            
        }

        public static void GetKey(ProductInfo.ProductType type)
        {
            switch (type)
            {
                case ProductInfo.ProductType.Full:
                    realKey = 0x3c;
                    trialKey = 0x5c;
                    break;
                case ProductInfo.ProductType.Lite:
                    realKey = 0x25;
                    trialKey = 0x26;
                    break;
                default:
                    realKey = 0x25;
                    trialKey = 0x26;
                    break;
            }
        }

        public static string GenerateKey(string strSeed, bool bReal, int nDays)
        {
            byte thebyte;
            byte[] IszKey = new byte[8];
            string strSerial = string.Empty;
            
            for (int i = 0; i < strSeed.Length; i++)
            {
                thebyte = Asc(strSeed[i]);

                if (bReal)
                {
                    thebyte ^= realKey;
                }
                else
                {
                    thebyte ^= trialKey;
                }
                        
                IszKey[i] = thebyte;
            }

            for (int j = 0; j < 8; j++)
            {
                string strth = IszKey[j].ToString("x");
                if (strth.Length == 1)
                {
                    strSerial += "0" + strth;
                }
                else
                {
                    strSerial += strth;
                }
            }

            if (bReal)  // 10-20-2010 Scott
            {
                nDays = 0;
            }

            strSerial += ToBinary(nDays);
            return strSerial;
        }

        public static bool CheckSerial(string strSeed, string strSerial,out bool bReal,out int nDays)
        {
            bool bRet = false;
            int nDayCount = 8;

            bReal = false;
            nDays = 0;

            if (strSerial != null && strSerial.Length > nDayCount)
            {
                foreach (ProductInfo.ProductType type in Enum.GetValues(typeof(ProductInfo.ProductType)))
                {
                    GetKey(type);

                    string strKey = GenerateKey(strSeed, true, nDays);

                    if (string.Compare(strKey.Remove(strKey.Length - nDayCount), strSerial.Remove(strSerial.Length - nDayCount)) == 0)
                    {
                        bReal = true;

                        bRet = true;

                        ProductInfo.Type = type;

                        Webb.Playbook.Activate.Register.SetProductTypeReg();

                        break;
                    }

                    strKey = GenerateKey(strSeed, false, nDays);

                    if (string.Compare(strKey.Remove(strKey.Length - nDayCount), strSerial.Remove(strSerial.Length - nDayCount)) == 0)
                    {
                        bRet = true;

                        string strDay = strSerial.Substring(strKey.Length - nDayCount);

                        nDays = GetInt(strDay);

                        ProductInfo.Type = type;

                        Webb.Playbook.Activate.Register.SetProductTypeReg();

                        break;
                    }
                }
            }

            return bRet;
        }

        public static int GetInt(string strBinary)
        {
            int nRet = 0;

            for (int i = 0; i < strBinary.Length; i++)
            {
                char c = strBinary[strBinary.Length - i - 1];

                switch (c)
                {
                    case '0':
                        break;
                    case '1':
                        nRet += (int)Math.Pow(2, i);
                        break;
                    default:
                        return 0;
                }
            }

            return nRet;
        }

        public static byte Asc(char strInput)
        {
            char[] cInput = new char[1];
            cInput[0] = strInput;

            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte intAsciiCode = Convert.ToByte(asciiEncoding.GetBytes(cInput)[0]);
            return (intAsciiCode);
        }

        public static string ToBinary(int val)
        {
            string strRet = string.Empty;
            for (int i = 7; i >= 0; i--)
            {
                if ((val&(1 << i))!=0)
                {
                    strRet += "1";
                }
                else
                {
                    strRet += "0";
                }
            }
            return strRet;
        }
    }
}