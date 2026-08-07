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

namespace ZamenisHealth.Medicina
{
    public partial class CIE10 : Forma
    {
        private static readonly ICIE10 repositorioCIE10 = new MCIE10();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private string Tipo_His_CIE = "";
        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Cup;
        DataColumn Servicio;
        DataColumn Id;

        DataTable dt2 = new DataTable();
        DataColumn POS2;
        DataColumn Cup2;
        DataColumn Servicio2;
        DataColumn Id2;

        public CIE10(string TH)
        {
            InitializeComponent();
            Tipo_His_CIE = TH;         
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe escribir un texto de busqueda"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Seleccione un criterio de busqueda"); return; }

                if (comboBox1.Text == "Codigo CIE10")
                {
                    PorCodigo();
                }

                if (comboBox1.Text == "Descripcion")
                {
                    PorDesc();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CIE10_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Seleccion de CIE10";

            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            gridZH1.dataGridView1.CellClick += dataGridView1_CellClick;
            gridZH1.CeldaHeight = true;

            gridZH2.dataGridView1.CellClick += dataGridView2_CellClick;
            gridZH2.CeldaHeight = true;

            CargarUltimos();
        }
        void CargarUltimos()
        {
            try
            {
                int Bod = repositorioBodegas.getDatosUser(Contenedor.UsuarioLogueado).Bod_Numero;

                List<CXN_CIE10> _lista = repositorioCIE10.Ultimos(Bod);
                if (_lista != null)
                {
                    EncabezadosUltimos();

                    int Contador = 1;

                    foreach (CXN_CIE10 i in _lista)
                    {
                        DataRow row = dt2.NewRow();

                        row["POS"] = Contador;
                        row["Cup"] = i.Cie_Cod.ToString();
                        row["Servicio"] = i.Cie_Serv.ToString();
                        row["Id"] = Convert.ToInt32(i.Cie_Id);

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH2.dataGridView1, dt2);
                }
                else
                {
                    gridZH2.dataGridView1.DataSource = null;
                    EncabezadosUltimos();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EncabezadosUltimos()
        {
            dt2 = new DataTable();
            POS2 = dt2.Columns.Add("POS", typeof(int));
            Cup2 = dt2.Columns.Add("Cup", typeof(string));
            Servicio2 = dt2.Columns.Add("Servicio", typeof(string));
            Id2 = dt2.Columns.Add("Id", typeof(int));
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Cup = dt.Columns.Add("Cup", typeof(string));
            Servicio = dt.Columns.Add("Servicio", typeof(string));
            Id = dt.Columns.Add("Id", typeof(int));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;
        }
        private void PorCodigo()
        {
            try
            {
                var _lista = repositorioCIE10.PorCodigo(textBox1.Text);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Cup"] = i.Cie_Cod.ToString();
                        row["Servicio"] = i.Cie_Serv.ToString();
                        row["Id"] = Convert.ToInt32(i.Cie_Id);

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void PorDesc()
        {
            try
            {
                var _lista = repositorioCIE10.PorDesc(textBox1.Text);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Cup"] = i.Cie_Cod.ToString();
                        row["Servicio"] = i.Cie_Serv.ToString();
                        row["Id"] = Convert.ToInt32(i.Cie_Id);

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox1.Text)
                {
                    case "Codigo CIE10":
                        label3.Text = "Digite el codigo CIE10 completo y haga click en buscar";
                        break;

                    case "Descripcion":
                        label3.Text = "Escriba una parte del nombre del diagnostico y haga click en buscar";
                        break;

                    default:
                        label3.Text = "Seleccione un metodo de busqueda";
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Cod = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string Serv = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                if (Tipo_His_CIE == "DX1_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox8.Text = Cod;
                    f1.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX3_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox9.Text = Cod;
                    f1.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "DX1_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox8.Text = Cod;
                    f1.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX3_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox9.Text = Cod;
                    f1.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }


                if (Tipo_His_CIE == "DX1_COrdenS")
                {
                    Medicina.OrdenesMedicas f2 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f2.textBox7.Text = Cod;
                    f2.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_COrdenS")
                {
                    Medicina.OrdenesMedicas f3 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f3.textBox8.Text = Cod;
                    f3.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "DX3_COrdenS")
                {
                    Medicina.OrdenesMedicas f4 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f4.textBox9.Text = Cod;
                    f4.textBox12.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMG1")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox39.Text = Cod;
                    f8.textBox38.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCMG2")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f9 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f9.textBox37.Text = Cod;
                    f9.textBox36.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMG3")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f10 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f10.textBox35.Text = Cod;
                    f10.textBox34.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMGANTFAM")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox6.Text = Cod;
                    f8.textBox3.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCFI1")
                {
                    HistoriasClinicas.Historia_Fisiatria f11 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f11.textBox52.Text = Cod;
                    f11.textBox51.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFI2")
                {
                    HistoriasClinicas.Historia_Fisiatria f12 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f12.textBox50.Text = Cod;
                    f12.textBox49.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFI3")
                {
                    HistoriasClinicas.Historia_Fisiatria f13 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f13.textBox48.Text = Cod;
                    f13.textBox47.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFIANTFAM")
                {
                    HistoriasClinicas.Historia_Fisiatria f13 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f13.textBox61.Text = Cod;
                    f13.textBox60.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "Evolucion")
                {
                    HistoriasClinicas.Historia_Evoluciones f14 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Evoluciones>().SingleOrDefault();
                    f14.textBox10.Text = Cod;
                    f14.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HTO")
                {
                    HistoriasClinicas.Historia_TerOcupacional f15 = Application.OpenForms.OfType<HistoriasClinicas.Historia_TerOcupacional>().SingleOrDefault();
                    f15.textBox18.Text = Cod;
                    f15.textBox17.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "TFIS")
                {
                    HistoriasClinicas.Historia_TerFisica f16 = Application.OpenForms.OfType<HistoriasClinicas.Historia_TerFisica>().SingleOrDefault();
                    f16.textBox11.Text = Cod;
                    f16.textBox12.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "PS1")
                {
                    HistoriasClinicas.Historia_Psicologia f17 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f17.textBox37.Text = Cod;
                    f17.textBox36.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "PS2")
                {
                    HistoriasClinicas.Historia_Psicologia f16 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f16.textBox35.Text = Cod;
                    f16.textBox34.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "PS3")
                {
                    HistoriasClinicas.Historia_Psicologia f17 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f17.textBox33.Text = Cod;
                    f17.textBox32.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX1_NotaEJ")
                {
                    HistoriasClinicas.Historia_JefeEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JefeEnfermeria>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "JMComplete")
                {
                    HistoriasClinicas.Historia_JM_Completar f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JM_Completar>().SingleOrDefault();
                    f1.setDX(Cod, Serv);
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCMGANTPAT")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox44.Text = Cod;
                    f8.textBox43.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFIANTPAT")
                {
                    HistoriasClinicas.Historia_Fisiatria f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f8.textBox63.Text = Cod;
                    f8.textBox62.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.CharacterCasing = CharacterCasing.Upper;
        }       
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Cod = gridZH2.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string Serv = gridZH2.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                if (Tipo_His_CIE == "DX1_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox8.Text = Cod;
                    f1.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX3_Nota")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f1.textBox9.Text = Cod;
                    f1.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "DX1_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox8.Text = Cod;
                    f1.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX3_NotaCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f1 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f1.textBox9.Text = Cod;
                    f1.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "DX1_COrdenS")
                {
                    Medicina.OrdenesMedicas f2 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f2.textBox7.Text = Cod;
                    f2.textBox10.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX2_COrdenS")
                {
                    Medicina.OrdenesMedicas f3 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f3.textBox8.Text = Cod;
                    f3.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "DX3_COrdenS")
                {
                    Medicina.OrdenesMedicas f4 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().SingleOrDefault();
                    f4.textBox9.Text = Cod;
                    f4.textBox12.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMG1")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox39.Text = Cod;
                    f8.textBox38.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCMG2")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f9 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f9.textBox37.Text = Cod;
                    f9.textBox36.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMG3")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f10 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f10.textBox35.Text = Cod;
                    f10.textBox34.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCMGANTFAM")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox6.Text = Cod;
                    f8.textBox3.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HCFI1")
                {
                    HistoriasClinicas.Historia_Fisiatria f11 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f11.textBox52.Text = Cod;
                    f11.textBox51.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFI2")
                {
                    HistoriasClinicas.Historia_Fisiatria f12 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f12.textBox50.Text = Cod;
                    f12.textBox49.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFI3")
                {
                    HistoriasClinicas.Historia_Fisiatria f13 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f13.textBox48.Text = Cod;
                    f13.textBox47.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFIANTFAM")
                {
                    HistoriasClinicas.Historia_Fisiatria f13 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f13.textBox61.Text = Cod;
                    f13.textBox60.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "Evolucion")
                {
                    HistoriasClinicas.Historia_Evoluciones f14 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Evoluciones>().SingleOrDefault();
                    f14.textBox10.Text = Cod;
                    f14.textBox11.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "HTO")
                {
                    HistoriasClinicas.Historia_TerOcupacional f15 = Application.OpenForms.OfType<HistoriasClinicas.Historia_TerOcupacional>().SingleOrDefault();
                    f15.textBox18.Text = Cod;
                    f15.textBox17.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "TFIS")
                {
                    HistoriasClinicas.Historia_TerFisica f16 = Application.OpenForms.OfType<HistoriasClinicas.Historia_TerFisica>().SingleOrDefault();
                    f16.textBox11.Text = Cod;
                    f16.textBox12.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "PS1")
                {
                    HistoriasClinicas.Historia_Psicologia f17 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f17.textBox37.Text = Cod;
                    f17.textBox36.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (Tipo_His_CIE == "PS2")
                {
                    HistoriasClinicas.Historia_Psicologia f16 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f16.textBox35.Text = Cod;
                    f16.textBox34.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "PS3")
                {
                    HistoriasClinicas.Historia_Psicologia f17 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Psicologia>().SingleOrDefault();
                    f17.textBox33.Text = Cod;
                    f17.textBox32.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "DX1_NotaEJ")
                {
                    HistoriasClinicas.Historia_JefeEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JefeEnfermeria>().SingleOrDefault();
                    f1.textBox6.Text = Cod;
                    f1.textBox7.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "JMComplete")
                {
                    HistoriasClinicas.Historia_JM_Completar f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JM_Completar>().SingleOrDefault();
                    f1.setDX(Cod, Serv);
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCMGANTPAT")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral>().SingleOrDefault();
                    f8.textBox44.Text = Cod;
                    f8.textBox43.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_His_CIE == "HCFIANTPAT")
                {
                    HistoriasClinicas.Historia_Fisiatria f8 = Application.OpenForms.OfType<HistoriasClinicas.Historia_Fisiatria>().SingleOrDefault();
                    f8.textBox63.Text = Cod;
                    f8.textBox62.Text = Serv;
                    this.Dispose();
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
