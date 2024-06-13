using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System;
using System.Diagnostics;
using XyliTDMain.Static;
using System.IO;
using XyliTDMain.Dynamic;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace XyliTDMain.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            StartAnimation();
            Mask.Visibility = Visibility.Visible;
        }

        private void CreateCard(int index,ConversionTask conversionTask)
        {
            Border border = new()
            {
                Height = 80,
                Width = 330,
                Background = (Brush)FindResource("Xy_Blue_VeryLight_Brush"),
                CornerRadius = new CornerRadius(3),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Gray,
                    ShadowDepth = 0,
                    Direction = 0,
                    Opacity = 0.6,
                    BlurRadius = 2
                }
            };
            Canvas.SetLeft(border, 10);
            Canvas.SetTop(border, 10 + 90 * index);

            Canvas canvas = new()
            {
                Height = 80,
                Width = 330
            };

            Button DialogButton = new()
            {
                Height = 80,
                Width = 330,
                IsEnabled = false,
                Opacity = 0
            };
            DialogButton.Click += ((sender,e) => 
            {
                ShowDiaglog(conversionTask.MusicInfo);
            });

            ProgressBar progressBar = new()
            {
                Width = 320,
                Value = 0,
                Background = Brushes.White,
                Foreground = (LinearGradientBrush)FindResource("BlueLight_To_Green"),
                Style = (Style)FindResource("ProgressBarFlat")
            };
            Canvas.SetLeft(progressBar, 5);
            Canvas.SetBottom(progressBar, 5);

            Image image = new()
            {
                Width = 60,
                Height = 60
            };
            Canvas.SetLeft(image, 5);
            Canvas.SetTop(image, 5);

            Label label1 = new()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = conversionTask.title,
                Width = 250,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444")),
                Height = 25,
                Padding = new Thickness(0)
            };
            Canvas.SetLeft(label1, 70);
            Canvas.SetTop(label1, 5);

            Label label2 = new()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = "Text",
                Width = 180,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777777")),
                Height = 20,
                Padding = new Thickness(0)
            };
            Canvas.SetLeft(label2, 70);
            Canvas.SetTop(label2, 30);

            Label label3 = new()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = "Text",
                Width = 180,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777777")),
                Height = 20,
                Padding = new Thickness(0)
            };
            Canvas.SetLeft(label3, 70);
            Canvas.SetTop(label3, 45);

            Button openMusic = new()
            {
                Width = 30,
                Height = 30,
                BorderThickness = new Thickness(0),
                Opacity = 0.6,
                Background = (LinearGradientBrush)FindResource("BlueLight_To_Green"),
                Cursor = Cursors.Hand,
                IsEnabled = false
            };
            Image openMusicImage = new()
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri("pack://application:,,,/Image/Icons/OpenFile.png")),
                IsHitTestVisible = false
            };

            Canvas.SetRight(openMusic, 10);
            Canvas.SetTop(openMusic, 30);
            Canvas.SetRight(openMusicImage, 15);
            Canvas.SetTop(openMusicImage, 35);

            Button openFileDir = new()
            {
                Width = 30,
                Height = 30,
                BorderThickness = new Thickness(0),
                Opacity = 0.6,
                Background = (LinearGradientBrush)FindResource("BlueLight_To_Green"),
                IsEnabled = false,
                Cursor = Cursors.Hand
            };

            Image openFileImage = new()
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri("pack://application:,,,/Image/Icons/OpenFilePath.png")),
                IsHitTestVisible = false
            };

            Canvas.SetRight(openFileDir, 45);
            Canvas.SetTop(openFileDir, 30);

            Canvas.SetRight(openFileImage, 50);
            Canvas.SetTop(openFileImage, 35);


            openFileDir.Click += new((sender, e) =>
            {
                string filePath = conversionTask.outputFilePath;
                if (!File.Exists(filePath)) return; 
                string directoryPath = Path.GetDirectoryName(filePath)!;
                Process.Start("explorer.exe", directoryPath);
            });

            openMusic.Click += new((sender,e) => 
            {
                string filePath = conversionTask.outputFilePath;
                if (!File.Exists(filePath)) return;
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            });


            canvas.Children.Add(progressBar);
            canvas.Children.Add(image);
            canvas.Children.Add(label1);
            canvas.Children.Add(label2);
            canvas.Children.Add(label3);
            canvas.Children.Add(DialogButton);
            canvas.Children.Add(openMusic);
            canvas.Children.Add(openFileDir);
            canvas.Children.Add(openFileImage);
            canvas.Children.Add(openMusicImage);

            border.Child = canvas;
            ScrollCanvas.Children.Add(border);
            conversionTask.UISingleCard = new(image, label1, label2, label3,progressBar, openFileDir,openMusic, DialogButton);
        }
        private async void ADDTask(ConversionTask conversionTask)
        {
            int index = GlobalContent.conversionTaskList.Count;
            GlobalContent.conversionTaskList.Add(conversionTask);
            CreateCard(index, GlobalContent.conversionTaskList[index]);
            ScrollCanvas.Height = 90 * index;
            try
            {
                await Task.Run(conversionTask.ConvertAsync);
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
        }
        private void ShowDiaglog(MusicInfo musicInfo) 
        {
            Mask.Visibility = Visibility.Collapsed;
            MusicNameLable.Text = $"{musicInfo.musicName}";
            ArtistNameLable.Content = $"作者：{musicInfo.artistString}";
            AlbumLable.Text = $"所属专辑：{musicInfo.album}";
            BitrateLable.Content = $"比特率：{musicInfo.bitrate}bps";
        }

        private void NewConvertButton_Click(object sender, RoutedEventArgs e)
        {
            var convertConfiguration = XyMessageBox.ShowDownloadConfiguratorWindow();
            if (!convertConfiguration.isYes) return;
            ADDTask(new ConversionTask(convertConfiguration.url, convertConfiguration.filePath,convertConfiguration.MusicInfo,convertConfiguration.ncmFile));
        }

        private void StartAnimation()
        {
            ColorAnimation animation1 = new()
            {
                From = (Color)FindResource("Xy_Blue"),
                To = (Color)FindResource("Xy_Green"),
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            ColorAnimation animation2 = new()
            {
                From = (Color)FindResource("Xy_Green"),
                To = (Color)FindResource("Xy_Blue"),
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            GradientStop1.BeginAnimation(GradientStop.ColorProperty, animation1);
            GradientStop2.BeginAnimation(GradientStop.ColorProperty, animation2);
        }
    }
}
