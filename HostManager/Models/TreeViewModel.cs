using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostManager.Models
{
    public class TreeViewModel
    {
        private ObservableCollection<Node> _nodeList = null;

        public TreeViewModel() : this(null)
        {

        }

        public TreeViewModel(ObservableCollection<Node> nodeList)
        {
            _nodeList = nodeList;
        }

        public ObservableCollection<Node> NodeList
        {
            get
            {
                if (_nodeList == null)
                {
                    _nodeList = new ObservableCollection<Node>();
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
        private bool _isLastNode = false;
        private bool? _check = false;
        private bool _isSelected = false;
        private bool _isExpanded = false;
        private string _header = "";
        private string _tooltip = null;
        private Node _parentNode;
        private ObservableCollection<Node> _nodeList = null;

        public Node()
        {
            
        }

        public void Initialize()
        {
            foreach (Node childNode in this.NodeList)
            {
                childNode._parentNode = this;
                childNode.Initialize();
            }
        }

        public Node(bool? check, string header, string tooltip, ObservableCollection<Node> nodeList)
        {
            _check = check;
            _header = header;
            _tooltip = tooltip;
            _nodeList = nodeList;
        }

        public bool IsLastNode
        {
            get
            {
                return _isLastNode;
            }
            set
            {
                _isLastNode = value;
                this.OnPropertyChanged("IsLastNode");
            }
        }

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

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
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
                this.OnPropertyChanged("Header");
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
                this.OnPropertyChanged("Tooltip");
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

        public ObservableCollection<Node> NodeList
        {
            get
            {
                if (_nodeList == null)
                {
                    _nodeList = new ObservableCollection<Node>();
                }
                return _nodeList;
            }
            set
            {
                _nodeList = value;
            }
        }

        public bool? IsChecked
        {
            get
            {
                return _check;
            }
            set
            {
                this.SetIsChecked(value, true, true);
            }
        }

        public int Search(String searchString)
        {
            int count = 0;

            if (this.Header.ToUpper().Contains(searchString.ToUpper()))
            {
                count++;

                if (_isExpanded != true)
                {
                    _isExpanded = true;
                    this.OnPropertyChanged("IsExpanded");
                }

                if (_parentNode != null)
                {
                    _parentNode.ParentIsExpanded();
                }
            }
            else
            {
                if (_isExpanded != false)
                {
                    _isExpanded = false;
                    this.OnPropertyChanged("IsExpanded");
                }
            }
            if (NodeList != null || NodeList.Count > 0)
            {
                foreach (Node node in this.NodeList)
                {
                    count += node.Search(searchString);
                }
            }

            return count;
        }

        private void ParentIsExpanded()
        {
            if (_isExpanded != true)
            {
                _isExpanded = true;
                this.OnPropertyChanged("IsExpanded");
            }
            
            if (_parentNode != null)
            {
                _parentNode.ParentIsExpanded();
            }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _check)
            {
                return;
            }

            _check = value;

            if (updateChildren && _check.HasValue)
            {
                foreach(Node childNode in NodeList)
                {
                    childNode.SetIsChecked(_check, true, false);
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

            for (int i = 0; i < NodeList.Count; ++i)
            {
                bool? current = NodeList[i].IsChecked;

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

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
