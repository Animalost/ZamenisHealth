using Microsoft.Win32;
using System;

namespace LicenceZH
{
    public static class Serial
    {
        //Obtiene el serial de la aplicacion en el registro de windows en Base64
        public static string getSerial()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Zamenis"))
                {
                    Object KEY = key.GetValue("Zamenis");

                    if (KEY.ToString() != null)
                    {
                        return KEY.ToString();
                    }

                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
