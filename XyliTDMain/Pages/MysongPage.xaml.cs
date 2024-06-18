using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XyliTDMain.Static;

namespace XyliTDMain.Pages
{
    /// <summary>
    /// MysongPage.xaml 的交互逻辑
    /// </summary>
    public partial class MysongPage : Page
    {
        public MysongPage()
        {
            InitializeComponent();
        }

        public void InitializeUI() 
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(async() => 
                {
                    ScrollWarp.Children.Remove(TestCard);
                    await Task.Delay(300);
                    foreach (string songFilePath in GlobalContent.SongList)
                    {
                        CreatCard(songFilePath);
                        await Task.Delay(100);
                    }
                },DispatcherPriority.Background);
            });
            
        }
        
        private void CreatCard(string filePath) 
        {
            var musicFile = TagLib.File.Create(filePath);
            Canvas canvas = new() { Width = 185, Height = 185 };

            Border border = new()
            {
                Background = (LinearGradientBrush)FindResource("BlueLight_To_Green"),
                Width = 155,
                Height = 155,
            };
            Canvas.SetLeft(border, 15);
            Canvas.SetTop(border, 15);
            border.Effect = new DropShadowEffect { ShadowDepth = 0, Color = Colors.Gray, BlurRadius = 5 };

            BitmapImage bi = new();
            var ms = GlobalContent.Function.GetCover(musicFile);
            bi.BeginInit();
            bi.DecodePixelHeight = 155;
            bi.DecodePixelWidth = 155;
            if (ms != null){bi.StreamSource = ms;}
            else {bi.UriSource = new Uri("pack://application:,,,/Image/Icons/SongImage.png");}
            bi.EndInit();
            Image topImage = new()
            {
                Width = 155,
                Height = 155,
                Source = bi,
                Clip = new RectangleGeometry(new Rect(0, 0, 155, 105))
            };
            Canvas.SetLeft(topImage, 15);
            Canvas.SetTop(topImage, 15);

            Image bottomImage = new()
            {
                Width = 155,
                Height = 155,
                Source = bi,
                Clip = new RectangleGeometry(new Rect(0, 105, 155, 50)),
                Effect = new BlurEffect { Radius = 10 }
            };
            Canvas.SetLeft(bottomImage, 15);
            Canvas.SetTop(bottomImage, 15);


            Button button = new()
            {
                Opacity = 0,
                Height = 155,
                Width = 155,
                Cursor = Cursors.Hand
            };
            button.Click += ((s,e) => 
            {
                MediaPlayerController.LoadMusic(filePath);
            });

            button.MouseEnter += ((s,e) => 
            {
                Animations.Scale(canvas, null, 1.05, 0.1);
            });
            button.MouseLeave += ((s, e) =>
            {
                Animations.Scale(canvas, 1.05, 1, 0.1);
            });

            Canvas.SetLeft(button, 15);
            Canvas.SetTop(button, 15);


            Border bottomBorder = new()
            {
                Height = 50,
                Width = 155,
                Background = (SolidColorBrush)FindResource("Xy_Blue_Dark_Brush"),
                Opacity = 0.4,
            };
            Canvas.SetLeft(bottomBorder, 15);
            Canvas.SetBottom(bottomBorder, 15);

            string name = musicFile.Tag.Title;
            name ??= System.IO.Path.GetFileName(filePath);

            TextBlock textBlock = new()
            {
                Background = Brushes.Transparent,
                Height = 40,
                Width = 155,
                Text = name,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16
            };
            Canvas.SetLeft(textBlock, 15);
            Canvas.SetBottom(textBlock, 20);

            // 将所有元素添加到 Canvas
            canvas.Children.Add(border);
            canvas.Children.Add(topImage);
            canvas.Children.Add(bottomImage);
            canvas.Children.Add(bottomBorder);
            canvas.Children.Add(textBlock);
            canvas.Children.Add(button);
            ScrollWarp.Children.Add(canvas);
            Animations.ChangeOP(canvas,0,1,0.1);
        }
    }
}
