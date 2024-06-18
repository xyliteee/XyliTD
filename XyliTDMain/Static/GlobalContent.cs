using System.Diagnostics;
using System.IO;
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
        public static MysongPage MysongPage { set; get; } = new();
        public static AboutPage AboutPage { set; get; }
        public static List<string> MysongsPath { set; get; } = [];
        public static List<string> SongList { set; get; } = [];
        public static bool IsSearching = false;
        public static class Function 
        {
            public static void SearchforSong()
            {
                IsSearching = true;
                string[] fileTypes = [".mp3", ".flac", ".ogg", ".wav"];
                foreach (string songsDirectory in MysongsPath)
                {
                    if (!Directory.Exists(songsDirectory))
                    {
                        continue;
                    }
                    foreach (var file in Directory.EnumerateFiles(songsDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        if (fileTypes.Contains(Path.GetExtension(file)))
                        {
                            SongList.Add(file);
                        }
                    }
                }
                IsSearching = false;
            }
            public static MemoryStream? GetCover(TagLib.File file)
            {
                MemoryStream? ms = null;
                if (file.Tag.Pictures.Length > 0)
                {
                    var bin = file.Tag.Pictures[0].Data.Data;
                    ms = new(bin);
                }
                return ms;
            }
        }
        
    }
}
