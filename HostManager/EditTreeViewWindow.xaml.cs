using HostManager.Controllers;
using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        private TreeViewModelController treeViewModelController = new TreeViewModelController();
        private Node tmpNode = null;

        public EditTreeViewWindow(Node node, String stringTitle)
        {
            this.SourceInitialized += (x, y) =>
            {
                this.HideMinimizeAndMaximizeButtons();
            };

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
                if (DirectEditTextBox.Text.Trim() == "")
                {
                    MessageBox.Show("입력된 내용이 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    treeViewModel = treeViewModelController.ConverterToTreeViewModel(DirectEditTextBox.Text);

                    if (treeViewModel == null || treeViewModel.NodeList.Count == 0)
                    {
                        //MessageBox.Show("변환된 내용이 없습니다.\r\n작성된 내용을 확인하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            else
            {
                if (tmpNode == null)
                {
                    tmpNode = new Node();
                }

                if (CategoryRadioButton.IsChecked == true)
                {
                    if (CategoryNameTextBox.Text == "")
                    {
                        MessageBox.Show("항목명을 입력하세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        tmpNode.Header = CategoryNameTextBox.Text;
                        tmpNode.IsLastNode = false;

                        if (CategoryTooltipTextBox.Text != "")
                        {
                            tmpNode.Tooltip = CategoryTooltipTextBox.Text;
                        }
                    }
                }
                else if (HostRadioButton.IsChecked == true)
                {
                    Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");

                    if (regex.IsMatch(HostIPTextBox.Text))
                    {
                        if (HostDomainTextBox.Text == "")
                        {
                            MessageBox.Show("도메인을 입력하세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
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

                            tmpNode.Domain = HostDomainTextBox.Text;
                        }
                    }
                    else
                    {
                        MessageBox.Show("IP주소를 입력하지 않았거나 형식에 어긋납니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    tmpNode.IsLastNode = true;

                    if (HostTooltipTextBox.Text != "")
                    {
                        tmpNode.Tooltip = HostTooltipTextBox.Text;
                    }
                }
                else
                {
                    MessageBox.Show("작성된 내용이 없습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                treeViewModel.NodeList.Add(tmpNode);
            }

            Close();
        }

        private void EditNodeCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    internal static class WindowExtensions
    {
        // from winuser.h
        private const int GWL_STYLE = -16,
                          //WS_MAXIMIZEBOX = 0x10000,
                          WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        extern private static int GetWindowLongPtr(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLongPtr(IntPtr hwnd, int index, int value);

        internal static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLongPtr(hwnd, GWL_STYLE);

            SetWindowLongPtr(hwnd, GWL_STYLE, (currentStyle & ~WS_MINIMIZEBOX));
        }
    }
}
