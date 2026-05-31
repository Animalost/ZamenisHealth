namespace EmbededBussiness
{
    public class Conection
    {
        public static Dictionary<string, string> ConectCore()
        {
            //return Conexion.Conection();
            Dictionary<string, string> D = new Dictionary<string, string>
            {
                {"Conexion", "Data Source=slsoft.net,14330;Initial Catalog=CXN;User ID=sa;Password=Sharon*55284;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;" }
            };

            return D;
        }
    }
}
