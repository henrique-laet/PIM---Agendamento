using VitaCare.Api.Data;

// Faz o Dapper entender colunas como "senha_hash" em propriedades como "SenhaHash"
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();