using HostManager.Models;
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
        /// 기본 파일 읽기
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
        /// 선택한 파일 읽기
        /// </summary>
        /// <param name="Path">파일 경로</param>
        /// <returns>string</returns>
        public string HostLoad(string path)
        {
            try
            {
                StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
                string host = streamReader.ReadToEnd();

                streamReader.Close();

                return host;
            }
            catch (Exception e)
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

        public string FileLoad()
        {
            string path = "";
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            // Set filter for file extension and default file extension
            //dlg.DefaultExt = ".txt";
            dlg.Filter = "모든파일 (*.*)|*.*|텍스트 파일 (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            System.Windows.Forms.DialogResult dlgResult = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (dlgResult == System.Windows.Forms.DialogResult.OK)
            {
                path = dlg.FileName;
            }

            return path;
        }

        public void OpenNotepad(string path)
        {
            try
            {
                Process.Start("Notepad.exe", path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenFolder(string path)
        {
            string[] tmpArr = path.Split('\\');
            string fileName = "";

            if(path.EndsWith(tmpArr.Last()))
            {
                fileName = path.Replace(tmpArr.Last(), "");
            }

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = fileName;
            process.Start();
        }
    }
}
