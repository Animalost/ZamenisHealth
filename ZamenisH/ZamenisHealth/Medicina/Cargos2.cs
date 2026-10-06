using Domain;
using Domain.CONSUMOS;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.AdminSystem;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Cargos2 : Forma2
    {
        private ICargos repoCargos;
        private IAgendaC repoAgenda;
        private IProdsConsumo repoProdConsumo;
        private IMovimientosConsumo repoMovConsumo;
        private IAsignacion oAsignacion;
        private MensajesGeneral MG;

        private int Ase, Id, Cia, Bod, Admision;
        public int IdProducto;
        private string Tipo;
        private DateTime fecha;
        private bool InicialGrid;
        public int CantidadCargo;

        public Cargos2()
        {
            InitializeComponent();
            SoloNumeros(textBox1);
            repoCargos = new MCargos();
            repoAgenda = new MAgendaC();
            repoProdConsumo = new MProdsConsumo();
            oAsignacion = new MAsignacion();
            repoMovConsumo = new MMovimientosConsumo();
        }
        public Cargos2(int admision)
        {
            InitializeComponent();
            SoloNumeros(textBox1);
            repoCargos = new MCargos();
            repoAgenda = new MAgendaC();
            repoProdConsumo = new MProdsConsumo();
            oAsignacion = new MAsignacion();
            repoMovConsumo = new MMovimientosConsumo();
            Admision = admision;
        }

        void BuscarBarCode()
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    CXN_INVENTARIO DatoProd = repoCargos.DatoProdBarCode(Ase, textBox2.Text.Trim());
                    if (DatoProd == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "El codigo de barras no existe"
                        };

                        MG.ShowDialog();

                        textBox2.Focus();
                    }
                    else
                    {
                        if (InicialGrid == true)
                        {
                            dataGridView1.DataSource = null;
                            InicialGrid = false;

                            dataGridView1.Columns.Add("CodeBar", "Codigo de Barras");
                            dataGridView1.Columns.Add("CodeOrigin", "Codigo Origen");
                            dataGridView1.Columns.Add("ProductName", "Nombre del Producto");
                            dataGridView1.Columns.Add("Cantidad", "Cantidad");
                            dataGridView1.Columns.Add("VR", "VR");
                            dataGridView1.Columns.Add("DETALLE", "DETALLE");
                            dataGridView1.Columns.Add("CODEPS", "CODEPS");
                            dataGridView1.Columns.Add("FRACCION", "FRACCION");

                            dataGridView1.Columns["VR"].Visible = false;
                            dataGridView1.Columns["DETALLE"].Visible = false;
                            dataGridView1.Columns["CODEPS"].Visible = false;
                            dataGridView1.Columns["FRACCION"].Visible = false;

                            dataGridView1.Columns["CodeBar"].Width = 200;
                            dataGridView1.Columns["CodeOrigin"].Width = 100;
                            dataGridView1.Columns["ProductName"].Width = 350;
                            dataGridView1.Columns["Cantidad"].Width = 90;
                        }

                        //AQUI ES DONDE ABRO EL FORM DE CANTIDAD
                        Cargos3 C3 = new Cargos3();
                        C3.ShowDialog();

                        if (CantidadCargo <= 0)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "La cantidad a ingresar del cargo debe ser superior a 0",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            string parcial = Admision > 0 ? checkBox1.Checked == true ? "S" : "N" : "N";

                            dataGridView1.Rows.Add(textBox2.Text.Trim(),
                                                    DatoProd.InvCod,
                                                    DatoProd.InvItem,
                                                    CantidadCargo,
                                                    DatoProd.InvPrecio,
                                                    DatoProd.InvDetalle,
                                                    DatoProd.InvImagen,
                                                    parcial);

                            textBox2.Text = "";
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            BuscarBarCode();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int fila = e.RowIndex;
                dataGridView1.Rows.RemoveAt(fila);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void boton1_Click(object sender, EventArgs e)
        {
            Grabar();
        }
        async void Grabar()
        {
            try
            {
                string AdherenciaEnfermeria = "";

                if (Admision > 0 && comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar consultorio de consumos",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow || row.Cells.Cast<DataGridViewCell>().All(c => c.Value == null || string.IsNullOrWhiteSpace(c.Value.ToString())))
                    {
                        continue; // Salta a la siguiente fila si la actual está vacía
                    }

                    string codigo = row.Cells["CodeOrigin"]?.Value?.ToString();
                    string item = row.Cells["ProductName"]?.Value?.ToString();
                    string cantidad = row.Cells["Cantidad"]?.Value?.ToString();
                    string unitario = row.Cells["VR"]?.Value?.ToString();
                    int total = Convert.ToInt32(row.Cells["VR"]?.Value) * Convert.ToInt32(row.Cells["Cantidad"]?.Value);
                    string descripcion = row.Cells["DETALLE"]?.Value?.ToString();
                    string codigoeps = row.Cells["CODEPS"]?.Value?.ToString();
                    string FRACCION = row.Cells["FRACCION"]?.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(codigo) ||
                        string.IsNullOrWhiteSpace(item) ||
                        string.IsNullOrWhiteSpace(cantidad) ||
                        string.IsNullOrWhiteSpace(unitario) ||
                        string.IsNullOrWhiteSpace(total.ToString()) ||
                        string.IsNullOrWhiteSpace(descripcion) ||
                        string.IsNullOrWhiteSpace(codigoeps))
                    {
                        // Si alguna está vacía, omitimos esta fila y pasamos a la siguiente
                        continue;
                    }

                    if (codigo == null || item == null || cantidad == null || unitario == null || total.ToString() == null || descripcion == null || codigoeps == null)
                    {
                        TXTException T = new TXTException
                        {
                            FechaHora = DateTime.Now,
                            Error = "No hay datos para grabar del cargo: " + textBox1.Text + " --> NO GRABADO --> " + codigo,
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
                                Car_Fecha = Convert.ToDateTime(fecha),
                                Car_Estado = "G",
                                Car_Tipo = "Cargo",
                                Car_Cod = codigo.ToString().Trim(),
                                Car_Cant = Convert.ToInt32(cantidad),
                                Car_Val_Un = Convert.ToInt32(P1.ValorProducto),
                                Car_Val_Tot = Convert.ToInt32(P1.ValorProducto) * Convert.ToInt32(cantidad),
                                Car_Item = P1.NombreProducto.Trim(),
                                Car_Detalle = P1.DetalleProducto.Trim(),
                                Car_Usr_Graba = Contenedor.UsuarioLogueado,
                                Car_Tipo_Serv = Tipo,
                                CarCodEPSConvenio = P1.CodigoEPS.ToString().Trim()
                            };

                            await repoCargos.SaveCargo(C);

                            AdherenciaEnfermeria = AdherenciaEnfermeria + item + "\r";

                            if (Admision > 0)
                            {
                                if (FRACCION != "S")
                                {
                                    List<CON_PRODUCTOS> dataProdConsumo = repoProdConsumo.GetProductoByCodeInterno(C.Car_Cod);
                                    if (dataProdConsumo != null)
                                    {
                                        dataProdConsumo = dataProdConsumo.Where(x => x.Con_Prod_Status == true).ToList();

                                        CON_CONSUMOS Consumo;
                                        CON_ASIGNACION dataUser = oAsignacion.GetBodega(comboBox1.Text);

                                        if (dataProdConsumo.Count > 1)
                                        {
                                            Cargos4 C4 = new Cargos4(dataProdConsumo);
                                            C4.ShowDialog();

                                            Consumo = new CON_CONSUMOS
                                            {
                                                Con_Cons_Cantidad = C.Car_Cant,
                                                Con_Cons_Fecha = DateTime.Now.Date,
                                                Con_Cons_Idconsultorio = dataUser.Asi_Number,
                                                Con_Cons_IdProducto = IdProducto,
                                                Con_Cons_Status = true,
                                                Con_Cons_User = Contenedor.UsuarioLogueado
                                            };
                                        }
                                        else
                                        {
                                            Consumo = new CON_CONSUMOS
                                            {
                                                Con_Cons_Cantidad = C.Car_Cant,
                                                Con_Cons_Fecha = DateTime.Now.Date,
                                                Con_Cons_Idconsultorio = dataUser.Asi_Number,
                                                Con_Cons_IdProducto = dataProdConsumo[0].Con_Prod_Id,
                                                Con_Cons_Status = true,
                                                Con_Cons_User = Contenedor.UsuarioLogueado
                                            };
                                        }

                                        bool save = repoMovConsumo.SaveConsumo(Consumo);
                                    }
                                }
                            }
                        }
                    }
                }

                MG = new Comunes.MensajesGeneral
                {
                    Mensaje = "Cargo grabado correctamente",
                    TipoImagen = 3
                };

                MG.ShowDialog();

                if (Admision > 0)
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f1 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().LastOrDefault();
                    f1.richTextBox1.Text = AdherenciaEnfermeria.ToString();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarBarCode();
            }
        }
        private void Cargos2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                Grabar();
            }
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            BarCodesForm B = new BarCodesForm();
            B.ShowDialog();
        }
        void CargarBodegas()
        {
            List<CON_ASIGNACION> getBods = oAsignacion.GetBodegas();
            if (getBods != null)
            {
                getBods = getBods.Where(x => x.Asi_Status == true).ToList();

                foreach (CON_ASIGNACION i in getBods)
                {
                    comboBox1.Items.Add(i.Asi_Name);
                }

                comboBox1.SelectedItem = 0;
            }
        }
        private void Cargos2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cargos CodeBar GS1-128";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarBodegas();

            InicialGrid = true;

            if (Admision > 0)
            {
                textBox1.Text = Admision.ToString();
                textBox1.Enabled = false;
                checkBox1.Visible = true;
                label3.Visible = true;
                comboBox1.Visible = true;
                boton2.Visible = false;
                BuscarNota();
            }
        }
        void BuscarNota()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 1000,
                        Mensaje = "Debe digitar un cargo valido"
                    };

                    MG.ShowDialog();
                    return;
                }

                if (Admision == 0)
                {
                    CXN_HORARIO getCargo = repoCargos.BuscarCargo(Convert.ToInt32(textBox1.Text));
                    if (getCargo == null)
                    {
                        Encabezados();
                        panel1.Enabled = false;

                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "La nota de enfermeria no existe"
                        };

                        MG.ShowDialog();
                    }
                    else if (getCargo.Hor_Pac_Ase == 0)
                    {
                        Encabezados();
                        panel1.Enabled = false;

                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "Esta nota de enfermeria ya tiene cargos grabados"
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        string dataCargo = $"PACIENTE: {getCargo.Hor_Imp_Age.ToString()}\n" +
                                           $"FECHA ATENCION: {Convert.ToDateTime(getCargo.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"])}\r" +
                                           $"ASEGURADORA: {getCargo.PacienteAseguradora.ToString()}\r" +
                                           $"PROFESIONAL: {getCargo.Hor_Observacion.ToString()}\r\r" +
                                           $"ADHERENCIA: \r\r" + getCargo.PacienteTelefono.ToString().ToUpper();

                        Ase = Convert.ToInt32(getCargo.Hor_Pac_Ase);
                        Tipo = getCargo.Hor_Pac_Tipo_Serv.ToString();
                        Id = Convert.ToInt32(getCargo.Hor_Pac_Id);
                        Cia = Convert.ToInt32(getCargo.Hor_Pac_Cia);
                        Bod = Convert.ToInt32(getCargo.Hor_Pac_Bod);
                        textBox1.Enabled = false;
                        fecha = getCargo.Hor_Pac_Fecha_Cita;

                        richTextBox1.Text = dataCargo;

                        panel1.Enabled = true;
                        textBox2.Focus();
                        return;
                    }

                    Ase = 0;
                    Tipo = "";
                    Id = 0;
                    Cia = 0;
                    Bod = 0;

                    Encabezados();

                    textBox2.Text = "";
                }
                else
                {
                    otrosDatosPacienteHorario DataPac = repoAgenda.cargarAdmision(Admision, "'P','H','A'");
                    if (DataPac == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No es posible anexar cargos a esta admision, probablemente ya esten grabados.  Contacte al administrador",
                            TipoImagen = 0
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        string dataCargo = $"PACIENTE: {DataPac.Hor_Imp_Age.ToString()}\n" +
                                           $"FECHA ATENCION: {Convert.ToDateTime(DataPac.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"])}\r" +
                                           $"ASEGURADORA: {DataPac.PacienteAseguradora.ToString()}\r" +
                                           $"PROFESIONAL: {DataPac.Hor_Observacion.ToString()}";

                        Ase = Convert.ToInt32(DataPac.Hor_Pac_Ase);
                        Tipo = DataPac.Hor_Pac_Tipo_Serv.ToString();
                        Id = Convert.ToInt32(DataPac.Hor_Pac_Id);
                        Cia = Convert.ToInt32(DataPac.Hor_Pac_Cia);
                        Bod = Convert.ToInt32(DataPac.Hor_Pac_Bod);
                        fecha = DataPac.Hor_Pac_Fecha_Cita;

                        richTextBox1.Text = dataCargo;

                        panel1.Enabled = true;
                        textBox2.Focus();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                MG = new MensajesGeneral()
                {
                    TipoImagen = 1000,
                    Mensaje = "La nota no existe"
                };

                MG.ShowDialog();
            }
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarNota();
            }                
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;

            dataGridView1.Columns.Add("CodeBar", "Codigo de Barras");
            dataGridView1.Columns.Add("CodeOrigin", "Codigo Origen");
            dataGridView1.Columns.Add("ProductName", "Nombre del Producto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            dataGridView1.Columns.Add("VR", "VR");
            dataGridView1.Columns.Add("DETALLE", "DETALLE");
            dataGridView1.Columns.Add("CODEPS", "CODEPS");
            dataGridView1.Columns.Add("FRACCION", "FRACCION");

            dataGridView1.Columns["VR"].Visible = false;
            dataGridView1.Columns["DETALLE"].Visible = false;
            dataGridView1.Columns["CODEPS"].Visible = false;
            dataGridView1.Columns["FRACCION"].Visible = false;

            dataGridView1.Columns["CodeBar"].Width = 200;
            dataGridView1.Columns["CodeOrigin"].Width = 100;
            dataGridView1.Columns["ProductName"].Width = 350;
            dataGridView1.Columns["Cantidad"].Width = 90;
        }
    }
}
