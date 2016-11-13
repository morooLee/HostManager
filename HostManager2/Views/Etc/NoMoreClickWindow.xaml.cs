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

namespace HostManager.Views.Etc
{
    /// <summary>
    /// NoMoreClickWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoMoreClickWindow : Window
    {
        public bool doNotAgainRespect = false;

        public NoMoreClickWindow()
        {
            InitializeComponent();
        }

        // 확인버튼 클릭 이벤트
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsApplyAlert = true;
            Settings.Default.Save();

            this.Close();
        }

        // 닫기버튼 클릭 이벤트
        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsApplyAlert = false;
            Settings.Default.Save();

            this.Close();
        }

        // 취향존중 버튼 클릭 이벤트
        private void Respect_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 윈도우 로드 이벤트
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsApplyAlert)
            {
                IsApplyAlert_Panel.Visibility = Visibility.Hidden;
                Respect_Panel.Visibility = Visibility.Visible;
            }
            else
            {
                Respect_Panel.Visibility = Visibility.Hidden;
                IsApplyAlert_Panel.Visibility = Visibility.Visible;
            }
        }

        // 다시보지 않기 기능 구현
        private void DoNotAgain_TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            doNotAgainRespect = true;
            this.Close();
        }
    }
}
