using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Gerenciales
{    
    public partial class EncuestasSatis : Forma
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IEncuestasSatis repoSatis = new MEncuestasSatis();
        private static readonly ICompañia repoCia = new MCompañia();

        private string res = "";
        private string TServ = "";
        private DateTime f;
        private int Cia;

        private List<CXN_ENCUESTASATIS> L;
        private Thread thread;

        public EncuestasSatis()
        {
            InitializeComponent();
        }
        private async void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 0 || comboBox2.SelectedIndex == 0 || comboBox3.SelectedIndex == 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione las opciones de consulta";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.SelectedIndex == 1)
                {
                    TServ = "CU";
                }

                f = new DateTime(Convert.ToInt32(comboBox3.Text), Capitalize(comboBox2.Text), 1);

                Task oTask = null;
                oTask = new Task(TotalGeneral);

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    //codigo cuando termine aqui
                    if (this.res == "OK")
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Generado en C CXN REPORTES";
                        MG.ShowDialog();
                    }
                    else if (this.res == "NO")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay resultados con estos parametros seleccionados";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Ocurrio un error inesperado, consulte el log de transacciones";
                        MG.ShowDialog();
                    }

                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 0 || comboBox2.SelectedIndex == 0 || comboBox3.SelectedIndex == 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione el servicio, mes y año en la parte superior";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.SelectedIndex == 1)
                {
                    TServ = "CU";
                }

                Task oTask = null;
                oTask = new Task(GenerarInforme);

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    //codigo cuando termine aqui
                    if (this.res == "NO")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay resultados con estos parametros seleccionados";
                        MG.ShowDialog();
                    }
                    else if (this.res == "")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Ocurrio un error inesperado, consulte el log de transacciones";
                        MG.ShowDialog();
                    }

                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione el servicio";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.SelectedIndex == 1)
                {
                    TServ = "CU";
                }

                Task oTask = null;
                oTask = new Task(GenerarInformeTrimestral);

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    //codigo cuando termine aqui
                    if (this.res == "NO")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay resultados con estos parametros seleccionados";
                        MG.ShowDialog();
                    }
                    else if (this.res == "")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Ocurrio un error inesperado, consulte el log de transacciones";
                        MG.ShowDialog();
                    }

                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 0 || comboBox2.SelectedIndex == 0 || comboBox3.SelectedIndex == 0 || textBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione las opciones de consulta y digite el documento del paciente";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox2.Text == "" || comboBox3.Text == "" || comboBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione mes, año, digite el documento del paciente y seleccione el tipo de encuestas para generar las encuestas";
                    MG.ShowDialog();
                    return;
                }

                TServ = "";

                if (comboBox1.SelectedIndex == 1)
                {
                    TServ = "CU";
                }
                
                f = new DateTime(Convert.ToInt32(comboBox3.Text), Capitalize(comboBox2.Text), 1);
                Console.WriteLine(f);

                Task oTask = null;
                oTask = new Task(TotalPaciente);

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    //codigo cuando termine aqui
                    if (this.res == "OK")
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Generado en C CXN REPORTES";
                        MG.ShowDialog();
                    }
                    else if (this.res == "NO")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay resultados con estos parametros seleccionados";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Ocurrio un error inesperado, consulte el log de transacciones";
                        MG.ShowDialog();
                    }

                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void TotalGeneral()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();
                Dictionary<int, List<CXN_ENCUESTASATIS>> D = repoSatis.getEncuestas(f, TServ);
                if (D != null)
                {
                    int Contador = 1;
                    ReportViewer R = new ReportViewer();
                    int Total = D.Keys.Count;
                    int contadorArchivosGenerados = 0;

                    while (Contador <= Total)
                    {
                        contadorArchivosGenerados++;

                        R.LocalReport.DataSources.Clear();
                        R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Satis", D[Contador]));
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_EncuestaSatis.rdlc";
                        R.SetDisplayMode(DisplayMode.PrintLayout);
                        R.ZoomMode = ZoomMode.Percent;
                        R.ZoomPercent = 100;
                        R.Font = new Font("Arial", 7);
                        R.LocalReport.EnableExternalImages = true;
                        R.RefreshReport();
                        R.Dock = System.Windows.Forms.DockStyle.Fill;

                        byte[] bytes = R.LocalReport.Render("PDF");
                        FileStream fss = new FileStream("C:\\CXN\\Reportes\\" + Contador.ToString() + ".pdf", FileMode.Create);
                        fss.Write(bytes, 0, bytes.Length);
                        fss.Close();
                        Contador = Contador + 1;

                        int currentProgress = (int)(((double)contadorArchivosGenerados / Total) * 100);
                        progressBar1.Increment(currentProgress);
                    }

                    Contador = Contador - 1;
                    Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                    U.Unificar_Estructura(@"C:\CXN\Reportes\",
                                          1.ToString(),
                                          Total.ToString(),
                                          "UNIFICADO.pdf");

                    R.Dispose();

                    res = "OK";
                }
                else
                {
                    res = "NO";                    
                }
            }
            catch (Exception ex)
            {
                res = "";
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        int Capitalize(string Mes)
        {
            switch (Mes)
            {
                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;
                default:
                    return 0;
            }
        }
        void TotalPaciente()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();
                CXN_PACIENTES getPacId = repoPacientes.LlamarPacienteNumDoc(textBox1.Text);
                if (getPacId != null)
                {
                    int contadorArchivosGenerados = 0;

                    Dictionary<int, List<CXN_ENCUESTASATIS>> D = repoSatis.getEncuestas(getPacId.Pac_Id, f, TServ);
                    if (D != null)
                    {
                        int Contador = 1;
                        ReportViewer R = new ReportViewer();
                        int Total = D.Keys.Count;

                        while (Contador <= Total)
                        {
                            contadorArchivosGenerados++;
                            R.LocalReport.DataSources.Clear();
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Satis", D[Contador]));
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_EncuestaSatis.rdlc";
                            R.SetDisplayMode(DisplayMode.PrintLayout);
                            R.ZoomMode = ZoomMode.Percent;
                            R.ZoomPercent = 100;
                            R.Font = new Font("Arial", 7);
                            R.LocalReport.EnableExternalImages = true;
                            R.RefreshReport();
                            R.Dock = System.Windows.Forms.DockStyle.Fill;

                            byte[] bytes = R.LocalReport.Render("PDF");
                            FileStream fss = new FileStream("C:\\CXN\\Reportes\\" + Contador.ToString() + ".pdf", FileMode.Create);
                            fss.Write(bytes, 0, bytes.Length);
                            fss.Close();
                            Contador = Contador + 1;

                            int currentProgress = (int)(((double)contadorArchivosGenerados / Total) * 100);
                            progressBar1.Increment(currentProgress);
                        }

                        Contador = Contador - 1;
                        Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                        U.Unificar_Estructura(@"C:\CXN\Reportes\",
                                              1.ToString(),
                                              Total.ToString(),
                                              "UNIFICADO.pdf");

                        R.Dispose();

                        res = "OK";
                    }
                    else
                    {
                        res = "NO";
                    }
                }
                else
                {
                    res = "NO";
                }
            }
            catch (Exception ex)
            {
                res = "";
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void GenerarInforme()
        {
            try
            {
                DateTime fecha = new DateTime(Convert.ToInt32(comboBox4.Text), Capitalize(comboBox5.Text), 1);

                List<CXN_ENCUESTASATIS> tempList = repoSatis.InformeMensual(fecha, "CU", Cia);
                if (tempList != null)
                {
                    int r1 = 0; int r2 = 0; int r3 = 0; int r4 = 0; int r5 = 0; int r6 = 0; //cuantas veces marcaaron dicha respuesta
                    int rP11 = 0; int rP12 = 0; int rP13 = 0; int rP14 = 0; int rP15 = 0; int rP16 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 1
                    int rP21 = 0; int rP22 = 0; int rP23 = 0; int rP24 = 0; int rP25 = 0; int rP26 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 2
                    int rP31 = 0; int rP32 = 0; int rP33 = 0; int rP34 = 0; int rP35 = 0; int rP36 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 3
                    int rP41 = 0; int rP42 = 0; int rP43 = 0; int rP44 = 0; int rP45 = 0; int rP46 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 4
                    int rP51 = 0; int rP52 = 0; int rP53 = 0; int rP54 = 0; int rP55 = 0; int rP56 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 5
                    int rP61 = 0; int rP62 = 0; int rP63 = 0; int rP64 = 0; int rP65 = 0; int rP66 = 0; //cuantas veces marcaaron dicha respuesta de la pregunta 6
                    int TotalEncuestas = tempList.Count;
                    int TotalPreguntas = TotalEncuestas * 6; //total de preguntas por defecto son 6

                    foreach (CXN_ENCUESTASATIS t in tempList)
                    {
                        //lor r son acumulativos para el ponderato total final
                        switch (t.P1)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }
                        switch (t.P2)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }
                        switch (t.P3)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }
                        switch (t.P4)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }
                        switch (t.P5)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }
                        switch (t.P6)
                        {
                            case 1:
                                r1++;
                                break;
                            case 2:
                                r2++;
                                break;
                            case 3:
                                r3++;
                                break;
                            case 4:
                                r4++;
                                break;
                            case 5:
                                r5++;
                                break;
                            case 6:
                                r6++;
                                break;
                        }

                        switch (t.P1)
                        {
                            case 1:
                                rP11++;
                                break;
                            case 2:
                                rP12++;
                                break;
                            case 3:
                                rP13++;
                                break;
                            case 4:
                                rP14++;
                                break;
                            case 5:
                                rP15++;
                                break;
                            case 6:
                                rP16++;
                                break;
                        }
                        switch (t.P2)
                        {
                            case 1:
                                rP21++;
                                break;
                            case 2:
                                rP22++;
                                break;
                            case 3:
                                rP23++;
                                break;
                            case 4:
                                rP24++;
                                break;
                            case 5:
                                rP25++;
                                break;
                            case 6:
                                rP26++;
                                break;
                        }
                        switch (t.P3)
                        {
                            case 1:
                                rP31++;
                                break;
                            case 2:
                                rP32++;
                                break;
                            case 3:
                                rP33++;
                                break;
                            case 4:
                                rP34++;
                                break;
                            case 5:
                                rP35++;
                                break;
                            case 6:
                                rP36++;
                                break;
                        }
                        switch (t.P4)
                        {
                            case 1:
                                rP41++;
                                break;
                            case 2:
                                rP42++;
                                break;
                            case 3:
                                rP43++;
                                break;
                            case 4:
                                rP44++;
                                break;
                            case 5:
                                rP45++;
                                break;
                            case 6:
                                rP46++;
                                break;
                        }
                        switch (t.P5)
                        {
                            case 1:
                                rP51++;
                                break;
                            case 2:
                                rP52++;
                                break;
                            case 3:
                                rP53++;
                                break;
                            case 4:
                                rP54++;
                                break;
                            case 5:
                                rP55++;
                                break;
                            case 6:
                                rP56++;
                                break;
                        }
                        switch (t.P6)
                        {
                            case 1:
                                rP61++;
                                break;
                            case 2:
                                rP62++;
                                break;
                            case 3:
                                rP63++;
                                break;
                            case 4:
                                rP64++;
                                break;
                            case 5:
                                rP65++;
                                break;
                            case 6:
                                rP66++;
                                break;
                        }
                    }

                    double pTemp1 = 0;
                    double pTemp2 = 0;
                    double pTemp3 = 0;
                    double pTemp4 = 0;

                    //Ponderado para el infoirme sobre preguntas marcadas en general
                    pTemp1 = TotalPreguntas - r1;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1 = pTemp4 * (-1);

                    pTemp1 = TotalPreguntas - r2;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2 = pTemp4 * (-1);

                    pTemp1 = TotalPreguntas - r3;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3 = pTemp4 * (-1);

                    pTemp1 = TotalPreguntas - r4;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4 = pTemp4 * (-1);

                    pTemp1 = TotalPreguntas - r5;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5 = pTemp4 * (-1);

                    pTemp1 = TotalPreguntas - r6;
                    pTemp2 = pTemp1 / TotalPreguntas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6 = pTemp4 * (-1);

                    //Ponderado total por pregunta y por respuesta para el informe generañ
                    //PREGUNTA 1
                    pTemp1 = TotalEncuestas - rP11;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP12;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP13;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP14;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP15;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP16;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p1r6 = pTemp4 * (-1);

                    //PREGUNTA 2
                    pTemp1 = TotalEncuestas - rP21;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP22;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP23;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP24;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP25;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP26;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p2r6 = pTemp4 * (-1);

                    //PREGUNTA 3
                    pTemp1 = TotalEncuestas - rP31;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP32;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP33;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP34;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP35;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP36;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p3r6 = pTemp4 * (-1);

                    //PREGUNTA 4
                    pTemp1 = TotalEncuestas - rP41;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP42;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;

                    double p4r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP43;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP44;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP45;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP46;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p4r6 = pTemp4 * (-1);

                    //PREGUNTA 5
                    pTemp1 = TotalEncuestas - rP51;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP52;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP53;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP54;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP55;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP56;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p5r6 = pTemp4 * (-1);

                    //PREGUNTA 6
                    pTemp1 = TotalEncuestas - rP61;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r1 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP62;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r2 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP63;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r3 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP64;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r4 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP65;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r5 = pTemp4 * (-1);

                    pTemp1 = TotalEncuestas - rP66;
                    pTemp2 = pTemp1 / TotalEncuestas;
                    pTemp3 = pTemp2 * 100;
                    pTemp4 = pTemp3 - 100;
                    double p6r6 = pTemp4 * (-1);

                    L = new List<CXN_ENCUESTASATIS>();

                    int numEncuesta = 0;

                    foreach (CXN_ENCUESTASATIS rEnc in tempList)
                    {
                        numEncuesta++;

                        L.Add(new CXN_ENCUESTASATIS 
                        {
                            Periodo = comboBox5.Text + " - " + comboBox4.Text,
                            TotalEncuestas = TotalEncuestas,
                            numEncuesta = numEncuesta,
                            P1 = rEnc.P1,
                            P2 = rEnc.P2,
                            P3 = rEnc.P3,
                            P4 = rEnc.P4,
                            P5 = rEnc.P5,
                            P6 = rEnc.P6,
                            Deficiente = p1.ToString("N2"),
                            Malo = p2.ToString("N2"),
                            Regular = p3.ToString("N2"),
                            Bueno = p4.ToString("N2"),
                            Excelente = p5.ToString("N2"),
                            //Parte de abajo 
                            //PREGUNTA 1
                            P1R1Cantidad = rP11.ToString(),
                            P1R2Cantidad = rP12.ToString(),
                            P1R3Cantidad = rP13.ToString(),
                            P1R4Cantidad = rP14.ToString(),
                            P1R5Cantidad = rP15.ToString(),
                            P1R6Cantidad = rP16.ToString(),
                            P1R1Percentual = p1r1.ToString("N2"),
                            P1R2Percentual = p1r2.ToString("N2"),
                            P1R3Percentual = p1r3.ToString("N2"),
                            P1R4Percentual = p1r4.ToString("N2"),
                            P1R5Percentual = p1r5.ToString("N2"),
                            P1R6Percentual = p1r6.ToString("N2"),
                            //PREGUNTA 2
                            P2R1Cantidad = rP21.ToString(),
                            P2R2Cantidad = rP22.ToString(),
                            P2R3Cantidad = rP23.ToString(),
                            P2R4Cantidad = rP24.ToString(),
                            P2R5Cantidad = rP25.ToString(),
                            P2R6Cantidad = rP26.ToString(),
                            P2R1Percentual = p2r1.ToString("N2"),
                            P2R2Percentual = p2r2.ToString("N2"),
                            P2R3Percentual = p2r3.ToString("N2"),
                            P2R4Percentual = p2r4.ToString("N2"),
                            P2R5Percentual = p2r5.ToString("N2"),
                            P2R6Percentual = p2r6.ToString("N2"),
                            //PREGUNTA 3
                            P3R1Cantidad = rP31.ToString(),
                            P3R2Cantidad = rP32.ToString(),
                            P3R3Cantidad = rP33.ToString(),
                            P3R4Cantidad = rP34.ToString(),
                            P3R5Cantidad = rP35.ToString(),
                            P3R6Cantidad = rP36.ToString(),
                            P3R1Percentual = p3r1.ToString("N2"),
                            P3R2Percentual = p3r2.ToString("N2"),
                            P3R3Percentual = p3r3.ToString("N2"),
                            P3R4Percentual = p3r4.ToString("N2"),
                            P3R5Percentual = p3r5.ToString("N2"),
                            P3R6Percentual = p3r6.ToString("N2"),
                            //PREGUNTA 4
                            P4R1Cantidad = rP41.ToString(),
                            P4R2Cantidad = rP42.ToString(),
                            P4R3Cantidad = rP43.ToString(),
                            P4R4Cantidad = rP44.ToString(),
                            P4R5Cantidad = rP45.ToString(),
                            P4R6Cantidad = rP46.ToString(),
                            P4R1Percentual = p4r1.ToString("N2"),
                            P4R2Percentual = p4r2.ToString("N2"),
                            P4R3Percentual = p4r3.ToString("N2"),
                            P4R4Percentual = p4r4.ToString("N2"),
                            P4R5Percentual = p4r5.ToString("N2"),
                            P4R6Percentual = p4r6.ToString("N2"),
                            //PREGUNTA 5
                            P5R1Cantidad = rP51.ToString(),
                            P5R2Cantidad = rP52.ToString(),
                            P5R3Cantidad = rP53.ToString(),
                            P5R4Cantidad = rP54.ToString(),
                            P5R5Cantidad = rP55.ToString(),
                            P5R6Cantidad = rP56.ToString(),
                            P5R1Percentual = p5r1.ToString("N2"),
                            P5R2Percentual = p5r2.ToString("N2"),
                            P5R3Percentual = p5r3.ToString("N2"),
                            P5R4Percentual = p5r4.ToString("N2"),
                            P5R5Percentual = p5r5.ToString("N2"),
                            P5R6Percentual = p5r6.ToString("N2"),
                            //PREGUNTA 6
                            P6R1Cantidad = rP61.ToString(),
                            P6R2Cantidad = rP62.ToString(),
                            P6R3Cantidad = rP63.ToString(),
                            P6R4Cantidad = rP64.ToString(),
                            P6R5Cantidad = rP65.ToString(),
                            P6R6Cantidad = rP66.ToString(),
                            P6R1Percentual = p6r1.ToString("N2"),
                            P6R2Percentual = p6r2.ToString("N2"),
                            P6R3Percentual = p6r3.ToString("N2"),
                            P6R4Percentual = p6r4.ToString("N2"),
                            P6R5Percentual = p6r5.ToString("N2"),
                            P6R6Percentual = p6r6.ToString("N2")
                        });

                        progressBar1.Increment(numEncuesta);
                    }

                    thread = new Thread(M);
                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                    thread.Start();

                    res = "OK";
                }
                else
                {
                    res = "NO";
                }
            }
            catch (Exception ex)
            {
                res = "";
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Satisfaction",
                                             "ZamenisHealth.Reportes.InformeSatisfaccion.rdlc",
                                             L);
             
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void GenerarInformeTrimestral()
        {
            try
            {
                CXN_ENCUESTASATIS getResuTim = repoSatis.getTrimestre(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "CU", Cia);
                if (getResuTim != null)
                {
                    L = new List<CXN_ENCUESTASATIS>();

                    //Respuestas 1
                    int RespuestaGlobalMuyMala = Convert.ToInt32(getResuTim.P1R6Cantidad); // MUY MALA
                    int RespuestaGlobalMala = Convert.ToInt32(getResuTim.P1R2Cantidad); // MALA
                    int RespuestaGlobaRegular = Convert.ToInt32(getResuTim.P1R3Cantidad); // REGULAR
                    int RespuestaGlobaBuena = Convert.ToInt32(getResuTim.P1R4Cantidad); // BUENA
                    int RespuestaGlobaMuyBuena = Convert.ToInt32(getResuTim.P1R5Cantidad); // MUY BUENA

                    //Respuestas 2
                    int RespuestaGlobaAmigosFamiliaDN = Convert.ToInt32(getResuTim.P6R1Cantidad);
                    int RespuestaGlobaAmigosFamiliaN = Convert.ToInt32(getResuTim.P6R2Cantidad);
                    int RespuestaGlobaAmigosFamiliaNI = Convert.ToInt32(getResuTim.P6R3Cantidad);
                    int RespuestaGlobaAmigosFamiliaPS = Convert.ToInt32(getResuTim.P6R4Cantidad);
                    int RespuestaGlobaAmigosFamiliaDS = Convert.ToInt32(getResuTim.P6R5Cantidad);

                    string getMonthDesde = Capitalize(dateTimePicker1.Value.Date.Month).ToString().ToUpper();
                    string getMonthHasta = Capitalize(dateTimePicker2.Value.Date.Month).ToString().ToUpper();

                    //Porcentaje 1
                    int dat1Temp = RespuestaGlobaMuyBuena + RespuestaGlobaBuena;
                    double dat2Temp = getResuTim.TotalEncuestas - dat1Temp;
                    double dat3Temp = dat2Temp / getResuTim.TotalEncuestas;
                    double dat4Temp = dat3Temp * 100;
                    double res1 = (((dat4Temp < 0 ? Math.Abs(dat4Temp) : dat4Temp) - 100)) * -1;

                    //Porcentaje 2
                    int dat1TempRec = RespuestaGlobaAmigosFamiliaPS + RespuestaGlobaAmigosFamiliaDS;
                    double dat2TempRec = getResuTim.TotalEncuestas - dat1TempRec;
                    double dat3TempRec = dat2TempRec / getResuTim.TotalEncuestas;
                    double dat4TempRec = dat3TempRec * 100;
                    double res2 = (((dat4TempRec < 0 ? Math.Abs(dat4TempRec) : dat4TempRec) - 100)) * -1;

                    CXN_CIA C = repoCia.getPrestadorbyCode(Cia);
                    byte[] logo = Convert.FromBase64String(C.Com_Logo);

                    L.Add(new CXN_ENCUESTASATIS
                    {
                        Logo = logo,

                        P1 = RespuestaGlobalMuyMala,
                        P2 = RespuestaGlobalMala,
                        P3 = RespuestaGlobaRegular,
                        P4 = RespuestaGlobaBuena,
                        P5 = RespuestaGlobaMuyBuena,

                        P6R1Cantidad = RespuestaGlobaAmigosFamiliaDN.ToString(),
                        P6R2Cantidad = RespuestaGlobaAmigosFamiliaN.ToString(),
                        P6R3Cantidad = RespuestaGlobaAmigosFamiliaNI.ToString(),
                        P6R4Cantidad = RespuestaGlobaAmigosFamiliaPS.ToString(),
                        P6R5Cantidad = RespuestaGlobaAmigosFamiliaDS.ToString(),

                        Periodo = "PERIODO " + getMonthDesde + " de " + dateTimePicker1.Value.Date.Year.ToString() + " HASTA " + getMonthHasta + " de " + dateTimePicker2.Value.Date.Year.ToString(),

                        Excelente = res1.ToString("N2"),
                        Bueno = res2.ToString("N2")
                    });

                    thread = new Thread(M2);
                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                    thread.Start();

                    res = "OK";
                }
                else
                {
                    res = "NO";
                }
            }
            catch (Exception ex)
            {
                res = "";
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
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
        void M2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Satisfaccion",
                                             "ZamenisHealth.Reportes.InformeSatisfaccionTrimestral.rdlc",
                                             L);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void EncuestasSatis_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Encuesta de Satisfaccion";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerarTodasMes = new ToolStripButton();
            btnGenerarTodasMes = createToolButton("Generar Todas");
            MenuLateral.Items.Add(btnGenerarTodasMes);
            btnGenerarTodasMes.Click += btnZamenis1_ButtonClick;

            ToolStripButton btnGenerarMensual = new ToolStripButton();
            btnGenerarMensual = createToolButton("Generar Mensual");
            MenuLateral.Items.Add(btnGenerarMensual);
            btnGenerarMensual.Click += btnZamenis2_ButtonClick;

            ToolStripButton btnGenerarExtendido = new ToolStripButton();
            btnGenerarExtendido = createToolButton("Generar Extendido");
            MenuLateral.Items.Add(btnGenerarExtendido);
            btnGenerarExtendido.Click += btnZamenis3_ButtonClick;

            ToolStripButton btnGenerarPaciente = new ToolStripButton();
            btnGenerarPaciente = createToolButton("Generar por Paciente");
            MenuLateral.Items.Add(btnGenerarPaciente);
            btnGenerarPaciente.Click += btnZamenis4_ButtonClick;

            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 1;
            comboBox5.SelectedIndex = 1;

            List<CXN_CIA> getCias = repoCia.getAllCompañias();

            foreach (CXN_CIA c in getCias)
            {
                comboBox6.Items.Add(c.Com_Nombre);
            }

            comboBox6.SelectedIndex = 0;
        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = repoCia.getPrestadorbyName(comboBox6.Text).Com_Identificador;
        }
    }
}
