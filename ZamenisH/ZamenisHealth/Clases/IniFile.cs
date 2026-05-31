using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ZamenisHealth.Clases
{
    public class IniFile
    {
        private string _filePath;
        public IniFile(string filePath)
        {
            _filePath = filePath;
        }
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder returnValue, int size, string filePath);
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }
        public string Read(string section, string key, string defaultValue = "")
        {
            var returnValue = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, returnValue, 255, _filePath);
            return returnValue.ToString();
        }
        public bool SectionExists(string section)
        {
            return File.ReadAllLines(this._filePath).Any(line => line.Trim().Equals($"[{section}]", StringComparison.OrdinalIgnoreCase));
        }
        public bool KeyExists(string section, string key)
        {
            bool sectionFound = false;

            foreach (var line in File.ReadLines(this._filePath))
            {
                if (line.Trim().Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))
                {
                    sectionFound = true; // Encontró la sección
                }
                else if (sectionFound && line.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Encontró la clave dentro de la sección
                }
                else if (line.StartsWith("[") && line.EndsWith("]") && sectionFound)
                {
                    break; // Terminó la sección sin encontrar la clave
                }
            }

            return false;
        }
    }
}
