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
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina.Extras;

namespace ZamenisHealth.Medicina
{
    public partial class VerImagenes : Forma
    {
        private static readonly IPacientes repoPacs = new MPacientes();
        private ImagesController oController;

        Comunes.MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;
        DataColumn Grafica;
        DataColumn Nota;
        DataColumn Base;

        public VerImagenes()
        {
            InitializeComponent();
            oController = new ImagesController();
        }    
        private void VerImagenes_Load(object sender, EventArgs e)
        {
            
            Titulo.Text = "Imagenes";

            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            gridZH1.dataGridView1.CellMouseDoubleClick += dataGridView1_CellMouseDoubleClick;
            gridZH1.CeldaHeight = true;

            CargarDocumentos();
        }
        private void CargarDocumentos()
        {
            var ListaDocs = repoPacs.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes P = new Comunes.BuscarPacientes();
            P.Tipo_Busca_Pac = "VerImagen";
            P.ShowDialog();
        }
        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                POS = dt.Columns.Add("POS", typeof(int));
                Id = dt.Columns.Add("Id", typeof(int));
                Admision = dt.Columns.Add("Admision", typeof(int));
                Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
                Profesional = dt.Columns.Add("Profesional", typeof(string));
                Grafica = dt.Columns.Add("Grafica", typeof(string));
                Nota = dt.Columns.Add("Nota", typeof(string));
                Base = dt.Columns.Add("Base", typeof(string));
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
            D.Columns["Id"].Visible = false;
            D.Columns["Grafica"].Visible = false;
            D.Columns["Nota"].Visible = false;
            D.Columns["Base"].Visible = false;
            D.BackgroundColor = Color.White;
        }
        void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new Comunes.MensajesGeneral();

                Dictionary<string, string> getData = Conexion.Conection();
                string cadena = getData["Conexion"];

                CXN_PACIENTES p = repoPacs.LlamarPacienteDOC(comboBox1.Text, textBox1.Text.Trim());
                if (p == null)
                { 
                    MG.TipoImagen = 3;
                    MG.Mensaje = "No se encontró el paciente en la base de datos";
                    MG.ShowDialog();
                    return;
                }

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

                List<CXN_IMAGENES> getImages = oController.GetImages(p.Pac_Id, cadena);
                if (getImages != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_IMAGENES i in getImages)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Ima_Id.ToString();
                        row["Admision"] = i.Ima_Adm.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Ima_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = i.Pac_Acudiente.ToString();
                        row["Base"] = i.Ima_Ruta.ToString();
                        row["Grafica"] = "";
                        row["Nota"] = i.Ima_Nota.ToString().Trim();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;

                        label4.Text = i.Pac_PrimerN.ToString();
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    Encabezados();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "No hay imagenes registradas para este usuario hasta el momento";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return;
            }
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
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

                string getImageB64 = oController.ViewImage(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString());
                if (getImageB64 != null)
                {
                    RegImagenesSubir regImagenes = new RegImagenesSubir(0, 0, true, getImageB64, gridZH1.dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());
                    regImagenes.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
