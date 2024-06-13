using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyliTDMain.Windows;
using static XyliTDMain.Windows.ConvertConfiguratorWindow;

namespace XyliTDMain.Static
{
    public class XyMessageBox
    {
        public static ConvertConfiguration ShowDownloadConfiguratorWindow() 
        {
            ConvertConfiguration convertConfiguration = new();
            ConvertConfiguratorWindow convertConfiguratorWindow = new(convertConfiguration) 
            {
                Owner = GlobalContent.MainWindow
            };
            convertConfiguratorWindow.ShowDialog();
            return convertConfiguration;
        }
    }
}
