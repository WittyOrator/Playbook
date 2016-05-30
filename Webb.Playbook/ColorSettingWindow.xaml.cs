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
using Webb.Playbook.Data;
using Draw = Webb.Playbook.Geometry.Drawing;

namespace Webb.Playbook
{
    /// <summary>
    /// ColorSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSettingWindow : Window
    {
        private ColorSettingsViewModel ColorSettingsVM;
        private Draw Drawing;

        public ColorSettingWindow(Draw drawing)
        {
            InitializeComponent();

            ColorSettingsVM = new ColorSettingsViewModel(ColorSetting.Instance);

            colorSettingControl.DataContext = ColorSettingsVM;

            Drawing = drawing;
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void Edit_Color_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                ColorSettingViewModel csvm = btn.DataContext as ColorSettingViewModel;

                if (csvm != null)
                {
                    Microsoft.Samples.CustomControls.ColorPickerDialog colorDiag = new Microsoft.Samples.CustomControls.ColorPickerDialog();
                    colorDiag.Owner = this;

                    switch (csvm.Name)
                    {
                        case "Offensive Player Color":
                            colorDiag.StartingColor = ColorSetting.Instance.OffensivePlayerColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.OffensivePlayerColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "Defensive Player Color":
                            colorDiag.StartingColor = ColorSetting.Instance.DefensivePlayerColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.DefensivePlayerColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "Route Color":
                            colorDiag.StartingColor = ColorSetting.Instance.RouteColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.RouteColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "Zone Line Color":
                            colorDiag.StartingColor = ColorSetting.Instance.ZoneLineColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.ZoneLineColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "Zone Color":
                            colorDiag.StartingColor = ColorSetting.Instance.ZoneColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.ZoneColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "PreSnapMotion Color":
                            colorDiag.StartingColor = ColorSetting.Instance.PreSnapMotionColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.PreSnapMotionColor = colorDiag.SelectedColor;
                            }
                            break;
                        case "Block Color":
                            colorDiag.StartingColor = ColorSetting.Instance.BlockColor;
                            if (colorDiag.ShowDialog() == true)
                            {
                                ColorSetting.Instance.BlockColor = colorDiag.SelectedColor;
                            }
                            break;
                        default:
                            break;
                    }

                    ColorSettingsVM.Refresh();

                    colorSettingControl.DataContext = null;
                    colorSettingControl.DataContext = ColorSettingsVM;

                    Drawing.Figures.UpdateVisual();
                }
            }
        }
    }

    public class ColorSettingsViewModel
    {
        private ColorSetting colorSetting;

        private List<ColorSettingViewModel> colorSettings;
        public List<ColorSettingViewModel> ColorSettings
        {
            get
            {
                if (colorSettings == null)
                {
                    colorSettings = new List<ColorSettingViewModel>();
                }

                return colorSettings;
            }
        }

        public ColorSettingsViewModel(ColorSetting cs)
        {
            colorSetting = cs;

            Refresh();
        }

        public void Refresh()
        {
            ColorSettings.Clear();

            ColorSettings.Add(new ColorSettingViewModel("Offensive Player Color", colorSetting.OffensivePlayerColor));
            ColorSettings.Add(new ColorSettingViewModel("Defensive Player Color", colorSetting.DefensivePlayerColor));
            ColorSettings.Add(new ColorSettingViewModel("Route Color", colorSetting.RouteColor));
            ColorSettings.Add(new ColorSettingViewModel("PreSnapMotion Color", colorSetting.PreSnapMotionColor));
            ColorSettings.Add(new ColorSettingViewModel("Block Color", colorSetting.BlockColor));
            ColorSettings.Add(new ColorSettingViewModel("Zone Color", colorSetting.ZoneColor));
            ColorSettings.Add(new ColorSettingViewModel("Zone Line Color", colorSetting.ZoneLineColor));
        }
    }

    public class ColorSettingViewModel
    {
        public ColorSettingViewModel(string strName, Color color)
        {
            Name = strName;
            Brush = new SolidColorBrush(color);
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Brush brush;
        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
    }
}
