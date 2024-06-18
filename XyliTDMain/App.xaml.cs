using System.Configuration;
using System.Data;
using System.Windows;
using XyliTDMain.Static;

namespace XyliTDMain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override  void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WorkDirectory.CreatDir();
            LoadInitSetting();
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
        private static void LoadInitSetting() 
        {
            GlobalContent.MysongsPath.Add("C:\\Users\\99568\\Music\\");
        }
    }

}
