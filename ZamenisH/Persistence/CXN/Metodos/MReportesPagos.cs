using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Persistence.CXN.Metodos
{
    public class MReportesPagos : IReportesPagos
    {
        private IAseguradoras _aseguradoras;
        private ICompañia _compañia;

        public MReportesPagos() 
        {
            _aseguradoras = new MAseguradoras();
            _compañia = new MCompañia();
        }

        async Task<List<CXN_PAGOSASEGURADORAS>> IReportesPagos.ReportePagos(CXN_PAGOSASEGURADORAS parametros)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_PAGOSASEGURADORAS P " +
                                         "INNER JOIN CXN_ASEGURADORA A ON P.Aseguradora = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON P.Prestador = C.Com_Identificador " +
                                         "WHERE FechaPago BETWEEN @param1 AND @param2 " +                                         
                                         "AND P.Prestador = @param3 " +
                                         "AND P.Aseguradora = @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(parametros.Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(parametros.Hasta.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", parametros.Prestador);
                        Carga_Command.Parameters.AddWithValue("@param4", parametros.Aseguradora);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            List<CXN_PAGOSASEGURADORAS> L = new List<CXN_PAGOSASEGURADORAS>();

                            if (Lectura_Hora.HasRows)
                            {
                                while (await Lectura_Hora.ReadAsync() == true)
                                {
                                    L.Add(new CXN_PAGOSASEGURADORAS
                                    {
                                        Aseguradora = Convert.ToInt32(Lectura_Hora["Aseguradora"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Valor = Convert.ToInt32(Lectura_Hora["Valor"]),
                                        Prestador = Convert.ToInt32(Lectura_Hora["Prestador"]),
                                        FechaPago = Convert.ToDateTime(Lectura_Hora["FechaPago"]),
                                        Extracto = Lectura_Hora["Extracto"].ToString(),
                                        Concepto = Lectura_Hora["Concepto"].ToString(),
                                        Factura = Lectura_Hora["Factura"].ToString(),
                                        AseguradoraName = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        PrestadorName =  Lectura_Hora["Com_Nombre"].ToString()
                                    });
                                }

                                return L;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                        
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        List<CXN_ASEGURADORA> IReportesPagos.getAseguradoras()
        {
            return _aseguradoras.getAseguradoras();
        }
        List<CXN_CIA> IReportesPagos.getAllCompañias()
        {
            return _compañia.getAllCompañias();
        }
        CXN_CIA IReportesPagos.getPrestadorbyName(string Name)
        {
            return _compañia.getPrestadorbyName(Name);
        }
        CXN_ASEGURADORA IReportesPagos.getInfoFromAsebyName(string Name)
        {
            return _aseguradoras.getInfoFromAsebyName(Name);
        }
    }
}
