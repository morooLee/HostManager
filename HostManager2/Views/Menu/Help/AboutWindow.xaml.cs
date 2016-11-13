using HostManager.Controllers;
using HostManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HostManager.Views.Menu
{
    /// <summary>
    /// AboutWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        // 윈도우 로드 이벤트
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string updateurl = Settings.Default.UpdateUrl;
            FileVersionInfo myFI = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            Version clientVersion = new Version(myFI.FileVersion);
            Version serverVersion = null;
            

            

            try
            {
                BrowserController browserController = new BrowserController();
                serverVersion = new Version(browserController.RequestJson(updateurl, "version"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n업데이트를 확인하지 못했습니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (serverVersion == null)
            {
                UpdateCheck_Text.Text = "업데이트를 확인하지 못했습니다.";
                Update_Button.IsEnabled = false;
                //Help_Update_Panel.Visibility = Visibility.Hidden;
                //Help_Update_Panel.Height = 0;
            }
            else if (serverVersion <= clientVersion)
            {
                UpdateCheck_Text.Text = "최신버전입니다.";
                UpdateCheck_Text.Foreground = Brushes.Black;

                Update_Button.Visibility = Visibility.Hidden;
                //NewVersion_Text.Visibility = Visibility.Hidden;
                NewVersion_Panel.Height = 0;
            }

            Version.Text = "(Ver. " + clientVersion + ")";
        }

        // 업데이트 버튼 클릭 이벤트
        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://moroosoft.azurewebsites.net/Application/HostManager");
        }

        // 하이퍼링크 처리
        private void OnNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        // 매뉴얼 버튼 클릭 이벤트
        private void Manual_Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://moroosoft.azurewebsites.net/Application/HostManager");
        }
    }
}
