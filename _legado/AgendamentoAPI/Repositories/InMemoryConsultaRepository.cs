using System;
using System.Collections.Generic;
using System.Linq;
using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Repositories
{
    public class InMemoryConsultaRepository : IConsultaRepository
    {
        private readonly List<Consulta> _consultas = new();

        public IQueryable<Consulta> Query() => _consultas.AsQueryable();

        public void Adicionar(Consulta entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _consultas.Add(entity);
        }

        public Consulta? BuscarPorId(int id)
        {
            return _consultas.FirstOrDefault(c => c.Id == id);
        }
    }
}
