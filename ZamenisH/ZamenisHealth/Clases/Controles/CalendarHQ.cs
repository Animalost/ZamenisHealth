using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public partial class CalendarHQ : UserControl
    {
        private CalendarController oController;

        private DataTable dt;

        public int CodeProfesional; //Enviar Este
        private int SizeFontDefault = 13;
        private int SizeFontSelection = 16;
        public int Año, Mes, Dia;
        private bool Recargar;
        public int DiaSeleccionadoRow, DiaSeleccionadoColumn;

        public CalendarHQ()
        {
            InitializeComponent();
            oController = new CalendarController();
        }

        #region EVENTOS CONTROLES
        public void MetodoPrincipal()
        {
            try
            {
                dTPCalendar.Value = new DateTime(Año, Mes, Dia);
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));

                Cargar();
            }
            catch (Exception ex)
            {
                dTPCalendar.Value = new DateTime(Año, Mes, 1);
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));

                Cargar();
                // MessageBox.Show(ex.Message, "METODO PRINCIPAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CambiosMesAño()
        {
            try
            {
                dTPCalendar.Value = new DateTime(Convert.ToInt32(cmbAño.Text), nMesCal(cmbMes.Text), Dia);
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));

                Cargar();
            }
            catch (Exception ex)
            {
                dTPCalendar.Value = new DateTime(Convert.ToInt32(cmbAño.Text), nMesCal(cmbMes.Text), 1);
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));

                Cargar();
                //MessageBox.Show(ex.Message, "CAMBIOSMESAÑO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CalendarHQ_Load(object sender, EventArgs e)
        {
            try
            {
                Recargar = false;

                CalControlDGV.Font = new Font("Arial", SizeFontDefault);
                CalControlDGV.MouseMove += CalControlDGV_MouseMove;
                CalControlDGV.CellMouseLeave += CalControlDGV_CellMouseLeave;
                CalControlDGV.MouseLeave += CalControlDGV_MouseLeave;
                CalControlDGV.CellClick += CalControlDGV_CellClick;

                Año = DateTime.Now.Year;
                Mes = DateTime.Now.Month;
                Dia = DateTime.Now.Day;

                dTPCalendar.Value = new DateTime(Año, Mes, Dia);
                txtFecha.Text = Convert.ToDateTime(dTPCalendar.Value).ToString("yyyy-MM-dd");
                txtDia.Text = Capitalize(Convert.ToDateTime(dTPCalendar.Value).ToString("dddd"));
                cmbAño.Text = dTPCalendar.Value.Year.ToString();
                cmbMes.Text = MesCal(dTPCalendar.Value.Date.Month);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CalendarHQ_Load: " + ex.Message);
            }
        }
        private void cmbAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Recargar == false)
                {
                    return;
                }

                Recargar = false;
                CambiosMesAño();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "cmbAño_SelectedIndexChanged", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Recargar == false)
                {
                    return;
                }

                Recargar = false;
                CambiosMesAño();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "cmbMes_SelectedIndexChanged", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Recargar = false;

                int obtenerMes = nMesCal(cmbMes.Text);
                int nuevoMes = 0;

                if (obtenerMes == 12)
                {
                    Año = Año + 1;
                    nuevoMes = 1;
                }
                else
                {
                    nuevoMes = obtenerMes + 1;
                }

                dTPCalendar.Value = new DateTime(Año, nuevoMes, Dia);
                cmbMes.Text = MesCal(nuevoMes);
                Mes = nuevoMes;
                MetodoPrincipal();
            }
            catch (Exception ex)
            {
                Recargar = false;

                int obtenerMes = nMesCal(cmbMes.Text);
                int nuevoMes = 0;

                if (obtenerMes == 12)
                {
                    Año = Año + 1;
                    nuevoMes = 1;
                }
                else
                {
                    nuevoMes = obtenerMes + 1;
                }

                dTPCalendar.Value = new DateTime(Año, nuevoMes, 1);
                cmbMes.Text = MesCal(nuevoMes);
                Mes = nuevoMes;
                MetodoPrincipal();

                //MessageBox.Show(ex.Message, "button1_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnPreviusMonth_Click(object sender, EventArgs e)
        {
            try
            {
                Recargar = false;

                int obtenerMes = nMesCal(cmbMes.Text);
                int nuevoMes = 0;

                if (obtenerMes == 1)
                {
                    nuevoMes = 12;
                    Año = Año - 1;
                }
                else
                {
                    nuevoMes = obtenerMes - 1;
                }

                dTPCalendar.Value = new DateTime(Año, nuevoMes, Dia);
                cmbMes.Text = MesCal(nuevoMes);
                Mes = nuevoMes;
                MetodoPrincipal();
            }
            catch (Exception ex)
            {
                Recargar = false;

                int obtenerMes = nMesCal(cmbMes.Text);
                int nuevoMes = 0;

                if (obtenerMes == 1)
                {
                    nuevoMes = 12;
                    Año = Año - 1;
                }
                else
                {
                    nuevoMes = obtenerMes - 1;
                }

                dTPCalendar.Value = new DateTime(Año, nuevoMes, 1);
                cmbMes.Text = MesCal(nuevoMes);
                Mes = nuevoMes;
                MetodoPrincipal();

                //MessageBox.Show(ex.Message, "btnPreviusMonth_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnHoyReturn_Click(object sender, EventArgs e)
        {
            try
            {
                Recargar = false;

                Año = DateTime.Now.Year;
                Mes = DateTime.Now.Month;
                Dia = DateTime.Now.Day;

                dTPCalendar.Value = new DateTime(Año, Mes, Dia);
                MetodoPrincipal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "btnHoyReturn_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        #endregion

        #region EVENTOS DEL MOUSE - ESTILOS
        private void CalControlDGV_MouseLeave(object sender, EventArgs e)
        {
            CalControlDGV.ClearSelection();
        }
        private void CalControlDGV_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CalControlDGV_CellMouseLeave", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "CalControlDGV_MouseMove", MessageBoxButtons.OK);
            }
        }
        void CalControlDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Recargar = false;

                if (CalControlDGV.Rows[e.RowIndex].Cells[CalControlDGV.CurrentCell.ColumnIndex].Value != null)
                {
                    Año = Convert.ToInt32(cmbAño.Text);
                    Mes = nMesCal(cmbMes.Text);
                    Dia = Convert.ToInt32(CalControlDGV.Rows[e.RowIndex].Cells[CalControlDGV.CurrentCell.ColumnIndex].Value.ToString());

                    DiaSeleccionadoRow = CalControlDGV.Rows[e.RowIndex].Index;
                    DiaSeleccionadoColumn = CalControlDGV.Rows[e.RowIndex].Cells[CalControlDGV.CurrentCell.ColumnIndex].ColumnIndex;

                    MetodoPrincipal();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "CalControlDGV_CellClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region EVENTOS PRINCIPALES       

        public void Cargar()
        {
            try
            {
                CargarDias();
                getFestivos();
                getBloqueosPforesional();
                //getIDESDay(); //falta obtener dias copados
                CalControlDGV.ClearSelection();
                posisionarDia();
                //DeshabilitarMayor15();

                Recargar = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cargar: " + ex.Message);
            }
        }
        void CargarDias()
        {
            try
            {
                CalControlDGV.DataSource = null;
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

                int getLastDay = getLastDays(dTPCalendar.Value.Date);
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

                Estilos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("CargarDias: " + ex.Message);
            }
        }
        public void Estilos()
        {
            try
            {
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
                txtDia.Text = Capitalize(dTPCalendar.Value.Date.ToString("dddd"));

                cmbAño.Text = dTPCalendar.Value.Date.Year.ToString();
                cmbMes.Text = MesCal(dTPCalendar.Value.Date.Month);

                CalControlDGV.Columns[6].DefaultCellStyle.BackColor = Color.Coral;
                CalControlDGV.Columns[6].DefaultCellStyle.ForeColor = Color.DarkRed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void getFestivos()
        {
            try
            {
                DateTime Desde = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, 1);
                DateTime Hasta = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, getLastDays(dTPCalendar.Value));

                List<DateTime> listaFestiva = oController.getFestivos(Desde, Hasta);
                if (listaFestiva != null)
                {
                    // Convertimos a HashSet para búsqueda rápida
                    HashSet<int> diasFestivos = listaFestiva
                        .Select(f => f.Day)
                        .ToHashSet();

                    foreach (DataGridViewRow row in CalControlDGV.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && int.TryParse(cell.Value.ToString(), out int dia))
                            {
                                if (diasFestivos.Contains(dia))
                                {
                                    cell.Style.BackColor = Color.Coral;
                                    cell.Style.ForeColor = Color.DarkRed;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("getFestivos: " + ex.Message);
            }
        }
        void LimpiarColoresGrid()
        {
            foreach (DataGridViewRow row in CalControlDGV.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex == 6)
                    {
                        cell.Style.BackColor = Color.Red;
                        cell.Style.ForeColor = Color.White;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.White;
                        cell.Style.ForeColor = Color.Black;
                    }
                }
            }
            getFestivos();
            posisionarDia();
        }
        public void getBloqueosPforesional()
        {
            try
            {
                LimpiarColoresGrid();

                DateTime Desde = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, 1);
                DateTime Hasta = new DateTime(dTPCalendar.Value.Year, dTPCalendar.Value.Month, getLastDays(dTPCalendar.Value));

                List<DateTime> listaBloqueada = oController.GetBloqueosProfesionales(Desde, Hasta, CodeProfesional);
                if (listaBloqueada != null)
                {
                    // Convertimos a HashSet para búsqueda rápida
                    HashSet<int> diasFestivos = listaBloqueada
                        .Select(f => f.Day)
                        .ToHashSet();

                    foreach (DataGridViewRow row in CalControlDGV.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && int.TryParse(cell.Value.ToString(), out int dia))
                            {
                                if (diasFestivos.Contains(dia))
                                {
                                    cell.Style.BackColor = Color.LightGray;
                                    cell.Style.ForeColor = Color.DimGray;                                    
                                }
                            }
                        }
                    }                   
                }

                VerificarDiasLlenos(Desde);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "getBloqueosPforesional", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void VerificarDiasLlenos(DateTime Fecha)
        { 
            try
            {
                // UNA SOLA CONSULTA A SQL
                List<DiaCalendarios> listaDias =
                    new CalendarController().ObtenerDiasLlenos(
                        CodeProfesional,
                        Fecha.Year,
                        Fecha.Month);

                if (listaDias == null || listaDias.Count == 0)
                    return;

                // Convertimos la lista en un diccionario
                // para encontrar rápidamente el día
                Dictionary<int, DiaCalendarios> dias =
                    listaDias.ToDictionary(x => x.Dia);

                foreach (DataGridViewRow row in CalControlDGV.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null &&
                            int.TryParse(cell.Value.ToString(), out int dia))
                        {
                            // Evitamos fechas inválidas
                            if (dia < 1 || dia > DateTime.DaysInMonth(
                                Fecha.Year,
                                Fecha.Month))
                            {
                                continue;
                            }

                            DateTime FechaLoop =
                                new DateTime(
                                    Fecha.Year,
                                    Fecha.Month,
                                    dia);

                            // Domingo no se procesa
                            if (FechaLoop.DayOfWeek == DayOfWeek.Sunday)
                                continue;

                            // ¿Tenemos información de este día?
                            if (dias.TryGetValue(dia, out DiaCalendarios datos))
                            {
                                // ¿Está lleno?
                                if (datos.CantidadUsada >=
                                    datos.CantidadHabilitada)
                                {
                                    cell.Style.BackColor = Color.LightBlue;
                                    cell.Style.ForeColor = Color.Blue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR DiasLLenos: {ex.Message}");
            }
        }
        void posisionarDia()
        {
            try
            {
                int diaSelected = dTPCalendar.Value.Day;

                foreach (DataGridViewRow row in CalControlDGV.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && int.TryParse(cell.Value.ToString(), out int dia))
                        {
                            if (diaSelected == dia)
                            {
                                cell.Style.BackColor = Color.LightGreen;
                                cell.Style.ForeColor = Color.Green;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "posisionarDia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region METODOS AUXILIARES
        private int getLastDays(DateTime sDate)
        {
            try
            {
                DateTime date = new DateTime(sDate.Year, sDate.Month, 1);
                DateTime oUltimoDiaDelMes = date.AddMonths(1).AddDays(-1);
                return oUltimoDiaDelMes.Day;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "getLastDays", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        private string MesCal(int nMes)
        {
            switch (nMes)
            {
                case 1:
                    return "ENERO";
                case 2:
                    return "FEBRERO";
                case 3:
                    return "MARZO";
                case 4:
                    return "ABRIL";
                case 5:
                    return "MAYO";
                case 6:
                    return "JUNIO";
                case 7:
                    return "JULIO";
                case 8:
                    return "AGOSTO";
                case 9:
                    return "SEPTIEMBRE";
                case 10:
                    return "OCTUBRE";
                case 11:
                    return "NOVIEMBRE";
                case 12:
                    return "DICIEMBRE";

                default:
                    return "ERROR";
            }
        }       
        private int nMesCal(string Mes)
        {
            switch (Mes)
            {
                case "ENERO":
                    return 1;
                case "FEBRERO":
                    return 2;
                case "MARZO":
                    return 3;
                case "ABRIL":
                    return 4;
                case "MAYO":
                    return 5;
                case "JUNIO":
                    return 6;
                case "JULIO":
                    return 7;
                case "AGOSTO":
                    return 8;
                case "SEPTIEMBRE":
                    return 9;
                case "OCTUBRE":
                    return 10;
                case "NOVIEMBRE":
                    return 11;
                case "DICIEMBRE":
                    return 12;

                default:
                    return 0;
            }
        }
        private string Capitalize(string s)
        {
            if (String.IsNullOrEmpty(s))
            {

            }
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
        #endregion

    }
}

