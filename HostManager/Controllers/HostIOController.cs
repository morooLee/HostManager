using HostManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HostManager.Controllers
{
    public class HostIOController
    {
        public TreeViewModel HostLoad()
        {
            try
            {
                TreeViewModelController treeViewModelController = new TreeViewModelController();
                StreamReader streamReader = new StreamReader(@"C:\Windows\System32\drivers\etc\Hosts");

                return treeViewModelController.ConverterTreeViewModel(streamReader.ReadToEnd());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Moroo | Host Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        } 
    }
}
