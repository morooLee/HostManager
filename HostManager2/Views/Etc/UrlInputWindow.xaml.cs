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

namespace HostManager.Views.Etc
{
    /// <summary>
    /// UrlInputWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UrlInputWindow : Window
    {
        private string exUrl = "ex) http://moroo.ipdisk.co.kr:80/publist/VOL1/HostManager/Files/hosts.txt";
        public string hosts = "";

        public UrlInputWindow()
        {
            InitializeComponent();
        }

        // 윈도우 로드 이벤트
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

        // Url 입력창에 PlaceHolder 기능 구현하기
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

        // Url 입력창이 포커스를 얻었을 경우 PlaceHolder 적용
        private void UrlInput_Textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputUrl_Textbox.Text == exUrl)
            {
                InputUrl_Textbox.Text = "";
                PlaceHolder(InputUrl_Textbox, false);
            }
        }

        // Url 입력창이 포커스를 잃었을 경우 PlaceHoder 적용
        private void UrlInput_Textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputUrl_Textbox.Text == "")
            {
                InputUrl_Textbox.Text = exUrl;
                PlaceHolder(InputUrl_Textbox, true);
            }
        }

        // 적용버튼 클릭 이벤트
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputUrl_Textbox.Text == exUrl || InputUrl_Textbox.Text == "")
            {
                MessageBox.Show("Url 주소가 입력되지 않았습니다.\r\n입력 후 다시 시도해주세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    BrowserController browserController = new BrowserController();

                    hosts = browserController.OpenFileForWeb(InputUrl_Textbox.Text);

                    if (hosts == "")
                    {
                        MessageBox.Show("호스트 내용이 없습니다.\r\nUrl을 다시 확인해 주세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Settings.Default.HostFileUrl = InputUrl_Textbox.Text;
                        Settings.Default.IsHostLoadedUrl = (bool)IsHostLoadedUrl_CheckBox.IsChecked;

                        Settings.Default.Save();

                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 닫기버튼 클릭 이벤트
        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // 텍스트 클릭 시 체크박스 상태값 변경하기
        private void IsHostLoadedUrl_TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHostLoadedUrl_CheckBox.IsChecked = !IsHostLoadedUrl_CheckBox.IsChecked;
        }
    }
}
