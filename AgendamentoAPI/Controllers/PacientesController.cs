using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AgendamentoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var pacientes = new List<object>();

            using var conn = ConexaoBanco.ObterConexao();
            using var cmd = new NpgsqlCommand(
                "SELECT id_paciente, nome, cpf, telefone, convenio, data_nascimento FROM paciente", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pacientes.Add(new {
                    id = reader["id_paciente"],
                    nome = reader["nome"],
                    cpf = reader["cpf"],
                    telefone = reader["telefone"] == DBNull.Value ? null : reader["telefone"],
                    convenio = reader["convenio"] == DBNull.Value ? null : reader["convenio"],
                    data_nascimento = reader["data_nascimento"] == DBNull.Value ? null : reader["data_nascimento"]
                });
            }

            return Ok(pacientes);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PacienteDto dto)
        {
            using var conn = ConexaoBanco.ObterConexao();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO paciente (nome, cpf, telefone, convenio, data_nascimento)
                  VALUES (@nome, @cpf, @telefone, @convenio, @nascimento)", conn);

            cmd.Parameters.AddWithValue("nome", dto.Nome);
            cmd.Parameters.AddWithValue("cpf", dto.Cpf.Replace(".", "").Replace("-", ""));
            cmd.Parameters.AddWithValue("telefone", dto.Telefone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("convenio", dto.Convenio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("nascimento", dto.DataNascimento.HasValue ? dto.DataNascimento.Value : DBNull.Value);

            cmd.ExecuteNonQuery();

            return Ok(new { mensagem = "Paciente cadastrado com sucesso!" });
        }
    }

    public class PacienteDto
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Convenio { get; set; }
        public DateTime? DataNascimento { get; set; }
    }
}