using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string updateurl = "http://www.moroosoft.com/Application/HostManager?version=Check";
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string newVersion = null;

            System.Net.WebClient wclient = new System.Net.WebClient();
            wclient.BaseAddress = updateurl;

            try
            {
                JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                dynamic json = jsonSer.DeserializeObject(wclient.DownloadString(updateurl));
                newVersion = json["version"];

                //NewVersion_Text.Text = "(Ver. " + newVersion + ")";
                Update_Button.Content = "Ver. " + newVersion;
            }
            catch
            {

            }
            
            if (newVersion == null)
            {
                Help_Update_Panel.Visibility = Visibility.Hidden;
                Help_Update_Panel.Height = 0;
            }
            else if (newVersion == version)
            {
                UpdateCheck_Text.Text = "최신버전입니다.";
                UpdateCheck_Text.Foreground = Brushes.Black;

                Update_Button.Visibility = Visibility.Hidden;
                //NewVersion_Text.Visibility = Visibility.Hidden;
                NewVersion_Panel.Height = 0;
            }

            Version.Text = "(Ver. " + version + ")";
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.moroosoft.com/Application/HostManager");
        }

        private void OnNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void Manual_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("작성하고 있습니다!");
        }
    }
}
