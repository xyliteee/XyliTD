using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace XyliTDMain.Static
{
    public static class MediaPlayerController
    {
        public static readonly MediaPlayer MediaPlayer = new() 
        {
            Volume = 1
        };
        public static bool IsPlayingAudio { get; set; } = false;
        public static bool IsControlling { get; set; } = false;
        public static int TotalTime = 0;
        public static DispatcherTimer Timer { get; set; }
        public static void LoadMusic(string filePath,string name,string artist)
        {
            MediaPlayer.MediaOpened += (sender, e) =>
            {
                if (MediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    TotalTime = (int)MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    IsPlayingAudio = false;
                    GlobalContent.MainWindow.MusicTitle.Content = name + "--" + artist;
                    GlobalContent.MainWindow.AudioTimeSlider.IsEnabled = true;
                    Timer = new()
                    {
                        Interval = TimeSpan.FromSeconds(0.5),
                    };
                    Timer.Tick += new((sender, e) => { UpdateUI(); });
                    Timer.Start();
                    PlayAudio();
                }
            };
            MediaPlayer.Open(new Uri(filePath));

        }
        public static int[] ToMinute(int second)
        {
            int[] time =
            [
                second / 60, 
                    second % 60,
                ];
            return time;
        }
        private static void UpdateUI()
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan && !IsControlling)
            {
                int percent = (int)(MediaPlayer.Position.TotalSeconds * 100 / MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                int[] totalTime = ToMinute(TotalTime);
                int[] currentTime = ToMinute((int)MediaPlayer.Position.TotalSeconds);
                GlobalContent.MainWindow.AudioTimeSlider.Value = percent;
                GlobalContent.MainWindow.AudioTimeLabel.Content = $"{currentTime[0]}:{currentTime[1]:D2}/{totalTime[0]}:{totalTime[1]:D2}";
            }
        }

        public static void PlayAudio()
        {
            if (MediaPlayer.Source == null) return;
            BitmapImage bitmapImage;
            if (IsPlayingAudio)
            {
                Timer.Stop();
                MediaPlayer.Pause();
                IsPlayingAudio = false;
                bitmapImage = new(new Uri("pack://application:,,,/Image/Icons/Play.png"));
            }
            else
            {
                Timer.Start();
                MediaPlayer.Play();
                IsPlayingAudio = true;
                bitmapImage = new(new Uri("pack://application:,,,/Image/Icons/Pause.png"));
            }
            GlobalContent.MainWindow.PlayImage.Source = bitmapImage;
        }
    }
}
