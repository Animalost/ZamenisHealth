namespace ZamenisHealth.Clases
{
    public static class PacRules
    {
        public static bool ValidarRegimen(string Regimen, string Categoria)
        {
            if (Regimen == "No afiliado")
            {
                if (Categoria == "Categoria A")
                {
                    return false;
                }
                else if (Categoria == "Categoria B")
                {
                    return false;
                }
                else if (Categoria == "Categoria C")
                {
                    return false;
                }
                else if (Categoria == "Categoria Z")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (Categoria == "No Aplica")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
