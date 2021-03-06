﻿using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HostManager.Controllers
{
    class TreeViewModelController
    {
        private Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");

        public TreeViewModel ConverterToTreeViewModel(String hostTxt)
        {
            TreeViewModel treeViewModel = new TreeViewModel();
            String[] tmpHostTxt = hostTxt.Split('\n');
            String LostTxt = "";
            int OpenNode = 0;
            int CloseNode = 0;

            if (tmpHostTxt.Length != 0)
            {
                List<Node> nodeList = new List<Node>();
                int NodeDepth = 0;

                for (int i = 0; i < tmpHostTxt.Length; i++)
                {
                    #region 노드를 열 때
                    if (tmpHostTxt[i].StartsWith("#{{"))
                    {
                        Node node = new Node();
                        tmpHostTxt[i] = tmpHostTxt[i].Substring(3).Replace("\t", "").Replace("\r", "").Trim();
                        String[] tmpString = tmpHostTxt[i].Split('#');

                        node.Header = tmpString[0].Trim();

                        if (tmpString.Length >= 2)
                        {
                            if (tmpString[1] != "")
                            {
                                node.Tooltip = tmpString[1].Trim();
                            }
                        }

                        nodeList.Add(node);
                        NodeDepth++;
                        OpenNode++;
                    }
                    #endregion

                    #region 호스트일 때
                    else if (regex.IsMatch(tmpHostTxt[i]) && tmpHostTxt[i].ElementAt(1).ToString() != " ")
                    {
                        Node node = new Node();
                        String IP = regex.Match(tmpHostTxt[i]).Value;
                        tmpHostTxt[i] = tmpHostTxt[i].Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim();
                        String[] tmpString = tmpHostTxt[i].Split('#');

                        if (tmpHostTxt[i].StartsWith("#"))
                        {
                            node.IsChecked = false;
                            node.Domain = tmpString[1].Replace(IP, "").Trim();

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + node.Domain;
                            }
                            else
                            {
                                node.Header = IP + "\t" + node.Domain;
                            }

                            if (tmpString.Length >= 3)
                            {
                                if (tmpString[2] != "")
                                {
                                    node.Tooltip = tmpString[2].Trim();
                                }
                            }
                        }
                        else
                        {
                            node.IsChecked = true;
                            node.Domain = tmpString[0].Replace(IP, "").Trim();

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + node.Domain;
                            }
                            else
                            {
                                node.Header = IP + "\t" + node.Domain;
                            }

                            if (tmpString.Length >= 2)
                            {
                                if (tmpString[1] != "")
                                {
                                    node.Tooltip = tmpString[1].Trim();
                                }
                            }
                        }

                        node.IsLastNode = true;
                        node.NodeList = null;

                        if (NodeDepth == 0)
                        {
                            treeViewModel.NodeList.Add(node);
                        }
                        else
                        {
                            nodeList.Last().NodeList.Add(node);
                        }
                    }
                    #endregion

                    #region 노드를 닫을 때
                    else if (tmpHostTxt[i].StartsWith("#}}"))
                    {
                        NodeDepth--;
                        int CheckCount = 0;

                        if (NodeDepth < 0)
                        {
                            MessageBox.Show("변환에 실패하였습니다.\r\n닫는 명령어가 먼저 입력되었거나 여는 명령어가 없습니다.\r\n텍스트 형식으로 로드합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                        else if (NodeDepth > 0)
                        {
                            Node tmpNode = new Node();
                            tmpNode = nodeList.Last();

                            foreach (Node item in tmpNode.NodeList)
                            {
                                if (item.IsChecked == null)
                                {
                                    tmpNode.IsChecked = null;
                                    break;
                                }
                                else if (item.IsChecked == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (tmpNode.IsChecked != null)
                            {
                                if (CheckCount == tmpNode.NodeList.Count && tmpNode.NodeList.Count != 0)
                                {
                                    tmpNode.IsChecked = true;
                                }
                                else if (CheckCount > 0)
                                {
                                    tmpNode.IsChecked = null;
                                }
                                else if (CheckCount == 0)
                                {
                                    tmpNode.IsChecked = false;
                                }
                            }

                            nodeList.Remove(nodeList.Last());
                            nodeList.Last().NodeList.Add(tmpNode);
                            CloseNode++;
                        }
                        else
                        {
                            foreach (Node item in nodeList.Last().NodeList)
                            {
                                if (item.IsChecked == null)
                                {
                                    nodeList.Last().IsChecked = null;
                                    break;
                                }
                                else if (item.IsChecked == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (nodeList.Last().IsChecked != null)
                            {
                                if (CheckCount == nodeList.Last().NodeList.Count && nodeList.Last().NodeList.Count != 0)
                                {
                                    nodeList.Last().IsChecked = true;
                                }
                                else if (CheckCount > 0)
                                {
                                    nodeList.Last().IsChecked = null;
                                }
                                else if (CheckCount == 0)
                                {
                                    nodeList.Last().IsChecked = false;
                                }
                            }

                            treeViewModel.NodeList.Add(nodeList.Last());
                            nodeList.Remove(nodeList.Last());
                            CloseNode++;
                        }
                    }
                    else
                    {
                        if (tmpHostTxt[i].Replace("\r", "").Replace("\n", "").Trim() != "")
                        {
                            LostTxt += (i + 1) +"\t" + tmpHostTxt[i];
                        }
                    }
                    #endregion
                }

                if (OpenNode != CloseNode)
                {
                    MessageBox.Show("변환에 실패하였습니다.\r\n항목을 열고 닫는 명령어의 수는 같아야 합니다.\r\n텍스트 형식으로 로드합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                else if (LostTxt != "")
                {
                    MessageBox.Show("변환 중 형식에 어긋나 포함되지 않은 내역\r\n\r\n줄번호\t텍스트 내용\r\n" + LostTxt, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("변환된 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                treeViewModel.NodeList.Clear();
                return treeViewModel;
            }

            if (treeViewModel == null || treeViewModel.NodeList.Count == 0)
            {
                MessageBox.Show("변환된 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                treeViewModel.NodeList.Clear();
                return treeViewModel;
            }
            else
            {
                foreach (Node Item in treeViewModel.NodeList)
                {
                    Item.Initialize();
                }
                return treeViewModel;
            }
        }

        public String ConverterToString(TreeViewModel treeViewModel)
        {
            String tmpText = "";

            if (treeViewModel.NodeList.Count != 0)
            {
                foreach (Node item in treeViewModel.NodeList)
                {
                    tmpText += SetString(item);
                }
            }

            return tmpText;
        }

        public String ConverterToString(Node node)
        {
            String tmpText = "";
            tmpText += SetString(node);

            return tmpText;
        }

        private String SetString(Node node)
        {
            String tmpString = "";

            if (regex.IsMatch(node.Header))
            {
                if (node.IsChecked == true)
                {
                    if (node.Tooltip == null || node.Tooltip == "")
                    {
                        tmpString += (node.Header + "\r\n");
                    }
                    else
                    {
                        tmpString += (node.Header + "\t\t#" + node.Tooltip + "\r\n");
                    }
                }
                else
                {
                    if (node.Tooltip == null || node.Tooltip == "")
                    {
                        tmpString += ("#" + node.Header + "\r\n");
                    }
                    else
                    {
                        tmpString += ("#" + node.Header + "\t\t#" + node.Tooltip + "\r\n");
                    }
                }
            }
            else
            {
                if (node.Tooltip == null || node.Tooltip == "")
                {
                    tmpString += ("#{{" + node.Header + "\r\n");
                }
                else
                {
                    tmpString += ("#{{" + node.Header + "\t\t#" + node.Tooltip + "\r\n");
                }

                if (node.NodeList != null || node.NodeList.Count == 0)
                {
                    foreach (Node _node in node.NodeList)
                    {
                        tmpString += SetString(_node);
                    }
                }

                tmpString += "#}}\r\n";

                if (node.ParentNode != null)
                {
                    if (node.ParentNode.NodeList.IndexOf(node) != (node.ParentNode.NodeList.Count - 1))
                    {
                        tmpString += "\r\n";
                    }
                }
                else
                {
                    tmpString += "\r\n";
                    tmpString += "\r\n";
                }
            }

            return tmpString;
        }
    }
}