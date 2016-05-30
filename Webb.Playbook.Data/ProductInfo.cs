using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    public class ProductInfo
    {
        public const string AssemblyVersion = "0.2.2.3";
        public const string FileVersion = "0.2.2.3";
        public const string ProductName = "Playbook";
        public const string Manufacturer = "Webb Electronics";
        public const string Title = "Webb Playbook";
        public const string Copyright = "Copyright © Webb Electronics 2011";

        public enum ProductType
        {
            Full = 0,
            Lite = 1,
        }

        public static ProductType Type = ProductType.Lite;

        public static string PresentationPath = AppDomain.CurrentDomain.BaseDirectory + @"\Presentations\";
    }
}