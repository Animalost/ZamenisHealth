using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Licence
{
    public partial class RegistroZH : ConfigForm.BaseForm
    {
        public RegistroZH()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RegistroZH_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Configuracion de sistema";
            ImageClose.Visible = false;
        }
        List<string> ListaTablas()
        {
            List<string> L = new List<string>();            
            L.Add("CXN_ADHERENCIA");
            L.Add("CXN_ASEGURADORA");
            L.Add("CXN_BODEGAS");
            L.Add("CXN_CARGOS");
            L.Add("CXN_CATEGORIA");
            L.Add("CXN_CIA");
            L.Add("CXN_CIE10");
            L.Add("CXN_CMAN");
            L.Add("CXN_COD_CAR");
            L.Add("CXN_CONDICIONES");
            L.Add("CXN_CONFENCUESTA");
            L.Add("CXN_CONTADORPAGES");
            L.Add("CXN_CONTROL");
            L.Add("CXN_CONVENIOS");            
            L.Add("CXN_DIAS_WEB");
            L.Add("CXN_DISPONIBILIDAD_2");
            L.Add("CXN_DOCUMENTOS");
            L.Add("CXN_EMAIL");
            L.Add("CXN_EMAIL2");
            L.Add("CXN_ENCUESTASATIS");
            L.Add("CXN_ESTADISTICAS");
            L.Add("CXN_EVOFIB");
            L.Add("CXN_EXCLUIDOS");
            L.Add("CXN_FACTURA");
            L.Add("CXN_FECHAS");
            L.Add("CXN_FORMATOS");
            L.Add("CXN_FORMATOS_2");
            L.Add("CXN_HCFI");
            L.Add("CXN_HCJUNTAS");
            L.Add("CXN_HCMED");
            L.Add("CXN_HCMG");
            L.Add("CXN_HCPSI");
            L.Add("CXN_HCTF");
            L.Add("CXN_HCTO");
            L.Add("CXN_HORARIO");
            L.Add("CXN_IMAGEN_SYSTEM");
            L.Add("CXN_IMAGENES");
            L.Add("CXN_INVENTARIO");
            L.Add("CXN_INVPPALCARGOS");
            L.Add("CXN_INVPPALLISTA");
            L.Add("CXN_LOG_SENDER");
            L.Add("CXN_LOGIN");
            L.Add("CXN_MENUHGMG");
            L.Add("CXN_MESSENGER");
            L.Add("CXN_NOTAS");
            L.Add("CXN_OM");
            L.Add("CXN_OPEND");
            L.Add("CXN_PACIENTES");
            L.Add("CXN_PAGOS");
            L.Add("CXN_PAIS");
            L.Add("CXN_PEDIDOS");
            L.Add("CXN_PEDIDOSF");
            L.Add("CXN_PLANTILLA");
            L.Add("CXN_PROVEEDORES");
            L.Add("CXN_RC_CAJA");
            L.Add("CXN_REGIMEN");
            L.Add("CXN_RESTOREPASS");
            L.Add("CXN_ROLES");
            L.Add("CXN_TECNOSALUD");
            L.Add("CXN_TECNOSALUDMOTATENCION");
            L.Add("CXN_VENTAS");
            L.Add("CXN_ZONAS");
            L.Add("FIB_ENCUESTA1");
            L.Add("FIB_ENCUESTA2");
            L.Add("FIB_ENCUESTA3");
            L.Add("HC_Alergias");
            L.Add("Inventario");
            L.Add("Inventario_Proveedores");
            L.Add("WS_Clients");
            L.Add("CXN_MEDIOSPAGO");
            return L;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea verificar el estructuramiento de las tablas de la aplicacion?",
                                                      "Zamenis Health - SQL Server - Tables",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    List<object> L = new List<object>();
                    string nombreTabla = "";
                    string connectionString = Conexion.getCadenaSQLServer(textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);

                    foreach (string tabla in ListaTablas())
                    {
                        nombreTabla = ObtenerNombreTabla(tabla);

                        if (VerificarTablaExistente(connectionString, nombreTabla) == true)
                        {
                            L.Add(Domain.TablesSQL.CrearInstancia(nombreTabla));
                        }
                        else
                        {
                            string definicionTabla = @"CREATE TABLE " + nombreTabla + @" (Id INT)";
                            CrearTabla(connectionString, definicionTabla);
                            L.Add(Domain.TablesSQL.CrearInstancia(nombreTabla));
                        }
                    }

                    foreach (var t in L)
                    {
                        var tipo = t.GetType();
                        string tableName = ObtenerNombreTabla(tipo.Name);
                        List<string> columnNames = Persistence.PTablesSQL.GetColumnNames(tipo);
                        PropertyInfo[] properties = tipo.GetProperties();                        

                        foreach (var columnName in columnNames)
                        {
                            if (Persistence.PTablesSQL.ConsultarTablaExistente(connectionString, tableName) == true) //verificar si existe la tabla en bd
                            {
                                if (Persistence.PTablesSQL.ConsultarBase(connectionString, tableName, columnName.ToString()) == 0) //verificar si existe el campo en bd
                                {
                                    /*string alterQuery = $"ALTER TABLE {tableName} ADD {columnName} TuTipoDeDato";
                                    SqlCommand alterCommand = new SqlCommand(alterQuery, connection);
                                    alterCommand.ExecuteNonQuery();*/
                                    TXTException T = new TXTException
                                    {
                                        Error = "El campo " + columnName + " de la tabla " + tableName + " no existe"
                                    };

                                    OverridesExtern.GenerarTXT(T);
                                }
                            }
                            else                             
                            {                                
                                TXTException T = new TXTException
                                {
                                    Error = "La Tabla " + tableName + " no existe"
                                };
                                OverridesExtern.GenerarTXT(T);
                            }
                        }
                    }

                    MessageBox.Show("Hecho");
                }
                else
                {
                    MessageBox.Show("Debe escoger una base de datos");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region //validadores de tablas

        static void CrearTabla(string connectionString, string definicionTabla)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(definicionTabla, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static string ObtenerNombreTabla(string nombreClase)
        {            
            return nombreClase;
        }

        // Método para verificar si una tabla existe en la base de datos
        static bool VerificarTablaExistente(string connectionString, string nombreTabla)
        {            
            string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{nombreTabla}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Conexion.Conection()["Conexion"] = Conexion.getCadenaSQLServer(textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
