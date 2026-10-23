using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AgendamentoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var medicos = new List<object>();

            using var conn = ConexaoBanco.ObterConexao();
            using var cmd = new NpgsqlCommand(
                "SELECT id_profissional, nome, registro_conselho, especialidade, email, telefone FROM profissional", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                medicos.Add(new {
                    id = reader["id_profissional"],
                    nome = reader["nome"],
                    crm = reader["registro_conselho"],
                    especialidade = reader["especialidade"],
                    email = reader["email"] == DBNull.Value ? null : reader["email"],
                    telefone = reader["telefone"] == DBNull.Value ? null : reader["telefone"]
                });
            }

            return Ok(medicos);
        }
    }
}