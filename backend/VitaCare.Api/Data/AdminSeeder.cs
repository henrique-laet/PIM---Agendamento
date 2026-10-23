using VitaCare.Api.Repositories;

namespace VitaCare.Api.Data;

public static class AdminSeeder
{
    public static async Task GarantirAdminAsync(IServiceProvider servicos, IConfiguration config)
    {
        var email = config["AdminInicial:Email"];
        var senha = config["AdminInicial:Senha"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return;

        using var escopo = servicos.CreateScope();
        var repo = escopo.ServiceProvider.GetRequiredService<UsuarioRepository>();

        if (await repo.ExisteAdminAsync())
            return;

        var nome = config["AdminInicial:Nome"] ?? "Administrador";
        await repo.CriarAdminAsync(
            nome, email.Trim().ToLowerInvariant(), BCrypt.Net.BCrypt.HashPassword(senha));

        Console.WriteLine($"Admin inicial criado: {email}");
    }
}