using HostManager.Controllers;
using HostManager.Models;
using HostManager.Views.EditHost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace HostManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
        HostIOController hostIOController = new HostIOController();
        TreeViewItemConverterController treeViewItemConverterController = new TreeViewItemConverterController();
        HashSet<string> domainHashSet = new HashSet<string>();
        public Node SelectedNode = null;
        bool isChangedHost = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            // 이벤트 발생 시 초기화
            base.OnInitialized(e);

            // 진행률 전송 여부
            backgroundWorker.WorkerReportsProgress = true;

            // 작업 취소 여부
            backgroundWorker.WorkerSupportsCancellation = true;

            // 작업 쓰레드 
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);

            // 진행률 변경
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);

            // 작업 완료
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadingImagePlay(true);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingImagePlay(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree(null);
        }

        #region TreeView 이벤트 및 관련 함수

        // 트리뷰 로드 이벤트
        private void Hosts_TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            //BindTree(null);
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
            tb.Name = "TextBlock_" + tb.GetHashCode();
            Hosts_TreeView.RegisterName(tb.Name, tb);
            Node node = tb.DataContext as Node;

            if (node.TextBlockName == null)
            {
                node.TextBlockName = tb.Name;
            }

            if (tb.Text.Length == 0)
            {
                tb.Text = "(비어있음)";
                tb.FontStyle = FontStyles.Italic;
            }
        }

        private void HeaderText_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text.Length == 0)
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
            LoadingImagePlay(true);

            bool isConverted = true;

            if (hosts == null)
            {
                try
                {
                    treeViewItemModel = treeViewItemConverterController.ConverterToNodeList(null);
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
                    treeViewItemModel = treeViewItemConverterController.ConverterToNodeList(hosts);
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

            this.Dispatcher.BeginInvoke(
                (ThreadStart)(() => { treeViewItemModel.AllISExpanded(true); }),
                DispatcherPriority.ApplicationIdle);

            this.Dispatcher.BeginInvoke(
                (ThreadStart)(() => { treeViewItemModel.AllISExpanded(false); }),
                DispatcherPriority.ApplicationIdle);

            LoadingImagePlay(false);
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
        }

        #endregion

        #region 도구모음 이벤트 및 관련 함수

        private void TreeView_Button_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
            BindTree(textRange.Text);

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

            ChangeButtonUI();
        }

        private void RichTextBox_Button_Click(object sender, RoutedEventArgs e)
        {
            Hosts_RichTextBox.Document.Blocks.Clear();
            Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewItemConverterController.ConverterToString(treeViewItemModel, null, false))));

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

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
            //LoadingImagePlay(true);

            if (SearchButtonImage.Tag.ToString() == "Search")
            {
                if (SearchBox.Text.Length == 0)
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

                    int count = 0;

                    if (Hosts_TreeView.Visibility == Visibility.Visible)
                    {
                        //Dispatcher.BeginInvoke(
                        //    (ThreadStart)(() => { count = HighlightText(Hosts_TreeView);
                        //        if (count == 0)
                        //        {
                        //            ChangeInfoLabel(InfoLabelType.Info, SearchBox.Text + "에 대한 검색 결과가 없습니다.");
                        //        }
                        //        else
                        //        {
                        //            ChangeInfoLabel(InfoLabelType.Info, count + "개 검색되었습니다.");
                        //        }
                        //    }),
                        //    DispatcherPriority.ApplicationIdle);
                        count = HighlightText(Hosts_TreeView);
                    }
                    else
                    {
                        //Dispatcher.BeginInvoke(
                        //    (ThreadStart)(() => {
                        //        count = HighlightText(Hosts_RichTextBox);
                        //        if (count == 0)
                        //        {
                        //            ChangeInfoLabel(InfoLabelType.Info, SearchBox.Text + "에 대한 검색 결과가 없습니다.");
                        //        }
                        //        else
                        //        {
                        //            ChangeInfoLabel(InfoLabelType.Info, count + "개 검색되었습니다.");
                        //        }
                        //    }),
                        //    DispatcherPriority.ApplicationIdle);
                        count = HighlightText(Hosts_RichTextBox);
                    }

                    if (count == 0)
                    {
                        ChangeInfoLabel(InfoLabelType.Info, SearchBox.Text + "에 대한 검색 결과가 없습니다.");
                    }
                    else
                    {
                        ChangeInfoLabel(InfoLabelType.Info, count + "개 검색되었습니다.");
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
                    //Dispatcher.BeginInvoke(
                    //    (ThreadStart)(() => {
                    //        HighlightText(Hosts_TreeView);
                    //        ChangeInfoLabel(InfoLabelType.Info, "검색을 취소하였습니다.");
                    //    }),
                    //    DispatcherPriority.ApplicationIdle);
                    HighlightText(Hosts_TreeView);
                }
                else
                {
                    //Dispatcher.BeginInvoke(
                    //    (ThreadStart)(() => {
                    //        HighlightText(Hosts_RichTextBox);
                    //        ChangeInfoLabel(InfoLabelType.Info, "검색을 취소하였습니다.");
                    //    }),
                    //    DispatcherPriority.ApplicationIdle);
                    HighlightText(Hosts_RichTextBox);
                }

                ChangeInfoLabel(InfoLabelType.Info, "검색을 취소하였습니다.");
            }

            //LoadingImagePlay(false);
        }

        // SearchBox 초기화
        private void SearchBoxClear()
        {
            if (SearchButtonImage.Tag.ToString() == "Cancel")
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
                    Hosts_TreeView.Focus();
                }
                else
                {
                    Hosts_RichTextBox.Focus();
                }

                ChangeInfoLabel(InfoLabelType.None, null);
            }
        }

        /// <summary>
        /// 하이라이트 설정하기
        /// </summary>
        /// <param name="itx">Control (TreeView / Node / RichTextBox)</param>
        /// <returns>하이라이트가 적용된 수</returns>
        private int HighlightText(object itx)
        {
            int count = 0;
            if (itx != null)
            {
                Regex regex = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);

                if (itx is TreeView)
                {

                    TreeView treeView = itx as TreeView;

                    foreach (Node node in treeView.Items)
                    {
                        count += HighlightText(node);
                    }
                }
                else if (itx is Node)
                {
                    int matchCount = 0;
                    Node node = itx as Node;

                    TextBlock textBlock = (TextBlock)this.Hosts_TreeView.FindName(node.TextBlockName);

                    if (SearchBox.Text.Length == 0)
                    {
                        string str = textBlock.Text;
                        textBlock.Inlines.Clear();
                        textBlock.Inlines.Add(str);

                        treeViewItemModel.AllISExpanded(false);
                    }
                    else
                    {
                        string[] substrings = substrings = regex.Split(textBlock.Text);
                        textBlock.Inlines.Clear();
                        foreach (string item in substrings)
                        {
                            if (regex.Match(item).Success)
                            {
                                Run runx = new Run(item);

                                runx.Background = Brushes.Red;
                                runx.Foreground = Brushes.White;
                                runx.FontWeight = FontWeights.Bold;
                                textBlock.Inlines.Add(runx);

                                treeViewItemModel.ParentIsExpanded(node.ParentNode);
                                matchCount++;
                            }
                            else
                            {
                                textBlock.Inlines.Add(item);
                            }
                        }
                    }

                    if (node.IsExternalNode == false)
                    {
                        foreach (Node childNode in node.NodeList)
                        {
                            matchCount += HighlightText(childNode);
                        }
                    }
                    return matchCount;
                }
                else if (itx is RichTextBox)
                {
                    TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
                    string tmpText = textRange.Text;

                    Hosts_RichTextBox.Document.Blocks.Clear();
                    Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(tmpText)));

                    if (SearchBox.Text.Length == 0)
                    {
                        return count;
                    }

                    for (TextPointer position = Hosts_RichTextBox.Document.ContentStart; position != null && position.CompareTo(Hosts_RichTextBox.Document.ContentEnd) <= 0; position = position.GetNextContextPosition(LogicalDirection.Forward))
                    {
                        if (position.CompareTo(Hosts_RichTextBox.Document.ContentEnd) == 0)
                        {
                            return count;
                        }

                        string textRun = position.GetTextInRun(LogicalDirection.Forward);
                        StringComparison stringComparison = StringComparison.CurrentCulture;
                        int indexInRun = textRun.IndexOf(SearchBox.Text, stringComparison);
                        if (indexInRun > 0)
                        {
                            position = position.GetPositionAtOffset(indexInRun);
                            if (position != null)
                            {
                                TextPointer nextPointer = position.GetPositionAtOffset(SearchBox.Text.Length);
                                textRange = new TextRange(position, nextPointer);
                                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));
                                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));
                                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

                                count++;
                            }
                        }
                    }
                }
            }
            return count;
        }

        // 버튼 UI 변경하기
        private void ChangeButtonUI()
        {
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
                        if (node.Header.Length == 0)
                        {
                            path = "Empty";
                        }
                        else
                        {
                            path = node.Header;
                        }

                        if (sbText.Length == 0)
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


        /// <summary>
        /// InfoLabel 설정
        /// </summary>
        /// <param name="type">정보 종류 (Warning / Success / Info / None)</param>
        /// <param name="msg">메시지</param>
        private void ChangeInfoLabel(InfoLabelType type, string msg)
        {
            string BackgroundColor = "";
            string ForegroundColor = "";
            BrushConverter Conv = new BrushConverter();

            switch (type)
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


        /// <summary>
        /// 로딩화면 출력
        /// </summary>
        /// <param name="isLoaded">로딩화면 출력 여부</param>
        private void LoadingImagePlay(bool isLoaded)
        {
            this.Dispatcher.BeginInvoke(
                (ThreadStart)(() =>
                {
                    if (isLoaded)
                    {
                        MainPanel.IsEnabled = false;
                        MainPanel.Opacity = 0.5;
                        Hosts_TreeView.Visibility = Visibility.Hidden;
                        GIFCtrl.Visibility = Visibility.Visible;
                        GIFCtrl.StartAnimate();
                    }
                    else
                    {
                        GIFCtrl.StopAnimate();
                        GIFCtrl.Visibility = Visibility.Hidden;
                        Hosts_TreeView.Visibility = Visibility.Visible;
                        MainPanel.IsEnabled = true;
                        MainPanel.Opacity = 1.0;
                    }
                }),
                DispatcherPriority.ApplicationIdle);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node.IsExternalNode && node.IsSelected)
            {
                EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(node, "선택한 항목 수정");
                editTreeViewWindow.Owner = Application.Current.MainWindow;
                editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                editTreeViewWindow.ShowDialog();

                if (editTreeViewWindow.treeViewItemModel != null && editTreeViewWindow.treeViewItemModel.NodeList.Count != 0)
                {
                    foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                    {
                        item.ParentNode = node.ParentNode;
                        item.IsChanged = true;
                        item.IsChecked = true;
                        item.IsExpanded = true;

                        if (node.ParentNode == null)
                        {
                            treeViewItemModel.NodeList.Insert(treeViewItemModel.NodeList.IndexOf(node), item);
                        }
                        else
                        {
                            node.ParentNode.NodeList.Insert(node.ParentNode.NodeList.IndexOf(node), item);
                        }
                    }

                    if (node.ParentNode == null)
                    {
                        treeViewItemModel.NodeList.RemoveAt(treeViewItemModel.NodeList.IndexOf(node));
                    }
                    else
                    {
                        node.ParentNode.NodeList.RemoveAt(node.ParentNode.NodeList.IndexOf(node));
                        node.ParentNode.IsSelected = false;
                    }

                    ChangeInfoLabel(InfoLabelType.Success, "선택한 항목이 수정되었습니다.");
                }
                else
                {
                    ChangeInfoLabel(InfoLabelType.Warning, "작업이 취소되었습니다.");
                }

                editTreeViewWindow.treeViewItemModel = null;
                editTreeViewWindow.Close();
            }
        }

        private void TreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node != null)
            {
                node.IsSelected = true;
            }
        }

        private void Root_Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;

            EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(null, menuItem.Header.ToString());
            editTreeViewWindow.Owner = Application.Current.MainWindow;
            editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewWindow.ShowDialog();

            if (editTreeViewWindow.treeViewItemModel != null && editTreeViewWindow.treeViewItemModel.NodeList.Count != 0)
            {
                editTreeViewWindow.treeViewItemModel.NodeList.Reverse();

                foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                {
                    item.ParentNode = null;
                    item.IsChanged = true;
                    item.IsChecked = true;
                    item.IsExpanded = true;

                    treeViewItemModel.NodeList.Add(item);
                }

                ChangeInfoLabel(InfoLabelType.Success, "호스트가 추가되었습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Warning, "작업이 취소되었습니다.");
            }

            editTreeViewWindow.treeViewItemModel = null;
            editTreeViewWindow.Close();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            hostIOController.OpenFolder();
        }

        private void TreeView_Notpad_Item_Click(object sender, RoutedEventArgs e)
        {
            OpenNotepad();
        }

        private void TextEdit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Hosts_RichTextBox.Document.Blocks.Clear();
            Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewItemConverterController.ConverterToString(treeViewItemModel, null, false))));

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

            ChangeButtonUI();
        }

        //여기까지
        private void AllCollapsed(object sender, RoutedEventArgs e)
        {
            treeViewItemModel.AllISExpanded(false);
            ChangeInfoLabel(InfoLabelType.Info, "카테고리를 모두 닫았습니다.");
        }

        private void AllExpanded(object sender, RoutedEventArgs e)
        {
            treeViewItemModel.AllISExpanded(true);
            ChangeInfoLabel(InfoLabelType.Info, "카테고리를 모두 확장하였습니다.");
        }

        private void Add_Child_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node parentNode = (menuItem.DataContext as Node);
            Node childNode = new Node();

            if (parentNode == null)
            {
                MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
            }
            else
            {
                EditTreeViewItem(childNode, menuItem.Name);
                childNode.ParentNode = parentNode;
                parentNode.NodeList.Insert(0, childNode);
            }
        }

        private void Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node childNode = menuItem.DataContext as Node;
            Node parentNode = null;

            if (childNode != null && childNode.ParentNode != null)
            {
                parentNode = childNode.ParentNode;
            }

            if (parentNode == null)
            {
                MessageBox.Show("호스트를 추가하지 못했습니다.\r\n다시 시도해 주십시오.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "호스트를 추가하지 못했습니다.");
            }
            else
            {
                EditTreeViewItem(parentNode, menuItem.Name);
            }
        }

        private void Edit_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node == null)
            {
                MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
            }
            else
            {
                EditTreeViewItem(node, menuItem.Name);
            }
        }

        private void Del_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node == null)
            {
                MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
            }
            else
            {
                if (node.ParentNode == null)
                {
                    treeViewItemModel.NodeList.Remove(node);
                }
                else
                {
                    node.ParentNode.NodeList.Remove(node);
                }
            }
        }

        private void MoveToUp_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node == null)
            {
                MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
            }
            else
            {
                if (node.ParentNode == null)
                {
                    if (treeViewItemModel.NodeList.IndexOf(node) > 0)
                    {
                        treeViewItemModel.NodeList.Move(treeViewItemModel.NodeList.IndexOf(node), treeViewItemModel.NodeList.IndexOf(node) - 1);
                    }
                    else
                    {
                        MessageBox.Show("이미 최상위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChangeInfoLabel(InfoLabelType.Warning, "더이상 위로 이동할 수 없습니다.");
                    }
                }
                else
                {
                    if (node.ParentNode.NodeList.IndexOf(node) > 0)
                    {
                        node.ParentNode.NodeList.Move(node.ParentNode.NodeList.IndexOf(node), node.ParentNode.NodeList.IndexOf(node) - 1);
                    }
                    else
                    {
                        MessageBox.Show("이미 최상위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChangeInfoLabel(InfoLabelType.Warning, "더이상 위로 이동할 수 없습니다.");
                    }
                }
            }
        }

        private void MoveToDown_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node == null)
            {
                MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
            }
            else
            {
                if (node.ParentNode == null)
                {
                    if (treeViewItemModel.NodeList.IndexOf(node) < (treeViewItemModel.NodeList.Count - 1))
                    {
                        treeViewItemModel.NodeList.Move(treeViewItemModel.NodeList.IndexOf(node), treeViewItemModel.NodeList.IndexOf(node) + 1);
                    }
                    else
                    {
                        MessageBox.Show("이미 최하위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChangeInfoLabel(InfoLabelType.Warning, "더이상 아래로 이동할 수 없습니다.");
                    }
                }
                else
                {
                    if (node.ParentNode.NodeList.IndexOf(node) < (node.ParentNode.NodeList.Count - 1))
                    {
                        node.ParentNode.NodeList.Move(node.ParentNode.NodeList.IndexOf(node), node.ParentNode.NodeList.IndexOf(node) + 1);
                    }
                    else
                    {
                        MessageBox.Show("이미 최하위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChangeInfoLabel(InfoLabelType.Warning, "더이상 아래로 이동할 수 없습니다.");
                    }
                }
            }
        }

        private void AllUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (Node node in treeViewItemModel.NodeList)
            {
                node.IsChecked = false;
            }
            ChangeInfoLabel(InfoLabelType.Info, "호스트가 모두 체크되었습니다.");
        }

        private void TextBox_Notepad_Item_Click(object sender, RoutedEventArgs e)
        {
            OpenNotepad();
        }

        private void TreeView_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewByTreeView();
        }

        private void ViewByTreeView()
        {
            TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
            BindTree(textRange.Text);

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

            ChangeButtonUI();
        }

        private void EditTreeViewItem(Node node, string windowTitle)
        {
            if (node != null)
            {
                EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(node, windowTitle);
                editTreeViewWindow.Owner = Application.Current.MainWindow;
                editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                editTreeViewWindow.ShowDialog();

                if (editTreeViewWindow.treeViewItemModel != null && editTreeViewWindow.treeViewItemModel.NodeList.Count != 0)
                {
                    foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                    {
                        item.ParentNode = node.ParentNode;
                        item.IsChanged = true;
                        item.IsChecked = true;
                        item.IsExpanded = true;

                        if (node.ParentNode == null)
                        {
                            treeViewItemModel.NodeList.Insert(treeViewItemModel.NodeList.IndexOf(node), item);
                        }
                        else
                        {
                            node.ParentNode.NodeList.Insert(node.ParentNode.NodeList.IndexOf(node), item);
                        }
                    }

                    if (node.ParentNode == null)
                    {
                        treeViewItemModel.NodeList.RemoveAt(treeViewItemModel.NodeList.IndexOf(node));
                    }
                    else
                    {
                        node.ParentNode.NodeList.RemoveAt(node.ParentNode.NodeList.IndexOf(node));
                        node.ParentNode.IsSelected = false;
                    }

                    isChangedHost = true;

                    ChangeInfoLabel(InfoLabelType.Success, "선택한 항목이 수정되었습니다.");
                }
                else
                {
                    ChangeInfoLabel(InfoLabelType.Warning, "작업이 취소되었습니다.");
                }


                editTreeViewWindow.treeViewItemModel = null;
                editTreeViewWindow.Close();
            }
        }
    }
}