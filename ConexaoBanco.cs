using Npgsql;

public class ConexaoBanco
{
    private const string StringConexao =
        "Host=localhost;" +
        "Port=5432;" +
        "Database=agendamento;" +
        "Username=postgres;" +
        "Password=1020;";

    public static NpgsqlConnection ObterConexao()
    {
        var conexao = new NpgsqlConnection(StringConexao);
        conexao.Open();
        return conexao;
    }
}
