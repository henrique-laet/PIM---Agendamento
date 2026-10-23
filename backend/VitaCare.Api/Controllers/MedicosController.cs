using Microsoft.AspNetCore.Mvc;
using VitaCare.Api.Repositories;

namespace VitaCare.Api.Controllers;

[ApiController]
[Route("api/medicos")]
public class MedicosController : ControllerBase
{
    private readonly CatalogoRepository _catalogo;

    public MedicosController(CatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    // GET /api/medicos?especialidadeId=2
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? especialidadeId) =>
        Ok(await _catalogo.ListarMedicosAsync(especialidadeId));

    // GET /api/medicos/3
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var medico = await _catalogo.ObterMedicoAsync(id);
        return medico is null ? NotFound(new { erro = "Médico não encontrado." }) : Ok(medico);
    }

    // GET /api/medicos/3/horarios?dia=2026-10-27
    [HttpGet("{id:int}/horarios")]
    public async Task<IActionResult> Horarios(int id, [FromQuery] DateTime? dia)
    {
        if (await _catalogo.ObterMedicoAsync(id) is null)
            return NotFound(new { erro = "Médico não encontrado." });

        return Ok(await _catalogo.ListarHorariosLivresAsync(id, dia));
    }
}