using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HostManager.Properties;
using System.Net;
using System.Windows;
using System.IO;

namespace HostManager.Controllers
{
    public class BrowserController
    {
        public void CheckDoBrowsers()
        {
            if (Settings.Default.AutoRestart_IE || Settings.Default.AutoRestart_Edge || Settings.Default.AutoRestart_Chrome || Settings.Default.AutoRestart_Firefox)
            {
                bool isIE = false;
                String pathIE = "";
                bool isEdge = false;
                String pathEdge = "";
                bool isChrome = false;
                String pathChrome = "";
                bool isFireFox = false;
                String pathFireFox = "";

                Process[] processList = Process.GetProcesses();
                List<Process> tmpList = new List<Process>();

                foreach (Process item in processList)
                {
                    if (item.ProcessName == "iexplore")
                    {
                        if (Settings.Default.AutoRestart_IE)
                        {
                            pathIE = item.MainModule.FileName;
                            isIE = true;
                            tmpList.Add(item);
                        }
                    }
                    else if (item.ProcessName == "MicrosoftEdge")
                    {
                        if (Settings.Default.AutoRestart_Edge)
                        {
                            pathEdge = item.MainModule.FileName;
                            isEdge = true;
                            tmpList.Add(item);
                        }
                    }
                    else if (item.ProcessName == "chrome")
                    {
                        if (Settings.Default.AutoRestart_Chrome)
                        {
                            pathChrome = item.MainModule.FileName;
                            isChrome = true;
                            tmpList.Add(item);
                        }
                    }
                    else if (item.ProcessName == "firefox")
                    {
                        if (Settings.Default.AutoRestart_Firefox)
                        {
                            pathFireFox = item.MainModule.FileName;
                            isFireFox = true;
                            tmpList.Add(item);
                        }
                    }
                }

                foreach (Process _item in tmpList)
                {
                    _item.Kill();
                }

                if (Settings.Default.TempFileDel_IE)
                {
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 8");
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2");
                }
                if (Settings.Default.TempFileDel_Edge)
                {

                }
                if (Settings.Default.TempFileDel_Chrome)
                {

                }
                if (Settings.Default.TempFileDel_Firefox)
                {

                }

                if (isIE)
                {
                    Process.Start(pathIE);
                    Thread.Sleep(500);
                }
                if (isEdge)
                {
                    Process.Start("microsoft-edge:http://www.nexon.com");
                    Thread.Sleep(500);
                }
                if (isChrome)
                {
                    Process.Start(pathChrome);
                    Thread.Sleep(500);
                }
                if (isFireFox)
                {
                    Process.Start(pathFireFox);
                    Thread.Sleep(500);
                }
            }
        }

        public string OpenFileForWeb(string url)
        {
            string hosts = "";
            try
            {
                Uri uri = new Uri(url);
                WebRequest req = WebRequest.Create(uri);
                req.Method = "HEAD";
                WebResponse res = req.GetResponse();

                if (res.ContentType == "text/html")
                {
                    throw new FileNotFoundException("파일을 찾을 수 없습니다.\r\n Url이 정확한지 다시 확인해 주세요.", "original");
                }
                else
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(url);

                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);

                    while (stream.Read(b, 0, b.Length) > 0)
                    {
                        hosts += temp.GetString(b);
                    }

                    stream.Close();

                    //Settings.Default.HostFileUrl = url;
                    //Settings.Default.IsHostLoadedUrl = true;
                    //WebClient client = new WebClient();
                    //hosts = client.DownloadString(uri);
                }
            }
            catch
            {
                throw;
            }

            return hosts;
        }

        public bool SaveFileForWeb(string hosts)
        {
            try
            {
                Uri uri = new Uri(Settings.Default.HostFileUrl);
                WebRequest req = WebRequest.Create(uri);
                req.Method = "HEAD";
                WebResponse res = req.GetResponse();

                if (res.ContentType == "text/html")
                {
                    throw new FileNotFoundException("파일을 찾을 수 없습니다.\r\n Url이 정확한지 다시 확인해 주세요.", "original");
                }
                else
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenWrite(Settings.Default.HostFileUrl);
                    Byte[] info = Encoding.ASCII.GetBytes(hosts);/* UTF8Encoding(true).GetBytes(hosts);*/

                    stream.Write(info, 0, info.Length);
                    stream.Close();

                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
