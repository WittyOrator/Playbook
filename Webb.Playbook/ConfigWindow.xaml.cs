using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Webb.Playbook.Data;

namespace Webb.Playbook
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            listProducts.DataContext = ProductsViewModel.Instance;

            ProductsViewModel.Instance.SetProduct(GameSetting.Instance.ProductType);

            listPlaygrounds.DataContext = PlaygroundsViewModel.Instance;

            PlaygroundsViewModel.Instance.SetPlayground(GameSetting.Instance.PlaygroundType);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            GameSetting.Instance.ProductType = ProductsViewModel.Instance.GetProduct();

            GameSetting.Instance.PlaygroundType = PlaygroundsViewModel.Instance.GetPlayground();

            this.Close();
        }
    }

    public class ProductsViewModel : NotifyObj
    {
        private static ProductsViewModel instance;
        public static ProductsViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProductsViewModel();
                }

                return instance;
            }
        }

        public ProductsViewModel()
        {
            Products.Clear();

            foreach (ProductType pt in Enum.GetValues(typeof(ProductType)))
            {
                Products.Add(new Product(pt));
            }
        }

        private Product selectedProduct;
        public Product SelectedProduct
        {
            get
            {
                return selectedProduct;
            }
            set
            {
                selectedProduct = value;
            }
        }

        private List<Product> products;
        public List<Product> Products
        {
            get
            {
                if (products == null)
                {
                    products = new List<Product>();
                }

                return products;
            }
        }

        public void SetProduct(ProductType pt)
        {
            foreach (Product product in Products)
            {
                if (product.ProductType == pt)
                {
                    product.Use = true;
                }
                else
                {
                    product.Use = false;
                }
            }
        }

        public ProductType GetProduct()
        {
            foreach (Product product in Products)
            {
                if (product.Use)
                {
                    return product.ProductType;
                }
            }

            return ProductType.None;
        }
    }

    public class PlaygroundsViewModel : NotifyObj
    {
        private static PlaygroundsViewModel instance;
        public static PlaygroundsViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlaygroundsViewModel();
                }

                return instance;
            }
        }

        public PlaygroundsViewModel()
        {
            Playgrounds.Clear();

            foreach (PlaygroundTypes pt in Enum.GetValues(typeof(PlaygroundTypes)))
            {
                Playgrounds.Add(new Playground(pt));
            }
        }

        private Playground selectedPlayground;
        public Playground SelectedPlayground
        {
            get
            {
                return selectedPlayground;
            }
            set
            {
                selectedPlayground = value;
            }
        }

        private List<Playground> playgrounds;
        public List<Playground> Playgrounds
        {
            get
            {
                if (playgrounds == null)
                {
                    playgrounds = new List<Playground>();
                }

                return playgrounds;
            }
        }

        public void SetPlayground(PlaygroundTypes pt)
        {
            foreach (Playground playground in Playgrounds)
            {
                if (playground.PlaygroundType == pt)
                {
                    playground.Use = true;
                }
                else
                {
                    playground.Use = false;
                }
            }
        }

        public PlaygroundTypes GetPlayground()
        {
            foreach (Playground playground in Playgrounds)
            {
                if (playground.Use)
                {
                    return playground.PlaygroundType;
                }
            }

            return PlaygroundTypes.NCAA;
        }
    }
}
