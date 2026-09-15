using System;
using Npgsql;

try
{
    using var conexao = ConexaoBanco.ObterConexao();
    Console.WriteLine("✅ Conexão com o banco realizada com sucesso!");

<<<<<<< HEAD
    using var cmd = new NpgsqlCommand("SELECT id, nome, cpf FROM paciente", conexao);
=======
    using var cmd = new NpgsqlCommand("SELECT id_paciente, nome, cpf FROM paciente", conexao);
>>>>>>> d2b1ad1847493744caf701b0d7186ef5db54e1b1
    using var reader = cmd.ExecuteReader();

    Console.WriteLine("\n📋 Pacientes cadastrados:");
    while (reader.Read())
    {
<<<<<<< HEAD
        Console.WriteLine($"  ID: {reader["id"]} | Nome: {reader["nome"]} | CPF: {reader["cpf"]}");
=======
        Console.WriteLine($"  ID: {reader["id_paciente"]} | Nome: {reader["nome"]} | CPF: {reader["cpf"]}");
>>>>>>> d2b1ad1847493744caf701b0d7186ef5db54e1b1
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro na conexão: {ex.Message}");
}