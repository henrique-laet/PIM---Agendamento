using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitaCare.Api.Dtos;
using VitaCare.Api.Models;
using VitaCare.Api.Repositories;
using VitaCare.Api.Services;

namespace VitaCare.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UsuarioRepository _usuarios;
    private readonly TokenService _tokens;

    public AuthController(UsuarioRepository usuarios, TokenService tokens)
    {
        _usuarios = usuarios;
        _tokens = tokens;
    }

    // POST /api/auth/cadastro
    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastro(CadastroRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var cpf = new string(req.Cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return BadRequest(new { erro = "O CPF deve ter 11 dígitos." });

        if (await _usuarios.EmailExisteAsync(email))
            return Conflict(new { erro = "Este e-mail já está cadastrado." });

        if (await _usuarios.CpfExisteAsync(cpf))
            return Conflict(new { erro = "Este CPF já está cadastrado." });

        var usuario = new Usuario
        {
            Nome = req.Nome.Trim(),
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha),
            Perfil = "paciente"
        };

        usuario.Id = await _usuarios.CriarPacienteAsync(
            usuario, cpf, req.Telefone, req.DataNascimento);

        return StatusCode(201, MontarResposta(usuario));
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var usuario = await _usuarios.ObterPorEmailAsync(req.Email.Trim());

        // A mesma mensagem para e-mail errado e senha errada,
        // para não revelar quais e-mails existem no sistema.
        if (usuario is null || !usuario.Ativo ||
            !BCrypt.Net.BCrypt.Verify(req.Senha, usuario.SenhaHash))
        {
            return Unauthorized(new { erro = "E-mail ou senha inválidos." });
        }

        return Ok(MontarResposta(usuario));
    }

    // GET /api/auth/me -> só funciona com token válido
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirst("sub")?.Value,
            nome = User.FindFirst("name")?.Value,
            email = User.FindFirst("email")?.Value,
            perfil = User.FindFirst("role")?.Value
        });
    }

    private AuthResponse MontarResposta(Usuario usuario)
    {
        var (token, expiraEm) = _tokens.Gerar(usuario);
        return new AuthResponse
        {
            Token = token,
            ExpiraEm = expiraEm,
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil
        };
    }
}