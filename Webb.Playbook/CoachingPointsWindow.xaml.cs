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

using Webb.Playbook.Geometry;
using Webb.Playbook.Geometry.Game;

namespace Webb.Playbook
{
    /// <summary>
    /// CoachingPointsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CoachingPointsWindow : Window
    {
        private PBPlayer player;

        public CoachingPointsWindow()
        {
            InitializeComponent();
        }

        public CoachingPointsWindow(PBPlayer player)
            : this()
        {
            this.player = player;

            rtbCoachingPoints.Document = player.CoachingPoints.CreateFlowDocumentFormText();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            player.CoachingPoints = rtbCoachingPoints.Document.GetTextFromFlowDocument();

            CloseWindow();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rtbCoachingPoints.Focus();

            rtbCoachingPoints.ScrollToEnd();

            rtbCoachingPoints.CaretPosition = rtbCoachingPoints.CaretPosition.DocumentEnd;
        }
    }

    public static class RichTextBoxUtility
    {
        public static string GetTextFromFlowDocument(this FlowDocument flowDoc)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Block block in flowDoc.Blocks)
            {
                if (block is Paragraph)
                {
                    Paragraph paragraph = block as Paragraph;

                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run)
                        {
                            Run run = inline as Run;

                            sb.Append(run.Text);

                            if (block.NextBlock != null)
                            {
                                sb.Append("\n");
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public static FlowDocument CreateFlowDocumentFormText(this string str)
        {
            FlowDocument flowDoc = new FlowDocument();

            if (str != null)
            {
                foreach (string strline in str.Split(new string[] { "\n" }, StringSplitOptions.None))
                {
                    Paragraph paragraph = new Paragraph(new Run(strline));

                    flowDoc.Blocks.Add(paragraph);
                }
            }

            return flowDoc;
        }
    }
}
