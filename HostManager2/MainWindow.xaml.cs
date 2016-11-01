﻿using HostManager.Controllers;
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
    }
}
