using Domain;
using Persistence;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Clases
{
    public static class JsonVerifications
    {
        public static string JsonLimpio(string cadenaJsonOriginal)
        {
            try
            {
                System.Text.Json.Nodes.JsonNode root = JsonNode.Parse(cadenaJsonOriginal);
                var usuarios = root?["usuarios"]?.AsArray();
                if (usuarios != null)
                {
                    foreach (var usuario in usuarios)
                    {
                        var servicios = usuario?["servicios"] as JsonObject;

                        if (servicios != null)
                        {
                            if (servicios != null)
                            {
                                if (servicios["consultas"] is JsonArray consultas && consultas.Count == 0)
                                    servicios.Remove("consultas");

                                if (servicios["procedimientos"] is JsonArray procedimientos && procedimientos.Count == 0)
                                    servicios.Remove("procedimientos");

                                if (servicios["otrosServicios"] is JsonArray otros && otros.Count == 0)
                                    servicios.Remove("otrosServicios");

                                if (servicios.Count == 0)
                                    usuario.AsObject().Remove("servicios");
                            }
                        }
                    }
                }

                return root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "JsonVerifications", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return cadenaJsonOriginal;
            }
        }        
    }
}
