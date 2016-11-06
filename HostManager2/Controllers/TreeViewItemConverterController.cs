using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace HostManager.Controllers
{
    public class TreeViewItemConverterController
    {
        private HostIOController hostIOController = new HostIOController(); 

        /// <summary>
        /// string 형태의 hosts 내용을 Node로 변환하여 리스트 만들기
        /// </summary>
        /// <param name="hosts">변경할 string</param>
        /// <returns>List<Node></returns>
        public List<Node> ConverterToNodeList(string hosts)
        {
            TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
            Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");
            int openNodeCount = 0;
            int closeNodeCount = 0;
            string lostTxt = "";
            string[] hostArr = null;

            if (hosts == null)
            {
                hostArr = hostIOController.HostLoad().Split('\n');
            }
            else
            {
                hostArr = hosts.Split('\n');
            }

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
                    return null;
                }
                else if (treeViewItemModel.NodeList.Count == 0)
                {
                    MessageBox.Show("호스트 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("호스트 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return treeViewItemModel.NodeList;
            }
        }

        /// <summary>
        /// TreeViewItemModel에서 Node 형태의 hosts 내용을 string로 변환하기
        /// (저장에 실패하였을 경우 null값 반환)
        /// </summary>
        /// <param name="treeViewItemModel">변환할 TreeViewItemModel</param>
        /// <param name="path">다른 이름으로 저장 시의 경로</param>
        /// <param name="isSave">저장 여부</param>
        /// <returns>저장에 실패하였을 경우 null값 반환</returns>
        public string ConverterToString(TreeViewItemModel treeViewItemModel, string path, bool isSave)
        {
            string hosts = "";

            foreach (Node node in treeViewItemModel.NodeList)
            {
                hosts += SetString(node);
            }

            if (isSave)
            {
                if(hostIOController.HostSave(hosts, path) == false)
                {
                    return null;
                }
            }

            return hosts;
        }

        /// <summary>
        /// Node를 string으로 변환하기
        /// </summary>
        /// <param name="node">string으로 변환할 Node</param>
        /// <returns>string</returns>
        private string SetString(Node node)
        {
            string hosts = "";

            if (node.IsExternalNode)
            {
                if (node.Tooltip == null || node.Tooltip == "")
                {
                    if (node.IsChecked == true)
                    {
                        hosts += (node.Header + "\r\n");
                    }
                    else
                    {
                        hosts += ("#" + node.Header + "\r\n");
                    }
                }
                else
                {
                    if (node.IsChecked == true)
                    {
                        hosts += (node.Header + "\t\t#" + node.Tooltip + "\r\n");
                    }
                    else
                    {
                        hosts += ("#" + node.Header + "\t\t#" + node.Tooltip + "\r\n");
                    }
                }
            }
            else
            {
                if (node.Tooltip == null || node.Tooltip == "")
                {
                    hosts += ("#{{" + node.Header + "\r\n");
                }
                else
                {
                    hosts += ("#{{" + node.Header + "\t\t#" + node.Tooltip + "\r\n");
                }

                if (node.NodeList != null || node.NodeList.Count == 0)
                {
                    foreach (Node childNode in node.NodeList)
                    {
                        hosts += SetString(childNode);
                    }
                }

                hosts += "#}}\r\n";

                if (node.ParentNode != null)
                {
                    if (node.ParentNode.NodeList.IndexOf(node) != (node.ParentNode.NodeList.Count - 1))
                    {
                        hosts += "\r\n";
                    }
                }
                else
                {
                    hosts += "\r\n";
                    hosts += "\r\n";
                }
            }

            return hosts;
        }
    }
}
