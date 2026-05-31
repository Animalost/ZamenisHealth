using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Tipificador
{
    public partial class Ingreso : Forma
    {
        private readonly IPacientes repositorioPacientes;
        private readonly IAseguradoras repositorioAse;
        private readonly ITipificador tipificador;

        private MensajesGeneral MG;
        private int CodAse;

        public Ingreso()
        {
            InitializeComponent();
            repositorioPacientes = new MPacientes();
            repositorioAse = new MAseguradoras();
            tipificador = new MTipificador();

            ConfigForm.SoloNumeros(textBox3);
        }

        private void Ingreso_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Tipificador";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSave;
            ToolStripButton btnHistorial;

            btnSave = new ToolStripButton();
            btnSave = createToolButton("Grabar");
            MenuLateral.Items.Add(btnSave);
            btnSave.Click += toolStripButton1_Click;

            btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += toolStripButton3_Click;

            CargarDocumentos();
            CargarAse();
            CargarRazones();
            textBox3.MaxLength = 10;
        }

        private void CargarDocumentos()
        {
            var ListaDocs = repositorioPacientes.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        private void CargarRazones()
        {
            var ListaRazones = tipificador.ListaRazones();
            if (ListaRazones != null)
            {
                foreach (var i in ListaRazones)
                {
                    comboBox3.Items.Add(i.RazonLlamada);
                }

                comboBox3.SelectedIndex = 0;
            }
        }

        private void CargarAse()
        {
            List<CXN_ASEGURADORA> CargarAse = repositorioAse.getAseguradoras();


            if (CargarAse != null)
            {
                foreach (var i in CargarAse)
                {
                    comboBox2.Items.Add(i.Ase_Descripcion.ToString());
                }

                comboBox2.SelectedIndex = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CodAse = repositorioAse.getInfoFromAsebyName(comboBox2.Text).Ase_Identificador;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox5.Text == "Curaciones")
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar el nombre de la persona que llama";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else if (string.IsNullOrEmpty(textBox2.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar el documento del paciente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else if (string.IsNullOrEmpty(textBox3.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar el celular del paciente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else if (string.IsNullOrEmpty(textBox5.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar el nombre del paciente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else if (string.IsNullOrEmpty(richTextBox1.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar la gestion de la llamada";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (comboBox1.Text == "")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Debe diligenciar el tipo de documento del paciente";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else if (comboBox2.Text == "")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Debe diligenciar la aseguradora del paciente";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else if (comboBox3.Text == "")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Debe diligenciar la razon de la llamada";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else if (comboBox4.Text == "")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Debe diligenciar el modo de ingreso del registro";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else if (comboBox5.Text == "")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Debe diligenciar el motivo del registro";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else if (repositorioPacientes.ValidaCelular(textBox3.Text) == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El formato del celular es invalido";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(textBox4.Text))
                            {
                                if (repositorioPacientes.ValidaEmail(textBox4.Text) == false)
                                {
                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "El formato del email es invalido, si no lo conoce deje el campo vacio";
                                    MG.TipoImagen = 1000;
                                    MG.ShowDialog();
                                    return;
                                }
                            }

                            CXN_TIPIFICADOR T = new CXN_TIPIFICADOR
                            {
                                Aseguradora = CodAse,
                                Celular = textBox3.Text,
                                Email = textBox4.Text,
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now,
                                Gestion = richTextBox1.Text.Trim(),
                                NombreLlama = textBox1.Text.Trim(),
                                NumIdPaciente = textBox2.Text.Trim(),
                                TipoIdPaciente = comboBox1.Text,
                                RazonLlamada = comboBox3.Text,
                                Usuario = Contenedor.UsuarioLogueado,
                                Ingreso = comboBox4.Text,
                                NombrePaciente = textBox5.Text.Trim()
                            };

                            bool crear = tipificador.Crea(T);
                            if (crear == true)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Grabado";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();

                                Limpiar();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro grabar el registro";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }
                    }
                }
                else //Fibromialgia y Otros
                {
                    if (comboBox4.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar el modo de ingreso del registro";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        CXN_TIPIFICADOR T = new CXN_TIPIFICADOR
                        {
                            Aseguradora = 99,
                            Celular = "N/A",
                            Email = "N/A",
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now,
                            Gestion = "N/A",
                            NombreLlama = textBox1.Text.Trim(),
                            NumIdPaciente = "Cedula de Ciudadania",
                            TipoIdPaciente = "N/A",
                            RazonLlamada = comboBox5.Text,
                            Usuario = Contenedor.UsuarioLogueado,
                            Ingreso = comboBox4.Text, 
                            NombrePaciente = "N/A"
                        };

                        bool crear = tipificador.Crea(T);
                        if (crear == true)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Grabado";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();

                            Limpiar();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro grabar el registro";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }                    
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Limpiar()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            richTextBox1.Text = "";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Historico historico = new Historico();
            historico.ShowDialog();
        }
    }
}
