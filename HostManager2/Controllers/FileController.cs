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
            string LostTxt = "";
            int OpenNode = 0;
            int CloseNode = 0;

            if (hostArr.Length != 0)
            {
                List<Node> tmpNodeList = new List<Node>();
                int NodeDepth = 0;

                for (int i = 0; i < hostArr.Length; i++)
                {
                    #region 노드를 열 때
                    if (hostArr[i].StartsWith("#{{"))
                    {
                        Node node = new Node();
                        hostArr[i] = hostArr[i].Substring(3).Replace("\t", "").Replace("\r", "").Trim();
                        string[] tmpString = hostArr[i].Split('#');

                        node.Header = tmpString[0].Trim();

                        if (tmpString.Length >= 2)
                        {
                            if (tmpString[1] != "")
                            {
                                node.Tooltip = tmpString[1].Trim();
                            }
                        }

                        tmpNodeList.Add(node);
                        NodeDepth++;
                        OpenNode++;
                    }
                    #endregion

                    #region 호스트일 때
                    else if (regex.IsMatch(hostArr[i]) && hostArr[i].ElementAt(1).ToString() != " ")
                    {
                        Node node = new Node();
                        hostArr[i] = hostArr[i].Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim();
                        string[] tmpString = hostArr[i].Split('#');
                        string IP = regex.Match(hostArr[i]).Value;
                        string Domain = tmpString[1].Replace(IP, "").Trim();

                        if (hostArr[i].StartsWith("#"))
                        {
                            node.Check = false;

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + Domain;
                            }
                            else
                            {
                                node.Header = IP + "\t" + Domain;
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
                            node.Check = true;
                            Domain = tmpString[0].Replace(IP, "").Trim();

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + Domain;
                            }
                            else
                            {
                                node.Header = IP + "\t" + Domain;
                            }

                            if (tmpString.Length >= 2)
                            {
                                if (tmpString[1] != "")
                                {
                                    node.Tooltip = tmpString[1].Trim();
                                }
                            }
                        }

                        node.NodeList = null;

                        if (NodeDepth == 0)
                        {
                            treeViewItemModel.NodeList.Add(node);
                        }
                        else
                        {
                            tmpNodeList.Last().NodeList.Add(node);
                        }
                    }
                    #endregion

                    #region 노드를 닫을 때
                    else if (hostArr[i].StartsWith("#}}"))
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
                            tmpNode = tmpNodeList.Last();

                            foreach (Node item in tmpNode.NodeList)
                            {
                                if (item.Check == null)
                                {
                                    tmpNode.Check = null;
                                    break;
                                }
                                else if (item.Check == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (tmpNode.Check != null)
                            {
                                if (CheckCount == tmpNode.NodeList.Count && tmpNode.NodeList.Count != 0)
                                {
                                    tmpNode.Check = true;
                                }
                                else if (CheckCount > 0)
                                {
                                    tmpNode.Check = null;
                                }
                                else if (CheckCount == 0)
                                {
                                    tmpNode.Check = false;
                                }
                            }

                            tmpNodeList.Remove(tmpNodeList.Last());
                            tmpNodeList.Last().NodeList.Add(tmpNode);
                            CloseNode++;
                        }
                        else
                        {
                            foreach (Node item in tmpNodeList.Last().NodeList)
                            {
                                if (item.Check == null)
                                {
                                    tmpNodeList.Last().Check = null;
                                    break;
                                }
                                else if (item.Check == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (tmpNodeList.Last().Check != null)
                            {
                                if (CheckCount == tmpNodeList.Last().NodeList.Count && tmpNodeList.Last().NodeList.Count != 0)
                                {
                                    tmpNodeList.Last().Check = true;
                                }
                                else if (CheckCount > 0)
                                {
                                    tmpNodeList.Last().Check = null;
                                }
                                else if (CheckCount == 0)
                                {
                                    tmpNodeList.Last().Check = false;
                                }
                            }

                            treeViewItemModel.NodeList.Add(tmpNodeList.Last());
                            tmpNodeList.Remove(tmpNodeList.Last());
                            CloseNode++;
                        }
                    }
                    else
                    {
                        if (hostArr[i].Replace("\r", "").Replace("\n", "").Trim() != "")
                        {
                            LostTxt += (i + 1) + "\t" + hostArr[i];
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

                treeViewItemModel.NodeList.Clear();
                return treeViewItemModel.NodeList;
            }

            if (treeViewItemModel.NodeList == null || treeViewItemModel.NodeList.Count == 0)
            {
                MessageBox.Show("변환된 내용이 없습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                treeViewItemModel.NodeList.Clear();
                return treeViewItemModel.NodeList;
            }
            else
            {
                foreach (Node Item in treeViewItemModel.NodeList)
                {
                    //Item.Initialize();
                }
                return treeViewItemModel.NodeList;
            }
        }
    }
}
