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
        TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
        HostIOController hostIOController = new HostIOController();
        TreeViewItemConverterController treeViewItemConverterController = new TreeViewItemConverterController();
        HashSet<string> domainHashSet = new HashSet<string>();
        bool isChangedHost = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region TreeView 이벤트 및 관련 함수

        // 트리뷰 로드 이벤트
        private void Hosts_TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree(null);
        }

        // 트리뷰 SelectedItemChanged 이벤트
        private void Hosts_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Node selectedNode = e.NewValue as Node;
            //SelectedNode = item;

            statusBarUpdate(selectedNode);
        }

        /// <summary>
        /// 트리뷰 바인딩
        /// </summary>
        /// <param name="hosts">바인딩할 string 개체 (null이면  파일에서 string 개체 생성)</param>
        private void BindTree(string hosts)
        {
            if (hosts == null)
            {
                treeViewItemModel.NodeList = treeViewItemConverterController.ConverterToNodeList(null);
            }
            else
            {
                treeViewItemModel.NodeList = treeViewItemConverterController.ConverterToNodeList(hosts);
            }

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

        // RichTextBox 이벤트 및 관련 함수
        private void Hosts_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hosts_RichTextBox.Document.Blocks == null)
            {
                //Console.WriteLine(Hosts_RichTextBox.Document.Blocks.Count);
            }
            Console.WriteLine(Hosts_RichTextBox.Document.Blocks.Count);
            //isChangedHost = true;
        }

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
            isChangedHost = true;
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

        private void TreeView_Button_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);

            BindTree(textRange.Text);

            ChangeButtonUI();
        }

        private void RichTextBox_Button_Click(object sender, RoutedEventArgs e)
        {
            Hosts_RichTextBox.Document.Blocks.Clear();
            Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewItemConverterController.ConverterToString(treeViewItemModel, null, false))));

            ChangeButtonUI();
        }

        private void NodePad_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenNotepad();
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            DoApply();
        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            DoRefresh();
        }

        private void ChangeButtonUI()
        {
            //SearchBoxClear();

            if (Hosts_TreeView.Visibility == Visibility.Visible)
            {
                TreeView_Button.IsEnabled = true;
                RichTextBox_Button.IsEnabled = false;
                Hosts_TreeView.Visibility = Visibility.Hidden;
                Hosts_RichTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                TreeView_Button.IsEnabled = false;
                RichTextBox_Button.IsEnabled = true;
                Hosts_TreeView.Visibility = Visibility.Visible;
                Hosts_RichTextBox.Visibility = Visibility.Hidden;
            }
        }

        private void OpenNotepad()
        {
            if (isChangedHost == true)
            {
                MessageBoxResult result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n저장하지 않은 정보는 메모장에 반영이 되지 않습니다.\r\n저장하고 메모장으로 여시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DoApply();
                }
            }

            hostIOController.OpenNotepad();

            //ChangeInfoLabel("Info", "메모장에서 수정한 것을 반영하려면 새로고침을 누르세요.", null);
        }

        private void DoRefresh()
        {
            //SearchBoxClear();
            MessageBoxResult result = MessageBoxResult.Yes;

            if (isChangedHost == true)
            {
                result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n새로고침을 할 경우 저장되지 않은 정보는 삭제됩니다.\r\n그래도 새로고침을 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            if (result == MessageBoxResult.Yes)
            {
                if (Hosts_TreeView.Visibility == Visibility.Visible)
                {
                    BindTree(null);
                }
                else
                {
                    Hosts_RichTextBox.Document.Blocks.Clear();
                    Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewItemConverterController.ConverterToString(treeViewItemModel, null, false))));
                }

                if (SearchButtonImage.Tag.ToString() == "Cancel")
                {
                    SearchBox.Text = "";

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri("Resources/Search.png", UriKind.Relative);
                    bi.EndInit();
                    SearchButtonImage.Source = bi;
                    SearchButtonImage.Tag = "Search";
                }

                Apply_Button.IsEnabled = false;
            }
            else
            {
                //ChangeInfoLabel("Info", "새로고침을 취소하였습니다.", null);
            }
        }

        // 저장하기
        private void DoApply()
        {
            treeViewItemConverterController.ConverterToString(treeViewItemModel, null, true);
            isChangedHost = false;
        }

        #endregion

        /// <summary>
        /// 선택한 노드 경로 출력하기
        /// </summary>
        /// <param name="selectedNode">선택한 노드</param>
        private void statusBarUpdate(Node selectedNode)
        {
            string sbText = "";

            if (selectedNode != null)
            {
                if (selectedNode.ParentNode != null)
                {
                    string path = "";
                    Node node = selectedNode.ParentNode;
                    while (true)
                    {
                        if (node.Header == "")
                        {
                            path = "Empty";
                        }
                        else
                        {
                            path = node.Header;
                        }

                        if (sbText == "")
                        {
                            sbText = path;
                        }
                        else
                        {
                            string tmpPath = sbText;
                            sbText = path + " > " + tmpPath;
                        }

                        if (node.ParentNode != null)
                        {
                            Node tmpNode = node.ParentNode;
                            node = tmpNode;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    sbText = "Root";
                }
            }

            statusBar.Items.Clear();
            statusBar.Items.Add(sbText);
        }
    }
}
