using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Interfaces
{
    public interface IConsultaRepository : IRepository<Consulta>
    {
        Consulta? BuscarPorId(int id);
    }
}
