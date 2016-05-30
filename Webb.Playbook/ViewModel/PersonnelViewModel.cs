using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Webb.Playbook.Data;

namespace Webb.Playbook.ViewModel
{
    public class PersonnelViewModel : TreeViewItemViewModel
    {
        protected Personnel personnel;
        public Personnel Personnel
        {
            get { return personnel; }
            set { personnel = value; }
        }

        public PersonnelViewModel(Personnel personnel, ScoutTypeViewModel stv)
            : base(stv, true)
        {
            this.personnel = personnel;

            // sorting
            System.ComponentModel.ICollectionView view = System.Windows.Data.CollectionViewSource.GetDefaultView(Children);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("FormationName", System.ComponentModel.ListSortDirection.Ascending));
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            foreach (string strFile in Directory.GetFiles(PersonnelPath,"*.Form"))
            {
                Formation formation = new Formation(strFile);
                base.Children.Add(new FormationViewModel(formation, this));
            }
        }

        public string PersonnelName
        {
            get { return personnel.Name; }
        }

        public string PersonnelPath
        {
            get { return personnel.Path; }
        }

        public override void RemoveChild(TreeViewItemViewModel child)
        {
            FormationViewModel fvm = child as FormationViewModel;

            if (fvm != null)
            {
                System.IO.File.Delete(fvm.FormationPath);
            }

            base.RemoveChild(child);
        }

        public override void AddChild(TreeViewItemViewModel child)
        {
            base.AddChild(child);
        }
    }
}
