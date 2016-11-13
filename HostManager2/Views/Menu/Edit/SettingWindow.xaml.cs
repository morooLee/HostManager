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
using HostManager.Properties;
using HostManager.Controllers;

namespace HostManager.Views.Menu
{
    /// <summary>
    /// PreferencesWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        // 윈도우 로드 이벤트
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RestartIE.IsChecked = Settings.Default.AutoRestart_IE;
            RestartEdge.IsChecked = Settings.Default.AutoRestart_Edge;
            RestartChrome.IsChecked = Settings.Default.AutoRestart_Chrome;
            RestartFirefox.IsChecked = Settings.Default.AutoRestart_Firefox;

            AutoDelIE.IsChecked = Settings.Default.TempFileDel_IE;
            //AutoDelEdge.IsChecked = Settings.Default.Edge_TmpFile_Del;
            //AutoDelChrome.IsChecked = Settings.Default.Chrome_TmpFile_Del;
            //AutoDelFirefox.IsChecked = Settings.Default.FF_TmpFile_Del;

            InputUrl_Textbox.Text = Settings.Default.HostFileUrl;
            IsHostLoadedUrl_CheckBox.IsChecked = Settings.Default.IsHostLoadedUrl;

            IsApplyAlert_CheckBox.IsChecked = Settings.Default.IsApplyAlert;

            IsUpdateCheck_CheckBox.IsChecked = Settings.Default.IsUpdateCheck;
        }

        // 적용버튼 클릭 이벤트
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            bool urlCheck = false;

            if (InputUrl_Textbox.Text != "")
            {
                try
                {
                    Uri uri = new Uri(InputUrl_Textbox.Text);
                    urlCheck = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (IsHostLoadedUrl_CheckBox.IsChecked == true)
                {
                    MessageBox.Show("Url이 설정되어 있지 않아 체크를 해제 합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsHostLoadedUrl_CheckBox.IsChecked = false;

                    urlCheck = false;
                }
                else
                {
                    urlCheck = true;
                }
            }

            if (urlCheck)
            {
                Settings.Default.AutoRestart_IE = (bool)RestartIE.IsChecked;
                Settings.Default.AutoRestart_Edge = (bool)RestartEdge.IsChecked;
                Settings.Default.AutoRestart_Chrome = (bool)RestartChrome.IsChecked;
                Settings.Default.AutoRestart_Firefox = (bool)RestartFirefox.IsChecked;

                Settings.Default.TempFileDel_IE = (bool)AutoDelIE.IsChecked;
                Settings.Default.TempFileDel_Edge = (bool)AutoDelEdge.IsChecked;
                Settings.Default.TempFileDel_Chrome = (bool)AutoDelChrome.IsChecked;
                Settings.Default.TempFileDel_Firefox = (bool)AutoDelFirefox.IsChecked;

                Settings.Default.HostFilePath = HostPath_TextBox.Text;

                Settings.Default.HostFileUrl = InputUrl_Textbox.Text;
                Settings.Default.IsHostLoadedUrl = (bool)IsHostLoadedUrl_CheckBox.IsChecked;

                Settings.Default.IsApplyAlert = (bool)IsApplyAlert_CheckBox.IsChecked;

                Settings.Default.IsUpdateCheck = (bool)IsUpdateCheck_CheckBox.IsChecked;

                Settings.Default.Save();

                this.DialogResult = true;
                this.Close();
            }
        }

        // 닫기버튼 클릭 이벤트
        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // 호스트 경로 입력창 클릭 시 폴더찾기 창 출력하기
        private void HostPath_TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditHostPath();
        }

        // 호스트 경로 버튼 클릭 이벤트
        private void HostPath_Button_Click(object sender, RoutedEventArgs e)
        {
            EditHostPath();
        }

        // 호스트 경로 변경하기
        private void EditHostPath()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                HostPath_TextBox.Text = dlg.SelectedPath;
            }
        }

        // 호스트 기본 경로 설정 하기
        private void PathInit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HostPath_TextBox.Text = @"C:\Windows\System32\drivers\etc";
        }

        // Url 텍스트 클릭 시 체크박스 상태값 변경 하기
        private void IsHostLoadedUrl_TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHostLoadedUrl_CheckBox.IsChecked = !IsHostLoadedUrl_CheckBox.IsChecked;
        }

        // 적용 버튼 알럿창 출력 여부
        private void IsApplyAlert_TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsApplyAlert_CheckBox.IsChecked = !IsApplyAlert_CheckBox.IsChecked;
        }

        // 업데이트 확인 텍스트 클릭 시 체크ㅡ박스 상태값 변경하기
        private void IsUpdateCheck_TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsUpdateCheck_CheckBox.IsChecked = !IsUpdateCheck_CheckBox.IsChecked;
        }

        // Url 형식 체크하기
        private void UrlCheck_Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputUrl_Textbox.Text == "")
            {
                MessageBox.Show("Url이 입력되지 않았습니다.", "UrlCheck", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                BrowserController browserController = new BrowserController();

                try
                {
                    browserController.OpenFileForWeb(InputUrl_Textbox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "UrlCheck", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
