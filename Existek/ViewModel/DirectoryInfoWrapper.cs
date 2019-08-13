using Existek.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace Existek.ViewModel
{
    class DirectoryInfoWrapper : INotifyPropertyChanged
    {
        private INode _data;
        private RoleType type;

        public RoleType Type
        {
            get { return type; }
            set { type = _data.getRole(); }
        }

        public DirectoryInfoWrapper(INode node)
        {
            _data = node;
        }

        private DirectoryInfoWrapper parent=null;

        public DirectoryInfoWrapper Parent
        {
            get { return parent; }
            set { parent = value; }
        }


        public INode Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    OnPropertyChanged("Data");
                }
            }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        private bool _isExpanded = false;

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
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        private ObservableCollection<DirectoryInfoWrapper> _children=new ObservableCollection<DirectoryInfoWrapper>();

        public ObservableCollection<DirectoryInfoWrapper> Children
        {
            get
            {
                return _children;
            }
            set
            {
                if (value != _children)
                {
                    _children = value;
                    OnPropertyChanged("Children");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string p_propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p_propertyName));
            }
        }
    }
}
