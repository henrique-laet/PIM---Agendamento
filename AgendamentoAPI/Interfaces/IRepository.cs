using System.Linq;

namespace AgendamentoMedico.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        void Adicionar(T entity);
    }
}
