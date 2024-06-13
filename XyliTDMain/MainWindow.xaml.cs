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
        private static bool isMenuShowing = true;
        public MainWindow()
        {
            InitializeComponent();
            UISetting_Initialize();
            GlobalContent.MainWindow = this;
            GlobalContent.HomePage ??= new();
            PageContent.Navigate(GlobalContent.HomePage);
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

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuShowing)
            {
                Animations.HorizoneMoveing(MenuBar, null, -50, 0.5);
                isMenuShowing = false;
            }
            else
            {
                Animations.HorizoneMoveing(MenuBar, null, 0, 0.5);
                isMenuShowing = true;
            }

        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalContent.HomePage ??= new();
            PageContent.Navigate(GlobalContent.HomePage);
        }
    }
}