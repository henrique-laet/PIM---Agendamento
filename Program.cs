using System;
using Npgsql;

try
{
    using var conexao = ConexaoBanco.ObterConexao();
    Console.WriteLine("✅ Conexão com o banco realizada com sucesso!");

    using var cmd = new NpgsqlCommand("SELECT id_paciente, nome, cpf FROM paciente", conexao);
    using var reader = cmd.ExecuteReader();

    Console.WriteLine("\n📋 Pacientes cadastrados:");
    while (reader.Read())
    {
        Console.WriteLine($"  ID: {reader["id_paciente"]} | Nome: {reader["nome"]} | CPF: {reader["cpf"]}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro na conexão: {ex.Message}");
}