// ============================================================
// PROGRAM.CS — Demonstração do sistema
// Mostra como todas as classes interagem entre si.
// Ideal para apresentação no projeto acadêmico.
// ============================================================

using AgendamentoMedico.Modelos;
using AgendamentoMedico.Servicos;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== SISTEMA DE AGENDAMENTO MÉDICO ===\n");

        // 1. Criar a Agenda (ponto central do sistema)
        var agenda = new Agenda();

        // 2. Criar Especialidades
        var especialidadeClinico = new Especialidade(
            TipoEspecialidade.Clinico_Geral, duracaoMinutos: 30, valorConsulta: 120m);

        var especialidadeCardio = new Especialidade(
            TipoEspecialidade.Cardiologia, duracaoMinutos: 45, valorConsulta: 250m);

        // 3. Criar Médicos (herança de Pessoa)
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

        // 4. Criar Pacientes (herança de Pessoa)
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

        // 5. Cadastrar na Agenda
        agenda.CadastrarMedico(medicoCarlos);
        agenda.CadastrarMedico(medicaAna);
        agenda.CadastrarPaciente(pacienteJoao);
        agenda.CadastrarPaciente(pacienteMaria);

        // 6. POLIMORFISMO — ExibirResumo() age diferente para Médico e Paciente
        Console.WriteLine("--- Cadastros ---");
        Console.WriteLine(medicoCarlos.ExibirResumo());
        Console.WriteLine(medicaAna.ExibirResumo());
        Console.WriteLine(pacienteJoao.ExibirResumo());
        Console.WriteLine(pacienteMaria.ExibirResumo());

        // 7. Agendar Consultas
        Console.WriteLine("\n--- Agendamentos ---");

        var amanha = DateTime.Today.AddDays(1).AddHours(9);
        var consulta1 = agenda.AgendarConsulta(pacienteJoao, medicoCarlos, amanha,
                                               "Consulta de rotina");
        Console.WriteLine($"✓ {consulta1}");

        var consulta2 = agenda.AgendarConsulta(pacienteMaria, medicaAna,
                                               amanha.AddHours(1), "Dor no peito");
        Console.WriteLine($"✓ {consulta2}");

        // 8. Confirmar consulta
        consulta1.Confirmar();
        Console.WriteLine($"\nConsulta 1 confirmada — Status: {consulta1.Status}");

        // 9. Listar horários disponíveis
        Console.WriteLine($"\n--- Horários disponíveis Dr. Carlos ({amanha:dd/MM/yyyy}) ---");
        var horarios = agenda.ListarHorariosDisponiveis(medicoCarlos, DateTime.Today.AddDays(1));
        horarios.ForEach(h => Console.WriteLine($"  {h:HH:mm}"));

        // 10. Remarcar consulta
        var novaData = amanha.AddDays(1);
        consulta2.Remarcar(novaData);
        Console.WriteLine($"\nConsulta 2 remarcada para: {novaData:dd/MM/yyyy HH:mm}");

        // 11. Estatísticas
        var stats = agenda.ObterEstatisticas();
        Console.WriteLine($"\n--- Estatísticas ---");
        Console.WriteLine($"Total: {stats.total} | Ativas: {stats.ativas} | " +
                          $"Canceladas: {stats.canceladas} | Realizadas: {stats.realizadas}");

        Console.WriteLine("\n=== FIM DA DEMONSTRAÇÃO ===");
    }
}
