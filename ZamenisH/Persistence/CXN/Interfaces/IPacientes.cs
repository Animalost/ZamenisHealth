using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IPacientes
    {
        string getTipoDoc(string Tipo);
        bool CrearClientes(CXN_PACIENTES p);
        bool ValidaCelular(string Val_Cel);
        void Actualiza_Pac2(CXN_PACIENTES P);
        bool ValidaEmail(string Val_Email);
        CXN_PACIENTES LlamarPacientebyId(int pacid);
        void Actualiza(CXN_PACIENTES Paciente);
        List<CXN_PACIENTES> LlamarPacienteDOCSimilares(string PApellido, string PNombre);
        string Carga_Regimen(string Cod_Reg);
        CXN_PACIENTES LlamarPacienteDOC(string TipoId, string NId);
        List<string> ListaRegimen();
        CXN_PACIENTES LlamarPacienteNumDoc(string NId);
        string Regimen(string Val_Reg);
        bool Crea_Paciente(CXN_PACIENTES P);
        bool Existente(string Doc, int pacid);
        bool Edita_Paciente(CXN_PACIENTES p);
        bool ActualizarPaciente(CXN_PACIENTES pacientes);
        bool ActualizarCelular(string Celular, int Admision);
        void Actualiza_Email(int Paciente, string Email);
        void Actualiza_Pac(CXN_PACIENTES P);
        void Actualiza_Pac3(CXN_PACIENTES P);
        void Rpt_Atenciones2(DateTime Desde, DateTime Hasta);
        List<string> ListaDocs();
        void VariasHeridas(int IdPac, string Heridas);
        void VariosDias(int IdPac, string Heridas);
        bool UpdatePacFibro(int Paciente, string Estado);
        void PacEspecial(int IdPac, string Especial);
        //bool ComprobarEdad(DateTime FechaSeleccionada, string TipoId);
        void RptCancelaciones(DateTime Desde, DateTime Hasta);
        void UpdateFac2(int IdPac, string Contrato, string Regimen);
        List<string> getListPaises();
        string getCodePais(string Pais);
        string getNamePais(string Code);
        void updateEmail(int IdPac, string Email);
        void SexAndDate(int IdPac, string Sex, DateTime Date);
        void setEnfermedades(int Paciente, string vih, string hepatitis);
        List<CXN_GENDERIDENTITY> ListaIdentidadGenero();
        string NameIdentidadGenero(string Code);
        string CodeIdentidadGenero(string Name);
    }
}
