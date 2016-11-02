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
        List<Node> checkedList = new List<Node>();

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

            foreach (Node node in treeViewItemModel.NodeList)
            {
                if(node.IsChecked != false)
                {
                    DomainDuplication(node, true);
                }
            }
            Console.WriteLine(domainHashSet.Count);
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

            if (node.IsSelected)
            {
                DomainDuplication(node, true);
            }
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

            if (node.IsSelected)
            {
                DomainDuplication(node, false);
            }
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

        private void DomainDuplication(Node node, bool isAdded)
        {
            if(isAdded == true)
            {
                if(node.IsChecked != false && node.Domain != null)
                {
                    bool hashSetResult = false;
                    hashSetResult = domainHashSet.Add(node.Domain);
                    if (hashSetResult == false)
                    {
                        treeViewItemModel.IsDomainDuplication(node.Domain);
                        node.IsChecked = true;
                    }
                }

                if (node.IsExternalNode == false)
                {
                    foreach (Node childNode in node.NodeList)
                    {
                        DomainDuplication(childNode, isAdded);
                    }
                }
            }
            else if (isAdded == false)
            {
                domainHashSet.Remove(node.Domain);

                if (node.IsExternalNode == false)
                {
                    foreach (Node childNode in node.NodeList)
                    {
                        DomainDuplication(childNode, isAdded);
                    }
                }
            }
        }
    }
}
