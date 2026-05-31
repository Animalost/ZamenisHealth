using Domain.CXN;
using EmbededBussiness.Interfaz;
using EmbededBussiness.Servicio;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmbededSystem.Pages.Historias
{
    public class NotaEnfermeriaModel : PageModel
    {
        private readonly INotasCuraciones oController = new MNotasCuraciones();

        public int Admision { get; set; }

        public string Errores;
        public string ErrorGrave = "";
        public string MensajeGeneral = "";
        public string arrastra;
        public int Paciente;
        public int Cia;
        public int Ase;
        public int Prof;
        public string CUP;
        public string TSERV;
        public DateTime Fecha_Serv;
        public string Reg_RIP;
        public string CMANID;
        public string CMANTID;
        public string textBox23;
        public string PACSAL;
        public string textBox17;
        public int Valor;
        public string Serv;
        public string textBox1;
        public string textBox2;
        public string textBox3;
        public string textBox5;
        public string textBox4;
        public string textBox6;
        public string textBox8;
        public string textBox9;
        public string textBox12;
        public string textBox13;
        public string textBox7;
        public string textBox10;
        public string textBox11;
        public string textBox22;
        public string textBox20;
        public string textBox21;
        public string checkBox1, checkBox2, checkBox4, checkBox3;
        public string toolStripButton9, button2;
        public string CaidaTxt, infeccionTxt, deterioroTxt, alergiaTxt, dificultadTxt, psiquiatricoTxt, mayorTxt, dificilTxt, requiereTxt;
        public string AplicaEncuesta = "NO";
        public string VIH, Hepatitis;

        public List<CXN_NOTASMED> MEDPUBLIC;

        public NotaEnfermeriaModel()
        {

        }

        public void OnGet(int Admision)
        {
            try
            {
                this.Admision = Admision;

                oController.Graba_Hora_Atencion(Admision);
                oController.OpenAdmition(Admision, "S");

                var DatosAdmision = oController.cargarAdmision(Admision, "'P','H','A'");
                if (DatosAdmision == null)
                {
                    ErrorGrave = "Error en esta admision";
                }
                else
                {
                    arrastra = DatosAdmision.Hor_ArrastraHistoria;
                    Paciente = DatosAdmision.Hor_Pac_Id;
                    Cia = DatosAdmision.Hor_Pac_Cia;
                    Ase = DatosAdmision.Hor_Pac_Ase;
                    Prof = DatosAdmision.Hor_Pac_Bod;
                    CUP = DatosAdmision.Hor_Pac_Cup;
                    TSERV = DatosAdmision.Hor_Pac_Tipo_Serv;
                    Fecha_Serv = Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita);
                    Reg_RIP = DatosAdmision.Hor_Regimen;
                    CMANID = DatosAdmision.Pac_IdNum.ToString();
                    CMANTID = DatosAdmision.Pac_TipoId.ToString();
                    textBox23 = DatosAdmision.Hor_Observacion;
                    PACSAL = DatosAdmision.Hor_Pac_Sal;
                    VIH = DatosAdmision.VIH;
                    Hepatitis = DatosAdmision.Hepatitis;

                    var VAl = oController.ServicioNombre(CUP, Ase, TSERV);
                    if (VAl == null)
                    {
                        ErrorGrave = "Error grave cargardo valor del servicio, vuelva a ingresar a la admision";
                    }
                    else
                    {
                        var As = oController.getInfoFromAsebyCode(Ase);
                        textBox17 = As.Ase_Descripcion.ToString();

                        Valor = VAl.Con_Valor;
                        Serv = VAl.Con_Nombre;

                        textBox1 = Admision.ToString();
                        textBox2 = DatosAdmision.Hor_Imp_Age.ToString();
                        textBox3 = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Domain.Variables.Format_Fecha);
                        textBox5 = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                        DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                        int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                        textBox4 = edad.ToString();

                        Diagnosticos(DatosAdmision.Hor_Pac_Id);
                        var DX = oController.BuscaDX(textBox6);
                        textBox7 = DX.ToString();
                        DX = oController.BuscaDX(textBox8);
                        textBox10 = DX.ToString();
                        DX = oController.BuscaDX(textBox9);
                        textBox11 = DX.ToString();

                        if (DatosAdmision.Hor_ArrastraHistoria == "S")
                        {
                            MensajeGeneral = MensajeGeneral + "Este paciente es un control de mitad de paquete de Alta Complejidad";
                        }

                        CXN_PACIENTES DatoPac = oController.LlamarPacientebyId(Paciente);
                        if (DatoPac.Pac_Doble == "S")
                        {
                            checkBox1 = "S";
                        }

                        if (DatoPac.Pac_2VXS == "S")
                        {
                            checkBox2 = "S";
                        }

                        if (DatoPac.Pac_Especial == "S")
                        {
                            checkBox4 = "S";
                            MensajeGeneral = MensajeGeneral + "ATENCION!!! \n\r \n\r ESTE PACIENTE TIENE CONDICIONES Y TRATOS ESPECIALES, PROBABLEMENTE REQUIERA ACOMPAÑANTE EN CONSULTA";
                        }

                        Dictionary<string, string> getConfig = oController.getListado();

                        if (PACSAL == "A")
                        {
                            if (getConfig != null)
                            {
                                _ = getConfig["AutocompletarNotas"] == "A" ? checkBox3 = "S" : checkBox3 = "N";
                            }
                        }
                        else
                        {
                            checkBox3 = "N";
                        }

                        CargarOpcionesRecomendaciones();

                        string getDocWeb = oController.getListado()["ConsentimientosWEB"];
                        if (getDocWeb != "A")
                        {
                            toolStripButton9 = "N";
                        }

                            bool dataForSal = oController.ConsultarNavyEnfermeria(Paciente, Prof, Fecha_Serv);

                            if (dataForSal == true)
                            {
                                MensajeGeneral = MensajeGeneral + "\r\r" + "En esta cita el paciente tiene un control intermedio con Medicina General, por favor llame al medico";
                            }
                        

                        if (getConfig["Cargos"] == "A")
                        {
                            button2 = "A";
                        }

                        CargarServicioMedico();
                        CargarCantidades();
                        randomEncuestaSatisfaccion();
                        LoadMedida(Admision);
                    }
                }               
            }
            catch (Exception ex)
            {
                this.Errores = this.Errores + "\r\r" + ex.Message;
            }
        }       

        void LoadMedida(int Admisiones)
        {
            try
            {                
                List<CXN_NOTASMED> MED = oController.LoadHeridas(Admisiones);
                if (MED != null)
                {
                    MEDPUBLIC = new List<CXN_NOTASMED>();
                    MEDPUBLIC = MED;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        string Capitalize(int Mes)
        {
            switch (Mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
                default:
                    return "";
            }
        }
        void randomEncuestaSatisfaccion()
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;
                string Mes = Capitalize(Hoy.Month);

                if (oController.getListado()["EncuestaSatisCU"] == "A")
                {
                    if (oController.getCantEncuestaCU("CU", Mes, Hoy.Year) == true)
                    {
                        CXN_CONFENCUESTA C = new CXN_CONFENCUESTA
                        {
                            Mes = Mes,
                            Año = Hoy.Year,
                            Servicio = "CU"
                        };

                        if (oController.getCantCitas(this.Paciente, C) == true)
                        {
                            if (oController.getCantEncuestasPaciente(this.Paciente, C) == true)
                            {
                                Random random = new Random();
                                int numero = random.Next(1, 2);
                                int numero2 = random.Next(1, 2);

                                if (numero == numero2)
                                {
                                    AplicaEncuesta = "SI";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void CargarCantidades()
        {
            try
            {
                textBox21 = oController.Calcular3(Paciente, TSERV);
            }
            catch
            {
                textBox21 = "Fallo en el calculo de sesiones, no hay registros de autorizaciones ingresados";
            }
        }
        private void CargarServicioMedico()
        {
            try
            {
                var getInfo = oController.SugerenciaServicio(Paciente);
                if (getInfo != null)
                {
                    textBox22 = getInfo["Cup"];
                    textBox20 = getInfo["Servicio"];
                }
                else
                {
                    textBox22 = "";
                    textBox20 = "";
                }
            }
            catch (Exception ex)
            {
                this.Errores = this.Errores + "\r\r" + ex.Message;
            }
        }
        private void Diagnosticos(int Paciente)
        {
            try
            {
                var getDX = oController.Diagnosticos(Paciente);
                if (getDX != null)
                {
                    textBox6 = getDX["DX1"];
                    textBox8 = getDX["DX2"];
                    textBox9 = getDX["DX3"];
                    textBox12 = getDX["Impresion"];
                    textBox13 = getDX["Patologia"];
                }
                else
                {
                    textBox6 = "";
                    textBox8 = "";
                    textBox9 = "";
                    textBox12 = "";
                    textBox13 = "N/A";
                }
            }
            catch (Exception ex)
            {
                this.Errores = this.Errores + "\r\r" + ex.Message;
            }
        }
        void CargarOpcionesRecomendaciones()
        {
            try
            {
                List<string> getConditions = oController.getCondiciones(this.Paciente);
                if (getConditions != null)
                {
                    string resultado = getConditions.Find(x => x == "CAIDA");
                    if (resultado != null)
                    {
                        CaidaTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "INFECCION");
                    if (resultado != null)
                    {
                        infeccionTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "DETERIORO DE LA PIEL");
                    if (resultado != null)
                    {
                        deterioroTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "ALERGIA");
                    if (resultado != null)
                    {
                        alergiaTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "DIFICULTAD DE COMUNICACION");
                    if (resultado != null)
                    {
                        dificultadTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE PSIQUIATRICO");
                    if (resultado != null)
                    {
                        psiquiatricoTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "MAYOR DE 70 AÑOS");
                    if (resultado != null)
                    {
                        mayorTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE DIFICIL");
                    if (resultado != null)
                    {
                        dificilTxt = "A";
                    }

                    resultado = getConditions.Find(x => x == "MEDICO LO REQUIERE");
                    if (resultado != null)
                    {
                        requiereTxt = "A";
                    }
                } 
                else
                {
                    CaidaTxt = "N";
                    infeccionTxt = "N";
                    alergiaTxt = "N";
                    deterioroTxt = "N";
                    psiquiatricoTxt = "N";
                    dificultadTxt = "N";
                    dificilTxt = "N";
                    mayorTxt = "N";
                    requiereTxt = "N";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
