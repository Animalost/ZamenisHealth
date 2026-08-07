using APIController.Images;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina.Extras;

namespace ZamenisHealth.Medicina
{
    public partial class ResumenHCMGNotas : Forma2
    {
        private static readonly IMedicinaGeneral repoMG = new MMedicinaGeneral();
        private static readonly IImagenes repoImagenes = new MImagenes();
        private ImagesController oController;

        private MensajesGeneral MG;
        public int Paci_Resumen_His;

        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;

        DataTable dt2;
        DataColumn POS2;
        DataColumn Admision2;
        DataColumn Fecha2;
        DataColumn Profesional2;
        DataColumn Grafica;
        DataColumn Ruta;

        public ResumenHCMGNotas()
        {
            InitializeComponent();            
            oController = new ImagesController();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Profesional = dt.Columns.Add("Profesional", typeof(string));

            Encabezados2();
        }
        void Encabezados2()
        {
            gridZH2.dataGridView1.DataSource = null;

            dt2 = new DataTable();
            POS2 = dt2.Columns.Add("POS2", typeof(int));
            Admision2 = dt2.Columns.Add("Admision2", typeof(string));
            Fecha2 = dt2.Columns.Add("Fecha2", typeof(DateTime));
            Profesional2 = dt2.Columns.Add("Profesional2", typeof(string));
            Grafica = dt2.Columns.Add("Grafica", typeof(byte[]));
            Ruta = dt2.Columns.Add("Ruta", typeof(string));
        }
        private void ResumenHCMGNotas_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Resumen Historia Clinica";

            try
            {
                richTextBox1.ContextMenu = new ContextMenu();
                textBox3.ContextMenu = new ContextMenu();

                gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
                gridZH1.CeldaHeight = true;

                gridZH2.dataGridView1.CellClick += dataGridView2_CellClick;
                gridZH2.CeldaHeight = true;

                var getHistorial = repoMG.ResumenHCMGNotas(Paci_Resumen_His);
                if (getHistorial != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in getHistorial)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.HC_Adm.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.HC_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = i.HC_PManejo.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    Encabezados();

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este paciente no tiene mas historial";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
        }
        void Estilos2(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS2"].Visible = false;
            D.Columns["Grafica"].Visible = false;
            D.Columns["Ruta"].Visible = false;
        }
        //CARGAR DESDE LA API
        private void Carga_List2(int Adm_Ima)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();
                string cadena = getData["Conexion"];

                //Generar token de autorizacion
                bool Log = oController.Loguear(Program.URLApiConexion, Contenedor.UsuarioLogueado, cadena);
                if (Log == false)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro conectar al servicio API Web, intente nuevamente o mas tarde";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                List<CXN_IMAGENES> get_Images = repoImagenes.getImagenesByAdmition(Adm_Ima);
                if (get_Images != null)
                {
                    Encabezados2();

                    int Contador = 1;

                    foreach (var i in get_Images)
                    {
                        DataRow row = dt2.NewRow();

                        row["POS2"] = Contador;
                        row["Admision2"] = i.Ima_Adm.ToString();
                        row["Fecha2"] = Convert.ToDateTime(i.Ima_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional2"] = i.Pac_PrimerA.Trim();
                        row["Grafica"] = null;
                        row["Ruta"] = i.Ima_Ruta.ToString();

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos2(gridZH2.dataGridView1, dt2);
                }
                else
                {
                    Encabezados2();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                var getResum = repoMG.getResumen(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                if (getResum != null)
                {
                    var DAT11 = new StringBuilder();
                    DAT11.AppendLine("DATOS DE LA HISTORIA - " + gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    var DAT1 = DAT11.ToString();

                    var DAT22 = new StringBuilder();
                    DAT22.AppendLine("----> Plan de Manejo: ");
                    var DAT2 = DAT22.ToString();

                    var DAT33 = new StringBuilder();
                    DAT33.AppendLine("----> Descripcion de la Herida: ");
                    var DAT3 = DAT33.ToString();

                    var DAT44 = new StringBuilder();
                    DAT44.AppendLine("----> Patologia: ");
                    var DAT4 = DAT44.ToString();

                    var DAT55 = new StringBuilder();
                    DAT55.AppendLine("----> Estado de la Piel: ");
                    var DAT5 = DAT55.ToString();

                    var DAT66 = new StringBuilder();
                    DAT66.AppendLine("----> Enfermedad Actual: ");
                    var DAT6 = DAT66.ToString();

                    var DAT77 = new StringBuilder();
                    DAT77.AppendLine("----> Caracteristica del Tejido: ");
                    var DAT7 = DAT77.ToString();

                    var DAT771 = new StringBuilder();
                    DAT771.AppendLine("----> Notas Aclaratorias: ");
                    var DAT8 = DAT771.ToString();

                    richTextBox1.Text = "";
                    string Dato_PAC = DAT1 + "\n\r" +
                                      DAT2 + getResum.HC_PManejo.ToString() + "\n\r" +
                                      DAT3 + getResum.HC_DescHer.ToString() + "\n\r" +
                                      DAT4 + getResum.HC_Patologia.ToString() + "\n\r" +
                                      DAT5 + getResum.HC_Piel.ToString() + "\n\r" +
                                      DAT6 + getResum.HC_EnfA.ToString() + "\n\r" +
                                      DAT7 + getResum.HC_CaracTej.ToString() + "\n\r" +
                                      DAT8 + getResum.HC_Nota_Acl.ToString();

                    richTextBox1.Text = Dato_PAC;
                    textBox1.Text = getResum.HC_DX1 + " - " + getResum.HC_DX1T.ToString();
                    textBox3.Text = getResum.HC_CupCatalogo.ToString() + " - " + getResum.HC_ServCatalogo.ToString();                 

                    Carga_List2(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se puede mostrar este historial, hay un inconveniente en esta fecha";
                    MG.ShowDialog();

                    Encabezados2();
                    richTextBox1.Text = "";
                    textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        //Cargar desde la API
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();
                string cadena = getData["Conexion"];

                //Generar token de autorizacion
                bool Log = oController.Loguear(Program.URLApiConexion, Contenedor.UsuarioLogueado, cadena);
                if (Log == false)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro conectar al servicio API Web, intente nuevamente o mas tarde";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                string getImageB64 = oController.ViewImage(gridZH2.dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString());
                if (getImageB64 != null)
                {
                    panel1.Visible = true;
                    RegImagenesSubir regImagenesSubir = new RegImagenesSubir(getImageB64);
                    regImagenesSubir.TopMost = true;
                    regImagenesSubir.ShowDialog();
                }
                else
                {
                    panel1.Visible = false;
                    richTextBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + gridZH2.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }    
        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }
        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
