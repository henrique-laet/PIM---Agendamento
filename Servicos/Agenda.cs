// ============================================================
// CLASSE: Agenda
// Classe central do sistema — gerencia todos os agendamentos.
// Aplica regras de negócio: conflito de horário, disponibilidade.
// POLIMORFISMO: trabalha com IAgendavel para flexibilidade.
// ============================================================

using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Servicos
{
    public class Agenda
    {
        // Armazena todas as consultas do sistema
        private List<Consulta> _consultas;

        // Armazena médicos e pacientes cadastrados
        private List<Medico> _medicos;
        private List<Paciente> _pacientes;

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
            // Verifica duplicidade por CRM
            if (_medicos.Any(m => m.CRM == medico.CRM))
                throw new InvalidOperationException($"Médico com CRM {medico.CRM} já cadastrado.");

            _medicos.Add(medico);
        }

        public void CadastrarPaciente(Paciente paciente)
        {
            // Verifica duplicidade por CPF
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
            // Regra 1: Verifica se o médico está disponível nesse horário
            if (!MedicoEstaDisponivel(medico, dataHora))
                throw new InvalidOperationException(
                    $"Médico {medico.Nome} não está disponível em {dataHora:dd/MM/yyyy HH:mm}.");

            // Regra 2: Verifica se o paciente já tem consulta no mesmo horário
            if (!PacienteEstaDisponivel(paciente, dataHora, medico.Especialidade.DuracaoConsultaMinutos))
                throw new InvalidOperationException(
                    $"Paciente {paciente.Nome} já possui consulta neste horário.");

            // Cria e registra a consulta
            var consulta = new Consulta(paciente, medico, dataHora, observacoes);
            _consultas.Add(consulta);

            return consulta;
        }

        // ============================================================
        // VERIFICAÇÕES DE DISPONIBILIDADE
        // ============================================================

        private bool MedicoEstaDisponivel(Medico medico, DateTime dataHora)
        {
            int duracaoMinutos = medico.Especialidade.DuracaoConsultaMinutos;

            // Verifica se o horário está dentro dos horários cadastrados do médico
            bool horarioValido = medico.HorariosDisponiveis
                .Any(h => h == dataHora.TimeOfDay);

            if (!horarioValido) return false;

            // Verifica conflito com consultas já agendadas do médico
            bool temConflito = _consultas
                .Where(c => c.Medico.CRM == medico.CRM && c.EstaAtivo())
                .Any(c =>
                {
                    // Calcula intervalo de tempo da consulta existente
                    DateTime inicio = c.DataHora;
                    DateTime fim = inicio.AddMinutes(duracaoMinutos);
                    DateTime novoInicio = dataHora;
                    DateTime novoFim = novoInicio.AddMinutes(duracaoMinutos);

                    // Há conflito se os intervalos se sobrepõem
                    return novoInicio < fim && novoFim > inicio;
                });

            return !temConflito;
        }

        private bool PacienteEstaDisponivel(Paciente paciente, DateTime dataHora, int duracaoMinutos)
        {
            return !_consultas
                .Where(c => c.Paciente.CPF == paciente.CPF && c.EstaAtivo())
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
            var horariosLivres = new List<DateTime>();

            foreach (var horario in medico.HorariosDisponiveis)
            {
                var dataHora = data.Date.Add(horario);
                if (dataHora > DateTime.Now && MedicoEstaDisponivel(medico, dataHora))
                    horariosLivres.Add(dataHora);
            }

            return horariosLivres;
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