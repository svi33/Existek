using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Existek.Model
{
    class ControlInGroup : INode, INotifyPropertyChanged
    {
        private static int i = 0;
        private string title;
        private GroupInTree _parent;
        private string name;
        private RoleType _myRole= RoleType.Control;
        private RoleType _selectedRole;
        private string value;

        public string Name { get { return name; } set { name = value; } }
        public RoleType getRole() { return _myRole; }
        public string Value { get { return name; } set { name = value; } }
        public string Title
        {
            get { return title; }
        }

        public ObservableCollection<INode> children { get; set; }
        private void SetName()
        {
            i++;
            title = "Control" + i.ToString()+" ("+name+", " + _selectedRole + ", "+value+ ")";
        }

        public ControlInGroup(GroupInTree parent, INode selected)
        {
            _parent = parent;
            name = selected.Title.Substring(0, selected.Title.IndexOf(' '));
            _selectedRole = selected.getRole();
            value = Regex.Split(selected.Title, @"\D+")[1];
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