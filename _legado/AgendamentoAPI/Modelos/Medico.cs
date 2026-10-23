// ============================================================
// CLASSE: Medico — herda de Pessoa
// HERANÇA: reutiliza Nome, CPF, Email, etc. da classe Pessoa.
// POLIMORFISMO: sobrescreve ObterTipo() e ExibirResumo().
// ENCAPSULAMENTO: CRM e Especialidade são protegidos.
// ============================================================
using System;
using System.Collections.Generic;
using AgendamentoMedico.Interfaces;

namespace AgendamentoMedico.Modelos
{
    public class Medico : Pessoa
    {
        // Atributos específicos do Médico
        private string _crm;
        private Especialidade _especialidade;
        private List<TimeSpan> _horariosDisponiveis;

        public Medico(string nome, string cpf, string email, string telefone,
                      DateTime dataNascimento, string crm, Especialidade especialidade)
            : base(nome, cpf, email, telefone, dataNascimento) // chama construtor da classe Pessoa
        {
            if (string.IsNullOrWhiteSpace(crm))
                throw new ArgumentException("CRM não pode ser vazio.");

            _crm = crm;
            _especialidade = especialidade ?? throw new ArgumentNullException(nameof(especialidade));

            // Horários padrão: 08h às 17h, de hora em hora
            _horariosDisponiveis = new List<TimeSpan>();
            for (int hora = 8; hora <= 17; hora++)
                _horariosDisponiveis.Add(new TimeSpan(hora, 0, 0));
        }

        // Propriedades do Médico
        public string CRM => _crm; // Somente leitura

        public Especialidade Especialidade
        {
            get => _especialidade;
            set => _especialidade = value ?? throw new ArgumentNullException(nameof(value));
        }

        // Lista de horários disponíveis (cópia protegida)
        public IReadOnlyList<TimeSpan> HorariosDisponiveis
            => _horariosDisponiveis.AsReadOnly();

        // Adiciona um horário disponível
        public void AdicionarHorario(TimeSpan horario)
        {
            if (!_horariosDisponiveis.Contains(horario))
                _horariosDisponiveis.Add(horario);
        }

        // Remove um horário disponível
        public void RemoverHorario(TimeSpan horario)
        {
            _horariosDisponiveis.Remove(horario);
        }

        // POLIMORFISMO: implementação do método abstrato de Pessoa
        public override string ObterTipo() => "Médico";

        // POLIMORFISMO: sobrescreve o ExibirResumo da classe base
        public override string ExibirResumo()
        {
            return $"[Médico] {Nome} | CRM: {CRM} | " +
                   $"Especialidade: {Especialidade.Nome} | " +
                   $"Valor: R$ {Especialidade.ValorConsulta:F2}";
        }
    }
}