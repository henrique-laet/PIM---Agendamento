using System.Data;
using Npgsql;

namespace VitaCare.Api.Data;

// Centraliza a criação de conexões com o PostgreSQL.
// Todos os repositórios vão pedir a conexão por aqui.
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Vitacare")
            ?? throw new InvalidOperationException(
                "A connection string 'Vitacare' não foi configurada.");
    }

    public IDbConnection Criar() => new NpgsqlConnection(_connectionString);
}