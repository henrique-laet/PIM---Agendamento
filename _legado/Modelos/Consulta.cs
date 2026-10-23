// ============================================================
// CLASSE: Consulta — implementa IAgendavel
// POLIMORFISMO: implementa a interface IAgendavel.
// ASSOCIAÇÃO: relaciona Paciente e Médico.
// ENCAPSULAMENTO: estado da consulta controlado internamente.
// ============================================================

using System;
using System.Collections.Generic;
using AgendamentoMedico.Interfaces;
using AgendamentoMedico.Modelos;

namespace AgendamentoMedico.Modelos
{
    // Enum para os possíveis estados de uma consulta
    public enum StatusConsulta
    {
        Agendada,
        Confirmada,
        Cancelada,
        Realizada,
        Remarcada
    }

    public class Consulta : IAgendavel
    {
        // Atributos privados — encapsulamento total do estado
        private static int _contadorId = 1; // gerador simples de ID
        private int _id;
        private Paciente _paciente;
        private Medico _medico;
        private DateTime _dataHora;
        private StatusConsulta _status;
        private string _observacoes;
        private DateTime _dataCriacao;

        public Consulta(Paciente paciente, Medico medico, DateTime dataHora,
                        string observacoes = "")
        {
            _paciente = paciente ?? throw new ArgumentNullException(nameof(paciente));
            _medico = medico ?? throw new ArgumentNullException(nameof(medico));

            // Valida que a consulta não pode ser no passado
            if (dataHora <= DateTime.Now)
                throw new ArgumentException("Data/hora da consulta deve ser futura.");

            _id = _contadorId++;
            _dataHora = dataHora;
            _status = StatusConsulta.Agendada; // estado inicial sempre Agendada
            _observacoes = observacoes;
            _dataCriacao = DateTime.Now;
        }

        // Propriedades — acesso controlado
        public int Id => _id;
        public Paciente Paciente => _paciente;
        public Medico Medico => _medico;
        public StatusConsulta Status => _status;
        public DateTime DataCriacao => _dataCriacao;
        public string Observacoes
        {
            get => _observacoes;
            set => _observacoes = value ?? "";
        }

        // IMPLEMENTAÇÃO DA INTERFACE IAgendavel
        public DateTime DataHora
        {
            get => _dataHora;
            set
            {
                if (value <= DateTime.Now)
                    throw new ArgumentException("Data/hora deve ser futura.");
                _dataHora = value;
            }
        }

        // Confirma a consulta (só pode confirmar se estiver Agendada)
        public bool Confirmar()
        {
            if (_status == StatusConsulta.Agendada)
            {
                _status = StatusConsulta.Confirmada;
                return true;
            }
            return false; // não foi possível confirmar
        }

        // Cancela a consulta (não pode cancelar se já foi Realizada)
        public bool Cancelar()
        {
            if (_status == StatusConsulta.Realizada || _status == StatusConsulta.Cancelada)
                return false;

            _status = StatusConsulta.Cancelada;
            return true;
        }

        // Verifica se está ativa (agendada ou confirmada)
        public bool EstaAtivo()
        {
            return _status == StatusConsulta.Agendada ||
                   _status == StatusConsulta.Confirmada;
        }

        // Marca a consulta como realizada
        public bool MarcarComoRealizada()
        {
            if (_status == StatusConsulta.Confirmada || _status == StatusConsulta.Agendada)
            {
                _status = StatusConsulta.Realizada;
                // Adiciona ao histórico do paciente automaticamente
                _paciente.AdicionarRegistroHistorico(
                    $"Consulta realizada com Dr(a). {_medico.Nome} ({_medico.Especialidade.Nome})");
                return true;
            }
            return false;
        }

        // Remarca a consulta para um novo horário
        public bool Remarcar(DateTime novaDataHora)
        {
            if (!EstaAtivo()) return false;

            if (novaDataHora <= DateTime.Now)
                throw new ArgumentException("Nova data/hora deve ser futura.");

            _dataHora = novaDataHora;
            _status = StatusConsulta.Remarcada;
            return true;
        }

        public override string ToString()
        {
            return $"Consulta #{Id} | {DataHora:dd/MM/yyyy HH:mm} | " +
                   $"Paciente: {Paciente.Nome} | Médico: {Medico.Nome} | " +
                   $"Status: {Status}";
        }
    }
}