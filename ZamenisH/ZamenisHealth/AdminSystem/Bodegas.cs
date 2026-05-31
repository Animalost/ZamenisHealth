using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Bodegas : Forma
    {
        private static readonly IMensajeria repoMensajeria = new MMensajeria();
        private static readonly IBodegas repoBodegas = new MBodegas();
        private string getUserToAsignsBod;
        private ToolStripButton btnGrabar;
        string dir, file;

        public Bodegas()
        {
            InitializeComponent();
        }

        private void Bodegas_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Bodegas";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += toolStripButton2_Click;

                var getUsers = repoMensajeria.getUsersforSendMessage();
                if (getUsers != null)
                {
                    foreach (var i in getUsers)
                    {
                        comboBox1.Items.Add(i.Value);
                    }
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


        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            dir = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                label8.Text = comboBox1.Text;

                getUserToAsignsBod = repoMensajeria.getUsertoSendMessage(label8.Text);

                var _comprobarExistencia = repoBodegas.getDatosUser(getUserToAsignsBod);
                if (_comprobarExistencia != null)
                {
                    textBox1.Text = _comprobarExistencia.Bod_Numero.ToString();
                    textBox1.Enabled = false;
                    comboBox1.Enabled = false;
                    btnGrabar.Text = "Actualizar";
                    btnGrabar.Enabled = true;
                    richTextBox1.Text = _comprobarExistencia.Bod_Firma;
                    textBox3.Text = _comprobarExistencia.Bod_Reg_Med;
                    textBox4.Text = _comprobarExistencia.Bod_Tipo;
                    comboBox3.Text = _comprobarExistencia.Bod_Estado;

                    MG.TipoImagen = 3;
                    MG.Mensaje = "El usuario seleccionado ya registra como profesional, se cargan datos para edicion";
                    MG.ShowDialog();
                }
                else
                {
                    textBox1.Enabled = true;
                    //comboBox1.Enabled = false;
                    btnGrabar.Text = "Grabar";
                    btnGrabar.Enabled = true;
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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (comboBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                CXN_BODEGAS B = new CXN_BODEGAS
                {
                    Bod_Numero = Convert.ToInt32(textBox1.Text),
                    Bod_Usuario = getUserToAsignsBod,
                    Bod_Responsable = comboBox1.Text,
                    Bod_Reg_Med = textBox3.Text,
                    Bod_Tipo = textBox4.Text,
                    Bod_Firma = richTextBox1.Text,
                    Bod_Estado = comboBox3.Text
                };

                if (checkBox1.Checked == true)
                {
                    if (dir == "") { MessageBox.Show("Debe seleccionar la representacion grafica de la firma del profesional", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    if (textBox2.Text == "") { MessageBox.Show("Debe seleccionar la representacion grafica de la firma del profesional", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if (btnGrabar.Text == "Actualizar")
                {
                    bool _update = repoBodegas.updateUser(B);
                    if (_update != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un error y no se logro actualizar la bodega";
                        MG.ShowDialog();
                        return;
                    }
                }

                if (btnGrabar.Text == "Grabar")
                {
                    bool _create = repoBodegas.createUser(B);
                    if (_create != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un error y no se logro crear la bodega";
                        MG.ShowDialog();
                        return;
                    }
                }

                if (checkBox1.Checked == true)
                {
                    Byte[] bytes = File.ReadAllBytes(textBox2.Text);
                    string file = Convert.ToBase64String(bytes);
                    richTextBox1.Text = file.ToString();
                    B.Bod_Firma = richTextBox1.Text;

                    bool _regFirma = repoBodegas.registerFirma(B);
                    if (_regFirma != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El usuario fue registrado correctamente pero no fue asignada la firma, vuelva a intentar asignar la firma nuevamente";
                        MG.ShowDialog();
                        return;
                    }
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "El usuario ha sido registrado como bodega exitosamente";
                MG.ShowDialog();

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

        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                string searchCode = repoBodegas.ProfesionalNombre(Convert.ToInt32(textBox1.Text));
                if (searchCode == "")
                {
                    textBox1.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Numero de Bodega ya existe con " + searchCode.ToString() + ", debe escoger otro", "Codigo en uso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Enabled = true;
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    dir = openFileDialog1.FileName;
                    string destino = Path.GetFileName(dir);
                    textBox2.Text = dir;
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
    }
}
