using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows;

using Microsoft.Win32;
using System.IO;

namespace DeleteRegister
{
    static class Program
    {
        static RegistryKey baseKey = Registry.LocalMachine;

        static string[] productKeyPathArr = { @"SOFTWARE\Webb Electronics\Playbook", @"SOFTWARE\Wow6432Node\Webb Electronics\Playbook" };

        /// <summary>
        /// 应用程序的主入口点。

        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            foreach (string productKeyPath in productKeyPathArr)
            {
                RegistryKey rKey = baseKey.OpenSubKey(productKeyPath, true);

                RemoveRegister(rKey);
            }
        }

        static void RemoveRegister(RegistryKey regKey)
        {
            if (regKey != null)
            {
                string[] arrValues = regKey.GetValueNames();

                if (arrValues.Contains("IsReal"))
                {
                    if (bool.Parse(regKey.GetValue("IsReal").ToString()))
                    {
                        baseKey.DeleteSubKey(@"SOFTWARE\Webb Electronics\Playbook");
                    }
                }
            }
        }
    }
}

