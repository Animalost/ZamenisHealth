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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Cargos : Forma
    {
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IGenerales repoGenerales = new MGenerales();

        List<CXN_HORARIO> getListas;
        MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Admision;
        DataColumn Paciente;
        DataColumn Profesional;
        DataColumn Fecha;
        DataColumn Estado;

        private ToolStripButton toolStripButton5;

        int Ase;
        string Tipo;
        int Id;
        int Cia;
        int Bod;
        bool Hecho, facturar;

        public Cargos(bool ActiveFacturar)
        {
            InitializeComponent();
            facturar = ActiveFacturar;

            textBox1.Focus();           
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            AdminSystem.ConsAdmision C = new AdminSystem.ConsAdmision();
            C.btnDelete.Visible = false;
            C.ShowDialog();
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            Historial_Medico_1 R = new Historial_Medico_1();
            R.ShowDialog();
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                string CargoDigitado = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la admision para eliminar el cargo grabado, solo podra eliminar cargos de aquellas " +
                            "admisiones que no hallan sido facturadas",
                            "Eliminar Cargos",
                                "");
                if (CargoDigitado != "")
                {
                    var Compriobacion = repoGenerales.ValidarNumerico(CargoDigitado);
                    if (Compriobacion != true)
                    {
                        MessageBox.Show("El valor ingresado no es numerico", "Error Grave", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    bool _delete = repoCargos.deleteCargo(Convert.ToInt32(CargoDigitado));
                    if (_delete != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Este numero de cargo no existe o ya se facturo, para eliminar un cargo este no debe estar facturado";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Cargos Eliminados";
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis5_ButtonClick(object sender, EventArgs e)
        {
            Grabar();
        }
        void Cerrar()
        {
            if (facturar == true)
            {
                this.Dispose();
                this.Close();
                return;
            }

            this.Dispose();
            this.Close();
        }
        void Grid()
        {
            int numberOfRows = 30; // Número de filas vacías que deseas agregar
            for (int i = 0; i < numberOfRows; i++)
            {
                dataGridView2.Rows.Add();
            }
        }
        private void Cargos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cargos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellClick += dataGridView1_CellClick;
            gridZH1.CeldaHeight = true;

            ToolStripButton btnConsultar;
            ToolStripButton btnHistorial;
            ToolStripButton btnEliminar;            

            btnConsultar = new ToolStripButton();
            btnConsultar = createToolButton("Consultar Autori...");
            MenuLateral.Items.Add(btnConsultar);
            btnConsultar.Click += btnZamenis1_ButtonClick;

            btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += btnZamenis2_ButtonClick;

            btnEliminar = new ToolStripButton();
            btnEliminar = createToolButton("Eliminar Cargo");
            MenuLateral.Items.Add(btnEliminar);
            btnEliminar.Click += btnZamenis3_ButtonClick;

            toolStripButton5 = new ToolStripButton();
            toolStripButton5 = createToolButton("Grabar");
            MenuLateral.Items.Add(toolStripButton5);
            toolStripButton5.Click += btnZamenis5_ButtonClick;
            toolStripButton5.Enabled = false;

            panel3.Size = new Size(926, 711);
            panel3.Location = new Point(138, 53);

            
            Grid();
        }
        public void HidePanel3()
        {
            panel3.Visible = false;
        }
        private void Buscar_Cargo()
        {
            MensajesGeneral MG = new MensajesGeneral();

            try
            {
                var getCargo = repoCargos.BuscarCargo(Convert.ToInt32(textBox1.Text));
                if (getCargo == null)
                {
                    //richTextBox1.Text = "";
                    //richTextBox2.Text = "";
                    //richTextBox3.Text = "";
                    richTextBox4.Text = "";
                    //richTextBox5.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox65.Text = "";
                    Ase = 0;
                    Tipo = "";
                    Id = 0;
                    Cia = 0;
                    Bod = 0;

                    gridZH1.dataGridView1.Enabled = true;
                    dataGridView2.Enabled = false;

                    toolStripButton5.Enabled = false;

                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La nota no existe";
                    MG.ShowDialog();                    
                }
                else if (getCargo.Hor_Pac_Ase == 0)
                {
                    //richTextBox1.Text = "";
                    //richTextBox2.Text = "";
                    //richTextBox3.Text = "";
                    richTextBox4.Text = "";
                    //richTextBox5.Text = "";
                    textBox2.Text = getCargo.Hor_Imp_Age.ToString();
                    textBox3.Text = Convert.ToDateTime(getCargo.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    textBox4.Text = getCargo.Hor_Observacion.ToString();
                    textBox65.Text = getCargo.PacienteAseguradora.ToString();
                    Ase = 0;
                    Tipo = "";
                    Id = 0;
                    Cia = 0;
                    Bod = 0;

                    toolStripButton5.Enabled = false;

                    MG.TipoImagen = 1000;
                    MG.Mensaje = "A esta nota ya se le grabo cargos";
                    MG.ShowDialog();

                    label13.Visible = false;
                    gridZH1.dataGridView1.Enabled = true;
                    dataGridView2.Enabled = false;
                }
                else
                {
                    textBox2.Text = getCargo.Hor_Imp_Age.ToString();
                    textBox3.Text = Convert.ToDateTime(getCargo.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    textBox4.Text = getCargo.Hor_Observacion.ToString();
                    textBox65.Text = getCargo.PacienteAseguradora.ToString();
                    Ase = Convert.ToInt32(getCargo.Hor_Pac_Ase);
                    Tipo = getCargo.Hor_Pac_Tipo_Serv.ToString();
                    Id = Convert.ToInt32(getCargo.Hor_Pac_Id);
                    Cia = Convert.ToInt32(getCargo.Hor_Pac_Cia);
                    Bod = Convert.ToInt32(getCargo.Hor_Pac_Bod);
                    textBox1.Enabled = false;

                    button1.Enabled = false;
                    label13.Visible = true;

                    //richTextBox1.Text = getCargo.PacienteDireccion.ToString().ToUpper();
                    //richTextBox2.Text = getCargo.PacienteIdentificacion.ToString().ToUpper();
                    //richTextBox3.Text = getCargo.PacienteNombre.ToString().ToUpper();
                    richTextBox4.Text = getCargo.PacienteTelefono.ToString().ToUpper();
                    //richTextBox5.Text = getCargo.Hor_Autoriza.ToString().ToUpper();

                    toolStripButton5.Enabled = true;
                    gridZH1.dataGridView1.Enabled = false;
                    dataGridView2.Enabled = true;

                    if (dataGridView2.Rows.Count > 0) // Verifica si hay al menos una fila
                    {
                        dataGridView2.CurrentCell = dataGridView2.Rows[0].Cells[0]; // Establece el foco en la primera celda
                        dataGridView2.BeginEdit(true); // Activa el modo de edición
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                MG.TipoImagen = 1000;
                MG.Mensaje = "La nota no existe";
                MG.ShowDialog();
            }
        }
        void EncabezadosGrilla()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }             
        async Task Carga_Grilla()
        {
            try
            {
                getListas = new List<CXN_HORARIO>();
                getListas = await repoCargos.ListaCargos(Convert.ToDateTime(dateTimePicker1.Value.Date), Hecho);
                if (getListas != null)
                {
                    EncabezadosGrilla();

                    int Contador = 1;

                    foreach (CXN_HORARIO h in getListas)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = Convert.ToInt32(h.Hor_Id);
                        row["Paciente"] = h.Hor_Imp_Age.ToString();
                        row["Fecha"] = Convert.ToDateTime(h.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = h.Hor_Observacion.ToString();                        
                        row["Estado"] = h.Hor_Estado.ToString();                    

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }

                else
                {
                    EncabezadosGrilla();
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos()
        {
            gridZH1.dataGridView1.DataSource = dt;
            gridZH1.dataGridView1.Columns["POS"].Visible = false;
        }
        void Shows()
        {
            pictureBox1.Visible = true;
        }
        void Hides()
        {
            pictureBox1.Visible = false;
        }       
        private void AbrirFormEnPanel(object Formhijo)
        {
            if (this.panel3.Controls.Count > 0)
                this.panel3.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panel3.Controls.Add(fh);
            this.panel3.Tag = fh;
            panel3.Visible = true;
            fh.Show();
        }
        private void label13_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (Id == 0 || textBox2.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay paciente seleccionado";
                    MG.ShowDialog();
                    return;
                }

                AbrirFormEnPanel(new Extras.CargosAnterior(Id));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cargos_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F2)
                {
                    if (toolStripButton5.Enabled == false)
                    {
                        return;
                    }
                    else
                    {
                        Grabar();
                    }
                }

                if (e.KeyData == Keys.Enter)
                {
                    if (button1.Enabled == true)
                    {
                        Buscar_Cargo();
                    }
                }

                if (e.KeyData == Keys.Escape)
                {
                    Cerrar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void Grabar()
        {
            try
            {
                richTextBox4.Focus(); //focus para saltar el ultimo dato digitado del grid

                dataGridView2.CellValueChanged -= dataGridView2_CellValueChanged;

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.IsNewRow || row.Cells.Cast<DataGridViewCell>().All(c => c.Value == null || string.IsNullOrWhiteSpace(c.Value.ToString())))
                    {
                        continue; // Salta a la siguiente fila si la actual está vacía
                    }

                    string codigo = row.Cells["Codigo"]?.Value?.ToString();
                    string item = row.Cells["Item"]?.Value?.ToString();
                    string cantidad = row.Cells["Cantidad"]?.Value?.ToString();
                    string unitario = row.Cells["Unitario"]?.Value?.ToString();
                    string total = row.Cells["Total"]?.Value?.ToString();
                    string descripcion = row.Cells["Descripcion"]?.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(codigo) ||
                        string.IsNullOrWhiteSpace(item) ||
                        string.IsNullOrWhiteSpace(cantidad) ||
                        string.IsNullOrWhiteSpace(unitario) ||
                        string.IsNullOrWhiteSpace(total) ||
                        string.IsNullOrWhiteSpace(descripcion))
                    {
                        // Si alguna está vacía, omitimos esta fila y pasamos a la siguiente
                        continue;
                    }

                    if (codigo == null || item == null || cantidad == null || unitario == null || total == null || descripcion == null)
                    {
                        TXTException T = new TXTException
                        {
                            FechaHora = DateTime.Now,
                            Error = "No hay datos para grabar del cargo: " + textBox1.Text + " --> NO GRABADO",
                            Formulario = this.Name,
                            Metodo = OverridesExtern.GetCurrentMethodName(),
                            Usuario = Contenedor.UsuarioLogueado
                        };

                        OverridesExtern.GenerarTXTException(T);
                    }
                    else
                    {
                        ListaProd P1 = repoCargos.DatoProd(Ase, codigo.ToString());
                        if (P1 == null)
                        {
                            TXTException T = new TXTException
                            {
                                FechaHora = DateTime.Now,
                                Error = "Codigo: " + codigo.ToString() + " del cargo: " + textBox1.Text + " no grabado",
                                Formulario = this.Name,
                                Metodo = OverridesExtern.GetCurrentMethodName(),
                                Usuario = Contenedor.UsuarioLogueado
                            };

                            OverridesExtern.GenerarTXTException(T);
                        }
                        else
                        {
                            CXN_CARGOS C = new CXN_CARGOS
                            {
                                Car_Adm_Id = Convert.ToInt32(textBox1.Text),
                                Car_Pac = Id,
                                Car_Cia = Cia,
                                Car_Ase = Ase,
                                Car_Prof = Bod,
                                Car_Fecha = Convert.ToDateTime(textBox3.Text),
                                Car_Estado = "G",
                                Car_Tipo = "Cargo",
                                Car_Cod = codigo.ToString().Trim(),
                                Car_Cant = Convert.ToInt32(cantidad),
                                Car_Val_Un = Convert.ToInt32(P1.ValorProducto),
                                Car_Val_Tot = Convert.ToInt32(P1.ValorProducto) * Convert.ToInt32(cantidad),
                                Car_Item = P1.NombreProducto.Trim(),
                                Car_Detalle = P1.DetalleProducto.Trim(),
                                Car_Usr_Graba = Contenedor.UsuarioLogueado,
                                Car_Tipo_Serv = Tipo
                            };

                            await repoCargos.SaveCargo(C);
                        }
                    }
                }

                label13.Visible = false;

                richTextBox4.Text = "";
                gridZH1.dataGridView1.Enabled = true;
                dataGridView2.Enabled = false;
                
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Cargo grabado correctamente";
                MG.TipoImagen = 3;
                MG.ShowDialog();

                Blanquear();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
            finally
            {
                // Reactivar el evento CellValueChanged después de modificar las celdas
                dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            }
        }        
        async void Blanquear()
        {
            try
            {
                Shows();
                await Carga_Grilla();

                textBox1.Enabled = true;
                button1.Enabled = true;

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox65.Text = "";

               /* richTextBox1.Text = "";
                richTextBox2.Text = "";
                richTextBox3.Text = "";
                richTextBox4.Text = "";
                richTextBox5.Text = "";*/

                Ase = 0;
                Tipo = "";
                Id = 0;
                Cia = 0;
                Bod = 0;

                label13.Visible = false;
                gridZH1.dataGridView1.Enabled = true;
                dataGridView2.Rows.Clear();
                Grid();
                dataGridView2.Enabled = false;                

                textBox1.Focus();

                Hides();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }          
        private async void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Shows();
            await Carga_Grilla();           
            Hides();
        }        
        private async void label14_Click(object sender, EventArgs e)
        {
            Hecho = false;
            
            Shows();
            await Carga_Grilla();
            Hides();
        }
        private async void label24_Click(object sender, EventArgs e)
        {
            Hecho = true;        

            Shows();
            await Carga_Grilla();
            Hides();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBox1.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }       
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView2.CellValueChanged -= dataGridView2_CellValueChanged;

                if (dataGridView2.Rows[e.RowIndex].Cells[0].Value != null &&
                !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()))
                {
                    ListaProd DatoProd = repoCargos.DatoProd(Ase, dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
                    if (DatoProd != null)
                    {
                        dataGridView2.Rows[e.RowIndex].Cells[1].Value = DatoProd.NombreProducto;
                        dataGridView2.Rows[e.RowIndex].Cells[3].Value = Convert.ToInt32(DatoProd.ValorProducto);
                        dataGridView2.Rows[e.RowIndex].Cells[5].Value = DatoProd.DetalleProducto;

                        if (dataGridView2.Rows[e.RowIndex].Cells[2].Value != null &&
                            !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString()) &&
                            dataGridView2.Rows[e.RowIndex].Cells[3].Value != null &&
                            !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString()))
                        {
                            dataGridView2.Rows[e.RowIndex].Cells[4].Value = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[2].Value) * Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[3].Value);
                        }
                        else
                        {
                            dataGridView2.Rows[e.RowIndex].Cells[3].Value = 0;
                            dataGridView2.Rows[e.RowIndex].Cells[4].Value = 0;
                        }
                    }
                    else
                    {
                        dataGridView2.Rows[e.RowIndex].Cells[1].Value = null;
                        dataGridView2.Rows[e.RowIndex].Cells[3].Value = null;
                        dataGridView2.Rows[e.RowIndex].Cells[4].Value = null;
                        dataGridView2.Rows[e.RowIndex].Cells[5].Value = null;
                    }       
                }
                else
                {
                    dataGridView2.Rows[e.RowIndex].Cells[1].Value = null;
                    dataGridView2.Rows[e.RowIndex].Cells[3].Value = null;
                    dataGridView2.Rows[e.RowIndex].Cells[4].Value = null;
                    dataGridView2.Rows[e.RowIndex].Cells[5].Value = null;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
            finally
            {
                // Reactivar el evento CellValueChanged después de modificar las celdas
                dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Buscar_Cargo();
        }
        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.BeginEdit(true);
        }
    }
}
