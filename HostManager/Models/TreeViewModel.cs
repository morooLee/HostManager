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

        public Node(string header) : this(false, header, null, null)
        {

        }

        public Node(bool? check, string header, string tooltip, ObservableCollection<Node> nodeList)
        {
            _check = check;
            _header = header;
            _tooltip = tooltip;
            _nodeList = nodeList;
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
                this.OnPropertyChanged("IsExpanded");
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

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
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

        void VerifyCheckedState()
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

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
