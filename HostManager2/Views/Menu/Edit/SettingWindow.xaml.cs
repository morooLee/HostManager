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
            Settings.Default.AutoRestart_IE = (bool)RestartIE.IsChecked;
            Settings.Default.AutoRestart_Edge = (bool)RestartEdge.IsChecked;
            Settings.Default.AutoRestart_Chrome = (bool)RestartChrome.IsChecked;
            Settings.Default.AutoRestart_Firefox = (bool)RestartFirefox.IsChecked;

            Settings.Default.TempFileDel_IE = (bool)AutoDelIE.IsChecked;
            Settings.Default.TempFileDel_Edge = (bool)AutoDelEdge.IsChecked;
            Settings.Default.TempFileDel_Chrome = (bool)AutoDelChrome.IsChecked;
            Settings.Default.TempFileDel_Firefox = (bool)AutoDelFirefox.IsChecked;

            Settings.Default.HostFilePath = HostPath_TextBox.Text;

            Settings.Default.Save();

            this.DialogResult = true;
            this.Close();
        }

        private void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void HostPath_TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditHostPath();
        }

        private void HostPath_Button_Click(object sender, RoutedEventArgs e)
        {
            EditHostPath();
        }

        private void EditHostPath()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                HostPath_TextBox.Text = dlg.SelectedPath;
            }
        }

        private void PathInit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HostPath_TextBox.Text = @"C:\Windows\System32\drivers\etc";
        }
    }
}
