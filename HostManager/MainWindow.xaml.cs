using HostManager.Controllers;
using HostManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private TreeViewModelController treeViewModelController = new TreeViewModelController();
        private HostIOController hostIOController = new HostIOController();
        private BrowserController browserController = new BrowserController();
        private TreeViewModel treeViewModel = new TreeViewModel();
        private List<String> headerIsMatchedList = new List<String>();
        public Node nodeCopy = null;

        public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            //Text_RichTextBox.Document.PageWidth = 1000;
            //SubWindow subWindow = new SubWindow();
            //subWindow.Show();

            System.Windows.Forms.ContextMenu trayContextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem trayMenuItem1 = new System.Windows.Forms.MenuItem();
            System.Windows.Forms.MenuItem trayMenuItem2 = new System.Windows.Forms.MenuItem();

            trayMenuItem1.Index = 0;
            trayMenuItem1.Text = "프로그램 열기";
            trayMenuItem1.Click += delegate (object click, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            trayMenuItem2.Index = 1;
            trayMenuItem2.Text = "프로그램 닫기";
            trayMenuItem2.Click += delegate (object click, EventArgs e)
            {
                notifyIcon.Visible = false;
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            };

            trayContextMenu.MenuItems.Add(trayMenuItem1);
            trayContextMenu.MenuItems.Add(trayMenuItem2);

            notifyIcon.Icon = Properties.Resources.Sign_Icon;
            notifyIcon.Text = "Moroo | Host Manager";
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu = trayContextMenu;
            notifyIcon.DoubleClick += delegate (object senders, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree();
            //BindTree(false, false);
        }

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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();
            Node node = sp.DataContext as Node;

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Red");

            if (node.IsSelected)
            {
                HeaderIsOverlap(node, true);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();
            Node node = sp.DataContext as Node;

            sp.Opacity = 0.5;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Black");

            if (node.IsSelected)
            {
                HeaderIsOverlap(node, false);
            }
        }

        private void HeaderIsOverlap(Node node, bool isChecked)
        {
            if (node.IsLastNode)
            {
                if (isChecked)
                {
                    headerIsMatchedList.Add(node.Domain);

                    if (headerIsMatchedList.Count(x => x == node.Domain) > 1)
                    {
                        MessageBox.Show("도메인이 중복으로 적용되어 체크할 수 없습니다.\r\n검색을 통해 적용되어 있는 도메인을 찾으세요.\r\n\r\n도메인명 : " + node.Domain, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChangeInfoLabel("Info", "중복되는 도메인명 : " + node.Domain, null);
                        node.IsChecked = false;
                        if (node.ParentNode.IsExpanded)
                        {
                            node.ParentNode.IsExpanded = true;
                        }
                    }
                    else
                    {
                        ChangeInfoLabel("None", "", true);
                    }
                }
                else
                {
                    if (headerIsMatchedList.Contains(node.Domain))
                    {
                        headerIsMatchedList.RemoveAt(headerIsMatchedList.LastIndexOf(node.Domain));
                    }

                    ChangeInfoLabel("None", "", true);
                }
            }
            else
            {
                if (node.NodeList == null || node.NodeList.Count == 0)
                {
                    ChangeInfoLabel("None", "", true);
                }
                else
                {
                    foreach (Node item in node.NodeList)
                    {
                        HeaderIsOverlap(item, isChecked);
                    }
                }
            }
        }

        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Red");
        }

        private void CheckBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            Node node = sp.DataContext as Node;

            node.IsSelected = true;
        }

        private void TreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Node node = item.DataContext as Node;

            if (e.Key == Key.Space)
            {
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
            else if (e.Key == Key.Enter && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
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
            else if (e.Key == Key.Escape)
            {
                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    return;
                }
                                
                if (SearchBox.Text != "" && SearchBox.Text != null)
                {
                    SearchButtonImage.Tag = "Cancel";
                    SearchButton_Click(SearchButton, null);
                }
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node.IsLastNode && node.IsSelected)
            {
                EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(node, "선택한 항목 수정");
                editTreeViewWindow.Owner = Application.Current.MainWindow;
                editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                editTreeViewWindow.ShowDialog();

                if (editTreeViewWindow.treeViewModel != null && editTreeViewWindow.treeViewModel.NodeList.Count != 0)
                {
                    node = editTreeViewWindow.treeViewModel.NodeList.ElementAt(0);
                    ChangeInfoLabel("Success", "선택한 항목이 수정되었습니다.", null);
                }
                else
                {
                    ChangeInfoLabel("Warning", "작업이 취소되었습니다.", null);
                }

                editTreeViewWindow.treeViewModel = null;
                editTreeViewWindow.Close();
            }
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

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node.IsLastNode == false)
            {
                ChangeInfoLabel("None", "", null);
            }
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Node node = treeViewItem.DataContext as Node;

            if (node.IsLastNode == false)
            {
                ChangeInfoLabel("None", "", null);
            }
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            DoApply();
            browserController.CheckDoBrowsers();
        }

        private bool DoApply()
        {
            bool applyCheck = false;

            if (HostsTreeView.Visibility == Visibility.Visible)
            {
                applyCheck = hostIOController.HostSave(treeViewModel);

                if (applyCheck)
                {
                    treeViewModel.IsChangedCancel();
                }
            }
            else
            {
                TextRange textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
                applyCheck = hostIOController.HostSave(textRange.Text);
            }

            if (applyCheck)
            {
                if (InfoLabel.Content.ToString() == "")
                {
                    ChangeInfoLabel("Success", "호스트에 적용되었습니다.", true);
                }
            }
            else
            {
                if (InfoLabel.Content.ToString() == "")
                {
                    ChangeInfoLabel("Warning", "호스트 적용에 실패하였습니다.", true);
                }
            }

            ChangeApplyButtonUI(false);
            return applyCheck;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchButtonImage.Tag.ToString() == "Search")
            {
                if (SearchBox.Text == "")
                {
                    ChangeInfoLabel("Warning", "검색어를 입력하세요.", null);
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

                    if (HostsTreeView.Visibility == Visibility.Visible)
                    {
                        for (int i = 0; i < treeViewModel.NodeList.Count; i++)
                        {
                            Count += treeViewModel.NodeList[i].Search(SearchBox.Text);
                        }

                        if (Count == 0)
                        {
                            ChangeInfoLabel("Info", SearchBox.Text + "에 대한 검색 결과가 없습니다.", null);
                        }
                        else
                        {
                            FindTreeViewItem(HostsTreeView);
                            ChangeInfoLabel("Info", Count + "개 검색되었습니다.", null);
                        }
                    }
                    else
                    {
                        Regex rg = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);
                        TextRange textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
                        MatchCollection matches = rg.Matches(textRange.Text);
                        Count = matches.Count;

                        if (Count == 0)
                        {
                            ChangeInfoLabel("Info", SearchBox.Text + "에 대한 검색 결과가 없습니다.", null);
                        }
                        else
                        {
                            FindRichTextBox(Text_RichTextBox);
                            ChangeInfoLabel("Info", Count + "개 검색되었습니다.", null);
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

                if (HostsTreeView.Visibility == Visibility.Visible)
                {
                    FindTreeViewItem(HostsTreeView);
                }
                else
                {
                    FindRichTextBox(Text_RichTextBox);
                }

                ChangeInfoLabel("Info", "검색을 취소하였습니다.", null);
            }
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

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text == "")
            {
                tb.Text = "카테고리명이 없습니다.";
                tb.FontStyle = FontStyles.Italic;
            }
        }

        private void TextBlock_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text == "")
            {
                tb.Text = "카테고리명이 없습니다.";
                tb.FontStyle = FontStyles.Italic;
            }
            else
            {
                tb.FontStyle = FontStyles.Normal;
            }
        }

        private void HostsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ChangeInfoLabel("None", "", null);

            #region 상태표시줄에 전체 경로 출력하기
            Node item = e.NewValue as Node;
            String sbText = "";

            if (item != null)
            {
                if (item.ParentNode != null)
                {
                    List<Node> nodeList = new List<Node>();
                    Node node = item.ParentNode;
                    while (true)
                    {
                        nodeList.Add(node);
                        
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

                    nodeList.Reverse();

                    foreach (Node nodeItem in nodeList)
                    {
                        if (nodeItem.Header == "")
                        {
                            nodeItem.Header = "Empty";
                        }
                        if (sbText == "")
                        {
                            sbText += nodeItem.Header;
                        }
                        else
                        {
                            sbText += " > " + nodeItem.Header;
                        }
                    }
                }
                else
                {
                    sbText += item.Header;
                }
            }

            statusBar.Items.Clear();
            statusBar.Items.Add(sbText);
            #endregion
        }

        private void BindTree()
        {
            treeViewModel = hostIOController.HostLoad();

            if (treeViewModel == null)
            {
                TextToTreeView(hostIOController.HostToString());
            }
            else
            {
                headerIsMatchedList = treeViewModel.DomainList();
                HostsTreeView.ItemsSource = treeViewModel.NodeList;
                treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (treeViewModel.Pass == false)
                {
                    MessageBox.Show("중복으로 적용된 도메인이 있어 모든 항목들의 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 모든 체크를 해제하였습니다.", true);
                }

                if (treeViewModel.NodeList.Count == 0)
                {
                    ChangeInfoLabel("Info", "호스트 내용이 없습니다.", null);
                }
            }
        }

        private void BindTree(bool isTreeView, bool isTextBox)
        {
            TextRange textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);

            if (isTreeView == false && isTextBox == false)
            {
                treeViewModel = hostIOController.HostLoad();
            }
            else if (isTreeView == true && isTextBox == false)
            {
                Text_RichTextBox.Document.Blocks.Clear();
                Text_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewModelController.ConverterToString(treeViewModel))));
            }
            else if (isTreeView == false && isTextBox == true)
            {
                textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
                treeViewModel = treeViewModelController.ConverterToTreeViewModel(textRange.Text);

                if (treeViewModel == null)
                {
                    return;
                }
            }

            if (treeViewModel == null)
            {
                treeViewModel = new TreeViewModel();
                textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
                TextToTreeView(textRange.Text);
            }
            else
            {
                headerIsMatchedList = treeViewModel.DomainList();
                HostsTreeView.ItemsSource = treeViewModel.NodeList;
                treeViewModel.DomainIsMatched(headerIsMatchedList);

                if(treeViewModel.Pass == false)
                {
                    MessageBox.Show("중복으로 적용된 도메인이 있어 모든 항목들의 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", true);
                }  
            }

            if (textRange.Text == "" || (textRange.Text == "" && treeViewModel.NodeList.Count == 0))
            {
                ChangeInfoLabel("Info", "호스트 내용이 없습니다.", null);
            }
        }

        private void ChangeInfoLabel(String type, String msg, bool? setEdit)
        {
            String BackgroundColor = "";
            String ForegroundColor = "";
            BrushConverter Conv = new BrushConverter();

            if (type == "Warning")
            {
                BackgroundColor = "#F2DEDE";
                ForegroundColor = "#A94442";
            }
            else if (type == "Success")
            {
                BackgroundColor = "#DFF0D8";
                ForegroundColor = "#3C763D";
            }
            else if (type == "Info")
            {
                BackgroundColor = "#D9EDF7";
                ForegroundColor = "#31708F";
            }
            else if (type == "None")
            {
                BackgroundColor = "#00FFFFFF";
                ForegroundColor = "#00FFFFFF";
            }

            InfoLabel.Background = (Brush)Conv.ConvertFromString(BackgroundColor);
            InfoLabel.Foreground = (Brush)Conv.ConvertFromString(ForegroundColor);
            InfoLabel.Content = msg;

            ChangeApplyButtonUI(setEdit);
        }

        private void ChangeButtonUI()
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

                if (HostsTreeView.Visibility == Visibility.Visible)
                {
                    FindTreeViewItem(HostsTreeView);
                }
                else
                {
                    FindRichTextBox(Text_RichTextBox);
                }
            }

            if (HostsTreeView.Visibility == Visibility.Visible)
            {
                TreeView_Button.IsEnabled = true;
                TextEdit_Button.IsEnabled = false;
                HostsTreeView.Visibility = Visibility.Hidden;
                Text_RichTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                TreeView_Button.IsEnabled = false;
                TextEdit_Button.IsEnabled = true;
                HostsTreeView.Visibility = Visibility.Visible;
                Text_RichTextBox.Visibility = Visibility.Hidden;
            }
        }

        private void ChangeApplyButtonUI(bool? isChanged)
        {
            if (isChanged == null)
            {
                return;
            }
            else if (isChanged == true)
            {
                if (Apply_Button.IsEnabled != true)
                {
                    Apply_Button.IsEnabled = true;
                }
            }
            else
            {
                if (Apply_Button.IsEnabled != false)
                {
                    Apply_Button.IsEnabled = false;
                }
            }
        }

        private void FindTreeViewItem(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                TreeViewItem treeViewItem = obj as TreeViewItem;
                if (treeViewItem != null)
                {
                    HighlightText(treeViewItem);
                }
                FindTreeViewItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
            }
        }

        private void FindRichTextBox(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                RichTextBox textBlock = obj as RichTextBox;
                if (textBlock != null)
                {
                    HighlightText(textBlock);
                }
                FindRichTextBox(VisualTreeHelper.GetChild(obj as DependencyObject, i));
            }
        }

        private void HighlightText(Object itx)
        {
            if (itx != null)
            {
                Regex regex = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);
                if (itx is TextBlock)
                {
                    TextBlock textBlock = itx as TextBlock;
                    if (SearchBox.Text.Length == 0)
                    {
                        string str = textBlock.Text;
                        textBlock.Inlines.Clear();
                        textBlock.Inlines.Add(str);
                        return;
                    }
                    string[] substrings = regex.Split(textBlock.Text);
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
                        }
                        else
                        {
                            textBlock.Inlines.Add(item);
                        }
                    }
                    return;
                }
                else if (itx is RichTextBox)
                {
                    TextRange textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
                    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.White));
                    textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                    textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                    if (SearchBox.Text.Length == 0)
                    {
                        return;
                    }
                    else
                    {
                        TextPointer start = Text_RichTextBox.Document.ContentStart;

                        while (start != null && start.CompareTo(Text_RichTextBox.Document.ContentEnd) < 0)
                        {
                            if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                            {
                                Match match = regex.Match(start.GetTextInRun(LogicalDirection.Forward));

                                TextRange searchRange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
                                searchRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));
                                searchRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));
                                searchRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                                start = textRange.End; // I'm not sure if this is correct or skips ahead too far, try it out!!!
                            }
                            start = start.GetNextContextPosition(LogicalDirection.Forward);
                        }
                        return;
                    }
                }
                else
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                    {
                        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                    }
                }
            }
        }

        private void Add_Child_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(null, menuItem.Header.ToString());
            editTreeViewWindow.Owner = Application.Current.MainWindow;
            editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewWindow.ShowDialog();

            if (editTreeViewWindow.treeViewModel != null && editTreeViewWindow.treeViewModel.NodeList.Count != 0)
            {
                editTreeViewWindow.treeViewModel.NodeList.Reverse();

                foreach (Node item in editTreeViewWindow.treeViewModel.NodeList)
                {
                    item.ParentNode = node;
                    item.IsChanged = true;
                    item.IsChecked = true;
                    item.IsExpanded = true;
                    node.NodeList.Insert(0, item);
                }

                node.IsExpanded = true;

                headerIsMatchedList = treeViewModel.DomainList();
                editTreeViewWindow.treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (editTreeViewWindow.treeViewModel.Pass == false)
                {
                    headerIsMatchedList = treeViewModel.DomainList();

                    MessageBox.Show("중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", true);
                }
                else
                {
                    ChangeInfoLabel("Success", "선택한 항목이 수정되었습니다.", true);
                }
            }
            else
            {
                ChangeInfoLabel("Warning", "작업이 취소되었습니다.", null);
            }

            editTreeViewWindow.treeViewModel = null;
            editTreeViewWindow.Close();
        }

        private void Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node childNode = menuItem.DataContext as Node;
            Node parentNode = childNode.ParentNode;

            EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(null, menuItem.Header.ToString());
            editTreeViewWindow.Owner = Application.Current.MainWindow;
            editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewWindow.ShowDialog();

            if (editTreeViewWindow.treeViewModel != null && editTreeViewWindow.treeViewModel.NodeList.Count != 0)
            {
                editTreeViewWindow.treeViewModel.NodeList.Reverse();

                foreach (Node item in editTreeViewWindow.treeViewModel.NodeList)
                {
                    item.ParentNode = parentNode;
                    item.IsChanged = true;
                    item.IsChecked = true;
                    item.IsExpanded = true;

                    if (parentNode == null)
                    {
                        if (treeViewModel.NodeList.IndexOf(childNode) < (treeViewModel.NodeList.Count - 1))
                        {
                            treeViewModel.NodeList.Insert(treeViewModel.NodeList.IndexOf(childNode) + 1, item);
                        }
                        else
                        {
                            treeViewModel.NodeList.Add(item);
                        }
                    }
                    else
                    {
                        if (parentNode.NodeList.IndexOf(childNode) < (parentNode.NodeList.Count - 1))
                        {
                            parentNode.NodeList.Insert(parentNode.NodeList.IndexOf(childNode) + 1, item);
                        }
                        else
                        {
                            parentNode.NodeList.Add(item);
                        }
                    }
                }

                headerIsMatchedList = treeViewModel.DomainList();
                editTreeViewWindow.treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (editTreeViewWindow.treeViewModel.Pass == false)
                {
                    headerIsMatchedList = treeViewModel.DomainList();

                    MessageBox.Show("중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", true);
                }
                else
                {
                    ChangeInfoLabel("Success", "선택한 항목이 수정되었습니다.", true);
                }
            }
            else
            {
                ChangeInfoLabel("Warning", "작업이 취소되었습니다.", null);
            }

            editTreeViewWindow.treeViewModel = null;
            editTreeViewWindow.Close();
        }

        private void Edit_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(node, menuItem.Header.ToString());
            editTreeViewWindow.Owner = Application.Current.MainWindow;
            editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewWindow.ShowDialog();

            if (editTreeViewWindow.treeViewModel != null && editTreeViewWindow.treeViewModel.NodeList.Count != 0)
            {
                foreach (Node item in editTreeViewWindow.treeViewModel.NodeList)
                {
                    item.ParentNode = node.ParentNode;
                    item.IsChanged = true;
                    item.IsChecked = true;
                    item.IsExpanded = true;

                    if (node.ParentNode == null)
                    {
                        treeViewModel.NodeList.Insert(treeViewModel.NodeList.IndexOf(node), item);
                    }
                    else
                    {
                        node.ParentNode.NodeList.Insert(node.ParentNode.NodeList.IndexOf(node), item);
                    }
                }

                if (node.ParentNode == null)
                {
                    treeViewModel.NodeList.RemoveAt(treeViewModel.NodeList.IndexOf(node));
                }
                else
                {
                    node.ParentNode.NodeList.RemoveAt(node.ParentNode.NodeList.IndexOf(node));
                    node.ParentNode.IsSelected = false;
                }

                headerIsMatchedList = treeViewModel.DomainList();
                editTreeViewWindow.treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (editTreeViewWindow.treeViewModel.Pass == false)
                {
                    headerIsMatchedList = treeViewModel.DomainList();

                    MessageBox.Show("중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", true);
                }
                else
                {
                    ChangeInfoLabel("Success", "선택한 항목이 수정되었습니다.", true);
                }
            }
            else
            {
                ChangeInfoLabel("Warning", "작업이 취소되었습니다.", null);
            }

            editTreeViewWindow.treeViewModel = null;
            editTreeViewWindow.Close();
        }

        private void Del_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node.ParentNode == null)
            {
                treeViewModel.NodeList.Remove(node);
            }
            else
            {
                node.ParentNode.NodeList.Remove(node);
            }

            headerIsMatchedList = treeViewModel.DomainList();
            ChangeInfoLabel("Success", "선택한 항목이 삭제되었습니다.", true);
        }

        private void MoveToUp_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node.ParentNode == null)
            {
                if (treeViewModel.NodeList.IndexOf(node) > 0)
                {
                    treeViewModel.NodeList.Move(treeViewModel.NodeList.IndexOf(node), treeViewModel.NodeList.IndexOf(node) - 1);
                }
                else
                {
                    MessageBox.Show("이미 최상위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "더이상 위로 이동할 수 없습니다.", true);
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
                    ChangeInfoLabel("Warning", "더이상 위로 이동할 수 없습니다.", true);
                }
            }
        }

        private void MoveToDown_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            Node node = menuItem.DataContext as Node;

            if (node.ParentNode == null)
            {
                if (treeViewModel.NodeList.IndexOf(node) < (treeViewModel.NodeList.Count - 1))
                {
                    treeViewModel.NodeList.Move(treeViewModel.NodeList.IndexOf(node), treeViewModel.NodeList.IndexOf(node) + 1);
                }
                else
                {
                    MessageBox.Show("이미 최하위에 있으므로 더이상 이동할 수 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "더이상 아래로 이동할 수 없습니다.", true);
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
                    ChangeInfoLabel("Warning", "더이상 아래로 이동할 수 없습니다.", true);
                }
            }
        }

        private void Root_Add_TreeViewItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;

            EditTreeViewWindow editTreeViewWindow = new EditTreeViewWindow(null, menuItem.Header.ToString());
            editTreeViewWindow.Owner = Application.Current.MainWindow;
            editTreeViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewWindow.ShowDialog();

            if (editTreeViewWindow.treeViewModel != null && editTreeViewWindow.treeViewModel.NodeList.Count != 0)
            {
                editTreeViewWindow.treeViewModel.NodeList.Reverse();

                foreach (Node item in editTreeViewWindow.treeViewModel.NodeList)
                {
                    item.ParentNode = null;
                    item.IsChanged = true;
                    item.IsChecked = true;
                    item.IsExpanded = true;

                    treeViewModel.NodeList.Add(item);
                }

                headerIsMatchedList = treeViewModel.DomainList();
                editTreeViewWindow.treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (editTreeViewWindow.treeViewModel.Pass == false)
                {
                    headerIsMatchedList = treeViewModel.DomainList();

                    MessageBox.Show("중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", true);
                }
                else
                {
                    ChangeInfoLabel("Success", "선택한 항목이 수정되었습니다.", true);
                }
            }
            else
            {
                ChangeInfoLabel("Warning", "작업이 취소되었습니다.", null);
            }

            editTreeViewWindow.treeViewModel = null;
            editTreeViewWindow.Close();
        }

        private void TextToTreeView()
        {
            Text_RichTextBox.Document.Blocks.Clear();
            Text_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeViewModelController.ConverterToString(treeViewModel))));
            ChangeButtonUI();
        }

        private void TextToTreeView(String hostString)
        {
            Text_RichTextBox.Document.Blocks.Clear();
            Text_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(hostString)));
            ChangeInfoLabel("Warning", "변환에 실패하여 텍스트 모드로 전환되었습니다.", null);
            ChangeButtonUI();
        }

        private void TreeViewToText()
        {
            TextRange textRange = new TextRange(Text_RichTextBox.Document.ContentStart, Text_RichTextBox.Document.ContentEnd);
            treeViewModel = treeViewModelController.ConverterToTreeViewModel(textRange.Text);

            if (treeViewModel == null)
            {
                headerIsMatchedList.Clear();
                ChangeInfoLabel("Warning", "변환에 실패하여 텍스트 모드로 전환되었습니다.", null);
            }
            else
            {
                headerIsMatchedList = treeViewModel.DomainList();
                HostsTreeView.ItemsSource = treeViewModel.NodeList;
                treeViewModel.DomainIsMatched(headerIsMatchedList);

                if (treeViewModel.Pass == false)
                {
                    MessageBox.Show("중복으로 적용된 도메인이 있어 모든 항목들의 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 모든 체크를 해제하였습니다.", true);
                }

                if (treeViewModel.NodeList.Count == 0)
                {
                    ChangeInfoLabel("Info", "호스트 내용이 없습니다.", null);
                }

                ChangeButtonUI();
            }
        }

        private void TextEdit_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            TextToTreeView();
        }

        private void TreeView_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            TreeViewToText();
        }

        private void TreeView_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            TreeViewToText();
        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);

            if (Apply_Button.IsEnabled == true)
            {
                MessageBoxResult result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n새로고침을 할 경우 저장되지 않은 정보는 삭제됩니다.\r\n그래도 새로고침을 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (HostsTreeView.Visibility == Visibility.Visible)
                    {
                        BindTree();
                    }
                    else
                    {
                        Text_RichTextBox.Document.Blocks.Clear();
                        Text_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(hostIOController.HostToString())));
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
                    ChangeInfoLabel("Info", "새로고침을 취소하였습니다.", null);
                    return;
                }
            }
            else
            {
                if (HostsTreeView.Visibility == Visibility.Visible)
                {
                    BindTree();
                }
                else
                {
                    Text_RichTextBox.Document.Blocks.Clear();
                    Text_RichTextBox.Document.Blocks.Add(new Paragraph(new Run(hostIOController.HostToString())));
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

            if (InfoLabel.Content.ToString() == "")
            {
                ChangeInfoLabel("Info", "호스트 정보가 갱신되었습니다.", null);
            }
        }

        private void DirectEdit_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Apply_Button.IsEnabled ==false && (e.OriginalSource as RichTextBox).IsKeyboardFocused)
            {
                Apply_Button.IsEnabled = true;
                ChangeInfoLabel("Info", "수정된 항목이 있습니다.", null);
            }
        }

        private void NodePad_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            OpenNotepad();
        }

        private void TreeView_Notpad_Item_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            OpenNotepad();
        }

        private void TextBox_Notepad_Item_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            OpenNotepad();
        }

        private void OpenNotepad()
        {
            if (Apply_Button.IsEnabled == true)
            {
                MessageBoxResult result = MessageBox.Show("저장하지 않은 정보가 있습니다.\r\n저장하지 않은 정보는 메모장에 반영이 되지 않습니다.\r\n저장하고 메모장으로 여시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DoApply();
                }
            }

            hostIOController.OpenNotepad();
            ChangeInfoLabel("Info", "메모장에서 수정한 것을 반영하려면 새로고침을 누르세요.", null);
        }

        private void TextEdit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeInfoLabel("None", "", null);
            TextToTreeView();
        }
    }
}
