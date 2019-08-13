using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Existek.Model
{
    class GroupInTree : INode, INotifyPropertyChanged
    {
        private static int i = 0;
        private string folder;
        private RoleType _myRole = RoleType.Group;

        public RoleType getRole() { return _myRole; }
        public string Title
        {
            get { return folder; }
        }

        public ObservableCollection<INode> children { get; set; }
        private void SetName()
        {
            i++;
            folder = "Group" + i.ToString() + " ";
        }

        public GroupInTree()
        {
            children = new ObservableCollection<INode>();
            SetName();
        }

        public GroupInTree(GroupInTree parent)
        {
  
            children = new ObservableCollection<INode>();
            SetName();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}