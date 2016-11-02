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

        public void IsDomainDuplication(string domain)
        {
            if(_nodeList != null)
            {
                foreach (Node node in _nodeList)
                {
                    SetIsChecked(node, domain);
                }
            }
        }

        private void SetIsChecked(Node node, string domain)
        {
            if (node.IsChecked == true && node.Domain == domain)
            {
                node.IsChecked = false;
            }

            if (node.IsExternalNode == false)
            {
                foreach (Node childeNode in node.NodeList)
                {
                    SetIsChecked(childeNode, domain);
                }
            }
        }
    }

    public class Node : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        private bool _isExternalNode = true;
        private bool? _isChecked = false;
        private string _ip = null;
        private string _domain = null;
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
                return _isChecked;
            }
            set
            {
                this.SetIsChecked(value, true, true);
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
                if (_isExternalNode == false && _nodeList == null)
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
            if(this.IsExternalNode == false)
            {
                foreach (Node childNode in this._nodeList)
                {
                    childNode._parentNode = this;
                    childNode.Initialize();
                }
            }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
            {
                return;
            }

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue && _isExternalNode == false)
            {
                foreach (Node childNode in _nodeList)
                {
                    childNode.SetIsChecked(_isChecked, true, false);
                }
            }

            if (updateParent && _parentNode != null)
            {
                _parentNode.VerifyCheckedState();
            }

            this.OnPropertyChanged("IsChecked");
        }

        private void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < this._nodeList.Count; ++i)
            {
                bool? current = this._nodeList[i].IsChecked;

                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        //깊은 복사
        public object Clone()
        {
            Node node = new Node();
            node._isSelected = this._isSelected;
            node._isExternalNode = this._isExternalNode;
            node._isChecked = this._isChecked;
            node._ip = this._ip;
            node._domain = this._domain;
            node._header = this._header;
            node._tooltip = this._tooltip;

            if (this._parentNode != null)
            {
                node.ParentNode = (Node)this._parentNode.Clone();
            }            

            if (this._nodeList != null)
            {
                node.NodeList = new List<Node>();

                foreach (Node childNode in this._nodeList)
                {
                    node.NodeList.Add((Node)childNode.Clone());
                }
            }

            return node;
        }

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
