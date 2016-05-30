using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Webb.Playbook.Print
{
    public class CustomDocumentViewer : DocumentViewer
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnPrintCommand()
        {
            //base.OnPrintCommand();
            PrintDialog printDialog = new PrintDialog();

            printDialog.PrintTicket.PageOrientation = Webb.Playbook.Data.GameSetting.Instance.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;

            printDialog.PrintTicket.PageResolution = new PageResolution(LocalPrinter.DefaultPageSettings.PrinterResolution.X, LocalPrinter.DefaultPageSettings.PrinterResolution.X);

            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(LocalPrinter.PageWidth, LocalPrinter.PageHeight);

            try
            {
                printDialog.PrintDocument(this.Document.DocumentPaginator, "Webb Playbook");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }

    public class LocalPrinter
    {
        public static void ShowWindow()
        {
            System.Windows.Window win = new System.Windows.Window()
            {
                ShowInTaskbar = false,
                Left = -10000,
                Top = -10000,
                Width = 0,
                Height = 0,
            };

            win.Show();
            win.Close();
        }

        private static System.Drawing.Printing.PrintDocument fPrintDocument = new System.Drawing.Printing.PrintDocument();

        public static String DefaultPrinter
        {
            get { return fPrintDocument.PrinterSettings.PrinterName; }
        }

        public static int PageWidth
        {
            get
            {
                return DefaultPageSettings.PaperSize.Width;
            }
        }

        public static int PageHeight
        {
            get
            {
                return DefaultPageSettings.PaperSize.Height;
            }
        }

        public static bool Landscape
        {
            get
            {
                return DefaultPageSettings.Landscape;
            }
        }

        public static System.Drawing.Printing.PageSettings DefaultPageSettings
        {
            get { return new System.Drawing.Printing.PageSettings(fPrintDocument.PrinterSettings); }
        }

        public static System.Drawing.Printing.PrinterSettings DefaultPrinterSettings
        {
            get { return fPrintDocument.PrinterSettings; }
        }

        public static List<String> GetLocalPrinters()
        {
            List<String> fPrinters = new List<string>();
            fPrinters.Add(DefaultPrinter);
            foreach (String fPrinterName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return fPrinters;
        }
    }
}
