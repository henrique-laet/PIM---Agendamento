// ============================================================
// CLASSE: Paciente — herda de Pessoa
// HERANÇA: reutiliza todos os atributos e métodos de Pessoa.
// POLIMORFISMO: sobrescreve ObterTipo().
// ENCAPSULAMENTO: histórico e convênio protegidos.
// ============================================================
using System;
using System.Collections.Generic;
namespace AgendamentoMedico.Modelos
{
    public class Paciente : Pessoa
    {
        // Atributos específicos do Paciente
        private string _numeroCarteirinha;
        private string _convenio;
        private List<string> _historicoMedico; // registros simples de histórico

        public Paciente(string nome, string cpf, string email, string telefone,
                        DateTime dataNascimento, string convenio = "Particular",
                        string numeroCarteirinha = "")
            : base(nome, cpf, email, telefone, dataNascimento)
        {
            _convenio = convenio;
            _numeroCarteirinha = numeroCarteirinha;
            _historicoMedico = new List<string>();
        }

        // Propriedades específicas do Paciente
        public string Convenio
        {
            get => _convenio;
            set => _convenio = value ?? "Particular";
        }

        public string NumeroCarteirinha
        {
            get => _numeroCarteirinha;
            set => _numeroCarteirinha = value ?? "";
        }

        // Histórico somente leitura — protegido de modificações externas diretas
        public IReadOnlyList<string> HistoricoMedico
            => _historicoMedico.AsReadOnly();

        // Adiciona um registro ao histórico (único ponto de entrada)
        public void AdicionarRegistroHistorico(string registro)
        {
            if (!string.IsNullOrWhiteSpace(registro))
                _historicoMedico.Add($"[{DateTime.Now:dd/MM/yyyy}] {registro}");
        }

        // POLIMORFISMO: implementação do método abstrato de Pessoa
        public override string ObterTipo() => "Paciente";

        // POLIMORFISMO: sobrescreve o ExibirResumo da classe base
        public override string ExibirResumo()
        {
            return $"[Paciente] {Nome} | CPF: {CPF} | " +
                   $"Convênio: {Convenio} | " +
                   $"Carteirinha: {(string.IsNullOrEmpty(NumeroCarteirinha) ? "N/A" : NumeroCarteirinha)}";
        }
    }
}