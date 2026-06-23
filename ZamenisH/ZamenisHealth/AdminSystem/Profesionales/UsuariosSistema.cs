using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;

namespace ZamenisHealth.AdminSystem.Profesionales
{
    public partial class UsuariosSistema : Forma2
    {
        private ILogin oLogin;

        private DataTable dt;

        public UsuariosSistema()
        {
            InitializeComponent();
            oLogin = new MLogin();
        }

        private void UsuariosSistema_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Usuarios del Sistema";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
            gridZH1.CeldaHeight = true;

            CargarFuncionarios();
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string userSelected = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                Bodegas B = new Bodegas(userSelected);
                B.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            DataColumn Funcionario = dt.Columns.Add("Funcionario", typeof(string));
            DataColumn Usuaario = dt.Columns.Add("Usuario", typeof(string));
        }
        void CargarFuncionarios()
        {
            try
            {
                var users = oLogin.getUsersforSendMessage();
                if (users != null) 
                {
                    Encabezados();

                    foreach (var i in users)
                    {
                        if (i.Log_Habilitado == "A")
                        {
                            DataRow row = dt.NewRow();

                            row["Funcionario"] = i.Log_PrimerA + " " + i.Log_SegundoA + " " + i.Log_PrimerN + " " + i.Log_SegundoN;
                            row["Usuario"] = i.Log_Usuario.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }                        
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
