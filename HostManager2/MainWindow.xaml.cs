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
        FileController fileController = new FileController();
        HashSet<string> domainHashSet = new HashSet<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void HostsTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            BindTree();
        }

        private void BindTree()
        {
            treeViewItemModel.NodeList = fileController.ToNodeList();
            HostsTreeView.ItemsSource = treeViewItemModel.NodeList;
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
            BrushConverter conv = new BrushConverter();            
            Node node = sp.DataContext as Node;

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Red");

            //if (node.IsSelected)
            //{
            //    HeaderIsOverlap(node, true);
            //}
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter conv = new BrushConverter();
            Node node = sp.DataContext as Node;

            sp.Opacity = 0.5;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Black");

            //if (node.IsSelected)
            //{
            //    HeaderIsOverlap(node, false);
            //}
        }

        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            
            BrushConverter conv = new BrushConverter();

            sp.Opacity = 1.0;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)conv.ConvertFromString("Red");
        }

        private void CheckBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            Node node = sp.DataContext as Node;
            Console.WriteLine(node.GetHashCode());
            
            node.IsSelected = true;
        }

        //private void DomainDuplication(Node node, bool isChecked)
        //{
        //    if (node.IsExternalNode)
        //    {
        //        if (isChecked)
        //        {
        //            headerIsMatchedList.Add(node.Domain);

        //            if (headerIsMatchedList.Count(x => x == node.Domain) > 1)
        //            {
        //                treeViewModel.SearchNode(node.Domain);
        //                //MessageBox.Show("도메인이 중복으로 적용되어 체크할 수 없습니다.\r\n검색을 통해 적용되어 있는 도메인을 찾으세요.\r\n\r\n도메인명 : " + node.Domain, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                //ChangeInfoLabel("Info", "중복되는 도메인명 : " + node.Domain, null);
        //                //node.IsChecked = false;
        //                //if (node.ParentNode.IsExpanded)
        //                //{
        //                //    node.ParentNode.IsExpanded = true;
        //                //}
        //                headerIsMatchedList.RemoveAt(headerIsMatchedList.LastIndexOf(node.Domain));
        //                node.IsChecked = true;
        //            }
        //            else
        //            {
        //                ChangeInfoLabel("None", "", true);
        //            }
        //        }
        //        else
        //        {
        //            if (headerIsMatchedList.Contains(node.Domain))
        //            {
        //                headerIsMatchedList.RemoveAt(headerIsMatchedList.LastIndexOf(node.Domain));
        //            }

        //            ChangeInfoLabel("None", "", true);
        //        }
        //    }
        //    else
        //    {
        //        if (node.NodeList == null || node.NodeList.Count == 0)
        //        {
        //            ChangeInfoLabel("None", "", true);
        //        }
        //        else
        //        {
        //            foreach (Node item in node.NodeList)
        //            {
        //                HeaderIsOverlap(item, isChecked);
        //            }
        //        }
        //    }
        //}
    }
}
