using System;

namespace AgendamentoMedico.Modelos
{
    public abstract class Pessoa
    {
        private string _nome;
        private string _cpf;
        private string _email;
        private string _telefone;
        private DateTime _dataNascimento;

        protected Pessoa(string nome, string cpf, string email,
                         string telefone, DateTime dataNascimento)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                throw new ArgumentException("CPF inválido.");

            _nome = nome;
            _cpf = cpf;
            _email = email;
            _telefone = telefone;
            _dataNascimento = dataNascimento;
        }

        public string Nome
        {
            get => _nome;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nome não pode ser vazio.");

                _nome = value;
            }
        }

        public string CPF => _cpf;

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public string Telefone
        {
            get => _telefone;
            set => _telefone = value;
        }

        public DateTime DataNascimento => _dataNascimento;

        public int Idade
        {
            get
            {
                int idade = DateTime.Today.Year - _dataNascimento.Year;
                if (_dataNascimento.Date > DateTime.Today.AddYears(-idade))
                    idade--;

                return idade;
            }
        }

        public abstract string ObterTipo();

        public virtual string ExibirResumo()
        {
            return $"[{ObterTipo()}] {Nome} | CPF: {CPF} | Idade: {Idade} anos";
        }

        public override string ToString() => ExibirResumo();
    }
}
