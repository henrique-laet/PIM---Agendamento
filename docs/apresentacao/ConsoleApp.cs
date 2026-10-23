using System;
using AgendamentoMedico.Modelos;
using AgendamentoMedico.Servicos;

namespace AgendamentoMedico.Apresentacao
{
    public class ConsoleApp
    {
        private readonly Agenda _agenda;

        public ConsoleApp()
        {
            _agenda = new Agenda();
        }

        public void ExecutarDemo()
        {
            Console.WriteLine("=== SISTEMA DE AGENDAMENTO MÉDICO ===\n");

            var especialidadeClinico = new Especialidade(
                TipoEspecialidade.Clinico_Geral, duracaoMinutos: 30, valorConsulta: 120m);

            var especialidadeCardio = new Especialidade(
                TipoEspecialidade.Cardiologia, duracaoMinutos: 45, valorConsulta: 250m);

            var medicoCarlos = new Medico(
                nome: "Carlos Eduardo",
                cpf: "12345678901",
                email: "carlos@clinica.com",
                telefone: "11999990001",
                dataNascimento: new DateTime(1975, 3, 15),
                crm: "CRM-SP-12345",
                especialidade: especialidadeClinico
            );

            var medicaAna = new Medico(
                nome: "Ana Paula",
                cpf: "98765432100",
                email: "ana@clinica.com",
                telefone: "11999990002",
                dataNascimento: new DateTime(1980, 7, 22),
                crm: "CRM-SP-54321",
                especialidade: especialidadeCardio
            );

            var pacienteJoao = new Paciente(
                nome: "João Silva",
                cpf: "11122233344",
                email: "joao@email.com",
                telefone: "11888880001",
                dataNascimento: new DateTime(1990, 5, 10),
                convenio: "Unimed",
                numeroCarteirinha: "123456789"
            );

            var pacienteMaria = new Paciente(
                nome: "Maria Souza",
                cpf: "55566677788",
                email: "maria@email.com",
                telefone: "11888880002",
                dataNascimento: new DateTime(1985, 11, 30),
                convenio: "Particular"
            );

            _agenda.CadastrarMedico(medicoCarlos);
            _agenda.CadastrarMedico(medicaAna);
            _agenda.CadastrarPaciente(pacienteJoao);
            _agenda.CadastrarPaciente(pacienteMaria);

            Console.WriteLine("--- Cadastros ---");
            Console.WriteLine(medicoCarlos.ExibirResumo());
            Console.WriteLine(medicaAna.ExibirResumo());
            Console.WriteLine(pacienteJoao.ExibirResumo());
            Console.WriteLine(pacienteMaria.ExibirResumo());

            Console.WriteLine("\n--- Agendamentos ---");

            var amanha = DateTime.Today.AddDays(1).AddHours(9);
            var consulta1 = _agenda.AgendarConsulta(pacienteJoao, medicoCarlos, amanha,
                                                   "Consulta de rotina");
            Console.WriteLine($"✓ {consulta1}");

            var consulta2 = _agenda.AgendarConsulta(pacienteMaria, medicaAna,
                                                   amanha.AddHours(1), "Dor no peito");
            Console.WriteLine($"✓ {consulta2}");

            consulta1.Confirmar();
            Console.WriteLine($"\nConsulta 1 confirmada — Status: {consulta1.Status}");

            Console.WriteLine($"\n--- Horários disponíveis Dr. Carlos ({amanha:dd/MM/yyyy}) ---");
            var horarios = _agenda.ListarHorariosDisponiveis(medicoCarlos, DateTime.Today.AddDays(1));
            horarios.ForEach(h => Console.WriteLine($"  {h:HH:mm}"));

            var novaData = amanha.AddDays(1);
            _agenda.Remarcar(consulta2.Id, novaData);
            Console.WriteLine($"\nConsulta 2 remarcada para: {consulta2.DataHora:dd/MM/yyyy HH:mm} | Status: {consulta2.Status}");

            Console.WriteLine($"\n--- Consultas do Dr. {medicoCarlos.Nome} ---");
            _agenda.ListarConsultasPorMedico(medicoCarlos, DateTime.Today.AddDays(1))
                .ForEach(c => Console.WriteLine(c));

            Console.WriteLine($"\n--- Consultas do paciente {pacienteJoao.Nome} ---");
            _agenda.ListarConsultasPorPaciente(pacienteJoao)
                .ForEach(c => Console.WriteLine(c));

            var stats = _agenda.ObterEstatisticas();
            Console.WriteLine($"\n--- Estatísticas ---");
            Console.WriteLine($"Total: {stats.total} | Ativas: {stats.ativas} | " +
                              $"Canceladas: {stats.canceladas} | Realizadas: {stats.realizadas}");

            Console.WriteLine("\n=== FIM DA DEMONSTRAÇÃO ===");
        }
    }
}
