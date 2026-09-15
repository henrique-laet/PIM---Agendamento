using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AgendamentoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var consultas = new List<object>();

            using var conn = ConexaoBanco.ObterConexao();
            using var cmd = new NpgsqlCommand(
                @"SELECT c.id_consulta, c.data_hora, c.status, c.observacoes,
                         p.nome AS paciente, pr.nome AS profissional
                  FROM consulta c
                  JOIN paciente p ON p.id_paciente = c.id_paciente
                  JOIN profissional pr ON pr.id_profissional = c.id_profissional
                  ORDER BY c.data_hora DESC", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                consultas.Add(new {
                    id = reader["id_consulta"],
                    dataHora = reader["data_hora"],
                    status = reader["status"],
                    observacoes = reader["observacoes"] == DBNull.Value ? null : reader["observacoes"],
                    paciente = reader["paciente"],
                    profissional = reader["profissional"]
                });
            }

            return Ok(consultas);
        }

        [HttpPost]
public IActionResult Create([FromBody] ConsultaDto dto)
{
    // Verifica o que está chegando
    Console.WriteLine($"IdPaciente: {dto.IdPaciente}, IdProfissional: {dto.IdProfissional}, DataHora: {dto.DataHora}");
    
    using var conn = ConexaoBanco.ObterConexao();
    using var cmd = new NpgsqlCommand(
        @"INSERT INTO consulta (id_paciente, id_profissional, data_hora, duracao_min, tipo, status, observacoes)
          VALUES (@paciente, @profissional, @dataHora, 30, 'presencial', 'agendada', @obs)", conn);

    cmd.Parameters.AddWithValue("paciente", dto.IdPaciente);
    cmd.Parameters.AddWithValue("profissional", dto.IdProfissional);
    cmd.Parameters.AddWithValue("dataHora", dto.DataHora);
    cmd.Parameters.AddWithValue("obs", dto.Observacoes ?? (object)DBNull.Value);

    cmd.ExecuteNonQuery();
    return Ok(new { mensagem = "Consulta agendada com sucesso!" });
}

        [HttpPut("{id}/cancelar")]
        public IActionResult Cancelar(int id)
        {
            using var conn = ConexaoBanco.ObterConexao();
            using var cmd = new NpgsqlCommand(
                "UPDATE consulta SET status = 'cancelada' WHERE id_consulta = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
            return Ok(new { mensagem = "Consulta cancelada!" });
        }
    }

    public class ConsultaDto
    {
        public int IdPaciente { get; set; }
        public int IdProfissional { get; set; }
        public DateTime DataHora { get; set; }
        public string? Observacoes { get; set; }
    }
}