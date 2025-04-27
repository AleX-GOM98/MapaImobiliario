using DotNetEnv;
using MapaImobiliario.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

// Adicionar serviços ao contêiner
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL")));
builder.Services.AddControllers();
builder.Services.AddScoped<ScraperService>();

// Configurar o logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configuração do pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();  // Defina as rotas antes de configurar os endpoints

// Se quiser redirecionar para HTTPS, descomente a linha abaixo
// app.UseHttpsRedirection();

app.MapControllers();  // Configura as rotas dos controladores (incluindo sua API)

app.Run();
