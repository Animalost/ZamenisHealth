using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Cie10Admin : Forma
    {
        private static readonly ICIE10 repoCIE10 = new MCIE10();

        public Cie10Admin()
        {
            InitializeComponent();
        }

        private void Cie10Admin_Load(object sender, EventArgs e)
        {
            Titulo.Text = "CIE10";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            ToolStripButton btnCrear = new ToolStripButton();
            btnCrear = createToolButton("Crear");
            MenuLateral.Items.Add(btnCrear);
            btnCrear.Click += button2_Click;

            
            textBox2.MaxLength = 4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
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
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Cup", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Servicio", 350, HorizontalAlignment.Left);
            listView1.Columns.Add("Id", 0, HorizontalAlignment.Left);
        }

        private void PorCodigo()
        {
            try
            {
                var lista = repoCIE10.PorCodigo(textBox1.Text);
                if (lista != null)
                {
                    Encabezados();

                    foreach (var i in lista)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Cie_Cod.ToString(),
                            i.Cie_Serv.ToString(),
                            i.Cie_Id.ToString()
                        }));
                    }
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void PorDesc()
        {
            try
            {
                var lista = repoCIE10.PorDesc(textBox1.Text);
                if (lista != null)
                {
                    Encabezados();

                    foreach (var i in lista)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Cie_Cod.ToString(),
                            i.Cie_Serv.ToString(),
                            i.Cie_Id.ToString()
                        }));
                    }
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar los dos campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar los dos campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                string _existente = repoCIE10.BuscaDX(textBox2.Text);
                if (_existente == "")
                {
                    CXN_CIE10 CIE = new CXN_CIE10
                    {
                        Cie_Cod = textBox2.Text,
                        Cie_Serv = textBox3.Text
                    };

                    bool _inserta = repoCIE10.CreaCIE10(CIE);
                    if (_inserta != true)
                    {
                        MessageBox.Show("No se ha logrado crear el diagnostico", "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Este codigo de servicio digitado ya existe con el servicio: " + _existente);
                    return;
                }

                MessageBox.Show("Creado", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.CharacterCasing = CharacterCasing.Upper;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.CharacterCasing = CharacterCasing.Upper;
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Visible = true;
                label8.Text = listView1.SelectedItems[0].SubItems[0].Text;
                textBox4.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            label8.Text = "";
            textBox4.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (label8.Text == "") { MessageBox.Show("Debe diligenciar los dos campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar los dos campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                CXN_CIE10 CIE = new CXN_CIE10
                {
                    Cie_Cod = label8.Text,
                    Cie_Serv = textBox4.Text
                };

                bool _inserta = repoCIE10.UpdateCIE10(CIE);
                if (_inserta != true)
                {
                    MessageBox.Show("No se ha logrado modificar el diagnostico", "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Actualizado!!", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.CharacterCasing = CharacterCasing.Upper;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.CharacterCasing = CharacterCasing.Upper;
        }
    }
}
