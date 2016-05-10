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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //RestartIE.IsChecked = Settings.Default.IE_Auto_Restart;
            //RestartEdge.IsChecked = Settings.Default.Edge_Auto_Restart;
            //RestartChrome.IsChecked = Settings.Default.Chrome_Auto_Restart;
            //RestartFirefox.IsChecked = Settings.Default.FF_Auto_Restart;

            //AutoDelIE.IsChecked = Settings.Default.IE_TmpFile_Del;
            //AutoDelEdge.IsChecked = Settings.Default.Edge_TmpFile_Del;
            //AutoDelChrome.IsChecked = Settings.Default.Chrome_TmpFile_Del;
            //AutoDelFirefox.IsChecked = Settings.Default.FF_TmpFile_Del;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            this.Close();
        }

        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
