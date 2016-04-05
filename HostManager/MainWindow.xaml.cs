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

namespace HostManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        TreeViewModelController treeViewModelController = new TreeViewModelController();
        HostIOController hostIOController = new HostIOController();
        TreeViewModel treeViewModel = new TreeViewModel();

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
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            StackPanel sp = (StackPanel)ckb.Parent;
            BrushConverter Conv = new BrushConverter();

            sp.Opacity = 0.5;
            ckb.BorderThickness = new Thickness(1, 1, 1, 1);
            ckb.BorderBrush = (Brush)Conv.ConvertFromString("Black");
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
            }

            statusBar.Items.Clear();
            statusBar.Items.Add(sbText);
        }

        private void BindTree()
        {
            treeViewModel = hostIOController.HostLoad();
            HostsTreeView.ItemsSource = treeViewModel.NodeList;
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

            if (setEdit == true)
            {
                Apply_Button.IsEnabled = true;
                Cancle_Button.IsEnabled = true;
            }
            else if (setEdit == false)
            {
                Apply_Button.IsEnabled = false;
                Cancle_Button.IsEnabled = false;
            }
        }
    }
}
