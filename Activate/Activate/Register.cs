using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

using Webb.Playbook.Data;

namespace Webb.Playbook.Activate
{
    public class Register
    {
        public static readonly int RealDay = -888;

        static RegistryKey baseKey = Registry.LocalMachine;

        static string productKeyPath = @"SOFTWARE\Webb Electronics\Playbook";

        static Register()
        {

        }

        public bool CheckRegister(out int nExistDays, out bool bFirstRun)
        {
            bFirstRun = false;
            nExistDays = 0;

            RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

            if (rKey != null)
            {
                string[] arrValues = rKey.GetValueNames();

                if (arrValues.Contains("Activate"))
                {
                    object obj = rKey.GetValue("Activate");

                    bool bActivate = bool.Parse(obj.ToString());

                    if (bActivate)
                    {
                        if (arrValues.Contains("TrialDays"))
                        {
                            obj = rKey.GetValue("TrialDays");

                            int nTrialDays = (int)obj;

                            nExistDays = nTrialDays;

                            if (nTrialDays > 0)
                            {
                                int nSpanDays = 0;

                                if (arrValues.Contains("LastUse"))
                                {
                                    DateTime dt = DateTime.Parse(rKey.GetValue("LastUse").ToString());

                                    int nSubDays = new TimeSpan(dt.Ticks - DateTime.Today.Ticks).Days;

                                    nSpanDays = Math.Abs(nSubDays);
                                }

                                nExistDays = nTrialDays >= nSpanDays ? nTrialDays - nSpanDays : 0;

                                rKey.SetValue("TrialDays", nExistDays);

                                if (nExistDays > 0)
                                {
                                    rKey.SetValue("LastUse", DateTime.Today.ToShortDateString());
                                    return false;
                                }
                            }
                            else if (nTrialDays == Webb.Playbook.Activate.Register.RealDay)
                            {
                                rKey.SetValue("LastUse", DateTime.Today.ToShortDateString());
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    bFirstRun = true;
                }

                rKey.SetValue("LastUse", DateTime.Today.ToShortDateString());
            }
            else
            {
                bFirstRun = true;
            }

            return false;
        }

        public void SetRegister(bool bActivate, int nTrialDays, bool bReal)
        {
            RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

            if (rKey != null)
            {
                string[] arrValues = rKey.GetValueNames();

                rKey.SetValue("Activate", bActivate);
                
                rKey.SetValue("TrialDays", nTrialDays);

                rKey.SetValue("LastUse", DateTime.Today.ToShortDateString());

                rKey.SetValue("IsReal", bReal);
            }
        }

        public static void SetProductTypeReg()
        {
            RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

            if (rKey != null)
            {
                rKey.SetValue("Type", ProductInfo.Type.ToString());
            }
        }

        public static void GetProductTypeReg()
        {
            RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

            if (rKey != null)
            {
                try
                {
                    ProductInfo.Type = (ProductInfo.ProductType)Enum.Parse(typeof(ProductInfo.ProductType), rKey.GetValue("Type").ToString());
                }
                catch
                {
                    rKey.SetValue("Type", ProductInfo.ProductType.Lite.ToString());
                }
            }
        }

        public static bool? CheckRegister()
        {
            bool? bRet = null;

            RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

            if (rKey != null)
            {
                string[] arrValues = rKey.GetValueNames();

                if (arrValues.Contains("IsReal"))
                {
                    bRet = bool.Parse(rKey.GetValue("IsReal").ToString());
                }
            }

            return bRet;
        }
    }
}

