using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyliTD.Dynamic;
using XyliTDMain.Static;

namespace XyliTD.Static
{
    public class Section 
    {
        public const string MediaPlayer = "MediaPlayer";
        public const string Applicaiton = "Applicaiton";
    }

    public static class SettingManager
    {
        private static readonly string settingFilePath = Path.Combine(WorkDirectory.ConfigPath, "Application.ini");
        public static void CreatDefaultSetting() 
        {
            EditSetting(Section.MediaPlayer, "volume", "1");
            EditSetting(Section.Applicaiton, "is_first_use", "true");
        }
        public static void EditSetting (string section,string key,string value)
        {
            IniFile iniFile = new(settingFilePath);
            iniFile.Write(section, key, value);
        }

        public static double? ReadSettingDouble(string section, string key,double? min = null,double? max = null) 
        {
            if (min.HasValue && max.HasValue && min.Value >= max.Value)
            {
                throw new ArgumentException("Error limit");
            }
            IniFile iniFile = new(settingFilePath);
            string valueString = iniFile.Read(section, key);
            bool isSucessful = double.TryParse(valueString,out double value);
            if (!isSucessful) return null;
            if (min.HasValue && value < min.Value) return null;
            if (max.HasValue && value > max.Value) return null;
            return value;
        }

        public static bool? ReadSettingBool(string section,string key) 
        {
            IniFile iniFile = new(settingFilePath);
            string valueString = iniFile.Read(section, key);
            bool isSucessful = bool.TryParse(valueString, out bool value);
            if (!isSucessful) return null;
            return value;
        }

    }
}
