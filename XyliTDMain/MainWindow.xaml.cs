
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using XyliTD.Static;
using XyliTDMain.Pages;
using XyliTDMain.Static;

namespace XyliTDMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UISetting_Initialize();
            GlobalContent.MainWindow = this;
            GlobalContent.HomePage = new HomePage();
            GotoPage(GlobalContent.HomePage);
            Refresh();

        }

        private void GotoPage(Page page) 
        {
            Animations.FrameMoving(PageContent, 100,30);
            Animations.ChangeOP(PageContent, 0, 1, 0.3);
            PageContent.Navigate(page);
            Dictionary<Type, int> pageAndSilderLocation = new() 
            {
                { typeof(HomePage),5},
                { typeof(MysongPage),55},
                { typeof(AboutPage),505}
            };
            foreach (var item in pageAndSilderLocation.Where(item => page.GetType() == item.Key))
            {
                Animations.PageSilderMoveing(MenuSlider, item.Value);
                break;
            }
        }

        private void UISetting_Initialize() 
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            RefreshButton.Click += ((s, e) => { Refresh(); });
            VolumeSlider.Value = MediaPlayerController.MediaPlayer.Volume;
            StartAnimation();
        }
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinsizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalContent.HomePage ??= new HomePage();
            if (Equals(PageContent.Content, GlobalContent.HomePage)) return;
            GotoPage(GlobalContent.HomePage);
        }
        private void MySongButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalContent.MysongPage ??= new MysongPage();
            if (Equals(PageContent.Content, GlobalContent.MysongPage)) return;
            GotoPage(GlobalContent.MysongPage);
        }

        private void AboutPageButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalContent.AboutPage ??= new AboutPage();
            if (Equals(PageContent.Content, GlobalContent.AboutPage)) return;
            GotoPage(GlobalContent.AboutPage);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerController.PlayAudio();
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {

            if (GlobalContent.SongList.Count == 0) return;
            var index = GlobalContent.SongList.IndexOf(MediaPlayerController.CurrentSongPath);
            index = (index == 0) ? GlobalContent.SongList.Count - 1 : index - 1;
            MediaPlayerController.LoadMusic(GlobalContent.SongList[index]);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

            if (GlobalContent.SongList.Count == 0) return;
            int index = GlobalContent.SongList.IndexOf(MediaPlayerController.CurrentSongPath);
            index = (index == GlobalContent.SongList.Count - 1) ? 0 : index + 1;
            MediaPlayerController.LoadMusic(GlobalContent.SongList[index]);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var percent = (int)AudioTimeSlider.Value;
            if (MediaPlayerController.MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                double totalSeconds = MediaPlayerController.TotalTime;
                var newSeconds = totalSeconds * percent / 100.0;
                MediaPlayerController.MediaPlayer.Position = TimeSpan.FromSeconds(newSeconds);
            }
            MediaPlayerController.IsControlling = false;
        }
        private static void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            MediaPlayerController.IsControlling = true;
        }

        private void AudioTimeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            var thumb = (Track)AudioTimeSlider.Template.FindName("PART_Track", AudioTimeSlider);
            thumb.Thumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
            thumb.Thumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
        }

        private void AudioTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!MediaPlayerController.MediaPlayer.NaturalDuration.HasTimeSpan ||
                !MediaPlayerController.IsControlling) return;
            var percent = (int)AudioTimeSlider.Value;
            var totalTime = (int)MediaPlayerController.TotalTime;
            var currentTime = totalTime * percent / 100;
            var totalTimes = MediaPlayerController.ToMinute(totalTime);
            var currentTimes = MediaPlayerController.ToMinute(currentTime);
            GlobalContent.MainWindow.AudioTimeLabel.Content = $"{currentTimes[0]}:{currentTimes[1]:D2}/{totalTimes[0]}:{totalTimes[1]:D2}";
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayerController.MediaPlayer.Volume = VolumeSlider.Value;
            SettingManager.EditSetting(XyliTD.Static.Section.MediaPlayer,"volume", VolumeSlider.Value.ToString());
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var filePath = ((string[])e.Data.GetData(DataFormats.FileDrop)!)[0];
            MediaPlayerController.LoadMusic(filePath);
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

        private static async void Refresh()
        {
            GlobalContent.SongList.Clear();
            GlobalContent.MysongPage.ScrollWarp.Children.Clear();
            await Task.Run(GlobalContent.Function.SearchforSong);
            GlobalContent.MysongPage.InitializeUI();
        }
    }
}