﻿using HostManager.Models;
using HostManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace HostManager.Controllers
{
    public class HostIOController
    {
        /// <summary>
        /// 파일 읽기
        /// </summary>
        /// <returns>string</returns>
        public string HostLoad()
        {
            try
            {
                StreamReader streamReader = new StreamReader(Settings.Default.HostFilePath + @"\Hosts", Encoding.UTF8);
                string host = streamReader.ReadToEnd();
                
                streamReader.Close();

                return host;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }

        /// <summary>
        /// 파일 쓰기
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="path"></param>
        /// <returns>저장여부</returns>
        public bool HostSave(string hosts, string path)
        {
            try
            {
                StreamWriter StreamWriter = null;

                if (path == null)
                {
                    StreamWriter = new StreamWriter(Settings.Default.HostFilePath + @"\Hosts", false, Encoding.UTF8);
                }
                else
                {
                    StreamWriter = new StreamWriter(path, false, Encoding.UTF8);
                }

                StreamWriter.WriteLine(hosts);
                StreamWriter.Close();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void OpenNotepad()
        {
            try
            {
                Process.Start("Notepad.exe", Settings.Default.HostFilePath + @"\Hosts");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}