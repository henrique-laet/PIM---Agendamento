using Dapper;
using Microsoft.AspNetCore.Mvc;
using VitaCare.Api.Data;

namespace VitaCare.Api.Controllers;

[ApiController]
[Route("api/saude")]
public class SaudeController : ControllerBase
{
    private readonly DbConnectionFactory _db;

    public SaudeController(DbConnectionFactory db)
    {
        _db = db;
    }

    // GET /api/saude -> confirma que a API e o banco estão funcionando
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var conexao = _db.Criar();
        var medicos = await conexao.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM medicos");
        return Ok(new { status = "ok", banco = "conectado", medicos });
    }
}