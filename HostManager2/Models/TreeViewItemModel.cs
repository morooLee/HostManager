using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    /// <summary>
    /// 트리뷰 아이템 리스트 관리 모델
    /// </summary>
    public class TreeViewItemModel
    {
        // TreeView 아이템 리스트
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

        //public TreeViewItemModel()
        //{
        //    _nodeList = new List<Node>();
        //}

        //public TreeViewItemModel(List<Node> nodeList)
        //{
        //    _nodeList = nodeList;
        //}

        /// <summary>
        /// 중복체크를 위한 트리뷰 아이템 리스트 검색
        /// </summary>
        /// <param name="newNode">체크 상태가 갱신된 노드</param>
        public void IsDomainDuplication(Node newNode)
        {
            if(_nodeList != null)
            {
                foreach (Node node in _nodeList)
                { 
                    SetIsChecked(node, newNode);
                }
            }
        }

        /// <summary>
        /// 중복 시 기존 노드의 체크 상태 해제하기
        /// </summary>
        /// <param name="oldNode">중복여부를 검사할 노드</param>
        /// <param name="newNode">중복여부를 확인할 노드</param>
        private void SetIsChecked(Node oldNode, Node newNode)
        {
            if(oldNode.IsExternalNode)
            {
                if (oldNode.IsChecked == true && oldNode.Domain == newNode.Domain)
                {
                    if(oldNode.GetHashCode() != newNode.GetHashCode())
                    {
                        oldNode.IsChecked = false;
                        return;
                    }
                }
            }
            else
            {
                foreach (Node childeNode in oldNode.NodeList)
                {
                    SetIsChecked(childeNode, newNode);
                }
            }
        }

        public void AllISExpanded(bool isExpanded)
        {
            foreach (Node node in NodeList)
            {
                NodeIsExpanded(node, isExpanded);
            }
        }

        private void NodeIsExpanded(Node node, bool isExpanded)
        {
            if (node.IsChecked == false)
            {
                node.IsExpanded = isExpanded;
            }

            if (node.IsExternalNode == false)
            {
                foreach (Node childNode in node.NodeList)
                {
                    NodeIsExpanded(childNode, isExpanded);
                }
            }
        }

        public void ParentIsExpanded(Node parentNode)
        {
            if (parentNode != null)
            {
                if (parentNode.IsExpanded != true)
                {
                    parentNode.IsExpanded = true;
                }

                if (parentNode.ParentNode != null)
                {
                    ParentIsExpanded(parentNode.ParentNode);
                }
            }
        }
    }

    /// <summary>
    /// 트리뷰 아이템 모델
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        private bool _isExpanded = false;
        private bool _isExternalNode = true;
        private bool? _isChecked = false;
        private string _ip = null;
        private string _domain = null;
        private string _header = "";
        private string _tooltip = null;
        private Node _parentNode = null;
        private List<Node> _nodeList = null;
        private string _textBlockName = null;

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
                this.OnPropertyChanged("IsExpanded");
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

        public string TextBlockName
        {
            get
            {
                return _textBlockName;
            }
            set
            {
                _textBlockName = value;
            }
        }

        public Node()
        {

        }

        public Node(string header)
        {
            _header = header;
        }

        // 부모노드 설정
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

        /// <summary>
        /// 체크박스 업데이트
        /// </summary>
        /// <param name="value">체크 상태</param>
        /// <param name="updateChildren">자식노드 업데이트 여부</param>
        /// <param name="updateParent">부모노드 업데이트 확인</param>
        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
            {
                return;
            }

            _isChecked = value;
            
            // 체크값이 true 또는 null인 경우 노드 확장하기
            if (_isChecked != false)
            {
                this._isExpanded = true;
                this.OnPropertyChanged("IsExpanded");
            }
            else
            {
                this._isExpanded = false;
            }

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

        // 체크박스 상태 변경
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
        //public object Clone()
        //{
        //    Node node = new Node();
        //    node._isSelected = this._isSelected;
        //    node._isExternalNode = this._isExternalNode;
        //    node._isChecked = this._isChecked;
        //    node._ip = this._ip;
        //    node._domain = this._domain;
        //    node._header = this._header;
        //    node._tooltip = this._tooltip;
        //    node._textBlockName = this._textBlockName;

        //    if (this._parentNode != null)
        //    {
        //        node.ParentNode = (Node)this._parentNode.Clone();
        //    }            

        //    if (this._nodeList != null)
        //    {
        //        node.NodeList = new List<Node>();

        //        foreach (Node childNode in this._nodeList)
        //        {
        //            node.NodeList.Add((Node)childNode.Clone());
        //        }
        //    }

        //    return node;
        //}

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
        public void CopyTo(Node node)
        {
            this.IsSelected = node._isSelected;
            this.IsExternalNode = node._isExternalNode;
            this.IsChecked = node._isChecked;
            this.IP = node._ip;
            this.Domain = node._domain;
            this.Header = node._header;
            this.Tooltip = node._tooltip;
            this.TextBlockName = node._textBlockName;

            if (node.ParentNode != null)
            {
                this.ParentNode = node._parentNode;
            }

            if (node.NodeList != null)
            {
                if(this.NodeList == null)
                {
                    this.NodeList = new List<Node>();
                }
                foreach (Node childNode in node._nodeList)
                {
                    this.NodeList.Add(childNode);
                }
            }

            this.IsExpanded = true;
        }

        /// <summary>
        /// 이벤트 생성
        /// </summary>
        /// <param name="prop">생성할 이벤트명</param>
        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        // 이벤트 변수
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
