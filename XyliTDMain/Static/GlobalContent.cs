using System.Security.Permissions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XyliTDMain.Dynamic;
using XyliTDMain.Pages;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace XyliTDMain.Static
{
    public static class GlobalContent
    {
        public static MainWindow MainWindow { get; set; }
        public static HomePage HomePage { set; get; }
        public readonly static List<ConversionTask> conversionTaskList = [];
        public static AboutPage AboutPage { set; get; }
        
        
    }
}
