using Dapper;
using VitaCare.Api.Data;
using VitaCare.Api.Models;

namespace VitaCare.Api.Repositories;

public class UsuarioRepository
{
    private readonly DbConnectionFactory _db;

    public UsuarioRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        using var conexao = _db.Criar();
        return await conexao.QuerySingleOrDefaultAsync<Usuario>(
            "SELECT * FROM usuarios WHERE LOWER(email) = LOWER(@email)",
            new { email });
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        using var conexao = _db.Criar();
        return await conexao.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM usuarios WHERE LOWER(email) = LOWER(@email))",
            new { email });
    }

    public async Task<bool> CpfExisteAsync(string cpf)
    {
        using var conexao = _db.Criar();
        return await conexao.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM pacientes WHERE cpf = @cpf)",
            new { cpf });
    }

    public async Task<int> CriarPacienteAsync(
        Usuario usuario, string cpf, string? telefone, DateTime? dataNascimento)
    {
        using var conexao = _db.Criar();
        conexao.Open();
        using var transacao = conexao.BeginTransaction();

        var usuarioId = await conexao.ExecuteScalarAsync<int>(
            @"INSERT INTO usuarios (nome, email, senha_hash, perfil)
              VALUES (@Nome, @Email, @SenhaHash, 'paciente')
              RETURNING id",
            usuario, transacao);

        await conexao.ExecuteAsync(
            @"INSERT INTO pacientes (usuario_id, cpf, telefone, data_nascimento)
              VALUES (@usuarioId, @cpf, @telefone, @dataNascimento)",
            new { usuarioId, cpf, telefone, dataNascimento }, transacao);

        transacao.Commit();
        return usuarioId;
    }

    public async Task<bool> ExisteAdminAsync()
    {
        using var conexao = _db.Criar();
        return await conexao.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM usuarios WHERE perfil = 'admin')");
    }

    public async Task CriarAdminAsync(string nome, string email, string senhaHash)
    {
        using var conexao = _db.Criar();
        await conexao.ExecuteAsync(
            @"INSERT INTO usuarios (nome, email, senha_hash, perfil)
              VALUES (@nome, @email, @senhaHash, 'admin')",
            new { nome, email, senhaHash });
    }
}