using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HostManager.Controllers
{
    class TreeViewModelController
    {
        Regex regex = new Regex(@"((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]|[0-9])");

        public TreeViewModel ConverterToTreeViewModel(String hostTxt)
        {
            TreeViewModel treeViewModel = new TreeViewModel();
            String[] tmpHostTxt = hostTxt.Split('\n');

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
                    }
                    #endregion

                    #region 호스트일 때
                    else if (regex.IsMatch(tmpHostTxt[i]) && tmpHostTxt[i].ElementAt(1).ToString() != " ")
                    {
                        Node node = new Node();
                        String IP = regex.Match(tmpHostTxt[i]).Value;
                        tmpHostTxt[i] = tmpHostTxt[i].Replace("\t", "").Replace("\r", "").Trim();
                        String[] tmpString = tmpHostTxt[i].Split('#');

                        if (tmpHostTxt[i].StartsWith("#"))
                        {
                            node.IsChecked = false;

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + tmpString[1].Replace(IP, "").Trim();
                            }
                            else
                            {
                                node.Header = IP + "\t" + tmpString[1].Replace(IP, "").Trim();
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

                            if (IP.Length < 16)
                            {
                                node.Header = IP + "\t\t" + tmpString[0].Replace(IP, "").Trim();
                            }
                            else
                            {
                                node.Header = IP + "\t" + tmpString[0].Replace(IP, "").Trim();
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

                        if (NodeDepth > 0)
                        {
                            Node tmpNode = new Node();
                            tmpNode = nodeList.Last();

                            foreach (Node item in tmpNode.NodeList)
                            {
                                if (item.IsChecked == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (CheckCount == 0)
                            {
                                tmpNode.IsChecked = false;
                            }
                            else if (CheckCount == tmpNode.NodeList.Count)
                            {
                                tmpNode.IsChecked = true;
                            }
                            else
                            {
                                tmpNode.IsChecked = null;
                            }

                            nodeList.Remove(nodeList.Last());
                            nodeList.Last().NodeList.Add(tmpNode);
                        }
                        else if (NodeDepth == 0)
                        {
                            foreach (Node item in nodeList.Last().NodeList)
                            {
                                if (item.IsChecked == true)
                                {
                                    CheckCount++;
                                }
                            }

                            if (CheckCount == 0)
                            {
                                nodeList.Last().IsChecked = false;
                            }
                            else if (CheckCount == nodeList.Last().NodeList.Count)
                            {
                                nodeList.Last().IsChecked = true;
                            }
                            else
                            {
                                nodeList.Last().IsChecked = null;
                            }

                            treeViewModel.NodeList.Add(nodeList.Last());
                            nodeList.Remove(nodeList.Last());
                        }
                        else
                        {
                            Console.WriteLine("열려 있는 노드가 없는데 닫는 문장이 입력");
                        }
                    }
                    #endregion
                }
            }

            foreach (Node item in treeViewModel.NodeList)
            {
                item.Initialize();
            }
            return treeViewModel;
        }

        public String ConverterToString(TreeViewModel treeViewModel)
        {
            String tmpText = "";

            foreach (Node item in treeViewModel.NodeList)
            {
                tmpText += SetString(item);
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
                if (tmpString != "")
                {
                    if (node.NodeList != null && regex.IsMatch(node.NodeList.FirstOrDefault().Header) == false)
                    {
                        if (tmpString != "")
                        {
                            tmpString += "\r\n";
                            tmpString += "\r\n";
                        }
                    }
                    else if (node.NodeList != null)
                    {
                        tmpString += "\r\n";
                    }
                }

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
            }

            return tmpString;
        }
    }
}
