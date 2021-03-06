﻿using HostManager.Models;
using HostManager.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HostManager.Controllers
{
    public class HostIOController
    {
        private TreeViewModelController treeViewModelController = new TreeViewModelController();

        public TreeViewModel HostLoad()
        {
            try
            {
                TreeViewModel treeViewModel = new TreeViewModel();
                StreamReader streamReader = new StreamReader(Settings.Default.Host_File_Path + @"\Hosts", Encoding.UTF8);

                treeViewModel = treeViewModelController.ConverterToTreeViewModel(streamReader.ReadToEnd());
                streamReader.Close();


                return treeViewModel;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        public TreeViewModel HostLoad(String path)
        {
            try
            {
                TreeViewModel treeViewModel = new TreeViewModel();
                StreamReader streamReader = new StreamReader(path, Encoding.UTF8);

                treeViewModel = treeViewModelController.ConverterToTreeViewModel(streamReader.ReadToEnd());
                streamReader.Close();


                return treeViewModel;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        public String HostToString()
        {
            String hostString = "";

            try
            {
                StreamReader streamReader = new StreamReader(Settings.Default.Host_File_Path + @"\Hosts", Encoding.UTF8);

                hostString = streamReader.ReadToEnd();
                streamReader.Close();


                return hostString;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return hostString;
            }
        }

        public String HostToString(String path)
        {
            String hostString = "";

            try
            {
                StreamReader streamReader = new StreamReader(path, Encoding.UTF8);

                hostString = streamReader.ReadToEnd();
                streamReader.Close();


                return hostString;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return hostString;
            }
        }

        public bool HostSave(TreeViewModel treeViewModel, String path)
        {
            if (treeViewModel != null)
            {
                String HostTxt = "";

                //HostTxt += "# Copyright (c) 1993-2009 Microsoft Corp.\r\n";
                //HostTxt += "#\r\n";
                //HostTxt += "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.\r\n";
                //HostTxt += "#\r\n";
                //HostTxt += "# This file contains the mappings of IP addresses to host names. Each\r\n";
                //HostTxt += "# entry should be kept on an individual line. The IP address should\r\n";
                //HostTxt += "# be placed in the first column followed by the corresponding host name.\r\n";
                //HostTxt += "# The IP address and the host name should be separated by at least one\r\n";
                //HostTxt += "# space.\r\n";
                //HostTxt += "#\r\n";
                //HostTxt += "# Additionally, comments (such as these) may be inserted on individual\r\n";
                //HostTxt += "# lines or following the machine name denoted by a '#' symbol.\r\n";
                //HostTxt += "#\r\n";
                //HostTxt += "# For example:\r\n";
                //HostTxt += "#\r\n";
                //HostTxt += "#      102.54.94.97     rhino.acme.com          # source server\r\n";
                //HostTxt += "#       38.25.63.10     x.acme.com              # x client host\r\n";
                //HostTxt += "\r\n";
                //HostTxt += "# localhost name resolution is handled within DNS itself.\r\n";
                //HostTxt += "\r\n";
                //HostTxt += "\r\n";
                //HostTxt += "# Edited By Moroo Host Manager (Do not delete this line.)\r\n";
                //HostTxt += "\r\n";

                HostTxt = treeViewModelController.ConverterToString(treeViewModel);

                try
                {
                    StreamWriter hostsStreamWriter = null;

                    if (path == null)
                    {
                        hostsStreamWriter = new StreamWriter(Settings.Default.Host_File_Path + @"\Hosts");
                    }
                    else
                    {
                        hostsStreamWriter = new StreamWriter(path);
                    }

                    hostsStreamWriter.WriteLine(HostTxt);
                    hostsStreamWriter.Close();

                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool HostSave(String hostString, String path)
        {
            try
            {
                StreamWriter hostsStreamWriter = null;

                if (path == null)
                {
                    hostsStreamWriter = new StreamWriter(Settings.Default.Host_File_Path + @"\Hosts");
                }
                else
                {
                    hostsStreamWriter = new StreamWriter(path);
                }

                hostsStreamWriter.WriteLine(hostString);
                hostsStreamWriter.Close();

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
                Process.Start("Notepad.exe", Settings.Default.Host_File_Path + @"\Hosts");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenFolder()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Settings.Default.Host_File_Path;
            process.Start();
        }
    }
}
