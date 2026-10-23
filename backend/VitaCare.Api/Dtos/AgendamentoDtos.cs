using System.ComponentModel.DataAnnotations;

namespace VitaCare.Api.Dtos;

public class EspecialidadeDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string? Icone { get; set; }
}

public class MedicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Crm { get; set; } = "";
    public int EspecialidadeId { get; set; }
    public string EspecialidadeNome { get; set; } = "";
    public string? FotoUrl { get; set; }
    public string? Bio { get; set; }
}

public class HorarioDto
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
}

public class AgendarRequest
{
    [Required, Range(1, int.MaxValue)]
    public int HorarioId { get; set; }
}

public class AgendamentoDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime CriadoEm { get; set; }
    public int HorarioId { get; set; }
    public DateTime DataHora { get; set; }
    public int MedicoId { get; set; }
    public string MedicoNome { get; set; } = "";
    public string Especialidade { get; set; } = "";
}