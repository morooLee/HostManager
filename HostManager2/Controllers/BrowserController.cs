using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HostManager.Properties;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Web.Script.Serialization;

namespace HostManager.Controllers
{
    public class BrowserController
    {
        /// <summary>
        /// 브라우저 설정 저장하기
        /// </summary>
        public void CheckDoBrowsers()
        {
            if (Settings.Default.AutoRestart_IE || Settings.Default.AutoRestart_Edge || Settings.Default.AutoRestart_Chrome || Settings.Default.AutoRestart_Firefox)
            {
                bool isIE = false;
                string pathIE = "";
                bool isEdge = false;
                string pathEdge = "";
                bool isChrome = false;
                string pathChrome = "";
                bool isFireFox = false;
                string pathFireFox = "";

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

        /// <summary>
        /// 웹에서 호스트 파일 정보 가져오기
        /// </summary>
        /// <param name="url">URL 정보</param>
        /// <returns>호스트 정보</returns>
        public string OpenFileForWeb(string url)
        {
            string hosts = "";

            if (url != "")
            {
                try
                {
                    Uri uri = new Uri(url);
                    string contentType = "";
                    WebRequest req = WebRequest.Create(uri);
                    //req.Method = "HEAD";
                    WebResponse res = req.GetResponse();
                    contentType = res.ContentType;
                    res.Close();

                    if (contentType == "text/html")
                    {
                        throw new FileNotFoundException("파일을 찾을 수 없습니다.\r\n Url이 정확한지 다시 확인해 주세요.", "original");
                    }
                    else
                    {
                        WebClient webClient = new WebClient();
                        Stream stream = webClient.OpenRead(url);
                        StreamReader streamReader = new StreamReader(stream);

                        hosts = streamReader.ReadToEnd();
                        stream.Close();
                        stream.Dispose();
                        webClient.Dispose();

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
            }

            return hosts;
        }

        // 웹에 호스트 파일 저장하기 - 미 구현 (구현 불가)
        public bool SaveFileForWeb(string hosts)
        {
            try
            {
                //Uri uri = new Uri(Settings.Default.HostFileUrl);
                //string contentType = "";
                //WebRequest req = WebRequest.Create(uri);
                //req.Method = "HEAD";
                //WebResponse res = req.GetResponse();
                //contentType = res.ContentType;
                //res.Close();

                //if (contentType == "text/html")
                //{
                //    throw new FileNotFoundException("파일을 찾을 수 없습니다.\r\n Url이 정확한지 다시 확인해 주세요.", "original");
                //}
                //else
                //{
                //    byte[] data = Encoding.UTF8.GetBytes(hosts);
                //    WebClient webClient = new WebClient();



                //    Stream stream = webClient.OpenWrite(Settings.Default.HostFileUrl);

                //    if (data.Length > 0)
                //    {
                //        stream.Write(data, 0, data.Length);
                //        stream.Close();
                //    }

                //    //StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);

                //    //streamWriter.WriteLine(hostArr);
                //    //stream.Close();
                //    //stream.Dispose();
                //    //webClient.Dispose();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public string RequestJson(string url, string obj)
        {
            string result = "";
            try
            {
                WebClient webClient = new WebClient();
                webClient.BaseAddress = url;

                try
                {
                    JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                    dynamic json = jsonSer.DeserializeObject(webClient.DownloadString(url));

                    result = json[obj];
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return result;
        }

        public string UpdateCheck()
        {
            string updateurl = "http://moroosoft.azurewebsites.net/Application/HostManager?version=Check";
            string version = "";

            try
            {
                version = RequestJson(updateurl, "version");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return version;
        }
    }
}
