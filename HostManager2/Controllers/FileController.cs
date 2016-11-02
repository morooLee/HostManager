using HostManager.Models;
using HostManager.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace HostManager.Controllers
{
    public class FileController
    {
        private string HostLoad()
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

        public List<Node> ToNodeList()
        {
            TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
            Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");
            string[] hostArr = HostLoad().Split('\n');
            string lostTxt = "";
            int openNodeCount = 0;
            int closeNodeCount = 0;

            if (hostArr.Length != 0)
            {
                List<Node> nodeList = new List<Node>();
                int nodeDepth = 0;

                for (int i = 0; i < hostArr.Length; i++)
                {
                    #region 노드를 열 때
                    if (hostArr[i].StartsWith("#{{"))
                    {
                        Node node = new Node();
                        hostArr[i] = hostArr[i].Substring(3).Replace("\t", "").Replace("\r", "").Trim();
                        string[] tmpString = hostArr[i].Split('#');

                        node.Header = tmpString[0].Trim();
                        node.IsExternalNode = false;

                        if (tmpString.Length >= 2)
                        {
                            if (tmpString[1] != "")
                            {
                                node.Tooltip = tmpString[1].Trim();
                            }
                        }

                        nodeList.Add(node);
                        nodeDepth++;
                        openNodeCount++;
                    }
                    #endregion

                    #region 호스트일 때
                    else if (regex.IsMatch(hostArr[i]) && hostArr[i].ElementAt(1).ToString() != " ")
                    {
                        Node node = new Node();
                        hostArr[i] = hostArr[i].Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim();
                        string[] tmpString = hostArr[i].Split('#');
                        string ip = regex.Match(hostArr[i]).Value;

                        if (hostArr[i].StartsWith("#"))
                        {
                            node.IsChecked = false;
                            node.Domain = tmpString[1].Replace(ip, "").Trim();

                            if (ip.Length < 16)
                            {
                                node.Header = ip + "\t\t" + node.Domain;
                            }
                            else
                            {
                                node.Header = ip + "\t" + node.Domain;
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
                            node.Domain = tmpString[0].Replace(ip, "").Trim();

                            if (ip.Length < 16)
                            {
                                node.Header = ip + "\t\t" + node.Domain;
                            }
                            else
                            {
                                node.Header = ip + "\t" + node.Domain;
                            }

                            if (tmpString.Length >= 2)
                            {
                                if (tmpString[1] != "")
                                {
                                    node.Tooltip = tmpString[1].Trim();
                                }
                            }
                        }

                        node.IsExternalNode = true;

                        if (nodeDepth == 0)
                        {
                            treeViewItemModel.NodeList.Add(node);
                        }
                        else
                        {
                            nodeList.Last().NodeList.Add(node);
                        }
                    }
                    #endregion

                    #region 노드를 닫을 때
                    else if (hostArr[i].StartsWith("#}}"))
                    {
                        nodeDepth--;
                        int checkCount = 0;

                        if (nodeDepth < 0)
                        {
                            MessageBox.Show("변환에 실패하였습니다.\r\n닫는 명령어가 먼저 입력되었거나 여는 명령어가 없습니다.\r\n텍스트 형식으로 로드합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                        else if (nodeDepth > 0)
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
                                    checkCount++;
                                }
                            }

                            if (tmpNode.IsChecked != null)
                            {
                                if (checkCount == tmpNode.NodeList.Count && tmpNode.NodeList.Count != 0)
                                {
                                    tmpNode.IsChecked = true;
                                }
                                else if (checkCount > 0)
                                {
                                    tmpNode.IsChecked = null;
                                }
                                else if (checkCount == 0)
                                {
                                    tmpNode.IsChecked = false;
                                }
                            }

                            nodeList.Remove(nodeList.Last());
                            nodeList.Last().NodeList.Add(tmpNode);
                            closeNodeCount++;
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
                                    checkCount++;
                                }
                            }

                            if (nodeList.Last().IsChecked != null)
                            {
                                if (checkCount == nodeList.Last().NodeList.Count && nodeList.Last().NodeList.Count != 0)
                                {
                                    nodeList.Last().IsChecked = true;
                                }
                                else if (checkCount > 0)
                                {
                                    nodeList.Last().IsChecked = null;
                                }
                                else if (checkCount == 0)
                                {
                                    nodeList.Last().IsChecked = false;
                                }
                            }

                            treeViewItemModel.NodeList.Add(nodeList.Last());
                            nodeList.Remove(nodeList.Last());
                            closeNodeCount++;
                        }
                    }
                    else
                    {
                        if (hostArr[i].Replace("\r", "").Replace("\n", "").Trim() != "")
                        {
                            lostTxt += (i + 1) + "\t" + hostArr[i];
                        }
                    }
                    #endregion
                }

                if (openNodeCount != closeNodeCount)
                {
                    MessageBox.Show("변환에 실패하였습니다.\r\n항목을 열고 닫는 명령어의 수는 동일해야 합니다.\r\n텍스트 형식으로 로드합니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (treeViewItemModel.NodeList.Count == 0)
                {
                    MessageBox.Show("변환될 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    treeViewItemModel.NodeList.Clear();
                }
                else
                {
                    if (lostTxt != "")
                    {
                        MessageBox.Show("변환 중 형식에 어긋나 포함되지 않은 내역\r\n\r\n줄번호\t텍스트 내용\r\n" + lostTxt, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    foreach (Node Item in treeViewItemModel.NodeList)
                    {
                        Item.Initialize();
                    }
                }

                return treeViewItemModel.NodeList;
            }
            else
            {
                MessageBox.Show("변환될 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return treeViewItemModel.NodeList;
            }
        }
    }
}
