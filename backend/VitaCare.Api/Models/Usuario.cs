namespace VitaCare.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public string Perfil { get; set; } = "paciente";
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
}