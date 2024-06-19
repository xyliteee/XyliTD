using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XyliTD.Dynamic
{
    public partial class IniFile
    {
        private readonly string Path;
        [LibraryImport("kernel32", EntryPoint = "WritePrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniPath)
        {
            Path = iniPath;
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key.ToUpper(), value, Path);
        }

        public string Read(string section, string key)
        {
            StringBuilder SB = new(255);
            _ = GetPrivateProfileString(section, key.ToUpper(), "", SB, 255, Path);
            return SB.ToString();
        }
    }

}
