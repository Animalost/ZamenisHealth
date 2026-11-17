using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Admision;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class Zonas : Forma2
    {
        private static readonly IZonas repositorioZonas = new MZonas();

        private string Code_Dep_Filtra;
        private string TipoForm;
        private string Filtra_Dep;

        DataTable dt;
        DataColumn Codigo;
        DataColumn Nombre;

        public Zonas(string _tipoForm, string _filtro, string _codeDepFiltra)
        {
            InitializeComponent();
            this.TipoForm = _tipoForm;
            label2.Text = _filtro;
            this.Code_Dep_Filtra = _codeDepFiltra;
        }

        private void Zonas_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Zonas";
                string CodDep = repositorioZonas.DepartamentoCodigo(this.Code_Dep_Filtra);               
                this.Filtra_Dep = CodDep.ToString();

                gridZH1.CeldaHeight = true;
                gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
            }
            catch
            {
                this.Filtra_Dep = "";
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.TipoForm == "CreaEditaPac" && label2.Text == "Departamento")
                {
                    CrearEditarPaciente f1 = Application.OpenForms.OfType<CrearEditarPaciente>().SingleOrDefault();
                    f1.label23.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    this.Dispose();
                    this.Close();
                }

                if (this.TipoForm == "CreaEditaPac" && label2.Text == "Municipio")
                {
                    CrearEditarPaciente f2 = Application.OpenForms.OfType<CrearEditarPaciente>().SingleOrDefault();
                    f2.label20.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    this.Dispose();
                    this.Close();
                }

                if (this.TipoForm == "Admisiones" && label2.Text == "Departamento")
                {
                    Admisiones f3 = Application.OpenForms.OfType<Admisiones>().SingleOrDefault();
                    f3.label21.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f3.label22.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    this.Dispose();
                    this.Close();
                }

                if (this.TipoForm == "Admisiones" && label2.Text == "Municipio")
                {
                    Admisiones f4 = Application.OpenForms.OfType<Admisiones>().SingleOrDefault();
                    f4.label23.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f4.label24.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    this.Dispose();
                    this.Close();
                }

                if (this.TipoForm == "AdmisionesCuraciones" && label2.Text == "Departamento")
                {
                    AdmisionesCuraciones f3 = Application.OpenForms.OfType<AdmisionesCuraciones>().SingleOrDefault();
                    f3.label21.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f3.setCodeZSones("Dep", gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    this.Dispose();
                    this.Close();
                }

                if (this.TipoForm == "AdmisionesCuraciones" && label2.Text == "Municipio")
                {
                    AdmisionesCuraciones f4 = Application.OpenForms.OfType<AdmisionesCuraciones>().SingleOrDefault();
                    f4.label23.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f4.setCodeZSones("Mun", gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (label2.Text == "Departamento")
                {
                    List<CXN_ZONAS> _getList = repositorioZonas._listadoCodigos(textBox1.Text, "", label2.Text);

                    if (_getList != null)
                    {
                        Encabezados();

                        foreach (var i in _getList)
                        {
                            DataRow row = dt.NewRow();

                            row[Codigo] = i.Zon_Dep_Cod.ToString();
                            row[Nombre] = i.Zon_Mun.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }

                        gridZH1.dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        Encabezados();
                    }
                }

                if (label2.Text == "Municipio")
                {
                    List<CXN_ZONAS> _getList = repositorioZonas._listadoCodigos(Filtra_Dep, textBox1.Text, label2.Text);

                    if (_getList != null)
                    {
                        Encabezados();

                        foreach (var i in _getList)
                        {
                            DataRow row = dt.NewRow();

                            row[Codigo] = i.Zon_Dep_Cod.ToString();
                            row[Nombre] = i.Zon_Mun.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }

                        gridZH1.dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        Encabezados();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
