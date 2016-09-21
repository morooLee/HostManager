using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    public class Node
    {
        private bool? _check = false;
        private string _header = null;
        private string _tooltip = null;
        private string _domain = null;
        private Node _parentNode = null;
        private List<Node> _childrenNodeList = null;

        public Node()
        {

        }

        public void Initialize()
        {
            if (this.ChildrenNodeList != null)
            {
                foreach (Node childNode in this.ChildrenNodeList)
                {
                    childNode._parentNode = this;
                    childNode.Initialize();
                }
            }
        }

        public Node(bool? check, string header, string tooltip, string domain, Node parentNode, List<Node> nodeList)
        {
            _check = check;
            _header = header;
            _tooltip = tooltip;
            _domain = domain;
            _parentNode = parentNode;
            _childrenNodeList = nodeList;
        }

        public bool? Check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
            }
        }

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public string Tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                _tooltip = value;
            }
        }

        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        public Node ParentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                _parentNode = value;
            }
        }

        public List<Node> ChildrenNodeList
        {
            get
            {
                return _childrenNodeList;
            }
            set
            {
                _childrenNodeList = value;
            }
        }
    }
}
