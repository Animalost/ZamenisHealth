using Domain.CXN;

namespace EmbededBussiness.Interfaz
{
    public interface INotasCuraciones
    {
        otrosDatosPacienteHorario cargarAdmision(int Admision, string filter);
        CXN_CONVENIOS ServicioNombre(string CUP, int Ase, string Tipo);
        CXN_ASEGURADORA getInfoFromAsebyCode(int Code);
        string BuscaDX(string CodDX);
        CXN_PACIENTES LlamarPacientebyId(int pacid);
        Dictionary<string, string> getListado();
        bool ConsultarNavyEnfermeria(int Paciente, int Bodega, DateTime Fecha);
        string Calcular3(int Paciente, string TipoServicio);
        Dictionary<string, string> SugerenciaServicio(int Paciente);
        Dictionary<string, string> Diagnosticos(int Paciente);
        void Graba_Hora_Atencion(int Atention);
        void OpenAdmition(int Admision, string Estado);
        List<string> getCondiciones(int Pac);
        bool getCantEncuestaCU(string Service, string Mes, int Año);
        bool getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C);
        bool getCantCitas(int Paciente, CXN_CONFENCUESTA C);
        List<CXN_NOTASMED> LoadHeridas(int Admision);
    }
}
