using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.ControlUser.Clases;
using ZamenisHealth.Recepcion;

namespace ZamenisHealth.ControlUser.Controles
{
    public partial class CalZamenis : UserControl
    {
        private static readonly IAgendaC repoAgendaC = new MAgendaC();
        private static readonly IDisponibilidad repoDispo = new MDisponibilidad();

        private Comunes.MensajesGeneral MG;
        private CalZamenisClass calendar;
        private DataTable dt;
        private static List<DateTime> listaBloqueos;
        private Dictionary<string, int> dicOcupados;
        private List<DateTime> listaFestiva;

        public List<CXN_DISPONIBILIDAD_2> getlistasLunes;
        public List<CXN_DISPONIBILIDAD_2> getlistasMartes;
        public List<CXN_DISPONIBILIDAD_2> getlistasMiercoles;
        public List<CXN_DISPONIBILIDAD_2> getlistasJueves;
        public List<CXN_DISPONIBILIDAD_2> getlistasViernes;
        public List<CXN_DISPONIBILIDAD_2> getlistasSabado;
        public List<CXN_DISPONIBILIDAD_2> getlistasDomingo;
        public List<DiasAgenda> calendarFestivo;
        public int getCantDiaLunes;
        public int getCantDiaMartes;
        public int getCantDiaMiercoles;
        public int getCantDiaJueves;
        public int getCantDiaViernes;
        public int getCantDiaSabado;
        public int getCantDiaDomingo;
        public List<CXN_DIAS_WEB> getListaBloqueos = new List<CXN_DIAS_WEB>();
        public DateTime Max15;
        public int diasBloq;
        public int IdProf;
        public bool Anclar;

        int SizeFontDefault = 13;
        int SizeFontSelection = 16;

        public CalZamenis()
        {
            InitializeComponent();
        }

