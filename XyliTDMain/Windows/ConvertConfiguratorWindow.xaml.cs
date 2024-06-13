using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XyliTDMain.Dynamic;
using XyliTDMain.Static;
namespace XyliTDMain.Windows
{
    /// <summary>
    /// DownloadConfiguratorWindow.xaml 的交互逻辑
    /// </summary>
    /// 
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public partial class ConvertConfiguratorWindow : Window
    {

        private bool isAnalyzed = false;
        public ConvertConfiguration convertConfiguration;
        public class ConvertConfiguration
        {
            public string url = string.Empty;
            public string filePath = string.Empty;
            public bool isYes = false;
            public MusicInfo MusicInfo;
            public FileStream ncmFile;
        }
        public ConvertConfiguratorWindow(ConvertConfiguration convertConfiguration)
        {
            InitializeComponent();
            this.convertConfiguration = convertConfiguration;
            OutputFileBox.Text = Path.Combine(WorkDirectory.defaultMusicPath,"Audio.mp3");
            YesButton.IsEnabled = false;
            WindowStyle = WindowStyle.SingleBorderWindow;
            UrlBox.TextChanged += UrlBox_TextChanged;
        }   

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(OutputFileBox.Text)!);
                convertConfiguration.isYes = true;
                convertConfiguration.url = UrlBox.Text;
                convertConfiguration.filePath = OutputFileBox.Text;
            } 
            catch
            {
            }
            Close();
        }

        private void BoxInEdit(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Opacity = 1;
        }

        private void BoxOutEdit(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Opacity = 0.8;
        }

        private void ChoseFileButton_Click(object sender, RoutedEventArgs e)
        {
            
            string[] fileType = ["ncm"];
            string filter = string.Join(";", fileType.Select(x => "*." + x));
            string filterString = $"ncm文件 ({filter})|{filter}";
            OpenFileDialog openFileDialog = new()
            {
                Filter = filterString,
                DefaultDirectory = null
            };
            if (openFileDialog.ShowDialog() == true)
            {
                UrlBox.Text = openFileDialog.FileName;
            }
        }

        private void ChoseOutputButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                DefaultDirectory = WorkDirectory.defaultMusicPath,
            };
            if (convertConfiguration.MusicInfo != null) 
            {
                saveFileDialog.FileName = convertConfiguration.MusicInfo.musicName;
                string[] fileType = [convertConfiguration.MusicInfo.format!];
                string filter = string.Join(";", fileType.Select(x => "*." + x));
                string filterString = $"ncm文件 ({filter})|{filter}";
                saveFileDialog.Filter = filterString;
            };
            if (saveFileDialog.ShowDialog() == true) 
            {
                OutputFileBox.Text = saveFileDialog.FileName;
            }
        }

        private void UrlBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isAnalyzed = false;
            YesButton.IsEnabled = false;
            MusicTitle.Content = "=等待解析=";
            Artist.Content = "-----";
            string filePath = UrlBox.Text;
            if (File.Exists(filePath)) 
            {
                try 
                {
                    (convertConfiguration.MusicInfo, convertConfiguration.ncmFile) = ConversionTask.Analysis(filePath);
                    isAnalyzed = true;
                    MusicInfo mi = convertConfiguration.MusicInfo;
                    OutputFileBox.Text = Path.Combine(WorkDirectory.defaultMusicPath, mi.musicName! + "." + mi.format);
                    MusicTitle.Content = mi.musicName;
                    Artist.Content = mi.artistString;
                    YesButton.IsEnabled = true;
                } 
                catch(Exception ex)
                {
                    MusicTitle.Content = "解析出错";
                    Artist.Content = ex.Message;
                }
                
            }
        }

        
    }
}
