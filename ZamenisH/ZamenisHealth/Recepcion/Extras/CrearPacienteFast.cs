using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CrearPacienteFast : Forma
    {
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();
        private static readonly IPacientes repoPacientes = new MPacientes();

        private string TID, NID;
        
        public CrearPacienteFast(string _TID, string _NID)
        {
            InitializeComponent();
            this.TID = _TID;
            this.NID = _NID;
        }
        private void CargarRegimen()
        {
            List<string> ListaRegimen =repoPacientes.ListaRegimen();            

            if (ListaRegimen != null)
            {
                foreach (string r in ListaRegimen)
                {
                    comboBox3.Items.Add(r);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.Text == "" || textBox1.Text == "" || textBox3.Text == "" || textBox6.Text == "" 
                    || comboBox2.Text == "" || textBox2.Text == "" || comboBox6.Text == "" || comboBox5.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligienciar tipo de documento, numero de documento, primer nombre, primer apellido, aseguradora y numero de contacto movil, genero e identiodad de genero";
                    MG.ShowDialog();
                    return;
                }

                bool valCel = repoPacientes.ValidaCelular(textBox2.Text);
                if (valCel != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Numero de celular invalido, debe ser numerico de 10 digitos";
                    MG.ShowDialog();
                    return;
                }

                if (textBox7.Text != "")
                {
                    bool valEmail = repoPacientes.ValidaEmail(textBox7.Text);
                    if (valEmail != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Email invalido, debe ser tipo nombre@dominio.xxx";
                        MG.ShowDialog();
                        return;
                    }
                }

                if (PacRules.ValidarRegimen(comboBox3.Text, comboBox4.Text) == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Si el regimen es no afiliado, la categoria no puede ser A, B, C o Z.  Si el regimen es diferente a no afiliado, la categoria no puede ser no aplica";
                    MG.ShowDialog();
                    return;
                }

                CXN_ASEGURADORA getAseId = repoAseguradoras.getInfoFromAsebyName(comboBox2.Text);                

                if (getAseId != null)
                {
                    CXN_PACIENTES P = new CXN_PACIENTES
                    {
                        Pac_TipoId = comboBox1.Text,
                        Pac_IdNum = textBox1.Text.Trim(),
                        Pac_PrimerN = textBox3.Text,
                        Pac_SegundoN = textBox4.Text,
                        Pac_PrimerA = textBox6.Text,
                        Pac_SegundoA = textBox5.Text,
                        Pac_Aseguradora = getAseId.Ase_Identificador,
                        Pac_Telefono = textBox2.Text,
                        Pac_Email = textBox7.Text,
                        Pac_Sexo = comboBox6.Text == "Masculino" ? "M" : "F",
                        IdentidadGenero = repoPacientes.CodeIdentidadGenero(comboBox5.Text)
                    };

                    switch (comboBox4.SelectedIndex)
                    {
                        case 0:
                            P.Pac_Categoria = "A";
                            break;
                        case 1:
                            P.Pac_Categoria = "B";
                            break;
                        case 2:
                            P.Pac_Categoria = "C";
                            break;
                        case 3:
                            P.Pac_Categoria = "Z";
                            break;
                        case 4:
                            P.Pac_Categoria = "N";
                            break;

                        default:
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Seleccione una categoria";
                            MG.ShowDialog();
                            return;
                    }

                    bool create = repoPacientes.CrearClientes(P);                    

                    if (create != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se ha logrado crear el paciente, error interno";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Paciente creado exitosamente";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La aseguradora seleccionada presenta inconvenientes";
                    MG.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        
        private void label11_Click(object sender, EventArgs e)
        {           
            this.Dispose();
            this.Close();

            CrearEditarPaciente crearEditarPaciente = new CrearEditarPaciente();
            crearEditarPaciente.Size = new Size(619, 672);
            crearEditarPaciente.StartPosition = FormStartPosition.CenterScreen;
            crearEditarPaciente.FormBorderStyle = FormBorderStyle.FixedSingle;
            crearEditarPaciente.AutoScroll = false;
            crearEditarPaciente.ShowDialog();
        }
        private void CrearPacienteFast_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Crear Paciente";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Crear Paciente");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;

            ConfigForm.SoloNumeros(textBox2);
            CargarIdentidadGenero();
            CargaDatos();
            comboBox1.Text = this.TID;
            textBox1.Text = this.NID;

            textBox2.MaxLength = 10;

            CargarRegimen();
        }
        private void CargarIdentidadGenero()
        {
            List<CXN_GENDERIDENTITY> ListaIdGenero = repoPacientes.ListaIdentidadGenero();

            if (ListaIdGenero != null)
            {
                comboBox5.Items.Clear();

                foreach (CXN_GENDERIDENTITY r in ListaIdGenero)
                {
                    comboBox5.Items.Add(r.Identidad);
                }
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                e.Handled = true;
            }
        }        
        void CargaDatos()
        {
            try
            {
                List<CXN_ASEGURADORA> getAses = repoAseguradoras.getAseguradoras();                

                if (getAses != null)
                {
                    foreach (var i in getAses)
                    {
                        comboBox2.Items.Add(i.Ase_Descripcion);
                    }
                }

                List<string> getDocs = repoPacientes.ListaDocs();                

                if (getDocs != null)
                {
                    foreach (var i in getDocs)
                    {
                        comboBox1.Items.Add(i);
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
