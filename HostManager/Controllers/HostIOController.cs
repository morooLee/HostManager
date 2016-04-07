using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        TreeViewModelController treeViewModelController = new TreeViewModelController();

        public TreeViewModel HostLoad()
        {
            try
            {
                TreeViewModel treeViewModel = new TreeViewModel();
                StreamReader streamReader = new StreamReader(@"C:\Windows\System32\drivers\etc\Hosts");

                treeViewModel = treeViewModelController.ConverterTreeViewModelFromString(streamReader.ReadToEnd());
                streamReader.Close();

                return treeViewModel;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Moroo | Host Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        public bool HostSave(TreeViewModel treeViewModel)
        {
            if (treeViewModel != null)
            {
                String HostTxt = treeViewModelController.ConverterStringFromTreeViewModel(treeViewModel);

                try
                {
                    StreamWriter hostsStreamWriter = new StreamWriter(@"C:\Windows\System32\drivers\etc\Hosts");

                    hostsStreamWriter.WriteLine(HostTxt);
                    hostsStreamWriter.Close();

                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Moroo | Host Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
