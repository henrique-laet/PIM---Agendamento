using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VitaCare.Api.Models;

namespace VitaCare.Api.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime ExpiraEm) Gerar(Usuario usuario)
    {
        var chave = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");
        var minutos = int.Parse(_config["Jwt:ExpiraEmMinutos"] ?? "120");
        var expira = DateTime.UtcNow.AddMinutes(minutos);

        var descritor = new SecurityTokenDescriptor
        {
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            Expires = expira,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = usuario.Id.ToString(),
                ["email"] = usuario.Email,
                ["name"] = usuario.Nome,
                ["role"] = usuario.Perfil
            },
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
                SecurityAlgorithms.HmacSha256)
        };

        return (new JsonWebTokenHandler().CreateToken(descritor), expira);
    }
}