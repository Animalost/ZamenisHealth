using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    public partial class HistorialAdmisiones : Forma2
    {
        private IAgendaC oAgendaC;
        DataTable dt;
        private DataView vistaHistorial;

        DataColumn Admision;
        DataColumn FechaCita;
        DataColumn Paciente;
        DataColumn FechaGestion;
        DataColumn Usuario;
        DataColumn Gestion;

        public HistorialAdmisiones()
        {
            InitializeComponent();
            oAgendaC = new MAgendaC();
        }

        private async void HistorialAdmisiones_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Movimiento de Admisiones";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.DisableRowsAdded();
            await CargarGrrilla();
        }
        async Task CargarGrrilla()
        {
            pictureBox1.Visible = true;
            boton1.Enabled = false;

            try
            {
                // Permitir que Windows Forms actualice la interfaz.
                await Task.Yield();
                List<CXN_HISTORIAL_ADMISION> _lista = await oAgendaC.HistorialAdmision();

                // Crear la estructura sin enlazarla todavía al grid.
                dt = new DataTable();
                Admision = dt.Columns.Add("Admision", typeof(int));
                FechaCita = dt.Columns.Add("FechaCita", typeof(DateTime));
                Paciente = dt.Columns.Add("Paciente", typeof(string));
                FechaGestion = dt.Columns.Add("FechaGestion", typeof(DateTime));
                Usuario = dt.Columns.Add("Usuario", typeof(string));
                Gestion = dt.Columns.Add("Gestion", typeof(string));

                if (_lista != null)
                {
                    // Desactivar temporalmente las notificaciones de cambios.
                    dt.BeginLoadData();

                    try
                    {
                        foreach (CXN_HISTORIAL_ADMISION i in _lista)
                        {
                            DataRow row = dt.NewRow();
                            row[Admision] = i.Admision;
                            row[FechaCita] = i.Horario.Hor_Pac_Fecha_Cita;
                            row[Paciente] = i.Horario.Hor_Imp_Age?.ToString();
                            row[FechaGestion] = i.Fecha;
                            row[Usuario] = i.Usuario;
                            row[Gestion] = i.Gestion;

                            dt.Rows.Add(row);
                        }
                    }
                    finally
                    {
                        dt.EndLoadData();
                    }
                }
                // Crear la vista una vez terminada la carga.

                vistaHistorial = new DataView(dt);
                var grid = gridZH1.dataGridView1;

                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; 
                grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                grid.DataSource = vistaHistorial;

                grid.Columns["Admision"].Width = 90;
                grid.Columns["FechaCita"].Width = 110;
                grid.Columns["Paciente"].Width = 260;
                grid.Columns["FechaGestion"].Width = 160;
                grid.Columns["Usuario"].Width = 120;
                grid.Columns["Gestion"].Width = 310;

                if (grid.Rows.Count > 0 && !gridZH1.CeldaHeight) 
                { 
                    grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells); 
                }

                grid.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pictureBox1.Visible = false;
                boton1.Enabled = true;
            }
        }
        private async void boton1_Click(object sender, EventArgs e)
        {
            await CargarGrrilla();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (vistaHistorial == null)
                    return;

                string texto = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(texto))
                {
                    vistaHistorial.RowFilter = string.Empty;
                }
                else
                {
                    string filtro = texto.Replace("'", "''");

                    vistaHistorial.RowFilter = $"Convert([{Admision}], 'System.String') " +
                                               $"LIKE '%{filtro}%' " +
                                               $"OR " + $"Convert([{Paciente}], 'System.String') " +
                                               $"LIKE '%{filtro}%' " +
                                               $"OR " + $"Convert([{Usuario}], 'System.String') " +
                                               $"LIKE '%{filtro}%' " +
                                               $"OR " + $"Convert([{Gestion}], 'System.String') " +
                                               $"LIKE '%{filtro}%'";

                    if (!gridZH1.CeldaHeight)
                    {
                        gridZH1.dataGridView1.AutoResizeRows(
                            DataGridViewAutoSizeRowsMode.AllCells);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
