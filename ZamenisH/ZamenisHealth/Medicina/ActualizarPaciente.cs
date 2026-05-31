using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;
using ZamenisHealth.Medicina.Extras;

namespace ZamenisHealth.Medicina
{
    public partial class ActualizarPaciente : Forma2
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IAseguradoras repositorioAse = new MAseguradoras();

        private int PacienteId, AseId;
        private MensajesGeneral MG;

        public ActualizarPaciente(int _paci)
        {
            InitializeComponent();
            this.PacienteId = _paci;     
        }
        private void ActualizarPaciente_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Actualizacion de Datos";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                this.ImageClose.Visible = false;
                this.ImageMinimize.Visible = false;

                cargarDiscapacidades();
                cargarEtnias();
                cargarIdentidadGenero();
                cargarAseguradoras();
                cargarPaises();

                var DatosRes = repositorioPacientes.LlamarPacientebyId(PacienteId);
                if (DatosRes == null)
                {
                    MessageBox.Show("Paciente con inconvenientes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                    return;
                }

                List<string> ListaDocs = repositorioPacientes.ListaDocs();
                if (ListaDocs != null)
                {
                    foreach (string i2 in ListaDocs)
                    {
                        comboBox1.Items.Add(i2);
                    }
                }

                textBox1.Text = DatosRes.Pac_PrimerN;
                textBox2.Text = DatosRes.Pac_SegundoN;
                textBox3.Text = DatosRes.Pac_PrimerA;
                textBox4.Text = DatosRes.Pac_SegundoA;
                textBox5.Text = DatosRes.Pac_Telefono;
                textBox6.Text = DatosRes.Pac_TelefonoAux;
                textBox7.Text = DatosRes.Pac_Email;
                textBox8.Text = DatosRes.Pac_Direccion;
                textBox9.Text = DatosRes.Pac_Localidad;
                textBox10.Text = DatosRes.Pac_Acudiente;
                textBox11.Text = DatosRes.Pac_Parentesco;
                textBox12.Text = DatosRes.Pac_DireccionAcu;
                textBox13.Text = DatosRes.Pac_TelefonoAcu;
                textBox14.Text = DatosRes.Pac_CorreoAcu;
                comboBox1.Text = DatosRes.Pac_TipoId;
                comboBox2.Text = DatosRes.Pac_ECivil;                
                comboBox4.Text = DatosRes.Discapacidad;
                comboBox5.Text = repositorioAse.getInfoFromAsebyCode(DatosRes.Pac_Aseguradora).Ase_Descripcion;
                comboBox3.Text = DatosRes.Etnia;
                comboBox6.Text = DatosRes.Pac_Sexo == "M" ? "Hombre" : DatosRes.Pac_Sexo == "F" ? "Mujer" : "Indeterminado/Intersexual";
                comboBox7.Text = repositorioPacientes.NameIdentidadGenero(DatosRes.IdentidadGenero);
                comboBox8.Text = repositorioPacientes.getNamePais(DatosRes.Pac_PaisOrigen);
                comboBox9.Text = repositorioPacientes.getNamePais(DatosRes.Pac_Residencia);
                comboBox10.Text = DatosRes.Pac_Zona == "R" ? "Rural" : "Urbana";
                dateTimePicker1.Text = Convert.ToDateTime(DatosRes.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                dateTimePicker2.Value = Convert.ToDateTime(DatosRes.HoraNto);

                if (DatosRes.Pac_Ocupacion == "N/A")
                {
                    textBox15.Text = "N/A";
                    checkBox1.Checked = true;
                }
                else
                {
                    textBox15.Text = DatosRes.Pac_Ocupacion;
                    checkBox1.Checked = false;
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }   
        void cargarPaises()
        {
            List<string> lista = repositorioPacientes.getListPaises();
            if (lista != null)
            {
                foreach (string i in lista)
                {
                    comboBox8.Items.Add(i);
                }
                foreach (string i2 in lista)
                {
                    comboBox9.Items.Add(i2);
                }
            }
        }
        void cargarAseguradoras()
        {
            List<CXN_ASEGURADORA> listado = repositorioAse.getAseguradoras();
            if (listado != null)
            {
                foreach (CXN_ASEGURADORA i in listado)
                {
                    comboBox5.Items.Add(i.Ase_Descripcion);
                }
            }
        }
        void cargarIdentidadGenero()
        {
            List<CXN_GENDERIDENTITY> lista = repositorioPacientes.ListaIdentidadGenero();
            if (lista != null)
            {
                foreach (CXN_GENDERIDENTITY i in lista)
                {
                    comboBox7.Items.Add(i.Identidad);
                }
            }
        }
        void cargarDiscapacidades()
        {
            List<CXN_DISCAPACIDAD> lista = repositorioGenerales.ListaDiscapacidades();
            if (lista != null)
            {
                foreach (CXN_DISCAPACIDAD i in lista)
                {
                    comboBox4.Items.Add(i.Discapacidad);
                }
            }
        }
        void cargarEtnias()
        {
            List<CXN_ETNIA> lista = repositorioGenerales.ListaEtnias();
            if (lista != null)
            {
                foreach (CXN_ETNIA i in lista)
                {
                    comboBox3.Items.Add(i.Etnia);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;

                if (Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) == Convert.ToDateTime(Hoy).ToString(Conexion.ConectionDictionary["Format_Fecha"]))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La fecha de nacimiento no puede ser hoy";
                    MG.ShowDialog();
                    return;
                }

                var celular = repositorioPacientes.ValidaCelular(textBox5.Text);
                if (celular != true)
                {
                    MessageBox.Show("El numero de celular es incorrecto, este campo es oligatorio", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (textBox7.Text != "")
                {
                    var email = repositorioPacientes.ValidaEmail(textBox7.Text);
                    if (email != true)
                    {
                        MessageBox.Show("El formato del correo es incorrecto, si no lo conoce deje esta casilla en blanco", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (comboBox4.Text == "")
                {
                    MessageBox.Show("La discapacidad es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string ocupation = "N/A";

                if (checkBox1.Checked == false)
                {
                    if (string.IsNullOrEmpty(textBox15.Text))
                    {
                        MessageBox.Show("La ocupacion es obligatoria, si no aparece en la lista marque la opcion No Registra En Lista", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        ocupation = textBox15.Text;
                    }
                }                
                
                if (comboBox5.Text == "")
                {
                    MessageBox.Show("La aseguradora es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox3.Text == "")
                {
                    MessageBox.Show("La etnia es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox8.Text == "")
                {
                    MessageBox.Show("El pais de residencia es obligatorio", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox9.Text == "")
                {
                    MessageBox.Show("La etnia es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox10.Text == "")
                {
                    MessageBox.Show("La zona urbana es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox6.Text == "")
                {
                    MessageBox.Show("El sexo biologico es obligatorio", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox7.Text == "")
                {
                    MessageBox.Show("La identyidad de genero es obligatoria", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                CXN_PACIENTES P = new CXN_PACIENTES
                {
                    Pac_PrimerN = textBox1.Text,
                    Pac_PrimerA = textBox3.Text,
                    Pac_SegundoN = textBox2.Text,
                    Pac_SegundoA = textBox4.Text,
                    Pac_Telefono = textBox5.Text,
                    Pac_TelefonoAux = textBox6.Text,
                    Pac_Email = textBox7.Text,
                    Pac_Direccion = textBox8.Text,
                    Pac_Localidad = textBox9.Text,
                    Pac_FechaNto = Convert.ToDateTime(dateTimePicker1.Value.Date.ToString(Conexion.ConectionDictionary["Format_Fecha"])),
                    Pac_Acudiente = textBox10.Text,
                    Pac_TelefonoAcu = textBox13.Text,
                    Pac_DireccionAcu = textBox12.Text,
                    Pac_Parentesco = textBox11.Text,
                    Pac_CorreoAcu = textBox14.Text,
                    Pac_TipoId = comboBox1.Text,
                    Pac_ECivil = comboBox2.Text,
                    Pac_Ocupacion = ocupation,
                    Pac_Id = PacienteId,
                    Discapacidad = comboBox4.Text,
                    Etnia = comboBox3.Text,

                    HoraNto = dateTimePicker2.Value,
                    Pac_Sexo = comboBox6.Text == "Hombre" ? "M" : comboBox6.Text == "Mujer" ? "F" : "I",
                    IdentidadGenero = repositorioPacientes.CodeIdentidadGenero(comboBox7.Text),
                    Pac_PaisOrigen = repositorioPacientes.getCodePais(comboBox8.Text),
                    Pac_Residencia = repositorioPacientes.getCodePais(comboBox9.Text),
                    Pac_Zona = comboBox10.Text == "Rural" ? "R" : "U",
                    Pac_Aseguradora = AseId
                };

                repositorioPacientes.Actualiza(P);

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox15_DoubleClick(object sender, EventArgs e)
        {
            OcupacionPac ocupacionPac = new OcupacionPac(this);
            ocupacionPac.ShowDialog();
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            Alergias alergias = new Alergias("ALERGIA", PacienteId);
            alergias.ShowDialog();
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            Recomendaciones recomendaciones = new Recomendaciones(PacienteId);
            recomendaciones.ShowDialog();
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                AseId = repositorioAse.getInfoFromAsebyName(comboBox5.Text).Ase_Identificador;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
