using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    public enum NodeEditPosition
    {
        ChildNodeAdd = -1,
        ThisNodeEdit = 0,
        SameNodeAdd = 1,
        RootNodeAdd = 2,
        ThisNodeDel = 10
    }
}
