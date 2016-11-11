using HostManager.Controllers;
using HostManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HostManager.Views
{
    /// <summary>
    /// UrlInputWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UrlInputWindow : Window
    {
        private BrowserController browserController = new BrowserController();
        public string hosts = "";

        public UrlInputWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.HostFileUrl == "")
            {
                InputUrl_Textbox.Text = "ex) http://moroo.ipdisk.co.kr:80/publist/VOL1/HostManager/Files/hosts.txt";
                PlaceHolder(InputUrl_Textbox, true);
            }
            else
            {
                InputUrl_Textbox.Text = Settings.Default.HostFileUrl;
            }
        }

        private void PlaceHolder(TextBox textBox, bool isSet)
        {
            if(isSet)
            {
                textBox.Foreground = Brushes.Gray;
                textBox.FontStyle = FontStyles.Italic;
            }
            else
            {
                textBox.Foreground = Brushes.Black;
                textBox.FontStyle = FontStyles.Normal;
            }
        }

        private void UrlInput_Textbox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void UrlInput_Textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputUrl_Textbox.Text == "ex) http://moroo.ipdisk.co.kr:80/publist/VOL1/HostManager/Files/hosts.txt")
            {
                InputUrl_Textbox.Text = "";
                PlaceHolder(InputUrl_Textbox, false);
            }
        }

        private void UrlInput_Textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputUrl_Textbox.Text == "")
            {
                InputUrl_Textbox.Text = "ex) http://moroo.ipdisk.co.kr:80/publist/VOL1/HostManager/Files/hosts.txt";
                PlaceHolder(InputUrl_Textbox, true);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputUrl_Textbox.Text == "ex) http://moroo.ipdisk.co.kr:80/publist/VOL1/HostManager/Files/hosts.txt" || InputUrl_Textbox.Text == "")
            {
                MessageBox.Show("Url 주소가 입력되지 않았습니다.\r\n 입력 후 다시 시도해주세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    hosts = browserController.OpenFileForWeb(InputUrl_Textbox.Text);
                    Settings.Default.HostFileUrl = InputUrl_Textbox.Text;
                    Settings.Default.IsHostLoadedUrl = (bool) IsHostLoadedUrl_CheckBox.IsChecked;

                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
