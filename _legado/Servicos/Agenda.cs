// ============================================================
// CLASSE: Agenda
// Classe central do sistema — gerencia todos os agendamentos.
// Aplica regras de negócio: conflito de horário, disponibilidade.
// POLIMORFISMO: trabalha com IAgendavel para flexibilidade.
// ============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Servicos
{
    public class Agenda
    {
        // Armazena todas as consultas do sistema
        private readonly List<Consulta> _consultas;

        // Armazena médicos e pacientes cadastrados
        private readonly List<Medico> _medicos;
        private readonly List<Paciente> _pacientes;

        public Agenda()
        {
            _consultas = new List<Consulta>();
            _medicos = new List<Medico>();
            _pacientes = new List<Paciente>();
        }

        // ============================================================
        // CADASTRO DE MÉDICOS E PACIENTES
        // ============================================================

        public void CadastrarMedico(Medico medico)
        {
            if (medico is null)
                throw new ArgumentNullException(nameof(medico));

            if (_medicos.Any(m => m.CRM == medico.CRM))
                throw new InvalidOperationException($"Médico com CRM {medico.CRM} já cadastrado.");

            _medicos.Add(medico);
        }

        public void CadastrarPaciente(Paciente paciente)
        {
            if (paciente is null)
                throw new ArgumentNullException(nameof(paciente));

            if (_pacientes.Any(p => p.CPF == paciente.CPF))
                throw new InvalidOperationException($"Paciente com CPF {paciente.CPF} já cadastrado.");

            _pacientes.Add(paciente);
        }

        // ============================================================
        // AGENDAMENTO DE CONSULTAS
        // ============================================================

        public Consulta AgendarConsulta(Paciente paciente, Medico medico,
                                        DateTime dataHora, string observacoes = "")
        {
            if (paciente is null)
                throw new ArgumentNullException(nameof(paciente));
            if (medico is null)
                throw new ArgumentNullException(nameof(medico));
            if (dataHora <= DateTime.Now)
                throw new ArgumentException("Data/hora da consulta deve ser futura.", nameof(dataHora));
            if (!_medicos.Any(m => m.CRM == medico.CRM))
                throw new InvalidOperationException("Médico não cadastrado na agenda.");
            if (!_pacientes.Any(p => p.CPF == paciente.CPF))
                throw new InvalidOperationException("Paciente não cadastrado na agenda.");

            int duracaoMinutos = medico.Especialidade.DuracaoConsultaMinutos;

            if (!MedicoEstaDisponivel(medico, dataHora))
                throw new InvalidOperationException(
                    $"Médico {medico.Nome} não está disponível em {dataHora:dd/MM/yyyy HH:mm}.");

            if (!PacienteEstaDisponivel(paciente, dataHora, duracaoMinutos))
                throw new InvalidOperationException(
                    $"Paciente {paciente.Nome} já possui consulta neste horário.");

            var consulta = new Consulta(paciente, medico, dataHora, observacoes);
            _consultas.Add(consulta);

            return consulta;
        }

        public Consulta Remarcar(int consultaId, DateTime novaDataHora)
        {
            if (novaDataHora <= DateTime.Now)
                throw new ArgumentException("Nova data/hora deve ser futura.", nameof(novaDataHora));

            var consulta = BuscarConsultaPorId(consultaId)
                ?? throw new InvalidOperationException("Consulta não encontrada.");

            if (!consulta.EstaAtivo())
                throw new InvalidOperationException("Somente consultas ativas podem ser remarcadas.");

            var medico = consulta.Medico;
            var paciente = consulta.Paciente;
            int duracaoMinutos = medico.Especialidade.DuracaoConsultaMinutos;

            if (!MedicoEstaDisponivel(medico, novaDataHora, consultaId))
                throw new InvalidOperationException(
                    $"Médico {medico.Nome} não está disponível em {novaDataHora:dd/MM/yyyy HH:mm}.");

            if (!PacienteEstaDisponivel(paciente, novaDataHora, duracaoMinutos, consultaId))
                throw new InvalidOperationException(
                    $"Paciente {paciente.Nome} já possui outra consulta neste horário.");

            if (!consulta.Remarcar(novaDataHora))
                throw new InvalidOperationException("Não foi possível remarcar a consulta.");

            return consulta;
        }

        // ============================================================
        // VERIFICAÇÕES DE DISPONIBILIDADE
        // ============================================================

        private bool MedicoEstaDisponivel(Medico medico, DateTime dataHora, int? excluirConsultaId = null)
        {
            if (medico is null)
                throw new ArgumentNullException(nameof(medico));

            if (dataHora <= DateTime.Now)
                return false;

            int duracaoMinutos = medico.Especialidade.DuracaoConsultaMinutos;

            bool horarioValido = medico.HorariosDisponiveis
                .Any(h => h == dataHora.TimeOfDay);

            if (!horarioValido)
                return false;

            return !_consultas
                .Where(c => c.Medico.CRM == medico.CRM && c.EstaAtivo())
                .Where(c => !excluirConsultaId.HasValue || c.Id != excluirConsultaId.Value)
                .Any(c =>
                {
                    DateTime inicio = c.DataHora;
                    DateTime fim = inicio.AddMinutes(duracaoMinutos);
                    DateTime novoInicio = dataHora;
                    DateTime novoFim = novoInicio.AddMinutes(duracaoMinutos);
                    return novoInicio < fim && novoFim > inicio;
                });
        }

        private bool PacienteEstaDisponivel(Paciente paciente, DateTime dataHora, int duracaoMinutos, int? excluirConsultaId = null)
        {
            if (paciente is null)
                throw new ArgumentNullException(nameof(paciente));

            if (dataHora <= DateTime.Now)
                return false;

            return !_consultas
                .Where(c => c.Paciente.CPF == paciente.CPF && c.EstaAtivo())
                .Where(c => !excluirConsultaId.HasValue || c.Id != excluirConsultaId.Value)
                .Any(c =>
                {
                    DateTime inicio = c.DataHora;
                    DateTime fim = inicio.AddMinutes(duracaoMinutos);
                    DateTime novoInicio = dataHora;
                    DateTime novoFim = novoInicio.AddMinutes(duracaoMinutos);
                    return novoInicio < fim && novoFim > inicio;
                });
        }

        // ============================================================
        // CONSULTAS E LISTAGENS
        // ============================================================

        // Lista todas as consultas de um médico em uma data
        public List<Consulta> ListarConsultasPorMedico(Medico medico, DateTime? data = null)
        {
            var query = _consultas.Where(c => c.Medico.CRM == medico.CRM);
            if (data.HasValue)
                query = query.Where(c => c.DataHora.Date == data.Value.Date);
            return query.OrderBy(c => c.DataHora).ToList();
        }

        // Lista todas as consultas de um paciente
        public List<Consulta> ListarConsultasPorPaciente(Paciente paciente)
        {
            return _consultas
                .Where(c => c.Paciente.CPF == paciente.CPF)
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        // Lista horários disponíveis de um médico em uma data
        public List<DateTime> ListarHorariosDisponiveis(Medico medico, DateTime data)
        {
            if (medico is null)
                throw new ArgumentNullException(nameof(medico));

            return medico.HorariosDisponiveis
                .Select(horario => data.Date.Add(horario))
                .Where(dataHora => dataHora > DateTime.Now && MedicoEstaDisponivel(medico, dataHora))
                .OrderBy(dataHora => dataHora)
                .ToList();
        }

        // Lista todos os médicos por especialidade
        public List<Medico> ListarMedicosPorEspecialidade(TipoEspecialidade tipo)
        {
            return _medicos
                .Where(m => m.Especialidade.Tipo == tipo)
                .ToList();
        }

        // Busca consulta por ID
        public Consulta BuscarConsultaPorId(int id)
        {
            return _consultas.FirstOrDefault(c => c.Id == id);
        }

        // Retorna totais para relatório simples
        public (int total, int ativas, int canceladas, int realizadas) ObterEstatisticas()
        {
            return (
                _consultas.Count,
                _consultas.Count(c => c.EstaAtivo()),
                _consultas.Count(c => c.Status == StatusConsulta.Cancelada),
                _consultas.Count(c => c.Status == StatusConsulta.Realizada)
            );
        }
    }
}