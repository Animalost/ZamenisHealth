using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class PacientesService
    {
        public async Task<List<CXN_PACIENTES>> ObtenerListaPacientes(string Nombres, string Apellidos) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Nombre = Nombres,
                    Apellido = Apellidos
                };

                return APIController.SendMessageToAPI<List<CXN_PACIENTES>>(datos, "/api/pacientes/ListaPacientesNombres", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> ActualizarCelular(int admision, string celular) //HRCHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Celular = celular
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/pacientes/ActualizarCelular", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<CXN_PACIENTES> LlamarPacienteDOC(string tipoid, string numid) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TipoId = tipoid,
                    NumId = numid
                };

                return APIController.SendMessageToAPI<CXN_PACIENTES>(datos, "/api/Agendamiento/LlamarPacienteDOC", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_PACIENTES> LlamarPacienteOnlyDOC(string numid) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NumId = numid
                };

                return APIController.SendMessageToAPI<CXN_PACIENTES>(datos, "/api/Agendamiento/LlamarPacienteOnlyDOC", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Carga_Regimen(string coderegimen) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodRegimen = coderegimen
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Agendamiento/Carga_Regimen", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> Actualiza_Pac(CXN_PACIENTES P) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new
            {
                Pac_Telefono = P.Pac_Telefono,
                Pac_TelefonoAux = P.Pac_TelefonoAux,
                Pac_PrimerN = P.Pac_PrimerN,
                Pac_SegundoN = P.Pac_SegundoN,
                Pac_PrimerA = P.Pac_PrimerA,
                Pac_SegundoA = P.Pac_SegundoA,
                Pac_Email = P.Pac_Email,
                Pac_Regimen = P.Pac_Regimen,
                Pac_Sexo = P.Pac_Sexo,
                Pac_Dep_Cod = P.Pac_Dep_Cod,
                Pac_Mun_Cod = P.Pac_Mun_Cod,
                Pac_Aseguradora = Convert.ToInt32(P.Pac_Aseguradora.ToString()),
                Pac_FechaNto = Convert.ToDateTime(P.Pac_FechaNto.ToString()),
                Pac_Id = Convert.ToInt32(P.Pac_Id.ToString()),
                Pac_Zona = P.Pac_Zona,
                Pac_TipoId = P.Pac_TipoId,
                Pac_Contrato = P.Pac_Contrato,
                Pac_PaisOrigen = P.Pac_PaisOrigen,
                Pac_Residencia = P.Pac_Residencia,
                Pac_ECivil = P.Pac_ECivil,
                Pac_Acudiente = P.Pac_Acudiente,
                Pac_Parentesco = P.Pac_Parentesco,
                Pac_DireccionAcu = P.Pac_DireccionAcu,
                Pac_TelefonoAcu = P.Pac_TelefonoAcu,
                Pac_CorreoAcu = P.Pac_CorreoAcu,
                Pac_Categoria = P.Pac_Categoria
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/Admisiones/Actualiza_Pac", true).GetAwaiter().GetResult();

        }

        public async Task<bool> Crea_Paciente(CXN_PACIENTES P) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new
            {
                Pac_TipoId = P.Pac_TipoId,
                Pac_IdNum = P.Pac_IdNum.TrimStart().TrimEnd(),
                Pac_PrimerN = P.Pac_PrimerN,
                Pac_PrimerA = P.Pac_PrimerA,
                Pac_SegundoN = P.Pac_SegundoN,
                Pac_SegundoA = P.Pac_SegundoA,
                Pac_FechaNto = P.Pac_FechaNto,
                Pac_Sexo = P.Pac_Sexo,
                Pac_Telefono = P.Pac_Telefono,
                Pac_TelefonoAux = P.Pac_TelefonoAux,
                Pac_Direccion = P.Pac_Direccion,
                Pac_Email = P.Pac_Email,
                Pac_Mun_Cod = P.Pac_Mun_Cod,
                Pac_Dep_Cod = P.Pac_Dep_Cod,
                Pac_Zona = P.Pac_Zona,
                Pac_Localidad = P.Pac_Localidad,
                Pac_Aseguradora = P.Pac_Aseguradora,
                Pac_Acudiente = P.Pac_Acudiente,
                Pac_Parentesco = P.Pac_Parentesco,
                Pac_DireccionAcu = P.Pac_DireccionAcu,
                Pac_TelefonoAcu = P.Pac_TelefonoAcu,
                Pac_CorreoAcu = P.Pac_CorreoAcu,
                Pac_UsrGraba = P.Pac_UsrGraba,
                Pac_Regimen = P.Pac_Regimen,
                Pac_Contrato = P.Pac_Contrato,
                Pac_Categoria = P.Pac_Categoria
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/pacientes/Crea_Paciente", true).GetAwaiter().GetResult();

        }

        public async Task<bool> Existente(string document, int idpac) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Document = document,
                    IdPac = idpac
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/pacientes/Existente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> Edita_Paciente(CXN_PACIENTES P) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new
            {
                Pac_TipoId = P.Pac_TipoId,
                Pac_IdNum = P.Pac_IdNum.TrimStart().TrimEnd(),
                Pac_PrimerN = P.Pac_PrimerN,
                Pac_PrimerA = P.Pac_PrimerA,
                Pac_SegundoN = P.Pac_SegundoN,
                Pac_SegundoA = P.Pac_SegundoA,
                Pac_FechaNto = P.Pac_FechaNto,
                Pac_Sexo = P.Pac_Sexo,
                Pac_Telefono = P.Pac_Telefono,
                Pac_TelefonoAux = P.Pac_TelefonoAux,
                Pac_Direccion = P.Pac_Direccion,
                Pac_Email = P.Pac_Email,
                Pac_Mun_Cod = P.Pac_Mun_Cod,
                Pac_Dep_Cod = P.Pac_Dep_Cod,
                Pac_Zona = P.Pac_Zona,
                Pac_Localidad = P.Pac_Localidad,
                Pac_Aseguradora = P.Pac_Aseguradora,
                Pac_Acudiente = P.Pac_Acudiente,
                Pac_Parentesco = P.Pac_Parentesco,
                Pac_DireccionAcu = P.Pac_DireccionAcu,
                Pac_TelefonoAcu = P.Pac_TelefonoAcu,
                Pac_CorreoAcu = P.Pac_CorreoAcu,
                Pac_UsrGraba = P.Pac_UsrGraba,
                Pac_Regimen = P.Pac_Regimen,
                Pac_Contrato = P.Pac_Contrato,
                Pac_Categoria = P.Pac_Categoria,
                Pac_Id = P.Pac_Id
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/pacientes/Edita_Paciente", true).GetAwaiter().GetResult();

        }

        public async Task<bool> Actualiza_Email(int pac, string email) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new
            {
                IdPac = pac,
                Document = email           
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/CitasSender/Actualiza_Email", true).GetAwaiter().GetResult();

        }

        public async Task<bool> CrearClientes(CXN_PACIENTES P) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new
            {
                Pac_PrimerN = P.Pac_PrimerN,
                Pac_SegundoN = P.Pac_SegundoN,
                Pac_PrimerA = P.Pac_PrimerA,
                Pac_SegundoA = P.Pac_SegundoA,
                Pac_Telefono = P.Pac_Telefono,
                Pac_Email = P.Pac_Email,
                Pac_Aseguradora = P.Pac_Aseguradora,
                Pac_TipoId = P.Pac_TipoId,
                Pac_IdNum = P.Pac_IdNum,
                Pac_Categoria = P.Pac_Categoria
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/pacientes/CrearClientes", true).GetAwaiter().GetResult();

        }

        public async Task<CXN_PACIENTES> LlamarPacientebyId(int pacid) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdPac = pacid,
                    Document = ""
                };

                return APIController.SendMessageToAPI<CXN_PACIENTES>(datos, "/api/DatosCitas/LlamarPacientebyId", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> getValor(string categoria) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodRegimen = categoria
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/pacientes/getValor", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
    }
}
