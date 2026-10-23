using System;
using System.Collections.Generic;
using System.Linq;
using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Repositories
{
    public class InMemoryMedicoRepository : IMedicoRepository
    {
        private readonly List<Medico> _medicos = new();

        public IQueryable<Medico> Query() => _medicos.AsQueryable();

        public void Adicionar(Medico entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _medicos.Add(entity);
        }

        public Medico? BuscarPorCrm(string crm)
        {
            return _medicos.FirstOrDefault(m => m.CRM == crm);
        }

        public bool ExistePorCrm(string crm)
        {
            return _medicos.Any(m => m.CRM == crm);
        }
    }
}
