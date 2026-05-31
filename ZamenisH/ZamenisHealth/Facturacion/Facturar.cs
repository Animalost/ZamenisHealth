using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar : Forma
    {
        private static readonly IPacientes repoPacs = new MPacientes();
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();

        int Cia, Ase, Pac_Id;
        string Pos_Selected, Adm_Selected;

        DataTable dt;
        DataColumn POSCargo;
        DataColumn TipoCargo;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn Vr_Unitario;
        DataColumn Vr_Total;
        DataColumn Paciente;
        DataColumn Documento;
        DataColumn Pac_Id2;

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Code = repoAse.getInfoFromAsebyName(comboBox2.Text);
            Ase = Code.Ase_Identificador;
            label11.Text = Ase.ToString();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Code = repoCia.getPrestadorbyName(comboBox3.Text);
            Cia = Code.Com_Identificador;
            label12.Text = Cia.ToString();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "Facturacion";
            buscarPacientes.ShowDialog();
        }

        public Facturar()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Metodo_Busqueda();
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
                if (checkBox1.Checked == true)
                {
                    if (label8.Text == "")
                    {
                        MessageBox.Show("No hay servicios para facturar o ya fueron facturados",
                            "Revisar Datos",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if (Convert.ToDateTime(dateTimePicker2.Value.Date) > DateTime.Now.Date)
                {
                    MessageBox.Show("No es posible facturar servicios de fechas superiores a la actual.  Revise la fecha Hasta",
                            "Revisar Datos",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    return;
                }

                if (checkBox1.Checked == false)
                {
                    if (label26.Visible == true)
                    {
                        MessageBox.Show("No hay servicios para facturar o ya fueron facturados",
                            "Revisar Datos",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                Facturar2 Facturacion_2 = new Facturar2((comboBox8.SelectedIndex == 0 ? true : false));
                Facturacion_2.Ase = Ase;
                Facturacion_2.Cia = Cia;
                Facturacion_2.Pac_Id = Pac_Id;
                if (panel1.Visible == true)
                {
                    Facturacion_2.FacIndividual = true;
                    Facturacion_2.Desde = dateTimePicker1.Value;
                    Facturacion_2.Hasta = dateTimePicker2.Value;
                    Facturacion_2.AFacturar = comboBox4.Text;
                }
                if (panel1.Visible == false)
                {
                    Facturacion_2.FacIndividual = false;
                    Facturacion_2.Desde = dateTimePicker4.Value;
                    Facturacion_2.Hasta = dateTimePicker3.Value;
                    Facturacion_2.AFacturar = comboBox7.Text;
                }

                Facturacion_2.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            Medicina.Cargos C = new Medicina.Cargos(true);
            C.StartPosition = FormStartPosition.CenterScreen;
            C.AutoScroll = false;
            C.ShowDialog();
        }
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            Facturar6Otros facturar6Otros = new Facturar6Otros(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, Ase, Pac_Id);
            facturar6Otros.ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                panel1.Visible = true;
                panel2.Visible = false;
            }
            if (checkBox1.Checked == false)
            {
                panel1.Visible = false;
                panel2.Visible = true;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
            MessageBox.Show("El color verde indica que el servicio esta marcado como Procedimiento, " +
               "Quirurgicos y derivados",
               "Ayuda",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("El color rojo indica que el servicio esta marcado como Consultas, " +
              "Diagnosticos y derivados",
              "Ayuda",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Ases = repoAse.getInfoFromAsebyName(comboBox6.Text);
            Ase = Ases.Ase_Identificador;
            label19.Text = Ases.Ase_Identificador.ToString();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Cias = repoCia.getPrestadorbyName(comboBox5.Text);
            Cia = Cias.Com_Identificador;
            label18.Text = Cias.Com_Identificador.ToString();
        }

        private void cambiarAServicioDeProcedimientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                repoFacturacion.changeTypeCargo(Convert.ToInt32(Pos_Selected), "Nota");
                Metodo_Busqueda();
                MessageBox.Show("Actualizado con Exito", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void excluirEsteCargoDeEstaFacturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                repoFacturacion.excluirCargoFactura(Convert.ToInt32(Pos_Selected));
                Metodo_Busqueda();
                MessageBox.Show("Actualizado con Exito", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void cambiarValorYCantidadDeEsteCargoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Facturar5Val f = new Facturar5Val(Convert.ToInt32(Adm_Selected), Convert.ToInt32(Pos_Selected));
                f.ShowDialog();
                Metodo_Busqueda();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void holaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                repoFacturacion.changeTypeCargo(Convert.ToInt32(Pos_Selected), "Historia");
                Metodo_Busqueda();
                MessageBox.Show("Actualizado con Exito", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Metodo_Busqueda()
        {
            try
            {
                String Query = "";

                if (checkBox1.Checked == true)
                {
                    switch (comboBox4.Text)
                    {
                        case "Todo":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                   "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Curaciones y Consultas":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                   "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('CU','MG') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Terapias":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                   "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('TF','TO','PS') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Fisiatria":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                    "FROM CXN_CARGOS C " +
                                    "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                    "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                    "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                    "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                    "AND C.Car_Estado = 'G' " +
                                    "AND C.Car_Cia = '" + Cia + "' " +
                                    "AND C.Car_Ase = '" + Ase + "' " +
                                    "AND C.Car_Tipo_Serv = 'FI' " +
                                    "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Terapias y Fisiatria":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                   "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('TF','TO','PS','FI') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Radiologia":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker2.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND P.Pac_TipoId = '" + comboBox1.Text + "' " +
                                   "AND P.Pac_IdNum = '" + textBox1.Text + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('RA') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        default:
                            MessageBox.Show("Seleccione un tipo de datos a facturar",
                                "Falta informacion",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                    }
                    FiltroFacturas(Query);
                }
                if (checkBox1.Checked == false)
                {
                    switch (comboBox7.Text)
                    {
                        case "Todo":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Curaciones y Consultas":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('CU','MG') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Terapias":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('TF','TO','PS') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Fisiatria":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                    "FROM CXN_CARGOS C " +
                                    "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                    "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                    "AND C.Car_Estado = 'G' " +
                                    "AND C.Car_Cia = '" + Cia + "' " +
                                    "AND C.Car_Ase = '" + Ase + "' " +
                                    "AND C.Car_Tipo_Serv = 'FI' " +
                                    "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Terapias y Fisiatria":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('TF','TO','PS','FI') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        case "Radiologia":
                            Query = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS PAC, " +
                                   "C.Car_Id, C.Car_Adm_Id, C.Car_Tipo, C.Car_Fecha, C.Car_Cod, C.Car_Item, C.Car_Cant, C.Car_Val_Un, C.Car_Val_Tot, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, P.Pac_Id " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(dateTimePicker4.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(dateTimePicker3.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND C.Car_Estado = 'G' " +
                                   "AND C.Car_Cia = '" + Cia + "' " +
                                   "AND C.Car_Ase = '" + Ase + "' " +
                                   "AND C.Car_Tipo_Serv IN ('RA') " +
                                   "ORDER BY C.Car_Fecha, C.Car_Cant ASC";
                            break;

                        default:
                            MessageBox.Show("Seleccione un tipo de datos a facturar",
                                "Falta informacion",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                    }
                    FiltroFacturas(Query);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Facturar_Load(object sender, EventArgs e)
        {
            try
            {
                var Cias = repoCia.getAllCompañias();
                if (Cias != null)
                {
                    foreach (var i in Cias)
                    {
                        comboBox3.Items.Add(i.Com_Nombre);
                    }

                    foreach (var i2 in Cias)
                    {
                        comboBox5.Items.Add(i2.Com_Nombre);
                    }
                }

                comboBox8.SelectedIndex = 0;

                var Ases = repoAse.getAseguradoras();
                if (Ases != null)
                {
                    foreach (var i3 in Ases)
                    {
                        comboBox2.Items.Add(i3.Ase_Descripcion);
                    }

                    foreach (var i4 in Ases)
                    {
                        comboBox6.Items.Add(i4.Ase_Descripcion);
                    }
                }

                Titulo.Text = "PreFacturacion";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnConsultar = new ToolStripButton();
                btnConsultar = createToolButton("Consultar Factura");
                MenuLateral.Items.Add(btnConsultar);
                btnConsultar.Click += btnZamenis1_ButtonClick;

                ToolStripButton btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Crear Prefactura");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += btnZamenis2_ButtonClick;

                ToolStripButton btnCargos = new ToolStripButton();
                btnCargos = createToolButton("Cargos");
                MenuLateral.Items.Add(btnCargos);
                btnCargos.Click += btnZamenis3_ButtonClick;

                ToolStripButton btnModify = new ToolStripButton();
                btnModify = createToolButton("Modificar");
                MenuLateral.Items.Add(btnModify);
                btnModify.Click += btnZamenis4_ButtonClick;

                CargarDocumentos();
                Encabezados();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void mODIFICAROTROSDATOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Facturar6Otros facturar6Otros = new Facturar6Otros(Adm_Selected.ToString());
            facturar6Otros.ShowDialog();
        }

        private void CargarDocumentos()
        {
            var ListaDocs = repoPacs.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Visible = true;
                    contextMenuStrip1.Location = new Point(dataGridView1.Location.X + 600, dataGridView1.Location.Y + 100);
                    Pos_Selected = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    Adm_Selected = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        public void Encabezados() 
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POSCargo = dt.Columns.Add("POSCargo", typeof(int));
            TipoCargo = dt.Columns.Add("TipoCargo", typeof(string));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            Vr_Unitario = dt.Columns.Add("Vr_Unitario", typeof(string));
            Vr_Total = dt.Columns.Add("Vr_Total", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Documento = dt.Columns.Add("Documento", typeof(string));
            Pac_Id2 = dt.Columns.Add("Pac_Id2", typeof(int));
        }

        private void FiltroFacturas(string Query)
        {
            try
            {
                List<CXN_CARGOS> get_carFac = repoFacturacion.getCargosForInvoice(Query);
                if (get_carFac != null)
                {
                    Encabezados();

                    foreach (CXN_CARGOS i in get_carFac)
                    {
                        DataRow row = dt.NewRow();

                        row["POSCargo"] = i.Car_Id.ToString();
                        row["TipoCargo"] = i.Car_Tipo.ToString();
                        row["Admision"] = i.Car_Adm_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Car_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Codigo"] = i.Car_Cod.ToString();
                        row["Item"] = i.Car_Item.ToString();
                        row["Cantidad"] = i.Car_Cant.ToString();
                        row["Vr_Unitario"] = Convert.ToInt32(i.Car_Val_Un).ToString("N0");
                        row["Vr_Total"] = Convert.ToInt32(i.Car_Val_Tot).ToString("N0");
                        row["Paciente"] = i.Car_Detalle.ToString();
                        row["Documento"] = i.Car_Estado.ToString();
                        row["Pac_Id2"] = i.Car_Pac.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        if (checkBox1.Checked == true)
                        {
                            label8.Text = i.Car_Detalle.ToString();
                            Pac_Id = Convert.ToInt32(i.Car_Pac);
                        }

                        if (checkBox1.Checked == false)
                        {
                            label26.Visible = false;
                        }
                    }

                    Estilos(dataGridView1, dt);                  
                }
                else
                {
                    if (checkBox1.Checked == true)
                    {
                        label8.Text = "";
                        Pac_Id = 0;
                    }

                    if (checkBox1.Checked == false)
                    {
                        label26.Visible = true;
                    }

                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;
            D.RowHeadersVisible = false;
            D.Font = new Font("Arial", 9, FontStyle.Bold);

            D.DataSource = t;

            D.Columns["Admision"].Width = 80;
            D.Columns["Fecha"].Width = 80;
            D.Columns["Codigo"].Width = 80;
            D.Columns["Item"].Width = 350;
            D.Columns["Cantidad"].Width = 80;
            D.Columns["Vr_Unitario"].Width = 80;
            D.Columns["Vr_Total"].Width = 80;
            D.Columns["Paciente"].Width = 200;
            D.Columns["Documento"].Width = 120;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Vr_Unitario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Vr_Total"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Documento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Item"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Cantidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Vr_Unitario"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Vr_Total"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Documento"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["PosCargo"].Visible = false;
            D.Columns["TipoCargo"].Visible = false;
            D.Columns["Pac_Id2"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                string Texto = row.Cells["TipoCargo"].Value.ToString();

                if (Texto == "Historia")
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                    row.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else if (Texto == "Nota")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                    row.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            dataGridView1.ClearSelection();
        }
    }
}
