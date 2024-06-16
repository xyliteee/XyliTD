using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            GlobalContent.HomePage = new();
            GotoPage(GlobalContent.HomePage);
        }
        private  void GotoPage(Page page) 
        {
            Animations.FrameMoving(PageContent, 100,30);
            Animations.ChangeOP(PageContent, 0, 1, 0.3);
            PageContent.Navigate(page);
            Dictionary<Type, int> pageAndSilderLocation = new() 
            {
                { typeof(HomePage),5},
                { typeof(AboutPage),505}
            };
            foreach (var item in pageAndSilderLocation) 
            {
                if (page.GetType() == item.Key) 
                {
                    Animations.PageSilderMoveing(MenuSlider, item.Value);
                    break;
                }
            }
        }

        private void UISetting_Initialize() 
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
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
            GlobalContent.HomePage ??= new();
            if (PageContent.Content == GlobalContent.HomePage) return;
            GotoPage(GlobalContent.HomePage);
        }

        private void AboutPageButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalContent.AboutPage ??= new();
            if (PageContent.Content == GlobalContent.AboutPage) return;
            GotoPage(GlobalContent.AboutPage);
        }
    }
}