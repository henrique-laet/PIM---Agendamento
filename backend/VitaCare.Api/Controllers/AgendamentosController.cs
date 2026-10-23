using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitaCare.Api.Dtos;
using VitaCare.Api.Repositories;

namespace VitaCare.Api.Controllers;

[ApiController]
[Route("api/agendamentos")]
[Authorize(Roles = "paciente")]
public class AgendamentosController : ControllerBase
{
    private readonly AgendamentoRepository _agendamentos;

    public AgendamentosController(AgendamentoRepository agendamentos)
    {
        _agendamentos = agendamentos;
    }

    private int UsuarioId => int.Parse(User.FindFirst("sub")!.Value);

    // POST /api/agendamentos  { "horarioId": 12 }
    [HttpPost]
    public async Task<IActionResult> Agendar(AgendarRequest req)
    {
        var pacienteId = await _agendamentos.ObterPacienteIdAsync(UsuarioId);
        if (pacienteId is null)
            return BadRequest(new { erro = "Cadastro de paciente não encontrado." });

        var resultado = await _agendamentos.AgendarAsync(pacienteId.Value, req.HorarioId);
        if (!resultado.Ok)
            return Conflict(new { erro = resultado.Erro });

        return StatusCode(201, new { id = resultado.Id });
    }

    // GET /api/agendamentos/meus
    [HttpGet("meus")]
    public async Task<IActionResult> Meus()
    {
        var pacienteId = await _agendamentos.ObterPacienteIdAsync(UsuarioId);
        if (pacienteId is null) return Ok(Array.Empty<AgendamentoDto>());

        return Ok(await _agendamentos.ListarDoPacienteAsync(pacienteId.Value));
    }

    // DELETE /api/agendamentos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var pacienteId = await _agendamentos.ObterPacienteIdAsync(UsuarioId);
        if (pacienteId is null) return NotFound();

        var cancelado = await _agendamentos.CancelarAsync(id, pacienteId.Value);
        return cancelado
            ? NoContent()
            : NotFound(new { erro = "Agendamento não encontrado ou já cancelado." });
    }
}