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

namespace Webb.Playbook.Print
{
    /// <summary>
    /// GameSetupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameSetupWindow : Window
    {
        Webb.Playbook.Geometry.Drawing draw;
        Webb.Playbook.Geometry.Shapes.PBLabel label = new Webb.Playbook.Geometry.Shapes.PBLabel()
        {
            Text = "Title",
            FontSize = 40,
            Coordinates = new Point(-2, 15),
        };

        public GameSetupWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(GameSetupWindow_Loaded);

            cbPlayground.Checked += new RoutedEventHandler(cb_Checked);
            cbTitle.Checked += new RoutedEventHandler(cb_Checked);
            cbColor.Checked += new RoutedEventHandler(cb_Checked);
            cbSymbolColor.Checked += new RoutedEventHandler(cb_Checked);
            
            cbPlayground.Unchecked += new RoutedEventHandler(cb_Unchecked);
            cbTitle.Unchecked += new RoutedEventHandler(cb_Unchecked);
            cbColor.Unchecked += new RoutedEventHandler(cb_Unchecked);
            cbSymbolColor.Unchecked += new RoutedEventHandler(cb_Unchecked);
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == cbPlayground)
            {
                if (draw != null)
                {
                    draw.SetPlaygroundVisible(Visibility.Visible);
                }
            }

            if (sender == cbTitle)
            {
                if (draw != null)
                {
                    draw.Add(label);
                }
            }

            if (sender == cbColor)
            {
                if (draw != null)
                {
                    draw.SetPlaygroundColor(Webb.Playbook.Data.PlaygroundColors.Color);
                }
            }

            if (sender == cbSymbolColor)
            {
                if (draw != null)
                {
                    draw.SetFiguresColor(true);
                }
            }
        }

        void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender == cbPlayground)
            {
                draw.SetPlaygroundVisible(Visibility.Hidden);
            }

            if (sender == cbTitle)
            {
                draw.Remove(label);
            }

            if (sender == cbColor)
            {
                draw.SetPlaygroundColor(Webb.Playbook.Data.PlaygroundColors.BlackAndWhite);
            }

            if (sender == cbSymbolColor)
            {
                draw.SetFiguresColor(false);
            }
        }

        void GameSetupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cbPlayground.IsChecked = Webb.Playbook.Data.GameSetting.Instance.ShowPlayground;
            cbColor.IsChecked = Webb.Playbook.Data.GameSetting.Instance.EnableColor;
            cbTitle.IsChecked = Webb.Playbook.Data.GameSetting.Instance.EnableTitle;
            cbSymbolColor.IsChecked = Webb.Playbook.Data.GameSetting.Instance.EnableSymbolColor;

            canvasDrawing.Clip = new RectangleGeometry(new Rect(0, 0, canvasDrawing.ActualWidth, canvasDrawing.ActualHeight));
            
            string strSample = AppDomain.CurrentDomain.BaseDirectory + @"\Offensive\Formation\Offensive\WEBB LIBRARY\DIAMOND LEFT.Form";
            if (System.IO.File.Exists(strSample))
            {
                draw = Webb.Playbook.Geometry.Drawing.Load(strSample, canvasDrawing);
            }
            else
            {
                draw = new Webb.Playbook.Geometry.Drawing(canvasDrawing);
                PlayersCreator pc = new PlayersCreator();
                Webb.Playbook.Data.Personnel personnel = new Webb.Playbook.Data.Personnel();
                personnel.ScoutType = Webb.Playbook.Data.ScoutTypes.Offensive;
                personnel.Positions.Clear();
                personnel.Positions.AddRange(PersonnelEditor.Setting.GetOffensePositions());
                pc.CreatePlayers(draw, personnel, true, 0);
            }

            draw.SetPlaygroundVisible(cbPlayground.IsChecked.Value ? Visibility.Visible : Visibility.Hidden);

            if (cbTitle.IsChecked.Value)
            {
                draw.Add(label);
            }

            draw.SetPlaygroundColor(cbColor.IsChecked.Value ? Webb.Playbook.Data.PlaygroundColors.Color : Webb.Playbook.Data.PlaygroundColors.BlackAndWhite);
            draw.SetFiguresColor(cbSymbolColor.IsChecked.Value);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Webb.Playbook.Data.GameSetting.Instance.ShowPlayground = cbPlayground.IsChecked.Value;
            Webb.Playbook.Data.GameSetting.Instance.EnableColor = cbColor.IsChecked.Value;
            Webb.Playbook.Data.GameSetting.Instance.EnableTitle = cbTitle.IsChecked.Value;
            Webb.Playbook.Data.GameSetting.Instance.EnableSymbolColor = cbSymbolColor.IsChecked.Value;

            CloseWindow();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            this.Close();
        }
    }
}
