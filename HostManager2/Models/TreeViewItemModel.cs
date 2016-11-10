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
        #region Members

        private ObservableCollection<Node> _nodeList = null;

        #endregion

        #region Properties

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

        #endregion

        #region 생성자

        public TreeViewItemModel() : this(null)
        {

        }

        public TreeViewItemModel(ObservableCollection<Node> nodeList)
        {
            _nodeList = nodeList;
        }

        #endregion

        #region Function

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

        /// <summary>
        /// 전체 노드 열기/접기
        /// </summary>
        /// <param name="isExpanded">true : 열기 / false : 접기</param>
        public void AllISExpanded(bool isExpanded)
        {
            foreach (Node node in NodeList)
            {
                NodeIsExpanded(node, isExpanded);
            }
        }

        /// <summary>
        /// 체크안된 노드 열기/접기
        /// </summary>
        /// <param name="node">체크여부 확인할 노드</param>
        /// <param name="isExpanded">true : 열기 / false : 접기</param>
        private void NodeIsExpanded(Node node, bool isExpanded)
        {
            node.IsExpanded = isExpanded;

            if (node.IsExternalNode == false)
            {
                foreach (Node childNode in node.NodeList)
                {
                    NodeIsExpanded(childNode, isExpanded);
                }
            }
        }

        /// <summary>
        /// 전체 노드 변경여부 취소하기
        /// </summary>
        public void SetIsChangedAll(bool isChanged)
        {
            foreach (Node node in NodeList)
            {
                node.SetIsChanged(isChanged);
            }
        }

        public int WordSearchInNode(string word)
        {
            int count = 0;

            foreach(Node node in NodeList)
            {
                count += node.Search(word);
            }

            return count;
        }

        #endregion
    }

    /// <summary>
    /// 트리뷰 아이템 모델
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        #region Members

        private bool _isSelected = false;
        private bool _isExpanded = false;
        private bool _isExternalNode = true;
        private bool _isChanged = false;
        private bool? _isChecked = false;
        private string _ip = null;
        private string _domain = null;
        private string _header = "";
        private string _tooltip = null;
        private Node _parentNode = null;
        private ObservableCollection<Node> _nodeList = null;
        private string _textBlockName = null;

        #endregion

        #region Properties

        // 선택 여부
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
        // 확장 여부
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
        // 마지막 노드 여부
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
        // 변경 여부
        public bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                _isChanged = value;
                this.OnPropertyChanged("IsChanged");
            }
        }
        // 체크 여부
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
        // 자식 노드들
        public ObservableCollection<Node> NodeList
        {
            get
            {
                if (_isExternalNode == false && _nodeList == null)
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
        // 노드와 연결된 TextBlock Name
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

        #endregion

        #region 생성자

        public Node()
        {

        }

        public Node(string header)
        {
            _header = header;
        }

        #endregion

        #region Function

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

        // 노드 수정 여부
        public void SetIsChanged(bool value)
        {
            _isChanged = value;

            if (this._isExternalNode == false)
            {
                foreach (Node childNode in NodeList)
                {
                    childNode.SetIsChanged(value);
                }
            }

            this.OnPropertyChanged("IsChanged");
        }

        public int Search(string searchString)
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
            if (_isExternalNode == false)
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

        #endregion
    }
}
