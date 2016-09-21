using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    class TreeViewModel
    {
        private bool _pass = true;
        private List<Node> _nodeList = null;

        public TreeViewModel() : this(null)
        {

        }

        public TreeViewModel(List<Node> nodeList)
        {
            _nodeList = nodeList;
        }

        public bool Pass
        {
            get
            {
                return _pass;
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

        public Node SearchNode(String domain)
        {
            Node tmpNode = null;

            foreach (Node node in _nodeList)
            {
                if ((tmpNode = SetSearchNode(node, domain)) != null)
                {
                    return tmpNode;
                }
            }

            return tmpNode;
        }

        private Node SetSearchNode(Node node, String domain)
        {
            Node tmpNode = null;

            foreach (Node item in node.ChildrenNodeList)
            {
                if (item.ChildrenNodeList == null)
                {
                    if (item.Domain == domain && item.Check == true)
                    {
                        item.Check = false;
                        break;
                    }
                }
                else
                {
                    foreach (Node childNode in node.ChildrenNodeList)
                    {
                        SetSearchNode(item, domain);
                    }
                }
            }

            return tmpNode;
        }

        public void DomainIsMatched()
        {
            List<String> domainList = DomainList();

            foreach (Node node in _nodeList)
            {
                if (SetDomainIsMatched(node, domainList) == false)
                {
                    foreach (Node item in _nodeList)
                    {
                        item.Check = false;
                    }
                    _pass = false;
                    return;
                }
            }

            _pass = true;
        }

        public void DomainIsMatched(List<String> domainList)
        {
            foreach (Node node in _nodeList)
            {
                if (SetDomainIsMatched(node, domainList) == false)
                {
                    foreach (Node item in _nodeList)
                    {
                        item.Check = false;
                    }
                    _pass = false;
                    return;
                }
            }

            _pass = true;
        }

        private bool SetDomainIsMatched(Node node, List<String> domainList)
        {
            int count = 0;

            if (node.Check == true && node.ChildrenNodeList == null)
            {
                count = domainList.Count(x => x == node.Domain);
                if (count > 1)
                {
                    return false;
                }
            }

            if (node.ChildrenNodeList != null || node.ChildrenNodeList.Count != 0)
            {
                foreach (Node childNode in node.ChildrenNodeList)
                {
                    if (SetDomainIsMatched(childNode, domainList) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public List<String> DomainList()
        {
            List<String> domainList = new List<string>();

            if (this._nodeList == null && this._nodeList.Count == 0)
            {
                return domainList;
            }
            else
            {
                foreach (Node node in this._nodeList)
                {
                    domainList.AddRange(SetDomainIsMatched(node));
                }
            }

            return domainList;
        }

        private List<String> SetDomainIsMatched(Node node)
        {
            List<String> domainList = new List<string>();

            if (node.Check == true && node.ChildrenNodeList == null)
            {
                domainList.Add(node.Domain);
            }

            if (node.ChildrenNodeList != null || node.ChildrenNodeList.Count != 0)
            {
                foreach (Node childNode in node.ChildrenNodeList)
                {
                    domainList.AddRange(SetDomainIsMatched(childNode));
                }
            }

            return domainList;
        }
    }
}
