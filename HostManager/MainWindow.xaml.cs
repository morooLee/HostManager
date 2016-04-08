using HostManager.Controllers;
using HostManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private TreeViewModel tmpTreeViewNode = null;

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

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Red");

            ChangeButtonUI(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();

            sp.Opacity = 0.5;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Black");

            ChangeButtonUI(true);
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

            item.IsSelected = true;
        }

        private void BindTree()
        {
            treeViewModel = hostIOController.HostLoad();

            //if (tmpTreeViewNode == null)
            //{
            //    tmpTreeViewNode = new TreeViewModel();

            //    foreach (Node item in treeViewModel.NodeList.ToList())
            //    {
            //        tmpTreeViewNode.Add(item);
            //    }
            //}

            HostsTreeView.ItemsSource = treeViewModel.NodeList;
        }

        private void ChangeInfoLabel(String type, String msg, bool setEdit)
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

        private void ChangeButtonUI(bool isChanged)
        {
            if (Apply_Button.IsEnabled != isChanged)
            {
                Apply_Button.IsEnabled = isChanged;
            }

            if (Cancle_Button.IsEnabled != isChanged)
            {
                Cancle_Button.IsEnabled = isChanged;
            }
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            if (hostIOController.HostSave(treeViewModel))
            {
                ChangeInfoLabel("Success", "적용되었습니다.", true);
            }
            else
            {
                ChangeInfoLabel("Warning", "실패하였습니다.", true);
            }

            //tmpTreeViewNode.Clear();

            //foreach (Node item in treeViewModel.NodeList.ToList().AsReadOnly())
            //{
            //    tmpTreeViewNode.Add(item);
            //}
            
            ChangeButtonUI(false);
        }

        private void Cancle_Button_Click(object sender, RoutedEventArgs e)
        {
            //treeViewModel.NodeList.Clear();

            //foreach (Node item in tmpTreeViewNode.ToList().AsReadOnly())
            //{
            //    treeViewModel.NodeList.Add(item);
            //}

            //HostsTreeView.ItemsSource = treeViewModel.NodeList;

            ChangeInfoLabel("Warning", "취소하였습니다.", true);
            ChangeButtonUI(false);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Node node = item.DataContext as Node;

            if (item.IsSelected)
            {
                if (e.OriginalSource == sender)
                {
                    if (e.Key == Key.Space)
                    {
                        // ignore alt+space which invokes the system menu
                        if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            return;
                        }

                        node.IsChecked = !node.IsChecked;
                    }
                    else if (e.Key == Key.Enter && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
                    {
                        node.IsChecked = !node.IsChecked;
                    }
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchButtonImage.Tag.ToString() == "Search")
            {
                if (SearchBox.Text == "")
                {
                    ChangeInfoLabel("Warning", "검색어를 입력하세요.", false);
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri("Resources/Cancel.png", UriKind.Relative);
                    bi.EndInit();
                    SearchButtonImage.Source = bi;
                    SearchButtonImage.Tag = "Cancle";


                    foreach (Node node in treeViewModel.NodeList)
                    {
                        jumpToFolder(HostsTreeView, SearchBox.Text);
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

                ChangeInfoLabel("Info", "검색을 취소하였습니다.", false);
            }
            
            Console.WriteLine(SearchBox.Text);
        }

        private void SearchTreeView(Node node)
        {
            if (node.Header.ToString().Contains(SearchBox.Text))
            {
                node.IsExpanded = true;
            }
            else
            {
                node.IsExpanded = true;
            }

            if (node.NodeList != null)
            {
                foreach (Node _node in node.NodeList)
                {
                    SearchTreeView(_node);
                }
            }
        }
    }
}
