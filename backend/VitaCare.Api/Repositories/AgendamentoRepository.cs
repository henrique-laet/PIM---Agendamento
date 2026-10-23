using Dapper;
using VitaCare.Api.Data;
using VitaCare.Api.Dtos;

namespace VitaCare.Api.Repositories;

public class AgendamentoRepository
{
    private readonly DbConnectionFactory _db;

    public AgendamentoRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<int?> ObterPacienteIdAsync(int usuarioId)
    {
        using var conexao = _db.Criar();
        return await conexao.ExecuteScalarAsync<int?>(
            "SELECT id FROM pacientes WHERE usuario_id = @usuarioId",
            new { usuarioId });
    }

    public async Task<(bool Ok, string? Erro, int Id)> AgendarAsync(int pacienteId, int horarioId)
    {
        using var conexao = _db.Criar();
        conexao.Open();
        using var transacao = conexao.BeginTransaction();

        // Reserva atômica: só um paciente consegue marcar o horário como ocupado
        var medicoId = await conexao.ExecuteScalarAsync<int?>(
            @"UPDATE horarios_disponiveis
              SET disponivel = FALSE
              WHERE id = @horarioId
                AND disponivel = TRUE
                AND data_hora > LOCALTIMESTAMP
              RETURNING medico_id",
            new { horarioId }, transacao);

        if (medicoId is null)
        {
            transacao.Rollback();
            return (false, "Este horário não está mais disponível.", 0);
        }

        var id = await conexao.ExecuteScalarAsync<int>(
            @"INSERT INTO agendamentos (paciente_id, medico_id, horario_id)
              VALUES (@pacienteId, @medicoId, @horarioId)
              RETURNING id",
            new { pacienteId, medicoId, horarioId }, transacao);

        transacao.Commit();
        return (true, null, id);
    }

    public async Task<IEnumerable<AgendamentoDto>> ListarDoPacienteAsync(int pacienteId)
    {
        using var conexao = _db.Criar();
        return await conexao.QueryAsync<AgendamentoDto>(
            @"SELECT a.id, a.status, a.criado_em, a.horario_id, h.data_hora,
                     a.medico_id, m.nome AS medico_nome, e.nome AS especialidade
              FROM agendamentos a
              JOIN horarios_disponiveis h ON h.id = a.horario_id
              JOIN medicos m ON m.id = a.medico_id
              JOIN especialidades e ON e.id = m.especialidade_id
              WHERE a.paciente_id = @pacienteId
              ORDER BY h.data_hora DESC",
            new { pacienteId });
    }

    // Só cancela se o agendamento for do próprio paciente e ainda estiver ativo
    public async Task<bool> CancelarAsync(int agendamentoId, int pacienteId)
    {
        using var conexao = _db.Criar();
        conexao.Open();
        using var transacao = conexao.BeginTransaction();

        var horarioId = await conexao.ExecuteScalarAsync<int?>(
            @"UPDATE agendamentos
              SET status = 'cancelada'
              WHERE id = @agendamentoId
                AND paciente_id = @pacienteId
                AND status = 'agendada'
              RETURNING horario_id",
            new { agendamentoId, pacienteId }, transacao);

        if (horarioId is null)
        {
            transacao.Rollback();
            return false;
        }

        // Libera o horário para outro paciente
        await conexao.ExecuteAsync(
            "UPDATE horarios_disponiveis SET disponivel = TRUE WHERE id = @horarioId",
            new { horarioId }, transacao);

        transacao.Commit();
        return true;
    }
}