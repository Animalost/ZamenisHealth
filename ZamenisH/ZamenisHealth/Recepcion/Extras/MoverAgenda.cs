using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.AgendaDiaria;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class MoverAgenda : Forma2
    {
        private static readonly IAgendaC repositorioHorario = new MAgendaC();
        private static readonly IAgenda repositorioHorario2 = new MAgenda();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private int Admision;
        private string TipoCita, Observacion;
        private int NumeroBodega, Compañia;
        private DateTime Max15;
        private int diasBloq;

        private void MoverAgenda_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Mover Pacientes";

                otrosDatosPacienteHorario _datos =repositorioHorario.cargarAdmision(Admision, "'A'");                

                if (_datos != null)
                {
                    label2.Text = Convert.ToInt32(Admision).ToString();
                    label3.Text = _datos.Hor_Imp_Age;
                    label5.Text = Convert.ToDateTime(_datos.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    label7.Text = Convert.ToDateTime(_datos.Hor_Pac_Hora_Cita).ToString("HH:mm tt");
                    label9.Text = _datos.Bod_Responsable;
                    label13.Text = Convert.ToDateTime(monthCalendar1.SelectionStart).ToString("dddd", CultureInfo.CreateSpecificCulture("es-ES"));
                    TipoCita = _datos.Hor_Pac_Tipo_Serv;
                    Compañia = Convert.ToInt32(_datos.Hor_Pac_Cia);
                    Observacion = _datos.Hor_Observacion;
                    Carga_Profesionales(TipoCita);

                    Max15 = DateTime.Now.Date;

                    diasBloq = Preferencias.Bloqueo;
                    if (diasBloq >= 1)
                    {
                        Max15 = Convert.ToDateTime(Max15.AddDays(diasBloq));
                    }
                }
                else
                {
                    MessageBox.Show("Esta admision tiene errores o ya esta consumida, las admisiones para mover agendas " +
                            "no deben estar admisionadas ni hechas por los profesionales", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                (int CodProf, string TipoBod) CodeProf = (0, "");

                
                    CodeProf = repositorioBodegas.ProfesionalId(listBox1.SelectedItem.ToString());
                

                if (CodeProf.CodProf == 0)
                {
                    MessageBox.Show("Error inesperado ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }

                NumeroBodega = Convert.ToInt32(CodeProf.CodProf);
                CargaGrid();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public MoverAgenda(int _adm)
        {
            InitializeComponent();
            this.Admision = _adm;
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea reasignar la cita para la hora seleccionada?",
                                                  "Zamenis Health - Reasignacion de citas",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string ObservacionNueva = Observacion + " | --> NOVEDAD: Cita actualizada por " + Comunes.Contenedor.UsuarioLogueado;
                    DateTime FechaCita = Convert.ToDateTime(monthCalendar1.SelectionStart);

                    bool _chageProf = false;

                    
                        _chageProf = repositorioHorario2.changeProfesional(Admision,
                                                                          NumeroBodega,
                                                                          listView1.SelectedItems[0].SubItems[2].Text,
                                                                          ObservacionNueva,
                                                                          FechaCita,
                                                                          Convert.ToDateTime(listView1.SelectedItems[0].SubItems[0].Text));
                    

                    if (_chageProf != true)
                    {
                        MessageBox.Show("Inconveniente cambiando cita de profesional", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Reasignado con exito", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        Agendamiento f7 = Application.OpenForms.OfType<Agendamiento>().FirstOrDefault();
                        f7.EventoInicial();

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Hora", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Espacio", 400, HorizontalAlignment.Left);
            listView1.Columns.Add("IDE", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Habilita", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado", 0, HorizontalAlignment.Left);
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            try
            {
                label13.Text = Convert.ToDateTime(monthCalendar1.SelectionStart).ToString("dddd", CultureInfo.CreateSpecificCulture("es-ES"));

                if (diasBloq >= 1 && Convert.ToDateTime(monthCalendar1.SelectionStart) >= Convert.ToDateTime(Max15))
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "La agenda solo muestra " + diasBloq.ToString() + " dias habilitados para agendar citas, no es posible verificar mas espacios, " +
                        "sera redirigido nuevamente a la fecha de hoy.  Mayor informacion consulte al Gerente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    DateTime Hoy = DateTime.Now.Date;
                    monthCalendar1.SelectionStart = Hoy;                    
                }
                else
                {
                    CargaGrid();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CargaGrid()
        {
            try
            {
                List<CXN_HORARIO> _datosAgenda = new List<CXN_HORARIO>();

                
                    _datosAgenda = repositorioHorario2.CargarAgenda(NumeroBodega, Compañia, Convert.ToDateTime(monthCalendar1.SelectionStart), label13.Text);
                

                if (_datosAgenda != null)
                {
                    Encabezados();

                    foreach (var i in _datosAgenda)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                           {
                                Convert.ToDateTime(i.Hor_Pac_Hora_Cita).ToString("HH:mm tt"),
                                i.Hor_Imp_Age,
                                i.Hor_Pac_Id_Hora,
                                i.Hor_Autoriza,
                                i.Hor_Estado
                           }));
                    }

                    foreach (ListViewItem lvw in listView1.Items)
                    {
                        lvw.UseItemStyleForSubItems = false;

                        if (lvw.SubItems[3].Text != "A")
                        {
                            lvw.SubItems[0].ForeColor = Color.Red;
                        }
                        else
                        {
                            lvw.SubItems[0].ForeColor = Color.Green;
                        }

                        if (lvw.SubItems[1].Text != "")
                        {
                            lvw.Remove();
                        }
                    }
                }
                else
                {
                    Encabezados();
                    MessageBox.Show("Error inesperado en Grid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Carga_Profesionales(string TC)
        {
            try
            {
                string _filter = "";

                switch (TC)
                {
                    case "CU":
                        _filter = "Enfermeros";
                        break;

                    case "MG":
                        _filter = "Medicos Generales";
                        break;

                    case "FI":
                        _filter = "Fisiatras";
                        break;

                    case "TF":
                        _filter = "Terapeutas";
                        break;

                    case "TO":
                        _filter = "Terapeutas";
                        break;

                    case "PS":
                        _filter = "Terapeutas";
                        break;

                    default:
                        MessageBox.Show("Inconveniente interno", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                        return;
                }

                List<string> _profs = new List<string>();

                
                    _profs = repositorioBodegas.Profesionales(_filter);
                

                if (_profs != null)
                {
                    listBox1.Items.Clear();

                    foreach (var i in _profs)
                    {
                        listBox1.Items.Add(i);
                    }

                    listBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

    }
}
