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
using System.Windows.Markup;

namespace Webb.Playbook.Print
{
    /// <summary>
    /// PrintPreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private List<String> printFiles;
        public List<String> PrintFiles
        {
            get
            {
                if (printFiles == null)
                {
                    printFiles = new List<string>();
                }

                return printFiles;
            }
        }

        public PrintPreviewWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(PrintPreviewWindow_Loaded);
        }

        public void Print()
        {
            docViewer.Print();
        }

        public static void FillDocument(FixedDocument fixedDoc, List<string> files, int nStartPage)
        {
            if (files.Count > 0)
            {
                int nPageNum = nStartPage;

                foreach (string printFile in files)
                {
                    double space = 20;
                    double width = Webb.Playbook.Data.GameSetting.Instance.Landscape ? LocalPrinter.DefaultPageSettings.PrintableArea.Height : LocalPrinter.DefaultPageSettings.PrintableArea.Width;
                    double height = Webb.Playbook.Data.GameSetting.Instance.Landscape ? LocalPrinter.DefaultPageSettings.PrintableArea.Width : LocalPrinter.DefaultPageSettings.PrintableArea.Height;

                    FixedPage page = new FixedPage();
                    page.Margin = new Thickness(space);
                    //page.Width = fixedDoc.DocumentPaginator.PageSize.Width;
                    //page.Height = fixedDoc.DocumentPaginator.PageSize.Height;
                    page.Width = width;
                    page.Height = height;

                    Grid grid = new Grid();
                    grid.Width = page.Width - 2 * space;
                    grid.Height = page.Height - 2 * space;
                    Canvas canvas = new Canvas();
                    grid.Children.Add(canvas);
                    page.Children.Add(grid);

                    // add logo
                    Image imgLogo = new Image()
                    {
                        Stretch = Stretch.Fill,
                        Width = 120,
                        Height = 30,
                        Margin = new Thickness(0,0,width - 120,height - 30),
                    };
                    
                    string strLogoPath = AppDomain.CurrentDomain.BaseDirectory + @"\Resource\logo.PNG";
                    imgLogo.Source = new BitmapImage(new Uri(strLogoPath,UriKind.RelativeOrAbsolute));
                    page.Children.Add(imgLogo);

                    FullScale(canvas);
                    TextBlock tbPageNumber = new TextBlock()
                    {
                        Text = nPageNum.ToString(),
                        FontSize = 30,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(space),
                    };
                    grid.Children.Add(tbPageNumber);
                    Webb.Playbook.Geometry.Drawing draw = Webb.Playbook.Geometry.Drawing.Load(printFile, canvas);

                    draw.SetStartYard(width, draw.GetBallPoint().Y);   // 01-16-2012 Scott

                    // 04-11-2011 Scott
                    if (draw.Figures.Any(f => f is Webb.Playbook.Geometry.Game.PBBall))
                    {
                        Webb.Playbook.Geometry.Game.PBBall ball = draw.Figures.OfType<Webb.Playbook.Geometry.Game.PBBall>().First();

                        if (ball.Visible)
                        {
                            ball.Visible = false;
                        }
                    }

                    if (Webb.Playbook.Data.GameSetting.Instance.ShowPlayground)
                    {
                        draw.SetPlaygroundColor(Webb.Playbook.Data.GameSetting.Instance.EnableColor ? Webb.Playbook.Data.PlaygroundColors.Color : Webb.Playbook.Data.PlaygroundColors.BlackAndWhite);
                    }
                    else
                    {
                        if (draw.Canvas.Children.Contains(draw.Playground.UCPlayground))
                        {
                            draw.Canvas.Children.Remove(draw.Playground.UCPlayground);
                        }
                    }

                    draw.SetFiguresColor(Webb.Playbook.Data.GameSetting.Instance.EnableSymbolColor);

                    if (Webb.Playbook.Data.GameSetting.Instance.EnableTitle)
                    {
                        TextBlock tbTitle = new TextBlock()
                        {
                            Text = System.IO.Path.GetFileNameWithoutExtension(printFile),
                            FontSize = 30,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(space, 2 * space, space, space),
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                        };
                        Panel.SetZIndex(tbTitle, 10000);
                        grid.Children.Add(tbTitle);
                    }

                    PageContent pageContent = new PageContent();
                    ((IAddChild)pageContent).AddChild(page);
                    fixedDoc.Pages.Add(pageContent);

                    nPageNum++;
                }
            }
        }

        void PrintPreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Printing.LocalPrintServer.GetDefaultPrintQueue();

            FillDocument(fixedDoc, PrintFiles, 1);

            docViewer.Focus();
        }

        public static void Scale(Canvas canvas)
        {
            int nSpace = (int)(20 + (100 - Webb.Playbook.Data.GameSetting.Instance.Scaling) / 50 * 100);

            canvas.Margin = new Thickness(nSpace, 5, nSpace, 5);
        }

        public static void FullScale(Canvas canvas)
        {
            canvas.Margin = new Thickness(0, 5, 0, 5);
        }
    }
}