        private void CalZamenis_Load(object sender, EventArgs e)
        {
            try
            {
                this.Anclar = false;

                CalControlDGV.Font = new Font("Arial", SizeFontDefault);
                CalControlDGV.MouseMove += CalControlDGV_MouseMove;
                CalControlDGV.CellMouseLeave += CalControlDGV_CellMouseLeave;
                CalControlDGV.MouseLeave += CalControlDGV_MouseLeave;

                dTPCalendar.Value = DateTime.Now.Date;
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));
                //Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo1: " + ex.Message);
            }
        }

        void OcultarControl()
        {
            if (this.Anclar == false)
            {
                this.Visible = false;
            }            
            
            try
            {
                Agenda f2 = Application.OpenForms.OfType<Agenda>().FirstOrDefault();
                f2.setDayAgenda(txtDia.Text, txtFecha.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo CalendarSetFechaChanged: " + ex.Message, "ERROR");
            }
        }

        private void CalControlDGV_MouseLeave(object sender, EventArgs e)
        {
            CalControlDGV.ClearSelection();
        }

        private void CalControlDGV_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell currentCell = CalControlDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];
                currentCell.Style.Font = new Font("Arial", SizeFontDefault); // Cambiar el tamaño de fuente de vuelta al valor predeterminado

                if (currentCell.Style.BackColor == Color.LightGreen)
                {
                    currentCell.Style.Font = new Font("Arial", SizeFontSelection, FontStyle.Bold);
                }
            }
        }

        private void CalControlDGV_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point currentMousePosition = CalControlDGV.PointToClient(MousePosition);
                DataGridView.HitTestInfo hitTestInfo = CalControlDGV.HitTest(currentMousePosition.X, currentMousePosition.Y);

                int rowIndex = hitTestInfo.RowIndex;
                int columnIndex = hitTestInfo.ColumnIndex;

                if (rowIndex >= 0 && columnIndex >= 0)
                {
                    DataGridViewCell currentCell = CalControlDGV.Rows[rowIndex].Cells[columnIndex];
                    currentCell.Selected = true;
                    CalControlDGV.CurrentCell = currentCell;

                    currentCell.Style.Font = new Font("Arial", SizeFontSelection);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void Cargar()
        {
            try
            {
                CargarDias();
                getFestivos();
                getBloqueosCalendar();
                getIDESDay();
                CalControlDGV.ClearSelection();
                posisionarDia();
                DeshabilitarMayor15();

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();
                f2.fecha();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo2: " + ex.Message);
            }
        }
        void CargarDias()
        {
            try
            {
                CalControlDGV.DataSource = null;

                calendar = new CalZamenisClass();

                DateTime oDataMonth = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, 1);

                string nameDay = oDataMonth.ToString("dddd").ToUpper();
                int posInicioGrilla;

                switch (nameDay)
                {
                    case "LUNES":
                        posInicioGrilla = 0;
                        break;

                    case "MARTES":
                        posInicioGrilla = 1;
                        break;

                    case "MIÉRCOLES":
                        posInicioGrilla = 2;
                        break;

                    case "JUEVES":
                        posInicioGrilla = 3;
                        break;

                    case "VIERNES":
                        posInicioGrilla = 4;
                        break;

                    case "SÁBADO":
                        posInicioGrilla = 5;
                        break;

                    case "DOMINGO":
                        posInicioGrilla = 6;
                        break;

                    default:
                        MessageBox.Show("Error");
                        return;
                }

                dt = new DataTable();
                dt.Columns.Add("Lun", typeof(string));
                dt.Columns.Add("Mar", typeof(string));
                dt.Columns.Add("Mie", typeof(string));
                dt.Columns.Add("Jue", typeof(string));
                dt.Columns.Add("Vie", typeof(string));
                dt.Columns.Add("Sab", typeof(string));
                dt.Columns.Add("Dom", typeof(string));

                int getLastDay = calendar.getLastDay(dTPCalendar.Value.Date);
                int currentDay = 1;

                DataRow row = dt.NewRow();

                for (int i = 0; i < 6; i++)
                {                    
                    for (int j = 0; j < 7; j++)
                    {
                        if (i == 0 && j < posInicioGrilla)
                        {
                            row[j] = "";
                        }
                        else if (currentDay <= getLastDay)
                        {
                            row[j] = currentDay.ToString();
                            currentDay++;
                        }
                        else
                        {
                            row[j] = "";
                        }
                    }
                    dt.Rows.Add(row);
                    row = dt.NewRow();
                }

                CalControlDGV.DataSource = dt;
                
                CalControlDGV.Rows[0].Height = 40;
                CalControlDGV.Rows[1].Height = 40;
                CalControlDGV.Rows[2].Height = 40;
                CalControlDGV.Rows[3].Height = 40;
                CalControlDGV.Rows[4].Height = 40;
                CalControlDGV.Rows[5].Height = 40;

                CalControlDGV.Columns["Lun"].Width = 50;
                CalControlDGV.Columns["Mar"].Width = 50;
                CalControlDGV.Columns["Mie"].Width = 50;
                CalControlDGV.Columns["Jue"].Width = 50;
                CalControlDGV.Columns["Sab"].Width = 50;
                CalControlDGV.Columns["Dom"].Width = 50;
                CalControlDGV.Columns["Vie"].Width = 50;

                CalControlDGV.Columns["Lun"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Mar"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Mie"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Jue"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Vie"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Sab"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CalControlDGV.Columns["Dom"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                CalControlDGV.Columns["Lun"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Mar"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Mie"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Jue"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Vie"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Sab"].SortMode = DataGridViewColumnSortMode.NotSortable;
                CalControlDGV.Columns["Dom"].SortMode = DataGridViewColumnSortMode.NotSortable;

                CalControlDGV.EnableHeadersVisualStyles = false;

                CalControlDGV.ColumnHeadersDefaultCellStyle.Font = new Font(CalControlDGV.Font, FontStyle.Bold);
                CalControlDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.RoyalBlue;
                CalControlDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value.Date).ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));

                cmbAño.Text = dTPCalendar.Value.Date.Year.ToString();
                cmbMes.Text = calendar.Mes(dTPCalendar.Value.Date.Month);                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo3: " + ex.Message);
            }
        }
        void getFestivos()
        {
            try
            {
                List<DiasAgenda> getFestivo = calendarFestivo;
                if (getFestivo != null)
                {
                    listaFestiva = new List<DateTime>();

                    foreach (DiasAgenda agenda in getFestivo)
                    {
                        DateTime f = new DateTime(Convert.ToInt32(agenda.oAño), Convert.ToInt32(agenda.oMes), Convert.ToInt32(agenda.oDia));
                        listaFestiva.Add(f);
                    }

                    Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();
                    f2.listaFestiva = this.listaFestiva;

                    Festivos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo4: " + ex.Message);
            }
        }
        public void Festivos()
        {
            try
            {
                DateTime dateTemp;
                DateTime s = new DateTime(0001, 01, 1);
                DateTime c = new DateTime(0001, 01, 1);

                if (listaFestiva != null)
                {
                    foreach (DataGridViewRow r in CalControlDGV.Rows)
                    {
                        r.Cells["Dom"].Style.ForeColor = Color.Red;
                        r.Cells["Dom"].Style.BackColor = Color.FromArgb(255, 192, 192);
                        r.Cells["Dom"].Style.Font = new Font("Arial", SizeFontDefault);

                        if (r.Cells["Lun"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Lun"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Lun"].Style.ForeColor = Color.Red;
                                r.Cells["Lun"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Lun"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Mar"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mar"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Mar"].Style.ForeColor = Color.Red;
                                r.Cells["Mar"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Mar"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Mie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mie"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Mie"].Style.ForeColor = Color.Red;
                                r.Cells["Mie"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Mie"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Jue"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Jue"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Jue"].Style.ForeColor = Color.Red;
                                r.Cells["Jue"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Jue"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Vie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Vie"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Vie"].Style.ForeColor = Color.Red;
                                r.Cells["Vie"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Vie"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Sab"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Sab"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Sab"].Style.ForeColor = Color.Red;
                                r.Cells["Sab"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Sab"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }

                        if (r.Cells["Dom"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Dom"].Value.ToString()));
                            s = listaFestiva.Find(DateTime => DateTime == dateTemp);
                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Dom"].Style.ForeColor = Color.Red;
                                r.Cells["Dom"].Style.BackColor = Color.FromArgb(255, 192, 192);
                                r.Cells["Dom"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo5: " + ex.Message);
            }
        }
        void getBloqueosCalendar()
        {
            try
            {
                List<CXN_DIAS_WEB> getBloqueos = new List<CXN_DIAS_WEB>();
                getBloqueos = this.getListaBloqueos;

                if (getBloqueos != null)
                {
                    listaBloqueos = new List<DateTime>();

                    foreach (CXN_DIAS_WEB dias in getBloqueos)
                    {
                        DateTime f = new DateTime(dias.F_Fecha.Year, dias.F_Fecha.Month, dias.F_Fecha.Day);
                        listaBloqueos.Add(f);
                    }

                    DiasBloquados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo6: " + ex.Message);
            }
        }
        public void DiasBloquados()
        {
            try
            {
                DateTime dateTemp;
                DateTime s = new DateTime(0001, 01, 1);
                DateTime c = new DateTime(0001, 01, 1);
                DataGridViewCell U1;

                if (listaBloqueos != null)
                {
                    foreach (DataGridViewRow r in CalControlDGV.Rows)
                    {
                        if (r.Cells["Lun"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Lun"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Lun"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Lun"].Style.ForeColor = Color.White;
                                r.Cells["Lun"].Style.BackColor = Color.Red;
                                r.Cells["Lun"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Lun"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Lun"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Mar"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mar"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Mar"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Mar"].Style.ForeColor = Color.White;
                                r.Cells["Mar"].Style.BackColor = Color.Red;
                                r.Cells["Mar"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Mar"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Mar"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Mie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mie"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Mie"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Mie"].Style.ForeColor = Color.White;
                                r.Cells["Mie"].Style.BackColor = Color.Red;
                                r.Cells["Mie"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Mie"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Mie"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Jue"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Jue"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Jue"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Jue"].Style.ForeColor = Color.White;
                                r.Cells["Jue"].Style.BackColor = Color.Red;
                                r.Cells["Jue"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Jue"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Jue"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Vie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Vie"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Vie"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Vie"].Style.ForeColor = Color.White;
                                r.Cells["Vie"].Style.BackColor = Color.Red;
                                r.Cells["Vie"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Vie"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Vie"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Sab"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Sab"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Sab"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Sab"].Style.ForeColor = Color.White;
                                r.Cells["Sab"].Style.BackColor = Color.Red;
                                r.Cells["Sab"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Sab"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Sab"].Style.BackColor = U1.Style.BackColor;
                            }
                        }

                        if (r.Cells["Dom"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Dom"].Value.ToString()));
                            s = listaBloqueos.Find(DateTime => DateTime == dateTemp);

                            U1 = r.Cells["Dom"];

                            if (Convert.ToDateTime(s) != Convert.ToDateTime(c))
                            {
                                r.Cells["Dom"].Style.ForeColor = Color.White;
                                r.Cells["Dom"].Style.BackColor = Color.Red;
                                r.Cells["Dom"].Style.Font = new Font("Arial", SizeFontDefault);
                            }
                            else
                            {
                                r.Cells["Dom"].Style.ForeColor = U1.Style.ForeColor;
                                r.Cells["Dom"].Style.BackColor = U1.Style.BackColor;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo7: " + ex.Message);
            }
        }
        void getIDESDay()
        {
            try
            {
                dicOcupados = new Dictionary<string, int>();

                List<CXN_DISPONIBILIDAD_2> getlistas = new List<CXN_DISPONIBILIDAD_2>();

                getlistas = getlistasLunes;
                if (getlistas != null)
                {
                    dicOcupados.Add("Lunes", getlistas.Count);
                }
                getlistas = getlistasMartes;
                if (getlistas != null)
                {
                    dicOcupados.Add("Martes", getlistas.Count);
                }
                getlistas = getlistasMiercoles;
                if (getlistas != null)
                {
                    dicOcupados.Add("Miércoles", getlistas.Count);
                }
                getlistas = getlistasJueves;
                if (getlistas != null)
                {
                    dicOcupados.Add("Jueves", getlistas.Count);
                }
                getlistas = getlistasViernes;
                if (getlistas != null)
                {
                    dicOcupados.Add("Viernes", getlistas.Count);
                }
                getlistas = getlistasSabado;
                if (getlistas != null)
                {
                    dicOcupados.Add("Sábado", getlistas.Count);
                }
                getlistas = getlistasDomingo;
                if (getlistas != null)
                {
                    dicOcupados.Add("Domingo", getlistas.Count);
                }

                Ocupados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo8: " + ex.Message);
            }
        }
        void Ocupados()
        {
            try
            {
                DateTime dateTemp;
                DataGridViewCell U1;

                if (dicOcupados != null)
                {
                    foreach (DataGridViewRow r in CalControlDGV.Rows)
                    {
                        if (r.Cells["Lun"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Lun"].Value.ToString()));

                            U1 = r.Cells["Lun"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Lunes"])
                                {
                                    r.Cells["Lun"].Style.ForeColor = Color.Blue;
                                    r.Cells["Lun"].Style.BackColor = Color.Yellow;
                                    r.Cells["Lun"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Lun"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Lun"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }
                        if (r.Cells["Mar"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mar"].Value.ToString()));

                            U1 = r.Cells["Mar"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Martes"])
                                {
                                    r.Cells["Mar"].Style.ForeColor = Color.Blue;
                                    r.Cells["Mar"].Style.BackColor = Color.Yellow;
                                    r.Cells["Mar"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Mar"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Mar"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }
                        if (r.Cells["Mie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Mie"].Value.ToString()));

                            U1 = r.Cells["Mie"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Miércoles"])
                                {
                                    r.Cells["Mie"].Style.ForeColor = Color.Blue;
                                    r.Cells["Mie"].Style.BackColor = Color.Yellow;
                                    r.Cells["Mie"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Mie"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Mie"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }
                        if (r.Cells["Jue"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Jue"].Value.ToString()));

                            U1 = r.Cells["Jue"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Jueves"])
                                {
                                    r.Cells["Jue"].Style.ForeColor = Color.Blue;
                                    r.Cells["Jue"].Style.BackColor = Color.Yellow;
                                    r.Cells["Jue"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Jue"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Jue"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }
                        if (r.Cells["Vie"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Vie"].Value.ToString()));

                            U1 = r.Cells["Vie"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Viernes"])
                                {
                                    r.Cells["Vie"].Style.ForeColor = Color.Blue;
                                    r.Cells["Vie"].Style.BackColor = Color.Yellow;
                                    r.Cells["Vie"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Vie"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Vie"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }
                        if (r.Cells["Sab"].Value.ToString() != "")
                        {
                            dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Sab"].Value.ToString()));

                            U1 = r.Cells["Sab"];

                            int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                            if (getCantDia != 0)
                            {
                                if (getCantDia == dicOcupados["Sábado"])
                                {
                                    r.Cells["Sab"].Style.ForeColor = Color.Blue;
                                    r.Cells["Sab"].Style.BackColor = Color.Yellow;
                                    r.Cells["Sab"].Style.Font = new Font("Arial", SizeFontDefault);
                                }
                                else
                                {
                                    r.Cells["Sab"].Style.ForeColor = U1.Style.ForeColor;
                                    r.Cells["Sab"].Style.BackColor = U1.Style.BackColor;
                                }
                            }
                        }

                        if (dicOcupados.ContainsKey("Domingo"))
                        {
                            if (r.Cells["Dom"].Value.ToString() != "")
                            {
                                dateTemp = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, Convert.ToInt32(r.Cells["Dom"].Value.ToString()));

                                U1 = r.Cells["Dom"];

                                int getCantDia = repoAgendaC.getOcupados(Convert.ToDateTime(dateTemp), this.IdProf);
                                if (getCantDia != 0)
                                {
                                    if (getCantDia == dicOcupados["Domingo"])
                                    {
                                        r.Cells["Dom"].Style.ForeColor = Color.Blue;
                                        r.Cells["Dom"].Style.BackColor = Color.Yellow;
                                        r.Cells["Dom"].Style.Font = new Font("Arial", SizeFontDefault);
                                    }
                                    else
                                    {
                                        r.Cells["Dom"].Style.ForeColor = U1.Style.ForeColor;
                                        r.Cells["Dom"].Style.BackColor = U1.Style.BackColor;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Codigo9: " + ex.Message);
            }
        }
        void posisionarDia()
        {
            try
            {
                int daySelected = dTPCalendar.Value.Day;

                foreach (DataGridViewRow r in CalControlDGV.Rows)
                {
                    if (r.Cells["Lun"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Lun"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Lun"].Style.BackColor = Color.LightGreen;
                        r.Cells["Lun"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Mar"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Mar"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Mar"].Style.BackColor = Color.LightGreen;
                        r.Cells["Mar"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Mie"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Mie"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Mie"].Style.BackColor = Color.LightGreen;
                        r.Cells["Mie"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Jue"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Jue"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Jue"].Style.BackColor = Color.LightGreen;
                        r.Cells["Jue"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Vie"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Vie"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Vie"].Style.BackColor = Color.LightGreen;
                        r.Cells["Vie"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Sab"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Sab"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Sab"].Style.BackColor = Color.LightGreen;
                        r.Cells["Sab"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }

                    if (r.Cells["Dom"].Value.ToString() == daySelected.ToString())
                    {
                        r.Cells["Dom"].Style.ForeColor = Color.DarkGreen;
                        r.Cells["Dom"].Style.BackColor = Color.LightGreen;
                        r.Cells["Dom"].Style.Font = new Font("Arial", SizeFontSelection);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo10: " + ex.Message);
            }
        }
        void DeshabilitarMayor15()
        {
            try
            {
                if (diasBloq >= 1)
                {
                    foreach (DataGridViewRow r in CalControlDGV.Rows)
                    {
                        if (r.Cells["Lun"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Lun"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Lun"].Style.ForeColor = Color.Gray;
                                r.Cells["Lun"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Mar"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Mar"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Mar"].Style.ForeColor = Color.Gray;
                                r.Cells["Mar"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Mie"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Mie"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Mie"].Style.ForeColor = Color.Gray;
                                r.Cells["Mie"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Jue"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Jue"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Jue"].Style.ForeColor = Color.Gray;
                                r.Cells["Jue"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Vie"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Vie"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Vie"].Style.ForeColor = Color.Gray;
                                r.Cells["Vie"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Sab"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Sab"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Sab"].Style.ForeColor = Color.Gray;
                                r.Cells["Sab"].Style.BackColor = Color.DarkGray;
                            }
                        }
                        if (r.Cells["Dom"].Value.ToString() != "")
                        {
                            DateTime fechaTemp = new DateTime(Convert.ToInt32(cmbAño.Text), Convert.ToInt32(calendar.nMes(cmbMes.Text)), Convert.ToInt32(r.Cells["Dom"].Value.ToString()));
                            if (Convert.ToDateTime(fechaTemp) >= Convert.ToDateTime(Max15))
                            {
                                r.Cells["Dom"].Style.ForeColor = Color.Gray;
                                r.Cells["Dom"].Style.BackColor = Color.DarkGray;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo11: " + ex.Message);
            }
        }
        public void CalControlDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (CalControlDGV.Rows[e.RowIndex].Cells[CalControlDGV.CurrentCell.ColumnIndex].Value.ToString() == "")
                {
                    CalControlDGV.ClearSelection();
                    return;
                }

                CalControlDGV.Enabled = false;

                calendar = new CalZamenisClass();

                int DiaSelTemp = Convert.ToInt32(CalControlDGV.Rows[e.RowIndex].Cells[CalControlDGV.CurrentCell.ColumnIndex].Value.ToString());
                int MesSelTemp = calendar.nMes(cmbMes.Text);
                int AñoSelTemp = Convert.ToInt32(cmbAño.Text);

                DateTime fSelected = new DateTime(AñoSelTemp, MesSelTemp, DiaSelTemp);
                dTPCalendar.Value = fSelected;

                txtFecha.Text = new DateTime(AñoSelTemp, MesSelTemp, DiaSelTemp).ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(new DateTime(AñoSelTemp, MesSelTemp, DiaSelTemp).ToString("dddd"));

                string fSelectedTxt = txtFecha.Text;
                string fSelectedLetters = txtDia.Text;

                if (diasBloq >= 1 && Convert.ToDateTime(txtFecha.Text) >= Convert.ToDateTime(Max15))
                {
                    string Mensaje = "La agenda solo muestra " + diasBloq.ToString() + " dias habilitados para agendar citas, no es posible verificar mas espacios, " +
                        "sera redirigido nuevamente a la fecha de hoy.  Mayor informacion consulte al Gerente";
                    MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = Mensaje;
                    MG.TipoImagen = 0;
                    MG.ShowDialog();

                    DateTime Hoy = DateTime.Now.Date;
                    CalControlDGV.ClearSelection();
                    dTPCalendar.Value = Hoy;
                    txtFecha.Text = Hoy.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(Hoy.ToString("dddd"));

                    CalControlDGV.Enabled = true;
                }
                else
                {
                    Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                    this.IdProf = f2.IdProf;

                    this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                    this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                    this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                    this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                    this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                    this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                    this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                    this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                    this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                    this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                    CargarDias(); //Llena el calendario
                    getFestivos(); //Obtiene los festivos del calendario
                    getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                    getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                    CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                    posisionarDia(); //Resalta en verde el dia seleccionado
                    DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                           //                    fecha(); //Llena el grid de la agenda

                    f2.fecha();


                    CalControlDGV.Enabled = true;
                }

                OcultarControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo12: " + ex.Message);
            }
        }
        private void cmbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        private void cmbAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        private void cmbMes_Leave(object sender, EventArgs e)
        {
            try
            {
                calendar = new CalZamenisClass();
                dTPCalendar.Value = new DateTime(dTPCalendar.Value.Year, calendar.nMes(cmbMes.SelectedItem.ToString()), dTPCalendar.Value.Day);
                txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.ToString("dddd"));

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }
            catch (Exception ex)
            {
                DateTime fSelected = dTPCalendar.Value;
                CalZamenisClass C = new CalZamenisClass();

                if (fSelected.Day == 31 || fSelected.Day == 30)
                {
                    int getmonth = C.nMes(cmbMes.Text);
                    DateTime rmpl = new DateTime(fSelected.Year, getmonth, fSelected.Day - 2);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }
                else
                {
                    int getmonth = C.nMes(cmbMes.Text);
                    DateTime rmpl = new DateTime(fSelected.Year, getmonth, fSelected.Day);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();

                MessageBox.Show("Codigo13: " + ex.Message);
            }
        }
        private void cmbAño_Leave(object sender, EventArgs e)
        {
            try
            {
                dTPCalendar.Value = new DateTime(Convert.ToInt32(cmbAño.SelectedItem), dTPCalendar.Value.Month, dTPCalendar.Value.Day);
                txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.ToString("dddd"));

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo14: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int año = 0;

                switch (cmbMes.Text)
                {
                    case "ENERO":
                        cmbMes.SelectedItem = "FEBRERO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 2, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "FEBRERO":
                        cmbMes.SelectedItem = "MARZO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 3, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "MARZO":
                        cmbMes.SelectedItem = "ABRIL";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 4, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "ABRIL":
                        cmbMes.SelectedItem = "MAYO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 5, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "MAYO":
                        cmbMes.SelectedItem = "JUNIO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 6, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "JUNIO":
                        cmbMes.SelectedItem = "JULIO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 7, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "JULIO":
                        cmbMes.SelectedItem = "AGOSTO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 8, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "AGOSTO":
                        cmbMes.SelectedItem = "SEPTIEMBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 9, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "SEPTIEMBRE":
                        cmbMes.SelectedItem = "OCTUBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 10, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "OCTUBRE":
                        cmbMes.SelectedItem = "NOVIEMBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 11, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "NOVIEMBRE":
                        cmbMes.SelectedItem = "DICIEMBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 12, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "DICIEMBRE":
                        año = Convert.ToInt32(cmbAño.Text) + 1;
                        dTPCalendar.Value = new DateTime(año, 1, 1);
                        cmbMes.SelectedItem = "ENERO";
                        cmbAño.Text = año.ToString();
                        break;
                }


                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }
            catch //(Exception ex)
            {
                DateTime fSelected = dTPCalendar.Value;

                if (fSelected.Month == 12)
                {
                    DateTime rmpl = new DateTime(fSelected.Year + 1, 1, 1);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }
                else
                {
                    DateTime rmpl = new DateTime(fSelected.Year, fSelected.Month + 1, 1);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }

                //MessageBox.Show("Codigo15: " + ex.Message);


                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }
        }
        private void btnPreviusMonth_Click(object sender, EventArgs e)
        {
            try
            {
                int año = 0;

                switch (cmbMes.Text)
                {
                    case "ENERO":
                        año = Convert.ToInt32(cmbAño.Text) - 1;
                        dTPCalendar.Value = new DateTime(año, 12, 1);
                        cmbAño.Text = año.ToString();
                        cmbMes.SelectedItem = "DICIEMBRE";

                        break;

                    case "FEBRERO":
                        cmbMes.SelectedItem = "ENERO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 1, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "MARZO":
                        cmbMes.SelectedItem = "FEBRERO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 2, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "ABRIL":
                        cmbMes.SelectedItem = "MARZO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 3, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "MAYO":
                        cmbMes.SelectedItem = "ABRIL";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 4, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "JUNIO":
                        cmbMes.SelectedItem = "MAYO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 4, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "JULIO":
                        cmbMes.SelectedItem = "JUNIO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 6, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "AGOSTO":
                        cmbMes.SelectedItem = "JULIO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 7, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "SEPTIEMBRE":
                        cmbMes.SelectedItem = "AGOSTO";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 8, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "OCTUBRE":
                        cmbMes.SelectedItem = "SEPTIEMBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 9, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "NOVIEMBRE":
                        cmbMes.SelectedItem = "OCTUBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 10, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;

                    case "DICIEMBRE":
                        cmbMes.SelectedItem = "NOVIEMBRE";

                        año = Convert.ToInt32(cmbAño.Text);
                        dTPCalendar.Value = new DateTime(año, 11, Convert.ToInt32(Convert.ToDateTime(txtFecha.Text).Day));
                        txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                        txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                        break;
                }

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }
            catch //(Exception ex)
            {
                DateTime fSelected = dTPCalendar.Value;

                if (fSelected.Month == 1)
                {
                    DateTime rmpl = new DateTime(fSelected.Year - 1, 12, 1);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }
                else
                {
                    DateTime rmpl = new DateTime(fSelected.Year, fSelected.Month - 1, 1);
                    dTPCalendar.Value = rmpl;
                    txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                    txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));
                }

                //MessageBox.Show("Codigo16: " + ex.Message);

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();
            }

        }
        private void btnHoyReturn_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;
                dTPCalendar.Value = Hoy;
                txtFecha.Text = dTPCalendar.Value.ToString("yyyy-MM-dd");
                txtDia.Text = CalZamenisClass.Capitalize(dTPCalendar.Value.Date.ToString("dddd"));

                Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                this.IdProf = f2.IdProf;

                this.getlistasLunes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMartes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasMiercoles = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasJueves = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasViernes = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasSabado = new List<CXN_DISPONIBILIDAD_2>();
                this.getlistasDomingo = new List<CXN_DISPONIBILIDAD_2>();

                this.getListaBloqueos = repoAgendaC.CargarListBlocked(f2.IdProf);
                this.getlistasLunes = repoDispo.getHorariosHabilitados(f2.IdProf, "Lunes");
                this.getlistasMartes = repoDispo.getHorariosHabilitados(f2.IdProf, "Martes");
                this.getlistasMiercoles = repoDispo.getHorariosHabilitados(f2.IdProf, "Miércoles");
                this.getlistasJueves = repoDispo.getHorariosHabilitados(f2.IdProf, "Jueves");
                this.getlistasViernes = repoDispo.getHorariosHabilitados(f2.IdProf, "Viernes");
                this.getlistasSabado = repoDispo.getHorariosHabilitados(f2.IdProf, "Sábado");
                this.getlistasDomingo = repoDispo.getHorariosHabilitados(f2.IdProf, "Domingo");

                CargarDias(); //Llena el calendario
                getFestivos(); //Obtiene los festivos del calendario
                getBloqueosCalendar(); //Obtiene los dias bloqueados por profesional dependiendo del que esta seleccionado actualmente (Rojo)
                getIDESDay(); //Determina si un dia dependiendo de profesional seleccionado esta lleno o no (Amarillo)
                CalControlDGV.ClearSelection(); //Limpia la seleccion hecha defecto del datagrid 
                posisionarDia(); //Resalta en verde el dia seleccionado
                DeshabilitarMayor15(); //Verifica cuantos dias por delante se bloquea la agenda para citas
                                       //                    fecha(); //Llena el grid de la agenda

                f2.fecha();

                OcultarControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Codigo17: " + ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OcultarControl();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (label9.Text == "DesAnclado")
            {
                label9.Text = "Anclado";
                this.Anclar = true;
                return;
            }

            if (label9.Text == "Anclado")
            {
                label9.Text = "DesAnclado";
                this.Anclar = false;
                return;
            }
        }
    }
}

