using System;

namespace AgendamentoMedico.Interfaces
{
    public interface IAgendavel
    {
        DateTime DataHora { get; set; }

        bool Confirmar();

        bool Cancelar();

        bool EstaAtivo();
    }
}
