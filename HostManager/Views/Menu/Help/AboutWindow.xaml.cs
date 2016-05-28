using Newtonsoft.Json;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string updateurl;

            if (IntPtr.Size == 8)
            {
                updateurl = "http://www.moroosoft.com/Application/HostManager?version=64";
            }
            else
            {
                updateurl = "http://www.moroosoft.com/Application/HostManager?version=32";
            }

            System.Net.WebClient wclient = new System.Net.WebClient();
            wclient.BaseAddress = updateurl;

            try
            {
                dynamic json = JsonConvert.DeserializeObject(wclient.DownloadString(updateurl));
                string newVersion = json["version"];

                NewVersion.Text = newVersion;
            }
            catch
            {

            }
            Version.Text = version;
        }
    }
}
