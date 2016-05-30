using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;

using Webb.Playbook.Data;

namespace Webb.Playbook
{
    /// <summary>
    /// NewPersonnel.xaml 的交互逻辑
    /// </summary>
    public partial class NewPersonnel : Window
    {
        private Personnel personnel = new Personnel();
        public Personnel Personnel
        {
            get { return personnel; }
            set { personnel = value; }
        }

        private PositionSelectorViewModel PositionSelector = new PositionSelectorViewModel();

        public NewPersonnel()
        {
            InitializeComponent();

            this.DataContext = Personnel;
            this.gridSelector.DataContext = PositionSelector;
        }

        public NewPersonnel(ScoutTypes scoutType)
        {
            InitializeComponent();

            this.DataContext = Personnel;
            Personnel.ScoutType = scoutType;
            this.radioOffense.IsEnabled = false;
            this.radioDefense.IsEnabled = false;
            this.gridSelector.DataContext = PositionSelector;
            this.listAllPosition.Items.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
            this.listSelectedPosition.Items.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
                
        }

        private void Add()
        {
            Position pos = this.listAllPosition.SelectedItem as Position;
            if (pos != null)
            {
                PositionSelector.SelectedPositions.Add(pos);
                PositionSelector.ExistingPositions.Remove(pos);
                Personnel.Positions.Add(pos);
            }
        }

        private void Remove()
        {
            Position pos = this.listSelectedPosition.SelectedItem as Position;
            if (pos != null)
            {
                PositionSelector.ExistingPositions.Add(pos);
                PositionSelector.SelectedPositions.Remove(pos);
                Personnel.Positions.Remove(pos);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void listAllPosition_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(ScrollViewer))
            {
                Add();
            }
        }

        private void listSelectedPosition_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(ScrollViewer))
            {
                Remove();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            PositionSelector.Init(Personnel.ScoutType);
            Personnel.Positions.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textName.Focus();
        }
    }

    public class PositionSelectorViewModel
    {
        private ObservableCollection<Position> existingPositions;
        public ObservableCollection<Position> ExistingPositions
        {
            get { return existingPositions; }
            set { existingPositions = value; }
        }

        private ObservableCollection<Position> selectedPositions;
        public ObservableCollection<Position> SelectedPositions
        {
            get { return selectedPositions; }
            set { selectedPositions = value; }
        }

        public PositionSelectorViewModel()
        {
            PersonnelEditor.Setting.Load(PersonnelEditor.File);

            ExistingPositions = new ObservableCollection<Position>();

            SelectedPositions = new ObservableCollection<Position>();

            ICollectionView view = System.Windows.Data.CollectionViewSource.GetDefaultView(ExistingPositions);
            view.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
            
            view = System.Windows.Data.CollectionViewSource.GetDefaultView(SelectedPositions);
            view.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
        }

        public void Init(ScoutTypes scoutType)
        {
            ExistingPositions.Clear();
            switch (scoutType)
            {
                case ScoutTypes.Offensive:
                    foreach (Position pos in PersonnelEditor.Setting.GetOffensePositions())
                    {
                        ExistingPositions.Add(pos);
                    }
                    break;
                case ScoutTypes.Defensive:
                    foreach (Position pos in PersonnelEditor.Setting.GetDefensePositions())
                    {
                        ExistingPositions.Add(pos);
                    }
                    break;
            }
            SelectedPositions.Clear();
        }
    }
}
