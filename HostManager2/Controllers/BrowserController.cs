﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HostManager.Properties;

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
    }
}
