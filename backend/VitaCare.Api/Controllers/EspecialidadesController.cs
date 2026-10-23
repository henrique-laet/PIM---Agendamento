using Microsoft.AspNetCore.Mvc;
using VitaCare.Api.Repositories;

namespace VitaCare.Api.Controllers;

[ApiController]
[Route("api/especialidades")]
public class EspecialidadesController : ControllerBase
{
    private readonly CatalogoRepository _catalogo;

    public EspecialidadesController(CatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    // GET /api/especialidades
    [HttpGet]
    public async Task<IActionResult> Listar() =>
        Ok(await _catalogo.ListarEspecialidadesAsync());
}