using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using XyliTD.Static;
using XyliTDMain.Static;

namespace XyliTDMain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WorkDirectory.CreatDir();
            WriteInitSetting();
            LoadInitSetting();
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
        private static void WriteInitSetting() 
        {
            bool? isFirstUse = SettingManager.ReadSettingBool(Section.Applicaiton, "is_first_use");
            isFirstUse ??= true;
            if (!(bool)isFirstUse) return;
            SettingManager.CreatDefaultSetting();
            SettingManager.EditSetting(Section.Applicaiton, "is_first_use", "false");
        }
        private static void LoadInitSetting() 
        {
            GlobalContent.MysongsPath.Add($"C:\\Users\\{Environment.UserName}\\Music\\");
            double? volume = SettingManager.ReadSettingDouble(Section.MediaPlayer, "Volume", 0, 1);
            if (volume == null) 
            {
                volume = 1;
                SettingManager.EditSetting(Section.MediaPlayer, "Volume", "1");
            }
            MediaPlayerController.MediaPlayer.Volume = (double)volume;
        }
    }

}
