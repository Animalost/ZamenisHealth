using System;
using System.IO;

namespace ZamenisHealth.Clases
{
    public class Preferencias
    {
        public static string AppSound { get; set; }
        public static string Colorimetria { get; set; }
        public static string TCPIP { get; set; }
        public static string TicketCitas { get; set; }
        public static int Bloqueo { get; set; }
        public static string CuracionesCORE { get; set; }
        public static string TabletaFirmas { get; set; }

        public static void GetPreferences(string key, string value)
        {
            if (key == "Sonido")
            {
                AppSound = value;
            }
            if (key == "Colorimetria")
            {
                Colorimetria = value;
            }
            if (key == "TCPIP")
            {
                TCPIP = value;
            }
            if (key == "TicketCitas")
            {
                TicketCitas = value;
            }
            if (key == "Bloqueo")
            {
                Bloqueo = Convert.ToInt32(value);
            }
            if (key == "CuracionesCORE")
            {
                CuracionesCORE = value;
            }
            if (key == "TabletaFirmas")
            {
                TabletaFirmas = value;
            }
        }
        public static void SetPreferences(string key, string value)
        {
            string iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZamenisIni", "config.ini");
            var iniFile = new IniFile(iniPath);

            if (key == "Sonido")
            {
                iniFile.Write("PreferenciasUIUX", key, value);
                AppSound = value;
            }
            if (key == "Colorimetria")
            {
                iniFile.Write("PreferenciasUIUX", key, value);
                Colorimetria = value;
            }
            if (key == "TCPIP")
            {
                iniFile.Write("PreferenciasCom", key, value);
                TCPIP = value;
            }
            if (key == "TicketCitas")
            {
                iniFile.Write("PreferenciasCom", key, value);
                TicketCitas = value;
            }
            if (key == "Bloqueo")
            {
                iniFile.Write("PreferenciasConfig", key, value);
                Bloqueo = Convert.ToInt32(value);
            }
            if (key == "CuracionesCORE")
            {
                iniFile.Write("PreferenciasConfig", key, value);
                CuracionesCORE = value;
            }
            if (key == "TabletaFirmas")
            {
                iniFile.Write("PreferenciasConfig", key, value);
                TabletaFirmas = value;
            }
        }
    }
}
