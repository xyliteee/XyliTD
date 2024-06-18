using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyliTDMain.Static
{
    public class WorkDirectory
    {
        public static readonly string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string defaultMusicPath = Path.Combine(rootDirectory, "Music");
        public static readonly string ImagePath = Path.Combine(rootDirectory, "Images");
        public static void CreatDir() 
        {
            Directory.CreateDirectory(defaultMusicPath);
            Directory.CreateDirectory(ImagePath);
        }
    }
}
