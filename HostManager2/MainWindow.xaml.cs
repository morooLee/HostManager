﻿using HostManager.Controllers;
using HostManager.Models;
using HostManager.Views.EditHost;
using HostManager.Views.Menu;
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
        public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        private BrowserController browserController = new BrowserController();
        private TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
        private HostIOController hostIOController = new HostIOController();
        private TreeViewItemConverterController treeViewItemConverterController = new TreeViewItemConverterController();
        private HashSet<string> domainHashSet = new HashSet<string>();
        private Node SelectedNode = null;
        private bool isChangedHost = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 이벤트 함수

        // 메인윈도우 로드 이벤트
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree(null);
        }

        // 메인윈도우 키 이벤트
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SearchBox.Focus();
                }
            }
        }

        // 트리뷰 로드 이벤트
        private void Hosts_TreeView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // 트리뷰 SelectedItemChanged 이벤트
        private void Hosts_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Node selectedNode = e.NewValue as Node;
            SelectedNode = selectedNode;

            statusBarUpdate(selectedNode);
        }

        // 헤더 로드 이벤트
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

        // 헤더 텍스트 입력 이벤트
        private void HeaderText_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text.Length == 0)
            {
                tb.Text = "(비어있음)";
                tb.FontStyle = FontStyles.Italic;
            }
        }

        // RichTextBox TextChanged 이벤트
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

        // 체크박스 로드 이벤트
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();

            if (ckb.IsChecked == false)
            {
                sp.Opacity = 0.5;
                ckb.BorderThickness = new Thickness(1, 1, 1, 1);
                ckb.BorderBrush = (Brush)Conv.ConvertFromString("Black");
            }
            else
            {
                sp.Opacity = 1.0;
                ckb.BorderThickness = new Thickness(1, 1, 1, 1);
                ckb.BorderBrush = (Brush)Conv.ConvertFromString("Red");
            }
        }

        // 체크박스 체크 이벤트
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

        // 체크박스 체크 해제 이벤트
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

        // 체크박스 Null 이벤트
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

        // 체크박스 좌클릭 이벤트
        private void CheckBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            Node node = sp.DataContext as Node;

            // 좌클릭 시 트리뷰 선택하기
            node.IsSelected = true;
            isChangedHost = true;
        }

        // 적용 버튼 클릭 이벤트
        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            DoApply();
            browserController.CheckDoBrowsers();
        }

        // 새로고침 버튼 클릭 이벤트
        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            DoRefresh();
        }

        // 검색창 TextChanged 이벤트
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("Resources/Search.png", UriKind.Relative);
            bi.EndInit();
            SearchButtonImage.Source = bi;
            SearchButtonImage.Tag = "Search";
        }

        // 검색창 키 입력 이벤트
        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SearchBox.Text != "")
            {
                // Enter
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;

                    // ignore alt+space which invokes the system menu
                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        return;
                    }

                    SearchButtonImage.Tag = "Search";
                    SearchButton_Click(SearchButton, null);
                }
                // ESC
                else if ((e.Key == Key.Escape))
                {
                    e.Handled = true;

                    // ignore alt+space which invokes the system menu
                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        return;
                    }

                    SearchButtonImage.Tag = "Cancel";
                    SearchButton_Click(SearchButton, null);
                }
            }
        }

        // 검색창 Click 이벤트
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
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
                        count = HighlightText(Hosts_TreeView);
                    }
                    else
                    {
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
                    HighlightText(Hosts_TreeView);
                }
                else
                {
                    HighlightText(Hosts_RichTextBox);
                }

                ChangeInfoLabel(InfoLabelType.Info, "검색을 취소하였습니다.");
            }
        }

        // 트리뷰 아이템 더블클릭 이벤트
        private void TreeViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            EditTreeViewItem(NodeEditPosition.ThisNodeEdit, node, "호스트 수정");
        }

        // 트리뷰 아이템 키 입력 이벤트
        private void TreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Node node = item.DataContext as Node;

            // SPACE
            if (e.Key == Key.Space)
            {
                e.Handled = true;

                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    return;
                }

                if (item.IsSelected)
                {
                    if (e.OriginalSource == sender)
                    {
                        if (node.IsChecked == true)
                        {
                            node.IsChecked = false;
                        }
                        else
                        {
                            node.IsChecked = true;
                        }
                    }
                }
            }
            // ENTER
            else if (e.Key == Key.Enter && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                e.Handled = true;

                if (item.IsSelected)
                {
                    if (e.OriginalSource == sender)
                    {
                        if (node.IsChecked == true)
                        {
                            node.IsChecked = false;
                        }
                        else
                        {
                            node.IsChecked = true;
                        }
                    }
                }
            }
            // ESC
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;

                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    return;
                }

                if (SearchBox.Text != "")
                {
                    SearchButtonImage.Tag = "Cancel";
                    SearchButton_Click(SearchButton, null);
                }
            }
            // DEL
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                EditTreeViewItem(NodeEditPosition.ThisNodeDel, node, "");
            }
        }

        // 트리뷰 아이템 우클릭 이벤트
        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node != null)
            {
                node.IsSelected = true;
            }
        }

        // 메뉴 - 새로만들기
        private void Menu_File_Clear_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("모든 호스트 정보가 삭제됩니다.\r\n 그래도 새로 만드시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                treeViewItemModel.NodeList.Clear();
                Hosts_TreeView.ItemsSource = treeViewItemModel.NodeList;
                domainHashSet.Clear();
                Hosts_RichTextBox.Document.Blocks.Clear();

                ChangeInfoLabel(InfoLabelType.Success, "모든 호스트 정보가 삭제되었습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Warning, "새로 만들기를 취소하였습니다.");
            }
        }

        // 메뉴 - 열기
        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            bool IsFileOpen = false;

            if (isChangedHost)
            {
                MessageBoxResult result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n그래도 불러오시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    IsFileOpen = true;
                }
                else
                {
                    ChangeInfoLabel(InfoLabelType.Info, "불러오기를 취소하였습니다.");
                }
            }
            else
            {
                IsFileOpen = true;
            }

            if (IsFileOpen)
            {
                System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

                // Set filter for file extension and default file extension
                //dlg.DefaultExt = ".txt";
                dlg.Filter = "모든파일 (*.*)|*.*|텍스트 파일 (*.txt)|*.txt";

                // Display OpenFileDialog by calling ShowDialog method
                System.Windows.Forms.DialogResult dlgResult = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox
                if (dlgResult == System.Windows.Forms.DialogResult.OK)
                {
                    // Open document
                    if (Hosts_TreeView.IsEnabled != true)
                    {
                        ChangeButtonUI();
                    }
                    else
                    {
                        SearchBoxClear();
                    }

                    BindTree(hostIOController.HostLoad(dlg.FileName));
                    ChangeInfoLabel(InfoLabelType.Success, dlg.SafeFileName + "을 불러왔습니다.");
                }
                else
                {
                    SearchBoxClear();
                    ChangeInfoLabel(InfoLabelType.Info, "불러오기를 취소하였습니다.");
                }
            }
        }

        // 메뉴 - 저장하기
        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            DoApply();
        }

        // 메뉴 - 다른이름으로 저장하기
        private void Menu_File_AsSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "모든파일 (*.*)|*.*|텍스트 파일 (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            System.Windows.Forms.DialogResult dlgResult = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (dlgResult == System.Windows.Forms.DialogResult.OK)
            {
                // Open document
                //hostPath = dlg.FileName;
                if (Hosts_TreeView.IsEnabled != true)
                {
                    ChangeButtonUI();
                }
                else
                {
                    SearchBoxClear();
                }

                DoApply(dlg.FileName);
            }
            else
            {
                SearchBoxClear();
                ChangeInfoLabel(InfoLabelType.Info, "다른이름으로 저장하기를 취소하였습니다.");
            }
        }

        // 메뉴 - 프로그램 종료
        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            DoExit();
        }

        // 메뉴 - 속성창 열기
        private void Menu_Edit_Pref_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxClear();

            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Owner = Application.Current.MainWindow;
            settingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bool? result = settingWindow.ShowDialog();

            if (result == true)
            {
                ChangeInfoLabel(InfoLabelType.Success, "저장되었습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Info, "취소되었습니다.");
            }
        }

        // 메뉴 - About창 열기
        private void Menu_Help_About_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxClear();

            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            aboutWindow.ShowDialog();
        }

        #endregion

        #region 기능 함수

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

            isChangedHost = false;

            this.Dispatcher.BeginInvoke(
                (ThreadStart)(() => { treeViewItemModel.AllISExpanded(true); }),
                DispatcherPriority.ApplicationIdle);

            this.Dispatcher.BeginInvoke(
                (ThreadStart)(() => { treeViewItemModel.AllISExpanded(false); }),
                DispatcherPriority.ApplicationIdle);

            LoadingImagePlay(false);
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

        // 트리 - 텍스트 전환 시 버튼 UI 변경하기
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
        private void OpenNotepad(object sender, RoutedEventArgs e)
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
            ChangeInfoLabel(InfoLabelType.Info, "메모장에서 수정한 것을 반영하려면 새로고침을 누르세요.");
        }

        // 새로고침
        private void DoRefresh()
        {
            SearchBoxClear();
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

                ChangeInfoLabel(InfoLabelType.Success, "호스트 정보를 새로 불러들였습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Info, "새로고침을 취소하였습니다.");
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
                ChangeInfoLabel(InfoLabelType.Warning, "호스트가 저장되었습니다.");
            }

            treeViewItemModel.SetIsChangedAll(false);
            isChangedHost = false;
        }

        // 다른이름으로 저장하기
        private void DoApply(string path)
        {
            if (treeViewItemConverterController.ConverterToString(treeViewItemModel, path, true) == null)
            {
                MessageBox.Show("호스트 적용에 실패하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeInfoLabel(InfoLabelType.Warning, "호스트 적용에 실패하였습니다.");
            }
            else
            {
                ChangeInfoLabel(InfoLabelType.Warning, "호스트가 저장되었습니다.");
            }

            treeViewItemModel.SetIsChangedAll(false);
            isChangedHost = false;
        }


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

        // 루트에 트리 추가
        private void Root_Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            MenuItem menuItem = e.OriginalSource as MenuItem;
            
            if (menuItem.Name == "")
            {
                EditTreeViewItem(NodeEditPosition.RootNodeAdd, null, menuItem.Header.ToString());
            }

            else
            {
                EditTreeViewItem(NodeEditPosition.RootNodeAdd, null, menuItem.Name);
            }
        }

        // 기본 폴더 열기
        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            hostIOController.OpenFolder();
        }

        //모두 접기
        private void AllCollapsed(object sender, RoutedEventArgs e)
        {
            treeViewItemModel.AllISExpanded(false);
            ChangeInfoLabel(InfoLabelType.Info, "카테고리를 모두 닫았습니다.");
        }

        // 모두 열기
        private void AllExpanded(object sender, RoutedEventArgs e)
        {
            treeViewItemModel.AllISExpanded(true);
            ChangeInfoLabel(InfoLabelType.Info, "카테고리를 모두 확장하였습니다.");
        }

        // 자식 트리에 추가
        private void Add_Child_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = (menuItem.DataContext as Node);

            EditTreeViewItem(NodeEditPosition.ChildNodeAdd, node, menuItem.Name);
        }

        // 트리 추가
        private void Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            EditTreeViewItem(NodeEditPosition.SameNodeAdd, node, menuItem.Name);
        }

        // 트리 수정
        private void Edit_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node == null)
            {
                node = SelectedNode;
            }

            EditTreeViewItem(NodeEditPosition.ThisNodeEdit, node, menuItem.Name);
        }

        // 트리 삭제
        private void Del_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            EditTreeViewItem(NodeEditPosition.ThisNodeDel, node, "");
        }

        //트리 위로 이동
        private void MoveToUp_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

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

                isChangedHost = true;
            }
        }

        // 트리 아래로 이동
        private void MoveToDown_TreeViewItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

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

                isChangedHost = true;
            }
        }

        // CheckBox 전체 해제
        private void AllUnchecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            foreach (Node node in treeViewItemModel.NodeList)
            {
                node.IsChecked = false;
            }

            ChangeInfoLabel(InfoLabelType.Info, "호스트가 모두 해제되었습니다.");
        }

        // 트리 형식으로 전환
        private void ViewByTreeView(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TextRange textRange = new TextRange(Hosts_RichTextBox.Document.ContentStart, Hosts_RichTextBox.Document.ContentEnd);
            BindTree(textRange.Text);

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

            ChangeInfoLabel(InfoLabelType.Info, "트리 형식으로 전환합니다.");
            ChangeButtonUI();
        }

        // 텍스트 형식으로 전환
        private void ViewByRichTextBox(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Hosts_RichTextBox.Document.Blocks.Clear();
            Hosts_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewItemConverterController.ConverterToString(treeViewItemModel, null, false))));

            if (SearchBox.Text.Length > 0)
            {
                SearchBoxClear();
            }

            ChangeInfoLabel(InfoLabelType.Info, "텍스트 형식으로 전환합니다.");
            ChangeButtonUI();
        }

        // Edit창 띄워서 호스트 수정하기
        private void EditTreeViewItem(NodeEditPosition nodeEditPosition, Node node, string windowTitle)
        {
            if (nodeEditPosition == NodeEditPosition.ThisNodeDel)
            {
                node.ParentNode.IsChanged = true;
                domainHashSet.Remove(node.Domain);

                if (node.ParentNode == null)
                {
                    treeViewItemModel.NodeList.Remove(node);
                }
                else
                {
                    node.ParentNode.NodeList.Remove(node);
                }

                isChangedHost = true;
                ChangeInfoLabel(InfoLabelType.Success, "호스트가 삭제 되었습니다.");
            }
            else
            {
                if (node == null && nodeEditPosition != NodeEditPosition.RootNodeAdd)
                {
                    MessageBox.Show("선택된 호스트가 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel(InfoLabelType.Warning, "선택된 호스트가 없습니다.");
                }
                else
                {
                    EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(node, windowTitle);
                    editTreeViewWindow.Owner = Application.Current.MainWindow;
                    editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editTreeViewWindow.ShowDialog();

                    if (editTreeViewWindow.treeViewItemModel != null && editTreeViewWindow.treeViewItemModel.NodeList.Count != 0)
                    {
                        editTreeViewWindow.treeViewItemModel.SetIsChangedAll(true);

                        switch (nodeEditPosition)
                        {
                            case NodeEditPosition.ChildNodeAdd:
                                {
                                    if (node.IsSelected)
                                    {
                                        foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                                        {
                                            item.ParentNode = node;
                                            item.IsChanged = true;
                                            item.IsChecked = true;
                                            item.IsExpanded = true;
                                            node.NodeList.Add(item);
                                        }

                                        ChangeInfoLabel(InfoLabelType.Warning, "호스트가 추가되었습니다.");
                                        node.IsExpanded = true;
                                    }

                                    break;
                                }
                            case NodeEditPosition.ThisNodeEdit:
                                {
                                    if (node.IsSelected)
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

                                        domainHashSet.Remove(node.Domain);

                                        if (node.ParentNode == null)
                                        {
                                            treeViewItemModel.NodeList.RemoveAt(treeViewItemModel.NodeList.IndexOf(node));
                                        }
                                        else
                                        {
                                            node.ParentNode.NodeList.RemoveAt(node.ParentNode.NodeList.IndexOf(node));
                                        }

                                        ChangeInfoLabel(InfoLabelType.Warning, "호스트를 수정하였습니다.");
                                    }

                                    break;
                                }
                            case NodeEditPosition.SameNodeAdd:
                                {
                                    if (node.IsSelected)
                                    {
                                        foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                                        {
                                            item.ParentNode = node.ParentNode;
                                            item.IsChanged = true;
                                            item.IsChecked = true;
                                            item.IsExpanded = true;

                                            if (node.ParentNode == null)
                                            {

                                                treeViewItemModel.NodeList.Add(item);
                                            }
                                            else
                                            {
                                                node.ParentNode.NodeList.Add(item);
                                            }
                                        }

                                        ChangeInfoLabel(InfoLabelType.Warning, "호스트가 추가되었습니다.");
                                    }

                                    break;
                                }
                            case NodeEditPosition.RootNodeAdd:
                                {
                                    foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                                    {
                                        treeViewItemModel.NodeList.Add(item);
                                    }

                                    ChangeInfoLabel(InfoLabelType.Warning, "호스트가 추가되었습니다..");

                                    break;
                                }
                        }

                        foreach (Node item in editTreeViewWindow.treeViewItemModel.NodeList)
                        {
                            DomainDuplication(item);
                        }

                        isChangedHost = true;
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

        private void DoExit()
        {
            if (isChangedHost)
            {
                MessageBoxResult result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n그래도 종료하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    notifyIcon.Visible = false;
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                else
                {
                    ChangeInfoLabel(InfoLabelType.Info, "프로그램 종료를 취소하였습니다.");
                }
            }
            else
            {
                notifyIcon.Visible = false;
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        #endregion
    }
}