using Domain;
using Microsoft.Win32;
using Persistence;
using System;
using System.Globalization;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth
{
    internal static class Program
    {       
        public static SoundPlayer soundPlayer, soundButton;
        public static string URLApiConexion { get; set; }
        public static System.Windows.Forms.Timer timerGlobal = new System.Windows.Forms.Timer();

        [STAThreadAttribute] 
        static void Main()
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.AddMessageFilter(new CloseAppInactivity());
            timerGlobal.Interval = 1000; // 1 segundo
            timerGlobal.Tick += Inactividad.TimerEventProcessor;
            timerGlobal.Start();

            DesactivarIPV6();
            ConfigurarCulturaPersonalizada();

            Control.CheckForIllegalCrossThreadCalls = false;

            VerificarCarpetaMadre();

            SplashScreen splashScreen = new SplashScreen();
            if (splashScreen.ShowDialog() == DialogResult.OK )
            {
                soundPlayer = new SoundPlayer(Properties.Resources.SoundSeleccion);
                soundButton = new SoundPlayer(Properties.Resources.SoundClic);
                soundPlayer.LoadAsync();
                soundButton.LoadAsync();

                Inicial j = new Inicial();
                j.ShowDialog();
            }            
        }
        static void ConfigurarCulturaPersonalizada()
        {
            // Crear copia de la cultura base (ej. Español de Colombia)
            CultureInfo cultura = new CultureInfo("es-CO", true);

            // Personalizar los separadores
            cultura.NumberFormat.NumberDecimalSeparator = ",";
            cultura.NumberFormat.CurrencyDecimalSeparator = ",";
            cultura.NumberFormat.NumberGroupSeparator = ".";
            cultura.NumberFormat.CurrencyGroupSeparator = ".";

            // Aplicar la cultura a nivel de hilo actual
            Thread.CurrentThread.CurrentCulture = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;
        }

        static void DesactivarIPV6()
        {
            try
            {
                const string keyPath = @"SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("DisabledComponents", 0xFF, RegistryValueKind.DWord);
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "IPv6 desactivado correctamente. Reinicie el sistema para aplicar los cambios", 
                                                            Formulario = "Program.cs", 
                                                            Metodo = OverridesExtern.GetCurrentMethodName(), 
                                                            Usuario = "BackEnd" }; 
                        OverridesExtern.GenerarTXTException(T);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo acceder a la clave del registro");
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Program.cs", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }

        static void VerificarCarpetaMadre()
        {
            try
            {
                string carpetaCXN = @"C:\CXN";
                string carpetaPlantillas = @"C:\CXN\Plantillas";
                string carpetaReportes = @"C:\CXN\Reportes";
                string carpetaReportesDocumentos = @"C:\CXN\Reportes\Documentos";
                string carpetaRips = @"C:\CXN\Rips";
                string carpetaNotas = @"C:\CXN\Reportes\Notas";
                string carpetaHistorias = @"C:\CXN\Reportes\Historias";
                string carpetaRDA = @"C:\CXN\Reportes\RDA";
                string carpetaConsIdCompo = @"C:\CXN\Reportes\RDA\ConsultaIdComposition";

                string carpetaDIAN = @"C:\CXN\RespuestasDIAN";
                string carpetaDIANPDF = @"C:\CXN\RespuestasDIAN\RespuestaPDF";
                string carpetaDIANXML = @"C:\CXN\RespuestasDIAN\RespuestaXML";

                string NameFileHomologosFacturas = "P_Homologos.xls";
                string NameFileHomologosCaja = "P_HomologosRcCaja.xls";
                string NameFileHomologosPagos = "P_Pagos.xls";
                string NameFileHomologosSurtidos = "InventarioPpal.xlsx";
                string NameFileHomologosSurtidosSub = "InventarioSub.xlsx";
                string NameFileCertificados = "Certificado.txt";
                string NameCuvMasivo = "CUVMasivo.xls";

                FileIniApp();

                if (!Directory.Exists(carpetaCXN))
                {
                    //Si CXN No existe se crea todo
                    Directory.CreateDirectory(carpetaCXN);
                    Directory.CreateDirectory(carpetaPlantillas);
                    Directory.CreateDirectory(carpetaReportes);
                    Directory.CreateDirectory(carpetaRips);

                    Directory.CreateDirectory(carpetaDIAN);
                    Directory.CreateDirectory(carpetaDIANPDF);
                    Directory.CreateDirectory(carpetaDIANXML);

                    GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosFacturas);
                    GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosCaja);
                    GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosPagos);
                    GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosSurtidos);
                    GuardarArchivoDesdeRecursos(carpetaCXN, NameFileCertificados);
                }
                else
                {
                    if (!File.Exists(NameFileCertificados))
                    {
                        GuardarArchivoDesdeRecursos(carpetaCXN, NameFileCertificados);
                    }

                    //Carpetas DIAN
                    if (!Directory.Exists(carpetaDIAN))
                    {
                        Directory.CreateDirectory(carpetaDIAN);
                        Directory.CreateDirectory(carpetaDIANPDF);
                        Directory.CreateDirectory(carpetaDIANXML);
                    }
                    else
                    {
                        if (!Directory.Exists(carpetaDIANPDF))
                        {
                            Directory.CreateDirectory(carpetaDIANPDF);
                        }
                        if (!Directory.Exists(carpetaDIANXML))
                        {
                            Directory.CreateDirectory(carpetaDIANXML);
                        }
                    }

                    //Si existe CXN verificar las otras carpetas
                    if (!Directory.Exists(carpetaPlantillas))
                    {
                        Directory.CreateDirectory(carpetaPlantillas);

                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosFacturas);
                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosCaja);
                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosPagos);
                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosSurtidos);
                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosSurtidosSub);
                        GuardarArchivoDesdeRecursos(carpetaPlantillas, NameCuvMasivo);
                    }
                    else
                    {
                        // Verificar si el archivo ya existe, si no, copiarlo desde los recursos
                        if (!File.Exists(NameFileHomologosFacturas))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosFacturas);
                        }
                        if (!File.Exists(NameFileHomologosCaja))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosCaja);
                        }
                        if (!File.Exists(NameFileHomologosPagos))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosPagos);
                        }
                        if (!File.Exists(NameFileHomologosSurtidos))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosSurtidos);
                        }
                        if (!File.Exists(NameFileHomologosSurtidosSub))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameFileHomologosSurtidosSub);
                        }
                        if (!File.Exists(NameCuvMasivo))
                        {
                            GuardarArchivoDesdeRecursos(carpetaPlantillas, NameCuvMasivo);
                        }
                    }

                    if (!Directory.Exists(carpetaReportes))
                    {
                        Directory.CreateDirectory(carpetaReportes);
                        Directory.CreateDirectory(carpetaReportesDocumentos);
                        Directory.CreateDirectory(carpetaNotas);
                        Directory.CreateDirectory(carpetaHistorias);
                        Directory.CreateDirectory(carpetaRDA);
                        Directory.CreateDirectory(carpetaConsIdCompo);
                    }
                    else
                    {
                        if (!Directory.Exists(carpetaReportesDocumentos))
                        {
                            Directory.CreateDirectory(carpetaReportesDocumentos);                         
                        }
                        if (!Directory.Exists(carpetaNotas))
                        {
                            Directory.CreateDirectory(carpetaNotas);
                        }
                        if (!Directory.Exists(carpetaHistorias))
                        {
                            Directory.CreateDirectory(carpetaHistorias);
                        }
                        if (!Directory.Exists(carpetaRDA))
                        {
                            Directory.CreateDirectory(carpetaRDA);
                        }
                        if (!Directory.Exists(carpetaConsIdCompo))
                        {
                            Directory.CreateDirectory(carpetaConsIdCompo);
                        }
                    }

                    if (!Directory.Exists(carpetaRips))
                    {
                        Directory.CreateDirectory(carpetaRips);
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creando capeta madre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void FileIniApp()
        {
            try
            {
                string iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZamenisIni", "config.ini");
                var iniFile = new IniFile(iniPath);

                if (!File.Exists(iniPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(iniPath)); // Crear la carpeta si no existe
                    File.Create(iniPath).Dispose();

                    iniFile.Write("PreferenciasUIUX", "Sonido", "A");
                    iniFile.Write("PreferenciasUIUX", "Colorimetria", "N");
                    iniFile.Write("PreferenciasCom", "TCPIP", "N");
                    iniFile.Write("PreferenciasCom", "TicketCitas", "N");
                    iniFile.Write("PreferenciasConfig", "Bloqueo", "0");
                    iniFile.Write("PreferenciasConfig", "CuracionesCORE", "N");
                    iniFile.Write("PreferenciasConfig", "TabletaFirmas", "N");
                }

                if (!iniFile.SectionExists("PreferenciasUIUX")) 
                {
                    iniFile.Write("PreferenciasUIUX", "Sonido", "A");
                    iniFile.Write("PreferenciasUIUX", "Colorimetria", "N");
                }
                else
                {
                    if (!iniFile.KeyExists("PreferenciasUIUX", "Sonido"))
                    {
                        iniFile.Write("PreferenciasUIUX", "Sonido", "A");
                    }
                    if (!iniFile.KeyExists("PreferenciasUIUX", "Colorimetria"))
                    {
                        iniFile.Write("PreferenciasUIUX", "Colorimetria", "N");
                    }

                }
                if (!iniFile.SectionExists("PreferenciasCom"))
                {
                    iniFile.Write("PreferenciasCom", "TCPIP", "N");
                    iniFile.Write("PreferenciasCom", "TicketCitas", "N");
                }
                else
                {
                    if (!iniFile.KeyExists("PreferenciasCom", "TCPIP"))
                    {
                        iniFile.Write("PreferenciasCom", "TCPIP", "N");
                    }
                    if (!iniFile.KeyExists("PreferenciasCom", "TicketCitas"))
                    {
                        iniFile.Write("PreferenciasCom", "TicketCitas", "N");
                    }
                }
                if (!iniFile.SectionExists("PreferenciasConfig"))
                {
                    iniFile.Write("PreferenciasConfig", "Bloqueo", "0");
                    iniFile.Write("PreferenciasConfig", "CuracionesCORE", "N");
                    iniFile.Write("PreferenciasConfig", "TabletaFirmas", "N");
                }
                else
                {
                    if (!iniFile.KeyExists("PreferenciasConfig", "Bloqueo"))
                    {
                        iniFile.Write("PreferenciasConfig", "Bloqueo", "0");
                    }
                    if (!iniFile.KeyExists("PreferenciasConfig", "CuracionesCORE"))
                    {
                        iniFile.Write("PreferenciasConfig", "CuracionesCORE", "N");
                    }
                    if (!iniFile.KeyExists("PreferenciasConfig", "TabletaFirmas"))
                    {
                        iniFile.Write("PreferenciasConfig", "TabletaFirmas", "N");
                    }
                }
                string sonido = iniFile.Read("PreferenciasUIUX", "Sonido", "A");
                string colorimetria = iniFile.Read("PreferenciasUIUX", "Colorimetria", "N");
                string tcpip = iniFile.Read("PreferenciasCom", "TCPIP", "N");
                string ticketcitas = iniFile.Read("PreferenciasCom", "TicketCitas", "N");
                string bloqueo = iniFile.Read("PreferenciasConfig", "Bloqueo", "0");
                string curacionesCORE = iniFile.Read("PreferenciasConfig", "CuracionesCORE", "N");
                string TabletaFirmas = iniFile.Read("PreferenciasConfig", "TabletaFirmas", "N");

                Preferencias.GetPreferences("Sonido", sonido);
                Preferencias.GetPreferences("Colorimetria", colorimetria);                
                Preferencias.GetPreferences("TCPIP", tcpip);
                Preferencias.GetPreferences("TicketCitas", ticketcitas);
                Preferencias.GetPreferences("Bloqueo", bloqueo);
                Preferencias.GetPreferences("CuracionesCORE", curacionesCORE);
                Preferencias.GetPreferences("TabletaFirmas", TabletaFirmas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de lectura Archivo INI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static void GuardarArchivoDesdeRecursos(string rutaDestino, string Archivo)
        {
            byte[] archivoExcel = null;
            string archivoTxt = "";

            switch (Archivo)
            {
                case "P_Homologos.xls":
                    archivoExcel = Properties.Resources.P_Homologos;
                    File.WriteAllBytes(Path.Combine(rutaDestino, Archivo), archivoExcel);
                    break;
                case "P_HomologosRcCaja.xls":
                    archivoExcel = Properties.Resources.P_HomologosRcCaja;
                    File.WriteAllBytes(Path.Combine(rutaDestino, Archivo), archivoExcel);
                    break;
                case "P_Pagos.xls":
                    archivoExcel = Properties.Resources.P_Pagos;
                    File.WriteAllBytes(Path.Combine(rutaDestino, Archivo), archivoExcel);
                    break;
                case "InventarioSub.xlsx":
                    archivoExcel = Properties.Resources.InventarioSub;
                    File.WriteAllBytes(Path.Combine(rutaDestino, Archivo), archivoExcel);
                    break;
                case "InventarioPpal.xlsx":
                    archivoExcel = Properties.Resources.InventarioPpal;
                    File.WriteAllBytes(Path.Combine(rutaDestino, Archivo), archivoExcel);
                    break;
                case "Certificado.txt":
                    archivoTxt = Properties.Resources.Certificado;
                    File.WriteAllText(Path.Combine(rutaDestino, Archivo), archivoTxt);
                    break;
                default:
                    break;
            }            
        }
    }    
}
