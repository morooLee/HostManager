using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    public class TreeViewItemModel
    {
        private List<Node> _nodeList = null;

        public List<Node> NodeList
        {
            get
            {
                return _nodeList;
            }
            set
            {
                _nodeList = value;
            }
        }

        // 깊은 복사
        public object Clone()
        {
            List<Node> nodeList = new List<Node>();

            foreach(Node node in this._nodeList)
            {
                nodeList.Add((Node)node.Clone());
            }
            return nodeList;
        }

        public static List<Node> NodeListGetAll()
        {
            List<Node> parentList = new List<Node>();
            List<Node> childrenList1 = new List<Node>();
            List<Node> childrenList2 = new List<Node>();
            List<Node> childrenList3 = new List<Node>();

            childrenList1.Add(new Node("child_A"));
            childrenList1.Add(new Node("child_B"));
            childrenList1.Add(new Node("child_C"));
            childrenList1.Add(new Node("child_D"));
            childrenList1.Add(new Node("child_E"));
            childrenList1.Add(new Node("child_F"));

            foreach(Node node in childrenList1)
            {
                childrenList2.Add((Node)node.Clone());
            }

            foreach (Node node in childrenList1)
            {
                childrenList3.Add((Node)node.Clone());
            }

            parentList.Add(new Node("parent_A"));
            parentList[0].NodeList = childrenList1;
            parentList.Add(new Node("parent_B"));
            parentList[1].NodeList = childrenList2;
            parentList.Add(new Node("parent_C"));
            parentList[2].NodeList = childrenList3;

            return parentList;
        }
    }

    public class Node
    {
        private bool? _check = false;
        private string _header = "";
        private string _tooltip = "";
        private Node _parentNode = null;
        private List<Node> _nodeList = null;

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

        public List<Node> NodeList
        {
            get
            {
                return _nodeList;
            }
            set
            {
                _nodeList = value;
            }
        }

        public Node()
        {

        }

        public Node(string header)
        {
            _header = header;
        }

        //깊은 복사
        public object Clone()
        {
            Node node = new Node();
            node.Check = this._check;
            node.Header = this._header;
            node.Tooltip = this._tooltip;

            if (this._parentNode != null)
            {
                node.ParentNode = (Node)this._parentNode.Clone();
            }            

            if (this._nodeList != null)
            {
                node.NodeList = new List<Node>();

                foreach (Node childNode in this.NodeList)
                {
                    node.NodeList.Add((Node)childNode.Clone());
                }
            }

            return node;
        }
    }
}
