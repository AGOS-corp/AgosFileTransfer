using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgosFileTransfer
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
            int size, string filePath);

        private static string _devicePathFile = AppDomain.CurrentDomain.BaseDirectory + "Info.ini";

        public static string GetINIData(string Section, string Key, string val)
        {
            StringBuilder _temp = new StringBuilder();
            GetPrivateProfileString(Section, Key, val, _temp, 32, _devicePathFile);            

            return _temp.ToString();
        }
    }
}
