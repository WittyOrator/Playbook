using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    public enum ProductType
    {
        None,
        Victory,
        QuickCuts,
        CoachesPlayMaker,
        GameDay,
        Advantage,
    }

    public class Product : NotifyObj
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

        private ProductType productType = ProductType.None;
        public ProductType ProductType
        {
            get { return productType; }
            set { productType = value; }
        }

        private string productName = string.Empty;
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        public Product(ProductType pt)
        {
            productType = pt;

            productName = GetProductName(pt);
        }

        public override string ToString()
        {
            return ProductName;
        }

        public static string GetProductName(ProductType pt)
        {
            string retProductName = string.Empty;

            switch (pt)
            {
                case ProductType.Victory:
                    retProductName = "Victory";
                    break;
                case ProductType.QuickCuts:
                    retProductName = "Quick Cuts";
                    break;
                case ProductType.CoachesPlayMaker:
                    retProductName = "Coaches PlayMaker (DCPM)";
                    break;
                case ProductType.GameDay:
                    retProductName = "Game Day";
                    break;
                case ProductType.Advantage:
                    retProductName = "Advantage";
                    break;
                case ProductType.None:
                    retProductName = "None";
                    break;
                default:
                    retProductName = string.Empty;
                    break;
            }

            return retProductName;
        }
    }
}
