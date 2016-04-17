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
        public Node node = null;
        public MainWindow ParentWindow;
        TreeViewModelController treeViewModelController = new TreeViewModelController();

        public EditTreeViewWindow(Node node, String stringTitle)
        {
            InitializeComponent();
            this.Title = stringTitle;
            if (node != null)
            {
                this.node = node;
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
            if (this.node != null)
            {
                String nodeString = treeViewModelController.ConverterToString(this.node);
                DirectEditTextBox.AppendText(nodeString);
                
                if (this.node.IsLastNode)
                {
                    Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");

                    String stringIP = regex.Match(this.node.Header).Value;
                    String stringDomain = this.node.Header.Replace(stringIP, "").Replace("\t", "").Trim();

                    HostRadioButton.IsChecked = true;
                    HostIPTextBox.Text = stringIP;
                    HostDomainTextBox.Text = stringDomain;
                    HostTooltipTextBox.Text = this.node.Tooltip;
                }
                else
                {
                    CategoryRadioButton.IsChecked = true;
                    CategoryNameTextBox.Text = this.node.Header;
                    CategoryTooltipTextBox.Text = this.node.Tooltip;
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
                TreeViewModel treeViewModel = treeViewModelController.ConverterToTreeViewModel(DirectEditTextBox.Text);
            }
            else
            {
                if (CategoryRadioButton.IsChecked == true)
                {
                    node.Header = CategoryNameTextBox.Text;
                    node.IsLastNode = false;
                    node.Tooltip = CategoryTooltipTextBox.Text;
                }
                else
                {
                    if (HostIPTextBox.Text.Length < 16)
                    {
                        node.Header = HostIPTextBox.Text + "\t\t" + HostDomainTextBox.Text;
                    }
                    else
                    {
                        node.Header = HostIPTextBox.Text + "\t" + HostDomainTextBox.Text;
                    }
                    node.IsLastNode = true;
                    node.Tooltip = HostTooltipTextBox.Text;
                }
            }
            ((MainWindow)(this.Owner)).ApplyEditNode(node);
            Close();
        }

        private void EditNodeCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
