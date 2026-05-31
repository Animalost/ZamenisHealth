using Persistence;
using System;
using System.Data;
using System.Data.SqlClient;

namespace APIFhir.Servicio
{
    public interface IConsultaPDF
    {
        string SearchURLPDF(string IdComposition);
    }

    public class MConsultaPDF : IConsultaPDF
    {
        string IConsultaPDF.SearchURLPDF(string IdComposition)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Query = "SELECT URLPdf " +
                                   "FROM CXN_RDA " +
                                   "WHERE idCompositionRecorded = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", IdComposition);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["URLPdf"].ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }
    }
}
