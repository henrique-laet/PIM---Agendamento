using System;
using System.Collections.Generic;
using System.Linq;
using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Repositories
{
    public class InMemoryPacienteRepository : IPacienteRepository
    {
        private readonly List<Paciente> _pacientes = new();

        public IQueryable<Paciente> Query() => _pacientes.AsQueryable();

        public void Adicionar(Paciente entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _pacientes.Add(entity);
        }

        public Paciente? BuscarPorCpf(string cpf)
        {
            return _pacientes.FirstOrDefault(p => p.CPF == cpf);
        }

        public bool ExistePorCpf(string cpf)
        {
            return _pacientes.Any(p => p.CPF == cpf);
        }
    }
}
