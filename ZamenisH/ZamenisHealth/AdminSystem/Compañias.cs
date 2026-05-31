using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Compañias : Forma2
    {
        private static readonly ICompañia repoCIA = new MCompañia();
        
        string dir, file;

        public Compañias()
        {
            InitializeComponent();

            ConfigForm.GraficarControl(button3, 1, Color.Red);
            ConfigForm.GraficarControl(button4, 1, Color.Red);
        }

        private void Compañias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Prestadores";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            ConfigForm.SoloNumeros(textBox1);
            ConfigForm.SoloNumeros(textBox13);
            ConfigForm.SoloNumeros(textBox14);
            ConfigForm.SoloNumeros(textBox15);
            ConfigForm.SoloNumeros(textBox16);
            ConfigForm.SoloNumeros(textBox17);
            
        }                

        private void button4_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
            dir = "";
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
                    textBox6.Text = dir;
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

        private void Actualizar()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                CXN_CIA C = new CXN_CIA
                {
                    Com_Nombre = textBox2.Text,
                    Com_Identificacion = textBox3.Text,
                    Com_Direccion = textBox4.Text,
                    Com_Telefono = textBox5.Text,
                    Com_Cod_Prestador = textBox7.Text,
                    Com_Cod_Prestador_2 = textBox8.Text,
                    Com_Email = textBox9.Text,
                    Com_Resolucion = textBox10.Text,
                    Com_Tipo_Doc = comboBox1.Text,
                    Com_Nombre_SMS = textBox11.Text,
                    Com_Telefono_SMS = textBox12.Text,
                    Com_OP = Convert.ToInt32(textBox13.Text),
                    Com_Fac = Convert.ToInt32(textBox14.Text),
                    Com_Cotiza = Convert.ToInt32(textBox15.Text),
                    Com_DE = Convert.ToInt32(textBox16.Text),
                    Com_OM = Convert.ToInt32(textBox17.Text),
                    Com_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Com_Identificador = Convert.ToInt32(textBox1.Text)
                };

                bool _updateCIA = repoCIA.updateCompañia(C);
                if (_updateCIA != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar la compañia";
                    MG.ShowDialog();
                    return;
                }

                if (checkBox1.Checked == true)
                {
                    Graba_Logo();
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Actualizado con Exito!!!";
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
        private void Grabar()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                CXN_CIA C = new CXN_CIA
                {
                    Com_Nombre = textBox2.Text,
                    Com_Identificacion = textBox3.Text,
                    Com_Direccion = textBox4.Text,
                    Com_Telefono = textBox5.Text,
                    Com_Cod_Prestador = textBox7.Text,
                    Com_Cod_Prestador_2 = textBox8.Text,
                    Com_Email = textBox9.Text,
                    Com_Resolucion = textBox10.Text,
                    Com_Tipo_Doc = comboBox1.Text,
                    Com_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Com_OP = Convert.ToInt32(textBox13.Text),
                    Com_Fac = Convert.ToInt32(textBox14.Text),
                    Com_Cotiza = Convert.ToInt32(textBox15.Text),
                    Com_DE = Convert.ToInt32(textBox16.Text),
                    Com_OM = Convert.ToInt32(textBox17.Text),
                    Com_Identificador = Convert.ToInt32(textBox1.Text),
                    Com_Nombre_SMS = textBox11.Text,
                    Com_Telefono_SMS = textBox12.Text
                };

                bool _createCIA = repoCIA.createCompañia(C);
                if (_createCIA != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro crear la compañia";
                    MG.ShowDialog();
                    return;
                }

                if (checkBox1.Checked == true)
                {
                    Graba_Logo();
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Compañia creada Exitosamente!!";
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
                var Datos = repoCIA.getPrestadorbyCode(Convert.ToInt32(textBox1.Text));
                if (Datos == null)
                {
                    button1.Text = "Grabar";
                    button1.Enabled = true;
                    
                    textBox1.Enabled = false;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    comboBox1.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    textBox7.Enabled = true;
                    textBox8.Enabled = true;
                    textBox9.Enabled = true;
                    textBox10.Enabled = true;
                    textBox11.Enabled = true;
                    textBox12.Enabled = true;
                    textBox13.Enabled = true;
                    textBox14.Enabled = true;
                    textBox15.Enabled = true;
                    textBox16.Enabled = true;
                    textBox17.Enabled = true;

                    textBox2.Text = "";
                    textBox3.Text = "";
                    comboBox1.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "";
                    textBox14.Text = "";
                    textBox15.Text = "";
                    textBox16.Text = "";
                    textBox17.Text = "";
                    return;
                }

                MessageBox.Show("Este numero de identificador de compañia ya existe, se cargan datos",
                    "Existente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                button1.Text = "Actualizar";
                button1.Enabled = true;
                textBox1.Enabled = false;
                textBox2.Text = Datos.Com_Nombre;
                textBox3.Text = Datos.Com_Identificacion;
                comboBox1.Text = Datos.Com_Tipo_Doc;
                textBox4.Text = Datos.Com_Direccion;
                textBox5.Text = Datos.Com_Telefono;
                textBox7.Text = Datos.Com_Cod_Prestador;
                textBox8.Text = Datos.Com_Cod_Prestador_2;
                textBox9.Text = Datos.Com_Email;
                textBox10.Text = Datos.Com_Resolucion;
                textBox11.Text = Datos.Com_Nombre_SMS;
                textBox12.Text = Datos.Com_Telefono_SMS;
                textBox13.Text = Datos.Com_OP.ToString();
                textBox14.Text = Datos.Com_Fac.ToString();
                textBox15.Text = Datos.Com_Cotiza.ToString();
                textBox16.Text = Datos.Com_DE.ToString();
                textBox17.Text = Datos.Com_OM.ToString();

                textBox2.Enabled = true;
                textBox3.Enabled = true;
                comboBox1.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                textBox11.Enabled = true;
                textBox12.Enabled = true;
                textBox13.Enabled = true;
                textBox14.Enabled = true;
                textBox15.Enabled = true;
                textBox16.Enabled = true;
                textBox17.Enabled = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox12.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                if (textBox13.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox17.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                if (button1.Text == "Actualizar")
                {
                    if (checkBox1.Checked == true)
                    {
                        if (dir == "") { MessageBox.Show("Debe seleccionar la representacion grafica del logo de la empresa"); return; }
                        if (textBox6.Text == "") { MessageBox.Show("Debe seleccionar la representacion grafica del logo de la empresa"); return; }
                    }
                    Actualizar();
                    return;
                }

                if (button1.Text == "Grabar")
                {
                    if (checkBox1.Checked == true)
                    {
                        if (dir == "") { MessageBox.Show("Debe seleccionar la representacion grafica del logo de la empresa"); return; }
                        if (textBox6.Text == "") { MessageBox.Show("Debe seleccionar la representacion grafica del logo de la empresa"); return; }
                    }
                    Grabar();
                    return;
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

        private void Graba_Logo()
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(dir);
                file = Convert.ToBase64String(bytes);
                richTextBox1.Text = file.ToString();

                CXN_CIA C = new CXN_CIA
                {
                    Com_Logo = richTextBox1.Text,
                    Com_Identificador = Convert.ToInt32(textBox1.Text)
                };

                repoCIA.grabaLogo(C);
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
