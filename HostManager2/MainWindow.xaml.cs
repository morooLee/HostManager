using HostManager.Controllers;
using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HostManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 트리뷰 아이템 모델 생성
        /// </summary>
        TreeViewItemModel treeViewItemModel = new TreeViewItemModel();

        /// <summary>
        /// 파일 컨트롤러 등록
        /// </summary>
        FileController fileController = new FileController();

        /// <summary>
        /// 체크된 Domain 리스트
        /// </summary>
        HashSet<string> domainHashSet = new HashSet<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region TreeView 이벤트 및 관련 함수

        /// <summary>
        /// 트리뷰 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hosts_TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree();
        }

        /// <summary>
        /// 트리뷰 바인딩
        /// </summary>
        private void BindTree()
        {
            treeViewItemModel.NodeList = fileController.ConverterToNodeList();
            Hosts_TreeView.ItemsSource = treeViewItemModel.NodeList;

            // 최초 체크상태 UI 업데이트
            foreach (Node node in treeViewItemModel.NodeList)
            {
                if (node.IsChecked != false)
                {
                    DomainDuplication(node);
                }
            }
        }

        #endregion

        #region CheckBox 이벤트 및 관련 함수

        /// <summary>
        /// 체크박스 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();

            if (ckb.IsChecked == true || ckb.IsChecked == null)
            {
                sp.Opacity = 1.0;
                ckb.BorderThickness = new Thickness(1, 1, 1, 1);
                ckb.BorderBrush = (Brush)Conv.ConvertFromString("Red");
            }
            else
            {
                sp.Opacity = 0.5;
                ckb.BorderThickness = new Thickness(1, 1, 1, 1);
                ckb.BorderBrush = (Brush)Conv.ConvertFromString("Black");
            }
        }

        /// <summary>
        /// 체크박스 Check 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter conv = new BrushConverter();            
            Node node = sp.DataContext as Node;

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Red");

            // 중복체크 확인
            if (node.IsSelected)
            {
                DomainDuplication(node);
            }
        }

        /// <summary>
        /// 체크박스 UnCheck 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter conv = new BrushConverter();
            Node node = sp.DataContext as Node;

            sp.Opacity = 0.5;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Black");

            // 중복체크 확인
            if (node.IsSelected)
            {
                DomainDuplication(node);
            }
        }

        /// <summary>
        /// 체크박스 Null 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;           
            BrushConverter conv = new BrushConverter();
            Node node = sp.DataContext as Node;

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Red");

            // 중복체크 확인
            if (node.IsSelected)
            {
                DomainDuplication(node);
            }
        }

        /// <summary>
        /// 체크박스 좌클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            Node node = sp.DataContext as Node;

            // 좌클릭 시 트리뷰 선택하기
            node.IsSelected = true;
        }

        /// <summary>
        /// 도메인 중복체크 확인
        /// </summary>
        /// <param name="node">중복체크를 여부를 확인 할 노드</param>
        private void DomainDuplication(Node node)
        {
            if (node.IsChecked != false)
            {
                if (node.IsExternalNode)
                {
                    bool hashSetResult = false;
                    hashSetResult = domainHashSet.Add(node.Domain);

                    if (hashSetResult == false)
                    {
                        treeViewItemModel.IsDomainDuplication(node);
                    }
                }
                else
                {
                    foreach (Node childNode in node.NodeList)
                    {
                        if (childNode.IsChecked != false)
                        {
                            DomainDuplication(childNode);
                        }
                    }
                }
            }
            else
            {
                if (node.IsExternalNode)
                {
                    if (node.IsChecked == true)
                    {
                        domainHashSet.Remove(node.Domain);
                    }
                }
                else
                {
                    foreach (Node childNode in node.NodeList)
                    {
                        DomainDuplication(childNode);
                    }
                }
            }
        }

        #endregion

        #region 도구모음 이벤트 및 관련 함수

        /// <summary>
        /// 적용버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            DoApply();
        }

        /// <summary>
        /// 호스트 저장하기
        /// </summary>
        /// <returns></returns>
        private bool DoApply()
        {
            bool applyResult = false;
            applyResult = fileController.ConverterToString(treeViewItemModel, null);

            return applyResult;
        }

        #endregion
    }
}
