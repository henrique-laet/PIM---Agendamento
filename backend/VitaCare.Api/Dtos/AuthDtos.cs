using System.ComponentModel.DataAnnotations;

namespace VitaCare.Api.Dtos;

public class CadastroRequest
{
    [Required, StringLength(120, MinimumLength = 3)]
    public string Nome { get; set; } = "";

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; set; } = "";

    // O limite de 72 existe porque o BCrypt só considera até 72 bytes
    [Required, StringLength(72, MinimumLength = 8)]
    public string Senha { get; set; } = "";

    [Required]
    public string Cpf { get; set; } = "";

    [StringLength(20)]
    public string? Telefone { get; set; }

    public DateTime? DataNascimento { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Senha { get; set; } = "";
}

public class AuthResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiraEm { get; set; }
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Perfil { get; set; } = "";
}