using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostManager.Controllers
{
    public class BrowserController
    {
        public void CheckDoBrowsers()
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
                    pathIE = item.MainModule.FileName;
                    isIE = true;
                    tmpList.Add(item);
                }
                else if (item.ProcessName == "MicrosoftEdge")
                {
                    pathEdge = item.MainModule.FileName;
                    isEdge = true;
                    tmpList.Add(item);
                }
                else if (item.ProcessName == "chrome")
                {
                    pathChrome = item.MainModule.FileName;
                    isChrome = true;
                    tmpList.Add(item);
                }
                else if (item.ProcessName == "firefox")
                {
                    pathFireFox = item.MainModule.FileName;
                    isFireFox = true;
                    tmpList.Add(item);
                }
            }
            foreach (Process _item in tmpList)
            {
                Console.WriteLine(_item.ProcessName);
            }
            foreach (Process _item in tmpList)
            {
                _item.Close();
            }
            if (isIE)
            {
                Console.WriteLine(pathIE);
            }
            if (isEdge)
            {
                Console.WriteLine(pathEdge);
            }
            if (isChrome)
            {
                Console.WriteLine(pathChrome);
            }
            if (isFireFox)
            {
                Console.WriteLine(pathFireFox);
            }
            Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 8");
            Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2");
        }
    }
}
