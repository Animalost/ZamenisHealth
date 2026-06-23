using Domain;
using FormAndControls;
using Newtonsoft.Json;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class RIPS : Forma
    {
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly ICompañia repoCom = new MCompañia();
        private static readonly IRipsEventoTotal repoRip = new MRipsEventoTotal();
        private static readonly IRIPS repoRips = new MRIPS();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IRIPSJSON repoRipsJSON = new MRipsJSON();

        MensajesGeneral MG;
        Espera E;
        int Cia, Ase;
        string Tip, combo5;
        string consub, tipoDoc;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var A = repoAse.getInfoFromAsebyName(comboBox1.Text);
            Ase = Convert.ToInt32(A.Ase_Identificador);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var C = repoCom.getPrestadorbyName(comboBox2.Text);
            Cia = Convert.ToInt32(C.Com_Identificador);
        }

        void GenerarRIPSEvento()
        {
            try
            {
                repoRip.setDatos(Ase, Cia, dateTimePicker1.Value, dateTimePicker2.Value, Tip, combo5);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Shows()
        {
            E = new Espera();
            E.Show();
        }

        void Hides()
        {
            if (E != null)
                E.Close();
        }        
         
        void GenerarRIPS_2275_2023()
        {
            try
            {
                RIPS_Class r = new RIPS_Class
                {
                    desdeRIPS = dateTimePicker1.Value.Date,
                    hastaRIPS = dateTimePicker2.Value.Date,
                    aseRIPS = Ase,
                    ciaRIPS = Cia,
                    tDocumentRIPS = (comboBox4.Text == "Ordenes de Pedido" ? "OP" : comboBox4.Text == "Facturas" ? "FA" : "DE")
                };

                Dictionary<string, Transaccion> D = repoRipsJSON.GenrateTotal(r);
                if (D != null) 
                {
                    foreach (var d in D)
                    {
                        string json = JsonConvert.SerializeObject(d.Value, Formatting.Indented);                       

                        //Verifica vacios
                        string jsonLimpio = JsonVerifications.JsonLimpio(json);
                        //fin verifica vacios

                        if (checkBox1.Checked == true)
                        {
                            string rutaCarpeta = @"C:\CXN\Rips\" + d.Key.ToString();                            

                            if (!Directory.Exists(rutaCarpeta))
                            {
                                Directory.CreateDirectory(rutaCarpeta);
                            }

                            System.IO.File.WriteAllText(@"C:\CXN\RIPS\" + d.Key + "\\" + d.Key + ".json", jsonLimpio);
                        }
                        else
                        {
                            System.IO.File.WriteAllText(@"C:\CXN\RIPS\" + d.Key + ".json", jsonLimpio);
                        }                        
                    };

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Generado en C CXN Rips";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "No hay resultados";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }             

        private void RIPS_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "RIPS";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnInd = new ToolStripButton();
                btnInd = createToolButton("RIP Individual");
                MenuLateral.Items.Add(btnInd);
                btnInd.Click += btnZamenis1_ButtonClick;

                ToolStripButton btn2000 = new ToolStripButton();
                btn2000 = createToolButton("Res 3374/2000");
                MenuLateral.Items.Add(btn2000);
                btn2000.Click += btnZamenis2_ButtonClick;

                ToolStripButton btnRepairMP = new ToolStripButton();
                btnRepairMP = createToolButton("Preparar MP");
                MenuLateral.Items.Add(btnRepairMP);
                btnRepairMP.Click += btnZamenis4_ButtonClick;

                ToolStripButton btnRes2000and2275 = new ToolStripButton();
                btnRes2000and2275 = createToolButton("RIPS 3374 y 2275");
                MenuLateral.Items.Add(btnRes2000and2275);
                btnRes2000and2275.Click += btnZamenis6_ButtonClick;

                ToolStripButton btn3275 = new ToolStripButton();
                btn3275 = createToolButton("Res 2275/2024");
                MenuLateral.Items.Add(btn3275);
                btn3275.Click += btnZamenis3_ButtonClick;

                ToolStripButton btnDocker = new ToolStripButton();
                btnDocker = createToolButton("Docker");
                MenuLateral.Items.Add(btnDocker);
                btnDocker.Click += btnZamenis7_ButtonClick;
                btnDocker.Enabled = false;

                
                var Ases = repoAse.getAseguradoras();
                if (Ases != null)
                {
                    foreach (var i in Ases)
                    {
                        comboBox1.Items.Add(i.Ase_Descripcion);
                    }
                }

                var Cias = repoCom.getAllCompañias();
                if (Cias != null)
                {
                    foreach (var i in Cias)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }
                }

                List<string> Regi = repoPacientes.ListaRegimen();
                if (Regi != null)
                {
                    foreach (string r in Regi)
                    {
                        comboBox5.Items.Add(r);
                    }
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        
     
        public RIPS()
        {
            InitializeComponent();
        }

        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            RIPS_IND R = new RIPS_IND();
            R.ShowDialog();
        }

        private async void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox4.Text)
                {
                    case "Facturas":
                        Tip = "FA";
                        break;

                    case "Ordenes de Pedido":
                        Tip = "OP";
                        break;

                    case "Documentos Equivalentes":
                        Tip = "DE";
                        break;

                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una opcion valida del tipo de documentos";
                        MG.ShowDialog();
                        return;
                }

                switch (comboBox3.Text)
                {
                    case "Evento":
                        combo5 = comboBox5.Text;
                        Shows();
                        Task oTask = new Task(GenerarRIPSEvento);
                        oTask.Start();
                        await oTask;
                        Hides();
                        break;

                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una opcion valida";
                        MG.ShowDialog();
                        break;
                }

                Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                M.TipoImagen = 3;
                M.Mensaje = "Se han generado los RIPS en la ubicacion en C: Cxn Rips";
                M.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private async void btnZamenis6_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                consub = repoPacientes.Regimen(comboBox5.Text);
                tipoDoc = "";

                switch (comboBox4.Text)
                {
                    case "Ordenes de Pedido":
                        tipoDoc = "OP";
                        break;
                    case "Facturas":
                        tipoDoc = "FA";
                        break;
                    case "Documentos Equivalentes":
                        tipoDoc = "DE";
                        break;
                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Tipo de documento seleccionado invalido";
                        return;
                }

                Shows();
                Task oTask = new Task(GenerarRIPS_2275_2023_txt); //este
                oTask.Start();
                await oTask;
                Hides();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void btnZamenis7_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                RIPS_Docker rD = new RIPS_Docker();
                rD.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }     

        void GenerarRIPS_2275_2023_txt()
        {
            try
            {
                RIPS_Class r = new RIPS_Class
                {
                    desdeRIPS = dateTimePicker1.Value.Date,
                    hastaRIPS = dateTimePicker2.Value.Date,
                    aseRIPS = Ase,
                    ciaRIPS = Cia,
                    tDocumentRIPS = (comboBox4.Text == "Ordenes de Pedido" ? "OP" : comboBox4.Text == "Facturas" ? "FA" : "DE")
                };

                Dictionary<string, Transaccion> D = repoRipsJSON.GenrateTotal(r);
                if (D != null)
                {                                       
                    foreach (var d in D)
                    {
                        string clave = d.Key;  
                        string rutaCarpeta = @"C:\CXN\Rips";

                        if (checkBox1.Checked == true)
                        {
                            rutaCarpeta = @"C:\CXN\Rips\" + clave.ToString();

                            if (!Directory.Exists(rutaCarpeta))
                            {
                                Directory.CreateDirectory(rutaCarpeta);
                            }
                        }                        

                        FileStream QueryTxtUS = new FileStream(rutaCarpeta + "\\US" + clave + ".txt", FileMode.Append, FileAccess.Write);
                        FileStream QueryTxtAC = new FileStream(rutaCarpeta + "\\AC" + clave + ".txt", FileMode.Append, FileAccess.Write);
                        FileStream QueryTxtAP = new FileStream(rutaCarpeta + "\\AP" + clave + ".txt", FileMode.Append, FileAccess.Write);
                        FileStream QueryTxtAT = new FileStream(rutaCarpeta + "\\AT" + clave + ".txt", FileMode.Append, FileAccess.Write);
                        FileStream QueryTxtCT = new FileStream(rutaCarpeta + "\\CT" + clave + ".txt", FileMode.Append, FileAccess.Write);

                        string US = "";
                        string AC = "";
                        string AP = "";
                        string AT = "";
                        string CT = "";

                        Transaccion transaccion = d.Value; // Obtiene la transacción asociada                      

                        foreach (var usuario in transaccion.usuarios)
                        {
                            string cUsuario = usuario.numDocumentoIdentificacion;

                            CT = CT + transaccion.numDocumentoIdObligado + "|" +
                                      transaccion.numFactura + "|" +
                                      transaccion.tipoNota + "|" +
                                      transaccion.numNota;

                            //ARCHIVO US
                            US = US + usuario.tipoDocumentoIdentificacion + "|" + 
                                      usuario.numDocumentoIdentificacion + "|" + 
                                      "" + "|" +
                                      usuario.tipoUsuario + "|" + 
                                      Convert.ToDateTime(usuario.fechaNacimiento).ToString("yyyy-MM-dd") + "|" + 
                                      usuario.codSexo + "|" + 
                                      usuario.codPaisResidencia + "|" + 
                                      usuario.codMunicipioResidencia + "|" + 
                                      usuario.codZonaTerritorialResidencia + "|" + 
                                      usuario.incapacidad + "|" + 
                                      usuario.codPaisOrigen + "|" + 
                                      usuario.consecutivo + "\r";

                            //ARCHIVO AC
                            foreach (var consulta in usuario.servicios.consultas)
                            {
                             AC = AC + cUsuario + "|" +
                                      consulta.codPrestador + "|" +
                                      Convert.ToDateTime(consulta.fechaInicioAtencion).ToString("yyyy-MM-dd") + "|" +
                                      consulta.numAutorizacion + "|" +
                                      consulta.codConsulta + "|" +
                                      consulta.modalidadGrupoServicioTecSal + "|" +
                                      consulta.grupoServicios + "|" +
                                      consulta.codServicio + "|" +
                                      consulta.finalidadTecnologiaSalud + "|" +
                                      consulta.causaMotivoAtencion + "|" +
                                      consulta.codDiagnosticoPrincipal + "|" +
                                      consulta.codDiagnosticoRelacionado1 + "|" +
                                      consulta.codDiagnosticoRelacionado2 + "|" +
                                      consulta.codDiagnosticoRelacionado3 + "|" +
                                      consulta.tipoDiagnosticoPrincipal + "|" +
                                      consulta.tipoDocumentoIdentificacion + "|" +
                                      consulta.numDocumentoIdentificacion + "|" +
                                      consulta.vrServicio + "|" +
                                      consulta.conceptoRecaudo + "|" +
                                      consulta.valorPagoModerador + "|" +
                                      consulta.numFEVPagoModerador + "|" +
                                      consulta.consecutivo + "\r";
                            }

                            //ARCHIVO AP
                            foreach (var procedimiento in usuario.servicios.procedimientos)
                            {
                                AP = AP + cUsuario + "|" +
                                      procedimiento.codPrestador + "|" +
                                      Convert.ToDateTime(procedimiento.fechaInicioAtencion).ToString("yyyy-MM-dd") + "|" +
                                      procedimiento.idMIPRES + "|" +
                                      procedimiento.numAutorizacion + "|" +
                                      procedimiento.codProcedimiento + "|" +
                                      procedimiento.viaIngresoServicioSalud + "|" +
                                      procedimiento.modalidadGrupoServicioTecSal + "|" +
                                      procedimiento.grupoServicios + "|" +
                                      procedimiento.codServicio + "|" +
                                      procedimiento.finalidadTecnologiaSalud + "|" +
                                      procedimiento.tipoDocumentoIdentificacion + "|" +
                                      procedimiento.numDocumentoIdentificacion + "|" +
                                      procedimiento.codDiagnosticoPrincipal + "|" +
                                      procedimiento.codDiagnosticoRelacionado + "|" +
                                      procedimiento.codComplicacion + "|" +
                                      procedimiento.vrServicio + "|" +
                                      procedimiento.conceptoRecaudo + "|" +
                                      procedimiento.valorPagoModerador + "|" +
                                      procedimiento.numFEVPagoModerador + "|" +
                                      procedimiento.consecutivo + "\r";
                            }

                            //ARCHIVO AT
                            foreach (var otrosServicios in usuario.servicios.otrosServicios)
                            {
                                AT = AT + cUsuario + "|" +
                                      otrosServicios.codPrestador + "|" +
                                      otrosServicios.numAutorizacion + "|" +
                                      otrosServicios.idMIPRES + "|" +
                                      Convert.ToDateTime(otrosServicios.fechaSuministroTecnologia).ToString("yyyy-MM-dd") + "|" +
                                      otrosServicios.tipoOS + "|" +
                                      otrosServicios.codTecnologiaSalud + "|" +
                                      otrosServicios.nomTecnologiaSalud + "|" +
                                      otrosServicios.cantidadOS + "|" +
                                      otrosServicios.tipoDocumentoIdentificacion + "|" +
                                      otrosServicios.numDocumentoIdentificacion + "|" +
                                      otrosServicios.vrUnitOS + "|" +
                                      otrosServicios.vrServicio + "|" +
                                      otrosServicios.conceptoRecaudo + "|" +
                                      otrosServicios.valorPagoModerador + "|" +
                                      otrosServicios.numFEVPagoModerador + "|" +
                                      otrosServicios.consecutivo + "\r";
                            }
                        }

                        StreamWriter EscribaUS = new StreamWriter(QueryTxtUS);
                        StreamWriter EscribaAC = new StreamWriter(QueryTxtAC);
                        StreamWriter EscribaAP = new StreamWriter(QueryTxtAP);
                        StreamWriter EscribaAT = new StreamWriter(QueryTxtAT);
                        StreamWriter EscribaCT = new StreamWriter(QueryTxtCT);

                        EscribaUS.Write(US);
                        EscribaAC.Write(AC);
                        EscribaAP.Write(AP);
                        EscribaAT.Write(AT);
                        EscribaCT.Write(CT);

                        EscribaUS.Close();
                        EscribaAC.Close();
                        EscribaAP.Close();
                        EscribaAT.Close();
                        EscribaCT.Close();

                        FileInfo fileUS = new FileInfo(rutaCarpeta + "\\US" + clave + ".txt");
                        FileInfo fileAC = new FileInfo(rutaCarpeta + "\\AC" + clave + ".txt");
                        FileInfo fileAP = new FileInfo(rutaCarpeta + "\\AP" + clave + ".txt");
                        FileInfo fileAT = new FileInfo(rutaCarpeta + "\\AT" + clave + ".txt");
                        FileInfo fileCT = new FileInfo(rutaCarpeta + "\\CT" + clave + ".txt");

                        if (fileUS.Exists && fileUS.Length == 0) { fileUS.Delete(); }
                        if (fileAC.Exists && fileAC.Length == 0) { fileAC.Delete(); }
                        if (fileAP.Exists && fileAP.Length == 0) { fileAP.Delete(); }
                        if (fileAT.Exists && fileAT.Length == 0) { fileAT.Delete(); }
                        if (fileCT.Exists && fileCT.Length == 0) { fileCT.Delete(); }
                    }
                    ;                   

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Generado en C CXN Rips";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "No hay resultados";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        private async void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                consub = repoPacientes.Regimen(comboBox5.Text);
                tipoDoc = "";

                switch (comboBox4.Text)
                {
                    case "Ordenes de Pedido":
                        tipoDoc = "OP";
                        break;
                    case "Facturas":
                        tipoDoc = "FA";
                        break;
                    case "Documentos Equivalentes":
                        tipoDoc = "DE";
                        break;
                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Tipo de documento seleccionado invalido";
                        return;
                }

                Shows();
                Task oTask = new Task(GenerarRIPS_2275_2023); //este
                oTask.Start();
                await oTask;
                Hides();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Desea reparar los codigos CUPS de los RIPS, esta accion no tiene marcha atras",
                                                 "Zamenis Health - CUPS Cargos Vs Horario",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    RIPS_Class R = new RIPS_Class
                    {
                        desdeRIPS = Convert.ToDateTime(dateTimePicker1.Value.Date),
                        hastaRIPS = Convert.ToDateTime(dateTimePicker2.Value.Date),
                        aseRIPS = Ase,
                        ciaRIPS = Cia
                    };

                    repoRips.RepararRIPSCupCarVsHor(R);

                    repoRips.updateRegimenMP(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, Cia, Ase);

                    MG = new MensajesGeneral();
                    MG.Mensaje = "Hecho";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
       
    }
}
