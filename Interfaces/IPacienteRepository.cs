using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Interfaces
{
    public interface IPacienteRepository : IRepository<Paciente>
    {
        Paciente? BuscarPorCpf(string cpf);
        bool ExistePorCpf(string cpf);
    }
}
