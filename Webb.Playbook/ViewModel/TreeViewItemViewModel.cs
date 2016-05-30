using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Webb.Playbook.ViewModel
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        #region Data

        static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        readonly ObservableCollection<TreeViewItemViewModel> _children;
        readonly TreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;

        private object toolTip;
        public object ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        // 09-19-2010 Scott
        private string childrenPath;
        public string ChildrenPath
        {
            get { return childrenPath; }
            set { childrenPath = value; }
        }

        private string image;
        public string Image
        {
            get { return image; }
            set 
            { 
                image = value;

                OnPropertyChanged("Image");
            }
        }

        #endregion // Data

        #region Constructors

        // 09-19-2010 Scott
        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren, string strChildrenPath)
        {
            ChildrenPath = strChildrenPath;

            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren && HasChildren())
                _children.Add(DummyChild);

            image = string.Empty;
        }

        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren && HasChildren())
                _children.Add(DummyChild);

            image = string.Empty;
        }

        // This is used to create the DummyChild instance.
        public TreeViewItemViewModel()
        {

        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        // 09-19-2010 Scott
        protected virtual bool HasChildren()
        {
            return true;
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get 
            {
                bool bHasDummy = false;
                
                foreach (TreeViewItemViewModel tvivm in this.Children)
                {
                    if (tvivm == DummyChild)
                    {
                        bHasDummy = true;

                        break;
                    }
                }
                return bHasDummy;
            }
        }

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get 
            {
                return _isExpanded; 
            }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }

                //03-30-2010 scott
                OnExpanded(IsExpanded);
            }
        }

        public virtual void OnExpanded(bool bExpanded)
        {

        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                    
                    //07-30-2009 scott
                    OnSelected(_isSelected);
                }
            }
        }

        //07-30-2009 scott
        public virtual void OnSelected(bool bSelected)
        {
            FormationRootViewModel.SelectedViewModel = bSelected ? this : null;
            PlaybookRootViewModel.SelectedViewModel = bSelected ? this : null;
        }

        #endregion // IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
            this.Children.Clear();
        }

        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, arg);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region manage chidren
        public virtual void AddChild(TreeViewItemViewModel child)
        {
            // Lazy load the child items, if necessary.
            if (this.HasDummyChild)
            {
                this.Children.Remove(DummyChild);
            }

            this.AddChildByOrder(child);    // 09-21-2010 Scott

            //this.LoadChildren();          // 09-21-2010 Remove by Scott
            this.IsExpanded = true;         // 09-21-2010 Scott
            child.IsSelected = true;        // 09-21-2010 Scott
        }

        protected virtual void AddChildByOrder(TreeViewItemViewModel child)
        {
            this.Children.Add(child);
        }

        public virtual void RemoveChild(TreeViewItemViewModel child)
        {
            this.Children.Remove(child);
        }
        #endregion

        // 09-15-2010 Scott
        public virtual void Refresh()
        {
            this.LoadChildren();

            foreach (TreeViewItemViewModel tvivm in this.Children)
            {
                tvivm.LoadChildren();
            }
        }

        // 11-16-2010 Scott
        public virtual T GetChild<T>(string strName)
            where T : ViewModel.TreeViewItemViewModel
        {
            if (Children != null)
            {
                IEnumerable<T> arrayT = Children.OfType<T>();

                if(typeof(T) == typeof(ViewModel.FormationViewModel))
                {
                    IEnumerable<ViewModel.FormationViewModel> arrayForm = arrayT.OfType<ViewModel.FormationViewModel>();

                    if (arrayForm != null && arrayForm.Count() > 0)
                    {
                        return arrayForm.First(f => string.Compare(f.FormationName,strName,true) == 0) as T;
                    }
                }

                if (typeof(T) == typeof(ViewModel.PlayViewModel))
                {
                    IEnumerable<ViewModel.PlayViewModel> arrayPlay = arrayT.OfType<ViewModel.PlayViewModel>();

                    if (arrayPlay != null && arrayPlay.Count() > 0)
                    {
                        return arrayPlay.First(p => string.Compare(p.PlayName, strName, true) == 0) as T;
                    }
                }

                if (typeof(T) == typeof(ViewModel.FolderViewModel))
                {
                    IEnumerable<ViewModel.FolderViewModel> arrayFolder = arrayT.OfType<ViewModel.FolderViewModel>();

                    if (arrayFolder != null && arrayFolder.Count() > 0)
                    {
                        return arrayFolder.First(f => string.Compare(f.FolderName, strName, true) == 0) as T;
                    }
                }

                if (typeof(T) == typeof(ViewModel.TitleViewModel))
                {
                    IEnumerable<ViewModel.TitleViewModel> arrayFolder = arrayT.OfType<ViewModel.TitleViewModel>();

                    if (arrayFolder != null && arrayFolder.Count() > 0)
                    {
                        return arrayFolder.First(f => string.Compare(f.TitleName, strName, true) == 0) as T;
                    }
                }
            }

            return null;
        }
    }
}
