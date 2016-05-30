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

using Webb.Playbook.Geometry.Shapes;
using Draw = Webb.Playbook.Geometry.Drawing;

namespace Webb.Playbook
{
    /// <summary>
    /// ToolListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToolListWindow : Window
    {
        private Draw Drawing = null;
        private ToolManager toolManagerMenu = new ToolManager();

        public ToolListWindow()
        {
            InitializeComponent();

            this.listBoxTool.DataContext = toolManagerMenu;

            toolManagerMenu.InitLabelMenu();
        }

        public ToolListWindow(Draw draw)
            : this()
        {
            Drawing = draw;
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void listBoxTool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void listBoxTool_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxTool_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)
                {
                    switch (selectedTool.Command)
                    {
                        case Commands.Label:
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Labels\\" + selectedTool.Text;
                                if (System.IO.File.Exists(file))
                                {
                                    PBLabel label = new PBLabel();
                                    Drawing.Add(label);
                                    label.LoadLabel(file);
                                    label.Coordinates = new Point(-8, 5);
                                    label.UpdateVisual();
                                    this.DialogResult = false;
                                    this.CloseWindow();
                                }
                            }
                            break;
                        case Commands.CreateNewLabel:
                            {
                                this.DialogResult = true;
                                this.CloseWindow();
                            }
                            break;
                    }
                }
            }
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is FrameworkElement)
            {
                Tool selectedTool = (Mouse.DirectlyOver as FrameworkElement).DataContext as Tool;

                if (selectedTool != null)
                {
                    switch (selectedTool.Command)
                    {
                        case Commands.Label:
                            if (MessageBox.Show(String.Format("Do you want to delete [{0}]?", selectedTool.Text), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string file = AppDomain.CurrentDomain.BaseDirectory + "Labels\\" + selectedTool.Text;

                                if (System.IO.File.Exists(file))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(file);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                                else
                                {
                                    selectedTool.Parent.Remove(selectedTool);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
