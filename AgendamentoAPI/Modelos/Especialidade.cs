// ============================================================
// CLASSE: Especialidade
// Representa a especialidade médica.
// ENCAPSULAMENTO: dados protegidos e validados internamente.
// ============================================================
using System;
using System.Collections.Generic;

namespace AgendamentoMedico.Modelos
{
    // Enum para definir as especialidades disponíveis no sistema
    public enum TipoEspecialidade
    {
        Clinico_Geral,
        Cardiologia,
        Dermatologia,
        Ortopedia,
        Pediatria,
        Ginecologia,
        Neurologia,
        Oftalmologia,
        Psiquiatria,
        Endocrinologia
    }

    public class Especialidade
    {
        // Atributos privados
        private TipoEspecialidade _tipo;
        private int _duracaoConsultaMinutos;
        private decimal _valorConsulta;

        public Especialidade(TipoEspecialidade tipo,
                             int duracaoMinutos = 30,
                             decimal valorConsulta = 150.00m)
        {
            // Validação da duração: entre 15 e 120 minutos
            if (duracaoMinutos < 15 || duracaoMinutos > 120)
                throw new ArgumentException("Duração deve ser entre 15 e 120 minutos.");

            if (valorConsulta < 0)
                throw new ArgumentException("Valor não pode ser negativo.");

            _tipo = tipo;
            _duracaoConsultaMinutos = duracaoMinutos;
            _valorConsulta = valorConsulta;
        }

        // Propriedades somente leitura — tipo não deve mudar após criação
        public TipoEspecialidade Tipo => _tipo;

        public string Nome => _tipo.ToString().Replace("_", " ");

        public int DuracaoConsultaMinutos
        {
            get => _duracaoConsultaMinutos;
            set
            {
                if (value < 15 || value > 120)
                    throw new ArgumentException("Duração inválida.");
                _duracaoConsultaMinutos = value;
            }
        }

        public decimal ValorConsulta
        {
            get => _valorConsulta;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Valor não pode ser negativo.");
                _valorConsulta = value;
            }
        }

        public override string ToString()
            => $"{Nome} | Duração: {DuracaoConsultaMinutos} min | Valor: R$ {ValorConsulta:F2}";
    }
}