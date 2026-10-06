using Domain.CONSUMOS;
using Domain.CXN;

using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace ZamenisHealth.Consumos
{
    public partial class VerMovimientosCompleto : Forma2
    {
        private IMovimientosConsumo oMovimientosConsumo;
        private ICompañia oCompañia;

        DataTable dt;

        DataColumn NombreProducto;
        DataColumn CodigoExterno;
        DataColumn TipoDoc;
        DataColumn Documento;
        DataColumn Fecha;
        DataColumn CodigoArticulo;
        DataColumn BodegaOrigen;
        DataColumn BodegaDestino;
        DataColumn Concepto;
        DataColumn IdentidadTercero;
        DataColumn Cantidad;
        DataColumn CostoUnitario;


        public VerMovimientosCompleto()
        {
            InitializeComponent();
            oMovimientosConsumo = new MMovimientosConsumo();
            oCompañia = new MCompañia();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();

            NombreProducto = dt.Columns.Add("NOMBRE", typeof(string));
            CodigoExterno = dt.Columns.Add("REFERENCIA", typeof(string));
            TipoDoc = dt.Columns.Add("TIPO DOC", typeof(string));
            Documento = dt.Columns.Add("DOCUMENTO", typeof(string));
            Fecha = dt.Columns.Add("FECHA", typeof(string));
            CodigoArticulo = dt.Columns.Add("CODIGO_ARTICULO", typeof(string));
            BodegaOrigen = dt.Columns.Add("BODEGA_ORIGEN", typeof(string));
            BodegaDestino = dt.Columns.Add("BODEGA_DESTINO", typeof(string));
            Concepto = dt.Columns.Add("CONCEPTO", typeof(string));
            IdentidadTercero = dt.Columns.Add("IDENTIDAD_TERCERO", typeof(string));
            Cantidad = dt.Columns.Add("CANTIDAD", typeof(string));
            CostoUnitario = dt.Columns.Add("COSTO_UNITARIO", typeof(string));
        }
        private void VerMovimientosCompleto_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Movimientos Completos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarGrilla(false);
        }
        void CargarGrilla(bool SaveCons)
        {
            try
            {
                List<CON_CONSUMOS> lista = oMovimientosConsumo.GetConsumos(dateTimePicker1.Value.Date);
                if (lista != null)
                {
                    lista = lista.Where(x => x.Con_Cons_Status == true).ToList();

                    var listaAgrupada = lista
                                                 .GroupBy(x => new
                                                 {
                                                     Fecha = x.Con_Cons_Fecha.Date,
                                                     Consultorio = x.Con_Cons_Idconsultorio,
                                                     IdProducto = x.Con_Cons_IdProducto,
                                                     CodigoExterno = x.Con_Prod_Cod_Externo
                                                 })
                                                 .Select(g => new CON_CONSUMOS
                                                 {
                                                     // Fecha
                                                     Con_Cons_Fecha = g.Key.Fecha,

                                                     // Consultorio
                                                     Con_Cons_Idconsultorio = g.Key.Consultorio,

                                                     // Producto
                                                     Con_Cons_IdProducto = g.Key.IdProducto,
                                                     Con_Prod_Cod_Externo = g.Key.CodigoExterno,
                                                     Con_Prod_Name = g.First().Con_Prod_Name,

                                                     // Cantidad acumulada
                                                     Con_Cons_Cantidad = g.Sum(x => x.Con_Cons_Cantidad)
                                                 })
                                                 .OrderBy(x => x.Con_Cons_Fecha)
                                                 .ThenBy(x => x.Con_Cons_Idconsultorio)
                                                 .ThenBy(x => x.Con_Prod_Cod_Externo)
                                                 .ToList();

                    CXN_CIA getDataCia = oCompañia.getPrestadorbyCode(10);
                    int consecutivo = getDataCia.Com_ConsContable;

                    foreach (var grupoFecha in listaAgrupada
                                     .GroupBy(x => x.Con_Cons_Fecha.Date))
                                                    {
                                                        foreach (var grupoConsultorio in grupoFecha
                                                            .GroupBy(x => x.Con_Cons_Idconsultorio)
                                                            .OrderBy(x => x.Key))
                                                        {
                                                            BigInteger codigo = BigInteger.Parse(
                                                                $"{grupoFecha.Key:yyyyMMdd}{consecutivo}"
                                                            );

                                                            foreach (var item in grupoConsultorio)
                                                            {
                                                                item.UniqueCode = codigo;
                                                            }

                                                            consecutivo++;
                                                        }
                                                    }
                    if (SaveCons == true)
                    {
                        oCompañia.ConsecutivoActualiza(10, "CONTABLE", consecutivo);
                    }                    

                    Encabezados();

                    foreach (var i in listaAgrupada)
                    {
                        DataRow row = dt.NewRow();

                        row[NombreProducto] = i.Con_Prod_Name.ToString();
                        row[CodigoExterno] = i.Con_Prod_Cod_Externo.ToString();
                        row[TipoDoc] = "SM";
                        row[Documento] = i.UniqueCode; 
                        row[Fecha] = i.Con_Cons_Fecha.ToString("yyyy-MM-dd");
                        row[CodigoArticulo] = i.Con_Prod_Cod_Externo.ToString();
                        row[BodegaOrigen] = i.Con_Cons_Idconsultorio.ToString();
                        row[BodegaDestino] = "N/A";
                        row[Concepto] = "Salida Inventario";
                        row[IdentidadTercero] = "N/A";
                        row[Cantidad] = i.Con_Cons_Cantidad.ToString();
                        row[CostoUnitario] = "0";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
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
                MessageBox.Show(ex.Message);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                CargarGrilla(true);

                string Texto = "NOMBRE|REFERENCIA|TIPO DOC|DOCUMENTO|FECHA|CODIGO_ARTICULO|BODEGA_ORIGEN|BODEGA_DESTINO|CONCEPTO|IDENTIDAD_TERCERO|CANTIDAD|COSTO_UNITARIO \r";

                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    Texto = Texto + row.Cells["NOMBRE"].Value.ToString() + "|" +
                                    row.Cells["REFERENCIA"].Value.ToString() + "|" +
                                    row.Cells["TIPO DOC"].Value.ToString() + "|" +
                                    row.Cells["DOCUMENTO"].Value.ToString() + "|" +
                                    row.Cells["FECHA"].Value.ToString() + "|" +
                                    row.Cells["CODIGO_ARTICULO"].Value.ToString() + "|" +
                                    row.Cells["BODEGA_ORIGEN"].Value.ToString() + "|" +
                                    row.Cells["BODEGA_DESTINO"].Value.ToString() + "|" +
                                    row.Cells["CONCEPTO"].Value.ToString() + "|" +
                                    row.Cells["IDENTIDAD_TERCERO"].Value.ToString() + "|" +
                                    row.Cells["CANTIDAD"].Value.ToString() + "|" +
                                    row.Cells["COSTO_UNITARIO"].Value.ToString() + "\r";
                }

                string NameFile = "C:/Cxn/Reportes/Plantilla_Inventario_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".xls";
                FileStream QueryTxt = new FileStream(NameFile, FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(QueryTxt);
                Escriba.Write(Texto);
                Escriba.Close();

                if (File.Exists(NameFile))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = NameFile,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("El archivo no existe.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarGrilla(false);
        }
    }
}
