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
using System.Windows.Shapes;

namespace HostManager
{
    /// <summary>
    /// EditTreeViewWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditTreeViewWindow : Window
    {
        public TreeViewModel treeViewModel = new TreeViewModel();
        private Node tmpNode = null;
        TreeViewModelController treeViewModelController = new TreeViewModelController();

        public EditTreeViewWindow(Node node, String stringTitle)
        {
            InitializeComponent();
            this.Title = stringTitle;
            if (node != null)
            {
                tmpNode = node;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditTreeViewHelpWindow editTreeViewHelpWindow = new EditTreeViewHelpWindow();
            editTreeViewHelpWindow.Owner = this;
            editTreeViewHelpWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editTreeViewHelpWindow.ShowDialog();
            editTreeViewHelpWindow.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tmpNode != null)
            {
                String nodeString = treeViewModelController.ConverterToString(tmpNode);
                DirectEditTextBox.AppendText(nodeString);
                
                if (tmpNode.IsLastNode)
                {
                    Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");

                    String stringIP = regex.Match(tmpNode.Header).Value;
                    String stringDomain = tmpNode.Header.Replace(stringIP, "").Replace("\t", "").Trim();

                    HostRadioButton.IsChecked = true;
                    HostIPTextBox.Text = stringIP;
                    HostDomainTextBox.Text = stringDomain;
                    HostTooltipTextBox.Text = tmpNode.Tooltip;
                    CategoryRadioButton.IsEnabled = false;
                    CategoryRadioButton.Foreground = Brushes.Gray;
                }
                else
                {
                    CategoryRadioButton.IsChecked = true;
                    CategoryNameTextBox.Text = tmpNode.Header;
                    CategoryTooltipTextBox.Text = tmpNode.Tooltip;
                    HostRadioButton.IsEnabled = false;
                    HostRadioButton.Foreground = Brushes.Gray;
                }
            }
        }

        private void RichTextBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            DirectEditTextBox.Text = "";
        }

        private void RichTextBoxAddButton_Click(object sender, RoutedEventArgs e)
        {
            String addString = "";
            addString += "#{{항목명\r\n";
            addString += "\r\n";
            addString += "#}}\r\n";

            DirectEditTextBox.AppendText(addString);
        }

        private void EditNodeApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextTabItem.IsSelected)
            {
                treeViewModel = treeViewModelController.ConverterToTreeViewModel(DirectEditTextBox.Text);
            }
            else
            {
                if (tmpNode == null)
                {
                    tmpNode = new Node();
                }

                if (CategoryRadioButton.IsChecked == true)
                {
                    tmpNode.Header = CategoryNameTextBox.Text;
                    tmpNode.IsLastNode = false;
                    if (CategoryTooltipTextBox.Text != "")
                    {
                        tmpNode.Tooltip = CategoryTooltipTextBox.Text;
                    }
                }
                else
                {
                    if (HostIPTextBox.Text.Length < 16)
                    {
                        tmpNode.Header = HostIPTextBox.Text + "\t\t" + HostDomainTextBox.Text;
                    }
                    else
                    {
                        tmpNode.Header = HostIPTextBox.Text + "\t" + HostDomainTextBox.Text;
                    }
                    tmpNode.IsLastNode = true;
                    if (HostTooltipTextBox.Text != "")
                    {
                        tmpNode.Tooltip = HostTooltipTextBox.Text;
                    }
                }
            }

            treeViewModel.NodeList.Add(tmpNode);
            Close();
        }

        private void EditNodeCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
