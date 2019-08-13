using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Existek.Model
{
    interface INode
    {
        string Title { get; }
        RoleType getRole();
        ObservableCollection<INode> children { get; set; }
    }
}
