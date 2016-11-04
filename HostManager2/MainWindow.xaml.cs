using HostManager.Controllers;
using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private void HeaderText_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text == "")
            {
                tb.Text = "(비어있음)";
                tb.FontStyle = FontStyles.Italic;
            }
        }

        private void HeaderText_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text == "")
            {
                tb.Text = "(비어있음)";
                tb.FontStyle = FontStyles.Italic;
            }
        }

        /// <summary>
        /// 트리뷰 바인딩
        /// </summary>
        /// <param name="hosts">바인딩할 string 개체 (null이면  파일에서 string 개체 생성)</param>
        private void BindTree(string hosts)
        {
            bool isConverted = true;

            if (hosts == null)
            {
                try
                {
                    treeViewItemModel.NodeList = treeViewItemConverterController.ConverterToNodeList(null).ToList();
                }
                catch (ArgumentNullException e)
                {
                    isConverted = false;
                }
            }
            else
            {
                try
                {
                    treeViewItemModel.NodeList = treeViewItemConverterController.ConverterToNodeList(hosts).ToList();
                }
                catch (ArgumentNullException e)
                {
                    isConverted = false;
                }
            }

            if (isConverted == false)
            {
                ChangeInfoLabel(InfoLabelType.Warning, "트리 변환에 실패하였습니다.");

                Hosts_RichTextBox.Document.Blocks.Clear();
                Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(hostIOController.HostLoad())));

                ChangeButtonUI();
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
            if (Hosts_RichTextBox.IsVisible == true)
            {
                if (isChangedHost == false && Hosts_RichTextBox.IsFocused)
                {
                    isChangedHost = true;
                    ChangeInfoLabel(InfoLabelType.Info, "수정된 항목이 있습니다.");
                }
            }
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
                    if (node.IsChecked == false)
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
            Console.WriteLine(domainHashSet.Count);
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

        // 버튼 UI 변경하기
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

        // Notepad 열기
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

        // 새로고침
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
            }
            else
            {
                //ChangeInfoLabel("Info", "새로고침을 취소하였습니다.", null);
            }

            isChangedHost = false;
        }

        // 저장하기
        private void DoApply()
        {
            if (treeViewItemConverterController.ConverterToString(treeViewItemModel, null, true) == null)
            {
                MessageBox.Show("호스트 적용에 실패하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "호스트 적용에 실패하였습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Warning, "적용되었습니다.");
            }

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

        private void ChangeInfoLabel(InfoLabelType type, String msg)
        {
            string BackgroundColor = "";
            string ForegroundColor = "";
            BrushConverter Conv = new BrushConverter();

            switch(type)
            {
                case InfoLabelType.Warning:
                    {
                        BackgroundColor = "#F2DEDE";
                        ForegroundColor = "#A94442";
                        break;
                    }
                case InfoLabelType.Success:
                    {
                        BackgroundColor = "#DFF0D8";
                        ForegroundColor = "#3C763D";
                        break;
                    }
                case InfoLabelType.Info:
                    {
                        BackgroundColor = "#D9EDF7";
                        ForegroundColor = "#31708F";
                        break;
                    }
                case InfoLabelType.None:
                    {
                        BackgroundColor = "#00FFFFFF";
                        ForegroundColor = "#00FFFFFF";
                        break;
                    }
            }

            InfoLabel.Background = (Brush)Conv.ConvertFromString(BackgroundColor);
            InfoLabel.Foreground = (Brush)Conv.ConvertFromString(ForegroundColor);
            InfoLabel.Content = msg;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("Resources/Search.png", UriKind.Relative);
            bi.EndInit();
            SearchButtonImage.Source = bi;
            SearchButtonImage.Tag = "Search";
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    return;
                }

                SearchButtonImage.Tag = "Search";
                SearchButton_Click(SearchButton, null);
            }
            else if ((e.Key == Key.Escape))
            {
                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    return;
                }

                SearchButtonImage.Tag = "Cancel";
                SearchButton_Click(SearchButton, null);
            }
            else if (e.Key == Key.Return && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                SearchButtonImage.Tag = "Search";
                SearchButton_Click(SearchButton, null);
            }
            else if (e.Key == Key.Escape && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                SearchButtonImage.Tag = "Cancel";
                SearchButton_Click(SearchButton, null);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchButtonImage.Tag.ToString() == "Search")
            {
                if (SearchBox.Text == "")
                {
                    ChangeInfoLabel(InfoLabelType.Warning, "검색어를 입력하세요.");
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri("Resources/Cancel.png", UriKind.Relative);
                    bi.EndInit();
                    SearchButtonImage.Source = bi;
                    SearchButtonImage.Tag = "Cancel";

                    int Count = 0;

                    if (Hosts_TreeView.Visibility == Visibility.Visible)
                    {
                        Count = treeViewItemModel.Search(SearchBox.Text);

                        if (Count == 0)
                        {
                            ChangeInfoLabel(InfoLabelType.Info, SearchBox.Text + "에 대한 검색 결과가 없습니다.");
                        }
                        else
                        {
                            HighlightText(Hosts_TreeView);
                            ChangeInfoLabel(InfoLabelType.Info, Count + "개 검색되었습니다.");
                        }
                    }
                    else
                    {
                        Regex rg = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);
                        TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
                        MatchCollection matches = rg.Matches(textRange.Text);
                        Count = matches.Count;

                        if (Count == 0)
                        {
                            ChangeInfoLabel(InfoLabelType.Info, SearchBox.Text + "에 대한 검색 결과가 없습니다.");
                        }
                        else
                        {
                            HighlightText(Hosts_RichTextBox);
                            ChangeInfoLabel(InfoLabelType.Info, Count + "개 검색되었습니다.");
                        }
                    }
                }
            }
            else
            {
                SearchBox.Text = "";

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Resources/Search.png", UriKind.Relative);
                bi.EndInit();
                SearchButtonImage.Source = bi;
                SearchButtonImage.Tag = "Search";

                if (Hosts_TreeView.Visibility == Visibility.Visible)
                {
                    HighlightText(Hosts_TreeView);
                }
                else
                {
                    HighlightText(Hosts_RichTextBox);
                }

                ChangeInfoLabel(InfoLabelType.Info, "검색을 취소하였습니다.");
            }
        }

        private void HighlightText(Object itx)
        {
            if (itx != null)
            {
                Regex regex = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);

                if (itx is TreeView)
                {
                    TreeView treeView = itx as TreeView;

                    Console.WriteLine(treeView.ItemTemplate.);
                }
                //if (itx is TextBlock)
                //{
                //    TextBlock textBlock = itx as TextBlock;
                //    Console.WriteLine(textBlock.Text);

                //    string str = textBlock.Text;
                //    textBlock.Inlines.Clear();
                //    textBlock.Inlines.Add(str);

                //    if (SearchBox.Text.Length == 0)
                //    {
                //        //string str = textBlock.Text;
                //        //textBlock.Inlines.Clear();
                //        //textBlock.Inlines.Add(str);

                //        return;
                //    }
                //    string[] substrings = regex.Split(textBlock.Text);
                //    textBlock.Inlines.Clear();
                //    foreach (string item in substrings)
                //    {
                //        if (regex.Match(item).Success)
                //        {
                //            Run runx = new Run(item);
                //            runx.Background = Brushes.Red;
                //            runx.Foreground = Brushes.White;
                //            runx.FontWeight = FontWeights.Bold;
                //            textBlock.Inlines.Add(runx);
                //        }
                //        else
                //        {
                //            textBlock.Inlines.Add(item);
                //        }
                //    }
                //    return;
                //}
                //else if (itx is RichTextBox)
                //{
                //    TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
                //    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.White));
                //    textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                //    textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                //    if (SearchBox.Text.Length == 0)
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        TextPointer start = Hosts_RichTextBox.Document.ContentStart;

                //        int count = 0;
                //        while (start != null && start.CompareTo(Hosts_RichTextBox.Document.ContentEnd) < 0)
                //        {
                //            if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                //            {
                //                Match match = regex.Match(start.GetTextInRun(LogicalDirection.Forward));

                //                TextRange searchRange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
                //                searchRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));
                //                searchRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));
                //                searchRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                //                start = searchRange.End; // I'm not sure if this is correct or skips ahead too far, try it out!!!
                //            }
                //            start = start.GetNextContextPosition(LogicalDirection.Forward);
                //            //Console.WriteLine(count++);
                //        }
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                //    {
                //        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                //        Console.WriteLine(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                //        Console.WriteLine(i);
                //    }
                //}
            }
        }
    }
}
