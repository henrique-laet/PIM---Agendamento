using Dapper;
using VitaCare.Api.Data;
using VitaCare.Api.Dtos;

namespace VitaCare.Api.Repositories;

public class CatalogoRepository
{
    private readonly DbConnectionFactory _db;

    public CatalogoRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EspecialidadeDto>> ListarEspecialidadesAsync()
    {
        using var conexao = _db.Criar();
        return await conexao.QueryAsync<EspecialidadeDto>(
            "SELECT id, nome, icone FROM especialidades ORDER BY nome");
    }

    public async Task<IEnumerable<MedicoDto>> ListarMedicosAsync(int? especialidadeId)
    {
        var sql = @"SELECT m.id, m.nome, m.crm, m.especialidade_id,
                           e.nome AS especialidade_nome, m.foto_url, m.bio
                    FROM medicos m
                    JOIN especialidades e ON e.id = m.especialidade_id
                    WHERE m.ativo";
        if (especialidadeId.HasValue)
            sql += " AND m.especialidade_id = @especialidadeId";
        sql += " ORDER BY m.nome";

        using var conexao = _db.Criar();
        return await conexao.QueryAsync<MedicoDto>(sql, new { especialidadeId });
    }

    public async Task<MedicoDto?> ObterMedicoAsync(int id)
    {
        using var conexao = _db.Criar();
        return await conexao.QuerySingleOrDefaultAsync<MedicoDto>(
            @"SELECT m.id, m.nome, m.crm, m.especialidade_id,
                     e.nome AS especialidade_nome, m.foto_url, m.bio
              FROM medicos m
              JOIN especialidades e ON e.id = m.especialidade_id
              WHERE m.id = @id AND m.ativo",
            new { id });
    }

    // Só horários livres e no futuro; opcionalmente filtrados por dia
    public async Task<IEnumerable<HorarioDto>> ListarHorariosLivresAsync(int medicoId, DateTime? dia)
    {
        var sql = @"SELECT id, data_hora
                    FROM horarios_disponiveis
                    WHERE medico_id = @medicoId
                      AND disponivel
                      AND data_hora > LOCALTIMESTAMP";
        if (dia.HasValue)
            sql += " AND data_hora::date = CAST(@dia AS date)";
        sql += " ORDER BY data_hora";

        using var conexao = _db.Criar();
        return await conexao.QueryAsync<HorarioDto>(sql, new { medicoId, dia });
    }
}
