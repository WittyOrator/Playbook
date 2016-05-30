using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class MenuViewModel
    {
        private string header;
        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        private string image;
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public MenuViewModel(string str)
        {
            Header = str;
            Image = string.Empty;
        }

        public override string ToString()
        {
            return Header;
        }
    }

    public class MenuManager
    {
        private ObservableCollection<MenuViewModel> menu;
        public ObservableCollection<MenuViewModel> Menu
        {
            get { return menu; }
            set { menu = value; }
        }

        private string[] FormationMenu = { /*"Open",*/"Copy", "Rename", "Create Play", "Delete" };
        private string[] PersonnelMenu = { "New Formation", "Delete" };
        private string[] ScoutTypeMenu = { /*"New Personnel",*/"Paste", "New Folder", "New Formation", "New Title" };
        private string[] PlayTypeMenu = { "Paste", "New Play", "New Folder" };
        private string[] PlayMenu = { /*"Open",*/ "Copy", "Rename", "Edit","Delete" };
        private string[] FolderMenu = { "Paste", "New Folder", "New Formation", "Delete Folder", "Rename Folder", "New Title" };
        private string[] PlayFolderMenu = { "Paste", "New Folder", "New Play", "Delete Folder", "Rename Folder" };
        private string[] AdjustmentTypeMenu = { "New Adjustment" };
        private string[] AdjustmentMenu = { "Delete" };
        private string[] FormationPlayMenu = { /*"Open",*/ "Rename", "Delete" };
        private string[] TitleMenu = { "Delete" };

        public MenuManager()
        {
            Menu = new ObservableCollection<MenuViewModel>();
        }

        public void Clear()
        {
            Menu.Clear();
        }

        public void LoadMenu(string[] menu)
        {
            Clear();

            foreach (string str in menu)
            {
                Menu.Add(new MenuViewModel(str));
            }
        }

        public void LoadFolderMenu(int nScoutType)
        {
            switch (nScoutType)
            {// 10-26-2011 Scott
                case 0:
                    FolderMenu[2] = "New " + GameSetting.Instance.OffensiveMainField;
                    break;
                case 1:
                    FolderMenu[2] = "New " + GameSetting.Instance.DefensiveMainField;
                    break;
                case 2:
                    FolderMenu[2] = "New " + GameSetting.Instance.KickMainField;
                    break;
                default:
                    FolderMenu[2] = "New " + GameSetting.Instance.OffensiveMainField;
                    break;
            }
            
            LoadMenu(FolderMenu);
        }

        public void LoadPlayFolderMenu()
        {
            LoadMenu(PlayFolderMenu);
        }

        public void LoadFormationMenu(int nScoutType)
        {
            switch (nScoutType)
            {// 10-26-2011 Scott
                case 0:
                    FormationMenu[2] = "Create " + GameSetting.Instance.OffensiveSubField;
                    break;
                case 1:
                    FormationMenu[2] = "Create " + GameSetting.Instance.DefensiveSubField;
                    break;
                case 2:
                    FormationMenu[2] = "Create " + GameSetting.Instance.KickSubField;
                    break;
                default:
                    FormationMenu[2] = "Create " + GameSetting.Instance.OffensiveSubField;
                    break;
            }
            
            LoadMenu(FormationMenu);
        }

        public void LoadPersonnelMenu()
        {
            LoadMenu(PersonnelMenu);
        }

        public void LoadScoutTypeMenu(int nScoutType)
        {
            switch (nScoutType)
            {// 10-26-2011 Scott
                case 0:
                    ScoutTypeMenu[2] = "New " + GameSetting.Instance.OffensiveMainField;
                    break;
                case 1:
                    ScoutTypeMenu[2] = "New " + GameSetting.Instance.DefensiveMainField;
                    break;
                case 2:
                    ScoutTypeMenu[2] = "New " + GameSetting.Instance.KickMainField;
                    break;
                default:
                    ScoutTypeMenu[2] = "New " + GameSetting.Instance.OffensiveMainField;
                    break;
            }

            LoadMenu(ScoutTypeMenu);
        }

        public void LoadPlayTypeMenu()
        {
            LoadMenu(PlayTypeMenu);
        }

        public void LoadPlayMenu()
        {
            LoadMenu(PlayMenu);
        }

        public void LoadAdjustmentTypeMenu()
        {
            LoadMenu(AdjustmentTypeMenu);
        }

        public void LoadAdjustmentMenu()
        {
            LoadMenu(AdjustmentMenu);
        }

        public void LoadTitleMenu()
        {
            LoadMenu(TitleMenu);
        }

        public void LoadFormationPlayMenu()
        {
            LoadMenu(FormationPlayMenu);
        }
    }
}
