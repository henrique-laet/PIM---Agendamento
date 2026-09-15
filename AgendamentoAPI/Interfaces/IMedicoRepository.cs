using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Interfaces
{
    public interface IMedicoRepository : IRepository<Medico>
    {
        Medico? BuscarPorCrm(string crm);
        bool ExistePorCrm(string crm);
    }
}
