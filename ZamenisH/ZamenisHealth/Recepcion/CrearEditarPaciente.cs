using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion
{
    public partial class CrearEditarPaciente : ConfigForm.BaseForm
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAseguradoras repositorioAseguradora = new MAseguradoras();
        private static readonly IZonas repositorioZonas = new MZonas();
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IFHIR rFHIR = new MFHIR();

        private int Pac_Id_Existente;
        private MensajesGeneral MG;

        private string tdoc, doc;

        public CrearEditarPaciente()
        {
            InitializeComponent();

            btnZamenis1.Click += btnZamenis1_ButtonClick;
            btnZamenis2.Click += btnZamenis2_ButtonClick;

            btnZamenis1.ToolTipText = "Graba nuevo paciente";
            btnZamenis2.ToolTipText = "Actualiza paciente existente";

            cargarDiscapacidades();
            cargarOcupacion("");
            cargarEtnias();

            ConfigForm.MoverForma(panel3, this);
        }

        public CrearEditarPaciente(string TDoc, string Doc)
        {
            InitializeComponent();

            btnZamenis1.Click += btnZamenis1_ButtonClick;
            btnZamenis2.Click += btnZamenis2_ButtonClick;

            btnZamenis1.ToolTipText = "Graba nuevo paciente";
            btnZamenis2.ToolTipText = "Actualiza paciente existente";

            cargarDiscapacidades();
            cargarOcupacion("");
            cargarEtnias();

            ConfigForm.MoverForma(panel3, this);

            tdoc = TDoc;
            doc = Doc;            
        }

        void cargarOcupacion(string Ocupacion)
        {
            List<string> lista = rFHIR.GetOcupaciones(Ocupacion);
            if (lista != null)
            {
                foreach (string i in lista)
                {
                    comboBox7.Items.Add(i);
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
                    comboBox9.Items.Add(i.Etnia);
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
                    comboBox8.Items.Add(i.Discapacidad);
                }
            }
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Grabado("Creacion");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Grabado("Edicion");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CargarDocumentos()
        {
            List<string> ListaDocs = repositorioPacientes.ListaDocs();
            
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void CargarRegimen()
        {
            List<string> ListaRegimen =  repositorioPacientes.ListaRegimen();
            
            if (ListaRegimen != null)
            {
                foreach (string r in ListaRegimen)
                {
                    comboBox5.Items.Add(r);
                }
            }           
        }
        private void CrearEditarPaciente_Load(object sender, EventArgs e)
        {
            this.Titulo.Visible = false;
            ImageClose.Visible = false;

            List<CXN_ASEGURADORA> ListAse =  repositorioAseguradora.getAseguradoras();

            if (ListAse != null)
            {
                foreach (var i in ListAse)
                {
                    comboBox4.Items.Add(i.Ase_Descripcion);
                }
            }

            CargarDocumentos();
            CargarRegimen();

            textBox6.MaxLength = 10;
            panel1.Enabled = false;

            if (!string.IsNullOrEmpty(tdoc) && !string.IsNullOrEmpty(doc))
            {
                comboBox1.Text = tdoc;
                textBox1.Text = doc;

                BuscarPaciente();
            }
        }
        private void Limpiar()
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";

            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";

            label23.Text = "";
            label20.Text = "";
            label30.Text = "";

            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            textBox15.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            BuscarPaciente();
        }
        void BuscarPaciente()
        {
            try
            {
                Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();

                if (comboBox1.Text == "")
                {
                    M.Mensaje = "Debe seleccionar el tipo de documento";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }

                if (textBox1.Text == "")
                {
                    M.Mensaje = "Debe digitar el numero de documento";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }

                CXN_PACIENTES DatosPac =  repositorioPacientes.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
                Limpiar();

                panel1.Enabled = true;
                if (DatosPac == null)
                {
                    //Crear Paciente
                    btnZamenis1.Visible = true;
                    btnZamenis2.Visible = false;
                    comboBox1.Enabled = false;
                    textBox1.Enabled = false;

                    CXN_PACIENTES DatosPacsoloDoc =  repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);

                    if (DatosPacsoloDoc != null)
                    {
                        Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral
                        {
                            Mensaje = "El tipo y numero de documento no existen en sistema pero el numero de documento existe con otro tipo de documento " +
                            DatosPacsoloDoc.Pac_TipoId.ToString() + ".  Puede buscarlo asi antes de crear un paciente nuevo y duplicarlo",
                            TipoImagen = 0
                        };

                        mensajesGeneral.ShowDialog();

                        DialogResult result = MessageBox.Show("¿Desea volver a buscar los datos con la sugerencia indicada anteriormente?",
                                                  "Gestion de paciente",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            DatosPac = repositorioPacientes.LlamarPacienteDOC(DatosPacsoloDoc.Pac_TipoId.ToString(), textBox1.Text);

                            // Cargar Paciente existente
                            comboBox1.Enabled = true;
                            comboBox1.Text = DatosPacsoloDoc.Pac_TipoId.ToString();
                            textBox1.Enabled = true;
                            button1.Enabled = false;
                            btnZamenis2.Visible = true;
                            btnZamenis1.Visible = false;
                            Pac_Id_Existente = Convert.ToInt32(DatosPac.Pac_Id);
                            textBox2.Text = DatosPac.Pac_PrimerN;
                            textBox4.Text = DatosPac.Pac_PrimerA;
                            textBox3.Text = DatosPac.Pac_SegundoN;
                            textBox5.Text = DatosPac.Pac_PrimerA;
                            textBox6.Text = DatosPac.Pac_Telefono;
                            textBox7.Text = DatosPac.Pac_TelefonoAux;
                            textBox8.Text = DatosPac.Pac_Email;
                            textBox9.Text = DatosPac.Pac_Direccion;
                            textBox10.Text = DatosPac.Pac_Localidad;
                            textBox11.Text = DatosPac.Pac_Acudiente;
                            textBox12.Text = DatosPac.Pac_Parentesco;
                            textBox13.Text = DatosPac.Pac_DireccionAcu;
                            textBox14.Text = DatosPac.Pac_TelefonoAcu;
                            textBox15.Text = DatosPac.Pac_CorreoAcu;
                            textBox16.Text = DatosPac.Pac_Contrato;
                            comboBox2.Text = DatosPac.Pac_Sexo;
                            comboBox3.Text = DatosPac.Pac_Zona;
                            dateTimePicker1.Value = Convert.ToDateTime(DatosPac.Pac_FechaNto);
                            comboBox7.Text = DatosPac.Pac_Ocupacion;
                            comboBox9.Text = DatosPac.Etnia;
                            comboBox8.Text = DatosPac.Discapacidad;
                            /*Categoria A
                              Categoria B
                              Categoria C
                              Categoria Z
                              No Aplica*/
                            comboBox6.Text = (DatosPac.Pac_Categoria == "A" ? "Categoria A" : DatosPac.Pac_Categoria == "B" ? "Categoria B" :
                                DatosPac.Pac_Categoria == "C" ? "Categoria C" : DatosPac.Pac_Categoria == "Z" ? "Categoria Z" : "No Aplica");

                            string RegimenNombre1 = "";
                            CXN_ASEGURADORA AseguradoraNombre1 = new CXN_ASEGURADORA();
                            string DepartamentoaNombre1 = "";
                            string MunicipioNombre1 = "";


                            RegimenNombre1 = repositorioPacientes.Carga_Regimen(DatosPac.Pac_Regimen);
                            AseguradoraNombre1 = repositorioAseguradora.getInfoFromAsebyCode(DatosPac.Pac_Aseguradora);
                            DepartamentoaNombre1 = repositorioZonas.DepartamentoNombre(DatosPac.Pac_Dep_Cod);
                            MunicipioNombre1 = repositorioZonas.MunicipioNombre(DatosPac.Pac_Mun_Cod, DatosPac.Pac_Dep_Cod);


                            comboBox5.Text = RegimenNombre1.ToString();
                            comboBox4.Text = AseguradoraNombre1.Ase_Descripcion.ToString();
                            label23.Text = DepartamentoaNombre1.ToString();
                            label20.Text = MunicipioNombre1.ToString();

                            Comunes.MensajesGeneral M3 = new Comunes.MensajesGeneral();
                            M3.Mensaje = "El tipo y numero de documento digitados ya existen, se han cargado datos para su edicion";
                            M3.TipoImagen = 0;
                            M3.ShowDialog();
                        }

                        return;

                    }
                    else
                    {
                        Comunes.MensajesGeneral M4 = new Comunes.MensajesGeneral();
                        M4.Mensaje = "El tipo y numero de documento digitados NO existen, puede crear al paciente";
                        M4.TipoImagen = 3;
                        M4.ShowDialog();
                    }
                }
                else
                {
                    // Cargar Paciente existente
                    button1.Enabled = false;
                    btnZamenis2.Visible = true;
                    btnZamenis1.Visible = false;
                    Pac_Id_Existente = Convert.ToInt32(DatosPac.Pac_Id);
                    textBox2.Text = DatosPac.Pac_PrimerN;
                    textBox4.Text = DatosPac.Pac_PrimerA;
                    textBox3.Text = DatosPac.Pac_SegundoN;
                    textBox5.Text = DatosPac.Pac_SegundoA;
                    textBox6.Text = DatosPac.Pac_Telefono;
                    textBox7.Text = DatosPac.Pac_TelefonoAux;
                    textBox8.Text = DatosPac.Pac_Email;
                    textBox9.Text = DatosPac.Pac_Direccion;
                    textBox10.Text = DatosPac.Pac_Localidad;
                    textBox11.Text = DatosPac.Pac_Acudiente;
                    textBox12.Text = DatosPac.Pac_Parentesco;
                    textBox13.Text = DatosPac.Pac_DireccionAcu;
                    textBox14.Text = DatosPac.Pac_TelefonoAcu;
                    textBox15.Text = DatosPac.Pac_CorreoAcu;
                    textBox16.Text = DatosPac.Pac_Contrato;
                    comboBox2.Text = DatosPac.Pac_Sexo;
                    comboBox3.Text = DatosPac.Pac_Zona;
                    dateTimePicker1.Value = Convert.ToDateTime(DatosPac.Pac_FechaNto);
                    comboBox7.Text = DatosPac.Pac_Ocupacion;
                    comboBox9.Text = DatosPac.Etnia;
                    comboBox8.Text = DatosPac.Discapacidad;

                    comboBox6.Text = (DatosPac.Pac_Categoria == "A" ? "Categoria A" : DatosPac.Pac_Categoria == "B" ? "Categoria B" :
                                DatosPac.Pac_Categoria == "C" ? "Categoria C" : DatosPac.Pac_Categoria == "Z" ? "Categoria Z" : "No Aplica");

                    string RegimenNombre = "";
                    CXN_ASEGURADORA AseguradoraNombre = new CXN_ASEGURADORA();
                    string DepartamentoaNombre = "";
                    string MunicipioNombre = "";

                    RegimenNombre = repositorioPacientes.Carga_Regimen(DatosPac.Pac_Regimen);
                    AseguradoraNombre = repositorioAseguradora.getInfoFromAsebyCode(DatosPac.Pac_Aseguradora);
                    DepartamentoaNombre = repositorioZonas.DepartamentoNombre(DatosPac.Pac_Dep_Cod);
                    MunicipioNombre = repositorioZonas.MunicipioNombre(DatosPac.Pac_Mun_Cod, DatosPac.Pac_Dep_Cod);

                    comboBox5.Text = RegimenNombre.ToString();
                    comboBox4.Text = AseguradoraNombre.Ase_Descripcion.ToString();
                    label23.Text = DepartamentoaNombre.ToString();
                    label20.Text = MunicipioNombre.ToString();

                    Comunes.MensajesGeneral M2 = new Comunes.MensajesGeneral();
                    M2.Mensaje = "El tipo y numero de documento digitados ya existen, se han cargado datos para su edicion";
                    M2.TipoImagen = 0;
                    M2.ShowDialog();
                }
            }
            catch //(Exception ex)
            {
                //TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CXN_ASEGURADORA IdAse =  repositorioAseguradora.getInfoFromAsebyName(comboBox4.Text);
                           
                if (IdAse == null)
                {
                    label30.Text = "Error";
                    return;
                }

                label30.Text = IdAse.Ase_Identificador.ToString();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            Recepcion.Extras.Zonas Z = new Recepcion.Extras.Zonas("CreaEditaPac", "Departamento", "");
            Z.ShowDialog();
        }
        private void label20_DoubleClick(object sender, EventArgs e)
        {
            if (label23.Text == "")
            {
                Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                M.Mensaje = "Antes de seleccionar el municipio, debe escoger primero el departamento";
                M.TipoImagen = 1000;
                M.ShowDialog();
                return;
            }

            Extras.Zonas Z = new Extras.Zonas("CreaEditaPac", "Municipio", label23.Text);
            Z.ShowDialog();
        }
        private void Grabado(string Tipo)
        {
            if (textBox8.Text != "")
            {
                var EmailValida = repositorioPacientes.ValidaEmail(textBox8.Text);

                if (EmailValida != true)
                {
                    Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                    M.Mensaje = "El formato del correo del paciente es incorrecto, si no posee correo deje el campo vacio";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }
            }

            if (checkBox1.Checked == true)
            {
                if (textBox16.Text == "")
                {
                    Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                    M.Mensaje = "Debe diligenciar el numero de contrato o poliza, si no posee el usuario un contrato marque el check de No Aplica";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }
            }

            /*if (repositorioPacientes.ComprobarEdad(dateTimePicker1.Value, comboBox1.Text) == false)
            {
                Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                M.Mensaje = "La fecha de nacimiento no coincide con el tipo de documento seleccionado, verifique los datos.  Para Mayores de 18 años no puede tener " +
                    "tipos de documentos de menores de edad y viceversa";
                M.TipoImagen = 1000;
                M.ShowDialog();
                return;
            }*/

            if (PacRules.ValidarRegimen(comboBox5.Text, comboBox6.Text) == false)
            {
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "Si el regimen es no afiliado, la categoria no puede ser A, B, C o Z.  Si el regimen es diferente a no afiliado, la categoria no puede ser no aplica";
                MG.ShowDialog();
                return;
            }

            var CelularValida = repositorioPacientes.ValidaCelular(textBox6.Text);

            if (CelularValida != true)
            {
                Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                M.Mensaje = "Numero de celular incorrecto";
                M.TipoImagen = 1000;
                M.ShowDialog();
                return;
            }
            else
            {
                if (comboBox1.Text == "") { MessageBox.Show("Debe seleccionar el tipo de documento"); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Debe seleccionar el sexo del paciente"); return; }
                if (textBox1.Text == "") { MessageBox.Show("Debe digitar el numero de documento"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Digite primer nombre"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Digite contrato del paciente"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Digite primer apellido"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Debe digitar la direccion del paciente"); return; }
                if (label20.Text == "") { MessageBox.Show("Debe seleccionar Municipio del paciente"); return; }
                if (label23.Text == "") { MessageBox.Show("Debe seleccionar Departamento del paciente"); return; }
                if (label30.Text == "") { MessageBox.Show("Debe seleccionar aseguradora del paciente"); return; }
                if (comboBox5.Text == "") { MessageBox.Show("Debe seleccionar el regimen del paciente"); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Debe seleccionar Zona Urbana"); return; }
                if (comboBox6.Text == "") { MessageBox.Show("Debe seleccionar una categoria"); return; }

                string Mun = "";
                string Dep = "";
                string Reg = "";

                
                    Mun = repositorioZonas.MunicipioCodigo(label20.Text, label23.Text);
                    Dep = repositorioZonas.DepartamentoCodigo(label23.Text);           
                

                Reg = repositorioPacientes.Regimen(comboBox5.Text);

                CXN_PACIENTES personas = new CXN_PACIENTES
                {
                    Pac_Id = Pac_Id_Existente,
                    Pac_TipoId = comboBox1.Text,
                    Pac_IdNum = textBox1.Text.TrimStart().TrimEnd(),
                    Pac_PrimerN = textBox2.Text,
                    Pac_SegundoN = textBox3.Text,
                    Pac_PrimerA = textBox4.Text,
                    Pac_SegundoA = textBox5.Text,
                    Pac_FechaNto = dateTimePicker1.Value.Date,
                    Pac_Sexo = comboBox2.Text,
                    Pac_Telefono = textBox6.Text,
                    Pac_TelefonoAux = textBox7.Text,
                    Pac_Direccion = textBox9.Text,
                    Pac_Email = textBox8.Text,
                    Pac_Mun_Cod = Mun.ToString(),
                    Pac_Dep_Cod = Dep.ToString(),
                    Pac_Regimen = Reg,
                    Pac_Zona = comboBox3.Text,
                    Pac_Localidad = textBox10.Text,
                    Pac_Aseguradora = Convert.ToInt32(label30.Text),
                    Pac_Acudiente = textBox11.Text,
                    Pac_DireccionAcu = textBox13.Text,
                    Pac_TelefonoAcu = textBox14.Text,
                    Pac_CorreoAcu = textBox15.Text,
                    Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Pac_Parentesco = textBox12.Text,
                    Pac_Contrato = (checkBox1.Checked == true ? "" : textBox16.Text),
                    Pac_Ocupacion = comboBox7.Text,
                    Etnia = comboBox9.Text,
                    Discapacidad = comboBox8.Text
                };

                switch (comboBox6.SelectedIndex)
                {
                    case 0:
                        personas.Pac_Categoria = "A";
                        break;
                    case 1:
                        personas.Pac_Categoria = "B";
                        break;
                    case 2:
                        personas.Pac_Categoria = "C";
                        break;
                    case 3:
                        personas.Pac_Categoria = "Z";
                        break;
                    case 4:
                        personas.Pac_Categoria = "N";
                        break;

                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una categoria";
                        MG.ShowDialog();
                        return;
                }                               

                if (Tipo == "Creacion")
                {
                    CXN_PACIENTES Existe =  repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                    
                    if (Existe != null)
                    {
                        Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                        M.Mensaje = "El numero de documento ya existe en sistema, no puede volver a crear el mismo documento, puede volver " +
                        "a ingresar a este modulo y editar los datos si lo desea";
                        M.TipoImagen = 1000;
                        M.ShowDialog();
                        return;
                    }

                    bool _crear = repositorioPacientes.Crea_Paciente(personas);          

                    if (_crear != true)
                    {
                        Comunes.MensajesGeneral M6 = new Comunes.MensajesGeneral();
                        M6.Mensaje = "Inconveniente creando el paciente";
                        M6.TipoImagen = 1000;
                        M6.ShowDialog();
                        return;
                    }

                    Comunes.MensajesGeneral M7 = new Comunes.MensajesGeneral();
                    M7.Mensaje = "Creado con Exito";
                    M7.TipoImagen = 3;
                    M7.ShowDialog();

                    Recepcion.CrearEditarPaciente f1 = Application.OpenForms.OfType<Recepcion.CrearEditarPaciente>().SingleOrDefault();
                    f1.Dispose();
                    f1.Close();
                    return;
                }

                if (Tipo == "Edicion")
                {
                    bool Existe = repositorioPacientes.Existente(textBox1.Text, Pac_Id_Existente);                 

                    if (Existe == true)
                    {
                        MessageBox.Show("El numero de documento ya existe en sistema con otro paciente diferente a este, " +
                                        "no puede volver a crear el mismo documento, puede volver " +
                                        "a ingresar a este modulo y editar los datos si lo desea",
                                        "No se puede editar paciente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                        return;
                    }

                    bool _update =  repositorioPacientes.Edita_Paciente(personas);                  

                    if (_update != true)
                    {
                        Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                        M.Mensaje = "Error desconocido al editar paciente";
                        M.TipoImagen = 1000;
                        M.ShowDialog();
                        return;
                    }

                    Comunes.MensajesGeneral M2 = new Comunes.MensajesGeneral();
                    M2.Mensaje = "Actualizado con exito";
                    M2.TipoImagen = 3;
                    M2.ShowDialog();

                    Recepcion.CrearEditarPaciente f1 = Application.OpenForms.OfType<Recepcion.CrearEditarPaciente>().SingleOrDefault();
                    f1.Dispose();
                    f1.Close();
                }
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "Pacientes";
            buscarPacientes.ShowDialog();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                e.Handled = true;
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
