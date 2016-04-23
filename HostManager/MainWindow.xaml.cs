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
        private TreeViewModel treeViewModel = new TreeViewModel();
        private List<String> headerIsMatchedList = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UI_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree();
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
                        Console.WriteLine(headerIsMatchedList.Count);
                    }
                    else
                    {
                        ChangeInfoLabel("None", "", true);
                    }
                    Console.WriteLine(headerIsMatchedList.Count);
                }
                else
                {
                    if (headerIsMatchedList.Contains(node.Domain))
                    {
                        headerIsMatchedList.RemoveAt(headerIsMatchedList.LastIndexOf(node.Domain));
                        Console.WriteLine(headerIsMatchedList.Count);
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
                    SearchButtonImage.Tag = "Cancle";
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
            if (hostIOController.HostSave(treeViewModel))
            {
                treeViewModel.IsChangedCancel();
                ChangeInfoLabel("Success", "적용되었습니다.", true);
            }
            else
            {
                ChangeInfoLabel("Warning", "실패하였습니다.", true);
            }

            ChangeButtonUI(false);
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
                    SearchButtonImage.Tag = "Cancle";

                    int Count = 0;

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

                FindTreeViewItem(HostsTreeView);

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

                SearchButtonImage.Tag = "Cancle";
                SearchButton_Click(SearchButton, null);
            }
            else if (e.Key == Key.Return && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                SearchButtonImage.Tag = "Search";
                SearchButton_Click(SearchButton, null);
            }
            else if (e.Key == Key.Escape && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                SearchButtonImage.Tag = "Cancle";
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

            ChangeInfoLabel("None", "", null);
        }

        private void BindTree()
        {
            treeViewModel = hostIOController.HostLoad();
            headerIsMatchedList = treeViewModel.DomainList();

            if (treeViewModel == null)
            {
                treeViewModel = new TreeViewModel();
            }
            else
            {
                HostsTreeView.ItemsSource = treeViewModel.NodeList;
                treeViewModel.DomainIsMatched(headerIsMatchedList);

                if(treeViewModel.Pass == false)
                {
                    MessageBox.Show("중복으로 적용된 도메인이 있어 모든 항목들의 체크를 해제하였습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChangeInfoLabel("Warning", "중복으로 적용된 도메인이 있어 체크를 해제하였습니다.", null);
                }
            }

            if (treeViewModel.NodeList.Count == 0)
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

            ChangeButtonUI(setEdit);
        }

        private void ChangeButtonUI(bool? isChanged)
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

        private void HighlightText(Object itx)
        {
            if (itx != null)
            {
                if (itx is TextBlock)
                {
                    Regex regex = new Regex("(" + SearchBox.Text + ")", RegexOptions.IgnoreCase);
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
                    foreach (var item in substrings)
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
                else
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                    {
                        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                    }
                }
            }
        }

        private void AddChildItem_Click(object sender, RoutedEventArgs e)
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

        private void AddItem_Click(object sender, RoutedEventArgs e)
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
                        treeViewModel.NodeList.Insert(parentNode.NodeList.IndexOf(childNode), item);
                    }
                    else
                    {
                        parentNode.NodeList.Insert(parentNode.NodeList.IndexOf(childNode), item);
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

        private void EditItem_Click(object sender, RoutedEventArgs e)
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
                }

                node.ParentNode.IsSelected = false;
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

        private void DelItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
