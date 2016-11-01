using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                if (_nodeList == null)
                {
                    _nodeList = new List<Node>();
                }

                return _nodeList;
            }
            set
            {
                _nodeList = value;
            }
        }
    }

    public class Node : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        private bool _isExternalNode = false;
        private bool? _ischecked = false;
        private string _ip = "";
        private string _domain = "";
        private string _header = "";
        private string _tooltip = null;
        private Node _parentNode = null;
        private List<Node> _nodeList = null;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        public bool IsExternalNode
        {
            get
            {
                return _isExternalNode;
            }
            set
            {
                _isExternalNode = value;
                this.OnPropertyChanged("IsExternalNode");
            }
        }

        public bool? IsChecked
        {
            get
            {
                return _ischecked;
            }
            set
            {
                _ischecked = value;
            }
        }

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
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
                if (_nodeList == null)
                {
                    _nodeList = new List<Node>();
                }

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

        public void Initialize()
        {
            foreach (Node childNode in this.NodeList)
            {
                childNode._parentNode = this;
                childNode.Initialize();
            }
        }

        //깊은 복사
        public object Clone()
        {
            Node node = new Node();
            node.IsChecked = this._ischecked;
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

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
